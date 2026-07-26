extern alias gtk;
using System.Reflection;
using GtkSystem.ComponentModel;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class CheckBoxTests
{
    [Fact]
    public void CheckBox_GtkControl_And_Self_AreNotNull()
    {
        var cb = new CheckBox();

        Assert.NotNull(cb.GtkControl);
        Assert.NotNull(cb.self);
    }

    [Fact]
    public void CheckBox_Text_SetGet_MapsToUnderlyingLabel()
    {
        var cb = new CheckBox
        {
            Text = "check me",
            Name = "chk1"
        };

        Assert.Equal("check me", cb.Text);
        Assert.Equal("check me", cb.self.Label);
        Assert.Equal("chk1", cb.Name);
    }

    [Fact]
    public void Checked_SetGet_MapsToUnderlyingActive()
    {
        var form = new Form();
        var cb = new CheckBox();
        form.Controls.Add(cb);

        var cbChecked = false;
        var selfActive = false;
        var @checked=true;
        var active=true;

        void FormLoad(object? sender, EventArgs e)
        {
            cb.Checked = true;
            cbChecked = cb.Checked;
            selfActive = cb.self.Active;
            cb.Checked = false;
            @checked = cb.Checked;
            active = cb.self.Active;
            form.Close();
        }

        form.Load += FormLoad;
        form.ShowDialog();

        Assert.True(cbChecked);
        Assert.True(selfActive);

        Assert.False(@checked);
        Assert.False(active);
    }

    [Fact]
    public void CheckState_SetGet_Works_For_Indeterminate_Checked_Unchecked()
    {
        var cb = new CheckBox
        {
            CheckState = CheckState.Indeterminate
        };

        Assert.Equal(CheckState.Indeterminate, cb.CheckState);
        Assert.True(cb.self.Inconsistent);
        Assert.False(cb.self.Active);

        cb.CheckState = CheckState.Checked;
        Assert.Equal(CheckState.Checked, cb.CheckState);
        Assert.False(cb.self.Inconsistent);
        Assert.True(cb.self.Active);

        cb.CheckState = CheckState.Unchecked;
        Assert.Equal(CheckState.Unchecked, cb.CheckState);
        Assert.False(cb.self.Inconsistent);
        Assert.False(cb.self.Active);
    }

    [Fact]
    public void OnCheckedChanged_Raises_CheckedChanged_Event()
    {
        var cb = new CheckBox();
        var invoked = false;
        cb.CheckedChanged += (s, e) => invoked = true;

        var onCheckedChanged = typeof(CheckBox).GetMethod("OnCheckedChanged", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onCheckedChanged);

        onCheckedChanged!.Invoke(cb, new object[] { EventArgs.Empty });

        Assert.True(invoked);
    }

    [Fact]
    public void OnCheckStateChanged_Raises_CheckStateChanged_Event()
    {
        var cb = new CheckBox();
        var invoked = false;
        cb.CheckStateChanged += (s, e) => invoked = true;

        var onCheckStateChanged = typeof(CheckBox).GetMethod("OnCheckStateChanged", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onCheckStateChanged);

        onCheckStateChanged!.Invoke(cb, new object[] { EventArgs.Empty });

        Assert.True(invoked);
    }
}