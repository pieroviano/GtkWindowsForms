#pragma warning disable xUnit2013
namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class ListBoxTests
{
    [Fact]
    public void ListBox_DefaultProperties_AreSetCorrectly()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        var multiColumn = true;
        var sorted = true;
        var selectionMode = SelectionMode.MultiSimple;
        var integralHeight = false;
        var useTabStops = true;
        var scrollAlwaysVisible = true;
        object? items = null;
        var selectedIndex = 0;
        var selectedItem = new object();
        var selectedValue = new object();

        form.Load += (s, e) =>
        {
            multiColumn = listBox.MultiColumn;
            sorted = listBox.Sorted;
            selectionMode = listBox.SelectionMode;
            integralHeight = listBox.IntegralHeight;
            useTabStops = listBox.UseTabStops;
            scrollAlwaysVisible = listBox.ScrollAlwaysVisible;
            items = listBox.Items;
            selectedIndex = listBox.SelectedIndex;
            selectedItem = listBox.SelectedItem;
            selectedValue = listBox.SelectedValue;

            form.Close();
        };

        form.ShowDialog();

        Assert.False(multiColumn);
        Assert.False(sorted);
        Assert.Equal(SelectionMode.One, selectionMode);
        Assert.True(integralHeight);
        Assert.False(useTabStops);
        Assert.False(scrollAlwaysVisible);
        Assert.NotNull(items);
        Assert.Equal(-1, selectedIndex);
        Assert.Null(selectedItem);
        Assert.Null(selectedValue);
    }

    [Fact]
    public void ListBox_AddClearItems_WorkCorrectly()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        string? first = null;
        string? second = null;
        var countAfterClear = -1;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("item1");
            listBox.Items.Add("item2");

            first = (string)listBox.Items[0];
            second = (string)listBox.Items[1];

            listBox.Items.Clear();
            countAfterClear = listBox.Items.Count;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("item1", first);
        Assert.Equal("item2", second);
        Assert.Equal(0, countAfterClear);
    }

    [Fact]
    public void ListBox_SetSelectedIndex_ChangesSelectedItem()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        object? selectedItem = null;
        var selectedIndex = -1;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("item1");
            listBox.Items.Add("item2");

            listBox.SelectedIndex = 1;

            selectedItem = listBox.SelectedItem;
            selectedIndex = listBox.SelectedIndex;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("item2", selectedItem);
        Assert.Equal(1, selectedIndex);
    }

    [Fact]
    public void ListBox_SetSelectedItem_ChangesSelectedIndex()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        var selectedIndex = -1;
        object? selectedItem = null;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("item1");
            listBox.Items.Add("item2");

            listBox.SelectedItem = "item2";

            selectedIndex = listBox.SelectedIndex;
            selectedItem = listBox.SelectedItem;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, selectedIndex);
        Assert.Equal("item2", selectedItem);
    }

    [Fact]
    public void ListBox_SetSelectedValue_ChangesSelectedItem()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        object? selectedItem = null;
        var selectedIndex = -1;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("a");
            listBox.Items.Add("b");

            listBox.SelectedValue = "b";

            selectedItem = listBox.SelectedItem;
            selectedIndex = listBox.SelectedIndex;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("b", selectedItem);
        Assert.Equal(1, selectedIndex);
    }

    [Fact]
    public void ListBox_DataSource_SetsItemsCorrectly()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        var count = 0;
        string? first = null;
        string? second = null;
        string? third = null;

        form.Load += (s, e) =>
        {
            var data = new[] { "one", "two", "three" };
            listBox.DataSource = data;

            listBox.BeginUpdate();
            listBox.EndUpdate();

            count = listBox.Items.Count;
            first = (string)listBox.Items[0];
            second = (string)listBox.Items[1];
            third = (string)listBox.Items[2];

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(3, count);
        Assert.Equal("one", first);
        Assert.Equal("two", second);
        Assert.Equal("three", third);
    }

    [Fact]
    public void ListBox_GetItemText_ReturnsCorrectText()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        string? first = null;
        string? second = null;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("abc");
            listBox.Items.Add("xyz");

            first = listBox.GetItemText(listBox.Items[0]);
            second = listBox.GetItemText(listBox.Items[1]);

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("abc", first);
        Assert.Equal("xyz", second);
    }

    [Fact]
    public void ListBox_ClearSelected_ResetsSelection()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        var selectedIndex = 0;
        var selectedItem = new object();

        form.Load += (s, e) =>
        {
            listBox.Items.Add("item1");
            listBox.SelectedIndex = 0;

            listBox.ClearSelected();

            selectedIndex = listBox.SelectedIndex;
            selectedItem = listBox.SelectedItem;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(-1, selectedIndex);
        Assert.Null(selectedItem);
    }

    [Fact]
    public void ListBox_SetSelected_SelectsCorrectly()
    {
        var form = new Form();
        var listBox = new ListBox();
        form.Controls.Add(listBox);

        object? selectedAfterSelect = null;
        var indexAfterDeselect = 0;

        form.Load += (s, e) =>
        {
            listBox.Items.Add("one");
            listBox.Items.Add("two");

            listBox.SetSelected(1, true);
            selectedAfterSelect = listBox.SelectedItem;

            listBox.SetSelected(1, false);
            indexAfterDeselect = listBox.SelectedIndex;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("two", selectedAfterSelect);
        Assert.Equal(-1, indexAfterDeselect);
    }
}