extern alias gtk;
using System.Windows.Forms;
using Rectangle = global::System.Drawing.Rectangle;
using Size = global::System.Drawing.Size;
using ToolboxBitmapAttribute = gtk::System.Drawing.ToolboxBitmapAttribute;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Covers the §3.4/§3.7 types that did not exist: <see cref="ErrorProvider"/>, the standard
/// WinForms validation pattern; <see cref="Splitter"/>, the legacy splitter an old application docks; and
/// <c>ToolboxBitmapAttribute</c>, whose absence broke compilation on the *declaration* of any custom
/// control.</summary>
[Collection("NonParallelCollection")]
public class CommonControlsFunctionalTests
{
    // ---- ToolboxBitmapAttribute -------------------------------------------

    /// <summary>A control declared the way a real custom-control library declares one.</summary>
    [ToolboxBitmap(typeof(CommonControlsFunctionalTests), "no-such-image.bmp")]
    private sealed class DecoratedControl : Control
    {
    }

    [Fact]
    public void AControlCanBeDeclaredWithAToolboxBitmap()
    {
        // The point of the test is that this type compiles and instantiates at all.
        using var control = new DecoratedControl();

        Assert.NotNull(control);
    }

    [Fact]
    public void TheAttributeIsReadableFromTheDeclaringType()
    {
        var attribute = (ToolboxBitmapAttribute?)Attribute.GetCustomAttribute(
            typeof(DecoratedControl), typeof(ToolboxBitmapAttribute));

        Assert.NotNull(attribute);
    }

    [Fact]
    public void AMissingImageYieldsNullRatherThanThrowing()
    {
        var attribute = new ToolboxBitmapAttribute(typeof(CommonControlsFunctionalTests), "no-such-image.bmp");

        Assert.Null(attribute.GetImage(typeof(CommonControlsFunctionalTests)));
    }

    [Fact]
    public void AMissingFileYieldsNullRatherThanThrowing()
    {
        var attribute = new ToolboxBitmapAttribute("no-such-file-anywhere.bmp");

        Assert.Null(attribute.GetImage((object?)null));
    }

    [Fact]
    public void EquivalentAttributesCompareEqual()
    {
        var first = new ToolboxBitmapAttribute(typeof(CommonControlsFunctionalTests), "icon.bmp");
        var second = new ToolboxBitmapAttribute(typeof(CommonControlsFunctionalTests), "icon.bmp");
        var different = new ToolboxBitmapAttribute(typeof(CommonControlsFunctionalTests), "other.bmp");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, different);
    }

    // ---- ErrorProvider ----------------------------------------------------

    [Fact]
    public void SetError_ThenGetError_ReturnsTheMessage()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();

        provider.SetError(box, "Name is required");

        Assert.Equal("Name is required", provider.GetError(box));
    }

    [Fact]
    public void GetError_ForAControlWithNoErrorIsEmpty()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();

        Assert.Equal(string.Empty, provider.GetError(box));
    }

    [Fact]
    public void SetError_WithAnEmptyMessageClearsTheError()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();
        provider.SetError(box, "Name is required");

        provider.SetError(box, string.Empty);

        Assert.Equal(string.Empty, provider.GetError(box));
    }

    [Fact]
    public void SetError_TracksEachControlSeparately()
    {
        using var provider = new ErrorProvider();
        using var name = new TextBox();
        using var email = new TextBox();

        provider.SetError(name, "Name is required");
        provider.SetError(email, "Email is malformed");

        Assert.Equal("Name is required", provider.GetError(name));
        Assert.Equal("Email is malformed", provider.GetError(email));
    }

    [Fact]
    public void SetError_ReplacesAPreviousMessage()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();

        provider.SetError(box, "first");
        provider.SetError(box, "second");

        Assert.Equal("second", provider.GetError(box));
    }

    [Fact]
    public void Clear_RemovesEveryError()
    {
        using var provider = new ErrorProvider();
        using var name = new TextBox();
        using var email = new TextBox();
        provider.SetError(name, "Name is required");
        provider.SetError(email, "Email is malformed");

        provider.Clear();

        Assert.Equal(string.Empty, provider.GetError(name));
        Assert.Equal(string.Empty, provider.GetError(email));
    }

    [Fact]
    public void TheErrorReachesTheControlAsAnIndicator()
    {
        // WinForms draws a blinking icon beside the control; this layer surfaces the text on the control
        // itself, which is the indicator it can address reliably.
        using var provider = new ErrorProvider();
        using var box = new TextBox();

        provider.SetError(box, "Name is required");

        Assert.Equal("Name is required", box.Widget!.TooltipText);
    }

    [Fact]
    public void ClearingTheErrorRemovesTheIndicator()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();
        provider.SetError(box, "Name is required");

        provider.SetError(box, null);

        Assert.True(string.IsNullOrEmpty(box.Widget!.TooltipText), "the indicator should be removed with the error");
    }

    [Fact]
    public void IconAlignmentRoundTripsPerControl()
    {
        using var provider = new ErrorProvider();
        using var name = new TextBox();
        using var email = new TextBox();

        provider.SetIconAlignment(name, ErrorIconAlignment.TopLeft);
        provider.SetIconAlignment(email, ErrorIconAlignment.BottomRight);

        Assert.Equal(ErrorIconAlignment.TopLeft, provider.GetIconAlignment(name));
        Assert.Equal(ErrorIconAlignment.BottomRight, provider.GetIconAlignment(email));
        Assert.Equal(ErrorIconAlignment.MiddleRight, provider.GetIconAlignment(new TextBox()));
    }

    [Fact]
    public void IconPaddingRoundTripsPerControl()
    {
        using var provider = new ErrorProvider();
        using var box = new TextBox();

        provider.SetIconPadding(box, 6);

        Assert.Equal(6, provider.GetIconPadding(box));
        Assert.Equal(0, provider.GetIconPadding(new TextBox()));
    }

    [Fact]
    public void ProviderPropertiesRoundTrip()
    {
        using var provider = new ErrorProvider
        {
            BlinkRate = 400,
            BlinkStyle = ErrorBlinkStyle.NeverBlink,
            DataMember = "Rows",
            RightToLeft = true,
            Tag = 9
        };

        Assert.Equal(400, provider.BlinkRate);
        Assert.Equal(ErrorBlinkStyle.NeverBlink, provider.BlinkStyle);
        Assert.Equal("Rows", provider.DataMember);
        Assert.True(provider.RightToLeft);
        Assert.Equal(9, provider.Tag);
    }

    [Fact]
    public void CanExtend_IsTrueForControlsButNotForms()
    {
        using var provider = new ErrorProvider();

        Assert.True(provider.CanExtend(new TextBox()), "an ErrorProvider extends controls");
        Assert.False(provider.CanExtend(new Form()), "it does not extend the form itself");
        Assert.False(provider.CanExtend("not a control"), "it does not extend arbitrary objects");
    }

    [Fact]
    public void SetError_RejectsANullControl()
    {
        using var provider = new ErrorProvider();

        Assert.Throws<ArgumentNullException>(() => provider.SetError(null!, "boom"));
    }

    [Fact]
    public void TheProviderCanBeConstructedAgainstAContainerControl()
    {
        using var form = new Form();
        using var provider = new ErrorProvider(form);

        Assert.Same(form, provider.ContainerControl);
    }

    [Fact]
    public void TheValidationIdiomWorksEndToEnd()
    {
        // The pattern this type exists for.
        using var form = new Form();
        using var provider = new ErrorProvider(form);
        var box = new TextBox { Text = string.Empty };
        form.Controls.Add(box);

        if (string.IsNullOrWhiteSpace(box.Text))
        {
            provider.SetError(box, "Name is required");
        }
        else
        {
            provider.SetError(box, string.Empty);
        }

        Assert.Equal("Name is required", provider.GetError(box));

        box.Text = "Ada";
        provider.SetError(box, string.Empty);

        Assert.Equal(string.Empty, provider.GetError(box));
    }

    // ---- Splitter ---------------------------------------------------------

    [Fact]
    public void ASplitterIsAControlThatDocksToAnEdge()
    {
        using var splitter = new Splitter();

        Assert.Equal(DockStyle.Left, splitter.Dock);
        Assert.True(splitter.IsVertical, "a left-docked splitter runs vertically");
    }

    [Fact]
    public void DockingToTheTopMakesItHorizontal()
    {
        using var splitter = new Splitter { Dock = DockStyle.Top };

        Assert.False(splitter.IsVertical, "a top-docked splitter runs horizontally");
    }

    [Fact]
    public void ASplitterRefusesToFillItsContainer()
    {
        using var splitter = new Splitter();

        Assert.Throws<ArgumentException>(() => splitter.Dock = DockStyle.Fill);
        Assert.Throws<ArgumentException>(() => splitter.Dock = DockStyle.None);
    }

    [Fact]
    public void MinSizeAndMinExtraRoundTrip()
    {
        using var splitter = new Splitter { MinSize = 40, MinExtra = 60 };

        Assert.Equal(40, splitter.MinSize);
        Assert.Equal(60, splitter.MinExtra);
    }

    [Fact]
    public void SplitPositionIsMinusOneWithNothingToResize()
    {
        using var splitter = new Splitter();

        Assert.Equal(-1, splitter.SplitPosition);
    }

    [Fact]
    public void SettingSplitPositionResizesTheDockedSibling()
    {
        using var form = new Form { Size = new Size(400, 300) };
        var panel = new Panel { Dock = DockStyle.Left, Bounds = new Rectangle(0, 0, 100, 300) };
        var splitter = new Splitter { Dock = DockStyle.Left };
        form.Controls.Add(panel);
        form.Controls.Add(splitter);

        splitter.SplitPosition = 150;

        Assert.Equal(150, panel.Width);
        Assert.Equal(150, splitter.SplitPosition);
    }

    [Fact]
    public void SplitPositionIsClampedByMinSize()
    {
        using var form = new Form { Size = new Size(400, 300) };
        var panel = new Panel { Dock = DockStyle.Left, Bounds = new Rectangle(0, 0, 100, 300) };
        var splitter = new Splitter { Dock = DockStyle.Left, MinSize = 80 };
        form.Controls.Add(panel);
        form.Controls.Add(splitter);

        splitter.SplitPosition = 10;

        Assert.Equal(80, panel.Width);
    }

    [Fact]
    public void MovingTheSplitterRaisesBothEvents()
    {
        using var form = new Form { Size = new Size(400, 300) };
        var panel = new Panel { Dock = DockStyle.Left, Bounds = new Rectangle(0, 0, 100, 300) };
        var splitter = new Splitter { Dock = DockStyle.Left };
        form.Controls.Add(panel);
        form.Controls.Add(splitter);

        var moving = 0;
        var moved = 0;
        splitter.SplitterMoving += (_, _) => moving++;
        splitter.SplitterMoved += (_, _) => moved++;

        splitter.SplitPosition = 150;

        Assert.Equal(1, moving);
        Assert.Equal(1, moved);
    }

    [Fact]
    public void AHorizontalSplitterResizesHeightRatherThanWidth()
    {
        using var form = new Form { Size = new Size(400, 300) };
        var panel = new Panel { Dock = DockStyle.Top, Bounds = new Rectangle(0, 0, 400, 80) };
        var splitter = new Splitter { Dock = DockStyle.Top };
        form.Controls.Add(panel);
        form.Controls.Add(splitter);

        splitter.SplitPosition = 120;

        Assert.Equal(120, panel.Height);
    }
}
