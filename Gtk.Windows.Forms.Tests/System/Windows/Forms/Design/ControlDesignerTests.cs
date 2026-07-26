using System.Drawing;
using System.Reflection;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Windows.Forms.Design;

public class ControlDesignerTests
{
    [Fact]
    public void InvalidPoint_ShouldBeMinValues()
    {
        var field = typeof(ControlDesigner)
            .GetField("InvalidPoint", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(field);

        var value = (Point)field.GetValue(null);

        Assert.Equal(int.MinValue, value.X);
        Assert.Equal(int.MinValue, value.Y);
    }

    [Fact]
    public void ForceVisible_Default_IsTrue()
    {
        var designer = CreateInstance();

        var prop = typeof(ControlDesigner)
            .GetProperty("ForceVisible", BindingFlags.Instance | BindingFlags.NonPublic);

        var value = (bool)prop.GetValue(designer);

        Assert.True(value);
    }

    [Fact]
    public void ForceVisible_CanBeSet()
    {
        var designer = CreateInstance();

        var prop = typeof(ControlDesigner)
            .GetProperty("ForceVisible", BindingFlags.Instance | BindingFlags.NonPublic);

        prop.SetValue(designer, false);

        var value = (bool)prop.GetValue(designer);

        Assert.False(value);
    }

    [Fact]
    public void MouseDragLast_Default_IsInvalidPoint()
    {
        var designer = CreateInstance();

        var field = typeof(ControlDesigner)
            .GetField("_mouseDragLast", BindingFlags.Instance | BindingFlags.NonPublic);

        var value = (Point)field.GetValue(designer);

        Assert.Equal(int.MinValue, value.X);
        Assert.Equal(int.MinValue, value.Y);
    }

    private static ControlDesigner CreateInstance()
    {
        // ControlDesigner has no public constructor → use reflection
        return (ControlDesigner)Activator.CreateInstance(typeof(ControlDesigner), nonPublic: true);
    }
}