using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class ItemExportUploadGlobals
    {
        private int _currentActivityId = -1;
        private int _currentActivityDetailsId = -1;

        public int CurrentActivityId
        {
            get { return _currentActivityId; }
            set { _currentActivityId = value; }
        }

        public int CurrentActivityDetailsId
        {
            get { return _currentActivityDetailsId; }
            set { _currentActivityDetailsId = value; }
        }

        private ActionTypes _actionType = ActionTypes.Undefined;

        public enum ActionTypes
        {
            Undefined,
            Export,
            Upload
        }

        public ActionTypes ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }

        public string GetActionTypeStringFromEnum( ActionTypes actionType )
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
            Pharmaceutical
        }

        public enum ExportContents
        {
            Undefined,
            MedSurg,
            PharmaceuticalCoveredOnly,
            PharmaceuticalBoth          // both covered and non-covered
        }

      
        public string GetExportContentsFromEnum( ExportContents exportContents )
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

        private ExportUploadTypes _exportUploadType = ExportUploadTypes.Undefined;

        public ExportUploadTypes ExportUploadType
        {
            get { return _exportUploadType; }
            set { _exportUploadType = value; }
        }

        public ExportUploadTypes GetExportUploadTypeFromString( string exportUploadType )
        {
            ExportUploadTypes retVal = ExportUploadTypes.Undefined;

            if( exportUploadType.CompareTo( "M" ) == 0 )
                retVal = ExportUploadTypes.MedSurg;
            else if( exportUploadType.CompareTo( "P" ) == 0 )
                retVal = ExportUploadTypes.Pharmaceutical;

            return ( retVal );
        }

        public string GetExportUploadStringFromType( ExportUploadTypes exportUploadType )
        {
            string retVal = "";

            if( exportUploadType == ExportUploadTypes.MedSurg )
                retVal = "M";
            else if( exportUploadType == ExportUploadTypes.Pharmaceutical )
                retVal = "P";

            return ( retVal );
        }

        public enum RequestStatuses
        {
            Undefined,
            ExportRequested,
            UploadRequested,
            ExportInitiated,
            UploadInitiated,
            ExportError,
            UploadError,
            ExportComplete,
            UploadComplete
        }

        private RequestStatuses _requestStatus = RequestStatuses.Undefined;

        public RequestStatuses RequestStatus
        {
            get { return _requestStatus; }
            set { _requestStatus = value; }
        }

        public string GetRequestStatusFromEnum(RequestStatuses requestStatus)
        {
            string requestStatusString = "U";

            if (requestStatus == RequestStatuses.ExportRequested)
                requestStatusString = "ER";
            else if (requestStatus == RequestStatuses.UploadRequested)
                requestStatusString = "UR";
            else if (requestStatus == RequestStatuses.ExportInitiated)
                requestStatusString = "EI";
            else if (requestStatus == RequestStatuses.UploadInitiated)
                requestStatusString = "UI";
            else if (requestStatus == RequestStatuses.ExportError)
                requestStatusString = "EE";
            else if (requestStatus == RequestStatuses.UploadError)
                requestStatusString = "UE";
            else if (requestStatus == RequestStatuses.ExportComplete)
                requestStatusString = "EC";
            else if (requestStatus == RequestStatuses.UploadComplete)
                requestStatusString = "UC";

            return (requestStatusString);
        }
  
    }
}
