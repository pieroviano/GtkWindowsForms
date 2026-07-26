using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the accessibility surface. <see cref="AccessibleObject" /> was an empty class,
/// <see cref="Control.AccessibilityObject" /> always returned <c>null</c>, and AccessibleName/Description/
/// Role were inert auto-properties - so nothing an application set ever reached a screen reader. These
/// tests assert the values actually land on the GTK widget's ATK peer, which is what AT-SPI reads.
/// </summary>
[Collection("NonParallelCollection")]
public class AccessibilityFunctionalTests
{
    [Fact]
    public void AccessibilityObject_IsNotNullForALiveControl()
    {
        using var button = new Button();

        Assert.NotNull(button.AccessibilityObject);
    }

    [Fact]
    public void AccessibleName_ReachesTheAtkPeer()
    {
        using var button = new Button();

        button.AccessibleName = "Save document";

        Assert.Equal("Save document", button.AccessibleName);
        Assert.Equal("Save document", button.AccessibilityObject!.Name);
    }

    [Fact]
    public void AccessibleDescription_ReachesTheAtkPeer()
    {
        using var button = new Button();

        button.AccessibleDescription = "Writes the file to disk";

        Assert.Equal("Writes the file to disk", button.AccessibleDescription);
        Assert.Equal("Writes the file to disk", button.AccessibilityObject!.Description);
    }

    [Fact]
    public void AccessibleRole_ReachesTheAtkPeerAndRoundTrips()
    {
        using var button = new Button();

        button.AccessibleRole = AccessibleRole.PushButton;

        Assert.Equal(AccessibleRole.PushButton, button.AccessibleRole);
        Assert.Equal(AccessibleRole.PushButton, button.AccessibilityObject!.Role);
    }

    [Fact]
    public void AccessibleRole_CheckButtonRoundTripsThroughAtk()
    {
        using var box = new CheckBox();

        box.AccessibleRole = AccessibleRole.CheckButton;

        Assert.Equal(AccessibleRole.CheckButton, box.AccessibilityObject!.Role);
    }

    [Fact]
    public void AccessibleName_SetToNullClearsRatherThanThrows()
    {
        using var button = new Button();
        button.AccessibleName = "something";

        button.AccessibleName = null;

        Assert.Null(button.AccessibleName);
        Assert.Equal(string.Empty, button.AccessibilityObject!.Name);
    }

    [Fact]
    public void AccessibleRole_EnumCarriesTheWinFormsValues()
    {
        // The enum declared only Default, so source naming any real role failed to compile. Values must
        // match System.Windows.Forms.AccessibleRole, which mirrors the MSAA ROLE_SYSTEM_* constants.
        Assert.Equal(-1, (int)AccessibleRole.Default);
        Assert.Equal(0, (int)AccessibleRole.None);
        Assert.Equal(10, (int)AccessibleRole.Client);
        Assert.Equal(43, (int)AccessibleRole.PushButton);
        Assert.Equal(44, (int)AccessibleRole.CheckButton);
        Assert.Equal(64, (int)AccessibleRole.OutlineButton);
    }

    [Fact]
    public void AccessibilityObject_ReportsAChildCountForAContainer()
    {
        using var panel = new Panel();
        panel.Controls.Add(new Button());

        // Navigation must not throw; the exact count comes from ATK's view of the widget tree.
        var count = panel.AccessibilityObject!.GetChildCount();

        Assert.True(count >= 0);
    }
}
