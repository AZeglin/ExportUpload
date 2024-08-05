using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class ExcelProcessManager
    {
       
        protected Excel.Application _excelApplication = null;

        public Excel.Application CurrentExcelApplicationInstance
        {
            get { return _excelApplication; }
            set { _excelApplication = value; }
        }

        protected Excel.Workbook _wb = null;

        public Excel.Workbook CurrentOpenWorkbook
        {
            get { return _wb; }
            set { _wb = value; }
        }

        protected ExcelProcessManagerDatabase _db = null;
        protected string _applicationServerName = "";

        protected Guid _excelWindowCaption = Guid.Empty;
        protected ArrayList _excelPidsBeforeLaunch = new ArrayList();
        protected int _currentExcelPID = 0;

        protected string _taskType = "";

        public string TaskType
        {
            get { return _taskType; }
            set { _taskType = value; }
        }

        protected string _excelProcessManagerErrorMessage = "";

        public string ExcelProcessManagerErrorMessage
        {
            get { return _excelProcessManagerErrorMessage; }
            set { _excelProcessManagerErrorMessage = value; }
        }

        public ExcelProcessManager( UserInfo userInfo, ItemExportUploadGlobals globals, string applicationServerName )
        {
            _db = new ExcelProcessManagerDatabase( userInfo, globals );
            _applicationServerName = applicationServerName;
        }

        protected bool InitExcelApplicationInstance( string taskType )
        {
            if( _currentExcelPID != 0 )
            {
                _excelProcessManagerErrorMessage = "Previous instance of excel was not queued for deletion.";
                return ( false );
            }

            // convert to 2 char task type
            if( taskType.CompareTo( "E" ) == 0 )
            {
                _taskType = "IE";  // item export
            }
            else
            {
                _taskType = "IU";  // item upload
            }

            lock( this )
            {
                CaptureExcelInstances();

                _excelApplication = new Microsoft.Office.Interop.Excel.Application();

                _currentExcelPID = GetCurrentPID();
            }

            if( _excelApplication == null )
            {
                _excelProcessManagerErrorMessage = "Unable to create Excel application.";
                return ( false );
            }

            _excelWindowCaption = Guid.NewGuid();
            _excelApplication.Caption = _excelWindowCaption.ToString().ToUpper();

            return ( true );
         }

        protected void CaptureExcelInstances()
        {   
            _excelPidsBeforeLaunch.Clear();

            foreach( Process proc in System.Diagnostics.Process.GetProcessesByName( "EXCEL" ) )
            {
                _excelPidsBeforeLaunch.Add( proc.Id );
            }
        }

        protected int GetCurrentPID()
        {
            int aNewPid = 0;
            ArrayList currentPIDs = new ArrayList();
            ArrayList newPIDs = new ArrayList();

            foreach( Process proc in System.Diagnostics.Process.GetProcessesByName( "EXCEL" ) )
            {
                currentPIDs.Add( proc.Id );
            }

            for( int i = 0; i < currentPIDs.Count; i++ )
            {
                int currentPid = ( int )currentPIDs[ i ];
                bool bFound = false;
                for( int j = 0; j < _excelPidsBeforeLaunch.Count; j++ )
                {
                    int oldPid = ( int )_excelPidsBeforeLaunch[ j ];

                    if( currentPid == oldPid )
                    {
                        bFound = true;
                        break;
                    }
                }
                // its new
                if( bFound == false )
                {
                    newPIDs.Add( currentPid );
                    break; // for now just take the first one found
                }
            }
 
            if( newPIDs.Count > 0 )
            {
                aNewPid = ( int )newPIDs[ 0 ];
            }

            return ( aNewPid );
        }

        public bool SaveWorkbook()
        {
            bool bSuccess = true;
            try
            {
                if( _wb != null )
                {
                    _wb.Save();
                }
            }
            catch( Exception ex )
            {
                bSuccess = false;
                _excelProcessManagerErrorMessage = ex.Message;
            }
            return ( bSuccess );
        }

        public bool CloseCurrentWorkbookAndExcelInstance()
        {
            int processInstanceId = 0;
            int parentHwnd = -1;
            bool bSuccess = true;

            if ( _currentExcelPID != 0 )
            {
                if ( _excelApplication != null )
                {
                    parentHwnd = _excelApplication.Parent.Hwnd;
                    bSuccess = _db.QueueProcessForDelete( _currentExcelPID, parentHwnd, _excelWindowCaption, "EXCEL", _taskType, _applicationServerName, out processInstanceId );
                    if ( bSuccess == false )
                    {
                        _excelProcessManagerErrorMessage = _db.ErrorMessage;
                    }
                }
            }

            //if( _wb != null )
            //{
            //    _wb.Close( true, Type.Missing, Type.Missing );
            //}
            _wb = null;

            //if( _excelApplication != null )
            //{
            //    _excelApplication.Quit();
            //}     
            _excelApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
 
            _currentExcelPID = 0;
            _excelWindowCaption = Guid.Empty;

            return ( bSuccess );
        }
    }
}
