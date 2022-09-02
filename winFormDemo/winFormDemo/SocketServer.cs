using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace winFormServer
{
    public delegate void MessageCallback(String message);
    public enum TextType { 
        ClientIp, ClientPort, RequestFile
    }
    public delegate void ClientCallback(TextType type, String text);

    internal class SocketServer
    {
        private String clientIP;
        private int clientPort;
        private String requstFile;

        private readonly int serverPort = 8080;

        private readonly String okToken = "<|OK|>";
        private readonly String fileToken = "<|FILE|>";
        private readonly String getToken = "<|GET|>";
        private readonly String eofToken = "<|EOF|>";

        private Boolean working = true;
        public SocketServer() { }

        public void stopServer() {
            this.working = false;
        }
       
        public async void startServer(MessageCallback msgCallback, ClientCallback clientCallback) { 
          
            using Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
            listener.Bind(endPoint);
            listener.Listen(100);

            msgCallback("[MSG] Server start working.");

            var handler = await listener.AcceptAsync();
            var clientEndPoint = handler.RemoteEndPoint;
            if (clientEndPoint != null) {
                IPEndPoint clientEP = (IPEndPoint)clientEndPoint;
                this.clientIP = clientEP.Address.ToString();
                this.clientPort = clientEP.Port;

                clientCallback(TextType.ClientIp, this.clientIP);
                clientCallback(TextType.ClientPort, this.clientPort.ToString());
            }
            msgCallback("[MSG] Client connect ...");

            while (this.working) {
                var buffer = new byte[1024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var receivedMsg = Encoding.UTF8.GetString(buffer, 0, received);

                //msgCallback("[MSG] Receive message: " + receivedMsg);

                if (receivedMsg.StartsWith(fileToken))
                {

                    String filePath = receivedMsg.Replace(fileToken, "");

                    this.requstFile = filePath;
                    clientCallback(TextType.RequestFile, filePath);

                    checkFileExist(handler, filePath, msgCallback);

                }
                else if (receivedMsg.StartsWith(getToken))
                {
                    // Prepare to send file bytes
                    String filePath = receivedMsg.Replace(getToken, "");
                    sendFile(handler, filePath, msgCallback);
                }
                else if (receivedMsg.Equals("")) {
                    Thread.Sleep(300);
                } else { 
                    msgCallback("[ERROR] Invalid client request");
                }
            }

            msgCallback("[MSG] Server stop working.");
        }
        private Boolean checkFileExist(Socket handler, String filePath, MessageCallback msgCallback) {
            
            Boolean isFileExist = File.Exists(filePath);
            if (isFileExist == false)
            {
                this.sendResponse(handler, "[ERROR] Request file not found.");
                msgCallback("[ERROR] Request file not found.");
                return false;
            }

            // File exists, get file bytes, and sent to client.
            this.sendResponse(handler, okToken);
            return true;
        }

        private Boolean sendFile(Socket handler, String filePath, MessageCallback msgCallback)
        {
            using FileStream srcStream = new(filePath, FileMode.Open);

            int totalBytes = 0;
            var fileBuf = new byte[2048];
            int bytesRead;
            while ((bytesRead = srcStream.Read(fileBuf, 0, fileBuf.Length)) > 0)
            {
                var fileSentBytes = handler.Send(fileBuf, SocketFlags.None);
                totalBytes += fileSentBytes;
            }

            msgCallback("[MSG] Success sent file. #bytes: " + totalBytes.ToString());

            this.sendResponse(handler, eofToken);
            return true;
        }

        Boolean sendResponse(Socket socket, String response) {
            var bytes = Encoding.UTF8.GetBytes(response);
            var sentBytes = socket.Send(bytes, SocketFlags.None);

            Boolean sent = (sentBytes > 0);
            Debug.WriteLine("[SERV][MSG] did sendResponse: " + response);

            return sent;
        }
        

    }
}
