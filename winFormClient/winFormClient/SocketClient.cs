using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
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

        private Boolean working = false;

        private readonly String eomToken = "<|EOM|>";
        private readonly String awkToken = "<|AWK|>";
        private readonly String msgToken = "<|MSG|>";

        SocketClient(string ip, int port, string fileName, string localFilePath)
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

        async void startRequest() {
            IPEndPoint endPoint = createIPEndPoint(this.ip, this.port);

            String fName = Path.GetFileName(this.fileName);
            String destPath = Path.Combine(this.localFilePath, fName);
            using FileStream destStream = new(destPath, FileMode.Create);

            using Socket client = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await client.ConnectAsync(endPoint);
            working = true;

            while (working)
            {
                var sendMessage = msgToken + this.fileName + eomToken;
                var sendBytes = Encoding.UTF8.GetBytes(sendMessage);
                _ = await client.SendAsync(sendBytes, SocketFlags.None);
                Console.WriteLine("Socket client sent message: \"{sendMessage}\" ");

                do
                {
                    var buffer = new byte[1024];


                } while (true);
              

            }
        }
    }
}
