using System;
using System.Net;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using VA.NAC.Services.NACCMExportUploadSharedObj;

namespace VA.NAC.ItemExportUploadBrowser
{
    public partial class ExportRetrieval : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {

            if( Session[ "ItemExportUploadStartedProperly" ] == null )
            {            
                Response.StatusCode = ( int )System.Net.HttpStatusCode.Forbidden;
                Response.BufferOutput = true;
                Response.Redirect( "403A2.htm" );
            }
            
            if( HttpContext.Current != null )
            {
                // set the content type to excel
                Response.ContentType = "application/x-msexcel";

                string strFilePath = HttpContext.Current.Request.QueryString[ "ExtractFileName" ];
                string exportUploadType = HttpContext.Current.Request.QueryString[ "ExportUploadType" ];

                string fileName = Path.GetFileName( strFilePath );

                Response.AddHeader( "Content-Disposition", "attachment;filename=" + SimplifyDestinationFileName( fileName, WebUtility.HtmlEncode( exportUploadType ) ) );

             //   Response.WriteFile( strFilePath );

                using( Stream s = new FileStream( strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                {
                    s.CopyTo( Response.OutputStream );
                }

                Response.End();
            }
          
        }

        private string SimplifyDestinationFileName( string fileName, string exportUploadType )
        {
            string destinationFileName = "";

            string[] fileNameParts = fileName.Split( new char[] { '_' } );

            if( exportUploadType.CompareTo( "E" ) == 0 )
            {
                destinationFileName = string.Format( "{0}_{1}.xlsx", fileNameParts[ 0 ], fileNameParts[ 1 ] );
            }
            else if( exportUploadType.CompareTo( "U" ) == 0 )
            {
                int lastSegment = fileNameParts.GetLength( 0 );
                destinationFileName = string.Format( "{0}_{1}", fileNameParts[ 0 ], fileNameParts[ lastSegment - 1 ] );
            }

            return ( destinationFileName );
        }
    }
}
