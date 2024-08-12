using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    public partial class ItemUpload : System.Web.UI.Page
    {
        UserInfo _userInfo = null;

        protected void Page_Load( object sender, EventArgs e )
        {
            Session[ "ItemExportUploadStartedProperly" ] = true;
            
            if( Page.IsPostBack == false )
            {
                ClearSessionVariables();
            }

            string strContractNumber = HttpContext.Current.Request.QueryString[ "ContractNumber" ];
            string strScheduleNumber = HttpContext.Current.Request.QueryString[ "ScheduleNumber" ];
            string exportUploadTypeString = HttpContext.Current.Request.QueryString[ "ExportUploadType" ];
            string exportUploadPermissionsString = HttpContext.Current.Request.QueryString[ "Id" ];
            string strContractId = HttpContext.Current.Request.QueryString[ "ContractId" ];
            string strIsBPA = HttpContext.Current.Request.QueryString[ "IsBPA" ];
            string strApplicationVersion = HttpContext.Current.Request.QueryString["Ver"];

            if ( Session[ "ExportUploadInstanceType" ] == null )
            {
                Session[ "ExportUploadInstanceType" ] = new ExportUploadInstanceType();
            }
            
            this.ItemExportUploadControl1.ContractId = int.Parse( strContractId );
            this.ItemExportUploadControl1.IsBPA = bool.Parse( strIsBPA );

            this.ItemExportUploadControl1.ContractNumber = strContractNumber;
            this.ItemExportUploadControl1.ScheduleNumber = int.Parse( strScheduleNumber );
            this.ItemExportUploadControl1.ExportUploadTypeString = exportUploadTypeString;
            this.ItemExportUploadControl1.ExportUploadPermissionsString = exportUploadPermissionsString;
            this.ItemExportUploadControl1.ApplicationVersion = strApplicationVersion;

            if ( Session[ "ItemExportUploadUserInfo" ] != null )
            {
                _userInfo = ( UserInfo )Session[ "ItemExportUploadUserInfo" ];
            }
            else
            {
                _userInfo = new UserInfo(); // gets login name of current user
                Session[ "ItemExportUploadUserInfo" ] = _userInfo;
            }

            bool bIsMedSurgExportAuthorized = false;
            bool bIsMedSurgUploadAuthorized = false;
            bool bIsPharmExportAuthorized = false;
            bool bIsPharmUploadAuthorized = false;
            bool bCanEdit = false;
            bool bCanView = false;

            GetExportUploadPermissions( exportUploadPermissionsString, ref bCanEdit, ref bCanView, ref bIsMedSurgExportAuthorized, ref bIsMedSurgUploadAuthorized, ref bIsPharmExportAuthorized, ref bIsPharmUploadAuthorized );

            _userInfo.IsMedSurgExportAuthorized = bIsMedSurgExportAuthorized;
            _userInfo.IsMedSurgUploadAuthorized = bIsMedSurgUploadAuthorized;
            _userInfo.IsPharmExportAuthorized = bIsPharmExportAuthorized;
            _userInfo.IsPharmUploadAuthorized = bIsPharmUploadAuthorized;
            _userInfo.CanEdit = bCanEdit;
            _userInfo.CanView = bCanView;

            AddCloseButton( true ); 
           
        }

        private void ClearSessionVariables()
        {
            Session[ "ItemExportUploadUserInfo" ] = null;
            Session[ "ExportUploadInstanceType" ] = null;
        }

        // refresh is required after an upload
        public void AddCloseButton( bool bRefresh )
        {
            FormCloseButton.Attributes.Remove( "onclick" );
            FormCloseButton.Attributes.Add( "onclick", string.Format( "CloseWebSocketHelper(); CloseWindow( {0} );", bRefresh.ToString().ToLower() ));
        }

        protected void MedSurgExportUploadForm_OnPreRender( object sender, EventArgs e )
        {
            HtmlForm medSurgExportUploadForm = ( HtmlForm )sender;
            if( medSurgExportUploadForm != null )
            {
                Panel itemExportUploadControlPanel = ( Panel )medSurgExportUploadForm.FindControl( "ItemExportUploadControlPanel" );

                if( itemExportUploadControlPanel != null )
                {
                    //  background-image: -ms-linear-gradient(top, rgb(255,255,255) 10%, rgb(192,255,192) 81%); 
                    if( this.ItemExportUploadControl1.ExportUploadTypeString.CompareTo( "P" ) == 0 )
                    {
                        itemExportUploadControlPanel.Attributes[ "class" ] = "ItemExportUploadControlPanelPharmGradient";
                    }
                    else if( this.ItemExportUploadControl1.ExportUploadTypeString.CompareTo( "M" ) == 0 )
                    {
                        itemExportUploadControlPanel.Attributes[ "class" ] = "ItemExportUploadControlPanelMedSurgGradient";
                    }
                }
            }
            this.ItemExportUploadControl1.OnPreRender( this.ItemExportUploadControl1, e );
        }

        protected void ItemUploadScriptManager_OnAsyncPostBackError( object sender, AsyncPostBackErrorEventArgs e )
        {
            string errorMsg = "";

            if( e.Exception.Data[ "ItemUploadErrorMessage" ] != null )
            {
                errorMsg = string.Format( "The following error was encountered during async postback: {0} /nDetails: {1}", e.Exception.Message, e.Exception.Data[ "ItemUploadErrorMessage" ] );
            }
            else
            {
                errorMsg = string.Format( "The following error was encountered during async postback: {0}", e.Exception.Message );
            }

            ItemUploadScriptManager.AsyncPostBackErrorMessage = errorMsg;
        }

        // bit positions which match the positions used by the caller
        // any positions between  1 and 128
        private const int CanEdit = 87;
        private const int CanView = 88;
        private const int MSUpload = 22;
        private const int PharmExport = 24;
        private const int MSExport = 13;
        private const int PharmUpload = 37;
        private string _bitString = "";

        private void GetExportUploadPermissions( string exportUploadPermissionsString, ref bool bCanEdit, ref bool bCanView, ref bool bIsMedSurgExportAuthorized, ref bool bIsMedSurgUploadAuthorized, ref bool bIsPharmExportAuthorized, ref bool bIsPharmUploadAuthorized )
        {
            // extract permissions encoded in the guid id in the query string  
            Guid exportUploadPermissions = Guid.NewGuid();
            exportUploadPermissions = Guid.Empty;
            
            try
            {          
                if( Guid.TryParse( exportUploadPermissionsString, out exportUploadPermissions ) == true )
                {
                    Byte[] bytes = exportUploadPermissions.ToByteArray();
                    BitArray bits = new BitArray( bytes );

                    _bitString = GetStringFromBitArray( bits );

                    // set the permissions
                    bIsMedSurgExportAuthorized = bits[ MSExport ];
                    bIsMedSurgUploadAuthorized = bits[ MSUpload ];
                    bIsPharmExportAuthorized = bits[ PharmExport ];
                    bIsPharmUploadAuthorized = bits[ PharmUpload ];
                    bCanEdit = bits[ CanEdit ];
                    bCanView = bits[ CanView ];
                }
                else
                {
                    bCanEdit = false;
                    bCanView = false;
                    bIsMedSurgExportAuthorized = false;
                    bIsMedSurgUploadAuthorized = false;
                    bIsPharmExportAuthorized = false;
                    bIsPharmUploadAuthorized = false;
                }
            }
            catch( Exception ex )
            {
                string tmp = ex.Message;
            }
        }

        protected string GetStringFromBitArray( BitArray bits )
        {
            StringBuilder sb = new StringBuilder( 128 );

            for( int i = 0; i < bits.Count; i++ )
            {
                if( bits[ i ] == true )
                    sb.Append( "1" );
                else
                    sb.Append( "0" );
            }

            return ( sb.ToString() );
        }
    }
}
