using System;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    public partial class ItemExportUploadControl : System.Web.UI.UserControl  // , ICallbackEventHandler
    {
        protected ExportUploadInstanceType _exportUploadInstanceType = null;

        private ExportUploadDatabase _exportUploadDB = null;

        private UserInfo _userInfo = null;

        private string _contractNumber = "";
        private int _contractId = -1;

        private int _scheduleNumber = 0;
        private bool _bIsBPA = false;

        private string _modificationNumber = "";
        private string _exportUploadTypeString = "";
        private string _exportUploadPermissionsString = "";
        private string _applicationVersion = "";
       
        // control property
        public string ContractNumber
        {
            get { return _contractNumber; }
            set { _contractNumber = value; }
        }

        // control property
        public int ContractId
        {
            get { return _contractId; }
            set { _contractId = value; }
        }

        // control property
        public int ScheduleNumber
        {
            get { return _scheduleNumber; }
            set { _scheduleNumber = value; }
        }

        // control property
        public bool IsBPA
        {
            get { return _bIsBPA; }
            set { _bIsBPA = value; }
        }

        // control property
        public string ExportUploadTypeString
        {
            get { return _exportUploadTypeString; }
            set { _exportUploadTypeString = value; }
        }

        // control property
        public string ExportUploadPermissionsString
        {
            get { return _exportUploadPermissionsString; }
            set { _exportUploadPermissionsString = value; }
        }

        // control property
        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set { _applicationVersion = value; }
        }

        protected void Page_Load( object sender, EventArgs e )
        {           
            if( Session[ "ExportUploadInstanceType" ] != null )
            {
                _exportUploadInstanceType = ( ExportUploadInstanceType )Session[ "ExportUploadInstanceType" ];
            }
            else
            {
                throw new Exception( "Expected session variable (et) was null on postback. Session has timed out. Close application and retry." );
            }

            Page.Form.Attributes.Add( "enctype", "multipart/form-data" );

            if( Page.IsPostBack == false )
            {
                ClearSessionVariables();

                // members set by caller
                Session[ "ExportUploadContractNumber" ] = _contractNumber;
                Session[ "ExportUploadContractId" ] = _contractId;
                Session[ "ExportUploadScheduleNumber" ] = _scheduleNumber;
                Session[ "ExportUploadIsBPA" ] = _bIsBPA;
                Session[ "ApplicationVersion" ] = _applicationVersion;

              //  _globals.ExportUploadType = _globals.GetExportUploadTypeFromString( _exportUploadTypeString );            
                _exportUploadInstanceType.ExportUploadType = ExportUploadGlobals.GetExportUploadTypeFromString( _exportUploadTypeString );
            }
            else
            {
                if( Session[ "ExportUploadContractNumber" ] != null )
                {
                    _contractNumber = ( string )Session[ "ExportUploadContractNumber" ];
                }
                else
                {
                    throw new Exception( "Expected session variable (cn) was null on postback. Session has timed out. Close application and retry." );
                }

                int tempContractId = -1;
                string tempContractIdStr = "-1";
                if( Session[ "ExportUploadContractId" ] != null )
                {
                    tempContractIdStr = Session[ "ExportUploadContractId" ].ToString();
                    if( int.TryParse( tempContractIdStr, out tempContractId ) == true )
                    {
                        _contractId = tempContractId;
                    }
                }
                else
                {
                    throw new Exception( "Expected session variable (ci) was null on postback. Session has timed out. Close application and retry." );
                }

                int tempScheduleNumber = 0;
                string tempScheduleNumberStr = "0";

                if( Session[ "ExportUploadScheduleNumber" ] != null )
                {
                    tempScheduleNumberStr = Session[ "ExportUploadScheduleNumber" ].ToString();
                    if( int.TryParse( tempScheduleNumberStr, out tempScheduleNumber ) == true )
                    {
                        _scheduleNumber = tempScheduleNumber;
                    }
                }
                else
                {
                    throw new Exception( "Expected session variable (sn) was null on postback. Session has timed out. Close application and retry." );
                }

                bool tempBIsBPA = false;
                string tempBIsBPAStr = "false";

                if( Session[ "ExportUploadIsBPA" ] != null )
                {
                    tempBIsBPAStr = Session[ "ExportUploadIsBPA" ].ToString();
                    if( bool.TryParse( tempBIsBPAStr, out tempBIsBPA ) == true )
                    {
                        _bIsBPA = tempBIsBPA;
                    }
                }
                else
                {
                    throw new Exception( "Expected session variable (bp) was null on postback. Session has timed out. Close application and retry." );
                }

                if( Session[ "ApplicationVersion" ] != null )
                {
                    _applicationVersion = ( string )Session[ "ApplicationVersion" ];
                }
                else
                {
                    throw new Exception( "Expected session variable (ver) was null on postback. Session has timed out. Close application and retry." );
                }

            }

            DisplayStatusMessage( "" );

            if( Session[ "ItemExportUploadUserInfo" ] != null )
            {
                _userInfo = ( UserInfo )Session[ "ItemExportUploadUserInfo" ];
            }
            else
            {
                _userInfo = new UserInfo(); // gets login name of current user
                Session[ "ItemExportUploadUserInfo" ] = _userInfo;
            }


            if( Session[ "ExportUploadDB" ] != null )
            {
                _exportUploadDB = ( ExportUploadDatabase )Session[ "ExportUploadDB" ];
            }
            else
            {
                _exportUploadDB = new ExportUploadDatabase( ref _userInfo );
                Session[ "ExportUploadDB" ] = _exportUploadDB;
            }

            if( Page.IsPostBack != true )
            {
                // load user info from database 
                LoadUserInfo();
            }

            // enable controls based on current context
            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
            {
                InitExportUploadFields( _userInfo.IsMedSurgExportAuthorized, _userInfo.IsMedSurgUploadAuthorized );
            }
            else if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
            {
                InitExportUploadFields( _userInfo.IsPharmExportAuthorized, _userInfo.IsPharmUploadAuthorized );
            }

            // hidden fields used by status message loop
            ClientScriptManager clientScriptManager = Page.ClientScript;
            clientScriptManager.RegisterHiddenField( "ContainingControlId", this.ClientID.ToString() );
            clientScriptManager.RegisterHiddenField( "SocketHandlerUrl", ItemExportUploadConfiguration.SocketHandlerUrl );

            //string hideProgressBarScript = string.Format( "EnableProgressIndicator(false);"  );
            //clientScriptManager.RegisterStartupScript( this.GetType(), "HideProgressBarScript", hideProgressBarScript, true );

            if( Page.IsPostBack != true )
            {
                ExportToSpreadsheetButton.Attributes.Add( "onclick", "OnExport();" );
            UploadButton.Attributes.Add( "onclick", "OnUpload();" );
            }  


            // check if upload proceed anyway 
            bool bPreviousUploadProceedAnyway = false;
            HiddenField PreviousUploadProceedAnywayHiddenField = ( HiddenField )FindControl( "PreviousUploadProceedAnyway" );

            if( PreviousUploadProceedAnywayHiddenField != null )
            {
                if( bool.TryParse( PreviousUploadProceedAnywayHiddenField.Value, out bPreviousUploadProceedAnyway ) == true )
                {
                    if( bPreviousUploadProceedAnyway == true )
                    {
                        PreviousUploadProceedAnywayHiddenField.Value = "false";
                        CompleteInProgressUpload();
                    }
                }
            }
        }

        private void ClearSessionVariables()
        {
            // init error file name used by client pickup
            SetUploadErrorFileNameHiddenField( "" );

            Session[ "ExportUploadDB" ] = null;
            Session[ "ExportUploadContractNumber" ] = null;
            Session[ "ExportUploadContractId" ] = null;
            Session[ "ExportUploadScheduleNumber" ] = null;
            Session[ "ExportUploadIsBPA" ] = null;
            Session[ "FilePathAndNameOfUploadInProgress" ] = null;
            Session[ "OriginalFileNameOfUploadInProgress" ] = null;
            Session[ "ModificationNumberOfUploadInProgress" ] = null;
            Session[ "ApplicationVersion" ] = null;
        }
    
        // load user info from database and extract permissions from the query string
        private void LoadUserInfo()
        {
            bool bSuccess = true;

            Guid userGuid = Guid.Empty;
            string firstName = "";
            string lastName = "";
            string fullName = "";
            string email = "";
            string phone = "";
            string status = "";
            int oldUserId = -1;
            int userDivision = -1;

            bSuccess = _exportUploadDB.GetUserInfo( _userInfo.LoginName, ref userGuid, ref firstName, ref lastName, ref fullName, ref email, ref phone, ref status, ref oldUserId, ref userDivision );

            if( bSuccess == true )
            {
                if( status.CompareTo( "ACTIVE" ) == 0 )
                {
                    _userInfo.UserId = userGuid;
                    _userInfo.FirstName = firstName;
                    _userInfo.LastName = lastName;
                    _userInfo.FullName = fullName;
                    _userInfo.Email = email;
                    _userInfo.Phone = phone;
                    _userInfo.OldUserId = oldUserId;
                    _userInfo.Division = userDivision;

                    _exportUploadDB.CurrentUserId = _userInfo.UserId; // authorize requires guid

            //        bool bIsAuthorized = false;
                
                    // authorize for export
            //        bSuccess = _exportUploadDB.AuthorizeUser( ScheduleNumber, _globals.ExportUploadType, ExportUploadGlobals.ActionTypes.Export, ref bIsAuthorized );
            //        if( bSuccess == true )
            //        {
            //            _userInfo.IsAuthorizedForExport = bIsAuthorized;

            //            // authorize for upload
            //            bSuccess = _exportUploadDB.AuthorizeUser( ScheduleNumber, _globals.ExportUploadType, ExportUploadGlobals.ActionTypes.Upload, ref bIsAuthorized );
            //            if( bSuccess == true )
            //            {
            //                _userInfo.IsAuthorizedForUpload = bIsAuthorized;
            //            }
            //            else
            //            {
            //                DisplayStatusMessage( string.Format( "Error encountered when authorizing user for upload: {0}", _exportUploadDB.ErrorMessage ) );
            //            }
            //        }
            //        else
            //        {
            //            DisplayStatusMessage( string.Format( "Error encountered when authorizing user for export: {0}", _exportUploadDB.ErrorMessage ));
            //        }
                
                }
                else
                {
                    DisplayStatusMessage( "User exists but is not active." );
                }
            }
            else
            {
                DisplayStatusMessage( _exportUploadDB.ErrorMessage );              
            }
        }

    

        private void InitExportUploadFields( bool bIsAuthorizedForExport, bool bIsAuthorizedForUpload )
        {
            ContractNumberLabel.Text = _contractNumber;

            Label StartDateLabel = ( Label )FindControl( "StartDateLabel" );
            TextBox StartDateTextBox = ( TextBox )FindControl( "StartDateTextBox" );
            Label EndDateLabel = ( Label )FindControl( "EndDateLabel" );
            TextBox EndDateTextBox = ( TextBox )FindControl( "EndDateTextBox" );

            RadioButton ExportCoveredOnlyRadioButton = ( RadioButton )FindControl( "ExportCoveredOnly" );
            RadioButton ExportBothRadioButton = ( RadioButton )FindControl( "ExportBoth" );

            //Button IsAuthorizedForExport = ( Button )FindControl( "ExportToSpreadsheet" );
            //ExportToSpreadsheetButton.Enabled = bIsAuthorizedForExport;

            TextBox ModificationNumberTextBox = ( TextBox )FindControl( "ModificationNumberTextBox" );
            ModificationNumberTextBox.Enabled = bIsAuthorizedForUpload;

            FileUpload FileUploadControl = ( FileUpload )FindControl( "FileUpload1" );
            FileUploadControl.Enabled = bIsAuthorizedForUpload;          

            Button UploadButton = ( Button )FindControl( "UploadButton" );
            UploadButton.Enabled = bIsAuthorizedForUpload;
            
 
            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
            {
                StartDateLabel.Visible = GetStartDateEnabledValue();
                StartDateTextBox.Visible = GetStartDateEnabledValue();
                StartDateTextBox.Enabled = bIsAuthorizedForExport;

                EndDateLabel.Visible = GetEndDateEnabledValue();
                EndDateTextBox.Visible = GetEndDateEnabledValue();
                EndDateTextBox.Enabled = bIsAuthorizedForExport;

                ExportCoveredOnlyRadioButton.Visible = true;
                ExportCoveredOnlyRadioButton.Enabled = bIsAuthorizedForExport;
                ExportBothRadioButton.Visible = true;
                ExportBothRadioButton.Enabled = bIsAuthorizedForExport;

                if( Page.IsPostBack != true )
                {
                    StartDateTextBox.Text = GetDefaultStartDate();
                    EndDateTextBox.Text = GetDefaultEndDate();
                    ExportCoveredOnlyRadioButton.Checked = false;
                    ExportBothRadioButton.Checked = false;
                }
            }
            else
            {
                StartDateLabel.Visible = false;
                StartDateTextBox.Visible = false;
                EndDateLabel.Visible = false;
                EndDateTextBox.Visible = false;

                ExportCoveredOnlyRadioButton.Visible = false;
                ExportBothRadioButton.Visible = false;
            }
        }
      
        private string GetDefaultStartDate()
        {
            DateTime defaultStartDate;
            if( DateTime.Today.Month == 10 || DateTime.Today.Month == 11 || DateTime.Today.Month == 12 )
            {
                defaultStartDate = new DateTime( DateTime.Today.Year + 1, 1, 1 );
            }
            else
            {
                defaultStartDate = DateTime.Today;
            }
            return( defaultStartDate.ToString( "d" ) );
        }

        private string GetDefaultEndDate()
        {
            DateTime defaultEndDate;
            if( DateTime.Today.Month == 10 || DateTime.Today.Month == 11 || DateTime.Today.Month == 12 )
            {
                defaultEndDate = new DateTime( DateTime.Today.Year + 1, 12, 31 );
            }
            else
            {
                defaultEndDate =  new DateTime( DateTime.Today.Year, 12, 31 );
            }
            return( defaultEndDate.ToString( "d" ) );
        }

        private bool GetStartDateEnabledValue()
        {
            bool bStartDateEnabled = false;
            if( DateTime.Today.Month == 10 || DateTime.Today.Month == 11 || DateTime.Today.Month == 12 )
            {
                bStartDateEnabled = false;
            }
            else
            {
                bStartDateEnabled = true;
            }

            return( bStartDateEnabled );
        }

       private bool GetEndDateEnabledValue()
        {
            bool bEndDateEnabled = false;
            if( DateTime.Today.Month == 10 || DateTime.Today.Month == 11 || DateTime.Today.Month == 12 )
            {
                bEndDateEnabled = false;
            }
            else
            {
                bEndDateEnabled = true;
            }

            return( bEndDateEnabled );
        }

       private void DisplayStatusMessage( string statusMessage )
       {
           Label UploadStatusLabel = ( Label )FindControl( "UploadStatusLabel2" );
           UploadStatusLabel.Text = statusMessage;
       }

       private void SetUploadErrorFileNameHiddenField( string errorFileName )
       {
           HiddenField UploadErrorFileName = ( HiddenField )FindControl( "UploadErrorFileNameId" );
           if( UploadErrorFileName != null )
           {
               UploadErrorFileName.Value = errorFileName;
           }
       }

       private void SetExportSecurityBitHiddenField( bool bIsAuthorizedForExport )
       {
           HiddenField IsAuthorizedForExportHiddenField = ( HiddenField )FindControl( "IsAuthorizedForExport" );
           if( IsAuthorizedForExportHiddenField != null )
           {
               IsAuthorizedForExportHiddenField.Value = bIsAuthorizedForExport.ToString();
           }
       }

       private void SetProgressBarColor( string barClassName )
       {
           TableCell progressBarCell = ( TableCell )FindControl( "ProgressBar" );
           if( progressBarCell != null )
           {
               progressBarCell.Attributes[ "class" ] = barClassName;
           }                   
       }

       public string SerializeParmsForClient( string[] parms )
       {
           StringBuilder sb = new StringBuilder();
           for( int i = 0; i < parms.Length; i++ )
           {
               sb.Append( parms[ i ] );
               sb.Append( "|" );
           }

           return ( sb.ToString() );
       }

       private void CompleteInProgressUpload()
       {
           Upload();
       }


      

       protected void UploadButton_OnClick( object sender, EventArgs e )
       {
            _exportUploadInstanceType.ActionType = ExportUploadGlobals.ActionTypes.Upload;

            if( GatherUploadParms() == true )
            {
                if( FileUpload1.HasFile == true )
                {
                    string archiveDirectory = "";
                    if( ExportUploadGlobals.GetExportUploadTypeFromString( _exportUploadTypeString ) == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                    {
                        archiveDirectory = ItemExportUploadConfiguration.MedSurgUploadArchiveDirectoryPath;
                    }
                    else if( ExportUploadGlobals.GetExportUploadTypeFromString( _exportUploadTypeString )  == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
                    {
                        archiveDirectory = ItemExportUploadConfiguration.PharmaceuticalUploadArchiveDirectoryPath;
                    }
                    else
                    {
                        DisplayStatusMessage( "Unknown upload type." );
                        return;
                    }

                    string fileNameWithExtension = FileUpload1.FileName;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( FileUpload1.FileName );
                    string fileNameExtension = Path.GetExtension( FileUpload1.FileName );
                    string cleansedUserName = _userInfo.LoginName.Replace( '\\', '_' );
                    string timestamp = string.Format( "{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second );
                    string filePathAndName = String.Format( "{0}{1}{2}_{3}_{4}{5}", archiveDirectory, ( archiveDirectory.EndsWith( @"\" ) ) ? "" : @"\", fileNameWithoutExtension, cleansedUserName, timestamp, fileNameExtension );

                    try
                    {
                        FileUpload1.SaveAs( filePathAndName );  // this is both the archive and the staging area - new: to be picked up by the service
                    }
                    catch( Exception ex )
                    {
                        DisplayStatusMessage( ex.Message );
                        return;
                    }

                    //if( _globals.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                    //{
                    //    _medSurgItemSpreadsheet.Init();
                    //    _medSurgItemSpreadsheet.FileToUpload = filePathAndName;
                    //    _medSurgItemSpreadsheet.ContractNumber = _contractNumber;
                    //    _medSurgItemSpreadsheet.ModificationNumber = _modificationNumber;
                    //}
                    //else // Pharmaceutical 
                    //{
                    //    _pharmItemSpreadsheet.Init();
                    //    _pharmItemSpreadsheet.FileToUpload = filePathAndName;
                    //    _pharmItemSpreadsheet.ContractNumber = _contractNumber;
                    //    _pharmItemSpreadsheet.ModificationNumber = _modificationNumber;
                    //} 

                    // save for postback after "proceed" window
                    Session[ "FilePathAndNameOfUploadInProgress" ] = filePathAndName;  // modified with timestamp
                    Session[ "OriginalFileNameOfUploadInProgress" ] = fileNameWithExtension;
                    Session[ "ModificationNumberOfUploadInProgress" ] = _modificationNumber;

                    int previousUploadDays = 0;
                    int rowCount = 0;
                    DataSet dsModificationStatus = null;
                    
                    if( int.TryParse( ItemExportUploadConfiguration.CheckForPreviousUploadDays, out previousUploadDays ) == true )
                    {
                        DateTime uploadHistoryStartDate = DateTime.Now.AddDays( ( double )( previousUploadDays * -1 ) );
                       
                        if( _exportUploadDB.CheckForPreviousUpload( _contractNumber, uploadHistoryStartDate, DateTime.Now, ref dsModificationStatus, ref rowCount ) != true )
                        {
                            DisplayStatusMessage( string.Format( "Error encountered while checking for previous upload: {0}", _exportUploadDB.ErrorMessage ));
                            return;
                        }
                    }

                    if( rowCount > 0 )
                    {
                        DataTable previousUploadsTable = dsModificationStatus.Tables[ ExportUploadDatabase.ModificationStatusTableName ];

                        IEnumerable mostRecentSuccessfulUploadQuery = from u in previousUploadsTable.AsEnumerable()
                                                                      where u[ "ExportUploadStatus" ].ToString().CompareTo( "UC" ) == 0
                                                                      orderby u[ "CreationDate" ] descending
                                                                      select u;

                        string createdBy = "";
                        string creationDate = "";
                        int completedCount = 0;

                        foreach( var upload in mostRecentSuccessfulUploadQuery )
                        {
                            createdBy = ( ( DataRow )upload )[ "CreatedBy" ].ToString();
                            creationDate = ( ( DataRow )upload )[ "CreationDate" ].ToString();
                            completedCount++;
                            break;
                        }

                        // only raise the box if previous upload attempts have completed
                        if( completedCount > 0 )
                        {
                            string userName = "";
                            if( createdBy.Length > 0 )
                            {
                                string[] u = createdBy.Split( new char[] { '\\' } );
                                userName = u[ 1 ];
                            }

                            string message = "";
                            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                            {
                                message = string.Format( "An upload was recently completed for this contract by {0} on {1}.", userName, creationDate );
                            }
                            else
                            {
                                message = string.Format( "An upload was recently completed for this contract by {0} on {1}.", userName, creationDate );
                            }

                            Response.Write( string.Format( "<script>window.open( 'CheckForPreviousUpload.aspx?message={0}', 'CheckForPreviousUpload', 'toolbar=0,status=0,menubar=0,scrollbars=0,resizable=0,top=220,left=460,width=300,height=180, resizable=0' )</Script>", message ) );
                        }
                        else
                        {
                            Upload();
                        }
                    }
                    else
                    {
                        Upload();
                    }
               }
            }
        }
           

        private void Upload()
        {
            // left these 4 lines in during manual merge - revisit after build $$$
         //   int initialContractItemCount = 0;
         //   int initialContractPriceCount = 0;
         //   int finalContractItemCount = 0;
         //   int finalContractPriceCount = 0;

            bool bSuccess = true;
            string applicationServerName = "";
            string filePathOnly = "";
            string filePathAndName = ""; // path and name of upload in-progress
            string originalFileName = "";  // original file name of upload in-progress            
            string fileNameWithoutExtension = ""; // name of upload in-progress without extension
            string fileNameWithExtension = "";
            string fileExtension = "";
            string potentialErrorFilePathAndName = "";
            string destinationFileType = "";
            int currentActivityId = -1;
            int currentActivityDetailsId = -1;
            Guid guidForCurrentModification = Guid.Empty;
  
            int requestId = -1;
           
            DateTime creationDate = DateTime.Today;
            applicationServerName = ItemExportUploadConfiguration.ApplicationServerName;

            ExportUploadRequest statusRequest = new ExportUploadRequest();

            try
            {
                if( Session[ "FilePathAndNameOfUploadInProgress" ] != null )
                {
                    filePathAndName = ( string )Session[ "FilePathAndNameOfUploadInProgress" ];
                    filePathOnly = Path.GetDirectoryName( filePathAndName ) + "\\";
                    fileNameWithoutExtension = Path.GetFileNameWithoutExtension( filePathAndName );
                    fileExtension = Path.GetExtension( filePathAndName );
                    fileNameWithExtension = Path.GetFileName( filePathAndName );
                }

                if( Session[ "OriginalFileNameOfUploadInProgress" ] != null )
                {
                    originalFileName = ( string )Session[ "OriginalFileNameOfUploadInProgress" ];
                }

                if( Session[ "ModificationNumberOfUploadInProgress" ] != null )
                {
                    _modificationNumber = ( string )Session[ "ModificationNumberOfUploadInProgress" ];
                }

                // set up a status request message            
                statusRequest.RequestType = ExportUploadRequest.RequestTypes.Status;
                statusRequest.ContractNumber = _contractNumber;
                statusRequest.ScheduleNumber = _scheduleNumber;
                statusRequest.RequestingServerName = applicationServerName;
                statusRequest.CreatedBy = _userInfo.UserId;
                statusRequest.ApplicationVersion = _applicationVersion;


                if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                {
                    statusRequest.RequestSubType = ExportUploadRequest.RequestSubTypes.MedSurg;
                    statusRequest.StartDateCriteria = DateTime.MinValue;
                    statusRequest.EndDateCriteria = DateTime.MinValue;
                    statusRequest.EncodedCriteria += ( _bIsBPA == true ) ? "BPA;" : "";

                    // create the modification log item for the current run
                    if( _exportUploadDB.InitCurrentActivity( _exportUploadInstanceType.ActionType, _exportUploadInstanceType.ExportUploadType, _contractNumber, _modificationNumber, originalFileName, ref currentActivityId, ref currentActivityDetailsId, ref guidForCurrentModification ) == true )
                    {
                        statusRequest.GuidForCurrentModification = guidForCurrentModification;
                        _exportUploadInstanceType.CurrentActivityId = currentActivityId;
                        _exportUploadInstanceType.CurrentActivityDetailsId = currentActivityDetailsId;

                        // request an upload from the service
                        bSuccess = _exportUploadDB.UploadItems2( _contractNumber, _contractId, _scheduleNumber, _bIsBPA, ExportUploadGlobals.GetExportUploadStringFromType( _exportUploadInstanceType.ExportUploadType ), "XL", fileExtension, fileNameWithExtension, fileNameWithoutExtension, filePathOnly, filePathAndName, applicationServerName, currentActivityId, currentActivityDetailsId, guidForCurrentModification, _applicationVersion, ref potentialErrorFilePathAndName, ref destinationFileType, ref requestId, ref creationDate );
                    }

                    if( bSuccess == false )
                    {
                        // log the failure
                        _exportUploadDB.UpdateExportUploadActivityStatus( currentActivityId, ExportUploadGlobals.ExportUploadStatuses.RequestFailed );

                        throw new Exception( _exportUploadDB.ErrorMessage );
                    }
                    else
                    {
                        // log the success
                        _exportUploadDB.UpdateExportUploadActivityStatus( currentActivityId, ExportUploadGlobals.ExportUploadStatuses.UploadRequested );
                    }
                }
                else // pharm
                {     
                    statusRequest.RequestSubType = ExportUploadRequest.RequestSubTypes.Pharmaceutical;
                    statusRequest.StartDateCriteria = DateTime.MinValue;
                    statusRequest.EndDateCriteria = DateTime.MinValue;
                    statusRequest.EncodedCriteria += ( _bIsBPA == true ) ? "BPA;" : "";  // not relevant to pharm only added as a placeholder

                    // create the modification log item for the current run
                    if( _exportUploadDB.InitCurrentActivity( _exportUploadInstanceType.ActionType, _exportUploadInstanceType.ExportUploadType, _contractNumber, _modificationNumber, originalFileName, ref currentActivityId, ref currentActivityDetailsId, ref guidForCurrentModification ) == true )
                    {
                        statusRequest.GuidForCurrentModification = guidForCurrentModification;
                        _exportUploadInstanceType.CurrentActivityId = currentActivityId;
                        _exportUploadInstanceType.CurrentActivityDetailsId = currentActivityDetailsId;

                        // request an upload from the service
                        bSuccess = _exportUploadDB.UploadItems2( _contractNumber, _contractId, _scheduleNumber, _bIsBPA, ExportUploadGlobals.GetExportUploadStringFromType( _exportUploadInstanceType.ExportUploadType ), "XL", fileExtension, fileNameWithExtension, fileNameWithoutExtension, filePathOnly, filePathAndName, applicationServerName, currentActivityId, currentActivityDetailsId, guidForCurrentModification, _applicationVersion, ref potentialErrorFilePathAndName, ref destinationFileType, ref requestId, ref creationDate );
                    }

                    if( bSuccess == false )
                    {
                        // log the failure
                        _exportUploadDB.UpdateExportUploadActivityStatus( currentActivityId, ExportUploadGlobals.ExportUploadStatuses.RequestFailed );

                        throw new Exception( _exportUploadDB.ErrorMessage );
                    }
                    else
                    {
                        // log the success
                        _exportUploadDB.UpdateExportUploadActivityStatus( currentActivityId, ExportUploadGlobals.ExportUploadStatuses.UploadRequested );
                    }
                }
            }
            catch( Exception ex )
            {
                bSuccess = false;
                DisplayStatusMessage( ex.Message );
            }

            if( bSuccess == true )
            {

           //     DisplayStatusMessage( "Upload requested." );   let the status loop display the success

                // complete remaining statusRequest fields
                statusRequest.RequestId = requestId;
                statusRequest.CreationDate = creationDate;                
                statusRequest.DestFilePathName = potentialErrorFilePathAndName;
                statusRequest.DestFileType = destinationFileType;

                statusRequest.ActivityId = _exportUploadInstanceType.CurrentActivityId;
                statusRequest.ActivityDetailsId = _exportUploadInstanceType.CurrentActivityDetailsId;

                InitiateStatusLoop( statusRequest );            
            }
        }







        public bool GatherUploadParms()
        {
            bool bSuccess = true;

            if( ModificationNumberTextBox.Text.Trim().Length > 0 )
            {
                _modificationNumber = ModificationNumberTextBox.Text.Trim();
            }
            else
            {
                DisplayStatusMessage( "Modification number is required." );
                bSuccess = false;
            }

            return ( bSuccess );
        }
        protected void ExportToSpreadsheetButton_Click( object sender, EventArgs e )
        {

            _exportUploadInstanceType.ActionType = ExportUploadGlobals.ActionTypes.Export;
       
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;
            string startDateString = "";
            string endDateString = "";

            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
            {
                if( StartDateTextBox.Text.Trim().Length > 0 )
                {
                    if( DateTime.TryParse( StartDateTextBox.Text.Trim(), out startDate ) == true )
                    {
                        if( startDate.Year < 1950 || startDate.Year > 2200 )
                        {
                            DisplayStatusMessage( "Start date year not valid." );
                            return;
                        }
                        else
                        {
                            startDateString = StartDateTextBox.Text.Trim();
                        }
                    }
                    else
                    {
                        DisplayStatusMessage( "Start date is not a valid date." );
                        return;
                    }
                }

                if( EndDateTextBox.Text.Trim().Length > 0 )
                {
                    if( DateTime.TryParse( EndDateTextBox.Text.Trim(), out endDate ) == true )
                    {
                        if( endDate.Year < 1950 || endDate.Year > 2200 )
                        {
                            DisplayStatusMessage( "End date year not valid." );
                            return;
                        }
                        else
                        {
                            endDateString = EndDateTextBox.Text.Trim();
                        }
                    }
                    else
                    {
                        DisplayStatusMessage( "End date is not a valid date." );
                        return;
                    }
                }

                if( ( startDateString.Trim().Length == 0 && endDateString.Trim().Length > 0 ) || ( startDateString.Trim().Length > 0 && endDateString.Trim().Length == 0 ) )
                {
                    DisplayStatusMessage( "Please specify both dates." );
                    return;
                }

                if( startDateString.Trim().Length > 0 && endDateString.Trim().Length > 0 && startDate.CompareTo( endDate ) > 0 )
                {
                    DisplayStatusMessage( "Start date must preceed end date." );
                    return;
                }

            }

            // consider covered/non-covered radio button selection
            ExportUploadGlobals.ExportContents exportContents = ExportUploadGlobals.ExportContents.Undefined;

            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
            {
                if( ExportCoveredOnly.Checked == false && ExportBoth.Checked == false )
                {
                    DisplayStatusMessage( "Please select either covered or both covered and non-covered items to export." );
                    return;
                }

                if( ExportCoveredOnly.Checked == true )
                {
                    exportContents = ExportUploadGlobals.ExportContents.PharmaceuticalCoveredOnly;
                }
                else
                {
                    exportContents = ExportUploadGlobals.ExportContents.PharmaceuticalBoth;
                }

            }
            else if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
            {
                exportContents = ExportUploadGlobals.ExportContents.MedSurg;
            }

            Export( _contractNumber, _contractId, _applicationVersion, _exportUploadInstanceType.ExportUploadType, exportContents, startDateString, endDateString );
        }

        private void Export( string contractNumber, int contractId, string applicationVersion, ExportUploadGlobals.ExportUploadTypes exportType, ExportUploadGlobals.ExportContents exportContents, string startDateString, string endDateString )
        {
            string destinationFileType = "";
            string destinationFileName = "";
            bool bSuccess = false;
            string applicationServerName = "";
            int currentActivityId = -1;
            int currentActivityDetailsId = -1;
            Guid guidForCurrentModification = Guid.Empty;

            int requestId = -1;
            DateTime creationDate = DateTime.Today; 

            applicationServerName = ItemExportUploadConfiguration.ApplicationServerName;

            // set up a status request message
            ExportUploadRequest statusRequest = new ExportUploadRequest();
            statusRequest.RequestType = ExportUploadRequest.RequestTypes.Status;
            if( exportType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
            {
                statusRequest.RequestSubType = ExportUploadRequest.RequestSubTypes.MedSurg;
                
                statusRequest.StartDateCriteria = DateTime.MinValue; 
                statusRequest.EndDateCriteria = DateTime.MinValue;
            }
            else if( exportType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
                {
                statusRequest.RequestSubType = ExportUploadRequest.RequestSubTypes.Pharmaceutical;

                if( exportContents == ExportUploadGlobals.ExportContents.PharmaceuticalCoveredOnly )
                    {
                    statusRequest.EncodedCriteria =  ExportUploadRequest.EncodedCriteriaCovered;
                    }
                else if( exportContents == ExportUploadGlobals.ExportContents.PharmaceuticalBoth )
                    {
                    statusRequest.EncodedCriteria = ExportUploadRequest.EncodedCriteriaBoth;
                    }

                DateTime startDate;
                DateTime endDate;
                if( DateTime.TryParse( startDateString, out startDate ) == true )
                    {
                    statusRequest.StartDateCriteria = startDate;
                    }
                    else
                    {
                    bSuccess = false;
                    DisplayStatusMessage( string.Format( "Start date is not a valid date." ) );
                    }
                if( DateTime.TryParse( endDateString, out endDate ) == true )
                {
                    statusRequest.EndDateCriteria = endDate;
                }
                else
            {
                bSuccess = false;
                    DisplayStatusMessage( string.Format( "End date is not a valid date." ) );
                }
            }

            statusRequest.ContractNumber = contractNumber;
            statusRequest.ContractId = contractId;
            statusRequest.ScheduleNumber = _scheduleNumber;
            statusRequest.RequestingServerName = applicationServerName;
            statusRequest.CreatedBy = _userInfo.UserId;
            statusRequest.CreationDate = creationDate;  // default
            statusRequest.ApplicationVersion = applicationVersion;
           
            // export
            try
            {
                // although the activity id is set within the base class, it doesn't make it to this instance of the globals.  Must be 2 sep instances.
                if( _exportUploadDB.InitCurrentActivity( _exportUploadInstanceType.ActionType, _exportUploadInstanceType.ExportUploadType, _contractNumber, "", "", ref currentActivityId, ref currentActivityDetailsId, ref guidForCurrentModification ) == true )
            {
                    if( exportType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                {
                        // request an export from the service
                        bSuccess = _exportUploadDB.ExportItems2( _contractNumber, _contractId, _scheduleNumber, exportContents, applicationServerName, currentActivityId, startDateString, endDateString, applicationVersion, ref destinationFileType, ref destinationFileName, ref requestId, ref creationDate );
            }
                    else if( exportType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
                    {                       
                        // request an export from the service
                        bSuccess = _exportUploadDB.ExportItems2( _contractNumber, _contractId, _scheduleNumber, exportContents, applicationServerName, currentActivityId, startDateString, endDateString, applicationVersion, ref destinationFileType, ref destinationFileName, ref requestId, ref creationDate );
        }

                    // log the target filename with the activity record
                    if( _exportUploadDB.UpdateExportUploadActivityInfo( currentActivityId, destinationFileName ) == false )
            {
                        // problem updating the activity record
                        throw new Exception( _exportUploadDB.ErrorMessage );
            }
                    else
                {
                        if( bSuccess == false )
                    {
                            // log the failure
                            _exportUploadDB.UpdateExportUploadActivityStatus( currentActivityId, ExportUploadGlobals.ExportUploadStatuses.RequestFailed );

                            throw new Exception( _exportUploadDB.ErrorMessage );
                }
                        // else the server will log success messages regarding this request
                       
                    }
                }
            }
            catch( Exception ex )
            {
                bSuccess = false;
                DisplayStatusMessage( ex.Message );
        }

            if( bSuccess == true )
            {
             //   DisplayStatusMessage( "Export requested." );   let the loop show the status

                // complete remaining statusRequest fields
                statusRequest.ActivityId = currentActivityId;
                statusRequest.RequestId = requestId;
                statusRequest.CreationDate = creationDate;
                statusRequest.DestFileType = destinationFileType;                   

                InitiateStatusLoop( statusRequest );               
            }
             }


        public void InitiateStatusLoop( ExportUploadRequest statusRequest )
            {
            ClientScriptManager clientScriptManager = Page.ClientScript;

            string statusRequestMessage = statusRequest.GetMessageStringFromObject();

            clientScriptManager.RegisterHiddenField( "StatusRequestMessage", statusRequestMessage );
            
            string sendStatusRequestMessageScript = string.Format( "SendStatusRequestMessage();"  );
            clientScriptManager.RegisterStartupScript( this.GetType(), "StatusRequestScript", sendStatusRequestMessageScript, true );
        }

        //public string GetCallbackResult()
        //{
        //    string fileName = "";
        //    if( _globals.ActionType == ExportUploadGlobals.ActionTypes.Export )
        //    {
        //        fileName = _exportFileName;
        //    }
        //    else if( _globals.ActionType == ExportUploadGlobals.ActionTypes.Upload )
        //    {
        //        _resultMessage = "";
        //        if( _globals.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
        //        {
        //            if( Session[ "MedSurgErrorSpreadSheet" ] != null )
        //            {
        //                _medSurgErrorSpreadsheet = ( MedSurgErrorSpreadSheet )Session[ "MedSurgErrorSpreadSheet" ];
        //                fileName = _medSurgErrorSpreadsheet.ErrorSpreadSheetFileName;
        //                _resultMessage = "Error file retrieved.";
        //            }
        //        }
        //        else // pharm
        //        {
        //            if( Session[ "PharmErrorSpreadsheet" ] != null )
        //            {
        //                _pharmErrorSpreadsheet = ( PharmErrorSpreadSheet )Session[ "PharmErrorSpreadsheet" ];
        //                fileName = _pharmErrorSpreadsheet.ErrorSpreadSheetFileName;
        //                _resultMessage = "Error file retrieved.";
        //            }
        //        }
        //    }

        //    return ( SerializeParmsForClient( new string[] { _resultMessage, fileName, _globals.GetActionTypeStringFromEnum( _globals.ActionType ) } ) );
        //}

        //public void RaiseCallbackEvent( string eventArgument )
        //{
        //    if( eventArgument.CompareTo( "Export" ) == 0 )
        //    {
        //  //      _globals.ActionType = ExportUploadGlobals.ActionTypes.Export;
        //  //      GatherParmsAndExport();
        //    }
        //    else if( eventArgument.CompareTo( "Upload" ) == 0 )
        //    {
        //        _globals.ActionType = ExportUploadGlobals.ActionTypes.Upload;
        //     }
        //    else
        //    {
        //        _globals.ActionType = ExportUploadGlobals.ActionTypes.Undefined;
        //        SetCallbackStatusMessage( "Unexpected event argument received from client." );
        //    }
        //}
 

        // the security bit was to allow the client to disable the client side buttons, but now all buttons are server side
        // may eventually discontinue use of bit
        public void OnPreRender( ItemExportUploadControl self, EventArgs e )
        {
            if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
            {
                SetExportSecurityBitHiddenField( _userInfo.IsMedSurgExportAuthorized );

                SetProgressBarColor( "ItemExportUploadControlMedSurgProgressBar" );
            }
            else if( _exportUploadInstanceType.ExportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
            {
                SetExportSecurityBitHiddenField( _userInfo.IsPharmExportAuthorized );

                SetProgressBarColor( "ItemExportUploadControlPharmProgressBar" );
            }
        }
    }
}
