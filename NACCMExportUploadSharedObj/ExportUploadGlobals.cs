using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VA.NAC.Services.NACCMExportUploadSharedObj
{
    [Serializable]
    public class ExportUploadGlobals
    {
      
        [Serializable]
        public static class ActivityTypes
        {
            public const string Export = "E";
            public const string Upload = "U";
        }


        [Serializable]
        public static class ActivityDataTypes
        {
            public const string PharmaceuticalCoveredOnly = "C";
            public const string PharmaceuticalBoth = "B";
            public const string MedSurg = "M";
            public const string Pharmaceutical = "P";
        }

      

        public enum ActionTypes
        {
            Undefined,
            Export,
            Upload
        }

        public static string GetActionTypeStringFromEnum( ActionTypes actionType )
        {
            string actionTypeString = "Undefined";
            if( actionType == ActionTypes.Export )
                actionTypeString = "Export";
            else if( actionType == ActionTypes.Upload )
                actionTypeString = "Upload";

            return ( actionTypeString );
        }

        public enum ExportUploadTypes
        {
            Undefined,
            MedSurg,
            Pharmaceutical,
            Sales
        }


        public static ExportUploadTypes GetExportUploadTypeFromString( string exportUploadType )
        {
            ExportUploadTypes retVal = ExportUploadTypes.Undefined;

            if( exportUploadType.CompareTo( "M" ) == 0 )
                retVal = ExportUploadTypes.MedSurg;
            else if( exportUploadType.CompareTo( "P" ) == 0 )
                retVal = ExportUploadTypes.Pharmaceutical;
            else if( exportUploadType.CompareTo( "S" ) == 0 )
                retVal = ExportUploadTypes.Sales;

            return ( retVal );
        }


        public static string GetExportUploadStringFromType( ExportUploadTypes exportUploadType ) 
        {
            string exportUploadTypeString = "U";

            if( exportUploadType == ExportUploadTypes.MedSurg )
                exportUploadTypeString = "M";
            else if( exportUploadType == ExportUploadTypes.Pharmaceutical )
                exportUploadTypeString = "P";
            else if( exportUploadType == ExportUploadTypes.Sales )
                exportUploadTypeString = "S";

            return ( exportUploadTypeString );
        }

        public enum ExportContents
        {
            Undefined,
            MedSurg,
            PharmaceuticalCoveredOnly,
            PharmaceuticalBoth          // both covered and non-covered
        }

      
        public static string GetExportContentsFromEnum( ExportContents exportContents )
        {
            string exportContentsString = "U";

            if( exportContents == ExportContents.MedSurg )
                exportContentsString = "M";
            else if( exportContents == ExportContents.PharmaceuticalBoth )
                exportContentsString = "B";
            else if( exportContents == ExportContents.PharmaceuticalCoveredOnly )
                exportContentsString = "C";

            return ( exportContentsString );
        }



        //public enum RequestStatuses
        //{
        //    Undefined,
        //    ExportRequested,
        //    UploadRequested,
        //    ExportInitiated,
        //    UploadInitiated,
        //    ExportError,
        //    UploadError,
        //    ExportComplete,
        //    UploadComplete,
        //    RequestFailed,
        //    ParseError,
        //    StatusFailed
        //}

        //private RequestStatuses _requestStatus = RequestStatuses.Undefined;

        //public RequestStatuses RequestStatus
        //{
        //    get { return _requestStatus; }
        //    set { _requestStatus = value; }
        //}

        //public string GetRequestStatusFromEnum(RequestStatuses requestStatus)
        //{
        //    string requestStatusString = ExportUploadStatuses.Undefined;

        //    if (requestStatus == RequestStatuses.ExportRequested)
        //        requestStatusString = ExportUploadStatuses.ExportRequested;
        //    else if (requestStatus == RequestStatuses.UploadRequested)
        //        requestStatusString = ExportUploadStatuses.UploadRequested;
        //    else if (requestStatus == RequestStatuses.ExportInitiated)
        //        requestStatusString = ExportUploadStatuses.ExportInitiated;
        //    else if (requestStatus == RequestStatuses.UploadInitiated)
        //        requestStatusString = ExportUploadStatuses.UploadInitiated;
        //    else if (requestStatus == RequestStatuses.ExportError)
        //        requestStatusString = ExportUploadStatuses.ExportError;
        //    else if (requestStatus == RequestStatuses.UploadError)
        //        requestStatusString = ExportUploadStatuses.UploadError;
        //    else if (requestStatus == RequestStatuses.ExportComplete)
        //        requestStatusString = ExportUploadStatuses.ExportComplete;
        //    else if (requestStatus == RequestStatuses.UploadComplete)
        //        requestStatusString = ExportUploadStatuses.UploadComplete;
        //    else if( requestStatus == RequestStatuses.RequestFailed )
        //        requestStatusString = ExportUploadStatuses.RequestFailed;
        //    else if( requestStatus == RequestStatuses.ParseError )
        //        requestStatusString = ExportUploadStatuses.ParseError;
        //    else if( requestStatus == RequestStatuses.StatusFailed )
        //        requestStatusString = ExportUploadStatuses.StatusFailed;
        //    return (requestStatusString);
        //}

        [Serializable]
        public static class ExportUploadStatuses
        {
            public const string Undefined = "U";

            public const string ExportRequested = "ER";
            public const string ExportInitiated = "EI";
            public const string ExportInProgress = "EP";
            public const string ExportComplete = "EC";
            public const string ExportSuccessful = "ES"; // used by the export SP only
            public const string ExportError = "EE";
            public const string ExportUndefined = "EU";
            
            public const string UploadRequested = "UR";
            public const string UploadInitiated = "UI";
            public const string UploadInProgress = "UP";
            public const string UploadComplete = "UC";
            public const string UploadError = "UE";
            public const string UploadUndefined = "UU";

            public const string RequestFailed = "RF";
            public const string StatusFailed = "SF";
            public const string ParseError = "PE";           
           

            public const string GatheredSINsForExport = "GS";   // these are sales specific
            public const string LoadComplete = "LC";
            public const string LoadError = "LE";
            public const string ParseComplete = "PC";
        }
    }
}
