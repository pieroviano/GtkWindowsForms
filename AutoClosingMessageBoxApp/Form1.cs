using System.Windows.Forms;

namespace AutoClosingMessageBoxApp;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        AutoClosingMessageBox.Install(500);
        MessageBox.Show("Auto closing");
        Close();
    }

}