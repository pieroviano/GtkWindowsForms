using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GtkWinFormsApp;

public partial class Form4 : Form
{
    public Form4()
    {
        InitializeComponent();
        this.Shown += Form4_Shown;

        button4.Click += Button4_Click;
        button10.Click += Button10_Click;
    }

    private void Button10_Click(object? sender, EventArgs e)
    {
    }

    private void Button4_Click(object? sender, EventArgs e)
    {
    }

    Point panel1Location = new();
    private void Form4_Shown(object? sender, EventArgs e)
    {
    }

    private void button3_Click(object sender, EventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            //Filter = Properties.GtkWinFormsAppResources.Gtk_Form4_jpg_bang_png1_2_bmp,
            Multiselect = true,
            Title = Properties.GtkWinFormsAppResources.Gtk_Click_Test_Open_File,
            DefaultExt = "*.png"
        };
        var dialogResult = ofd.ShowDialog(this);
        GtkLogger.Info("dialogResult:" + dialogResult.ToString());
        GtkLogger.Info("FileName:" + ofd.FileName);
        foreach (var file in ofd.FileNames ?? [])
        {
            GtkLogger.Info("FileNames:" + file);
        }
        GtkLogger.Info("SafeFileName:" + ofd.SafeFileName);
        foreach (var file in ofd.SafeFileNames)
        {
            GtkLogger.Info("SafeFileNames:" + file);
        }
    }

    private void button6_Click(object sender, EventArgs e)
    {
        var ofd = new SaveFileDialog
        {
            Filter = Properties.GtkWinFormsAppResources.Gtk_Image_files_jpg_png_Image_files_2_bmp,
            Title = Properties.GtkWinFormsAppResources.Gtk_Click_Test_Save_File
        };

        var dialogResult = ofd.ShowDialog();
        GtkLogger.Info("dialogResult:" + dialogResult.ToString());
        GtkLogger.Info("FileName:" + ofd.FileName);
        foreach (var file in ofd.FileNames ?? [])
        {
            GtkLogger.Info("FileNames:" + file);
        }
    }

    private void button7_Click(object sender, EventArgs e)
    {
        var ofd = new FolderBrowserDialog
        {
            Description = Properties.GtkWinFormsAppResources.Gtk_Click_Browse_Folder_Description
        };
        var dialogResult = ofd.ShowDialog();
        GtkLogger.Info("dialogResult:" + dialogResult.ToString());
        GtkLogger.Info("SelectedPath:" + ofd.SelectedPath);
    }

    private void button8_Click(object sender, EventArgs e)
    {
        var colorDialog = new ColorDialog();
        colorDialog.ShowDialog();

        //FontDialog fontDialog = new FontDialog();
        //fontDialog.ShowDialog();

        //Graphics g = CreateGraphics();
        //// g.DrawString("ddddddddd", new Font(FontFamily.GenericSansSerif, 16), new SolidBrush(Color.Red), 0, 0);
        //g.DrawRectangle(new Pen(new SolidBrush(Color.Red),2), new Rectangle(110, 110, 200, 200));

    }

    private void button9_Click(object sender, EventArgs e)
    {
        MessageBox.Show("test message test message test messagetest message test message test message test messagetest message test message test message test messagetest message test message test message test messagetest message test message test message test messagetest message", Properties.GtkWinFormsAppResources.Gtk_Click_Doubt, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        MessageBox.Show("test message test message \ntest messagetest message", Properties.GtkWinFormsAppResources.Gtk_Click_Warn, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
    }
}