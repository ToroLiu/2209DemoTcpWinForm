using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace winFormDemo
{
    public delegate void MessageDelegate(String message);

    public class SocketClient
    {
        private String ip;
        private int port;
        
        private readonly String okToken = "<|OK|>";
        private readonly String fileToken = "<|FILE|>";
        private readonly String getToken = "<|GET|>";
        private readonly String eofToken = "<|EOF|>";
        private readonly String msgToken = "<|MSG|>";

        public SocketClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        private IPEndPoint createIPEndPoint(String ip, int port) {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ip, out ipAddress)) {
                throw new FormatException("Invalid ip address");
            }

            return new IPEndPoint(ipAddress, port);
        }


        public async void startRequest(String fileName, String localFilePath, MessageDelegate msgCallback) {
      
            Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPEndPoint endPoint = createIPEndPoint(this.ip, this.port);
                await client.ConnectAsync(endPoint);

                // Stage 1, Check file exist. Report error if file not found. 
                var sendOk = await this.sendMessage(client, fileToken, fileName, true, msgCallback);
                if (sendOk == true)
                {
                    // Stage 2, Download file data
                    _ = await this.downloadFile(client, fileName, localFilePath, msgCallback);

                }

                // Stage 3, Notify Server EOF
                _ = await this.sendMessage(client, eofToken, "", false, msgCallback);

                client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                msgCallback("[ERROR] Exception: " + e.Message);
            }
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

            msgCallback("[Info] Socket client send message: " + sendMessage);

            if (chkResp == false) {
                return true;
            }

            var respBuf = new byte[1024];
            var received = await client.ReceiveAsync(respBuf, SocketFlags.None);
            var receivedMsg = Encoding.UTF8.GetString(respBuf, 0, received);
            if (receivedMsg.IndexOf(okToken) > -1) {
                msgCallback("[Info] Server response OK. " + receivedMsg);
                return true;
            }

            msgCallback("[ERROR] Server response error. " + receivedMsg);
            return false;
        }

        private async Task<Boolean> downloadFile(Socket client, String fileName, String localFilePath,  MessageDelegate msgCallback) {
            
            String fName = Path.GetFileName(fileName);
            String destPath = Path.Combine(localFilePath, fName);

            msgCallback("[Info] Prepare to download file: " + fName);

            _ = await this.sendMessage(client, getToken, fileName, false, msgCallback);

            using FileStream destStream = new(destPath, FileMode.Create);

            int totalReceived = 0;

            do
            {
                try
                {
                    var fileBuf = new byte[2048];
                    var fileReceived = await client.ReceiveAsync(fileBuf, SocketFlags.None);
                    var receivdMsg = Encoding.UTF8.GetString(fileBuf, 0, fileReceived);

                    if (receivdMsg != null && receivdMsg.Equals(okToken)) {
                        msgCallback("[Info] Receive OK Token. End of downloading");
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
                    msgCallback("[Info] Failed to download file. Exception. " + ex.Message);
                    return false;
                }
            } while (true);
            
            msgCallback("[Info] Success to download file. total #bytes: " + totalReceived.ToString());
            return true;
        }
    }
}
