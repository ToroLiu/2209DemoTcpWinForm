namespace winFormServer
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

            Console.WriteLine(message);
        }
        public void clientCallback(TextType type, String text) {
            switch (type) {
                case TextType.ClientIp:
                    this.textBox_ClientIP.Text = text;
                    break;

                case TextType.ClientPort:
                    this.textBox_Port.Text = text;
                    break;

                case TextType.RequestFile:
                    this.textBox_RequestFile.Text = text;
                    break;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.server.stopServer();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            this.server.stopServer();
        }
    }
}