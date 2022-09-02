using System.Diagnostics;
using winFormDemo;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SocketServer server = new SocketServer(8081);
            Task task = Task.Run(() => server.startServer(ServerMsgDelegate, ClientInfoDelegate));

            SocketClient client = new SocketClient("127.0.0.1", 8081, "D:/Temp/test.txt", "D:/Temp/b/");
            client.startRequest(ClientMsgDelegate);
        }

        public void ServerMsgDelegate(String msg) {
            Debug.WriteLine("[SERV]" + msg);
        }
        public void ClientInfoDelegate(TextType type, String msg) {
            Debug.WriteLine("type: " + type + ", msg: " + msg);
        }

        public void ClientMsgDelegate(String msg) {
            Debug.WriteLine("[Client]" + msg);
        }
    }
}