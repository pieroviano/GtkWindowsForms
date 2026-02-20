using System.Windows.Forms;

namespace AutoClosingMessageBoxApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AutoClosingMessageBox.Instance.MessageBoxTimeout = 3000;
            AutoClosingMessageBox.Instance.Show("Auto closing");
            await CloseAgterDelayASync();
        }

        private async Task CloseAgterDelayASync()
        {
            await Task.Delay(100);
            Close();
        }
    }
}
