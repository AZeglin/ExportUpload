<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemExportUploadControl.ascx.cs" Inherits="VA.NAC.ItemExportUploadBrowser.ItemExportUploadControl" %>

    <style type="text/css">
    .ItemExportUploadControlPharmProgressBar
    {
        background-color:rgba(184,222,184, 0.99);
        text-align:center; 
        height:20px; 
        width:0%; 
        border-width:1px; 
        border-color:darkgray;
        padding:0px; 
        margin:0px;
    }

    .ItemExportUploadControlMedSurgProgressBar
    {
        background-color:rgba(177, 201, 245, 0.99);
        text-align:center; 
        height:20px; 
        width:0%; 
        border-width:1px; 
        border-color:darkgray;
        padding:0px; 
        margin:0px;
    }
    </style>

   <script type="text/javascript">

      

       function EnableProgressIndicator(bEnable) {
       //    alert('EnableProgressIndicator with ' + bEnable);
           var progressIndicatorDiv = document.getElementById( '<%=ProgressIndicator.ClientID %>' );

           if (progressIndicatorDiv != null) {
               if (bEnable == true) {
                   progressIndicatorDiv.style.visibility = "visible";               
               }
               else {
                   progressIndicatorDiv.style.visibility = "hidden";
               }
           }
           else {
               alert('Div was null.');
           }
       }

       function isPostBack() {

           return document.referrer.indexOf(document.location.href) > -1;
       }

       // called from proceed dialog
       function ShowProgress() {
           var proceedAnyway = $get("PreviousUploadProceedAnywayShowProgress").value;
  
           if (proceedAnyway == "true") {
               EnableProgressIndicator(true);
           }
       }
    
       $(document).ready(function () {
           if( isPostBack() == false )
              EnableProgressIndicator(false);         
       });

       function OnUpload() {
           EnableProgressIndicator(true);
       }

       function OnExport() {                  
           EnableProgressIndicator(true);
       }
   
     
       function RetrieveFile() {                  
           if (_requestType == "E") {
               if (_requestStatus == "EC") {
                   
                   window.open("./ExportRetrieval.aspx?ExtractFileName=" + _destFilePathName + "&ExportUploadType=" + _requestType);
               }
               else {
                   alert("Export not available: " + _statusMessage);
               }
           }
           else {             
               window.open("./ExportRetrieval.aspx?ExtractFileName=" + _destFilePathName + "&ExportUploadType=" + _requestType);
           }
       }

       function DeserializeMessage(msg) {

           var parmArray = msg.split('|');

           if (parmArray.length > 0) {
               _requestId = parmArray[0];
           }
           if (parmArray.length > 1) {
               _requestingServerName = parmArray[1];
           }          
           if (parmArray.length > 2) {
               _activityId = parmArray[2];
           }
           if (parmArray.length > 3) {
               _activityDetailsId = parmArray[3];
           }
           if (parmArray.length > 4) {
               _salesExportUploadId = parmArray[4];
           }
           if (parmArray.length > 5) {
               _selectedFiscalYear = parmArray[5];
           }
           if (parmArray.length > 6) {
               _selectedFiscalQuarter = parmArray[6];
           }
           if (parmArray.length > 7) {
               _guidForCurrentModification = parmArray[7];
           }
           if (parmArray.length > 8) {
               _requestStatus = parmArray[8];
           }
           if (parmArray.length > 9) {
               _requestType = parmArray[9];
           }
           if (parmArray.length > 10) {
               _requestSubType = parmArray[10];
           }
           if (parmArray.length > 11) {
               _createdBy = parmArray[11];
           }
           if (parmArray.length > 12) {
               _creationDate = parmArray[12];
           }
           if (parmArray.length > 13) {
               _applicationVersion = parmArray[13];
           }
           if (parmArray.length > 14) {
               _contractNumber = parmArray[14];
           }
           if (parmArray.length > 15) {
               _contractId = parmArray[15];
           }
           if (parmArray.length > 16) {
               _scheduleNumber = parmArray[16];
           }
           if (parmArray.length > 17) {
               _startDateCriteria = parmArray[17];
           }
           if (parmArray.length > 18) {
               _endDateCriteria = parmArray[18];
           }
           if (parmArray.length > 19) {
               _encodedCriteria = parmArray[19];
           }
           if (parmArray.length > 20) {
               _sourceFileType = parmArray[20];
           }
           if (parmArray.length > 21) {
               _sourceFileExt = parmArray[21];
           }
           if (parmArray.length > 22) {
               _sourceFileName = parmArray[22];
           }
           if (parmArray.length > 23) {
               _sourceFilePathName = parmArray[23];
           }
           if (parmArray.length > 24) {
               _destFileType = parmArray[24];
           }
           if (parmArray.length > 25) {
               _destFileExt = parmArray[25];
           }
           if (parmArray.length > 26) {
               _destFileName = parmArray[26];
           }
           if (parmArray.length > 27) {
               _destFilePathName = parmArray[27];
           }
           if (parmArray.length > 28) {
               _statusMessage = parmArray[28];
           }
           if (parmArray.length > 29) {
               _completionLevel = parmArray[29];
           }         
       }

       // message fields for an ExportUploadRequest object
       var _requestId = "";                 // 0
       var _requestingServerName = "";
       var _activityId = "";
       var _activityDetailsId = "";
       var _salesExportUploadId = "";
       var _selectedFiscalYear = "";
       var _selectedFiscalQuarter = "";
       var _guidForCurrentModification = "";
       var _requestStatus = "";
       var _requestType = "";
       var _requestSubType = "";            //10
       var _createdBy = "";
       var _creationDate = "";
       var _applicationVersion = "";        //13
       var _contractNumber = "";
       var _contractId = "";                //15
       var _scheduleNumber = "";            
       var _startDateCriteria = "";
       var _endDateCriteria = "";
       var _encodedCriteria = "";
       var _sourceFileType = "";            //20
       var _sourceFileExt = "";
       var _sourceFileName = "";            
       var _sourceFilePathName = "";
       var _destFileType = "";
       var _destFileExt = "";
       var _destFileName = "";              
       var _destFilePathName = "";
       var _statusMessage = "";
       var _completionLevel = "";           //29
       // end of message fields 

       var spinner;

       var _webSocket;
     
       function GetStatusDescriptionFromRequestStatus(requestStatus) {

           var statusDescription = "";

           if (requestStatus == "ER")
               statusDescription = "Export Requested";
           if (requestStatus == "UR")
               statusDescription = "Upload Requested";
           if (requestStatus == "RF")
               statusDescription = "Request Failed";
           if (requestStatus == "SF")
               statusDescription = "Status Failed";
           if (requestStatus == "EI")
               statusDescription = "Export Initiated";
           if (requestStatus == "UI")
               statusDescription = "Upload Initiated";
           if (requestStatus == "EP")
               statusDescription = "Export In Progress";
           if (requestStatus == "UP")
               statusDescription = "Upload In Progress";
           if (requestStatus == "EE")
               statusDescription = "Export Error";
           if (requestStatus == "UE")
               statusDescription = "Upload Error";
           if (requestStatus == "EC")
               statusDescription = "Export Complete";
           if (requestStatus == "UC")
               statusDescription = "Upload Complete";
           if (requestStatus == "PE")
               statusDescription = "Parse Error";
           if (requestStatus == "GS")
               statusDescription = "Gathered SINs For Export";
           if (requestStatus == "LC")
               statusDescription = "Load Complete";
           if (requestStatus == "LE")
               statusDescription = "Load Error";
           if (requestStatus == "PC")
               statusDescription = "Parse Complete";

           return (statusDescription);
       }

       function IsRequestStatusComplete(requestStatus) {

           var isComplete = false;

           if (requestStatus == "ER")
               isComplete = false;
           if (requestStatus == "UR")
               isComplete = false;
           if (requestStatus == "RF")
               isComplete = true;
           if (requestStatus == "SF")
               isComplete = true;
           if (requestStatus == "EI")
               isComplete = false;
           if (requestStatus == "UI")
               isComplete = false;
           if (requestStatus == "EP")
               isComplete = false;
           if (requestStatus == "UP")
               isComplete = false;
           if (requestStatus == "EE")
               isComplete = true;
           if (requestStatus == "UE")
               isComplete = true;
           if (requestStatus == "EC")
               isComplete = true;
           if (requestStatus == "UC")
               isComplete = true;
           if (requestStatus == "PE")
               isComplete = true;
           if (requestStatus == "GS")
               isComplete = false;
           if (requestStatus == "LC")
               isComplete = false;
           if (requestStatus == "LE")
               isComplete = true;
           if (requestStatus == "PC")
               isComplete = false;

           return (isComplete);
       }

       // successful with respect to should an error message be displayed or not
       function IsRequestStatusSuccessful(requestStatus) {

           var isSuccessful = false;

           if (requestStatus == "ER")
               isSuccessful = true;
           if (requestStatus == "UR")
               isSuccessful = true;
           if (requestStatus == "RF")
               isSuccessful = false;
           if (requestStatus == "SF")
               isSuccessful = false;
           if (requestStatus == "EI")
               isSuccessful = true;
           if (requestStatus == "UI")
               isSuccessful = true;
           if (requestStatus == "EP")
               isSuccessful = true;
           if (requestStatus == "UP")
               isSuccessful = true;
           if (requestStatus == "EE")
               isSuccessful = false;
           if (requestStatus == "UE")
               isSuccessful = false;
           if (requestStatus == "EC")
               isSuccessful = true;
           if (requestStatus == "UC")
               isSuccessful = true;
           if (requestStatus == "PE")
               isSuccessful = false;
           if (requestStatus == "GS")
               isSuccessful = false;
           if (requestStatus == "LC")
               isSuccessful = false;
           if (requestStatus == "LE")
               isSuccessful = false;
           if (requestStatus == "PC")
               isSuccessful = false;

           return (isSuccessful);
       }
          
       function ShowCurrentStatusDescription() {
           var containingControlId = $get("ContainingControlId").value;       
           var uploadStatusLabel = document.getElementById(containingControlId + "_UploadStatusLabel1");
          
           uploadStatusLabel.innerHTML = GetStatusDescriptionFromRequestStatus(_requestStatus);
       }

       function ShowCurrentStatusMessage() {
           var containingControlId = $get("ContainingControlId").value;
           var uploadStatusLabel = document.getElementById(containingControlId + "_UploadStatusLabel2");
         
           uploadStatusLabel.innerHTML = _statusMessage;
       }

       function ShowCurrentCompletionLevel() {

           // bar not wide enough to display text until after 10%
           if (_completionLevel >= 10) {
               document.getElementById('<%=ProgressTextLabel.ClientID%>').innerHTML = _completionLevel + " %";
           }
           var unusedPercent = 100 - _completionLevel;

           document.getElementById("<%=ProgressBar.ClientID%>").style.width = _completionLevel + "%";
       
           document.getElementById("<%=UnusedBar.ClientID%>").style.width = unusedPercent + "%";                         
       }

       /*  web sockets code */
       function CloseWebSocket() {
           if (_webSocket) {             
               _webSocket.close();            
           }         
       }
  
       function SendStatusRequestMessage() {
           
           var socketHandlerUrl = document.getElementById("SocketHandlerUrl").value;
                  
           // initialize the websocket if required
           if (_webSocket == undefined) {            
 
                _webSocket = new WebSocket(socketHandlerUrl);          

                // opened connection handler function
                _webSocket.onopen = function () {
                    // next message to send was deposited by server                 
                    var statusRequestMessage = document.getElementById("StatusRequestMessage").value;

                    if (_webSocket) {
                        // send the data if the websocket is opened
                        if (_webSocket.OPEN && _webSocket.readyState == 1) {
                     
                            _webSocket.send(statusRequestMessage);               
                        }

                        // if the websocket is closed, show a closed message
                        if (_webSocket.readyState == 2 || _webSocket.readyState == 3) {
                            _statusMessage = "Connection unexpectedly closed. Please try again.";
                            ShowCurrentStatusMessage();
                        }
                    }
                };

                // message handler function
                _webSocket.onmessage = function (e) {
                    // received message
                    var message = e.data;
                     
                    // message processing
                    DeserializeMessage(message);

                
  
                    // display the status description for the status received in the message               
                    ShowCurrentStatusDescription();

                    // if not successful, then also show the status message
                    if(IsRequestStatusSuccessful(_requestStatus) == false )
                        ShowCurrentStatusMessage();

                    // next message to send was deposited by server       
                    var statusRequestMessage = document.getElementById("StatusRequestMessage").value;

              

                    // request is in-process, so send another status request message
                    if (_requestStatus == "ER" || _requestStatus == "UR" || _requestStatus == "EI" || _requestStatus == "UI" || _requestStatus == "EP" || _requestStatus == "UP") {
                        
             
                        ShowCurrentCompletionLevel();
                        
                        _webSocket.send(statusRequestMessage);
                    }
                                          
                    // if the status indicates there is something to retrieve, then retrieve the export or the upload error file
                    if (_requestStatus == "UE" || _requestStatus == "EC" || _requestStatus == "PE") {
                        CloseWebSocket();
                        if (_requestStatus == "UE" || _requestStatus == "PE") {
                            EnableProgressIndicator(false);
                        }
                        RetrieveFile();
                    }

                    // if the status indicates complete, close the connection
                    if (IsRequestStatusComplete(_requestStatus) == true) {

                        ShowCurrentCompletionLevel();

                        CloseWebSocket();

                     //   EnableProgressIndicator(false);
                    }               
               };

                // closed connection event handler
                _webSocket.onclose = function () {                   
                //    _statusMessage = "Operation complete.";  // it already says export complete, this is redundant and would overwrite any error messages
                 //   ShowCurrentStatusMessage();
                };

                // error event handler
                _webSocket.onerror = function (e) {            
                    _statusMessage = "Socket error: " + e.message;
                    ShowCurrentStatusMessage();
                };               
           }
           else {  // current socket ok to use

               // next message to send was deposited by server              
               var statusRequestMessage = document.getElementById("StatusRequestMessage").value;

               // if the websocket is open, send the message
               if (_webSocket.OPEN && _webSocket.readyState == 1) {
                   _webSocket.send(statusRequestMessage);
               }

               // if the websocket is closed, show a closed message
               if (_webSocket.readyState == 2 || _webSocket.readyState == 3) {
                   _statusMessage = "Connection unexpectedly closed. Please try again.";
                   ShowCurrentStatusMessage();
               }
           }
       }

      

   </script>



<asp:Label ID="ContractNumberLabelLabel" runat="server" Font-Names="Arial" Font-Size="10pt"  Style="z-index: 103; left: 89px;
    position: absolute; top: 62px" Text="Contract Number"></asp:Label>

<asp:Label ID="ContractNumberLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 104;
    left: 210px; position: absolute; top: 62px" ></asp:Label>

<asp:Panel ID="PharmaceuticalExportParametersPanel" runat="server" Font-Names="Arial" Font-Size="10pt" BackColor="AliceBlue" Style="z-index: 105;
    left: 30px; position: absolute; top: 76px"  ></asp:Panel>

<asp:Label ID="StartDateLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Text="Start Date" Style="z-index: 106;
    left: 90px; position: absolute; top: 93px; height: 16px; width: 71px;" 
    ToolTip="Start date for export selection." ></asp:Label>

<asp:TextBox ID="StartDateTextBox" runat="server" Font-Names="Arial" Font-Size="10pt" AutoPostBack="True" Style="z-index: 107;
    left: 181px; position: absolute; top: 91px; width: 100px; height: 20px;" 
    ToolTip="Enter start date for export selection." ></asp:TextBox>

<asp:RadioButton ID="ExportCoveredOnly" Text="Covered Only" runat="server" Font-Names="Arial"  Font-Size="10pt"
    GroupName="PharmExportRadioButtonGroup" AutoPostBack="false" 
    
    style="z-index: 123;
    left: 44px;  width: 117px; position: absolute; top: 153px; margin-left: 0px; height: 22px;"    />

<asp:TextBox ID="EndDateTextBox" runat="server" Font-Names="Arial" Font-Size="10pt" AutoPostBack="True" Style="z-index: 117;
    left: 181px; position: absolute; top: 122px; height: 20px; width: 100px;" 
    ToolTip="Enter end date for export selection." ></asp:TextBox>
 
<asp:Button ID="ExportToSpreadsheetButton"  ClientIDMode="static" CausesValidation="false" runat="server" Font-Names="Arial" Font-Size="10pt" OnClick="ExportToSpreadsheetButton_Click"  Style="z-index: 125;
    left: 112px; position: absolute; top: 182px; margin-left: 0px;" Width="155px" Text="Export To Spreadsheet"  
        
    ToolTip="Export the selected contract to an excel spreadsheet." />

<asp:RadioButton ID="ExportBoth" Text="Include Non-Covered" runat="server"  Font-Names="Arial" Font-Size="10pt"
    GroupName="PharmExportRadioButtonGroup"  AutoPostBack="false"
    
    style="z-index: 124;
    left: 188px;  width: 165px; position: absolute; top: 153px; margin-left: 0px; height: 21px;"    />

<asp:Label ID="ExportTitleLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Font-Bold="True" Height="16px" Style="z-index: 118;
    left: 120px; position: absolute; top: 35px; width: 165px;" 
    Text="Export Items To Excel"></asp:Label>

<asp:Label ID="ItemExportUploadLabel" runat="server" Font-Names="Arial" Font-Size="12pt" Style="z-index: 119;
    left: 86px; position: absolute; top: 0px; width: 237px;" 
    Text="Item Pricelist Export/Upload" Font-Bold="True"></asp:Label>

<hr style="z-index: 128; left: 2px; width: 376px; position: absolute; top: 18px; color: aqua; height: -12px;" />
<hr style="z-index: 129; left: 2px; width: 376px; position: absolute; top: 210px; color: aqua;" />

<asp:Label ID="UploadItemsLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Font-Bold="True" Style="z-index: 120;
    left: 106px; position: absolute; top: 228px" Text="Upload Items To NACCM" 
    Width="184px"></asp:Label>

<asp:Label ID="ModificationNumberLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 126;
    left: 67px; position: absolute; top: 257px; height: 21px; width: 157px;" 
    Text="Modification Number"></asp:Label>

<asp:TextBox ID="ModificationNumberTextBox" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 127;
    left: 229px; position: absolute; top: 255px; height: 20px; width: 87px;"></asp:TextBox>

<asp:Button ID="UploadButton" runat="server" Font-Names="Arial"  ClientIDMode="static" OnClick="UploadButton_OnClick" Style="z-index: 102;
    left: 155px; position: absolute; top: 374px" Width="64px" Height="25px" Text="Upload"  
    
    
    ToolTip="Upload the selected spreadsheet. Note that only spreadsheets created via export may be uploaded." />

<asp:Label ID="UploadStatusLabelLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Font-Bold="True" Style="z-index: 121;
    left: 5px; position: absolute; top: 422px" Text="Status:" Width="48px"></asp:Label>

<asp:Label ID="UploadStatusLabel1" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 122;
    left: 8px; position: absolute; top: 448px; height: 22px; width: 366px;" 
    ToolTip="Indicates status of last request" ></asp:Label>

<asp:Label ID="UploadStatusLabel2" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 123;
    left: 8px; position: absolute; top: 475px; height: 70px; width: 366px; text-wrap:normal;" 
    ToolTip="Indicates any errors from the last request" ></asp:Label>

<asp:Label ID="UploadStatusLabel3" runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 124;
    left: 8px; position: absolute; top: 549px; height: 62px; width: 366px; " 
    ToolTip="Indicates percent complete for the last request" ></asp:Label>

<div id="ProgressIndicator" runat="server" clientidmode="static"  Style="z-index: 124; left: 8px; position: absolute; top: 574px; height: 26px; width: 366px; background-color:transparent;"  >
    <asp:Table ID="ProgressTable" runat="server" style="position: relative; width:360px; top:2px; left:2px; padding:0px; margin:0px; border-spacing:0px 2px;">
        <asp:TableRow ID="ProgressRow" runat="server" style="text-align:left; border-width:0px; padding:0px; width:360px;">
            <asp:TableCell ID="ProgressBar" runat="server" >
                <asp:Label ID="ProgressTextLabel" runat="server"  Font-Names="Arial" Width="100%" Font-Size="10pt" Font-Bold="true" Text="" ></asp:Label> </asp:TableCell>
            <asp:TableCell ID="UnusedBar" runat="server" style="background-color:transparent; height:20px; width:100%; border-width:0px; padding:0px; margin:0px;"></asp:TableCell>
        </asp:TableRow>
    </asp:Table>   
</div>

<hr style="z-index: 130; left: 2px; width: 376px; position: absolute; top: 402px; color: aqua; height: 0px;" />



<asp:FileUpload ID="FileUpload1" ViewStateMode="Enabled" EnableViewState="true" AllowMultiple="false"
    runat="server" Font-Names="Arial" Font-Size="10pt" Style="z-index: 100; left: 48px;
    position: absolute; top: 311px; width: 302px; height: 23px; bottom: 75px;"   
    ToolTip="Browse to the item spreadsheet you wish to upload" BackColor="White" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px"  />


    
<div id="hiddenDiv"  style="display:none;" >
    <asp:HiddenField id="UploadErrorFileNameId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField id="PreviousUploadProceedAnyway" runat="server" ClientIDMode="Static" />
    <asp:HiddenField id="PreviousUploadProceedAnywayShowProgress" runat="server" ClientIDMode="Static" />
    <asp:HiddenField id="IsAuthorizedForExport" runat="server" ClientIDMode="Static" />
</div>

<asp:Label ID="EndDateLabel" runat="server" Font-Names="Arial" Font-Size="10pt" Text="End Date" Style="z-index: 116;
    left: 91px; position: absolute; top: 125px; height: 18px; width: 62px;" 
    ToolTip="End date for export selection." ></asp:Label>



<asp:RegularExpressionValidator ID="FileNameRegularExpressionValidator" runat="server" Font-Names="Arial" Font-Size="10pt"
    ControlToValidate="FileUpload1" ErrorMessage="Only Excel spreadsheets ( *.xls(x) ) may be uploaded."
    Style="z-index: 101; left: 31px; position: absolute; top: 342px" 
    ValidationExpression="^[\\\w0-9\-_\s].*(.xls|.XLS|.xlsx|.XLSX)$">
                    </asp:RegularExpressionValidator>

<asp:RequiredFieldValidator ID="ModificationNumberValidator" runat="server" Font-Names="Arial" Font-Size="10pt"
    ControlToValidate="ModificationNumberTextBox" ErrorMessage="Modification number is required."
     Style="z-index: 101; left: 87px; position: absolute; top: 282px;"  ></asp:RequiredFieldValidator>

