using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.NAC.Services.NACCMExportUploadSharedObj
{
    [Serializable]
    public class ExportUploadInstanceType 
    {
        private int _currentActivityId = -1;
        private int _currentActivityDetailsId = -1;

        public int CurrentActivityId
        {
            get
            {
                return _currentActivityId;
            }
            set
            {
                _currentActivityId = value;
            }
        }

        public int CurrentActivityDetailsId
        {
            get
            {
                return _currentActivityDetailsId;
            }
            set
            {
                _currentActivityDetailsId = value;
            }
        }

        private ExportUploadGlobals.ActionTypes _actionType = ExportUploadGlobals.ActionTypes.Undefined;

        public ExportUploadGlobals.ActionTypes ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
            }
        }

        private ExportUploadGlobals.ExportUploadTypes _exportUploadType = ExportUploadGlobals.ExportUploadTypes.Undefined;

        public ExportUploadGlobals.ExportUploadTypes ExportUploadType
        {
            get
            {
                return _exportUploadType;
            }
            set
            {
                _exportUploadType = value;
            }
        }
    }
}
