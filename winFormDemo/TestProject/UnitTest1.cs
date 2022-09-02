using System.Diagnostics;
using winFormDemo;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDownloadFiles()
        {
            SocketServer server = new SocketServer(8081);
            Task taskServ = Task.Run(() => server.startServer(ServerMsgDelegate, ClientInfoDelegate));

            SocketClient client = new SocketClient("127.0.0.1", 8081);
            Task task1 = Task.Run(() => client.startRequest("D:/Temp/test.txt", "D:/Temp/b/", ClientMsgDelegate));
            Task.WaitAll(task1);

            SocketClient client2 = new SocketClient("127.0.0.1", 8081);
            Task task2 = Task.Run(() => client2.startRequest("D:/Temp/shana.jpg", "D:/Temp/b/", ClientMsgDelegate));
            Task.WaitAll(task2);
        }

        [TestMethod]
        public void TestFileNotExist() {

            SocketServer server = new SocketServer(8082);
            Task task = Task.Run(() => server.startServer(ServerMsgDelegate, ClientInfoDelegate));

            SocketClient client = new SocketClient("127.0.0.1", 8082);
            Task task1 = Task.Run(() => client.startRequest("D:/Temp/no_exist.txt", "D:/Temp/b/", ClientMsgDelegate));

            Task.WaitAll(task1);
        }

        public void ServerMsgDelegate(String msg) {
            Debug.WriteLine("[SERV]" + msg);
        }
        public void ClientInfoDelegate(TextType type, String msg) {
            Debug.WriteLine("[SERV] type: " + type + ", msg: " + msg);
        }

        public void ClientMsgDelegate(String msg) {
            Debug.WriteLine("[Client]" + msg);
        }
    }
}