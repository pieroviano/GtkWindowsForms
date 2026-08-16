using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Covers where a <see cref="GroupBox" /> puts an explicitly located child.
/// <para>
/// A group box is a <see cref="Gtk.Frame" /> whose caption is laid out <em>above</em> its child, so the
/// overlay that children are positioned in starts a whole label band below the frame's top-left corner.
/// WinForms measures a child's <see cref="Control.Location" /> from the control's corner, caption or no
/// caption, so without compensation every child is pushed down by that band and the bottom-most one is
/// clipped by the group box. The Gtk3 build cancelled the band from <c>::size-allocate</c>; that handler
/// was dropped in the Gtk4 port because the signal no longer exists.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class GroupBoxCaptionBandTests
{
    /// <summary>
    /// Initializes GTK before any widget is constructed.
    /// </summary>
    public GroupBoxCaptionBandTests()
    {
        Application.Init();
    }

    [Fact]
    public void ChildAtTheBottomOfACaptionedGroupBox_StaysAtItsDesignedLocation()
    {
        const int childTop = 70;
        const int childLeft = 10;
        const int childHeight = 25;

        var groupBox = new GroupBox
        {
            Text = "captioned",
            Location = new Point(10, 10),
            Size = new Size(200, 100)
        };
        var child = new Button
        {
            Location = new Point(childLeft, childTop),
            Size = new Size(80, childHeight)
        };
        groupBox.Controls.Add(child);

        var placement = default(Gdk.Rectangle);
        var frameHeight = 0;

        WhenLaidOut(groupBox, () =>
        {
            placement = PlacementInGroupBox(groupBox, child);
            frameHeight = groupBox.self.Allocation.Height;
        });

        // Two pixels of slack: GTK rounds an allocation, it does not honour a request to the pixel.
        Assert.InRange(placement.X, childLeft - 2, childLeft + 2);
        Assert.InRange(placement.Y, childTop - 2, childTop + 2);

        // The designed layout fits, so nothing may be pushed past the bottom of the frame.
        Assert.True(placement.Y + placement.Height <= frameHeight,
            $"child bottom {placement.Y + placement.Height} is clipped by a group box only {frameHeight} tall");
        Assert.True(childHeight <= placement.Height, "the child was allocated less than its designed height");
    }

    [Fact]
    public void ChildOfAnUncaptionedGroupBox_IsNotOffsetEither()
    {
        const int childTop = 20;

        var groupBox = new GroupBox { Location = new Point(10, 10), Size = new Size(200, 100) };
        var child = new Button { Location = new Point(5, childTop), Size = new Size(80, 25) };
        groupBox.Controls.Add(child);

        var placement = default(Gdk.Rectangle);

        WhenLaidOut(groupBox, () => placement = PlacementInGroupBox(groupBox, child));

        Assert.InRange(placement.Y, childTop - 2, childTop + 2);
    }

    /// <summary>
    /// The rectangle <paramref name="child" /> ended up occupying, measured from the corner of
    /// <paramref name="groupBox" /> - which is where WinForms measures a child's
    /// <see cref="Control.Location" /> from.
    /// </summary>
    private static Gdk.Rectangle PlacementInGroupBox(GroupBox groupBox, Control child)
    {
        // The content overlay is the coordinate space the child is positioned in; where that space
        // sits inside the frame is exactly what the caption band displaces.
        Assert.True(groupBox.self.contaner.TranslateCoordinates(groupBox.self, 0, 0, out var originX, out var originY),
            "the group box content was never allocated");

        var allocation = child.Widget!.Allocation;

        return new Gdk.Rectangle((int)originX + allocation.X, (int)originY + allocation.Y,
            allocation.Width, allocation.Height);
    }

    /// <summary>
    /// Shows <paramref name="groupBox" /> on a form, runs <paramref name="body" /> once GTK has settled
    /// the allocation, then closes the form. Placement only exists after a real layout pass.
    /// </summary>
    private static void WhenLaidOut(GroupBox groupBox, Action body)
    {
        var form = new Form { Width = 300, Height = 220 };
        form.Controls.Add(groupBox);

        try
        {
            form.Show();

            // The children are placed when the overlay is mapped and the allocation only settles once
            // GTK has run a layout pass, so pump rather than assume one iteration is enough.
            for (var i = 0; i < 200; i++)
            {
                Application.DoEvents();
            }

            body();
        }
        finally
        {
            form.Close();
            Application.DoEvents();
        }
    }
}
