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
        MessageBox.ReplaceInstance(3000);
        MessageBox.Show("Auto closing");
        Close();
    }

}