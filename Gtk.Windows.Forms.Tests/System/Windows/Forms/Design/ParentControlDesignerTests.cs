using System.ComponentModel;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Windows.Forms.Design;

[Collection("NonParallelCollection")]
public class ParentControlDesignerTests
{
    public ParentControlDesignerTests()
    {
        Application.Init();
    }

    [Fact]
    public void CanModifyComponents_WithControl_ReturnsTrue()
    {
        var designer = CreateDesigner(new Panel());

        Assert.True(designer.CanModifyComponents);
    }

    [Fact]
    public void CanModifyComponents_WithNonControl_ReturnsFalse()
    {
        var designer = CreateDesigner(new TestComponent());

        Assert.False(designer.CanModifyComponents);
    }

    [Fact]
    public void AddComponent_Null_ReturnsFalse()
    {
        var designer = CreateDesigner(new Panel());

        var result = designer.AddComponent(null, "test", true);

        Assert.False(result);
    }

    [Fact]
    public void AddComponent_NonControl_ReturnsFalse()
    {
        var designer = CreateDesigner(new Panel());

        var result = designer.AddComponent(new TestComponent(), "test", true);

        Assert.False(result);
    }

    [Fact]
    public void AddComponent_ValidControl_AddsToParent()
    {
        var parent = new Panel();
        var designer = CreateDesigner(parent);

        var button = new Button();

        var result = designer.AddComponent(button, "btn1", true);

        Assert.True(result);
        Assert.Single(parent.Controls);
        Assert.Equal(button, parent.Controls[0]);
        Assert.Equal("btn1", button.Name);
    }

    [Fact]
    public void AddComponent_FirstAdd_SetsLocation()
    {
        var parent = new Panel();
        var designer = CreateDesigner(parent);

        var button = new Button();

        designer.AddComponent(button, "btn1", true);

        Assert.Equal(new System.Drawing.Point(10, 10), button.Location);
    }

    [Fact]
    public void AddComponent_NotFirstAdd_DoesNotForceLocation()
    {
        var parent = new Panel();
        var designer = CreateDesigner(parent);

        var button = new Button { Location = new System.Drawing.Point(50, 50) };

        designer.AddComponent(button, "btn1", false);

        Assert.Equal(new System.Drawing.Point(50, 50), button.Location);
    }

    [Fact]
    public void GetControlForComponent_WithControl_ReturnsSame()
    {
        var designer = CreateDesigner(new Panel());
        var button = new Button();

        var result = designer.GetControlForComponent(button);

        Assert.Equal(button, result);
    }

    [Fact]
    public void GetControlForComponent_Invalid_Throws()
    {
        var designer = CreateDesigner(new Panel());

        Assert.Throws<ArgumentException>(() =>
            designer.GetControlForComponent(new TestComponent()));
    }

    [Fact]
    public void GetDesignerControl_ReturnsRootControl()
    {
        var panel = new Panel();
        var designer = CreateDesigner(panel);

        var result = designer.GetDesignerControl();

        Assert.Equal(panel, result);
    }

    [Fact]
    public void GetDesignerControl_NoControl_Throws()
    {
        var designer = CreateDesigner(new TestComponent());

        Assert.Throws<InvalidOperationException>(() =>
            designer.GetDesignerControl());
    }

    [Fact]
    public void IsDropOk_WithControl_ReturnsTrue()
    {
        var designer = CreateDesigner(new Panel());

        var result = designer.IsDropOk(new Button());

        Assert.True(result);
    }

    [Fact]
    public void IsDropOk_WithNonControl_ReturnsFalse()
    {
        var designer = CreateDesigner(new Panel());

        var result = designer.IsDropOk(new TestComponent());

        Assert.False(result);
    }

    [Fact]
    public void IsDropOk_WhenCannotModify_ReturnsFalse()
    {
        var designer = CreateDesigner(new TestComponent());

        var result = designer.IsDropOk(new Button());

        Assert.False(result);
    }

    // Helpers

    private static ParentControlDesigner CreateDesigner(IComponent component)
    {
        var designer = new ParentControlDesigner
        {
            Component = component
        };
        return designer;
    }

    private class TestComponent : Component
    {
    }
}