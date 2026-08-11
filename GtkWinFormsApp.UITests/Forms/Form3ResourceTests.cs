using ComponentResourceManager = GtkSystem.ComponentModel.ComponentResourceManager;
using Icon = System.Drawing.Icon;
using Image = System.Drawing.Image;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>
/// Regression tests for reading the designer resources that <see cref="Form3" />'s
/// <c>InitializeComponent</c> pulls out of <c>Form3.resx</c>. These go straight through
/// <see cref="ComponentResourceManager" /> against the real embedded resources, so they do not need a
/// GTK display / GApplication registration (unlike the <see cref="Form3Tests" /> UI-driver tests).
/// </summary>
public class Form3ResourceTests
{
    /// <summary>
    /// <c>Form3.resx</c> stores <c>notifyIcon1.Icon</c> and <c>$this.Icon</c> as
    /// <c>type="System.Drawing.Icon, System.Drawing"</c> byte-array entries. The shim's resource reader
    /// cannot reconstruct the real <c>System.Drawing.Icon</c>, so it hands back the raw <see cref="byte" />
    /// array - but the designer casts the result straight to the shim <see cref="Icon" />
    /// (<c>notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon")</c>), which used to throw
    /// <see cref="InvalidCastException" /> ("Unable to cast Byte[] to System.Drawing.Icon") and crash the
    /// app in <c>Form3.InitializeComponent</c>. <c>GetObject</c> must materialize the shim
    /// <see cref="Icon" /> from those bytes instead.
    /// </summary>
    [Fact]
    public void GetObject_IconEntry_ReturnsIcon_NotRawBytes()
    {
        var resources = new ComponentResourceManager(typeof(Form3));

        var notifyIcon = resources.GetObject("notifyIcon1.Icon");
        Assert.NotNull(notifyIcon);
        Assert.IsNotType<byte[]>(notifyIcon);
        Assert.IsAssignableFrom<Icon>(notifyIcon);

        // The exact cast the generated designer performs must now succeed.
        var typed = (Icon)notifyIcon!;
        Assert.NotNull(typed);

        // The form's own icon is stored the same way and must round-trip identically.
        var formIcon = resources.GetObject("$this.Icon");
        Assert.IsAssignableFrom<Icon>(formIcon);
    }

    /// <summary>
    /// Companion to the icon case: byte-array image entries in the resx must come back as the shim
    /// <see cref="Image" /> the designer casts to, never as a raw <see cref="byte" /> array.
    /// </summary>
    [Fact]
    public void GetObject_ImageEntries_ReturnImages_NotRawBytes()
    {
        var resources = new ComponentResourceManager(typeof(Form3));

        // Any image-typed resx entries must materialize; a raw byte[] leaking through would break the
        // designer's `(Image)resources.GetObject(...)` casts the same way the icon one did.
        foreach (var name in new[] { "button1.Image", "pictureBox1.Image" })
        {
            var value = resources.GetObject(name);
            if (value is null)
            {
                continue; // entry not present in this form - nothing to assert.
            }

            Assert.IsNotType<byte[]>(value);
            Assert.IsAssignableFrom<Image>(value);
        }
    }
}
