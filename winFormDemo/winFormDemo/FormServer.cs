using System.Diagnostics;
using winFormDemo;

namespace DemoServer
{
    public partial class FormServer : Form
    {
        private SocketServer server;

        public FormServer()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.server = new SocketServer();
            await Task.Run(() => this.server.startServer(this.messageCallback, this.clientCallback));
        }

        public void messageCallback(String message) {

            Debug.WriteLine("[SERV]" + message);
        }
        public void clientCallback(TextType type, String text) {
            switch (type) {
                case TextType.ClientIp:
                    this.updateText(this.textBox_ClientIP, text);
                    
                    break;

                case TextType.ClientPort:
                    this.updateText(this.textBox_Port, text);
                    break;

                case TextType.RequestFile:
                    this.updateText(this.textBox_RequestFile, text);
                    break;
            }
        }

        private void updateText(TextBox textBox, String text) {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke((MethodInvoker)(() => {
                    textBox.Text = text;
                }));
            }
            else {
                textBox.Text = text;
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.server.stopServer();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Server Form Closing.");
            this.server.stopServer();
        }
    }
}