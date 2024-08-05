using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class ItemExportUploadConfiguration 
    {
        private static ItemExportUploadConfiguration _this = null;

        private static string _checkForPreviousUploadDays = "";
        private static string _applicationServerName = "";
        private static string _socketHandlerUrl = "";

        private static string _exportUploadDatabaseServerName = ""; 
        private static string _exportUploadDatabaseName = "";

        private static string _exportUploadDatabaseCommonUserName = "";
        private static string _exportUploadDatabaseCommonUserPassword = "";

        private static string _securityDatabaseServerName = "";
        private static string _securityDatabaseName = "";

        private static string _securityCommonUserName = "";
        private static string _securityCommonUserPassword = "";

        private static string _medSurgUploadArchiveDirectoryPath = "";
        private static string _pharmaceuticalUploadArchiveDirectoryPath = "";

        public static string CheckForPreviousUploadDays
        {
            get { return ItemExportUploadConfiguration._checkForPreviousUploadDays; }
            set { ItemExportUploadConfiguration._checkForPreviousUploadDays = value; }
        }

        public static string ApplicationServerName
        {
            get { return ItemExportUploadConfiguration._applicationServerName; }
            set { ItemExportUploadConfiguration._applicationServerName = value; }
        }
 
        public static string SocketHandlerUrl
        {
            get
        {
                return ItemExportUploadConfiguration._socketHandlerUrl;
        }
            set
        {
                ItemExportUploadConfiguration._socketHandlerUrl = value;
        }
        }

        public static string ExportUploadDatabaseServerName
        {
            get { return ItemExportUploadConfiguration._exportUploadDatabaseServerName; }
            set { ItemExportUploadConfiguration._exportUploadDatabaseServerName = value; }
        }
 
        public static string ExportUploadDatabaseName
        {
            get { return ItemExportUploadConfiguration._exportUploadDatabaseName; }
            set { ItemExportUploadConfiguration._exportUploadDatabaseName = value; }
        }

        public static string MedSurgUploadArchiveDirectoryPath
        {
            get { return ItemExportUploadConfiguration._medSurgUploadArchiveDirectoryPath; }
            set { ItemExportUploadConfiguration._medSurgUploadArchiveDirectoryPath = value; }
        }
 
        public static string PharmaceuticalUploadArchiveDirectoryPath
        {
            get { return ItemExportUploadConfiguration._pharmaceuticalUploadArchiveDirectoryPath; }
            set { ItemExportUploadConfiguration._pharmaceuticalUploadArchiveDirectoryPath = value; }
        }
 
        public static string SecurityCommonUserName
        {
            get { return ItemExportUploadConfiguration._securityCommonUserName; }
            set { ItemExportUploadConfiguration._securityCommonUserName = value; }
        }

        public static string SecurityCommonUserPassword
        {
            get { return ItemExportUploadConfiguration._securityCommonUserPassword; }
            set { ItemExportUploadConfiguration._securityCommonUserPassword = value; }
        }
        public static string SecurityDatabaseName
        {
            get { return ItemExportUploadConfiguration._securityDatabaseName; }
            set { ItemExportUploadConfiguration._securityDatabaseName = value; }
        }

        public static string SecurityDatabaseServerName
        {
            get { return ItemExportUploadConfiguration._securityDatabaseServerName; }
            set { ItemExportUploadConfiguration._securityDatabaseServerName = value; }
        }

        public static string ExportUploadlDatabaseCommonUserName
        {
            get { return ItemExportUploadConfiguration._exportUploadDatabaseCommonUserName; }
            set { ItemExportUploadConfiguration._exportUploadDatabaseCommonUserName = value; }
        }

        public static string ExportUploadDatabaseCommonUserPassword
        {
            get { return ItemExportUploadConfiguration._exportUploadDatabaseCommonUserPassword; }
            set { ItemExportUploadConfiguration._exportUploadDatabaseCommonUserPassword = value; }
        }

        public static ItemExportUploadConfiguration Create()
        {
            if( _this == null )
            {
                _this = new ItemExportUploadConfiguration();
            }

            return ( _this );
        }

        public ItemExportUploadConfiguration()
        {
          
        }

        public static void Init( NameValueCollection appSettings )
        {
            _applicationServerName = ( string )appSettings[ "applicationServerName" ];
            _socketHandlerUrl = ( string )appSettings[ "socketHandlerUrl" ];

            _checkForPreviousUploadDays = ( string )appSettings[ "checkForPreviousUploadDays" ];
            
            _exportUploadDatabaseServerName = ( string )appSettings[ "exportUploadDatabaseServerName" ];
            _exportUploadDatabaseName = ( string )appSettings[ "exportUploadDatabaseName" ];

            _medSurgUploadArchiveDirectoryPath = ( string )appSettings[ "medSurgUploadArchiveDirectoryPath" ];
            _pharmaceuticalUploadArchiveDirectoryPath = ( string )appSettings[ "pharmaceuticalUploadArchiveDirectoryPath" ];

            _exportUploadDatabaseCommonUserName = ( string )appSettings[ "exportUploadDatabaseCommonUserName" ];
            _exportUploadDatabaseCommonUserPassword = ( string )appSettings[ "exportUploadDatabaseCommonUserPassword" ];

            _securityDatabaseServerName = ( string )appSettings[ "securityDatabaseServerName" ];
            _securityDatabaseName = ( string )appSettings[ "securityDatabaseName" ];

            _securityCommonUserName = ( string )appSettings[ "securityCommonUserName" ];
            _securityCommonUserPassword = ( string )appSettings[ "securityCommonUserPassword" ];
        }

 
        public static bool IsValid()
        {
            return ( _exportUploadDatabaseServerName.Length > 0 && _exportUploadDatabaseName.Length > 0  );   
        }

  
    }
}
