using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GtkWinFormsApp;

[Designer(typeof(ControlDesigner))]
public partial class UserControl11 : UserControl
{
    public UserControl11()
    {
        Size = new System.Drawing.Size(144, 131);
        InitializeComponent();
        //this.MouseWheel += UserControl11_MouseWheel;
        PictureBox pictureBox2 = new PictureBox();
        pictureBox2.BackgroundImage = Properties.Resources.timg;
        pictureBox2.Location = new System.Drawing.Point(9, 3);
        pictureBox2.Name = "pictureBox1";
        pictureBox2.Size = new System.Drawing.Size(72, 40);
        pictureBox2.TabIndex = 1;
        pictureBox2.TabStop = false;
        this.Controls.Add(pictureBox2);
        //this.Show();
        //this.Invalidate();
    }

    private void UserControl11_MouseWheel(object sender, MouseEventArgs e)
    {
        GtkLogger.Info($"UserControl11_MouseWheel {e.Button},{e.Clicks},({e.X},{e.Y})");
    }

    private void UserControl11_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g?.DrawEllipse(new(new SolidBrush(Color.Red), 2), 0, 0, 30, 20);
        //g.FillEllipse(new SolidBrush(Color.Red), 40, 25, 30, 20);
    }

    private void UserControl11_ParentChanged(object sender, EventArgs e)
    {
    }

    private void UserControl11_Load(object sender, EventArgs e)
    {

    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {

        //using SolidBrush brush = new SolidBrush(_backgroundColor);
        //e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
        //using SolidBrush brush2 = new SolidBrush(Color.Red);
        //e.Graphics.DrawEllipse(new Pen(brush2), new Rectangle(30, 30, Width, Height));
        base.OnPaint(e);
    }

    private void UserControl11_MouseEnter(object sender, EventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseEnter");
    }

    private void UserControl11_MouseLeave(object sender, EventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseLeave");
    }

    private void UserControl11_MouseMove(object sender, MouseEventArgs e)
    {
        //GtkLogger.Info("UserControl11_MouseMove");
    }

    private void UserControl11_MouseUp(object sender, MouseEventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseUp");
    }

    private void UserControl11_MouseHover(object sender, EventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseHover");
    }

    private void UserControl11_MouseDown(object sender, MouseEventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseDown");
    }

    private void UserControl11_Click(object sender, EventArgs e)
    {
        GtkLogger.Info("UserControl11_Click");
    }

    private void UserControl11_DoubleClick(object sender, EventArgs e)
    {
        GtkLogger.Info("UserControl11_DoubleClick");
    }

    private void UserControl11_MouseClick(object sender, MouseEventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseClick");
    }

    private void UserControl11_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        GtkLogger.Info("UserControl11_MouseDoubleClick");
    }

    private void UserControl11_Scroll(object sender, ScrollEventArgs e)
    {
        GtkLogger.Info($"UserControl11_Scroll:{e.OldValue},{e.NewValue};{e.ScrollOrientation}");
    }
}
