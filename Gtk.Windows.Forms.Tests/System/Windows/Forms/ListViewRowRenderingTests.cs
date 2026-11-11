#pragma warning disable CS0612

#pragma warning disable CS0612

extern alias gtk;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Forms;
using Bitmap = gtk::System.Drawing.Bitmap;
using Size = global::System.Drawing.Size;
using Color = global::System.Drawing.Color;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Rendering regressions for <see cref="ListView" /> rows on Gtk4. Snapshots of the sample on both engines
/// showed the Gtk3 build drawing seven captioned rows where the Gtk4 build drew two rows with an icon and
/// no text at all. Two independent porting losses stacked up:
/// <list type="number">
/// <item><description>
/// Gtk3 packed the scroll area with <c>PackStart(scrolledWindow, expand: true, fill: true, 0)</c>. Gtk4's
/// <c>Box.Append</c> has no expand argument - expansion is the child's own <c>Vexpand</c> property - so the
/// scroller was granted only its (tiny) natural height and every row past the first couple was scrolled out
/// of view.
/// </description></item>
/// <item><description>
/// Gtk3's <c>GtkButton</c> carried an image <em>and</em> a label; a Gtk4 check button has a single content
/// slot, so the ported <c>checkButton.Child = new Gtk.Image(...)</c> destroyed the caption label outright.
/// </description></item>
/// </list>
/// </summary>
[Collection("NonParallelCollection")]
public class ListViewRowRenderingTests
{
    // These tests build real GTK widgets, so the type system has to be initialised first.
    public ListViewRowRenderingTests() => Application.Init();

    private const int ItemCount = 7;

    private static Bitmap Swatch(int size = 16)
    {
        var bmp = new Bitmap(size, size);
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                bmp.SetPixel(x, y, Color.Red);
            }
        }

        return bmp;
    }

    /// <summary>
    /// Builds a details-view list of <see cref="ItemCount" /> rows - with a small icon on each row when
    /// <paramref name="withImages" /> says so - shows it, and runs <paramref name="body" /> once GTK has
    /// laid it out.
    /// </summary>
    private static void WithLaidOutListView(bool withImages, Action<ListView> body, Color? foreColor = null)
    {
        var form = new Form { Width = 420, Height = 280 };
        var listView = new ListView
        {
            View = View.Details,
            CheckBoxes = true,
            Dock = DockStyle.Fill
        };

        listView.Columns.Add("caption", 200);
        listView.Columns.Add("second", 90);
        listView.Columns.Add("third", 90);

        ImageList? images = null;
        if (withImages)
        {
            images = new ImageList { ImageSize = new Size(16, 16) };
            images.Images.Add(Swatch());
            listView.SmallImageList = images;
        }

        for (var i = 0; i < ItemCount; i++)
        {
            var item = new ListViewItem(new[] { $"row{i}", $"second{i}", $"third{i}" }, withImages ? 0 : -1);
            if (foreColor.HasValue)
            {
                item.ForeColor = foreColor.Value;
            }

            listView.Items.Add(item);
        }

        form.Controls.Add(listView);

        try
        {
            form.Show();

            // The rows are built when the widget is realized (ListView.Control_Realized), and their
            // allocation only settles once GTK has run a layout pass, so pump the loop rather than
            // assuming one iteration is enough.
            for (var i = 0; i < 200; i++)
            {
                Application.DoEvents();
            }

            body(listView);
        }
        finally
        {
            form.Close();
            Application.DoEvents();
            images?.Dispose();
        }
    }

    private static IEnumerable<Gtk.Widget> Descendants(Gtk.Widget parent)
    {
        for (var child = parent.FirstChild; child != null; child = child.NextSibling)
        {
            yield return child;
            foreach (var grandChild in Descendants(child))
            {
                yield return grandChild;
            }
        }
    }

    /// <summary>The check button that carries a row's caption - the first cell of the row.</summary>
    private static Gtk.CheckButton CheckButtonOf(ListViewItem item)
    {
        Assert.NotNull(item._flowBoxChild);
        var button = Descendants(item._flowBoxChild!).OfType<Gtk.CheckButton>().FirstOrDefault();
        Assert.True(button != null, $"row '{item.Text}' has no check button - its first cell was not built");
        return button!;
    }

    /// <summary>
    /// The caption a row actually renders: the check button's own label when it still owns one, otherwise
    /// the label sharing its content box with the icon. An empty string means the caption is not on screen.
    /// </summary>
    private static string CaptionOf(ListViewItem item)
    {
        var button = CheckButtonOf(item);
        if (!string.IsNullOrEmpty(button.Label))
        {
            return button.Label;
        }

        return Descendants(button)
                   .OfType<Gtk.Label>()
                   .Select(l => l.Text)
                   .FirstOrDefault(t => !string.IsNullOrEmpty(t))
               ?? string.Empty;
    }

    [Fact]
    public void ScrollArea_ClaimsTheHeightTheBoxHasLeftOver_SoEveryRowIsReachable()
    {
        // Fault 1: without Vexpand the vertical box hands the scroller its natural height only (a couple of
        // rows' worth), and the remaining rows sit below the visible viewport for good.
        WithLaidOutListView(withImages: true, listView =>
        {
            var visible = listView.scrolledWindow.Allocation.Height;
            var content = listView.flowBoxContainer.Allocation.Height;

            Assert.True(content > 0, "the rows should have been allocated some height");
            Assert.True(visible >= content,
                $"the scroll area shows {visible}px of {content}px of rows - most rows are scrolled out of sight");

            // The direct statement of the dropped Gtk3 pack flag, in case a headless run ever makes the
            // allocations above unreliable.
            Assert.True(listView.scrolledWindow.Vexpand,
                "the scroll area must expand: Gtk3 packed it with expand:true, and Gtk4's Box.Append has no such argument");
        });
    }

    [Fact]
    public void EveryItemKeepsItsCaption_WhenTheItemsCarryAnIcon()
    {
        // Fault 2: assigning Gtk.CheckButton.Child destroys the built-in label, so every captioned row went
        // blank as soon as a SmallImageList was present.
        WithLaidOutListView(withImages: true, listView =>
        {
            foreach (ListViewItem item in listView.Items)
            {
                Assert.Equal(item.Text, CaptionOf(item));
            }
        });
    }

    [Fact]
    public void ItemsKeepTheirCaption_WithNoImageList()
    {
        // The icon-less path must stay exactly as it was: the check button's own label carries the text.
        WithLaidOutListView(withImages: false, listView =>
        {
            foreach (ListViewItem item in listView.Items)
            {
                Assert.Equal(item.Text, CaptionOf(item));
            }
        });
    }

    [Fact]
    public void CaptionAndIcon_ShareTheCheckButtonsContent()
    {
        // Guards against "fixing" the caption by dropping the icon: both have to live inside the button.
        WithLaidOutListView(withImages: true, listView =>
        {
            foreach (ListViewItem item in listView.Items)
            {
                var content = Descendants(CheckButtonOf(item)).ToList();

                Assert.True(content.OfType<Gtk.Image>().Any(), $"row '{item.Text}' lost its icon");
                Assert.True(content.OfType<Gtk.Label>().Any(l => l.Text == item.Text),
                    $"row '{item.Text}' has no label carrying its caption");
            }
        });
    }

    [Fact]
    public void ItemForeColor_LandsOnTheLabelThatRendersTheCaption()
    {
        // The fore colour used to be applied to the built-in label just before the icon assignment destroyed
        // it, so a coloured item rendered in the default colour - when it rendered at all.
        WithLaidOutListView(withImages: true, listView =>
        {
            foreach (ListViewItem item in listView.Items)
            {
                var caption = Descendants(CheckButtonOf(item)).OfType<Gtk.Label>().First(l => l.Text == item.Text);

                Assert.True(caption.Attributes != null,
                    $"row '{item.Text}' renders through a label that never received its ForeColor");
            }
        }, foreColor: Color.Blue);
    }
}

#pragma warning restore CS0612
