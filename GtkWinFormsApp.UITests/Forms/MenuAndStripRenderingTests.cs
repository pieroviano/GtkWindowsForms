using GtkWinFormsApp.UITests.Infrastructure;
using System.Drawing;
using System.Windows.Forms;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>
/// Rendering regressions that are only decidable once a form is actually on screen: the client area the
/// window settles at, the size an icon is scaled to from its allocation, and the selected state of the
/// menu entry whose drop-down is open. Each pins a difference the Gtk4 build had against the Gtk3 build.
/// </summary>
public class MenuAndStripRenderingTests
{
    [Fact]
    public void AGroupBoxCaptionLeavesRoomForAChildJustBelowIt()
    {
        // Regression: a child's Location inside a group box is measured from the frame's corner, past the
        // caption band. The themed padding around the caption made that band 24px where WinForms reserves
        // about the font height, so the first radio of a converted VB6 frame - placed at y=16, which clears
        // a WinForms caption - was drawn inside the caption instead of under it.
        UiDriver.Run(
            () =>
            {
                var form = new Form { ClientSize = new Size(417, 220) };
                var group = new GroupBox { Text = "Mode", Location = new Point(8, 8), Size = new Size(161, 65) };
                group.Controls.Add(new RadioButton { Text = "Simple", Location = new Point(8, 16), Size = new Size(137, 17) });
                form.Controls.Add(group);

                return form;
            },
            ctx =>
            {
                UiDriver.Pump(60);

                var caption = ((global::Gtk.Frame)ctx.Form.Controls[0].Widget).LabelWidget;
                Assert.NotNull(caption);
                Assert.True(caption!.Allocation.Height <= 16,
                    $"the caption band is {caption.Allocation.Height}px, so a child at y=16 lands inside it");
            });
    }

    [Fact]
    public void ADropDownListComboKeepsTheWidthTheFormGaveIt()
    {
        // Regression: a DropDownList squashed its entry to 1px and widened the arrow button to the whole
        // control, so the button carried the width instead of the combo. The arrow tore away from the text
        // and the combo's natural width grew past the size the form asked for - a 155px combo was allocated
        // 316px. Making the entry unusable is all a DropDownList needs, and capping the entry's width in
        // characters keeps the combo's natural width off the caption's length, so the form's size is honoured.
        UiDriver.Run(
            () =>
            {
                var form = new Form { ClientSize = new Size(417, 120) };
                var combo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(12, 12),
                    Size = new Size(155, 21),
                };
                combo.Items.Add("Light");
                combo.SelectedIndex = 0;
                form.Controls.Add(combo);

                return form;
            },
            ctx =>
            {
                UiDriver.Pump(60);

                var combo = ctx.Form.Controls[0];

                Assert.Equal(155, combo.Widget.Allocation.Width);
            });
    }

    [Fact]
    public void Form_OpensAtItsDesignedClientSize_SoADockedStripDoesNotEatTheContentGap()
    {
        // Regression: the content-fit pass sized the client to the measured extent of the children, which
        // is self-referential once anything is docked to an edge - a bottom-docked StatusStrip sits at the
        // bottom of whatever the client currently is, so it always measures exactly as tall as the client
        // already is. The client never reached its designed height and the designed gap between the last
        // control and the strip disappeared.
        const int designedHeight = 320;

        UiDriver.Run(
            () =>
            {
                var form = new Form { ClientSize = new Size(420, designedHeight) };
                form.Controls.Add(new StatusStrip { Dock = DockStyle.Bottom, AutoSize = false, Size = new Size(420, 30) });
                form.Controls.Add(new Button { Text = "content", Location = new Point(10, 10), Size = new Size(80, 24) });

                return form;
            },
            ctx =>
            {
                UiDriver.Pump(40);

                Assert.Equal(designedHeight, ctx.Form.ClientSize.Height);
            });
    }

    [Fact]
    public void Form_StillGrowsBeyondTheDesignedClientSize_WhenAControlWouldBeClipped()
    {
        // The floor must not turn into a ceiling: a control below the designed client area still has to
        // force the window to grow, which is the whole reason the content-fit pass exists.
        UiDriver.Run(
            () =>
            {
                var form = new Form { ClientSize = new Size(420, 200) };
                form.Controls.Add(new Button { Text = "low", Location = new Point(10, 320), Size = new Size(80, 24) });

                return form;
            },
            ctx =>
            {
                UiDriver.Pump(40);

                Assert.True(ctx.Form.ClientSize.Height > 200,
                    $"expected the window to grow past its designed height to reveal the control, got {ctx.Form.ClientSize.Height}");
            });
    }

    [Fact]
    public void MenuBarIcon_ScalesFromTheItemHeight_NotTheMinimum()
    {
        // Regression: the icon was scaled inside the "realized" callback, but Gtk4 realizes a widget before
        // it is allocated, so AllocatedHeight was still 0 and every menu-bar icon came out at the 20px
        // minimum - visibly smaller than the Gtk3 build, which scales to half the item height.
        UiDriver.Run(() => new Form3(), ctx =>
        {
            var form = (Form3)ctx.Form;
            UiDriver.Pump(40);

            var item = FirstMenuItemWithIcon(form.menuStrip1);
            Assert.NotNull(item);

            var height = item!.self.MenuItem.AllocatedHeight;
            Assert.True(height > 40, $"expected a tall icon-over-text menu-bar item, got {height}px");
            Assert.NotNull(item.self.Image);
            Assert.True(item.self.Image!.Height > 20,
                $"a {height}px item should scale its icon above the 20px minimum, got {item.self.Image.Height}px");

            // Scaling the pixbuf is not enough on Gtk4: GtkImage renders at the theme icon size (16px)
            // regardless of the pixbuf, so without PixelSize the icon still drew small.
            Assert.Equal(item.self.Image.Height, item.self.IconPixelSize);
        });
    }

    [Fact]
    public void OpeningASubmenu_MarksItsParentItemAsSelected()
    {
        // Regression: on Gtk3 a GtkMenuItem whose submenu is open puts itself in the selected state and the
        // theme paints it. Gtk4 has no menu-item widget, so the entry stayed unpainted and there was no way
        // to tell which item the open submenu belonged to.
        UiDriver.Run(() => new Form3(), ctx =>
        {
            var form = (Form3)ctx.Form;
            UiDriver.Pump(40);

            var parent = FirstMenuItemWithSubmenu(form.menuStrip1);
            Assert.NotNull(parent);
            Assert.False(parent!.self.IsDropDownOpenStyled, "nothing is open yet");

            parent.self.ShowDropDown();
            UiDriver.Pump(20);

            Assert.True(parent.self.IsDropDownOpenStyled,
                "the item whose drop-down is showing must read as selected");

            // The popover is parented to the button, so anything inheritable styled there cascades into
            // the menu it opens. That is exactly what happened once: the selected foreground colour reached
            // every caption in the drop-down and painted it white on white, so the menu rendered empty. The
            // foreground must live on the caption row, which is not an ancestor of the popover.
            Assert.False(parent.self.DropDownHost.StyleContext.HasClass(StripMenuItem.DropDownOpenRowClass),
                "the popover's host must not carry an inheritable foreground colour");

            parent.self.HideDropDown();
            UiDriver.Pump(20);

            Assert.False(parent.self.IsDropDownOpenStyled, "closing the drop-down clears the selection");
        });
    }

    [Fact]
    public void ToolStripTextBox_IsSizedByItsDesignedSize_NotTheThemesEntryMetrics()
    {
        // Regression: a ToolStripTextBox is a bare GtkEntry and nothing passed the item's Size down to it,
        // so it fell back to natural sizing - a couple of characters wide - while Adwaita's entry metrics
        // made it 42px tall. Being the tallest item it inflated the whole strip past its designed height
        // and pushed the trailing items off the right edge. ToolStripComboBox had always applied its size;
        // the text box was missing the counterpart.
        const int width = 120;
        const int height = 33;

        ToolStripTextBox? box = null;
        ToolStrip? strip = null;

        UiDriver.Run(
            () =>
            {
                var form = new Form { ClientSize = new Size(500, 200) };
                strip = new ToolStrip { Dock = DockStyle.Top, AutoSize = false, Size = new Size(500, height) };
                box = new ToolStripTextBox { Size = new Size(width, height) };
                strip.Items.Add(box);
                form.Controls.Add(strip);

                return form;
            },
            ctx =>
            {
                UiDriver.Pump(40);

                var widget = box!.Widget.ToolItem;
                Assert.Equal(width, widget.AllocatedWidth);

                widget.Measure(Gtk.Orientation.Vertical, -1, out var min, out _, out _, out _);
                Assert.True(min <= height,
                    $"the text box must not demand more than its designed {height}px height, got {min}px");

                strip!.self.Measure(Gtk.Orientation.Vertical, -1, out var stripMin, out _, out _, out _);
                Assert.True(stripMin <= height + 2,
                    $"the strip must not be inflated past its designed {height}px height, got {stripMin}px");
            });
    }

    private static ToolStripMenuItem? FirstMenuItemWithIcon(MenuStrip strip)
    {
        foreach (var item in strip.Items)
        {
            if (item is ToolStripMenuItem menuItem && menuItem.Image != null)
            {
                return menuItem;
            }
        }

        return null;
    }

    private static ToolStripMenuItem? FirstMenuItemWithSubmenu(MenuStrip strip)
    {
        foreach (var item in strip.Items)
        {
            if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
            {
                return menuItem;
            }
        }

        return null;
    }
}
