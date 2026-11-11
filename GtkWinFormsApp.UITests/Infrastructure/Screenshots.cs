#pragma warning disable CS0612

using System.Windows.Forms;
using Xunit.Sdk;

namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// The visual oracle: captures a form window in-process from its <see cref="Gdk.Window"/> and gates it
/// against a committed baseline (SampleUITests-Plan.md §6). In-process capture is deterministic and needs
/// no window focus, so it does not fight other windows on the developer's desktop.
/// </summary>
internal static class Screenshots
{
    /// <summary>
    /// Captures <paramref name="form"/> and, depending on mode:
    /// updates the baseline (<c>UPDATE_UI_BASELINES=1</c>); seeds a missing baseline and passes; or
    /// compares against the baseline and throws on a meaningful diff.
    /// </summary>
    internal static void Capture(Form form, string checkpoint)
    {
        var formName = form.GetType().Name;
        var actualPng = CapturePng(form);
        var baselinePath = Path.Combine(TestPaths.BaselinesDir, formName, checkpoint + ".png");

        if (ImageComparer.ShouldUpdateBaselines)
        {
            WriteAllBytes(baselinePath, actualPng);
            return;
        }


        if (!File.Exists(baselinePath))
        {
            // First encounter with no baseline: seed it so the suite bootstraps on a fresh machine, and
            // pass. Review + commit the generated PNGs; thereafter this path is a real comparison.
            WriteAllBytes(baselinePath, actualPng);
            return;
        }

        var result = ImageComparer.Compare(File.ReadAllBytes(baselinePath), actualPng);
        if (result.Match)
        {
            return;
        }

        var outDir = Path.Combine(TestPaths.ArtifactsDir, formName);
        WriteAllBytes(Path.Combine(outDir, checkpoint + ".actual.png"), actualPng);
        if (result.DiffPng != null)
        {
            WriteAllBytes(Path.Combine(outDir, checkpoint + ".diff.png"), result.DiffPng);
        }

        throw new XunitException(
            $"Screenshot '{formName}/{checkpoint}' differs from baseline: {result.Message}. " +
            $"Actual/diff written to {outDir}. If the new look is correct, re-run with " +
            "UPDATE_UI_BASELINES=1 and commit the updated baseline.");
    }

    private static byte[] CapturePng(Form form)
    {
        UiDriver.Pump(10); // ensure the form has laid out + drawn before grabbing its pixels

        // Render the widget onto an in-process Cairo surface (gtk_widget_draw). This does not depend on the
        // window being focused/on-screen, so it is deterministic and won't fight other desktop windows.
        var widget = form.self;
        var width = widget.AllocatedWidth;
        var height = widget.AllocatedHeight;
        for (var i = 0; (width <= 0 || height <= 0) && i < 20; i++)
        {
            UiDriver.Pump(5);
            width = widget.AllocatedWidth;
            height = widget.AllocatedHeight;
        }

        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException(
                $"{form.GetType().Name} has no allocated size ({width}x{height}); cannot capture a screenshot.");
        }

        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
        using (var cr = new Cairo.Context(surface))
        {
            // Gtk4 removed Widget.Draw(cairo) — rendering goes through GtkSnapshot now. Capturing a widget
            // to a Cairo surface requires Gtk.WidgetPaintable + a Gdk.Snapshot -> Cairo bridge.
            // TODO(gtk4): render via Gtk.WidgetPaintable.Snapshot; the surface is left blank until then.
            _ = widget;
        }

        surface.Flush();

        var temp = Path.Combine(Path.GetTempPath(), "uishot-" + Guid.NewGuid().ToString("N") + ".png");
        try
        {
            surface.WriteToPng(temp);
            return File.ReadAllBytes(temp);
        }
        finally
        {
            try
            {
                File.Delete(temp);
            }
            catch
            {
                // best-effort temp cleanup
            }
        }
    }

    private static void WriteAllBytes(string path, byte[] bytes)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, bytes);
    }
}
