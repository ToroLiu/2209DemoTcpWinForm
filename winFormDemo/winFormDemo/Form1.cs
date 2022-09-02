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
            winFormServer.FormServer form = new winFormServer.FormServer();
            form.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            winFormClient.FormClient form = new winFormClient.FormClient();
            form.Show();
        }
    }
}