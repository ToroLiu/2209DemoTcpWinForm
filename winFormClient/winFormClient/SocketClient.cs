using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace winFormClient
{
    internal class SocketClient
    {
        private String ip;
        private int port;
        private String fileName;
        private String localFilePath;

        
        private readonly String okToken = "<|OK|>";
        private readonly String fileToken = "<|FILE|>";
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

        async void startRequest(Action<String> msgCallback) {
            IPEndPoint endPoint = createIPEndPoint(this.ip, this.port);

            using Socket client = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await client.ConnectAsync(endPoint);

            // Stage 1, Request server file
            var sendOk = await this.sendMessage(client, fileToken, this.fileName, msgCallback);
            if (sendOk == false) {
                return;
            }

            // Stage 2, Download file data
            this.downloadFile(client, msgCallback);
        }
        
        private async Task<Boolean> sendMessage(Socket client, String token, String msg, Action<String> msgCallback) { 
            var buffer = new byte[1024];
            
            var sendMessage = token + msg;
            var bytes = Encoding.UTF8.GetBytes(sendMessage);
            var sentBytes = await client.SendAsync(bytes, SocketFlags.None);
            
            if (sentBytes <= 0) {
                msgCallback("[ERROR] No byte sent to server.");
                return false;            
            }

            msgCallback("[MSG] Socket client send message: " + sendMessage);

            var respBuf = new byte[1024];
            var received = await client.ReceiveAsync(respBuf, SocketFlags.None);
            var receivedMsg = Encoding.UTF8.GetString(respBuf, 0, received);
            if (receivedMsg.IndexOf(okToken) > -1) {
                msgCallback("[MSG] Server response OK");
                return true;
            }

            msgCallback("[ERROR] Server response error. " + receivedMsg);
            return false;
        }

        private async void downloadFile(Socket client, Action<String> msgCallback) {
            msgCallback("[MSG] Prepare to receive data");

            String fName = Path.GetFileName(this.fileName);
            String destPath = Path.Combine(this.localFilePath, fName);
            using FileStream destStream = new(destPath, FileMode.Create);

            do
            {
                var fileBuf = new byte[1024];
                var fileReceived = await client.ReceiveAsync(fileBuf, SocketFlags.None);
                if (fileReceived <= 0)
                {
                    break;
                }
                destStream.Write(fileBuf, 0, fileReceived);
            } while (true);

            msgCallback("[MSG] Success to download file.");
        }
    }
}
