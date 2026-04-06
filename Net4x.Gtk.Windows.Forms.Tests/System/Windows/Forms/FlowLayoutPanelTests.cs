using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class FlowLayoutPanelTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();

        // Run GTK-dependent inspections inside a Form.Load / ShowDialog to ensure widgets/models are realized.
        form.Controls.Add(panel);

        Gtk.Orientation actualOrientation = Gtk.Orientation.Horizontal;
        Align actualHalign = Align.Start;
        Align actualValign = Align.Start;
        uint actualMinChildren = 0;
        uint actualMaxChildren = 0;
        uint actualColumnSpacing = 0;
        uint actualBorderWidth = 0;
        object? controlsObj = null;

        form.Load += (s, e) =>
        {
            actualOrientation = panel.self.Orientation;
            actualHalign = panel.self.Halign;
            actualValign = panel.self.Valign;
            actualMinChildren = panel.self.MinChildrenPerLine;
            actualMaxChildren = panel.self.MaxChildrenPerLine;
            actualColumnSpacing = panel.self.ColumnSpacing;
            actualBorderWidth = panel.self.BorderWidth;
            controlsObj = panel.Controls;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(panel.self);
        Assert.Equal(Gtk.Orientation.Horizontal, actualOrientation);
        Assert.Equal(Align.Start, actualHalign);
        Assert.Equal(Align.Start, actualValign);
        Assert.Equal((uint)1, actualMinChildren);
        Assert.Equal((uint)999, actualMaxChildren);
        Assert.Equal(0u, actualColumnSpacing);
        Assert.Equal((uint)0, actualBorderWidth);
        Assert.NotNull(controlsObj);
    }

    [Theory]
    [InlineData(FlowDirection.LeftToRight, Gtk.Orientation.Horizontal)]
    [InlineData(FlowDirection.RightToLeft, Gtk.Orientation.Horizontal)]
    [InlineData(FlowDirection.TopDown, Gtk.Orientation.Vertical)]
    [InlineData(FlowDirection.BottomUp, Gtk.Orientation.Vertical)]
    public void FlowDirection_ShouldSetCorrectOrientation(FlowDirection direction, Gtk.Orientation expected)
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        form.Controls.Add(panel);

        Gtk.Orientation observed = Gtk.Orientation.Horizontal;

        form.Load += (s, e) =>
        {
            panel.FlowDirection = direction;
            observed = panel.self.Orientation;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(expected, observed);
    }

    [Fact]
    public void WrapContents_ShouldSetAndGetValue()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        form.Controls.Add(panel);

        bool panelWrapContents=false;
        bool wrapContents = true;
        form.Load += (s, e) =>
        {
            panel.WrapContents = true;
            panelWrapContents = panel.WrapContents;
            panel.WrapContents = false;
            wrapContents = panel.WrapContents;
            form.Close();
        };
        form.ShowDialog();

        Assert.True(panelWrapContents);

        Assert.False(wrapContents);
    }

    [Fact]
    public void CanExtend_ShouldAlwaysReturnTrue()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        form.Controls.Add(panel);
        bool result=false;
        form.Load += (s, e) =>
        {
            result = panel.CanExtend(new object());
            form.Close();
        };
        form.ShowDialog();

        Assert.True(result);
    }

    private void Form_Load(object? sender, EventArgs e) => throw new NotImplementedException();

    [Fact]
    public void GetFlowBreak_ShouldAlwaysReturnFalse_ByDefault()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        var control = new Label();
        form.Controls.Add(panel);
        bool result = true;
        form.Load += (s, e) =>
        {
            result = panel.GetFlowBreak(control);
            form.Close();
        };
        form.ShowDialog();

        Assert.False(result);
    }

    private void Form_Load1(object? sender, EventArgs e) => throw new NotImplementedException();

    [Fact]
    public void SetFlowBreak_ShouldNotThrow()
    {
        var panel = new FlowLayoutPanel();
        var control = new Label();

        var exception = Record.Exception(() => panel.SetFlowBreak(control, true));

        Assert.Null(exception);
    }

    [Fact]
    public void Controls_Add_ShouldAddControlToCollection()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        var control = new Label();
        form.Controls.Add(panel);
        bool contains = false;
        Control? parentAfter = null;

        form.Load += (s, e) =>
        {
            panel.Controls.Add(control);
            contains = panel.Controls.Contains(control);
            parentAfter = control.Parent;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(contains);
        Assert.Equal(panel, parentAfter);
    }

    [Fact]
    public void Controls_Add_ShouldSetControlProperties()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        var control = new Label();
        form.Controls.Add(panel);

        bool lockLocation = false;
        System.Drawing.Point location = new System.Drawing.Point(-1, -1);

        form.Load += (s, e) =>
        {
            panel.Controls.Add(control);
            lockLocation = control.LockLocation;
            location = control.Location;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(lockLocation);
        Assert.Equal(new System.Drawing.Point(0, 0), location);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var panel = new FlowLayoutPanel();
        form.Controls.Add(panel);
        object? gtkControl = null;
        FlowLayoutPanelBase? self=null;
        form.Load += (s, e) =>
        {
            gtkControl = panel.GtkControl;
            self = panel.self;
            form.Close();
        };
        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}