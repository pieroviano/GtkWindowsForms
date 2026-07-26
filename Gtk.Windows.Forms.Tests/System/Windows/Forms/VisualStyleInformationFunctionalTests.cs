namespace System.Windows.Forms;

using System.Windows.Forms.VisualStyles;

/// <summary>
/// Verifies <see cref="VisualStyleInformation" />, which reports the active theme state. On this GTK layer a
/// theme is always present, so visual styles report as supported and enabled. No GTK widgets are created.
/// </summary>
public class VisualStyleInformationFunctionalTests
{
    [Fact]
    public void ReportsSupportedAndEnabled()
    {
        Assert.True(VisualStyleInformation.IsSupportedByOS);
        Assert.True(VisualStyleInformation.IsEnabledByUser);
        Assert.True(VisualStyleInformation.SupportsFlatMenus);
        Assert.Equal(32, VisualStyleInformation.MinimumColorDepth);
    }

    [Fact]
    public void MetadataStringsAreNonNull()
    {
        // These never throw and never return null, so theme-probing code stays safe.
        Assert.NotNull(VisualStyleInformation.ColorScheme);
        Assert.NotNull(VisualStyleInformation.DisplayName);
        Assert.NotNull(VisualStyleInformation.Company);
        Assert.NotNull(VisualStyleInformation.Version);
    }

    [Fact]
    public void ThemeColorsAreValid()
    {
        Assert.False(VisualStyleInformation.TextControlBorder.IsEmpty);
        Assert.False(VisualStyleInformation.ControlHighlightHot.IsEmpty);
    }
}
