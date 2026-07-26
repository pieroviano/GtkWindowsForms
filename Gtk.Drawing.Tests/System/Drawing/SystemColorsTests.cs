extern alias gtk;

using System.Reflection;
using Graphics = gtk::System.Drawing.Graphics;
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class SystemColorsTests
{
    [Fact]
    public void AllPublicStaticProperties_MapTo_ColorFromName()
    {
        // Find the internal SystemColors type in the same assembly as Color
        var sysType = typeof(Graphics).Assembly.GetType("System.Drawing.SystemColors");
        Assert.NotNull(sysType);

        var props = sysType.GetProperties(BindingFlags.Public | BindingFlags.Static);
        Assert.NotEmpty(props);

        foreach (var prop in props.Where(p => p.Name != "UseAlternativeColorSet"))
        {
            var value = prop.GetValue(null);
            Assert.IsType<Color>(value);

            var actual = (Color)value;
            var expected = Color.FromName(prop.Name);

            // Color.FromName should match the implementation used by SystemColors in this repo.
            Assert.Equal(expected.ToArgb(), actual.ToArgb());
        }
    }

    [Fact]
    public void CoreSystemColors_AreNotTransparent()
    {
        var sysType = typeof(Color).Assembly.GetType("System.Drawing.SystemColors");
        Assert.NotNull(sysType);

        string[] checkNames = new[] { "Window", "Highlight", "ButtonFace", "ControlText", "WindowText" };

        foreach (var name in checkNames)
        {
            var prop = sysType.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(prop);

            var color = (Color)prop.GetValue(null);
            Assert.NotEqual(Color.Transparent, color);
            Assert.True(color.A != 0, $"Expected non-transparent alpha for SystemColors.{name}");
        }
    }
}