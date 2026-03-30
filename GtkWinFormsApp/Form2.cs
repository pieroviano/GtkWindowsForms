using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GtkWinFormsApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            // listView1.Items.Add(new ListViewItem("test1", new ListViewGroup("ListViewGroup1", "ListViewGroup1")) { });

          var dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Rows.Add("user1", DateTime.Now);
            dt.Rows.Add("user2", DateTime.Now.AddDays(1));
            dt.Rows.Add("user3", DateTime.Now.AddDays(2));
            dt.Rows.Add("user4", DateTime.Now.AddDays(3));
            //Dictionary<int, string> dic = new Dictionary<int, string>();
            //dic.Add(0, "111");
            //dic.Add(3, "333");
            //List<string> list = new List<string>();
            //list.Add("ddd");
            //list.Add("rrrr");
            //Hashtable hashtable = new Hashtable();
            //hashtable.Add("11", "ddd");
            //hashtable.Add("22", "rrrr");

            listBox1.DisplayMember = "CreateDate";
            listBox1.DataSource = dt;

            this.FormClosing += Form2_FormClosing;
            this.FormClosed += Form2_FormClosed;
            listView1.MouseDown += ListView1_MouseDown;
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            var item = listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                GtkLogger.Info(item.Text);
            }
        }

        int i = 4;
        private void Timer1_Tick(object sender, System.EventArgs e)
        {
            i++;
            listBox1.Items.Add(string.Format(GtkWinFormsApp.Properties.GtkWinFormsAppResources.Gtk_Exception_warning_i_Data_center_air_conditioning_operation_monitoring_event_DateTime_Now_Ticks_DateTime_Now_ToString, i, DateTime.Now.Ticks, DateTime.Now.ToString()));
            listBox1.TopIndex = i;
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Form2_FormClosed");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            var res = MessageBox.Show("Form2_FormClosing");
            e.Cancel = res != DialogResult.OK;
        }
        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            MessageBox.Show($"ItemCheck:{e.NewValue.ToString()},{e.CurrentValue.ToString()}");
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            richTextBox1.Text = "";
            foreach (var m in listView1.CheckedItems)
                richTextBox1.Text += $"{m.Text},Selected:{m.Selected},Checked:{m.Checked}；\n";
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            MessageBox.Show($"ItemSelectionChanged:{e.Item.Text},{e.Item.Selected}");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            foreach (var m in listView1.SelectedItems)
                richTextBox1.Text += $"{m.Text},Selected:{m.Selected},Checked:{m.Checked}；\n";
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            MessageBox.Show($"ColumnClick:{e.Column}");
        }

        private void listView1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //listView1.Clear();
            listView1.Groups.Add("listViewGroup11", "listViewGroup11");
            listView1.Groups.Add("listViewGroup21", "listViewGroup21");
            listView1.Items.Add(new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_Add_group_and_data_simultaneously) { ForeColor = Color.Red, BackColor = Color.Yellow, Group = listView1.Groups[0] });
            listView1.Items.Add(new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_Add_data_to_a_specified_group) { ForeColor = Color.Red, BackColor = Color.Yellow, Group = listView1.Groups[1] });

            var m = new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_This_is_a_method_to_add_multi_column_data, 0)
                {
                    Checked = true,
                    Selected = true,
                    ForeColor = Color.Green,
                    BackColor = Color.Yellow
                };
            m.SubItems.Add(new ListViewItem.ListViewSubItem(m, Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_1, Color.HotPink, Color.Gray, new Font(FontFamily.GenericSansSerif, 16)));
            m.SubItems.Add(new ListViewItem.ListViewSubItem(m, Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_2));
            m.SubItems.Add(new ListViewItem.ListViewSubItem(m, Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_3_Sub_column_data_3_Sub_column_data_3));
            m.SubItems.Add(new ListViewItem.ListViewSubItem(m, Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_1));
            listView1.Items.Add(m);
        }

        private void listView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text += $"{listBox1.SelectedIndex}-{listBox1.SelectedValue}\n";
        }
    }
} 
