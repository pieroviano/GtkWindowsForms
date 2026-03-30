
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace GtkWinFormsApp;

public partial class Form1 : Form
{
    public Form1()
    {

        InitializeComponent();
        this.Load += Form1_Load;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {

        treeView1.Nodes.Clear();
        treeView1.CheckBoxes = true;

        var jsontext = File.ReadAllText( (System.Globalization.CultureInfo.CurrentCulture.Name ?? "").StartsWith("zh")? System.IO.Path.Combine("Properties", "TestData1.json"): System.IO.Path.Combine("Properties", "TestData1.en.json"));
        using (var reader = new FileStream((System.Globalization.CultureInfo.CurrentCulture.Name ?? "").StartsWith("zh") ? System.IO.Path.Combine("Properties", "TestData1.json") : System.IO.Path.Combine("Properties", "TestData1.en.json"), FileMode.Open, FileAccess.Read))
        {
            var dataContractJson = new DataContractJsonSerializer(typeof(List<TestDataMode>));
            var json = dataContractJson.ReadObject(reader) as List<TestDataMode>;
            var childs = GetChild(null, json);
            treeView1.Nodes.AddRange(childs.ToArray());
            var i = 1;
            while (treeView1.Nodes[0].Nodes[2].Nodes.Count < 4)
            {
                treeView1.Nodes[0].Nodes[2].Nodes.Add(new(i.ToString()));
                i++;
            }
            foreach (var child in treeView1.Nodes)
                child.Expand();

            treeView1.Nodes[0].Nodes[2].Nodes[3].Checked = true;
            treeView1.SelectedNode = treeView1.Nodes[0].Nodes[2];
        }
        var tabPage=new TabPage
        {
            Location = new(4, 29),
            Margin = new(4),
            Name = "tabPage3",
            Padding = new(4),
            Size = new(1179, 426),
            TabIndex = 1,
            Text = "test",
            UseVisualStyleBackColor = true
        };

        tabControl1.Controls.Add(tabPage);
    }
    private IEnumerable<TreeNode> GetChild(string? treeID, IEnumerable<TestDataMode>? data)
    {
        var children = new List<TreeNode>();
        var list = data?.Where(w => w.parent == treeID);
        foreach (var d in list ?? [])
        {
            var node = new TreeNode(d.name) { Name = d.treeID };
            var childs = GetChild(d.treeID, data);
            if (childs.Count() > 0)
                node.Nodes.AddRange(childs.ToArray());
            children.Add(node);
        }
        return children;
    }
    public class TestDataMode
    {
        public string? name { get; set; }
        public string? treeID { get; set; }
        public string? parent { get; set; }
        public string? treeName { get; set; }
    }

    private void button1_Click(object sender, EventArgs e)
    {

        GtkLogger.Info(treeView1.SelectedNode?.Text);
        // b.Title = "test2";
        var result = MessageBox.Show(" 1. Load data point yes \n2. Do not load data point no", Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_2, MessageBoxButtons.YesNo);
        if (result == DialogResult.No)
        {
            return;
        }
        //1. Dataset list data source
        var data = new List<TestEntity>();
        var createdate = DateTime.Now;
        data.Add(new() { ID = 0, Title = " load data point yesLoad data\nPoint yesLoad data point yesLoad data point yesLoad data point yes", Info = "sdfdf", State = true, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "face-smile-big", PIC = Image.FromFile("./Resources/timg6.jpg") });
        data.Add(new() { ID = 1, Title = "test2", Info = "yyyy2", State = true, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "", PIC = Image.FromFile("./Resources/timg6.jpg") });
        data.Add(new() { ID = 3, Title = "test3", Info = "ddds", State = false, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "./Resources/BindingNavigator.Delete.ico", PIC = Image.FromFile("./Resources/timg2.jpg") });
        data.Add(new() { ID = 4, Title = "test4", Info = "yyyy", State = true, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "", PIC = Image.FromFile("./Resources/timg2.jpg") });

        data.Add(new() { ID = 5, Title = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_5, Info = "ddds", State = false, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "https://gitlab.gnome.org/uploads/-/system/project/avatar/13319/gi-docgen.png?width=48", PIC = Image.FromFile("./Resources/timg2.jpg") });
        data.Add(new() { ID = 6, Title = "test4", Info = "yyyy", State = true, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "", PIC = Image.FromFile("./Resources/timg2.jpg") });
        for (var i = 0; i < 10; i++)
            data.Add(new() { ID = i + 7, Title = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_5 + i.ToString(), Info = "ddds", State = false, CreateDate = createdate, Operate = Properties.GtkWinFormsAppResources.Gtk_TestDataForm_button1_4, PIC1 = "https://www.baidu.com/img/flexible/logo/pc/result.png?" + i.ToString(), PIC = Image.FromFile("./Resources/timg2.jpg") });


        this.dataGridView1.DataSource = data;
        this.comboBox1.DisplayMember = "Title";
        this.comboBox1.ValueMember = "ID";
        this.comboBox1.DataSource = data;
        // dataGridView1.Columns[1].Visible = false;

        ////2. Datatable data source
        //DataTable dt = new DataTable();
        //dt.Columns.Add("ID", typeof(string));
        //dt.Columns.Add("CreateDate1", typeof(DateTime));
        //dt.Columns.Add("State", typeof(bool));
        //dt.Rows.Add("test1dddd", DateTime.Now, true);
        //dt.Rows.Add("test2", DateTime.Now.AddDays(5), false);
        ////  this.dataGridView1.Columns.Clear();
        //this.dataGridView1.DataSource = dt;
    }
    public class TestEntity
    {
        public int ID { get; set; }
        public string? title;
        public string? Title { get { return title; } set { title = value; } }
        public string? Info { get; set; }
        public bool State { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Operate { get; set; }
        public string? PIC1 { get; set; }
        public Image? PIC { get; set; }
    }


    private void button2_Click(object sender, EventArgs e)
    {
        var column = new DataGridViewTextBoxColumn
        {
            HeaderText = "test1",
            MinimumWidth = 6,
            Name = "test1",
            Width = 225,
            DataPropertyName = "test1"
        };

        dataGridView1.Columns.Add(column);

        var cd = new ColorDialog();
        if (textBox1.Text?.Length >= 6)
        {
            try
            {
                cd.Color = ColorTranslator.FromHtml(textBox1.Text);
            }
            catch { }
        }
        var result = cd.ShowDialog(this);

        if (result == DialogResult.OK)
        {
            textBox1.Text = "#" + cd.Color.Name;
            textBox1.BackColor = ColorTranslator.FromHtml(textBox1.Text);
        }

        //DialogResult result = MessageBox.Show(this, " pop-up test", " informational tips", MessageBoxButtons.OKCancel);
        //GtkLogger.Info(result.ToString());
        //if (result == DialogResult.OK)
        //{
        //    GtkLogger.Info("DialogResult.OK");
        //    label2.Text = " you chose OK";
        //}
        //if (result == DialogResult.Cancel)
        //{
        //    GtkLogger.Info("DialogResult.Cancel");
        //    label2.Text = Properties.GtkWinFormsAppResources.Gtk_You_selected_Cancel;
        //}
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
        var menu = sender as ToolStripItem;
        GtkLogger.Info(menu?.Text ?? string.Empty);
    }

    private void textBox1_Validating(object sender, CancelEventArgs e)
    {
        GtkLogger.Info("textBox1_Validating");
    }

    private void textBox1_Enter(object sender, EventArgs e)
    {
        GtkLogger.Info("textBox1_Enter");
    }

    private void maskedTextBox2_Validated(object sender, EventArgs e)
    {
        GtkLogger.Info("maskedTextBox2_Validated");
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void checkBox2_CheckStateChanged(object sender, EventArgs e)
    {

    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("numericUpDown1_ValueChanged");
    }

    private void radioButton3_CheckedChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("radioButton3_CheckedChanged");
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("radioButton2_CheckedChanged");
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("radioButton1_CheckedChanged");
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("textBox1_TextChanged");
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GtkLogger.Info($"comboBox1_SelectedIndexChanged {comboBox1.SelectedIndex},{comboBox1.SelectedValue},{comboBox1.Text}");
    }

    private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
    {
        var i = comboBox1.SelectedIndex;
        var o = comboBox1.SelectedItem;
        GtkLogger.Info("comboBox1_SelectedValueChanged");
    }

    private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("dateTimePicker1_ValueChanged");
    }

    private void dataGridView1_SelectionChanged(object sender, EventArgs e)
    {
        //6
        //if(dataGridView1.SelectedRows.Count > 0) 
        //    dataGridView1.SelectedRows[0].Cells[3].Value = DateTime.Now;
        GtkLogger.Info("dataGridView1_SelectionChanged");
    }

    private void dataGridView1_MultiSelectChanged(object sender, EventArgs e)
    { //1
        GtkLogger.Info("dataGridView1_MultiSelectChanged");
    }

    private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        GtkLogger.Info("dataGridView1_CellValueChanged");
        if (e.RowIndex > -1)
        {
            var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            GtkLogger.Info($"{cell.Value},{cell.Selected}");
        }
        //foreach (DataGridViewRow row in dataGridView1.Rows)
        //{
        //    GtkLogger.Info(row.Cells[1].Value);
        //}
    }

    private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        //3
        GtkLogger.Info("dataGridView1_CellEnter");
    }

    private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
    {//4
        GtkLogger.Info("dataGridView1_CellLeave");
    }

    private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
        //7
        GtkLogger.Info("dataGridView1_CellValidated");

        GtkLogger.Info(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString());
    }

    private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        //5
        GtkLogger.Info("dataGridView1_CellValidating" + e.FormattedValue);
    }

    private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
        GtkLogger.Info("dataGridView1_RowEnter");
    }

    private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
        GtkLogger.Info("dataGridView1_RowLeave");
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        //2
        GtkLogger.Info("dataGridView1_CellClick");
    }

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("richTextBox1_TextChanged");
    }

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("tabControl1_SelectedIndexChanged");
    }

    private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
    {

        GtkLogger.Info("treeView1_BeforeSelect：" + e.Node?.Text);
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
        GtkLogger.Info("treeView1_AfterSelect：" + treeView1.SelectedNode?.FullPath);
        GtkLogger.Info("treeView1_AfterSelect：" + e.Node?.Text);
    }

    private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
    {

    }

    private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
    {

    }

    private void test2ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("test2ToolStripMenuItem_CheckedChanged");
    }

    private void test2ToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
    {
        GtkLogger.Info("test2ToolStripMenuItem_CheckedChanged");
    }

    private void test2ToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        GtkLogger.Info("test2ToolStripMenuItem_DropDownItemClicked");
    }

    private void toolStripMenuItem3_Click(object sender, EventArgs e)
    {
        GtkLogger.Info("toolStripMenuItem3_Click");
    }

    private void toolStripMenuItem3_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        GtkLogger.Info("toolStripMenuItem3_DropDownItemClicked");
    }

    private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
    {
        GtkLogger.Info($"checkedListBox1_SelectedValueChanged:{sender}");
    }

    private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        (sender as CheckBox)?.Text = "1234";
        checkedListBox1.Items[0] = DateTime.Now.ToString(CultureInfo.CurrentUICulture);
        GtkLogger.Info($"checkedListBox1_ItemCheck，{sender}: newvalue:{e.NewValue}-oldvalue:{e.CurrentValue}");
        if (e.Index == 2)
        {
            checkedListBox1.SetItemChecked(3, true);
            foreach (var o in checkedListBox1.CheckedItems)
            {
                GtkLogger.Info("ItemCheck，" + o.ToString());
            }
        }
    }

    private void pictureBox2_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g?.Clear(Color.White);

        if (GtkWinFormsApp.Properties.Resources.timg6 != null)
        {
            var mem = new MemoryStream(GtkWinFormsApp.Properties.Resources.timg6);

            //g.DrawImage(new Bitmap(mem), new Point(0, 0));
            g?.DrawImage(new Bitmap(mem), new Rectangle(0, 0, 192, 108), new(0, 0, 1920, 1080), GraphicsUnit.Pixel);

        }

        g?.FillRectangle(new SolidBrush(Color.AliceBlue), new(0, 0, 100, 50));
        // g.DrawLine(new Pen(new SolidBrush(Color.Blue), 2), new Point(10, 10), new Point(50, 30));
        var Rps = new List<PointF>();
        var rps = new List<PointF>();
        float R = 50;
        var rad = Math.PI / 180;
        var r = (float)(R * Math.Sin(18 * R) / Math.Cos(36 * R));
        float x = pictureBox2.Width / 2;
        float y = pictureBox2.Height / 2;
        for (var k = 0; k < 5; k++)
        {
            Rps.Add(new(x - (R * (float)Math.Cos((90 + k * 72) * rad)), y - (R * (float)Math.Sin((90 + k * 72) * rad))));
            rps.Add(new(x - (r * (float)Math.Cos((90 + k * 72 + 36) * rad)), y - (r * (float)Math.Sin((90 + k * 72 + 36) * rad))));
        }
        for (var i = 0; i < 5; i++)
        {
            //g.DrawLine(new Pen(new SolidBrush(Color.Blue), 2), Rps[i], rps[i]);
            //g.DrawLine(new Pen(new SolidBrush(Color.Blue), 2), rps[i], new PointF(x, y));
            //g.DrawLine(new Pen(new SolidBrush(Color.Blue), 2), new PointF(x, y), Rps[i]);

            g?.DrawLines(new(new SolidBrush(Color.Red), 2), [Rps[i], rps[i], new(x, y), Rps[i]]);
        }

        if (g != null)
        {
            g.DrawString(
                Properties.GtkWinFormsAppResources
                    .Gtk_TestDataForm_pictureBox2_Paint_This_is_the_Paint_Graphics_sample_effect,
                new(FontFamily.GenericSansSerif, 12, FontStyle.Regular), new SolidBrush(Color.Red), 0, 60);
            g.DrawArc(new(new SolidBrush(Color.Blue), 2),
                new(pictureBox2.Width / 2, pictureBox2.Height / 2, pictureBox2.Width, pictureBox2.Height),
                0, 270);

            g.DrawCurve(new(new SolidBrush(Color.Blue), 2),
                new PointF[] { new(50, 60), new(100, 80), new(75, 100) });
            g.DrawCurve(new(new SolidBrush(Color.Blue), 2),
                new PointF[] { new(75, 100), new(100, 120), new(120, 100) });
            g.DrawRectangle(new((Color)Color.Red), new(10, 10, 20, 20));
        }
    }

    private void button6_Click(object sender, EventArgs e)
    {
        // textBox1.InsertTextAtCursor("666 slip");
        GtkLogger.Info(textBox1.SelectionStart.ToString());
        GtkLogger.Info(textBox1.SelectionLength.ToString());
        textBox1.SelectionLength = 20;
        // richTextBox1.InsertTextAtCursor("666 slip");
        // GtkLogger.Info(richTextBox1.SelectionStart);
        //// GtkLogger.Info(richTextBox1.SelectionStart1);
        // GtkLogger.Info(richTextBox1.SelectionLength);
        richTextBox1.SelectionLength = 50;
    }

    private void button7_Click(object sender, EventArgs e)
    {
        var form = new Form2();
        var result = form.ShowDialog(this);
        if (result == DialogResult.None || result == DialogResult.Cancel)
        {
            // MessageBox.Show(" close the window to return");
        }
        //form.Show();
    }

    private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
    {
        var rect = tabControl1.GetTabRect(e.Index);
        //e.Graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
        e.Graphics.FillRectangle(new SolidBrush(Color.DarkBlue), e.Bounds);
        var font = new Font(FontFamily.GenericSansSerif, 12);
        e.Graphics.DrawString(string.Format("tab组{0}", e.Index), font, new SolidBrush(Color.Red), new PointF(0, 0));
        e.Graphics.DrawImage(Image.FromFile("./Resources/BindingNavigator.Delete.ico"), new Point(e.Bounds.Width - 16, 0));
    }

    private void button1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void textBox1_KeyDown(object sender, KeyEventArgs e)
    {

        GtkLogger.Info("textBox1_KeyDown");
    }

    private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        GtkLogger.Info("textBox1_KeyPress");
    }

    private void textBox1_KeyUp(object sender, KeyEventArgs e)
    {
        GtkLogger.Info("textBox1_KeyUp");
    }

    private void tabPage2_Click(object sender, EventArgs e)
    {
        // MessageBox.Show("ffsssssss");
    }

    private void comboBox1_DropDown(object sender, EventArgs e)
    {
        GtkLogger.Info("comboBox1_DropDown");
    }
}