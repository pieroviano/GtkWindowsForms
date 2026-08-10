using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkWinFormsApp
{
    public partial class Test1 : Form
    {
        public Test1()
        {
            InitializeComponent();

            ApplyCultureBackground();

            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("project", typeof(string));
            dt.Columns.Add("state", typeof(string));
            dt.Columns.Add("testnum", typeof(string));
            
            for(int i = 0; i < 50; i++)
            {
                dt.Rows.Add(i, $"project{i}", Properties.GtkWinFormsAppResources.Gtk_Test1_17, DateTime.Now.Ticks.ToString().Substring(15));
            }
             
            this.dataGridView1.DataSource = dt;
            dataGridView1.Click += DataGridView1_Click;

            this.FormClosing += Test1_FormClosing;
        }
        
        /// <summary>
        /// The form background (defined in theme/Test1.css) uses Test1_bgimage.png, which bakes Chinese
        /// text into the banner. Under a non-Chinese UI culture, override the CSS with the English
        /// rendering (Test1_bgimage.en.png) so the banner matches the rest of the localized UI. The
        /// override is applied at User priority so it wins over the app's Application-priority theme CSS.
        /// </summary>
        private void ApplyCultureBackground()
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
                    .Equals("zh", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var enImage = System.IO.Path.Combine(
                System.AppContext.BaseDirectory, "theme", "Test1", "Test1_bgimage.en.png");
            if (!System.IO.File.Exists(enImage))
            {
                return;
            }

            // GTK resolves a bare path in url() relative to the working directory; a file:// URI loads
            // the image regardless of where the process was launched from.
            var url = new Uri(enImage).AbsoluteUri;
            var provider = new Gtk.CssProvider();
            provider.LoadFromData($"#Test1 .Form {{ background-image: url(\"{url}\"); }}");
            // Gtk4: Gdk.Screen + AddProviderForScreen removed -> AddProviderForDisplay(Gdk.Display).
            Gtk.StyleContext.AddProviderForDisplay(
                Gdk.Display.Default, provider, (uint)Gtk.StyleProviderPriority.User);
        }

        private void Test1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
 
        private void DataGridView1_Click(object? sender, EventArgs e)
        {
            
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            string format = DateTime.Now.ToString("yyyy-MM-dd (dddd) HH:mm:ss");
            if (label16.Text != format)
            {
                label16.Text = format;
            }
            ticks +=0.005;
            pictureBox_PC1.Refresh();
            pictureBox_CC1.Refresh();
            pictureBox_chart.Refresh();
        }
        double ticks = 0;
        // simulated animation
        private int volatility(int maxval, double ratio) {

            return (int)(Math.Abs(Math.Sin(ticks + ratio)) * maxval);
        }
        
        private void button17_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows[0].Cells[0].Value = DateTime.Now.Second;
            dataGridView1.Refresh();
        }
        private void pictureBox_PC1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = e.ClipRectangle;
            int _val_pc1 = volatility(rect.Height, 1.2);
            e.Graphics!.Clear(Color.White);
            e.Graphics!.FillRectangle(new SolidBrush(Color.Blue), 0, _val_pc1, rect.Width, rect.Height);
        }
        private void pictureBox_CC1_Paint(object sender, PaintEventArgs e)
        {
            int _val_cc1 = volatility(pictureBox_CC1.Height, 2.2);
            e.Graphics!.FillRectangle(new SolidBrush(Color.LightSkyBlue), 0, _val_cc1, pictureBox_CC1.Width, pictureBox_CC1.Height);
        }

        private void pictureBox_PC2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics!.FillRectangle(new SolidBrush(Color.Blue), 0, pictureBox_PC2.Height / 2, pictureBox_PC2.Width, pictureBox_PC2.Height);
        }

        private void pictureBox_CC2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics!.FillRectangle(new SolidBrush(Color.LightSkyBlue), 0, pictureBox_CC2.Height / 2, pictureBox_CC2.Width, pictureBox_CC2.Height);
        }

        private void pictureBox_PC3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics!.FillRectangle(new SolidBrush(Color.LightSkyBlue), 0, pictureBox_PC3.Height / 2, pictureBox_PC3.Width, pictureBox_PC3.Height);
        }

        private void pictureBox_PC4_Paint(object sender, PaintEventArgs e)
        {
            int width = pictureBox_PC4.Width;
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.LightGray), 1), new PointF(0, 20), new PointF(width, 20));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.LightGray), 1), new PointF(0, 40), new PointF(width, 40));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.LightGray), 1), new PointF(0, 60), new PointF(width, 60));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.LightGray), 1), new PointF(0, 80), new PointF(width, 80));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.LightGray), 1), new PointF(0, 100), new PointF(width, 100));

            e.Graphics!.FillRectangle(new SolidBrush(Color.LightSkyBlue), 0, pictureBox_PC4.Height / 2, pictureBox_PC4.Width, pictureBox_PC4.Height);
        }

        private void pictureBox_CC4_Paint(object sender, PaintEventArgs e)
        {
            int width = pictureBox_CC4.Width;
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), new PointF(0,20), new PointF(width, 20));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), new PointF(0, 40), new PointF(width, 40));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), new PointF(0, 60), new PointF(width, 60));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), new PointF(0, 80), new PointF(width, 80));
            e.Graphics!.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), new PointF(0, 100), new PointF(width, 100));

            e.Graphics!.FillRectangle(new SolidBrush(Color.Blue), 0, pictureBox_CC4.Height / 2, pictureBox_CC4.Width, pictureBox_CC4.Height);
        }
        private void pictureBox_chart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics!;
            int radius = 30;
            int radiusdiff = 35;
            int angle = 20;
            int size = pictureBox_chart.Width;
            int val_chart1 = volatility(360,3.6);

            g.FillEllipse(new SolidBrush(Color.AliceBlue), radius, radius, size - radius*2, size - radius * 2);
            // sector
            radius = 30;
            // purple
            g.FillPie(new SolidBrush(Color.Lavender), radius, radius, size - radius * 2, size - radius * 2, -90+ val_chart1, angle);
            radius += radiusdiff;
            // yellow
            g.FillPie(new SolidBrush(Color.LightGoldenrodYellow), radius, radius, size - radius * 2, size - radius * 2, -90 + val_chart1, angle);
            radius += radiusdiff;
            // blue
            g.FillPie(new SolidBrush(Color.CadetBlue), radius, radius, size - radius * 2, size - radius * 2, -90 + val_chart1, angle);

            radius = 30;
            g.DrawEllipse(new Pen(new SolidBrush(Color.DeepSkyBlue),2), radius, radius, size - radius * 2, size - radius * 2);
            radius += radiusdiff;
            g.DrawEllipse(new Pen(new SolidBrush(Color.DeepSkyBlue), 2), radius, radius, size - radius * 2, size - radius * 2);
            radius += radiusdiff;
            g.DrawEllipse(new Pen(new SolidBrush(Color.DeepSkyBlue), 2), radius, radius, size - radius * 2, size - radius * 2);
            radius += radiusdiff;
            g.DrawEllipse(new Pen(new SolidBrush(Color.DeepSkyBlue), 2), radius, radius, size - radius * 2, size - radius * 2);


            radius = 30;
            for (int i = 1; i < 19; i++)
            {    
                g.DrawPie(new Pen(new SolidBrush(Color.DeepSkyBlue),2), radius, radius, size - radius * 2, size - radius * 2, -90, i* angle);
            }

            g.TranslateTransform(size / 2, size / 2);
            g.RotateTransform(-15f);
            for (int i = 1; i < 19; i++)
            {
                g.RotateTransform(20f);
                g.DrawString(i.ToString(), new Font(FontFamily.GenericSerif, 12), new SolidBrush(Color.Black), 12, 0 - (size / 2 - 10));
            }
            g.TranslateTransform(-size / 2, -size / 2);
            radius += 105;
            g.FillEllipse(new SolidBrush(Color.AliceBlue), radius, radius, size - radius * 2, size - radius * 2);
        }
    }
}
