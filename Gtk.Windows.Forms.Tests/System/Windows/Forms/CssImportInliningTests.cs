#pragma warning disable CS0612

namespace System.Windows.Forms;

using System.IO;
using System.Text;

/// <summary>
/// An <c>@import</c>ed stylesheet must reach Gtk4 whatever encoding it is written in.
/// </summary>
/// <remarks>
/// <para>
/// Leaving the import for Gtk to follow looks equivalent to inlining it and is not: Gtk then opens the file
/// itself, and its CSS parser requires valid UTF-8. One byte in a national codepage - in practice a comment
/// - makes it abandon the rules around that byte, and it says nothing at all: no parsing error, nothing on
/// stderr, the rules simply do not exist. Gtk3's parser did not validate, so such stylesheets worked.
/// </para>
/// <para>
/// This was found in the sample's own theme: <c>theme/Test1.css</c> is GBK-encoded, and the first invalid
/// byte sits in the comment immediately above <c>#Test1 #button16</c>. Gtk4 dropped that rule and the two
/// after it, so two toolbar buttons lost the icons the stylesheet gives them - while every other rule in
/// the file applied, which is what made it look like an image problem.
/// </para>
/// </remarks>
[Collection("NonParallelCollection")]
public class CssImportInliningTests : IDisposable
{
    private readonly string _root;

    public CssImportInliningTests()
    {
        // Widget-free, but the provider tests need GTK's type system.
        Application.Init();

        _root = Path.Combine(Path.GetTempPath(), "csstest-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "theme"));
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
            // A leaked temp directory is not worth failing a test over.
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void ARuleAfterNonUtf8Bytes_SurvivesInlining()
    {
        // The exact shape of the defect: a comment in a national codepage, then the rule that was lost.
        WriteGbk(Path.Combine(_root, "theme", "sheet.css"),
            "#a { color: rgb(1,2,3); }", "#b { color: rgb(4,5,6); }");

        var inlined = CssUri.InlineImports("@import url(\"theme/sheet.css\");", _root);

        Assert.Contains("#b", inlined, StringComparison.Ordinal);
        Assert.DoesNotContain("@import", inlined, StringComparison.Ordinal);
    }

    [Fact]
    public void GtkKeepsTheRule_OnlyWhenTheImportIsInlined()
    {
        // The oracle that matters: what Gtk ends up holding. Handing Gtk the import directly loses the
        // rule; handing it the inlined text does not. If this ever stops failing on the left-hand side,
        // Gtk has become tolerant and the workaround could go.
        var sheet = Path.Combine(_root, "theme", "sheet.css");
        WriteGbk(sheet, "#kept { color: rgb(1,2,3); }", "#lost { color: rgb(4,5,6); }");

        var importStatement = $"@import url(\"{new Uri(sheet).AbsoluteUri}\");";

        Assert.DoesNotContain("#lost", DumpOf(importStatement), StringComparison.Ordinal);
        Assert.Contains("#lost", DumpOf(CssUri.InlineImports("@import url(\"theme/sheet.css\");", _root)),
            StringComparison.Ordinal);
    }

    [Fact]
    public void AnImportedSheetsOwnReferences_ResolveWhereThatSheetLives()
    {
        // Gtk would have resolved them against the imported file's directory, because that is where it
        // opened the file. Inlining has to reproduce that, not resolve them against the outer sheet.
        Directory.CreateDirectory(Path.Combine(_root, "theme", "img"));
        File.WriteAllBytes(Path.Combine(_root, "theme", "img", "icon.png"), new byte[] { 1 });
        File.WriteAllText(
            Path.Combine(_root, "theme", "sheet.css"),
            "#a { background-image: url(\"img/icon.png\"); }");

        var inlined = CssUri.InlineImports("@import url(\"theme/sheet.css\");", _root);

        Assert.Contains("theme/img/icon.png", inlined.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("file:///", inlined, StringComparison.Ordinal);
    }

    [Fact]
    public void AQuotedImport_IsInlinedToo()
    {
        // Both spellings are legal CSS and themes in the wild use each.
        File.WriteAllText(Path.Combine(_root, "theme", "sheet.css"), "#a { color: rgb(1,2,3); }");

        Assert.Contains("#a", CssUri.InlineImports("@import \"theme/sheet.css\";", _root),
            StringComparison.Ordinal);
    }

    [Fact]
    public void AnImportThatCannotBeFound_IsLeftForTheUrlRewriterToHandle()
    {
        const string css = "@import url(\"theme/missing.css\");";

        Assert.Equal(css, CssUri.InlineImports(css, _root));
    }

    [Fact]
    public void AnImportCarryingAScheme_IsLeftAlone()
    {
        const string css = "@import url(\"resource:///org/example/style.css\");";

        Assert.Equal(css, CssUri.InlineImports(css, _root));
    }

    [Fact]
    public void SheetsThatImportEachOther_DoNotRecurseForever()
    {
        // Depth is capped rather than cycle-detected; what matters is that it returns.
        File.WriteAllText(Path.Combine(_root, "theme", "a.css"), "@import url(\"b.css\"); #a { color: red; }");
        File.WriteAllText(Path.Combine(_root, "theme", "b.css"), "@import url(\"a.css\"); #b { color: blue; }");

        var inlined = CssUri.InlineImports("@import url(\"theme/a.css\");", _root);

        Assert.Contains("#a", inlined, StringComparison.Ordinal);
        Assert.Contains("#b", inlined, StringComparison.Ordinal);
    }

    private static string DumpOf(string css)
    {
        var provider = new Gtk.CssProvider();
        provider.LoadFromData(css);
        return provider.ToString();
    }

    // The exact byte run from the sample's theme/Test1.css - the GBK encoding of the comment that sits
    // immediately above #Test1 #button16, which is the rule Gtk4 discards. A hand-rolled substitute is not
    // good enough: Gtk tolerates some invalid sequences and not others, and an approximation passes while
    // proving nothing.
    private static readonly byte[] GbkComment =
    {
        0xB0, 0xEF, 0xD6, 0xFA, 0xA3, 0xAC, 0xB4, 0xF2, 0xD3, 0xA1,
    };

    private static void WriteGbk(string path, string before, string after)
    {
        var bytes = new List<byte>();
        bytes.AddRange(Encoding.ASCII.GetBytes(before + "\r\n/* "));
        bytes.AddRange(GbkComment);
        bytes.AddRange(Encoding.ASCII.GetBytes("*/\r\n" + after + "\r\n"));

        File.WriteAllBytes(path, bytes.ToArray());

        // Guard the premise: if this ever became valid UTF-8 the test would pass for the wrong reason.
        Assert.Throws<DecoderFallbackException>(() =>
            new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(File.ReadAllBytes(path)));
    }
}
