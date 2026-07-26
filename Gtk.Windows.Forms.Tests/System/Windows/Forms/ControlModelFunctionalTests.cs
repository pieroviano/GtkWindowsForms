extern alias gtk;
using System.Windows.Forms;
using Color = global::System.Drawing.Color;
using Point = global::System.Drawing.Point;
using Size = global::System.Drawing.Size;
using Rectangle = global::System.Drawing.Rectangle;
using Font = gtk::System.Drawing.Font;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="Control"/>: the geometry properties must stay mutually
/// consistent (Bounds/Location/Size/Left/Top/Right/Bottom), the child collection must maintain parentage,
/// and the state properties must round-trip.</summary>
[Collection("NonParallelCollection")]
public class ControlModelFunctionalTests
{
    [Fact]
    public void Location_And_Size_AgreeWithBounds()
    {
        using var control = new Panel { Location = new Point(10, 20), Size = new Size(100, 50) };

        Assert.Equal(new Rectangle(10, 20, 100, 50), control.Bounds);
        Assert.Equal(10, control.Left);
        Assert.Equal(20, control.Top);
        Assert.Equal(100, control.Width);
        Assert.Equal(50, control.Height);
    }

    [Fact]
    public void SettingBounds_UpdatesLocationAndSize()
    {
        using var control = new Panel();

        control.Bounds = new Rectangle(5, 6, 70, 80);

        Assert.Equal(new Point(5, 6), control.Location);
        Assert.Equal(new Size(70, 80), control.Size);
    }

    [Fact]
    public void Right_And_Bottom_AreLeftPlusWidthAndTopPlusHeight()
    {
        using var control = new Panel { Location = new Point(10, 20), Size = new Size(100, 50) };

        Assert.Equal(110, control.Right);
        Assert.Equal(70, control.Bottom);
    }

    [Fact]
    public void SettingLeftAndTop_MovesTheControlWithoutResizing()
    {
        using var control = new Panel { Bounds = new Rectangle(0, 0, 40, 30) };

        control.Left = 15;
        control.Top = 25;

        Assert.Equal(new Point(15, 25), control.Location);
        Assert.Equal(new Size(40, 30), control.Size);
    }

    [Fact]
    public void SettingWidthAndHeight_ResizesWithoutMoving()
    {
        using var control = new Panel { Bounds = new Rectangle(7, 8, 40, 30) };

        control.Width = 60;
        control.Height = 90;

        Assert.Equal(new Point(7, 8), control.Location);
        Assert.Equal(new Size(60, 90), control.Size);
    }

    [Fact]
    public void SetBounds_AppliesAllFourValues()
    {
        using var control = new Panel();

        control.SetBounds(3, 4, 55, 66);

        Assert.Equal(new Rectangle(3, 4, 55, 66), control.Bounds);
    }

    [Fact]
    public void NameTextAndTag_RoundTrip()
    {
        using var control = new Panel { Name = "panel1", Text = "caption", Tag = 99 };

        Assert.Equal("panel1", control.Name);
        Assert.Equal("caption", control.Text);
        Assert.Equal(99, control.Tag);
    }

    [Fact]
    public void EnabledAndVisible_RoundTrip()
    {
        using var control = new Panel { Enabled = false, Visible = false };

        Assert.False(control.Enabled);
        Assert.False(control.Visible);

        control.Enabled = true;
        control.Visible = true;

        Assert.True(control.Enabled);
        Assert.True(control.Visible);
    }

    [Fact]
    public void ColorsAndFont_RoundTrip()
    {
        using var control = new Panel
        {
            BackColor = Color.CadetBlue,
            ForeColor = Color.DarkRed,
            Font = new Font("Sans", 11f)
        };

        Assert.Equal(Color.CadetBlue, control.BackColor);
        Assert.Equal(Color.DarkRed, control.ForeColor);
        Assert.Equal(11f, control.Font.Size, 2);
    }

    [Fact]
    public void AnchorAndDock_RoundTrip()
    {
        using var control = new Panel { Anchor = AnchorStyles.Top | AnchorStyles.Right };
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Right, control.Anchor);

        control.Dock = DockStyle.Fill;
        Assert.Equal(DockStyle.Fill, control.Dock);
    }

    [Fact]
    public void MarginAndPadding_RoundTrip()
    {
        using var control = new Panel { Margin = new Padding(1, 2, 3, 4), Padding = new Padding(5) };

        Assert.Equal(new Padding(1, 2, 3, 4), control.Margin);
        Assert.Equal(new Padding(5), control.Padding);
    }

    [Fact]
    public void MinimumAndMaximumSize_RoundTrip()
    {
        using var control = new Panel { MinimumSize = new Size(10, 20), MaximumSize = new Size(300, 400) };

        Assert.Equal(new Size(10, 20), control.MinimumSize);
        Assert.Equal(new Size(300, 400), control.MaximumSize);
    }

    [Fact]
    public void TabIndexAndTabStop_RoundTrip()
    {
        using var control = new Button { TabIndex = 4, TabStop = false };

        Assert.Equal(4, control.TabIndex);
        Assert.False(control.TabStop);
    }

    [Fact]
    public void Controls_Add_SetsTheParentAndIsFoundInTheCollection()
    {
        using var parent = new Panel();
        var child = new Button { Name = "child" };

        parent.Controls!.Add(child);

        Assert.Single(parent.Controls);
        Assert.Same(parent, child.Parent);
        Assert.True(parent.Controls.Contains(child), "the added child should be contained");
        Assert.Equal(0, parent.Controls.IndexOf(child));
    }

    [Fact]
    public void Controls_Remove_ClearsTheParent()
    {
        using var parent = new Panel();
        var child = new Button();
        parent.Controls!.Add(child);

        parent.Controls.Remove(child);

        Assert.Empty(parent.Controls);
        Assert.False(parent.Controls.Contains(child), "the removed child should no longer be contained");
    }

    [Fact]
    public void Controls_AddRange_AddsEveryChild()
    {
        using var parent = new Panel();
        var a = new Button();
        var b = new Button();

        parent.Controls!.AddRange(new Control[] { a, b });

        Assert.Equal(2, parent.Controls.Count);
        Assert.Same(parent, b.Parent);
    }

    [Fact]
    public void Controls_Clear_RemovesEveryChild()
    {
        using var parent = new Panel();
        parent.Controls!.Add(new Button());
        parent.Controls.Add(new Button());

        parent.Controls.Clear();

        Assert.Empty(parent.Controls);
    }

    [Fact]
    public void Controls_KeyedLookup_FindsTheChildByName()
    {
        using var parent = new Panel();
        parent.Controls!.Add(new Button { Name = "ok" });

        Assert.True(parent.Controls.ContainsKey("ok"), "the named child should be found by key");
        Assert.Equal(0, parent.Controls.IndexOfKey("ok"));
        Assert.NotNull(parent.Controls["ok"]);
    }

    [Fact]
    public void Controls_Enumerator_YieldsEveryChildInOrder()
    {
        using var parent = new Panel();
        var a = new Button { Name = "a" };
        var b = new Button { Name = "b" };
        parent.Controls!.Add(a);
        parent.Controls.Add(b);

        var names = new List<string>();
        foreach (Control c in parent.Controls)
        {
            names.Add(c.Name);
        }

        Assert.Equal(new[] { "a", "b" }, names);
    }

    [Fact]
    public void HasChildren_ReflectsTheControlCollection()
    {
        using var parent = new Panel();
        Assert.False(parent.HasChildren, "a fresh panel has no children");

        parent.Controls!.Add(new Button());

        Assert.True(parent.HasChildren, "a panel with a child should report HasChildren");
    }

    [Fact]
    public void Contains_IsTrueForDescendantsOnly()
    {
        using var parent = new Panel();
        var child = new Panel();
        var grandChild = new Button();
        parent.Controls!.Add(child);
        child.Controls!.Add(grandChild);
        var stranger = new Button();

        Assert.True(parent.Contains(child), "a direct child is contained");
        Assert.True(parent.Contains(grandChild), "a grandchild is contained");
        Assert.False(parent.Contains(stranger), "an unrelated control is not contained");
    }

    [Fact]
    public void Controls_Find_LocatesByNameWithAndWithoutRecursion()
    {
        using var parent = new Panel();
        var child = new Panel { Name = "child" };
        var grandChild = new Button { Name = "target" };
        parent.Controls!.Add(child);
        child.Controls!.Add(grandChild);

        var shallow = parent.Controls.Find("target", false);
        var deep = parent.Controls.Find("target", true);

        Assert.Empty(shallow);
        Assert.Single(deep);
        Assert.Same(grandChild, deep[0]);
    }

    [Fact]
    public void TopLevelControl_WalksUpToTheForm()
    {
        using var form = new Form();
        var panel = new Panel();
        var button = new Button();
        form.Controls!.Add(panel);
        panel.Controls!.Add(button);

        Assert.Same(form, button.TopLevelControl);
    }

    [Fact]
    public void Parent_IsNullUntilTheControlIsAdded()
    {
        var orphan = new Button();

        Assert.Null(orphan.Parent);
    }

    [Fact]
    public void ClientSize_TracksSizeForABorderlessPanel()
    {
        using var control = new Panel { Size = new Size(120, 60) };

        Assert.True(control.ClientSize.Width <= 120 && control.ClientSize.Width > 0,
            $"client width {control.ClientSize.Width} should be within the control width");
        Assert.True(control.ClientSize.Height <= 60 && control.ClientSize.Height > 0,
            $"client height {control.ClientSize.Height} should be within the control height");
    }

    [Fact]
    public void ClientRectangle_StartsAtTheOrigin()
    {
        using var control = new Panel { Size = new Size(120, 60) };

        Assert.Equal(0, control.ClientRectangle.X);
        Assert.Equal(0, control.ClientRectangle.Y);
    }

    [Fact]
    public void IsDisposed_BecomesTrueAfterDispose()
    {
        var control = new Panel();
        Assert.False(control.IsDisposed, "a live control is not disposed");

        control.Dispose();

        Assert.True(control.IsDisposed, "a disposed control should report IsDisposed");
    }

    [Fact]
    public void Handle_And_IsHandleCreated_AgreeWithEachOther()
    {
        using var control = new Panel();

        Assert.Equal(control.Handle != IntPtr.Zero, control.IsHandleCreated);
    }

    [Fact]
    public void RightToLeftAndAccessibility_RoundTrip()
    {
        using var control = new Panel
        {
            RightToLeft = RightToLeft.Yes,
            AccessibleName = "panel",
            AccessibleDescription = "a panel",
            AccessibleRole = AccessibleRole.Default
        };

        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal("panel", control.AccessibleName);
        Assert.Equal("a panel", control.AccessibleDescription);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
    }

    [Fact]
    public void AllowDropAndCausesValidation_RoundTrip()
    {
        using var control = new Panel { AllowDrop = true, CausesValidation = false };

        Assert.True(control.AllowDrop);
        Assert.False(control.CausesValidation);
    }

    [Fact]
    public void AddingAChildTwice_DoesNotDuplicateIt()
    {
        using var parent = new Panel();
        var child = new Button();

        parent.Controls!.Add(child);
        parent.Controls.Add(child);

        Assert.Single(parent.Controls);
    }

    [Fact]
    public void MovingAChildToANewParent_UpdatesBothCollections()
    {
        using var first = new Panel();
        using var second = new Panel();
        var child = new Button();
        first.Controls!.Add(child);

        second.Controls!.Add(child);

        Assert.Empty(first.Controls);
        Assert.Single(second.Controls);
        Assert.Same(second, child.Parent);
    }
}
