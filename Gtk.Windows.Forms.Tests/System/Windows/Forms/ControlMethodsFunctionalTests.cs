using System.Windows.Forms;
using Color = global::System.Drawing.Color;
using Point = global::System.Drawing.Point;
using Size = global::System.Drawing.Size;
using Rectangle = global::System.Drawing.Rectangle;
using SizeF = global::System.Drawing.SizeF;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="Control"/>'s methods: navigation up and down the control tree,
/// coordinate conversion, scaling, show/hide, and the Reset* family that must restore defaults.</summary>
[Collection("NonParallelCollection")]
public class ControlMethodsFunctionalTests
{
    [Fact]
    public void FindForm_WalksUpToTheOwningForm()
    {
        using var form = new Form();
        var panel = new Panel();
        var button = new Button();
        form.Controls.Add(panel);
        panel.Controls!.Add(button);

        Assert.Same(form, button.FindForm());
    }

    [Fact]
    public void FindForm_IsNullForAnUnparentedControl()
    {
        using var orphan = new Button();

        Assert.Null(orphan.FindForm());
    }

    [Fact]
    public void GetContainerControl_FindsTheNearestContainer()
    {
        using var form = new Form();
        var panel = new Panel();
        var button = new Button();
        form.Controls.Add(panel);
        panel.Controls!.Add(button);

        Assert.NotNull(button.GetContainerControl());
    }

    [Fact]
    public void GetNextControl_WalksTheChildrenForward()
    {
        using var parent = new Panel();
        var first = new Button { Name = "first", TabIndex = 0 };
        var second = new Button { Name = "second", TabIndex = 1 };
        parent.Controls!.Add(first);
        parent.Controls.Add(second);

        var next = parent.GetNextControl(first, true);

        Assert.Same(second, next);
    }

    [Fact]
    public void GetNextControl_WalksTheChildrenBackward()
    {
        using var parent = new Panel();
        var first = new Button { Name = "first", TabIndex = 0 };
        var second = new Button { Name = "second", TabIndex = 1 };
        parent.Controls!.Add(first);
        parent.Controls.Add(second);

        var previous = parent.GetNextControl(second, false);

        Assert.Same(first, previous);
    }

    [Fact]
    public void PointToScreen_And_PointToClient_AreInverses()
    {
        using var form = new Form { Location = new Point(50, 60), Size = new Size(300, 200) };
        var panel = new Panel { Location = new Point(10, 20), Size = new Size(100, 80) };
        form.Controls.Add(panel);

        var screen = panel.PointToScreen(new Point(5, 7));
        var back = panel.PointToClient(screen);

        Assert.Equal(new Point(5, 7), back);
    }

    [Fact]
    public void RectangleToScreen_And_RectangleToClient_AreInverses()
    {
        using var form = new Form { Location = new Point(50, 60), Size = new Size(300, 200) };
        var panel = new Panel { Location = new Point(10, 20), Size = new Size(100, 80) };
        form.Controls.Add(panel);

        var rect = new Rectangle(2, 3, 20, 30);
        var screen = panel.RectangleToScreen(rect);
        var back = panel.RectangleToClient(screen);

        Assert.Equal(rect, back);
    }

    [Fact]
    public void RectangleToScreen_KeepsTheSize()
    {
        using var panel = new Panel { Size = new Size(100, 80) };

        var screen = panel.RectangleToScreen(new Rectangle(2, 3, 20, 30));

        Assert.Equal(20, screen.Width);
        Assert.Equal(30, screen.Height);
    }

    [Fact]
    public void HideAndShow_FlipVisible()
    {
        using var panel = new Panel();

        panel.Hide();
        Assert.False(panel.Visible, "a hidden control should not be visible");

        panel.Show();
        Assert.True(panel.Visible, "a shown control should be visible");
    }

    [Fact]
    public void Scale_MultipliesTheSize()
    {
        using var panel = new Panel { Bounds = new Rectangle(10, 20, 100, 50) };

        panel.Scale(2f);

        Assert.Equal(200, panel.Width);
        Assert.Equal(100, panel.Height);
    }

    [Fact]
    public void Scale_WithSeparateFactors_ScalesEachAxis()
    {
        using var panel = new Panel { Bounds = new Rectangle(0, 0, 100, 50) };

        panel.Scale(2f, 3f);

        Assert.Equal(200, panel.Width);
        Assert.Equal(150, panel.Height);
    }

    [Fact]
    public void Scale_WithASizeF_MatchesTheTwoFactorOverload()
    {
        using var byFactors = new Panel { Bounds = new Rectangle(0, 0, 100, 50) };
        using var bySize = new Panel { Bounds = new Rectangle(0, 0, 100, 50) };

        byFactors.Scale(2f, 3f);
        bySize.Scale(new SizeF(2f, 3f));

        Assert.Equal(byFactors.Size, bySize.Size);
    }

    [Fact]
    public void SetBounds_WithBoundsSpecified_OnlyAppliesTheRequestedParts()
    {
        using var panel = new Panel { Bounds = new Rectangle(1, 2, 30, 40) };

        panel.SetBounds(9, 9, 99, 99, BoundsSpecified.Location);

        Assert.Equal(new Point(9, 9), panel.Location);
        Assert.Equal(new Size(30, 40), panel.Size);
    }

    [Fact]
    public void SetBounds_WithSizeOnly_LeavesTheLocation()
    {
        using var panel = new Panel { Bounds = new Rectangle(1, 2, 30, 40) };

        panel.SetBounds(9, 9, 99, 88, BoundsSpecified.Size);

        Assert.Equal(new Point(1, 2), panel.Location);
        Assert.Equal(new Size(99, 88), panel.Size);
    }

    [Fact]
    public void ResetBackColor_RestoresTheDefault()
    {
        using var panel = new Panel();
        var original = panel.BackColor;
        panel.BackColor = Color.Red;

        panel.ResetBackColor();

        Assert.Equal(original, panel.BackColor);
    }

    [Fact]
    public void ResetForeColor_RestoresTheDefault()
    {
        using var panel = new Panel();
        var original = panel.ForeColor;
        panel.ForeColor = Color.Red;

        panel.ResetForeColor();

        Assert.Equal(original, panel.ForeColor);
    }

    [Fact]
    public void ResetText_ClearsTheText()
    {
        using var panel = new Panel { Text = "something" };

        panel.ResetText();

        Assert.True(string.IsNullOrEmpty(panel.Text), $"ResetText should clear the text but left '{panel.Text}'");
    }

    [Fact]
    public void ResetRightToLeft_RestoresInheritance()
    {
        using var panel = new Panel { RightToLeft = RightToLeft.Yes };

        panel.ResetRightToLeft();

        Assert.NotEqual(RightToLeft.Yes, panel.RightToLeft);
    }

    [Fact]
    public void SuspendAndResumeLayout_LeaveTheControlUsable()
    {
        using var panel = new Panel();

        panel.SuspendLayout();
        panel.Controls!.Add(new Button());
        panel.ResumeLayout(true);

        Assert.Single(panel.Controls);
    }

    [Fact]
    public void InvalidateAndRefreshAndUpdate_DoNotDisturbTheControl()
    {
        using var panel = new Panel { Bounds = new Rectangle(0, 0, 50, 40) };

        panel.Invalidate();
        panel.Invalidate(true);
        panel.Invalidate(new Rectangle(0, 0, 10, 10));
        panel.Invalidate(new Rectangle(0, 0, 10, 10), true);
        panel.Refresh();
        panel.Update();

        Assert.Equal(new Size(50, 40), panel.Size);
    }

    [Fact]
    public void CreateGraphics_ReturnsAUsableSurface()
    {
        using var panel = new Panel { Bounds = new Rectangle(0, 0, 40, 30) };

        using var graphics = panel.CreateGraphics();

        Assert.NotNull(graphics);
    }

    [Fact]
    public void GetPreferredSize_IsNotNegative()
    {
        using var panel = new Panel { Bounds = new Rectangle(0, 0, 40, 30) };

        var preferred = panel.GetPreferredSize(new Size(100, 100));

        Assert.True(preferred.Width >= 0 && preferred.Height >= 0,
            $"preferred size should not be negative but was {preferred}");
    }

    [Fact]
    public void LogicalToDeviceUnits_IsMonotonic()
    {
        using var panel = new Panel();

        Assert.True(panel.LogicalToDeviceUnits(200) >= panel.LogicalToDeviceUnits(100),
            "converting a larger logical value must not produce a smaller device value");
    }

    [Fact]
    public void GetChildAtPoint_FindsTheChildUnderThePoint()
    {
        using var parent = new Panel { Bounds = new Rectangle(0, 0, 200, 200) };
        var child = new Button { Bounds = new Rectangle(10, 10, 50, 40) };
        parent.Controls!.Add(child);

        var hit = parent.GetChildAtPoint(new Point(20, 20));
        var miss = parent.GetChildAtPoint(new Point(150, 150));

        Assert.Same(child, hit);
        Assert.Null(miss);
    }

    [Fact]
    public void BringToFrontAndSendToBack_KeepTheChildInTheCollection()
    {
        using var parent = new Panel();
        var first = new Button();
        var second = new Button();
        parent.Controls!.Add(first);
        parent.Controls.Add(second);

        first.BringToFront();
        second.SendToBack();

        Assert.Equal(2, parent.Controls.Count);
        Assert.True(parent.Controls.Contains(first), "bringing a child to the front must not remove it");
        Assert.True(parent.Controls.Contains(second), "sending a child to the back must not remove it");
    }

    [Fact]
    public void Controls_SetChildIndex_ReordersTheChildren()
    {
        using var parent = new Panel();
        var first = new Button { Name = "first" };
        var second = new Button { Name = "second" };
        parent.Controls!.Add(first);
        parent.Controls.Add(second);

        parent.Controls.SetChildIndex(second, 0);

        Assert.Equal(0, parent.Controls.GetChildIndex(second));
        Assert.Equal(1, parent.Controls.GetChildIndex(first));
    }

    [Fact]
    public void Controls_RemoveAtDropsThatChild()
    {
        using var parent = new Panel();
        parent.Controls!.Add(new Button { Name = "first" });
        parent.Controls.Add(new Button { Name = "second" });

        parent.Controls.RemoveAt(0);

        Assert.Single(parent.Controls);
        Assert.Equal("second", parent.Controls[0].Name);
    }

    [Fact]
    public void Controls_RemoveByKeyDropsTheNamedChild()
    {
        using var parent = new Panel();
        parent.Controls!.Add(new Button { Name = "keep" });
        parent.Controls.Add(new Button { Name = "drop" });

        parent.Controls.RemoveByKey("drop");

        Assert.Single(parent.Controls);
        Assert.Equal("keep", parent.Controls[0].Name);
    }
}
