using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

using VA.NAC.ItemExportUploadBrowser;
using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    /// <summary>
    /// Summary description for ExportUploadHttpHandler
    /// </summary>
    public class ExportUploadHttpHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            // if the query is WebSocket request
            if( context.IsWebSocketRequest ) 
            {
                // attach the asynchronous handler function
                context.AcceptWebSocketRequest( ExportUploadHttpHandlerFunction );
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        // asynchronous request handler function
        public async Task ExportUploadHttpHandlerFunction( AspNetWebSocketContext webSocketContext )
        {
            WebSocket webSocket = webSocketContext.WebSocket;
          
            const int maxMessageSize = 1024;

            var receivedDataBuffer = new ArraySegment<Byte>( new Byte[ maxMessageSize ] );

            var cancellationToken = new CancellationToken();

            // while WebSocket is open
            while( webSocket.State == WebSocketState.Open )
            {
                // wait for a message
                WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync( receivedDataBuffer, cancellationToken );

                // if close requested, then close
                if( webSocketReceiveResult.MessageType == WebSocketMessageType.Close )
                {
                    await webSocket.CloseAsync( WebSocketCloseStatus.NormalClosure, String.Empty, cancellationToken );
                }
                else // presume text
                {
                    // copy data to byte array, up to null
                    byte[] payloadData = receivedDataBuffer.Array.Where( b => b != 0 ).ToArray();

                    // convert to string
                    string receiveString = System.Text.Encoding.UTF8.GetString( payloadData, 0, payloadData.Length );

                    // process the message
                    await ProcessMessage( webSocket, receiveString, cancellationToken );
                }
            }
        }

        public async Task ProcessMessage( WebSocket webSocket, string messageString, CancellationToken cancellationToken )
        {
            bool bSuccess = true;
            ExportUploadRequest statusRequest = new ExportUploadRequest();  // status only
            ExportUploadRequest statusResponse = null;

            ExportUploadDatabase exportUploadDB = null;

            try
            {
                statusRequest.PopulateObjectFromMessage( messageString );

                int requestCompletionLevel = statusRequest.CompletionLevel;

                if( statusRequest.RequestType == ExportUploadRequest.RequestTypes.Status )
                {
                 
                    DateTime lastModificationDate = DateTime.MinValue;
                    
                    exportUploadDB = new ExportUploadDatabase();
                    DataSet dsExportUploadRequestStatus = null;
                    int rowCount = 0;

                    bSuccess = exportUploadDB.GetExportUploadRequestStatus( statusRequest.RequestId, ref dsExportUploadRequestStatus, ref rowCount );
                    if( bSuccess  == true )
                    {                       
                        DataTable dtExportUploadRequestStatusTable = dsExportUploadRequestStatus.Tables[ ExportUploadDatabase.ExportUploadRequestStatusTableName ];
                        DataRow row = dtExportUploadRequestStatusTable.Rows[ 0 ];

                        statusResponse = new ExportUploadRequest();
                        statusResponse.PopulateObjectFromRow( row );

                        int newCompletionLevel = statusResponse.CompletionLevel;
 
                    }
                    else  // could not get the status
                    {
                        // repurpose the request
                        statusResponse = new ExportUploadRequest( statusRequest );

                        // respond with error info
                        statusResponse.RequestStatus = ExportUploadRequest.RequestStatuses.StatusFailed;
                        statusResponse.StatusMessage = string.Format( "{0};{1}", statusResponse.StatusMessage, exportUploadDB.ErrorMessage );

                    }
                }                
            }
            catch( Exception ex )
            {
                // repurpose the request
                statusResponse = new ExportUploadRequest( statusRequest );

                // respond with error info
                statusResponse.RequestStatus = ExportUploadRequest.RequestStatuses.RequestFailed;
                statusResponse.StatusMessage = string.Format( "{0};{1}", statusResponse.StatusMessage, ex.Message );                
            }

            // send a response
            if( statusResponse != null )
            {
                await SendMessage( webSocket, statusResponse.GetMessageStringFromObject(), cancellationToken );
            }
        }


        public async Task SendMessage( WebSocket webSocket, string messageString, CancellationToken cancellationToken )
        {
            // create response as a byte array
            var newString = messageString;
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes( newString );

            // send response to client
            await webSocket.SendAsync( new ArraySegment<byte>( bytes ), WebSocketMessageType.Text, true, cancellationToken );
        }
    }
}