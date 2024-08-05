using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Threading;
using System.Security.Principal;

using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    public class Global : System.Web.HttpApplication
    {
        private NameValueCollection _appSettings = null;
        
        private static IEModeHttpModule IEMode;
        private static bool _bAddedHeader = false;

        protected void Application_Start( object sender, EventArgs e )
        {
            IEMode = new IEModeHttpModule();

            _appSettings = ( NameValueCollection )ConfigurationManager.AppSettings;
            
            ItemExportUploadConfiguration config = ItemExportUploadConfiguration.Create();
            ItemExportUploadConfiguration.Init( _appSettings );
          
            ScriptManager.ScriptResourceMapping.AddDefinition( "jquery", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.7.1.min.js"

                // below taken from example code
         //       DebugPath = "~/Scripts/jquery-" + JQueryVer + ".js",
         //       CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".min.js",
         //       CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".js",
         //       CdnSupportsSecureConnection = true,
         //       LoadSuccessExpression = "window.jQuery"
            } );

        }

        public override void Init()
        {
            base.Init();

            if( _bAddedHeader == false )
            {
                if( IEMode != null )
                {
                    IEMode.Init( this );
                    _bAddedHeader = true;
                }
            }
        }

        protected void Session_Start( object sender, EventArgs e )
        {
         //   Application_Start( sender, e ); // remove after debug complete $$$

            
        }

        protected void Application_BeginRequest( object sender, EventArgs e )
        {

        }

        protected void Application_AuthenticateRequest( object sender, EventArgs e )
        {

        }

        protected void Application_Error( object sender, EventArgs e )
        {

        }

        protected void Session_End( object sender, EventArgs e )
        {

        }

        protected void Application_End( object sender, EventArgs e )
        {

        }
    }
}