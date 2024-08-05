<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckForPreviousUpload.aspx.cs" Inherits="VA.NAC.ItemExportUploadBrowser.CheckForPreviousUpload" %>

<!DOCTYPE html>

<script src="./Scripts/jquery-3.7.1.min.js"  type="text/javascript"></script>    
<script src="./Scripts/jquery.unobtrusive-ajax.min.js"   type="text/javascript"></script>

<html>
<head runat="server">
      <meta http-equiv="X-UA-Compatible" content="IE=Edge"> 

      <title>Previous Upload Warning</title>
      <script type="text/javascript">
       <!--
          function CloseWindow(proceedAnyway) {
              window.opener.document.forms[0].PreviousUploadProceedAnyway.value = proceedAnyway;
              window.opener.document.forms[0].PreviousUploadProceedAnywayShowProgress.value = proceedAnyway;

              window.opener.ShowProgress();

              window.opener.document.forms[0].submit();

              top.window.opener = top;
              top.window.open('', '_parent', '');
              top.window.close();
          }
        //-->
        </script>
  
</head>
<body>
    <form id="CheckForPreviousUploadForm" runat="server">
    <div>
        <table style="height:100%; width:100%;">
        <col style="width:120px;" />
        <col style="width:120px;" />
            <tr>
                <td colspan="2" style="text-align:justify;" >
                    <asp:Label ID="PreviousUploadLabel"  runat="server" Width="98%" />
                </td>
            </tr>
            <tr style="height:30px;">
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="CancelUploadButton" Text="Cancel Upload" runat="server" OnClientClick="CloseWindow(false);" Width="98%" />
                </td>
                <td>
                    <asp:Button ID="ProceedAnywayButton" Text="Proceed Anyway" runat="server" OnClientClick="CloseWindow(true);" Width="98%"/>
                </td>
            </tr>
        
        
        </table>
    </div>
    </form>
</body>
</html>
