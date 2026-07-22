using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// <see cref="Padding.Horizontal" /> computed <c>Right - Left</c> and <see cref="Padding.Vertical" />
/// computed <c>Bottom - Top</c>, contradicting both their own documentation and WinForms, where each is
/// the sum of its two edges. Symmetric padding - overwhelmingly the common case - therefore reported 0,
/// so every "content area = size minus padding" calculation silently used the full size.
/// </summary>
public class PaddingArithmeticFunctionalTests
{
    [Fact]
    public void Horizontal_IsTheSumOfLeftAndRight()
    {
        var padding = new Padding(3, 5, 7, 11);

        Assert.Equal(10, padding.Horizontal);
    }

    [Fact]
    public void Vertical_IsTheSumOfTopAndBottom()
    {
        var padding = new Padding(3, 5, 7, 11);

        Assert.Equal(16, padding.Vertical);
    }

    [Fact]
    public void SymmetricPadding_DoesNotCancelToZero()
    {
        // The failing case that mattered: uniform padding reported no padding at all.
        var padding = new Padding(5);

        Assert.Equal(10, padding.Horizontal);
        Assert.Equal(10, padding.Vertical);
    }

    [Fact]
    public void Size_ReportsTheCombinedPaddingRatherThanEmpty()
    {
        var padding = new Padding(3, 5, 7, 11);

        Assert.Equal(new global::System.Drawing.Size(10, 16), padding.Size);
    }

    [Fact]
    public void All_ReportsTheCommonEdgeWhenEveryEdgeMatches()
    {
        var padding = new Padding(6);

        Assert.Equal(6, padding.All);
    }

    [Fact]
    public void All_ReportsMinusOneWhenEdgesDiffer()
    {
        // This returned the left edge, so differing padding looked uniform.
        var padding = new Padding(3, 5, 7, 11);

        Assert.Equal(-1, padding.All);
    }

    [Fact]
    public void All_SetterAppliesToEveryEdge()
    {
        var padding = new Padding(3, 5, 7, 11) { All = 4 };

        Assert.Equal(4, padding.Left);
        Assert.Equal(4, padding.Top);
        Assert.Equal(4, padding.Right);
        Assert.Equal(4, padding.Bottom);
    }

    [Fact]
    public void EmptyPadding_HasNoExtent()
    {
        var padding = new Padding(0);

        Assert.Equal(0, padding.Horizontal);
        Assert.Equal(0, padding.Vertical);
        Assert.Equal(global::System.Drawing.Size.Empty, padding.Size);
    }
}
