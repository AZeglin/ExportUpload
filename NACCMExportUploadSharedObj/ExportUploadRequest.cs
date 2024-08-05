using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VA.NAC.Services.NACCMExportUploadSharedObj
{
    [Serializable]
    public class ExportUploadRequest
    {
        public ExportUploadRequest()
        {

        }

        public ExportUploadRequest( ExportUploadRequest req )
        {
            this.RequestId = req.RequestId;
            this.RequestingServerName = req.RequestingServerName;
            this.ActivityId = req.ActivityId;
            this.ActivityDetailsId = req.ActivityDetailsId;
            this.SalesExportUploadId = req.SalesExportUploadId;
            this.SelectedFiscalYear = req.SelectedFiscalYear;
            this.SelectedFiscalQuarter = req.SelectedFiscalQuarter;
            this.GuidForCurrentModification = req.GuidForCurrentModification;
            this.RequestStatus = req.RequestStatus;
            this.RequestType = req.RequestType;
            this.RequestSubType = req.RequestSubType;
            this.CreatedBy = req.CreatedBy;
            this.CreationDate = req.CreationDate;
            this.ApplicationVersion = req.ApplicationVersion;
            this.ContractNumber = req.ContractNumber;
            this.ContractId = req.ContractId; 
            this.ScheduleNumber = req.ScheduleNumber;
            this.StartDateCriteria = req.StartDateCriteria;
            this.EndDateCriteria = req.EndDateCriteria;
            this.EncodedCriteria = req.EncodedCriteria;
            this.SourceFileType = req.SourceFileType;
            this.SourceFileExt = req.SourceFileExt;
            this.SourceFileName = req.SourceFileName;
            this.SourceFilePathName = req.SourceFilePathName;
            this.DestFileType = req.DestFileType;
            this.DestFileExt = req.DestFileExt;
            this.DestFileName = req.DestFileName;
            this.DestFilePathName = req.DestFilePathName;
            this.StatusMessage = req.StatusMessage;
            this.CompletionLevel = req.CompletionLevel;
        }

        private int _requestId;

        public int RequestId
        {
            get { return _requestId; }
            set { _requestId = value; }
        }

        private string _requestingServerName = "";
        public string RequestingServerName
        {
            get
            {
                return _requestingServerName;
            }
            set
            {
                _requestingServerName = value;
            }
        }


        private int _activityId = -1;
        public int ActivityId
        {
            get
            {
                return _activityId;
            }
            set
            {
                _activityId = value;
            }
        }

        private int _activityDetailsId = -1;
        public int ActivityDetailsId
        {
            get
            {
                return _activityDetailsId;
            }
            set
            {
                _activityDetailsId = value;
            }
        }
 

        private Guid _salesExportUploadId;

        public Guid SalesExportUploadId
        {
            get
            {
                return _salesExportUploadId;
            }
            set
            {
                _salesExportUploadId = value;
            }
        }

        private int _selectedFiscalYear = 1979;

        public int SelectedFiscalYear
        {
            get
            {
                return _selectedFiscalYear;
            }
            set
            {
                _selectedFiscalYear = value;
            }
        }

        private int _selectedFiscalQuarter = 1;

        public int SelectedFiscalQuarter
        {
            get
            {
                return _selectedFiscalQuarter;
            }
            set
            {
                _selectedFiscalQuarter = value;
            }
        }

        private Guid _GuidForCurrentModification;

        public Guid GuidForCurrentModification
        {
            get
            {
                return _GuidForCurrentModification;
            }
            set
            {
                _GuidForCurrentModification = value;
            }
        }

        public enum RequestStatuses
        {
            Undefined,

            ExportRequested,
            UploadRequested,
            ExportInitiated,
            UploadInitiated,
            ExportInProgress,
            UploadInProgress,
            ExportComplete,
            UploadComplete,

            ExportError,
            UploadError,
            ExportUndefined,         
            UploadUndefined,
            
            RequestFailed,           
            StatusFailed,
            ParseError,

            GatheredSINsForExport, // these are sales specific
            LoadComplete,
            LoadError,
            ParseComplete
        }

        private RequestStatuses _requestStatus = RequestStatuses.Undefined;

        public RequestStatuses RequestStatus
        {
            get { return _requestStatus; }
            set { _requestStatus = value; }
        }

        public static string GetRequestStatusStringFromEnum( RequestStatuses requestStatus )
        {
            string requestStatusString = ExportUploadGlobals.ExportUploadStatuses.Undefined;

            if( requestStatus == RequestStatuses.ExportRequested )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ExportRequested;
            else if( requestStatus == RequestStatuses.UploadRequested )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.UploadRequested;
            else if( requestStatus == RequestStatuses.ExportInitiated )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ExportInitiated;
            else if( requestStatus == RequestStatuses.UploadInitiated )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.UploadInitiated;
            else if( requestStatus == RequestStatuses.ExportInProgress )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ExportInProgress;
            else if( requestStatus == RequestStatuses.UploadInProgress )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.UploadInProgress;
            else if( requestStatus == RequestStatuses.ExportError )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ExportError;
            else if( requestStatus == RequestStatuses.UploadError )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.UploadError;
            else if( requestStatus == RequestStatuses.ExportComplete )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ExportComplete;
            else if( requestStatus == RequestStatuses.UploadComplete )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.UploadComplete;
            else if( requestStatus == RequestStatuses.RequestFailed )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.RequestFailed;
            else if( requestStatus == RequestStatuses.ParseError )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ParseError;
            else if( requestStatus == RequestStatuses.StatusFailed )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.StatusFailed;
            else if( requestStatus == RequestStatuses.GatheredSINsForExport )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.GatheredSINsForExport;
            else if( requestStatus == RequestStatuses.LoadComplete )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.LoadComplete;
            else if( requestStatus == RequestStatuses.LoadError )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.LoadError;
            else if( requestStatus == RequestStatuses.ParseComplete )
                requestStatusString = ExportUploadGlobals.ExportUploadStatuses.ParseComplete;

            return ( requestStatusString );
        }
        
        public static RequestStatuses GetRequestStatusFromString( string requestStatusString )
        {
            RequestStatuses requestStatus = RequestStatuses.Undefined;

            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ExportRequested ) == 0 )
                requestStatus = RequestStatuses.ExportRequested;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.UploadRequested ) == 0 )
                requestStatus = RequestStatuses.UploadRequested;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.RequestFailed ) == 0 )
                requestStatus = RequestStatuses.RequestFailed;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.StatusFailed ) == 0 )
                requestStatus = RequestStatuses.StatusFailed;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ExportInitiated ) == 0 )
                requestStatus = RequestStatuses.ExportInitiated;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.UploadInitiated ) == 0 )
                requestStatus = RequestStatuses.UploadInitiated;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ExportInProgress ) == 0 )
                requestStatus = RequestStatuses.ExportInProgress;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.UploadInProgress ) == 0 )
                requestStatus = RequestStatuses.UploadInProgress;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ExportError ) == 0 )
                requestStatus = RequestStatuses.ExportError;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.UploadError ) == 0 )
                requestStatus = RequestStatuses.UploadError;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ExportComplete ) == 0 )
                requestStatus = RequestStatuses.ExportComplete;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.UploadComplete ) == 0 )
                requestStatus = RequestStatuses.UploadComplete;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ParseError ) == 0 )
                requestStatus = RequestStatuses.ParseError;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.GatheredSINsForExport ) == 0 )
                requestStatus = RequestStatuses.GatheredSINsForExport;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.LoadComplete ) == 0 )
                requestStatus = RequestStatuses.LoadComplete;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.LoadError ) == 0 )
                requestStatus = RequestStatuses.LoadError;
            if( requestStatusString.CompareTo( ExportUploadGlobals.ExportUploadStatuses.ParseComplete ) == 0 )
                requestStatus = RequestStatuses.ParseComplete;

            return ( requestStatus );
        }

        private RequestTypes _requestType = RequestTypes.Undefined;

        public enum RequestTypes
        {
            Undefined,
            Export,
            Upload,
            Status
        }

        public RequestTypes RequestType
        {
            get { return _requestType; }
            set { _requestType = value; }
        }

        public string GetRequestTypeStringFromEnum( RequestTypes requestType )
        {
            string requestTypeString = "X";            // 	nchar(2)   'E' or 'U'
            if( requestType == RequestTypes.Export )
                requestTypeString = "E";
            else if( requestType == RequestTypes.Upload )
                requestTypeString = "U";
             else if( requestType == RequestTypes.Status )
                requestTypeString = "T";

            return ( requestTypeString );
        }

        public RequestTypes GetRequestTypeFromString( string requestTypeString )
        {
            RequestTypes requestType = RequestTypes.Undefined;

            if( requestTypeString.CompareTo( "E" ) == 0 )
            {
                requestType = RequestTypes.Export;
            }
            else if( requestTypeString.CompareTo( "U") == 0 )
            {
                requestType = RequestTypes.Upload;
            }
            else if( requestTypeString.CompareTo( "T") == 0 )
            {
                requestType = RequestTypes.Status;
            }
            return ( requestType );
        }

        private RequestSubTypes _requestSubType = RequestSubTypes.Undefined;

        public enum RequestSubTypes
        {
            Undefined,
            MedSurg,
            Pharmaceutical,
            Sales
        }

        public RequestSubTypes RequestSubType
        {
            get { return _requestSubType; }
            set { _requestSubType = value; }
        }

        public string GetRequestSubTypeStringFromEnum( RequestSubTypes requestSubType )
        {
            string requestSubTypeString = "X";            // 	nchar(2)   'M' or 'P' or 'S'
            if( requestSubType == RequestSubTypes.MedSurg )
                requestSubTypeString = "M";
            else if( requestSubType == RequestSubTypes.Pharmaceutical )
                requestSubTypeString = "P";
            else if( requestSubType == RequestSubTypes.Sales )
                requestSubTypeString = "S";

            return ( requestSubTypeString );
        }

        // 	nchar(2)    'M' or 'P' or 'S'
        public RequestSubTypes GetRequestSubTypeFromString( string requestSubType )
        {
            RequestSubTypes retVal = RequestSubTypes.Undefined;

            if( requestSubType.CompareTo( "M" ) == 0 )
                retVal = RequestSubTypes.MedSurg;
            else if( requestSubType.CompareTo( "P" ) == 0 )
                retVal = RequestSubTypes.Pharmaceutical;
            else if( requestSubType.CompareTo( "S" ) == 0 )     
                retVal = RequestSubTypes.Sales;

            return ( retVal );
        }
        
        private Guid _createdBy;              // 	uniqueidentifier

        public Guid CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        
        private DateTime _creationDate;        

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; }
        }

        private string _applicationVersion;     //    char(8)

        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set { _applicationVersion = value; }
        }

        private string _contractNumber;         //   	nvarchar(20) 

        public string ContractNumber
        {
            get { return _contractNumber; }
            set { _contractNumber = value; }
        }

        private int _contractId;

        public int ContractId
        {
            get
            {
                return _contractId;
            }
            set
            {
                _contractId = value;
            }
        }


        private int _scheduleNumber;

        public int ScheduleNumber
        {
            get { return _scheduleNumber; }
            set { _scheduleNumber = value; }
        }
        
        private DateTime _startDateCriteria;   

        public DateTime StartDateCriteria
        {
            get { return _startDateCriteria; }
            set { _startDateCriteria = value; }
        }
        
        private DateTime _endDateCriteria;    

        public DateTime EndDateCriteria
        {
            get { return _endDateCriteria; }
            set { _endDateCriteria = value; }
        }
        
        private string _encodedCriteria;      //  	char(20)     

        public string EncodedCriteria
        {
            get { return _encodedCriteria; }
            set { _encodedCriteria = value; }
        }

        // possible export criteria
        public const string EncodedCriteriaCovered = "C";
        public const string EncodedCriteriaBoth = "B";

        // possible upload criteria
        public const string EncodedCriteriaBPA = "BPA";
        
        private string _sourceFileType;               //         	nchar(2) 

        public string SourceFileType
        {
            get { return _sourceFileType; }
            set { _sourceFileType = value; }
        }
        
        private string _sourceFileExt;      //    	nvarchar(10) 

        public string SourceFileExt
        {
            get { return _sourceFileExt; }
            set { _sourceFileExt = value; }
        }
        
        private string _sourceFileName;      //   	nvarchar(255) 

        public string SourceFileName
        {
            get { return _sourceFileName; }
            set { _sourceFileName = value; }
        }
        
        private string _sourceFilePathName;      //   	nvarchar(255) 

        public string SourceFilePathName
        {
            get { return _sourceFilePathName; }
            set { _sourceFilePathName = value; }
        }

        private string _destFileType;               //         	nchar(2) 

        public string DestFileType
        {
            get
            {
                return _destFileType;
            }
            set
            {
                _destFileType = value;
            }
        }
        
        private string _destFileExt;        //      	nvarchar(10) 

        public string DestFileExt
        {
            get { return _destFileExt; }
            set { _destFileExt = value; }
        }
        
        private string _destFileName;      //     	nvarchar(255)

        public string DestFileName
        {
            get { return _destFileName; }
            set { _destFileName = value; }
        }
        
        private string _destFilePathName;      //     	nvarchar(255) 

        public string DestFilePathName
        {
            get { return _destFilePathName; }
            set { _destFilePathName = value; }
        }
                
        private string _statusMessage;      //    	nvarchar(255) 

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; }
        }

        private int _completionLevel;     // int

        public int CompletionLevel
        {
            get { return _completionLevel; }
            set { _completionLevel = value; }
        }

        public void PopulateObjectFromRow( DataRow row )
        {
            Guid createdBy =  Guid.Empty;
            Guid GuidForCurrentModification = Guid.Empty;
            Guid salesExportUploadId = Guid.Empty;

            if( row.IsNull( "RequestId" ) != true )
                this.RequestId = ( int )row[ "RequestId" ];
            if( row.IsNull( "ActivityId" ) != true )
                this.ActivityId = ( int )row[ "ActivityId" ];
            if( row.IsNull( "ActivityDetailsId" ) != true )
                this.ActivityDetailsId = ( int )row[ "ActivityDetailsId" ];
            if( row.IsNull( "SalesExportUploadId" ) != true )
            {
                if( Guid.TryParse( row[ "SalesExportUploadId" ].ToString(), out salesExportUploadId ) == true )
                    this.SalesExportUploadId =  salesExportUploadId;
                else
                    this.SalesExportUploadId = Guid.Empty;
            }
              
            if( row.IsNull( "SelectedFiscalYear" ) != true )
                this.SelectedFiscalYear = ( int )row[ "SelectedFiscalYear" ];
            if( row.IsNull( "SelectedFiscalQuarter" ) != true )
                this.SelectedFiscalQuarter = ( int )row[ "SelectedFiscalQuarter" ];
                 
            if( row.IsNull( "GuidForCurrentModification" ) != true )
            {
                if( Guid.TryParse( row[ "GuidForCurrentModification" ].ToString(), out GuidForCurrentModification ) == true )
                    this.GuidForCurrentModification =  createdBy;
                else
                    this.GuidForCurrentModification = Guid.Empty;            
            }
            if( row.IsNull( "RequestStatus" ) != true )
                this.RequestStatus = GetRequestStatusFromString( ( string )row[ "RequestStatus" ] );
            if( row.IsNull( "RequestType" ) != true )
                this.RequestType = GetRequestTypeFromString( row[ "RequestType" ].ToString().Trim() );
            if( row.IsNull( "RequestSubType" ) != true )
                this.RequestSubType = GetRequestSubTypeFromString( row[ "RequestSubType" ].ToString().Trim() );
            if( row.IsNull( "CreatedBy" ) != true )
            {
                if( Guid.TryParse( row[ "CreatedBy" ].ToString(), out createdBy ) == true )
                    this.CreatedBy =  createdBy;
                else
                    this.CreatedBy = Guid.Empty;
            }
            if( row.IsNull( "CreationDate" ) != true )
                this.CreationDate = ( DateTime )row[ "CreationDate" ];
            if (row.IsNull("ApplicationVersion") != true)
                this.ApplicationVersion = (string)row["ApplicationVersion"];
            if ( row.IsNull( "ContractNumber" ) != true )
                this.ContractNumber = ( string )row[ "ContractNumber" ];
            if( row.IsNull( "ContractId" ) != true )
                this.ContractId = ( int )row[ "ContractId" ];
            if( row.IsNull( "ScheduleNumber" ) != true )
                this.ScheduleNumber = ( int )row[ "ScheduleNumber" ];
            if( row.IsNull( "StartDateCriteria" ) != true )
                this.StartDateCriteria = ( DateTime )row[ "StartDateCriteria" ];
            if( row.IsNull( "EndDateCriteria" ) != true )
                this.EndDateCriteria = ( DateTime )row[ "EndDateCriteria" ];
            if( row.IsNull( "EncodedCriteria" ) != true )
                this.EncodedCriteria = ( string )row[ "EncodedCriteria" ];
            if( row.IsNull( "SourceFileType" ) != true )
                this.SourceFileType = ( string )row[ "SourceFileType" ];
            if( row.IsNull( "SourceFileExt" ) != true )
                this.SourceFileExt = ( string )row[ "SourceFileExt" ];
            if( row.IsNull( "SourceFileName" ) != true )
                this.SourceFileName = ( string )row[ "SourceFileName" ];
            if( row.IsNull( "SourceFilePathName" ) != true )
                this.SourceFilePathName = ( string )row[ "SourceFilePathName" ];
            if( row.IsNull( "DestFileType" ) != true )
                this.DestFileType = ( string )row[ "DestFileType" ];
            if( row.IsNull( "DestFileExt" ) != true )
                this.DestFileExt = ( string )row[ "DestFileExt" ];
            if( row.IsNull( "DestFileName" ) != true )
                this.DestFileName = ( string )row[ "DestFileName" ];
            if( row.IsNull( "DestFilePathName" ) != true )
                this.DestFilePathName = ( string )row[ "DestFilePathName" ];
            if( row.IsNull( "StatusMessage" ) != true )
                this.StatusMessage = ( string )row[ "StatusMessage" ];
            if( row.IsNull( "CompletionLevel" ) != true )
                this.CompletionLevel = ( int )row[ "CompletionLevel" ];
        }
        
        // request from client
        public void PopulateObjectFromMessage( string messageString )
        {
            DateTime creationDate;
            DateTime startDate;
            DateTime endDate;            
            int completionLevel = 0;

            string[] messageParts = messageString.Split( '|' );
            if( messageParts.Length > 0 )
            {
                _requestId = int.Parse( messageParts[0] );   // for initial request, will be -1
            }
            if( messageParts.Length > 1 )
            {
                _requestingServerName = messageParts[ 1 ];
            }
            if( messageParts.Length > 2 )
            {
                _activityId = int.Parse( messageParts[ 2 ] );
            }
            if( messageParts.Length > 3 )
            {
                _activityDetailsId = int.Parse( messageParts[ 3 ] );
            }
              if( messageParts.Length > 4)
            {
                _salesExportUploadId = Guid.Parse( messageParts[ 4 ] );
            }
              if( messageParts.Length > 5 )
            {
                _selectedFiscalYear = int.Parse( messageParts[ 5 ] );
            }
              if( messageParts.Length > 6 )
            {
                _selectedFiscalQuarter = int.Parse( messageParts[ 6 ] );
            }                  
            if( messageParts.Length > 7 )
            {
                _GuidForCurrentModification = Guid.Parse( messageParts[ 7 ] );
            }
            if( messageParts.Length > 8 )
            {
                _requestStatus = GetRequestStatusFromString( messageParts[ 8 ] );
            }
            if( messageParts.Length > 9 )
            {
                _requestType = GetRequestTypeFromString( messageParts[ 9 ] );
            }
            if( messageParts.Length > 10 )
            {
                _requestSubType = GetRequestSubTypeFromString( messageParts[ 10 ] );
            }
            if( messageParts.Length > 11 )
            {
                _createdBy = Guid.Parse( messageParts[ 11 ] );
            }
            if( messageParts.Length > 12 )
            {                
                if( DateTime.TryParse( messageParts[ 12 ], out creationDate ) == true )
                {
                    _creationDate = creationDate;
                }
                else
                {
                    throw new Exception( "Creation date is not a valid date." );
                }
            }
            if (messageParts.Length > 13)
            {
                _applicationVersion = messageParts[13];
            }
            if ( messageParts.Length > 14 )
            {
                _contractNumber = messageParts[ 14 ];
            }
            if( messageParts.Length > 15 )
            {
                _contractId = int.Parse( messageParts[ 15 ] );
            }
            if( messageParts.Length > 16  )
            {
                _scheduleNumber = int.Parse( messageParts[ 16 ] );
            }
            if( messageParts.Length > 17 )
            {
                if( _requestSubType == RequestSubTypes.Pharmaceutical )
                {
                    if( DateTime.TryParse( messageParts[ 17 ], out startDate ) == true )
                    {
                        _startDateCriteria = startDate;
                    }
                    else
                    {
                        throw new Exception( "Start date is not a valid date." );
                    }
                }
                else
                {
                    _startDateCriteria = DateTime.MinValue;
                }
            }
            if( messageParts.Length > 18 )
            {               
                if( _requestSubType == RequestSubTypes.Pharmaceutical )
                {
                    if( DateTime.TryParse( messageParts[ 18 ], out endDate ) == true )
                    {
                        _endDateCriteria = endDate;
                    }
                    else
                    {
                        throw new Exception( "End date is not a valid date." );
                    }
                }
                else
                {
                    _endDateCriteria = DateTime.MinValue;
                }
            }
            if( messageParts.Length > 19 )
            {
                _encodedCriteria = messageParts[ 19 ];
            }
            if( messageParts.Length > 20 )
            {
                _sourceFileType = messageParts[ 20 ];
            }
            if( messageParts.Length > 21 )
            {
                _sourceFileExt = messageParts[ 21 ];
            }
            if( messageParts.Length > 22 )
            {
                _sourceFileName = messageParts[ 22 ];
            }
            if( messageParts.Length > 23 )
            {
                _sourceFilePathName = messageParts[ 23 ];
            }
            if( messageParts.Length > 24 )
            {
                _destFileType = messageParts[ 24 ];
            }
            if( messageParts.Length > 25 )
            {
                _destFileExt = messageParts[ 25 ];
            }
            if( messageParts.Length > 26 )
            {
                _destFileName = messageParts[ 26 ];
            }
            if( messageParts.Length > 27 )
            {
                _destFilePathName = messageParts[ 27 ];
            }
            if( messageParts.Length > 28 )
            {
                _statusMessage = messageParts[ 28 ];
            }
            if( messageParts.Length > 29 )
            {
                if( int.TryParse( messageParts[ 29 ], out completionLevel ) == true )
                {
                    _completionLevel = completionLevel;
                }
                else
                {
                    _completionLevel = 0;
                }
            }
            else
            {
                throw new Exception( "Incorrect field count received in ExportUploadRequest message." );
            }
        }

        // response back to client
        public string GetMessageStringFromObject()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( _requestId.ToString() );
            sb.Append( "|" );
            sb.Append( _requestingServerName );
            sb.Append( "|" );
            sb.Append( _activityId.ToString() );
            sb.Append( "|" );
            sb.Append( _activityDetailsId.ToString() );
            sb.Append( "|" );
            sb.Append( _salesExportUploadId.ToString() );
            sb.Append( "|" );
            sb.Append( _selectedFiscalYear.ToString() );
            sb.Append( "|" );
            sb.Append( _selectedFiscalQuarter.ToString() );
            sb.Append( "|" );
            sb.Append( _GuidForCurrentModification.ToString() );
            sb.Append( "|" );
            sb.Append( GetRequestStatusStringFromEnum( _requestStatus ) );
            sb.Append( "|" );
            sb.Append( GetRequestTypeStringFromEnum( _requestType ) );
            sb.Append( "|" );
            sb.Append( GetRequestSubTypeStringFromEnum( _requestSubType ) );
            sb.Append( "|" );
            sb.Append( _createdBy.ToString() );
            sb.Append( "|" );
            sb.Append( _creationDate.ToString( "MM/dd/yyyy" ) );
            sb.Append( "|" );
            sb.Append( _applicationVersion );
            sb.Append( "|" );
            sb.Append( _contractNumber );
            sb.Append( "|" );
            sb.Append( _contractId );
            sb.Append( "|" );
            sb.Append( _scheduleNumber.ToString() );
            sb.Append( "|" );
            sb.Append( _startDateCriteria.ToString( "MM/dd/yyyy" ) );
            sb.Append( "|" );
            sb.Append( _endDateCriteria.ToString( "MM/dd/yyyy" ) );
            sb.Append( "|" );
            sb.Append( _encodedCriteria );
            sb.Append( "|" );
            sb.Append( _sourceFileType );
            sb.Append( "|" );
            sb.Append( _sourceFileExt );
            sb.Append( "|" );
            sb.Append( _sourceFileName );
            sb.Append( "|" );
            sb.Append( _sourceFilePathName );
            sb.Append( "|" );
            sb.Append( _destFileType );
            sb.Append( "|" );
            sb.Append( _destFileExt );
            sb.Append( "|" );
            sb.Append( _destFileName );
            sb.Append( "|" );
            sb.Append( _destFilePathName );
            sb.Append( "|" );
            sb.Append( _statusMessage );
            sb.Append( "|" );
            sb.Append( _completionLevel.ToString() );
            sb.Append( "|" );

            return( sb.ToString() );
        }

    }
}
