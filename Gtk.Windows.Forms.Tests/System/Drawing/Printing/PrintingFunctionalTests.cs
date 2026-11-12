extern alias gtk;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using gtk::System.Drawing.Printing;
using System.Globalization;
using Bitmap = gtk::System.Drawing.Bitmap;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;
using Point = global::System.Drawing.Point;
using Rectangle = global::System.Drawing.Rectangle;
using Size = global::System.Drawing.Size;

namespace GtkWinForms.Tests.System.Drawing.Printing;

/// <summary>Behavioural tests for the <c>System.Drawing.Printing</c> model types, which had never been
/// exercised. They found six defects: <see cref="PaperSize.Kind"/> and <see cref="PaperSource.Kind"/>
/// always returned <c>Custom</c>/<c>the raw value</c>, <see cref="PageSettings.Bounds"/> was a stored
/// value nothing set (always empty), <see cref="PrinterSettings.Copies"/> defaulted to 0 with no
/// validation anywhere in the page-range family, <see cref="PrinterSettings.IsDirectPrintingSupported"/>
/// returned false unconditionally, and the <see cref="PrintDocument.PageSetup"/> setter swapped the
/// top/right margins while feeding GTK points into hundredths-of-an-inch properties.</summary>
[Collection("NonParallelCollection")]
public class PrintingFunctionalTests
{
    // ---- Margins ----------------------------------------------------------

    [Fact]
    public void MarginsDefaultToOneInchOnEachSide()
    {
        var margins = new Margins();

        Assert.Equal(100, margins.Left);
        Assert.Equal(100, margins.Right);
        Assert.Equal(100, margins.Top);
        Assert.Equal(100, margins.Bottom);
    }

    [Fact]
    public void ANegativeMarginIsRejectedByTheConstructor()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Margins(-1, 0, 0, 0));

        Assert.Equal("left", ex.ParamName);
    }

    [Fact]
    public void ANegativeMarginIsRejectedByTheSetters()
    {
        var margins = new Margins();

        Assert.Throws<ArgumentOutOfRangeException>(() => margins.Top = -5);
        Assert.Equal(100, margins.Top);
    }

    [Fact]
    public void MarginsWithTheSameValuesAreEqual()
    {
        var a = new Margins(1, 2, 3, 4);
        var b = new Margins(1, 2, 3, 4);

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.False(a.Equals(new Margins(1, 2, 3, 5)));
    }

    [Fact]
    public void MarginsCloneIsIndependent()
    {
        var original = new Margins(10, 20, 30, 40);

        var clone = (Margins)original.Clone();
        clone.Left = 99;

        Assert.Equal(10, original.Left);
        Assert.Equal(20, clone.Right);
    }

    [Fact]
    public void MarginsToStringNamesAllFourSides()
    {
        Assert.Equal("[Margins Left=1 Right=2 Top=3 Bottom=4]", new Margins(1, 2, 3, 4).ToString());
    }

    // ---- MarginsConverter -------------------------------------------------

    [Fact]
    public void TheConverterParsesAnInvariantString()
    {
        var converter = new MarginsConverter();

        var margins = Assert.IsType<Margins>(
            converter.ConvertFrom(null!, CultureInfo.InvariantCulture, "1, 2, 3, 4"));

        Assert.Equal(new Margins(1, 2, 3, 4), margins);
    }

    [Fact]
    public void TheConverterHonoursTheCultureListSeparator()
    {
        var converter = new MarginsConverter();
        var german = CultureInfo.GetCultureInfo("de-DE");

        var margins = Assert.IsType<Margins>(converter.ConvertFrom(null!, german, "5;6;7;8"));

        Assert.Equal(new Margins(5, 6, 7, 8), margins);
    }

    [Fact]
    public void AnEmptyStringConvertsToNull()
    {
        Assert.Null(new MarginsConverter().ConvertFrom(null!, CultureInfo.InvariantCulture, "  "));
    }

    [Fact]
    public void TheWrongNumberOfTokensIsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            new MarginsConverter().ConvertFrom(null!, CultureInfo.InvariantCulture, "1, 2, 3"));
    }

    [Fact]
    public void MarginsRoundTripThroughTheStringForm()
    {
        var converter = new MarginsConverter();
        var original = new Margins(11, 22, 33, 44);

        var text = (string)converter.ConvertTo(null!, CultureInfo.InvariantCulture, original, typeof(string));

        Assert.Equal("11, 22, 33, 44", text);
        Assert.Equal(original, converter.ConvertFrom(null!, CultureInfo.InvariantCulture, text));
    }

    [Fact]
    public void TheInstanceDescriptorRecreatesTheMargins()
    {
        var converter = new MarginsConverter();
        var original = new Margins(1, 2, 3, 4);

        Assert.True(converter.CanConvertTo(null!, typeof(InstanceDescriptor)));
        var descriptor = (InstanceDescriptor)converter.ConvertTo(
            null!, CultureInfo.InvariantCulture, original, typeof(InstanceDescriptor));

        Assert.Equal(original, descriptor.Invoke());
    }

    [Fact]
    public void CreateInstanceBuildsMarginsFromThePropertyBag()
    {
        var converter = new MarginsConverter();
        var values = new Hashtable { ["Left"] = 1, ["Right"] = 2, ["Top"] = 3, ["Bottom"] = 4 };

        Assert.True(converter.GetCreateInstanceSupported(null!));
        Assert.Equal(new Margins(1, 2, 3, 4), converter.CreateInstance(null!, values));
    }

    [Fact]
    public void CreateInstanceRejectsAMissingProperty()
    {
        var converter = new MarginsConverter();
        var values = new Hashtable { ["Left"] = 1, ["Right"] = 2, ["Top"] = 3 };

        Assert.Throws<ArgumentException>(() => converter.CreateInstance(null!, values));
    }

    // ---- PrinterUnitConvert ----------------------------------------------

    [Fact]
    public void DisplayUnitsConvertToThousandthsOfAnInch()
    {
        Assert.Equal(1000, PrinterUnitConvert.Convert(100, PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch));
        Assert.Equal(100, PrinterUnitConvert.Convert(1000, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display));
    }

    [Fact]
    public void OneInchIsTwoPointFiveFourCentimetres()
    {
        Assert.Equal(2540, PrinterUnitConvert.Convert(100, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter));
        Assert.Equal(254.0, PrinterUnitConvert.Convert(2540.0, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.TenthsOfAMillimeter), 6);
    }

    [Fact]
    public void PointSizeAndRectangleConvertComponentWise()
    {
        Assert.Equal(new Point(1000, 2000),
            PrinterUnitConvert.Convert(new Point(100, 200), PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch));
        Assert.Equal(new Size(500, 250),
            PrinterUnitConvert.Convert(new Size(50, 25), PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch));
        Assert.Equal(new Rectangle(10, 20, 30, 40),
            PrinterUnitConvert.Convert(new Rectangle(100, 200, 300, 400), PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display));
    }

    [Fact]
    public void MarginsConvertAllFourSides()
    {
        var converted = PrinterUnitConvert.Convert(
            new Margins(100, 200, 300, 400), PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch);

        Assert.Equal(new Margins(1000, 2000, 3000, 4000), converted);
    }

    // ---- PaperSize --------------------------------------------------------

    [Fact]
    public void ADefaultPaperSizeIsCustomAndEditable()
    {
        var size = new PaperSize();

        Assert.Equal(PaperKind.Custom, size.Kind);
        Assert.Equal(string.Empty, size.PaperName);

        size.Width = 400;
        size.Height = 600;
        Assert.Equal(400, size.Width);
        Assert.Equal(600, size.Height);
    }

    [Fact]
    public void TheKindReflectsTheRawKind()
    {
        // The getter used to return Custom unconditionally, hiding the stored kind.
        var size = new PaperSize
        {
            RawKind = (int)PaperKind.A4
        };

        Assert.Equal(PaperKind.A4, size.Kind);
        Assert.Equal((int)PaperKind.A4, size.RawKind);
    }

    [Fact]
    public void AnUnknownRawKindReportsCustom()
    {
        var size = new PaperSize
        {
            RawKind = 9999
        };

        Assert.Equal(PaperKind.Custom, size.Kind);
        Assert.Equal(9999, size.RawKind);
    }

    [Fact]
    public void ANamedPaperSizeRejectsDimensionChanges()
    {
        var a4 = new PaperSize(PaperKind.A4, "A4", 827, 1169);

        Assert.Throws<ArgumentException>(() => a4.Width = 500);
        Assert.Throws<ArgumentException>(() => a4.Height = 500);
        Assert.Throws<ArgumentException>(() => a4.PaperName = "other");
        Assert.Equal(827, a4.Width);
    }

    // ---- PaperSource ------------------------------------------------------

    [Fact]
    public void ADefaultPaperSourceIsCustom()
    {
        var source = new PaperSource();

        Assert.Equal(PaperSourceKind.Custom, source.Kind);
        Assert.Equal(string.Empty, source.SourceName);
    }

    [Fact]
    public void ThePaperSourceKindReflectsTheRawKind()
    {
        var source = new PaperSource { RawKind = (int)PaperSourceKind.Manual, SourceName = "Tray 1" };

        Assert.Equal(PaperSourceKind.Manual, source.Kind);
        Assert.Equal("Tray 1", source.SourceName);
    }

    [Fact]
    public void AnUnknownPaperSourceRawKindReportsCustom()
    {
        // The getter used to leak the raw value as an undefined enum member.
        var source = new PaperSource { RawKind = 9999 };

        Assert.Equal(PaperSourceKind.Custom, source.Kind);
        Assert.Equal(9999, source.RawKind);
    }

    // ---- PrinterResolution ------------------------------------------------

    [Fact]
    public void APrinterResolutionRoundTripsItsValues()
    {
        var resolution = new PrinterResolution { Kind = PrinterResolutionKind.High, X = 1200, Y = 600 };

        Assert.Equal(PrinterResolutionKind.High, resolution.Kind);
        Assert.Equal(1200, resolution.X);
        Assert.Equal(600, resolution.Y);
        Assert.Contains("High", resolution.ToString());
    }

    [Fact]
    public void ACustomResolutionPrintsItsDots()
    {
        var resolution = new PrinterResolution { X = 300, Y = 150 };

        Assert.Equal(PrinterResolutionKind.Custom, resolution.Kind);
        Assert.Contains("X=300", resolution.ToString());
        Assert.Contains("Y=150", resolution.ToString());
    }

    // ---- PageSettings -----------------------------------------------------

    [Fact]
    public void TheDefaultPageIsLetterSized()
    {
        var page = new PageSettings();

        Assert.Equal(PaperKind.Letter, page.PaperSize.Kind);
        Assert.Equal(850, page.PaperSize.Width);
        Assert.Equal(1100, page.PaperSize.Height);
        Assert.Equal(new Margins(100, 100, 100, 100), page.Margins);
    }

    [Fact]
    public void TheBoundsComeFromThePaperSize()
    {
        // Bounds was a stored value nothing ever set, so it was always empty.
        var page = new PageSettings();

        Assert.Equal(new Rectangle(0, 0, 850, 1100), page.Bounds);
    }

    [Fact]
    public void LandscapeSwapsTheBounds()
    {
        var page = new PageSettings { Landscape = true };

        Assert.Equal(new Rectangle(0, 0, 1100, 850), page.Bounds);
    }

    [Fact]
    public void TheBoundsFollowACustomPaperSize()
    {
        var page = new PageSettings { PaperSize = new PaperSize("card", 400, 600) };

        Assert.Equal(new Rectangle(0, 0, 400, 600), page.Bounds);

        page.Landscape = true;
        Assert.Equal(new Rectangle(0, 0, 600, 400), page.Bounds);
    }

    [Fact]
    public void ThePrintableAreaDefaultsToTheWholePage()
    {
        var page = new PageSettings();

        Assert.Equal(0f, page.PrintableArea.X);
        Assert.Equal(0f, page.PrintableArea.Y);
        Assert.Equal(850f, page.PrintableArea.Width);
        Assert.Equal(1100f, page.PrintableArea.Height);
    }

    [Fact]
    public void ThePaperSourceAndResolutionAreNeverNull()
    {
        var page = new PageSettings();

        Assert.Equal(PaperSourceKind.FormSource, page.PaperSource.Kind);
        Assert.Equal(200, page.PrinterResolution.X);
        Assert.Equal(200, page.PrinterResolution.Y);
    }

    [Fact]
    public void CloningAPageDeepCopiesTheMargins()
    {
        var page = new PageSettings { Margins = new Margins(10, 20, 30, 40) };

        var clone = (PageSettings)page.Clone();
        clone.Margins.Left = 99;

        Assert.Equal(10, page.Margins.Left);
        Assert.Equal(new Margins(99, 20, 30, 40), clone.Margins);
    }

    [Fact]
    public void ThePrinterSettingsAreWiredToTheirPage()
    {
        var settings = new PrinterSettings();

        Assert.Same(settings, settings.DefaultPageSettings.PrinterSettings);
    }

    [Fact]
    public void AssigningNullPrinterSettingsFallsBackToADefault()
    {
        var page = new PageSettings { PrinterSettings = null! };

        Assert.NotNull(page.PrinterSettings);
    }

    // ---- PrinterSettings --------------------------------------------------

    [Fact]
    public void CopiesDefaultToOne()
    {
        // The auto-property defaulted to printing zero copies.
        Assert.Equal(1, new PrinterSettings().Copies);
    }

    [Fact]
    public void CopiesRoundTripAndRejectNegatives()
    {
        var settings = new PrinterSettings
        {
            Copies = 3
        };

        Assert.Equal(3, settings.Copies);
        Assert.Throws<ArgumentException>(() => settings.Copies = -1);
        Assert.Equal(3, settings.Copies);
    }

    [Fact]
    public void ThePageRangeRejectsValuesBelowTheMinimum()
    {
        var settings = new PrinterSettings();

        Assert.Throws<ArgumentException>(() => settings.FromPage = -1);
        Assert.Throws<ArgumentException>(() => settings.ToPage = -1);
        Assert.Throws<ArgumentException>(() => settings.MinimumPage = -1);
        Assert.Throws<ArgumentException>(() => settings.MaximumPage = -1);

        settings.MinimumPage = 5;
        Assert.Throws<ArgumentException>(() => settings.FromPage = 4);

        settings.FromPage = 5;
        settings.ToPage = 9;
        Assert.Equal(5, settings.FromPage);
        Assert.Equal(9, settings.ToPage);
    }

    [Fact]
    public void ThePrintRangeOnlyAcceptsDefinedValues()
    {
        var settings = new PrinterSettings
        {
            PrintRange = PrintRange.SomePages
        };

        Assert.Equal(PrintRange.SomePages, settings.PrintRange);
        Assert.Throws<InvalidEnumArgumentException>(() => settings.PrintRange = (PrintRange)999);
    }

    [Fact]
    public void CloningPrinterSettingsIsIndependent()
    {
        var settings = new PrinterSettings { Copies = 5 };

        var clone = (PrinterSettings)settings.Clone();
        clone.Copies = 7;

        Assert.Equal(5, settings.Copies);
        Assert.Equal(7, clone.Copies);
    }

    [Fact]
    public void TheInstalledPrintersCollectionIsNeverNull()
    {
        Assert.NotNull(PrinterSettings.InstalledPrinters);
    }

    [Fact]
    public void DirectPrintingIsSupportedForPngButNotForBmp()
    {
        // The old implementation checked the format and then returned false on both paths. This also
        // guards the Image fix underneath: loading a BMP used to re-encode it as lossy JPEG and
        // report RawFormat Jpeg, which would make the BMP case pass here.
        var settings = new PrinterSettings();
        using var source = new Bitmap(4, 4);

        using var pngStream = new MemoryStream();
        source.Save(pngStream, ImageFormat.Png);
        using var png = new Bitmap(new MemoryStream(pngStream.ToArray()));

        using var bmpStream = new MemoryStream();
        source.Save(bmpStream, ImageFormat.Bmp);
        using var bmp = new Bitmap(new MemoryStream(bmpStream.ToArray()));

        Assert.True(settings.IsDirectPrintingSupported(png));
        Assert.False(settings.IsDirectPrintingSupported(bmp));
        Assert.False(settings.IsDirectPrintingSupported(source));
    }

    // ---- PrintDocument ----------------------------------------------------

    [Fact]
    public void ANewDocumentHasTheWinFormsDefaults()
    {
        using var document = new PrintDocument();

        Assert.Equal("document", document.DocumentName);
        Assert.False(document.OriginAtMargins);
        Assert.NotNull(document.PrinterSettings);
        Assert.NotNull(document.DefaultPageSettings);
    }

    [Fact]
    public void ANullDocumentNameBecomesEmpty()
    {
        using var document = new PrintDocument { DocumentName = null! };

        Assert.Equal(string.Empty, document.DocumentName);
    }

    [Fact]
    public void EachRaiserRaisesItsEvent()
    {
        using var document = new PrintDocument();
        var log = new List<string>();
        document.BeginPrint += (s, _) => log.Add($"begin:{ReferenceEquals(s, document)}");
        document.QueryPageSettings += (s, _) => log.Add($"query:{ReferenceEquals(s, document)}");
        document.PrintPage += (s, _) => log.Add($"page:{ReferenceEquals(s, document)}");
        document.EndPrint += (s, _) => log.Add($"end:{ReferenceEquals(s, document)}");

        document.OnBeginPrint(new PrintEventArgs());
        document.OnQueryPageSettings(new QueryPageSettingsEventArgs(document.DefaultPageSettings));
        document.OnPrintPage(new PrintPageEventArgs(null!, default, default, document.DefaultPageSettings));
        document.OnEndPrint(new PrintEventArgs());

        Assert.Equal(new[] { "begin:True", "query:True", "page:True", "end:True" }, log);
    }

    [Fact]
    public void ARemovedHandlerNoLongerFires()
    {
        using var document = new PrintDocument();
        var fired = 0;
        void Handler(object s, PrintEventArgs e) => fired++;

        document.BeginPrint += Handler;
        document.OnBeginPrint(new PrintEventArgs());
        document.BeginPrint -= Handler;
        document.OnBeginPrint(new PrintEventArgs());

        Assert.Equal(1, fired);
    }

    [Fact]
    public void RaisingWithNoSubscribersDoesNotThrow()
    {
        using var document = new PrintDocument();

        document.OnEndPrint(new PrintEventArgs());
    }

    [Fact]
    public void AssigningPrinterSettingsAdoptsTheirPageDefaults()
    {
        using var document = new PrintDocument();
        var settings = new PrinterSettings();

        document.PrinterSettings = settings;

        Assert.Same(settings.DefaultPageSettings, document.DefaultPageSettings);
    }

    [Fact]
    public void UserPageSettingsSurviveAPrinterSettingsChange()
    {
        using var document = new PrintDocument();
        var userPage = new PageSettings { Landscape = true };

        document.DefaultPageSettings = userPage;
        document.PrinterSettings = new PrinterSettings();

        Assert.Same(userPage, document.DefaultPageSettings);
    }

    [Fact]
    public void AGtkPageSetupIsConvertedToHundredthsOfAnInch()
    {
        // The margins used to be passed as (left, top, right, bottom) into a (left, right, top,
        // bottom) constructor - swapping top and right - and GTK points went in unconverted.
        using var boot = new global::System.Windows.Forms.Control();
        using var document = new PrintDocument();
        var setup = new global::Gtk.PageSetup
        {
            Orientation = global::Gtk.PageOrientation.Landscape,
            PaperSize = new global::Gtk.PaperSize("na_letter")
        };
        setup.SetLeftMargin(72, global::Gtk.Unit.Points);    // 1 inch
        setup.SetRightMargin(36, global::Gtk.Unit.Points);   // 0.5 inch
        setup.SetTopMargin(18, global::Gtk.Unit.Points);     // 0.25 inch
        setup.SetBottomMargin(144, global::Gtk.Unit.Points); // 2 inches

        document.PageSetup = setup;

        var page = document.DefaultPageSettings;
        Assert.True(page.Landscape);
        Assert.Equal(new Margins(100, 50, 25, 200), page.Margins);
        Assert.Equal(850, page.PaperSize.Width);
        Assert.Equal(1100, page.PaperSize.Height);
    }

    // ---- event args -------------------------------------------------------

    [Fact]
    public void PrintEventArgsCarryTheActionAndCancelFlag()
    {
        var args = new PrintEventArgs(PrintAction.PrintToPreview);

        Assert.Equal(PrintAction.PrintToPreview, args.PrintAction);
        Assert.False(args.Cancel);

        args.Cancel = true;
        Assert.True(args.Cancel);
    }

    [Fact]
    public void PrintPageEventArgsExposeTheirGeometry()
    {
        var page = new PageSettings();
        var margin = new Rectangle(100, 100, 650, 900);
        var bounds = new Rectangle(0, 0, 850, 1100);

        var args = new PrintPageEventArgs(null!, margin, bounds, page);

        Assert.Equal(margin, args.MarginBounds);
        Assert.Equal(bounds, args.PageBounds);
        Assert.Same(page, args.PageSettings);
        Assert.False(args.Cancel);
        Assert.False(args.HasMorePages);
    }

    [Fact]
    public void QueryPageSettingsEventArgsTrackAccessAndRejectNull()
    {
        var page = new PageSettings();
        var args = new QueryPageSettingsEventArgs(page);

        Assert.False(args.PageSettingsChanged);
        Assert.Same(page, args.PageSettings);
        Assert.True(args.PageSettingsChanged);

        args.PageSettings = null!;
        Assert.NotNull(args.PageSettings);
    }
}
