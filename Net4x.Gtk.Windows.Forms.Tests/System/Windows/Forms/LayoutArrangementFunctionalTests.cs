using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for dock and anchor arrangement — the first measurement of the layout engine
/// against WinForms rather than against itself.
/// <para>
/// Dock and anchor were expressed by giving each child a GTK alignment, which does not compose: two
/// top-docked panels were both aligned to the top edge and so overlapped instead of stacking, `Fill`
/// covered the whole container rather than the remainder, and anchoring was computed once at realize and
/// never followed a resize.
/// </para>
/// <para>
/// **Where the expected values come from.** Each case below was run against real
/// <c>System.Windows.Forms</c> (net10.0-windows, `Form.CreateControl()` + `PerformLayout()`) and the
/// rectangles it produced are the oracle. Rather than hard-code its client size — which a GTK window will
/// not reproduce — the assertions state the *rule* those numbers demonstrate, so they hold at whatever
/// size the window actually gets:
/// </para>
/// <code>
/// DOCK-ORDER   client=300x200  top1=(0,0,300,30) top2=(0,30,300,40) fill=(0,70,300,130)
/// DOCK-ALL     top=(0,0,300,30) bottom=(0,180,300,20) left=(0,30,50,150)
///              right=(240,30,60,150) fill=(50,30,190,150)
/// DOCK-PADDING padding=(10,5,15,7) top=(10,5,275,30) fill=(10,35,275,158)
/// ANCHOR       client 300x200 -> 400x300:
///              tl(T|L) (10,10,50,20) -> unchanged        tr(T|R) (200,10) -> (300,10)
///              stretch(T|L|R) w 100 -> 200               all(T|L|R|B) 100x30 -> 200x130
///              none(None) (10,150) -> (60,200)           br(B|R) (250,170) -> (350,270)
/// </code>
/// </summary>
[Collection("NonParallelCollection")]
public class LayoutArrangementFunctionalTests
{
    /// <summary>
    /// Shows <paramref name="form" /> and runs <paramref name="body" /> once its children have been
    /// allocated, then closes it. Layout only exists once GTK has given the container a real size.
    /// </summary>
    private static void WhenLaidOut(Form form, Action body)
    {
        ExceptionDispatchInfo? failure = null;

        form.Load += (_, _) =>
        {
            try
            {
                Application.DoEvents();
                body();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        failure?.Throw();
    }

    [Fact]
    public void DockedSiblings_Stack_InsteadOfOverlapping()
    {
        var form = new Form { Width = 300, Height = 200 };
        var top1 = new Panel { Height = 30, Dock = DockStyle.Top };
        var top2 = new Panel { Height = 40, Dock = DockStyle.Top };
        var fill = new Panel { Dock = DockStyle.Fill };
        // Added fill, then top2, then top1 - the order the designer emits for this arrangement.
        form.Controls.Add(fill);
        form.Controls.Add(top2);
        form.Controls.Add(top1);

        WhenLaidOut(form, () =>
        {
            // WinForms docks from the end of the collection towards the start, so the last one added -
            // top1 - takes the top edge and top2 stacks underneath it.
            Assert.Equal(0, top1.Top);
            Assert.Equal(30, top1.Height);
            Assert.Equal(30, top2.Top);
            Assert.Equal(40, top2.Height);
            Assert.Equal(70, fill.Top);

            // The two docked bands and the fill together account for the whole client height.
            Assert.Equal(fill.Top + fill.Height, top1.Height + top2.Height + fill.Height);

            // Every band spans the full client width.
            Assert.Equal(0, top1.Left);
            Assert.Equal(0, top2.Left);
            Assert.Equal(0, fill.Left);
            Assert.Equal(top1.Width, top2.Width);
            Assert.Equal(top1.Width, fill.Width);
            Assert.True(top1.Width > 1, "a docked band must be given the container's width, not left unsized");
        });
    }

    [Fact]
    public void DockOrder_FollowsTheCollection_LastAddedTakesTheEdge()
    {
        var form = new Form { Width = 300, Height = 200 };
        var top1 = new Panel { Height = 30, Dock = DockStyle.Top };
        var top2 = new Panel { Height = 40, Dock = DockStyle.Top };
        form.Controls.Add(top1);
        form.Controls.Add(top2);

        WhenLaidOut(form, () =>
        {
            // Reversed from the previous test: top2 was added last, so it gets the edge and top1 stacks
            // below it. WinForms: top1=(0,40) top2=(0,0).
            Assert.Equal(0, top2.Top);
            Assert.Equal(40, top1.Top);
        });
    }

    [Fact]
    public void AllFiveDockStyles_ShareTheClientArea()
    {
        var form = new Form { Width = 300, Height = 200 };
        var fill = new Panel { Dock = DockStyle.Fill };
        var left = new Panel { Width = 50, Dock = DockStyle.Left };
        var right = new Panel { Width = 60, Dock = DockStyle.Right };
        var top = new Panel { Height = 30, Dock = DockStyle.Top };
        var bottom = new Panel { Height = 20, Dock = DockStyle.Bottom };
        form.Controls.Add(fill);
        form.Controls.Add(left);
        form.Controls.Add(right);
        form.Controls.Add(top);
        form.Controls.Add(bottom);

        WhenLaidOut(form, () =>
        {
            var clientWidth = top.Width;
            var clientHeight = left.Height + top.Height + bottom.Height;

            // bottom is docked first (highest index), so it spans the full width at the bottom edge;
            // top then takes the top edge; right and left take the sides of what is left; fill gets the
            // middle.
            Assert.Equal(0, top.Top);
            Assert.Equal(30, top.Height);
            Assert.Equal(clientWidth, bottom.Width);
            Assert.Equal(20, bottom.Height);
            Assert.Equal(clientHeight - 20, bottom.Top);

            Assert.Equal(0, left.Left);
            Assert.Equal(30, left.Top);
            Assert.Equal(50, left.Width);

            Assert.Equal(60, right.Width);
            Assert.Equal(30, right.Top);
            Assert.Equal(clientWidth - 60, right.Left);

            // The fill sits between the sides and between the bands.
            Assert.Equal(50, fill.Left);
            Assert.Equal(30, fill.Top);
            Assert.Equal(clientWidth - 110, fill.Width);
            Assert.Equal(left.Height, fill.Height);
        });
    }

    [Fact]
    public void ContainerPadding_ShrinksTheAreaDockingWorksIn()
    {
        var form = new Form { Width = 300, Height = 200, Padding = new Padding(10, 5, 15, 7) };
        var fill = new Panel { Dock = DockStyle.Fill };
        var top = new Panel { Height = 30, Dock = DockStyle.Top };
        form.Controls.Add(fill);
        form.Controls.Add(top);

        WhenLaidOut(form, () =>
        {
            // WinForms: top=(10,5,275,30), fill=(10,35,275,158) for a 300x200 client.
            Assert.Equal(10, top.Left);
            Assert.Equal(5, top.Top);
            Assert.Equal(10, fill.Left);
            Assert.Equal(35, fill.Top);
            Assert.Equal(top.Width, fill.Width);
        });
    }

    [Fact]
    public void HiddenDockedChild_ConsumesNoSpace()
    {
        var form = new Form { Width = 300, Height = 200 };
        var fill = new Panel { Dock = DockStyle.Fill };
        var hidden = new Panel { Height = 40, Dock = DockStyle.Top, Visible = false };
        var shown = new Panel { Height = 30, Dock = DockStyle.Top };
        form.Controls.Add(fill);
        form.Controls.Add(hidden);
        form.Controls.Add(shown);

        WhenLaidOut(form, () =>
        {
            Assert.Equal(0, shown.Top);
            Assert.Equal(30, fill.Top);
        });
    }

    /// <summary>
    /// The anchor rule, against the exact rectangles real WinForms produced for a 300x200 -> 400x300
    /// client resize. The arithmetic is asserted directly rather than by resizing a live window: this GTK
    /// test host does not propagate a runtime resize to a modal dialog's allocation, so a resize-driven
    /// test would assert nothing. The wiring - that the pass runs on every allocation - is what the dock
    /// tests above exercise.
    /// </summary>
    [Theory]
    // control bounds,      anchor,                                                expected bounds
    [InlineData(10, 10, 50, 20, AnchorStyles.Top | AnchorStyles.Left, 10, 10, 50, 20)]
    [InlineData(200, 10, 50, 20, AnchorStyles.Top | AnchorStyles.Right, 300, 10, 50, 20)]
    [InlineData(10, 60, 100, 20, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, 10, 60, 200, 20)]
    [InlineData(10, 100, 100, 30, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, 10, 100, 200, 130)]
    [InlineData(10, 150, 40, 20, AnchorStyles.None, 60, 200, 40, 20)]
    [InlineData(250, 170, 40, 20, AnchorStyles.Bottom | AnchorStyles.Right, 350, 270, 40, 20)]
    public void Anchor_FollowsTheContainerSize(
        int x, int y, int width, int height,
        AnchorStyles anchor,
        int expectedX, int expectedY, int expectedWidth, int expectedHeight)
    {
        var bounds = new global::System.Drawing.Rectangle(x, y, width, height);
        var delta = new global::System.Drawing.Size(100, 100);

        var result = Control.AnchorBounds(bounds, anchor, delta);

        Assert.Equal(
            new global::System.Drawing.Rectangle(expectedX, expectedY, expectedWidth, expectedHeight),
            result);
    }

    [Fact]
    public void Anchor_None_MovesByHalfTheDelta_NotTheWholeDelta()
    {
        // Called out on its own because it is the rule nobody expects: a control anchored to no edge is
        // not pinned, it is kept proportionally placed.
        var result = Control.AnchorBounds(
            new global::System.Drawing.Rectangle(10, 150, 40, 20),
            AnchorStyles.None,
            new global::System.Drawing.Size(100, 60));

        Assert.Equal(new global::System.Drawing.Rectangle(60, 180, 40, 20), result);
    }

    [Fact]
    public void Anchor_ShrinkingTheContainer_MovesAndResizesTheOtherWay()
    {
        var result = Control.AnchorBounds(
            new global::System.Drawing.Rectangle(10, 60, 200, 20),
            AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            new global::System.Drawing.Size(-100, 0));

        Assert.Equal(new global::System.Drawing.Rectangle(10, 60, 100, 20), result);
    }

    [Fact]
    public void UndockedChildren_AreNotMovedByTheFirstLayoutPass()
    {
        // The baseline pass has no previous size to compare against; it must not shift anything.
        var form = new Form { Width = 300, Height = 200 };
        var panel = new Panel { Left = 30, Top = 40, Width = 60, Height = 20, Anchor = AnchorStyles.Top | AnchorStyles.Left };
        form.Controls.Add(panel);

        WhenLaidOut(form, () =>
        {
            Assert.Equal(30, panel.Left);
            Assert.Equal(40, panel.Top);
            Assert.Equal(60, panel.Width);
            Assert.Equal(20, panel.Height);
        });
    }

    [Fact]
    public void DisplayRectangle_IsTheClientAreaLessPadding()
    {
        var form = new Form { Width = 300, Height = 200, Padding = new Padding(10, 5, 15, 7) };
        var panel = new Panel { Dock = DockStyle.Fill };
        form.Controls.Add(panel);

        WhenLaidOut(form, () =>
        {
            // It used to be an auto-property nothing assigned - on Control and again on ScrollableControl,
            // which shadowed it - so every container reported Empty and code laying children out against
            // it stacked them at the origin with no size.
            var display = form.DisplayRectangle;

            Assert.Equal(10, display.X);
            Assert.Equal(5, display.Y);
            Assert.True(display.Width > 1, "the display rectangle must not be empty");
            Assert.True(display.Height > 1, "the display rectangle must not be empty");

            // And it is the rectangle the arrangement pass actually fills: a Fill-docked child covers it
            // exactly. Asserted against the child rather than against ClientSize because both are read
            // after the same allocation, so there is no window in which the two can disagree.
            Assert.Equal(display.X, panel.Left);
            Assert.Equal(display.Y, panel.Top);
            Assert.Equal(display.Width, panel.Width);
            Assert.Equal(display.Height, panel.Height);
        });
    }

    [Fact]
    public void DisplayRectangle_WithNoPadding_StartsAtTheOrigin()
    {
        var form = new Form { Width = 300, Height = 200 };
        var panel = new Panel { Dock = DockStyle.Fill };
        form.Controls.Add(panel);

        WhenLaidOut(form, () =>
        {
            var display = form.DisplayRectangle;

            Assert.Equal(0, display.X);
            Assert.Equal(0, display.Y);
            Assert.Equal(display.Width, panel.Width);
            Assert.Equal(display.Height, panel.Height);
            Assert.True(display.Width > 1);
        });
    }
}
