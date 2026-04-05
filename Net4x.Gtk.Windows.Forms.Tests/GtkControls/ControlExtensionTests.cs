using System.Windows.Forms;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using NSubstitute;

using Application= System.Windows.Forms.Application;
using Label = Gtk.Label;

namespace GtkControls;

[Collection("NonParallelCollection")]
public class ControlExtensionTests
{
    public ControlExtensionTests()
    {
        Application.Init();
    }

    [Fact]
    public void GetGridCell_Returns_Cell_When_All_Properties_Are_Valid()
    {
        // Arrange
        var grid = new Grid();
        var parent = new Label();
        var widget = new Label();

        grid.Attach(parent, 1, 2, 3, 4);
        widget.Parent = parent;

        var control = new TestControl(widget);

        // Act
        var result = control.GetGridCell(grid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.LeftAttach);
        Assert.Equal(2, result.TopAttach);
        Assert.Equal(3, result.Width);
        Assert.Equal(4, result.Height);
    }

    [Fact]
    public void GetGridCell_Returns_NotNull_When_Grid_Has_No_Attach_Properties()
    {
        // Arrange
        var grid = new Grid();
        var parent = new Label();
        var widget = new Label();

        // Add to grid WITHOUT Attach (no layout metadata)
        grid.Add(parent);

        widget.Parent = parent;

        var control = new TestControl(widget);

        // Act
        var result = control.GetGridCell(grid);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetGridCell_Returns_NotNull_When_Widget_Not_In_Grid()
    {
        // Arrange
        var grid = new Grid();
        var parent = new Label(); // NOT added to grid
        var widget = new Label
        {
            Parent = parent
        };

        var control = new TestControl(widget);

        // Act
        var result = control.GetGridCell(grid);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetGridCell_Returns_Null_When_Widget_Has_No_Parent()
    {
        // Arrange
        var grid = new Grid();
        var widget = new Label(); // no parent

        var control = new TestControl(widget);

        // Act
        var result = control.GetGridCell(grid);

        // Assert
        Assert.Null(result);
    }

    // Helper control
    private class TestControl : Control
    {
        public TestControl(Widget widget)
        {
            Widget = widget;
            var iControlGtk = Substitute.For<IControlGtk>();
            iControlGtk.Override.Returns(new GtkControlOverride(widget));
            GtkControl = iControlGtk;
        }

        public override Widget Widget { get; }

        public override object? GtkControl { get; set; }
    }
}