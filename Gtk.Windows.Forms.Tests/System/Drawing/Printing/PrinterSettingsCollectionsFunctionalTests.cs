extern alias gtk;

using System.Collections;
using gtk::System.Drawing.Printing;

namespace GtkWinForms.Tests.System.Drawing.Printing;

/// <summary>
/// Tests for the nested capability collections on <see cref="PrinterSettings"/> (§6 of API-COMPLETENESS):
/// PaperSizeCollection, PaperSourceCollection, PrinterResolutionCollection and StringCollection, plus the
/// PaperSizes/PaperSources/PrinterResolutions properties. These were absent, so code enumerating a
/// printer's capabilities did not compile. With no print system the exposed collections are empty; the
/// collection types themselves are exercised directly for their contract.
/// </summary>
public class PrinterSettingsCollectionsFunctionalTests
{
    [Fact]
    public void CapabilityProperties_AreNeverNullAndEmpty()
    {
        var settings = new PrinterSettings();

        Assert.NotNull(settings.PaperSizes);
        Assert.NotNull(settings.PaperSources);
        Assert.NotNull(settings.PrinterResolutions);
        Assert.Empty(settings.PaperSizes);
        Assert.Empty(settings.PaperSources);
        Assert.Empty(settings.PrinterResolutions);
    }

    [Fact]
    public void InstalledPrinters_IsTheNestedStringCollection()
    {
        var printers = PrinterSettings.InstalledPrinters;

        Assert.NotNull(printers);
        Assert.IsType<PrinterSettings.StringCollection>(printers);
    }

    [Fact]
    public void PaperSizeCollection_ExposesCountIndexerAndEnumerator()
    {
        var a = new PaperSize("A", 100, 200);
        var b = new PaperSize("B", 300, 400);
        var collection = new PrinterSettings.PaperSizeCollection(new[] { a, b });

        Assert.Equal(2, collection.Count);
        Assert.Same(a, collection[0]);
        Assert.Same(b, collection[1]);

        var seen = 0;
        foreach (PaperSize _ in collection)
        {
            seen++;
        }

        Assert.Equal(2, seen);
    }

    [Fact]
    public void PaperSizeCollection_CopyToAndAddWork()
    {
        var collection = new PrinterSettings.PaperSizeCollection(new[] { new PaperSize("A", 10, 20) });

        var index = collection.Add(new PaperSize("B", 30, 40));
        Assert.Equal(1, index);
        Assert.Equal(2, collection.Count);

        var target = new PaperSize[2];
        collection.CopyTo(target, 0);
        Assert.Equal("A", target[0].PaperName);
        Assert.Equal("B", target[1].PaperName);
    }

    [Fact]
    public void Collections_ImplementICollection()
    {
        ICollection sizes = new PrinterSettings.PaperSizeCollection(new[] { new PaperSize("A", 10, 20) });
        ICollection strings = new PrinterSettings.StringCollection(new[] { "p1", "p2" });

        Assert.Single(sizes);
        Assert.Equal(2, strings.Count);

        var arr = new object[2];
        strings.CopyTo(arr, 0);
        Assert.Equal("p1", arr[0]);
        Assert.Equal("p2", arr[1]);
    }
}
