using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace winFormClient
{
    public delegate void MessageDelegate(String message);

    internal class SocketClient
    {
        private String ip;
        private int port;
        private String fileName;
        private String localFilePath;

        private readonly String okToken = "<|OK|>";
        private readonly String fileToken = "<|FILE|>";
        private readonly String getToken = "<|GET|>";
        private readonly String eofToken = "<|EOF|>";
        
        public SocketClient(string ip, int port, string fileName, string localFilePath)
        {
            this.ip = ip;
            this.port = port;
            this.fileName = fileName;
            this.localFilePath = localFilePath;
        }

        private IPEndPoint createIPEndPoint(String ip, int port) {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ip, out ipAddress)) {
                throw new FormatException("Invalid ip address");
            }

            return new IPEndPoint(ipAddress, port);
        }

        private Socket socket = null;


        public async void startRequest(MessageDelegate msgCallback) {

            if (this.socket == null) { 
                this.socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            Socket client = this.socket;

            IPEndPoint endPoint = createIPEndPoint(this.ip, this.port);
            await client.ConnectAsync(endPoint);

            // Stage 1, Request server file
            var sendOk = await this.sendMessage(client, fileToken, this.fileName, true, msgCallback);
            if (sendOk == false) {
                return;
            }

            // Stage 2, Download file data
            await this.downloadFile(client, msgCallback);

            client.Shutdown(SocketShutdown.Both);
            client.Disconnect(true);
        }
        
        private async Task<Boolean> sendMessage(Socket client, String token, String msg, Boolean chkResp, MessageDelegate msgCallback) { 
            var buffer = new byte[1024];
            
            var sendMessage = token + msg;
            var bytes = Encoding.UTF8.GetBytes(sendMessage);
            var sentBytes = await client.SendAsync(bytes, SocketFlags.None);
            
            if (sentBytes <= 0) {
                msgCallback("[ERROR] No byte sent to server.");
                return false;            
            }

            msgCallback("[MSG] Socket client send message: " + sendMessage);

            if (chkResp == false) {
                return true;
            }

            var respBuf = new byte[1024];
            var received = await client.ReceiveAsync(respBuf, SocketFlags.None);
            var receivedMsg = Encoding.UTF8.GetString(respBuf, 0, received);
            if (receivedMsg.IndexOf(okToken) > -1) {
                msgCallback("[MSG] Server response OK. " + receivedMsg);
                return true;
            }

            msgCallback("[ERROR] Server response error. " + receivedMsg);
            return false;
        }

        private async Task<Boolean> downloadFile(Socket client, MessageDelegate msgCallback) {
            msgCallback("[MSG] Prepare to receive data");

            String fName = Path.GetFileName(this.fileName);
            String destPath = Path.Combine(this.localFilePath, fName);

            this.sendMessage(client, getToken, this.fileName, false, msgCallback);

            using FileStream destStream = new(destPath, FileMode.Create);

            int totalReceived = 0;

            do
            {
                try
                {
                    var fileBuf = new byte[2048];
                    var fileReceived = await client.ReceiveAsync(fileBuf, SocketFlags.None);
                    var receivdMsg = Encoding.UTF8.GetString(fileBuf, 0, fileReceived);

                    if (receivdMsg != null && receivdMsg.Equals(eofToken)) {
                        msgCallback("[MSG] Receive EOF Token.");
                        break;
                    }
                    totalReceived += fileReceived;

                    if (fileReceived > 0)
                    {
                        destStream.Write(fileBuf, 0, fileReceived);
                    }
                }
                catch (Exception ex)
                {
                    msgCallback("[MSG] Failed to download file. Exception. " + ex.Message);
                    return false;
                }
            } while (true);
            
            msgCallback("[MSG] Success to download file. total #bytes: " + totalReceived.ToString());
            return true;
        }
    }
}
