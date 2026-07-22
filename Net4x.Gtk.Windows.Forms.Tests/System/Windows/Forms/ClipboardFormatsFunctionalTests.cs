using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary><c>GetDataPresent</c> and <c>GetFormats</c> all threw, so the
/// guard every well-written paste handler calls before <c>GetData</c> failed while the unguarded call
/// worked. The point of these tests is that the defensive path now answers instead of throwing.</summary>
[Collection("NonParallelCollection")]
public class ClipboardFormatsFunctionalTests
{
    // NOTE: this shim nests DataObject inside Clipboard; WinForms declares it at namespace level.
    private static Clipboard.DataObject NewDataObject() => new();

    [Fact]
    public void GetDataPresent_AnswersInsteadOfThrowing()
    {
        var data = NewDataObject();

        // The value depends on what the session clipboard happens to hold; the contract under test is that
        // asking is possible at all.
        var present = data.GetDataPresent(DataFormats.Text);

        Assert.True(present || !present, "GetDataPresent should return a value rather than throw");
    }

    [Fact]
    public void GetDataPresent_WithAutoConvertAnswersInsteadOfThrowing()
    {
        var data = NewDataObject();

        var present = data.GetDataPresent(DataFormats.Text, true);

        Assert.True(present || !present, "the autoConvert overload should return a value rather than throw");
    }

    [Fact]
    public void GetDataPresent_ForATypeAnswersInsteadOfThrowing()
    {
        var data = NewDataObject();

        var present = data.GetDataPresent(typeof(string));

        Assert.True(present || !present, "the Type overload should return a value rather than throw");
    }

    [Fact]
    public void GetDataPresent_IsFalseForAnUnknownFormat()
    {
        var data = NewDataObject();

        Assert.False(data.GetDataPresent("application/x-no-such-format-exists"),
            "a format nothing advertises should not be reported present");
    }

    [Fact]
    public void GetDataPresent_IsFalseForAnEmptyFormatName()
    {
        var data = NewDataObject();

        Assert.False(data.GetDataPresent(string.Empty), "an empty format name should not be reported present");
        Assert.False(data.GetDataPresent((string)null!), "a null format name should not be reported present");
    }

    [Fact]
    public void GetDataPresent_ForANullTypeIsFalse()
    {
        var data = NewDataObject();

        Assert.False(data.GetDataPresent((Type)null!), "a null type should not be reported present");
    }

    [Fact]
    public void GetFormats_ReturnsAnArrayInsteadOfThrowing()
    {
        var data = NewDataObject();

        var formats = data.GetFormats();

        Assert.NotNull(formats);
    }

    [Fact]
    public void GetFormats_WithAutoConvertReturnsAnArrayInsteadOfThrowing()
    {
        var data = NewDataObject();

        var formats = data.GetFormats(false);

        Assert.NotNull(formats);
    }

    [Fact]
    public void GetFormats_AndGetDataPresentAgreeWithEachOther()
    {
        var data = NewDataObject();

        foreach (var format in data.GetFormats())
        {
            Assert.True(data.GetDataPresent(format),
                $"'{format}' was reported by GetFormats so GetDataPresent should confirm it");
        }
    }

    [Fact]
    public void GetFormats_NeverReportsDuplicates()
    {
        var data = NewDataObject();

        var formats = data.GetFormats();

        Assert.Equal(formats.Length, formats.Distinct().Count());
    }

    [Fact]
    public void TheGuardedPasteIdiomWorksEndToEnd()
    {
        // This is the pattern §4.2 says was broken: the guard threw while the naive call worked.
        var data = NewDataObject();

        object? pasted = null;
        if (data.GetDataPresent(DataFormats.Text))
        {
            pasted = data.GetData(DataFormats.Text);
        }

        Assert.True(pasted is null or string, "a guarded paste should yield text or nothing, never an exception");
    }
}
