<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemExportUpload.aspx.cs" Inherits="VA.NAC.ItemExportUploadBrowser.ItemUpload" %>
<%@ Register TagPrefix="UC" TagName="ItemExportUploadControl" Src="~/ItemExportUploadControl.ascx" %>

<!DOCTYPE html />
<html>

<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge"> 
  
    <title>Item Export/Upload</title>
         
<script src="./Scripts/jquery-3.7.1.min.js"  type="text/javascript"></script>    
<script src="./Scripts/jquery.unobtrusive-ajax.min.js"   type="text/javascript"></script>
    
    <link href="https://www.jqueryscript.net/css/jquerysctipttop.css" rel="stylesheet" type="text/css">

    <link rel="stylesheet" href="Content/themes/base/exportuploadcontrol2.css" type="text/css" >
  
    <script type="text/javascript">
        /* revert back to 2.1.3 ? */
      /*  jQuery.noConflict(true); */

        function CloseWindow(bRefresh) {
          
            if (window.opener.document.forms[0].fvContractInfo$RefreshPricelistScreenOnSubmit) {
            window.opener.document.forms[0].fvContractInfo$RefreshPricelistScreenOnSubmit.value = bRefresh;
            window.opener.document.forms[0].submit();
            }
            else if (window.opener.document.forms[0].ctl00$ctl00$ContentPlaceHolderMain$ContractViewContentPlaceHolder$ItemPriceCountsFormView$RefreshPricelistCountsOnSubmit) {
                window.opener.document.forms[0].ctl00$ctl00$ContentPlaceHolderMain$ContractViewContentPlaceHolder$ItemPriceCountsFormView$RefreshPricelistCountsOnSubmit.value = bRefresh;
                window.opener.document.forms[0].submit();
            }

          
            top.window.opener = top;
            top.window.open('', '_parent', '');
            top.window.close();
        }

        function CloseWebSocketHelper() {
         
            CloseWebSocket();
        }

    </script>
</head>
<body>
    
        
    <form id="MedSurgExportUploadForm" runat="server" onprerender="MedSurgExportUploadForm_OnPreRender" enctype="multipart/form-data"  >
            <asp:ScriptManager ID="ItemUploadScriptManager" runat="server" EnablePartialRendering="true"  OnAsyncPostBackError="ItemUploadScriptManager_OnAsyncPostBackError" >
            </asp:ScriptManager>
           <asp:Button ID="FormCloseButton" runat="server" CausesValidation="false" Style="z-index: 100; left: 316px; position: absolute;
            top: 22px" Text="Close" Width="60px" />
            <asp:Panel id="ItemExportUploadControlPanel" runat="server" Height="612px" Width="382px" >
            <UC:ItemExportUploadControl ID="ItemExportUploadControl1" runat="server" />

            </asp:Panel>
       
    </form>
</body>
</html>
