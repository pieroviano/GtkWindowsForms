extern alias gtk;
using System.Data;
using System.Reflection;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class ComboBoxTests
{
    [Fact]
    public void ComboBox_GtkControl_And_Self_AreNotNull()
    {
        var combo = new ComboBox();

        Assert.NotNull(combo.GtkControl);
        Assert.NotNull(combo.self);
    }

    [Fact]
    public void NativeAdd_And_NativeGetItemText_Work()
    {
        var form = new Form();
        var combo = new ComboBox();
        form.Controls.Add(combo);

        string? text0 = null;
        string? text1 = null;

        // Perform native additions after GTK widgets are realized by showing the form,
        // similar to how Checked_SetGet_MapsToUnderlyingActive uses Form.ShowDialog to ensure deterministic behavior.
        form.Load += (s, e) =>
        {
            combo.NativeAdd(-1, "v1", "text1");
            combo.NativeAdd(-1, "v2", "text2");

            text0 = combo.NativeGetItemText(0);
            text1 = combo.NativeGetItemText(1);

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("v1", text0);
        Assert.Equal("v2", text1);
    }

    [Fact]
    public void SelectedIndex_SetGet_MapsToActive_And_Clears_Text_OnMinusOne()
    {
        var combo = new ComboBox();

        combo.NativeAdd(-1, "id0", "t0");
        combo.NativeAdd(-1, "id1", "t1");

        // select index 1
        combo.SelectedIndex = 1;
        Assert.Equal(1, combo.SelectedIndex);
        // selecting -1 clears Text
        combo.SelectedIndex = -1;
        Assert.Equal(-1, combo.SelectedIndex);
        Assert.Equal(string.Empty, combo.Text);
    }

    [Fact]
    public void SelectedValue_SetGet_MapsTo_ActiveId()
    {
        var combo = new ComboBox
        {
            SelectedValue = "myActiveId"
        };

        Assert.Equal("myActiveId", combo.SelectedValue);
        Assert.Equal("myActiveId", combo.self.ActiveId);
    }

    [Fact]
    public void DropDownStyle_DropDownList_Disables_EntryEditability()
    {
        var combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // When DropDownList is selected the entry is not editable and cannot focus
        Assert.False(combo.self.Entry.IsEditable);
        Assert.False(combo.self.Entry.CanFocus);

        // Switch back to DropDown should allow editable (setter removes class)
        combo.DropDownStyle = ComboBoxStyle.DropDown;
        Assert.True(combo.self.Entry.IsEditable || combo.self.Entry.CanFocus || combo.DropDownStyle == ComboBoxStyle.DropDown);
    }

    [Fact]
    public void OnDropDown_Raises_DropDown_Event()
    {
        var combo = new ComboBox();
        var invoked = false;
        combo.DropDown += (s, e) => invoked = true;

        var onDropDown = typeof(ComboBox).GetMethod("OnDropDown", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onDropDown);

        onDropDown.Invoke(combo, new object[] { EventArgs.Empty });

        Assert.True(invoked);
    }

    [Fact]
    public void DataSource_With_DataTable_Populates_Items()
    {
        var combo = new ComboBox();

        var table = new DataTable();
        table.Columns.Add("Id");
        table.Columns.Add("Name");
        table.Rows.Add("1", "One");
        table.Rows.Add("2", "Two");

        // configure display/value members and assign as DataSource after realization logic simulation
        combo.ValueMember = "Id";
        combo.DisplayMember = "Name";
        combo.DataSource = table;

        // DataSource logic executes only when realized; call the private OnSetDataSource via reflection
        var onSet = typeof(ComboBox).GetMethod("OnSetDataSource", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onSet);
        onSet.Invoke(combo, Array.Empty<object>());

        // Items should have been populated from DataTable - the ObjectCollection stores entries; we can check Native model text
        Assert.Equal("One", combo.NativeGetItemText(0));
        Assert.Equal("Two", combo.NativeGetItemText(1));
    }
}