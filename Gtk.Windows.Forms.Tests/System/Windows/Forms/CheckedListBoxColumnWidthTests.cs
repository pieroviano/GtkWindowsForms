using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Forms;

/// <summary>
/// Covers the Gtk4 child-slot semantics that made <see cref="CheckedListBox.ColumnWidth" />
/// a no-op, and the <see cref="FlowLayoutPanel" /> collection storing widget wrappers instead
/// of the controls that were added to it.
/// </summary>
[Collection("NonParallelCollection")]
public class CheckedListBoxColumnWidthTests
{
    /// <summary>
    /// Initializes GTK before any widget is constructed.
    /// </summary>
    public CheckedListBoxColumnWidthTests()
    {
        Application.Init();
    }

    [Fact]
    public void LabelledCheckButton_ExposesItsLabel_EvenThoughChildSlotIsEmpty()
    {
        const string caption = "a very long caption that would never fit a narrow column";
        var checkButton = new Gtk.CheckButton { Label = caption };

        // Gtk4: gtk_check_button_set_label() keeps the label outside the child slot, so the Gtk3
        // idiom `Child is Gtk.Label` never matches.
        Assert.Null(checkButton.Child);

        var label = checkButton.GetLabelWidget();
        Assert.NotNull(label);
        Assert.Equal(caption, label!.Text);
    }

    [Fact]
    public void GetLabelWidget_ReturnsChildSlotLabel_AndNullWhenThereIsNoCaption()
    {
        var slotLabel = new Gtk.Label("in the child slot");
        var button = new Gtk.Button { Child = slotLabel };
        Assert.Equal(slotLabel.Handle, button.GetLabelWidget()!.Handle);

        Assert.Null(new Gtk.Box(Gtk.Orientation.Horizontal, 0).GetLabelWidget());
        Assert.Null(((Gtk.Widget?)null).GetLabelWidget());
    }

    [Fact]
    public void MultiColumn_ClampsEveryItemToColumnWidth_AndEllipsizesTheCaption()
    {
        var form = new Form();
        var clb = new CheckedListBox
        {
            MultiColumn = true,
            ColumnWidth = 70,
            Size = new Drawing.Size(270, 92)
        };
        form.Controls.Add(clb);

        var minimums = new List<int>();
        var ellipsizeModes = new List<Pango.EllipsizeMode>();

        form.Shown += (_, _) =>
        {
            foreach (var text in new[]
                     {
                         "Inner Mongolia Autonomous Region",
                         "Heilongjiang Province",
                         "Beijing",
                         "Guangxi Zhuang Autonomous Region"
                     })
            {
                clb.Items.Add(text);
            }

            GLib.Timeout.Add(80, () =>
            {
                foreach (var child in clb._flow.GetChildrenArray().OfType<Gtk.FlowBoxChild>())
                {
                    child.Measure(Gtk.Orientation.Horizontal, -1, out var minimum, out _, out _, out _);
                    minimums.Add(minimum);
                    var label = child.Child.GetLabelWidget();
                    if (label != null)
                        ellipsizeModes.Add(label.Ellipsize);
                }

                form.Close();
                return false;
            });
        };

        form.ShowDialog();

        Assert.Equal(4, minimums.Count);
        // Without the clamp the natural text width wins and every item is far wider than 70px.
        Assert.All(minimums, m => Assert.Equal(clb.ColumnWidth, m));
        Assert.Equal(4, ellipsizeModes.Count);
        Assert.All(ellipsizeModes, m => Assert.Equal(Pango.EllipsizeMode.End, m));
    }

    [Fact]
    public void FlowLayoutPanel_Controls_EnumeratesTheControlsThatWereAdded()
    {
        var flp = new FlowLayoutPanel();
        var checkBox = new CheckBox { Name = "c1" };
        var button = new Button { Name = "b1" };

        flp.Controls.Add(checkBox);
        flp.Controls.Add(button);

        Assert.Equal(2, flp.Controls.Count);
        Assert.Same(checkBox, flp.Controls[0]);
        Assert.Same(button, flp.Controls[1]);
        Assert.Contains(checkBox, flp.Controls.Cast<object>().OfType<Control>());
        Assert.Same(flp, checkBox.Parent);
    }
}
