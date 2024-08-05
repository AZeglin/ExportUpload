using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Principal;

namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class UserInfo
    {
        private System.Guid _userId = Guid.Empty;
        private string _firstName = System.String.Empty;
        private string _lastName = System.String.Empty;
        private string _fullName = System.String.Empty;
        private string _loginName = System.String.Empty; // with domain
        private string _email = System.String.Empty;
        private string _phone = System.String.Empty;
        private int _oldUserId = -1;
        private int _division = -1;
        //private bool _bAuthorizedForExport = false;
        //private bool _bAuthorizedForUpload = false;

        private bool _bIsMedSurgExportAuthorized = false;
        private bool _bIsMedSurgUploadAuthorized = false;
        private bool _bIsPharmExportAuthorized = false;
        private bool _bIsPharmUploadAuthorized = false;
        private bool _bCanEdit = false;
        private bool _bCanView = false;

        public UserInfo()
        {
            _loginName = GetLoginName();
        }

        private string GetLoginName()
        {
            string loginName = "";

            try
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy( PrincipalPolicy.WindowsPrincipal );
                WindowsPrincipal currentPrincipal = ( WindowsPrincipal )Thread.CurrentPrincipal;
                loginName = currentPrincipal.Identity.Name;

                if( loginName.Length == 0 )
                {
                    throw new Exception( "Login name from current windows principal was blank." );
                }
            }
            catch( Exception ex )
            {
                string msg = "Exception encountered in GetLoginName()";
                throw new Exception( msg, ex );
            }

            return ( loginName );
        }        

        public int OldUserId
        {
            get { return _oldUserId; }
            set { _oldUserId = value; }
        }

        public System.Guid UserId
        {
            get
            {
                return ( _userId );
            }
            set
            {
                _userId = value;
            }
        }

        public string GetUserIdString()
        {
            return ( _userId.ToString() );
        }

        public string FirstName
        {
            get
            {
                return ( _firstName );
            }
            set
            {
                _firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return ( _lastName );
            }
            set
            {
                _lastName = value;
            }
        }

        public string FullName
        {
            get
            {
                return ( _fullName );
            }
            set
            {
                _fullName = value;
            }
        }

        public string LoginName
        {
            get
            {
                return ( _loginName );
            }
            set
            {
                _loginName = value;
            }
        }

        public string Email
        {
            get
            {
                return ( _email );
            }
            set
            {
                _email = value;
            }
        }

        public string Phone
        {
            get
            {
                return ( _phone );
            }
            set
            {
                _phone = value;
            }
        }

        public int Division
        {
            get { return _division; }
            set { _division = value; }
        }

        //public bool IsAuthorizedForExport
        //{
        //    get { return _bAuthorizedForExport; }
        //    set { _bAuthorizedForExport = value; }
        //}

        //public bool IsAuthorizedForUpload
        //{
        //    get { return _bAuthorizedForUpload; }
        //    set { _bAuthorizedForUpload = value; }
        //}


        public bool IsMedSurgExportAuthorized
        {
            get { return _bIsMedSurgExportAuthorized; }
            set { _bIsMedSurgExportAuthorized = value; }
        }

        public bool IsMedSurgUploadAuthorized
        {
            get { return _bIsMedSurgUploadAuthorized; }
            set { _bIsMedSurgUploadAuthorized = value; }
        }

        public bool IsPharmExportAuthorized
        {
            get { return _bIsPharmExportAuthorized; }
            set { _bIsPharmExportAuthorized = value; }
        }

        public bool IsPharmUploadAuthorized
        {
            get { return _bIsPharmUploadAuthorized; }
            set { _bIsPharmUploadAuthorized = value; }
        }

        public bool CanEdit
        {
            get { return _bCanEdit; }
            set { _bCanEdit = value; }
        }

        public bool CanView
        {
            get { return _bCanView; }
            set { _bCanView = value; }
        }
    }
}
