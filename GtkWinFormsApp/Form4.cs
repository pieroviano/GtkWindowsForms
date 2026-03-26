using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GtkWinFormsApp
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.Shown += Form4_Shown;

            button4.Click += Button4_Click;
        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        Point panel1Location = new Point();
        private void Form4_Shown(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Properties.GtkWinFormsAppResources.Gtk_Image_files_jpg_png_Image_files_2_bmp;
            ofd.Multiselect = true;
            ofd.Title = Properties.GtkWinFormsAppResources.Gtk_Click_Test_Open_File;
            ofd.DefaultExt = "*.png";
            DialogResult dialogResult = ofd.ShowDialog(this);
            GtkLogger.Info("dialogResult:" + dialogResult.ToString());
            GtkLogger.Info("FileName:" + ofd.FileName);
            foreach (string file in ofd.FileNames)
            {
                GtkLogger.Info("FileNames:" + file);
            }
            GtkLogger.Info("SafeFileName:" + ofd.SafeFileName);
            foreach (string file in ofd.SafeFileNames)
            {
                GtkLogger.Info("SafeFileNames:" + file);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = Properties.GtkWinFormsAppResources.Gtk_Image_files_jpg_png_Image_files_2_bmp;
            ofd.Title = Properties.GtkWinFormsAppResources.Gtk_Click_Test_Save_File;

            DialogResult dialogResult = ofd.ShowDialog();
            GtkLogger.Info("dialogResult:" + dialogResult.ToString());
            GtkLogger.Info("FileName:" + ofd.FileName);
            foreach (string file in ofd.FileNames)
            {
                GtkLogger.Info("FileNames:" + file);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.Description = Properties.GtkWinFormsAppResources.Gtk_Click_Browse_Folder_Description;
            DialogResult dialogResult = ofd.ShowDialog();
            GtkLogger.Info("dialogResult:" + dialogResult.ToString());
            GtkLogger.Info("SelectedPath:" + ofd.SelectedPath);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
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

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
