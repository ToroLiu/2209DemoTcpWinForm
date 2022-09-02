using System.Diagnostics;

namespace winFormClient
{
    public partial class FormClient : Form
    {
        private SocketClient socketClient;

        public FormClient()
        {
            InitializeComponent();
        }

        private String curStatusText = "";

        private void btnStart_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnStart Click");

            this.curStatusText = "Start Request";
            this.textBox_ResponseOfServer.Text = curStatusText;

            String ip = this.textBox_ServerIP.Text;
            String port = this.textBox_ServerPort.Text;
            String fileName = this.textBox_FileName.Text;
            String localPath = this.textBox_PathToSave.Text;

            int portNumber = int.Parse(port);

            Debug.WriteLine("ip: " + ip + ", port: " + port + ", fileName: " + fileName + ", localPath: " + localPath);
            this.socketClient = new SocketClient(ip, portNumber, fileName, localPath);

            this.socketClient.startRequest(this.messageDelegate);
        }

        void messageDelegate(String message) {
            Debug.WriteLine("[Client]" + message);

            this.curStatusText += Environment.NewLine + message;
            if (this.textBox_ResponseOfServer.InvokeRequired)
            {
                this.textBox_ResponseOfServer.Invoke(() =>
                {
                    this.textBox_ResponseOfServer.Text = this.curStatusText; 
                });
            }
            else {
                this.textBox_ResponseOfServer.Text = this.curStatusText;
            }
            
        }
    }
}