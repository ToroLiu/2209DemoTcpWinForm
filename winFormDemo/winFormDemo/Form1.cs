namespace winFormDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            DemoServer.FormServer form = new DemoServer.FormServer();
            form.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            DemoClient.FormClient form = new DemoClient.FormClient();
            form.Show();
        }
    }
}