using System;
using System.net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VA.NAC.ItemExportUploadBrowser
{
    public partial class CheckForPreviousUpload : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            string message = HttpContext.Current.Request.QueryString[ "message" ];

            PreviousUploadLabel.Text = WebUtility.HtmlEncode( message );
        }
    }
}
