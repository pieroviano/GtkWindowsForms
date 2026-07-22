extern alias gtk;
using System.Reflection;
using Pen = gtk::System.Drawing.Pen;
using Pens = gtk::System.Drawing.Pens;
using Brushes = gtk::System.Drawing.Brushes;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

/// <summary>
/// Verifies every named entry of the <see cref="Pens"/> and <see cref="Brushes"/> palettes really exposes
/// a pen/brush of the colour its property name denotes. A mis-mapped entry (e.g. Pens.Coral returning
/// Salmon) would be caught here.
/// </summary>
[Collection("NonParallelCollection")]
public class PaletteFunctionalTests
{
    public static IEnumerable<object[]> PenNames() =>
        typeof(Pens).GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Pen))
            .Select(p => new object[] { p.Name });

    public static IEnumerable<object[]> BrushNames() =>
        typeof(Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(p => new object[] { p.Name });

    [Theory]
    [MemberData(nameof(PenNames))]
    public void Pen_ForNamedColour_HasThatColour(string name)
    {
        var pen = (Pen?)typeof(Pens).GetProperty(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null);

        Assert.NotNull(pen);
        Assert.Equal(Color.FromName(name).ToArgb(), pen!.Color.ToArgb());
    }

    [Theory]
    [MemberData(nameof(BrushNames))]
    public void Brush_ForNamedColour_IsSolidBrushOfThatColour(string name)
    {
        var brush = typeof(Brushes).GetProperty(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null);

        var solid = Assert.IsType<SolidBrush>(brush);
        Assert.Equal(Color.FromName(name).ToArgb(), solid.Color.ToArgb());
    }
}
