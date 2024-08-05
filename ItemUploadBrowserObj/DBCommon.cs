using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlClient;

using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class DBCommon
    {
        private string _connectionString = "";

        private string _errorMessage = "";

        private Guid _guidForCurrentModification = Guid.Empty;

        private string _userName = "";

        private Guid _currentUserId = Guid.Empty;

        public Guid GuidForCurrentModification
        {
            get { return _guidForCurrentModification; }
            set { _guidForCurrentModification = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public Guid CurrentUserId
        {
            get { return _currentUserId; }
            set { _currentUserId = value; }
        }



        public DBCommon( ref UserInfo userInfo )
        {
            _userName = userInfo.LoginName;
            _currentUserId = userInfo.UserId;
        }

        // version used by separate http handler which doesn't have access to session variables
        public DBCommon()
        {
            _userName = "";
            _currentUserId = Guid.Empty;
        }

        public SqlConnection CreateSqlConnection()
        {
            if( ItemExportUploadConfiguration.IsValid() )
            {
                return ( new SqlConnection( _connectionString ) );
            }
            else
            {
                _errorMessage = "Invalid connection string. Please edit application database configuration information settings.";
                return ( null );
            }
        }

        private const string _commonUserConnectionStringFormatString = "Data Source={0};Initial Catalog={1};UID={2};PWD={3};";

        public string CommonUserConnectionStringFormatString
        {
            get { return _commonUserConnectionStringFormatString; }
        }

        public enum TargetDatabases
        {
            Security,
            ExportUpload,
            ExportAction,
            Undefined
        }

 
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public void SetConnectionString( TargetDatabases targetDatabase )
        {
            switch( targetDatabase )
            {

                case TargetDatabases.Security:
                    {
                        _connectionString = string.Format( CommonUserConnectionStringFormatString, ItemExportUploadConfiguration.SecurityDatabaseServerName, ItemExportUploadConfiguration.SecurityDatabaseName, ItemExportUploadConfiguration.SecurityCommonUserName, ItemExportUploadConfiguration.SecurityCommonUserPassword );
                        break;
                    }
                case TargetDatabases.ExportUpload:
                    {
                        _connectionString = string.Format( CommonUserConnectionStringFormatString, ItemExportUploadConfiguration.ExportUploadDatabaseServerName, ItemExportUploadConfiguration.ExportUploadDatabaseName, ItemExportUploadConfiguration.ExportUploadlDatabaseCommonUserName, ItemExportUploadConfiguration.ExportUploadDatabaseCommonUserPassword );
                        break;
                    }
              
                case TargetDatabases.ExportAction:
                    {
                        _connectionString = string.Format(CommonUserConnectionStringFormatString, ItemExportUploadConfiguration.ExportUploadDatabaseServerName, ItemExportUploadConfiguration.ExportUploadDatabaseName, "vhamaster\\ammhindevdb", "Appdevdb2**" );
                        break;
                    }
                case TargetDatabases.Undefined:
                    {
                        _connectionString = "";
                        break;
                    }
            }
        }

        public string ErrorMessage
        {
            get
            {
                return ( _errorMessage );
            }
            set
            {
                _errorMessage = value;
            }
        }
    }
}
