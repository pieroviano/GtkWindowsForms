#pragma warning disable CS0612

namespace System.Windows.Forms;

extern alias gtk;

using System.IO;
using System.Reflection;

/// <summary>
/// A stylesheet the library loads from a string must keep resolving its relative <c>url()</c> references.
/// </summary>
/// <remarks>
/// Gtk3's parser resolved a relative <c>url()</c> in string-loaded CSS against the process working
/// directory. Gtk4 has no base directory there at all and drops the reference - an image, or an entire
/// <c>@import</c>ed sheet - without raising a parsing error or writing anything to stderr. That is why the
/// sample's <c>theme/Test1.css</c> silently contributed nothing on Gtk4 while everything looked healthy.
/// </remarks>
[Collection("NonParallelCollection")]
public class CssUrlResolutionTests : IDisposable
{
    private readonly string _root;

    // Constructing a Gdk.Pixbuf and a Gtk.CssProvider needs the type system initialised.
    public CssUrlResolutionTests()
    {
        Application.Init();
        _root = Path.Combine(Path.GetTempPath(), "CssUrlResolutionTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_root, true);
        }
        catch (IOException)
        {
            // A leftover temp directory is not worth failing a test over.
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void RelativeReferences_BecomeAbsoluteFileUris()
    {
        var css = "@import url(\"theme/Test1.css\");\n#x{background-image:url('img/a.png')}";

        var resolved = CssUri.ResolveUrls(css, _root);

        Assert.Contains($"url(\"{Uri(Path.Combine(_root, "theme", "Test1.css"))}\")", resolved);
        Assert.Contains($"url(\"{Uri(Path.Combine(_root, "img", "a.png"))}\")", resolved);
    }

    [Fact]
    public void ReferencesThatAreAlreadyResolvable_AreLeftAlone()
    {
        // Anything carrying a scheme, and any fragment, is already meaningful to the parser; rewriting it
        // would break it. A single letter before the colon is a Windows drive, not a scheme.
        var css = "a{background-image:url(\"file:///c:/x.png\")}"
                + "b{background-image:url(data:image/png;base64,AAA)}"
                + "c{background-image:url(resource:///org/app/x.png)}"
                + "d{background-image:url(https://example.invalid/x.png)}"
                + "e{fill:url(#grad)}"
                + "f{background-image:url()}";

        Assert.Equal(css, CssUri.ResolveUrls(css, _root));
    }

    [Fact]
    public void TheFirstBaseDirectoryHoldingTheTarget_Wins()
    {
        // The working directory keeps the Gtk3 contract; the application directory is the fallback for the
        // case Gtk3 got wrong - an app launched from somewhere else.
        var elsewhere = Path.Combine(_root, "elsewhere");
        Directory.CreateDirectory(elsewhere);
        File.WriteAllText(Path.Combine(elsewhere, "found.css"), "#y{color:#fff}");

        var resolved = CssUri.ResolveUrls("@import url(\"found.css\");", _root, elsewhere);

        Assert.Contains($"url(\"{Uri(Path.Combine(elsewhere, "found.css"))}\")", resolved);
    }

    [Fact]
    public void AMissingTarget_StillBecomesAbsolute()
    {
        // Gtk marks an unresolvable reference invalid either way; emitting the URI keeps the failure
        // diagnosable instead of turning the declaration into a no-op.
        var resolved = CssUri.ResolveUrls("#x{background-image:url(\"gone.png\")}", _root);

        Assert.Contains($"url(\"{Uri(Path.Combine(_root, "gone.png"))}\")", resolved);
    }

    [Fact]
    public void AnImportedSheetAndItsImages_ReachTheGtk4Parser()
    {
        // The oracle is Gtk4's own parser: the dump of a provider tells whether the imported sheet exists
        // at all, and how it printed the image - "invalid image" is Gtk's marker for a url() it could not
        // resolve, so it distinguishes "the rule loaded" from "the rule loaded and its image resolved".
        var probe = Path.Combine(_root, "sub", "p.png");
        Directory.CreateDirectory(Path.GetDirectoryName(probe)!);
        using (var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 1, 1))
        {
            pixbuf.Savev(probe, "png", new string[] { }, new string[] { });
        }

        File.WriteAllText(Path.Combine(_root, "a.css"), "#probe{background-image:url(\"sub/p.png\")}");
        var rootCss = Path.Combine(_root, "root.css");
        File.WriteAllText(rootCss, "@import url(\"a.css\");");
        var text = File.ReadAllText(rootCss);

        var asIs = new Gtk.CssProvider();
        asIs.LoadFromData(text);

        Assert.DoesNotContain("#probe", asIs.ToString());

        var resolved = new Gtk.CssProvider();
        resolved.LoadFromData(CssUri.ResolveUrls(text, _root));
        var dump = resolved.ToString();

        Assert.Contains("#probe", dump);
        Assert.DoesNotContain("invalid image", dump);
    }

    [Fact]
    public void AControlImage_ResolvesToAnImageGtkCanLoad()
    {
        // Control.SetStyle writes the Image to a PNG and points background/background-image at it. The
        // BackgroundImage-only branch was already made absolute; the Image branches were not, so a Button
        // with an Image produced a declaration Gtk quietly discarded. Gtk's own dump is the oracle:
        // "none /* FIXME */" is a resolved url() it has yet to load, "none /* invalid image */" is one it
        // could not resolve at all.
        using var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 4, 4);
        var button = new Button
        {
            Name = "cssUrlResolutionProbe",
            Image = new gtk::System.Drawing.Bitmap(4, 4) { Pixbuf = pixbuf },
            ImageAlign = gtk::System.Drawing.ContentAlignment.TopCenter,
        };

        try
        {
            var setStyle = typeof(Control).GetMethod(
                "SetStyle",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(Gtk.Widget) },
                null)!;
            setStyle.Invoke(button, new object[] { button.Widget! });

            var provider = (Gtk.CssProvider)typeof(Control)
                .GetField("provider", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(button)!;
            var dump = provider.ToString();

            Assert.Contains("background-image: none /* FIXME */", dump);
            Assert.DoesNotContain("invalid image", dump);
        }
        finally
        {
            // SetStyle materialises the image under Resources/<name>/ in the working directory.
            try
            {
                Directory.Delete(Path.Combine("Resources", "cssUrlResolutionProbe"), true);
            }
            catch (IOException)
            {
                // Leftovers are harmless.
            }
            catch (UnauthorizedAccessException)
            {
                // Leftovers are harmless.
            }
        }
    }

    /// <summary>
    /// The absolute URI form the resolver is expected to emit for a path.
    /// </summary>
    private static string Uri(string path) => new System.Uri(Path.GetFullPath(path)).AbsoluteUri;
}

#pragma warning restore CS0612
