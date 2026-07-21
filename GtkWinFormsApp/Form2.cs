
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GtkWinFormsApp;

public partial class Form2 : Form
{
    public Form2()
    {
        InitializeComponent();
        // listView1.Items.Add(new ListViewItem("test1", new ListViewGroup("ListViewGroup1", "ListViewGroup1")) { });

        this.FormClosing += Form2_FormClosing;
        this.FormClosed += Form2_FormClosed;
        listView1.MouseDown += ListView1_MouseDown;
        listView1.ItemCheck += ListView1_ItemCheck;
        listView1.ItemChecked += ListView1_ItemChecked;
        listView1.ItemSelectionChanged += ListView1_SelectedIndexChanged;
    }

    private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
    {
        GtkLogger.Info("ListView1_SelectedIndexChanged");
    }

    private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        GtkLogger.Info($"{listView1.CheckedItems.Count}");
    }

    private void ListView1_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        GtkLogger.Info($"{listView1.CheckedItems.Count}");
    }

    private void ListView1_MouseDown(object sender, MouseEventArgs e)
    {
        var item = listView1.GetItemAt(e.X, e.Y);
        if (item != null)
        {
            GtkLogger.Info($"{e.X},{e.Y},{item.Text}");
        }
    }

    int i = 4;
    private void Timer1_Tick(object sender, System.EventArgs e)
    {
        i++;
        listBox1.Items.Add(string.Format(GtkWinFormsApp.Properties.GtkWinFormsAppResources.Gtk_Exception_warning_i_Data_center_air_conditioning_operation_monitoring_event_DateTime_Now_Ticks_DateTime_Now_ToString, i, DateTime.Now.Ticks, DateTime.Now.ToString()));
        listBox1.TopIndex = 2;

        richTextBox1.AppendText(string.Format(GtkWinFormsApp.Properties.GtkWinFormsAppResources.Gtk_Exception_warning_i_Data_center_air_conditioning_operation_monitoring_event_DateTime_Now_Ticks_DateTime_Now_ToString, i, DateTime.Now.Ticks, DateTime.Now.ToString()) + "\n");
        richTextBox1.SelectionStart = richTextBox1.Text.Length;
        // richTextBox1.SelectionLength = 0;
        //richTextBox1.Focus();
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
        GtkLogger.Info("listView1_Click");
    }

    private void button1_Click(object sender, EventArgs e)
    {
        listView1.Groups.Add("listViewGroup11", "listViewGroup11");
        listView1.Groups.Add("listViewGroup21", "listViewGroup21");
        for (var index = 0; index < 20; index++)
        {
            //listView1.Clear();
            listView1.Items.Add(new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_Add_group_and_data_simultaneously) { ForeColor = Color.Red, BackColor = Color.Yellow, Group = listView1.Groups[0] });
            listView1.Items.Add(new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_Add_data_to_a_specified_group) { ForeColor = Color.Red, BackColor = Color.Yellow, Group = listView1.Groups[1] });

            var m = new ListViewItem(Properties.GtkWinFormsAppResources.Gtk_Click_This_is_a_method_to_add_multi_column_data, 0)
            {
                Checked = true,
                Selected = true,
                ForeColor = Color.Green,
                BackColor = Color.Yellow
            };
            if (m.SubItems != null)
            {
                m.SubItems.Add(new ListViewItem.ListViewSubItem(m,
                    Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_1, Color.HotPink, Color.Gray,
                    new(FontFamily.GenericSansSerif, 16)));
                m.SubItems.Add(new ListViewItem.ListViewSubItem(m,
                    Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_2));
                m.SubItems.Add(new ListViewItem.ListViewSubItem(m,
                    Properties.GtkWinFormsAppResources
                        .Gtk_Click_Sub_column_data_3_Sub_column_data_3_Sub_column_data_3));
                m.SubItems.Add(new ListViewItem.ListViewSubItem(m,
                    Properties.GtkWinFormsAppResources.Gtk_Click_Sub_column_data_1));
            }

            listView1.Items.Add(m);
        }
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        richTextBox1.Text += $"{listBox1.SelectedIndex}-{listBox1.SelectedValue}\n";
    }
}