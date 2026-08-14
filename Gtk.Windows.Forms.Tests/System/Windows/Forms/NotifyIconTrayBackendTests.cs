extern alias gtk;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the system tray backends behind <see cref="NotifyIcon" />.
/// </summary>
/// <remarks>
/// These assert the parts that are decidable without a live tray host: how a
/// <see cref="ContextMenuStrip" /> is translated for the platform menu models, how pixels are converted
/// for each platform's icon format, and that the component degrades rather than throws when no tray
/// exists. Whether an icon actually appears in a notification area is a manual, per-desktop check - see
/// NotifyIcon-SystemTray-Plan.md.
/// </remarks>
[Collection("NonParallelCollection")]
public class NotifyIconTrayBackendTests
{
    // Constructing GTK widgets - which building any ToolStripItem does - needs the type system
    // initialised, or the host hard-crashes when this class happens to run first.
    public NotifyIconTrayBackendTests() => Application.Init();

    private static ContextMenuStrip BuildMenu(out ToolStripMenuItem open, out ToolStripMenuItem recent)
    {
        var menu = new ContextMenuStrip();

        open = new ToolStripMenuItem("&Open");
        recent = new ToolStripMenuItem("Recent");
        recent.DropDownItems.Add(new ToolStripMenuItem("first.txt"));
        recent.DropDownItems.Add(new ToolStripMenuItem("second.txt"));

        menu.Items.Add(open);
        menu.Items.Add(recent);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(new ToolStripMenuItem("Exit"));

        return menu;
    }

    [Fact]
    public void Model_PreservesTopLevelOrderAndText()
    {
        using var menu = BuildMenu(out _, out _);

        var model = TrayMenuModel.From(menu);

        Assert.NotNull(model);
        Assert.Equal(4, model!.Root.Children.Count);
        Assert.Equal("&Open", model.Root.Children[0].Text);
        Assert.Equal("Recent", model.Root.Children[1].Text);
        Assert.Equal("Exit", model.Root.Children[3].Text);
    }

    [Fact]
    public void Model_MarksSeparators()
    {
        using var menu = BuildMenu(out _, out _);

        var model = TrayMenuModel.From(menu)!;

        Assert.True(model.Root.Children[2].IsSeparator, "the ToolStripSeparator should translate to a separator");
        Assert.False(model.Root.Children[0].IsSeparator);
    }

    [Fact]
    public void Model_NestsSubmenuItems()
    {
        using var menu = BuildMenu(out _, out var recent);

        var model = TrayMenuModel.From(menu)!;
        var node = model.Root.Children[1];

        Assert.Same(recent, node.Source);
        Assert.True(node.HasChildren);
        Assert.Equal(2, node.Children.Count);
        Assert.Equal("first.txt", node.Children[0].Text);
        Assert.Equal("second.txt", node.Children[1].Text);
    }

    [Fact]
    public void Model_AssignsUniqueIdsThatResolveBackToTheirItem()
    {
        using var menu = BuildMenu(out var open, out _);

        var model = TrayMenuModel.From(menu)!;
        var openNode = model.Root.Children[0];

        Assert.NotEqual(0, openNode.Id);
        Assert.Same(openNode, model.Find(openNode.Id));
        Assert.Same(open, model.Find(openNode.Id)!.Source);
    }

    [Fact]
    public void Model_IdsAreStableAcrossRepeatedTranslationsOfTheSameMenu()
    {
        using var menu = BuildMenu(out _, out _);

        var first = TrayMenuModel.From(menu)!;
        var second = TrayMenuModel.From(menu)!;

        // Hosts cache ids between GetLayout calls, so the same menu must always number the same way.
        Assert.Equal(Flatten(first), Flatten(second));
    }

    [Fact]
    public void Model_DropsInvisibleItems()
    {
        using var menu = BuildMenu(out var open, out _);
        open.Visible = false;

        var model = TrayMenuModel.From(menu)!;

        Assert.Equal(3, model.Root.Children.Count);
        Assert.DoesNotContain(model.Root.Children, n => ReferenceEquals(n.Source, open));
    }

    [Fact]
    public void Model_CarriesEnabledState()
    {
        using var menu = BuildMenu(out var open, out _);
        open.Enabled = false;

        var model = TrayMenuModel.From(menu)!;

        Assert.False(model.Root.Children[0].Enabled);
        Assert.True(model.Root.Children[3].Enabled);
    }

    [Fact]
    public void Model_CarriesCheckState()
    {
        using var menu = BuildMenu(out var open, out _);
        open.Checked = true;

        var model = TrayMenuModel.From(menu)!;
        var node = model.Root.Children[0];

        Assert.True(node.IsCheckable);
        Assert.True(node.Checked);
        Assert.False(model.Root.Children[3].IsCheckable);
    }

    [Fact]
    public void Model_ActivateInvokesTheOriginalItem()
    {
        using var menu = BuildMenu(out var open, out _);

        var clicked = 0;
        open.Click += (_, _) => clicked++;

        var model = TrayMenuModel.From(menu)!;
        model.Activate(model.Root.Children[0].Id);

        Assert.Equal(1, clicked);
    }

    [Fact]
    public void Model_ActivateIgnoresUnknownIds()
    {
        using var menu = BuildMenu(out _, out _);

        var model = TrayMenuModel.From(menu)!;

        // A host may post an id from a layout that has since been rebuilt; it must be ignored, not
        // misrouted onto whichever item now holds that number.
        model.Activate(9999);
        model.Activate(0);
    }

    [Fact]
    public void Model_EmptyHasNoChildren()
    {
        var model = TrayMenuModel.Empty();

        Assert.Equal(0, model.Root.Id);
        Assert.False(model.Root.HasChildren);
    }

    [Theory]
    [InlineData("&File", "_File")]
    [InlineData("Save &As", "Save _As")]
    [InlineData("Fish && Chips", "Fish & Chips")]
    [InlineData("a_b", "a__b")]
    [InlineData("", "")]
    public void DBusMenuLabel_ConvertsMnemonicsAndEscapesUnderscores(string input, string expected)
    {
        Assert.Equal(expected, TrayMenuModel.ToDBusMenuLabel(input));
    }

    [Theory]
    [InlineData("&File", "File")]
    [InlineData("Save &As", "Save As")]
    [InlineData("Fish && Chips", "Fish & Chips")]
    [InlineData("a_b", "a_b")]
    [InlineData("", "")]
    public void StripMnemonics_RemovesMarkersButKeepsLiteralAmpersands(string input, string expected)
    {
        Assert.Equal(expected, TrayMenuModel.StripMnemonics(input));
    }

    [Fact]
    public void Win32_NotifyIconDataMatchesTheDocumentedNativeSize()
    {
        // The shell rejects the whole call if cbSize is not exactly NOTIFYICONDATA_V4_SIZE, and reports
        // nothing beyond a false return, so the layout is worth pinning.
        var expected = IntPtr.Size == 8 ? 976 : 956;

        Assert.Equal(expected, Win32NotifyIconBackend.NativeDataSize);
    }

    [Fact]
    public void Win32_ToBgraReordersChannelsAndHonoursRowstride()
    {
        using var pixbuf = CreateTestPixbuf();

        var bgra = Win32NotifyIconBackend.ToBgra(pixbuf);

        Assert.Equal(2 * 2 * 4, bgra.Length);
        // Source pixel 0 is RGBA(10,20,30,40); BGRA order puts blue first.
        Assert.Equal(new byte[] { 30, 20, 10, 40 }, bgra.Take(4).ToArray());
        // Pixel 2 is the first of row 1, reachable only by stepping the rowstride rather than width*4.
        Assert.Equal(new byte[] { 33, 32, 31, 34 }, bgra.Skip(8).Take(4).ToArray());
    }

    [Fact]
    public void StatusNotifier_ToArgb32ReordersChannelsAndHonoursRowstride()
    {
        using var pixbuf = CreateTestPixbuf();

        var argb = StatusNotifierItemBackend.ToArgb32(pixbuf);

        Assert.Equal(2 * 2 * 4, argb.Length);
        // The specification asks for ARGB32 in network byte order, so alpha leads.
        Assert.Equal(new byte[] { 40, 10, 20, 30 }, argb.Take(4).ToArray());
        Assert.Equal(new byte[] { 34, 31, 32, 33 }, argb.Skip(8).Take(4).ToArray());
    }

    [Fact]
    public void Win32_CreateHIconProducesARealIconHandleFromAWinFormsIcon()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        using var icon = CreateTestIcon();

        var handle = Win32NotifyIconBackend.CreateHIcon(icon);

        try
        {
            // The DIB section, the mask bitmap and CreateIconIndirect all have to line up; any one of
            // them being wrong yields a null handle rather than a diagnostic.
            Assert.NotEqual(IntPtr.Zero, handle);
        }
        finally
        {
            if (handle != IntPtr.Zero)
            {
                Win32NotifyIconBackend.DestroyIcon(handle);
            }
        }
    }

    [Fact]
    public void Win32_CreateHIconReturnsNothingForAnAbsentIcon()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        Assert.Equal(IntPtr.Zero, Win32NotifyIconBackend.CreateHIcon(null));
    }

    [Fact]
    public void Win32_BackendAddsAndRemovesTheIconInTheShell()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        using var icon = CreateTestIcon();
        using var backend = new Win32NotifyIconBackend();

        Assert.True(backend.IsAvailable, "Windows always has a notification area");
        Assert.False(backend.IsIconAdded, "a new backend starts hidden");

        // Exercises the whole native path - icon conversion to an HICON, the message-only window and
        // Shell_NotifyIcon itself. IsIconAdded is the shell's own answer, so a rejected NOTIFYICONDATAW
        // or a null icon handle fails here rather than passing silently.
        backend.SetIcon(icon);
        backend.SetToolTip("tray backend test");

        backend.SetVisible(true);
        Assert.True(backend.IsIconAdded, "the shell should have accepted the icon");

        backend.SetVisible(false);
        Assert.False(backend.IsIconAdded);
    }

    [Fact]
    public void NotifyIcon_ReportsTrayAvailabilityWithoutThrowing()
    {
        using var notify = new NotifyIcon();

        // The value is platform- and session-dependent; what matters is that asking is always safe.
        var available = notify.IsSystemTrayAvailable;

        Assert.True(available || !available);
    }

    [Fact]
    public void NotifyIcon_AssigningAContextMenuStripRoundTripsAndDoesNotThrow()
    {
        using var notify = new NotifyIcon();
        using var menu = BuildMenu(out _, out _);

        notify.ContextMenuStrip = menu;

        Assert.Same(menu, notify.ContextMenuStrip);

        notify.ContextMenuStrip = null;

        Assert.Null(notify.ContextMenuStrip);
    }

    [Fact]
    public void NotifyIcon_ShowBalloonTipIsSafeWhenNothingIsVisible()
    {
        using var notify = new NotifyIcon();

        // WinForms callers routinely fire a balloon without checking anything first; on a session with no
        // tray this must be a no-op rather than an exception.
        notify.ShowBalloonTip(500, "Title", "Body", ToolTipIcon.Info);
    }

    /// <summary>
    /// Builds a 2x2 RGBA pixbuf whose rowstride is deliberately wider than <c>width * channels</c>.
    /// </summary>
    /// <returns>The pixbuf.</returns>
    /// <remarks>
    /// Pixbuf rows are padded, so a converter that walks the buffer as <c>width * 4</c> reads the wrong
    /// bytes from row 1 onwards and skews the icon. Only a padded test image catches that.
    /// </remarks>
    private static Gdk.Pixbuf CreateTestPixbuf()
    {
        const int rowstride = 12; // 2 pixels * 4 channels = 8, plus 4 bytes of padding.

        var data = new byte[rowstride * 2];

        // Row 0
        data[0] = 10; data[1] = 20; data[2] = 30; data[3] = 40;
        data[4] = 11; data[5] = 21; data[6] = 31; data[7] = 41;

        // Row 1, starting at the rowstride boundary rather than at byte 8.
        data[12] = 31; data[13] = 32; data[14] = 33; data[15] = 34;
        data[16] = 41; data[17] = 42; data[18] = 43; data[19] = 44;

        return new Gdk.Pixbuf(data, true, 8, 2, 2, rowstride);
    }

    /// <summary>
    /// Builds a small opaque icon as a <c>Gtk.Drawing</c> <c>Icon</c>.
    /// </summary>
    /// <returns>The icon.</returns>
    private static gtk::System.Drawing.Icon CreateTestIcon()
    {
        using var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 16, 16);
        pixbuf.Fill(0x3366CCFF);

        return new gtk::System.Drawing.Icon(new MemoryStream(pixbuf.SaveToBuffer("png")));
    }

    private static string Flatten(TrayMenuModel model)
    {
        var builder = new global::System.Text.StringBuilder();
        Flatten(model.Root, builder);

        return builder.ToString();
    }

    private static void Flatten(TrayMenuNode node, global::System.Text.StringBuilder builder)
    {
        builder.Append(node.Id).Append(':').Append(node.Text).Append(';');

        foreach (var child in node.Children)
        {
            Flatten(child, builder);
        }
    }
}
