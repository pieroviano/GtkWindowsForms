extern alias gtk;
using GtkSystem.ComponentModel;
#pragma warning disable xUnit2013
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class CheckedListBoxTests
{
    [Fact]
    public void CheckedListBox_GtkControl_And_Self_AreNotNull()
    {
        var clb = new CheckedListBox();

        Assert.NotNull(clb.GtkControl);
        Assert.NotNull(clb.self);
    }

    [Fact]
    public void Items_Add_Appends_CheckBox_And_FlowChildrenUpdated()
    {
        var form = new Form();
        var clb = new CheckedListBox();
        form.Controls.Add(clb);
        var idx = -1;
        var count=-1;
        object? clbItem = "";
        CheckBox? item = null;
        string? text = null;
        var length = -1;
        form.Load += (_, _) =>
        {
            idx = clb.Items.Add("one");
            count = clb.Items.Count;
            clbItem = clb.Items[0];
            item = clb.Items.GetCheckedListBoxItem(0);
            text = item?.Text;
            length = clb._flow.GetChildrenArray().Length;
            form.Close();
        };
        form.ShowDialog();

        Assert.Equal(0, idx);
        Assert.Equal(1, count);
        Assert.Equal("one", clbItem);

        Assert.NotNull(item);
        Assert.Equal("one", text);

        // Internal flow should have one child after adding
        Assert.Equal(1, length);
    }

    [Fact]
    public void CheckedIndices_And_CheckedItems_Reflect_Checked_Items()
    {
        var clb = new CheckedListBox();

        clb.Items.Add("a", true);
        clb.Items.Add("b", false);
        clb.Items.Add("c", true);

        // Two items should be checked: indices 0 and 2
        Assert.Equal(2, clb.CheckedIndices.Count);
        Assert.Contains(0, clb.CheckedIndices);
        Assert.Contains(2, clb.CheckedIndices);

        // CheckedItems should contain the string labels
        Assert.Equal(2, clb.CheckedItems.Count);
        Assert.Contains("a", clb.CheckedItems);
        Assert.Contains("c", clb.CheckedItems);
    }

    [Fact]
    public void SetItemChecked_And_SetItemCheckState_Update_ItemState()
    {
        var clb = new CheckedListBox();

        clb.Items.Add("x");
        var checkBox = clb.Items.GetCheckedListBoxItem(0);
        Assert.NotNull(checkBox);

        // Set checked via wrapper method
        clb.SetItemChecked(0, true);
        Assert.True(checkBox?.Checked);
        Assert.True(checkBox.self.Active);

        // Set check state via method
        clb.SetItemCheckState(0, CheckState.Unchecked);
        Assert.False(checkBox.self.Active);
        Assert.False(checkBox.Checked);

        clb.SetItemCheckState(0, CheckState.Checked);
        Assert.True(checkBox.self.Active);
        Assert.True(checkBox.Checked);
    }

    [Fact]
    public void Items_Clear_Removes_NativeFlowChildren()
    {
        var clb = new CheckedListBox();

        clb.Items.Add("one");
        clb.Items.Add("two");
        Assert.Equal(2, clb.Items.Count);
        Assert.Equal(2, clb._flow.GetChildrenArray().Length);

        clb.Items.Clear();

        Assert.Equal(0, clb.Items.Count);
        Assert.Equal(0, clb._flow.GetChildrenArray().Length);
    }

    [Fact]
    public void ItemCheck_Event_Raised_When_CheckButton_Toggled()
    {
        var clb = new CheckedListBox();

        clb.Items.Add("toggle");
        var cb = clb.Items.GetCheckedListBoxItem(0);
        Assert.NotNull(cb);

        ItemCheckEventArgs? received = null;
        clb.ItemCheck += (s, e) => received = e;

        // Toggle underlying GTK checkbutton to true; this should fire the native Toggled -> ItemCheck path.
        cb.self.Active = true;

        Assert.NotNull(received);
        Assert.Equal(0, received.Index);
        Assert.Equal(CheckState.Checked, received.NewValue);
    }
}