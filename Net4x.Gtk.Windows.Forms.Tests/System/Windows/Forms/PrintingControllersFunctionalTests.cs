extern alias gtk;

using System.Drawing.Printing;
using Bitmap = gtk::System.Drawing.Bitmap;
using Size = global::System.Drawing.Size;

namespace System.Windows.Forms;

/// <summary>
/// Verifies the print-controller family restored to <c>System.Drawing.Printing</c>: the abstract
/// <see cref="PrintController" /> base, <see cref="StandardPrintController" />,
/// <see cref="PreviewPrintController" /> (which actually captures pages), <see cref="PreviewPageInfo" />,
/// <see cref="InvalidPrinterException" />, the <see cref="PrintControllerWithStatusDialog" /> wrapper, and
/// the <see cref="PrintDocument.PrintController" /> hook.
/// </summary>
[Collection("NonParallelCollection")]
public class PrintingControllersFunctionalTests
{
    static PrintingControllersFunctionalTests()
    {
        // Bitmap/Graphics/PrinterSettings need an initialized GTK toolkit; the Form static constructor
        // calls Application.Init, so touch one before any drawing/printer object is created.
        try
        {
            using var _ = new Form();
        }
        catch
        {
            // ignored - initialization is best-effort
        }
    }

    [Fact]
    public void Hierarchy_And_IsPreview()
    {
        Assert.True(typeof(PrintController).IsAssignableFrom(typeof(StandardPrintController)));
        Assert.True(typeof(PrintController).IsAssignableFrom(typeof(PreviewPrintController)));
        Assert.True(typeof(PrintController).IsAssignableFrom(typeof(PrintControllerWithStatusDialog)));

        Assert.False(new StandardPrintController().IsPreview);
        Assert.True(new PreviewPrintController().IsPreview);
    }

    [Fact]
    public void PreviewPrintController_CapturesPages()
    {
        var controller = new PreviewPrintController { UseAntiAlias = true };
        Assert.True(controller.UseAntiAlias);
        Assert.Empty(controller.GetPreviewPageInfo());

        // Drive one page through the controller (null args exercise the default page size path).
        controller.OnStartPrint(null!, null!);
        var g = controller.OnStartPage(null!, null!);
        Assert.NotNull(g);
        controller.OnEndPage(null!, null!);

        var pages = controller.GetPreviewPageInfo();
        Assert.Single(pages);
        Assert.NotNull(pages[0].Image);
        Assert.Equal(new Size(850, 1100), pages[0].PhysicalSize);

        // a fresh print run clears the captured pages
        controller.OnStartPrint(null!, null!);
        Assert.Empty(controller.GetPreviewPageInfo());
    }

    [Fact]
    public void PreviewPageInfo_RoundTrip()
    {
        using var bmp = new Bitmap(20, 30);
        var info = new PreviewPageInfo(bmp, new Size(20, 30));
        Assert.Same(bmp, info.Image);
        Assert.Equal(new Size(20, 30), info.PhysicalSize);
    }

    [Fact]
    public void InvalidPrinterException_IsSystemException()
    {
        var settings = new PrinterSettings();
        var ex = new InvalidPrinterException(settings);
        Assert.IsAssignableFrom<SystemException>(ex);
        Assert.Contains("are not valid", ex.Message);
    }

    [Fact]
    public void PrintControllerWithStatusDialog_DelegatesIsPreview()
    {
        var withPreview = new PrintControllerWithStatusDialog(new PreviewPrintController());
        Assert.True(withPreview.IsPreview);

        var withStandard = new PrintControllerWithStatusDialog(new StandardPrintController(), "My Print");
        Assert.False(withStandard.IsPreview);
    }

    [Fact]
    public void PrintDocument_PrintController_DefaultsAndSets()
    {
        var doc = new PrintDocument();
        Assert.IsType<StandardPrintController>(doc.PrintController);

        var preview = new PreviewPrintController();
        doc.PrintController = preview;
        Assert.Same(preview, doc.PrintController);
    }
}
