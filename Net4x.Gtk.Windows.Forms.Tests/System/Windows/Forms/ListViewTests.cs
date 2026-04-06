#pragma warning disable xUnit2013

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class ListViewTests
{
    [Fact]
    public void ListView_DefaultProperties_AreSetCorrectly()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        bool multiSelect = false;
        bool gridLines = false;
        bool showGroups = false;
        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            multiSelect = lv.MultiSelect;
            gridLines = lv.GridLines;
            showGroups = lv.ShowGroups;
            gtkControl = lv.GtkControl;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(multiSelect);
        Assert.True(gridLines);
        Assert.True(showGroups);
        Assert.NotNull(gtkControl);
    }

    [Fact]
    public void ListView_Items_Add_And_Clear_WorkCorrectly()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int countAfterAdd = 0;
        int countAfterClear = 0;

        form.Load += (s, e) =>
        {
            lv.Items.Add("one");
            lv.Items.Add("two");

            countAfterAdd = lv.Items.Count;

            lv.Clear();
            countAfterClear = lv.Items.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(2, countAfterAdd);
        Assert.Equal(0, countAfterClear);
    }

    [Fact]
    public void ListView_CheckedItems_ReturnsOnlyChecked()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int checkedCount = 0;

        form.Load += (s, e) =>
        {
            var a = lv.Items.Add("a");
            var b = lv.Items.Add("b");

            a.Checked = true;

            checkedCount = lv.CheckedItems.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, checkedCount);
    }

    [Fact]
    public void ListView_SelectedItems_ReturnsOnlySelected()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int selectedCount = 0;

        form.Load += (s, e) =>
        {
            var a = lv.Items.Add("a");
            var b = lv.Items.Add("b");

            a.Selected = true;

            selectedCount = lv.SelectedItems.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, selectedCount);
    }

    [Fact]
    public void ListView_FindItemWithText_Works()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        ListViewItem? found = null;

        form.Load += (s, e) =>
        {
            lv.Items.Add("one");
            lv.Items.Add("two");

            found = lv.FindItemWithText("two");

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(found);
        Assert.Equal("two", found!.Text);
    }

    [Fact]
    public void ListView_FindItemWithText_WithStartIndex_Works()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        ListViewItem? found = null;

        form.Load += (s, e) =>
        {
            lv.Items.Add("one");
            lv.Items.Add("two");
            lv.Items.Add("two");

            found = lv.FindItemWithText("two", false, 1);

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(found);
        Assert.Equal("two", found!.Text);
    }

    [Fact]
    public void ListView_Columns_Add_Works()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int count = 0;

        form.Load += (s, e) =>
        {
            lv.Columns.Add("col1");
            lv.Columns.Add("col2");

            count = lv.Columns.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(2, count);
    }

    [Fact]
    public void ListView_Groups_Add_Works()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int count = 0;

        form.Load += (s, e) =>
        {
            lv.Groups.Add(new ListViewGroup("g1"));
            lv.Groups.Add(new ListViewGroup("g2"));

            count = lv.Groups.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(2, count);
    }

    [Fact]
    public void ListView_BeginUpdate_EndUpdate_DoNotThrow()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        bool success = false;

        form.Load += (s, e) =>
        {
            lv.BeginUpdate();
            lv.Items.Add("one");
            lv.EndUpdate();

            success = true;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(success);
    }

    [Fact]
    public void ListView_Clear_Removes_Items_And_Groups()
    {
        var form = new Form();
        var lv = new ListView();
        form.Controls.Add(lv);

        int items = -1;
        int groups = -1;

        form.Load += (s, e) =>
        {
            lv.Items.Add("one");
            lv.Groups.Add(new ListViewGroup("g1"));

            lv.Clear();

            items = lv.Items.Count;
            groups = lv.Groups.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0, items);
        Assert.Equal(0, groups);
    }
}