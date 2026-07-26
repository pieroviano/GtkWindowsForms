using System.Windows.Forms;
using Point = global::System.Drawing.Point;
using Rectangle = global::System.Drawing.Rectangle;
using Size = global::System.Drawing.Size;
using SizeF = global::System.Drawing.SizeF;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary><c>AutoScaleMode</c> and <c>AutoScaleDimensions</c> were inert
/// auto-properties, yet every designer-generated form sets them — four forms in this repository's own
/// sample app do. With them inert, a layout produced at one font metric is never rescaled at another,
/// which is the case a cross-platform layer hits hardest.</summary>
[Collection("NonParallelCollection")]
public class AutoScaleFunctionalTests
{
    /// <summary>A form with one child, laid out as a designer would leave it.</summary>
    private static Form DesignedForm(AutoScaleMode mode, SizeF designedAgainst, out Button child)
    {
        var form = new Form();
        form.SuspendLayout();
        child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.ClientSize = new Size(300, 200);
        form.AutoScaleDimensions = designedAgainst;
        form.AutoScaleMode = mode;

        return form;
    }

    [Fact]
    public void AutoScaleModeRoundTrips()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };

        Assert.Equal(AutoScaleMode.Font, form.AutoScaleMode);
    }

    [Fact]
    public void AutoScaleDimensionsRoundTrip()
    {
        using var form = new Form { AutoScaleDimensions = new SizeF(9f, 20f) };

        Assert.Equal(9f, form.AutoScaleDimensions.Width, 2);
        Assert.Equal(20f, form.AutoScaleDimensions.Height, 2);
    }

    [Fact]
    public void CurrentAutoScaleDimensions_AreMeasuredForFontMode()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };

        var current = form.CurrentAutoScaleDimensions;

        Assert.True(current.Width > 0f && current.Height > 0f,
            $"font-mode dimensions should be measured from the font but were {current}");
    }

    [Fact]
    public void CurrentAutoScaleDimensions_AreTheDpiForDpiMode()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Dpi };

        var current = form.CurrentAutoScaleDimensions;

        Assert.Equal(form.DeviceDpi, current.Width, 2);
        Assert.Equal(form.DeviceDpi, current.Height, 2);
    }

    [Fact]
    public void CurrentAutoScaleDimensions_AreEmptyWhenScalingIsOff()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.None };

        Assert.Equal(0f, form.CurrentAutoScaleDimensions.Width, 2);
    }

    [Fact]
    public void AutoScaleFactor_IsOneWhenNoDesignDimensionsAreSet()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };

        var factor = form.AutoScaleFactor;

        Assert.Equal(1f, factor.Width, 3);
        Assert.Equal(1f, factor.Height, 3);
    }

    [Fact]
    public void AutoScaleFactor_IsOneWhenTheFormWasDesignedAgainstTheCurrentMetric()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        form.AutoScaleDimensions = form.CurrentAutoScaleDimensions;

        var factor = form.AutoScaleFactor;

        Assert.Equal(1f, factor.Width, 2);
        Assert.Equal(1f, factor.Height, 2);
    }

    [Fact]
    public void AutoScaleFactor_ReportsTheRatioToTheDesignMetric()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var current = form.CurrentAutoScaleDimensions;

        // Pretend the form was designed against a metric exactly half the current one.
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        var factor = form.AutoScaleFactor;

        Assert.Equal(2f, factor.Width, 2);
        Assert.Equal(2f, factor.Height, 2);
    }

    [Fact]
    public void PerformAutoScale_ScalesTheChildrenByTheFactor()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);

        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        form.PerformAutoScale();

        Assert.Equal(200, child.Width);
        Assert.Equal(80, child.Height);
    }

    [Fact]
    public void PerformAutoScale_MovesTheChildrenAsWellAsResizingThem()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);

        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        form.PerformAutoScale();

        Assert.Equal(new Point(20, 40), child.Location);
    }

    [Fact]
    public void PerformAutoScale_DoesNothingWhenScalingIsOff()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.None };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.AutoScaleDimensions = new SizeF(1f, 1f);

        form.PerformAutoScale();

        Assert.Equal(100, child.Width);
        Assert.Equal(40, child.Height);
    }

    [Fact]
    public void PerformAutoScale_DoesNothingForInheritMode()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Inherit };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.AutoScaleDimensions = new SizeF(1f, 1f);

        form.PerformAutoScale();

        Assert.Equal(100, child.Width);
    }

    [Fact]
    public void PerformAutoScale_LeavesTheLayoutAloneWhenTheFactorIsOne()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.AutoScaleDimensions = form.CurrentAutoScaleDimensions;

        form.PerformAutoScale();

        Assert.Equal(new Rectangle(10, 20, 100, 40), child.Bounds);
    }

    [Fact]
    public void PerformAutoScale_RebasesTheDesignDimensionsSoItIsIdempotent()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);

        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        form.PerformAutoScale();
        var afterFirst = child.Bounds;
        form.PerformAutoScale();

        Assert.Equal(afterFirst, child.Bounds);
    }

    [Fact]
    public void PerformAutoScale_ScalesNestedChildrenToo()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var panel = new Panel { Bounds = new Rectangle(0, 0, 200, 100) };
        var nested = new Button { Bounds = new Rectangle(5, 5, 50, 20) };
        panel.Controls!.Add(nested);
        form.Controls.Add(panel);

        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        form.PerformAutoScale();

        Assert.Equal(100, nested.Width);
        Assert.Equal(40, nested.Height);
    }

    [Fact]
    public void ResumeLayout_DoesNotRescaleADesignerBuiltFormBehindTheCallersBack()
    {
        // Deliberate: a designer-generated form carries AutoScaleDimensions baked in by the Windows
        // designer, so the ratio against a Pango-measured font is the difference between two typefaces,
        // not a change of environment. Auto-applying it degrades the layout, so scaling is opt-in.
        using var form = new Form();
        form.SuspendLayout();
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.AutoScaleMode = AutoScaleMode.Font;
        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        form.ResumeLayout(true);

        Assert.Equal(new Rectangle(10, 20, 100, 40), child.Bounds);
    }

    [Fact]
    public void AnExplicitPerformAutoScaleStillScales()
    {
        // The API is live even though nothing applies it implicitly.
        using var form = new Form();
        form.SuspendLayout();
        var child = new Button { Bounds = new Rectangle(10, 20, 100, 40) };
        form.Controls.Add(child);
        form.AutoScaleMode = AutoScaleMode.Font;
        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);
        form.ResumeLayout(true);

        form.PerformAutoScale();

        Assert.Equal(200, child.Width);
    }

    [Fact]
    public void ResumeLayout_LeavesAFormWithScalingOffUntouched()
    {
        using var form = DesignedForm(AutoScaleMode.None, new SizeF(1f, 1f), out var child);

        form.ResumeLayout(true);

        Assert.Equal(new Rectangle(10, 20, 100, 40), child.Bounds);
        form.Dispose();
    }

    [Fact]
    public void AUserControlAlsoCarriesTheAutoScaleMembers()
    {
        // UserControl had its own shadowing copies of these; it now inherits the working implementation.
        using var control = new UserControl { AutoScaleMode = AutoScaleMode.Font };
        control.AutoScaleDimensions = new SizeF(6f, 13f);

        Assert.Equal(AutoScaleMode.Font, control.AutoScaleMode);
        Assert.Equal(6f, control.AutoScaleDimensions.Width, 2);
        Assert.True(control.CurrentAutoScaleDimensions.Width > 0f,
            "a UserControl should measure its current dimensions too");
    }

    [Fact]
    public void AUserControlScalesItsChildren()
    {
        using var control = new UserControl { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(4, 6, 40, 20) };
        control.Controls!.Add(child);

        var current = control.CurrentAutoScaleDimensions;
        control.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);

        control.PerformAutoScale();

        Assert.Equal(80, child.Width);
    }

    [Fact]
    public void SettingTheModeAgainAllowsScalingToBeReapplied()
    {
        using var form = new Form { AutoScaleMode = AutoScaleMode.Font };
        var child = new Button { Bounds = new Rectangle(0, 0, 100, 40) };
        form.Controls.Add(child);

        var current = form.CurrentAutoScaleDimensions;
        form.AutoScaleDimensions = new SizeF(current.Width / 2f, current.Height / 2f);
        form.PerformAutoScale();

        Assert.Equal(200, child.Width);
    }
}
