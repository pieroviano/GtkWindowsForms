using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for <see cref="MdiClient"/> (§4 of API-COMPLETENESS). The type was absent, so <c>is MdiClient</c>,
/// type references and <c>LayoutMdi</c> did not compile. It is a Control-derived scrollable container; the
/// tests pin that it is a Control with a working child collection, that LayoutMdi cascades and tiles its
/// children, and that the nested ControlCollection constructs.
/// </summary>
[Collection("NonParallelCollection")]
public class MdiClientFunctionalTests
{
    static MdiClientFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void MdiClient_IsAControl_WithAChildCollection()
    {
        using var client = new MdiClient();

        Assert.IsAssignableFrom<Control>(client);
        Assert.NotNull(client.Controls);
    }

    [Fact]
    public void LayoutMdi_Cascade_StepsEachChildDownAndRight()
    {
        using var client = new MdiClient();
        client.Size = new global::System.Drawing.Size(400, 300);
        var a = new Button();
        var b = new Button();
        client.Controls.Add(a);
        client.Controls.Add(b);

        client.LayoutMdi(MdiLayout.Cascade);

        // The second child is offset from the first by the cascade step in both axes.
        Assert.True(b.Left > a.Left, "cascade must step the second child to the right");
        Assert.True(b.Top > a.Top, "cascade must step the second child down");
    }

    [Fact]
    public void LayoutMdi_TileHorizontal_StacksChildrenVertically()
    {
        using var client = new MdiClient();
        client.Size = new global::System.Drawing.Size(400, 300);
        var a = new Button();
        var b = new Button();
        client.Controls.Add(a);
        client.Controls.Add(b);

        client.LayoutMdi(MdiLayout.TileHorizontal);

        // Horizontal tiling puts each child in its own full-width row, so the second sits below the first.
        Assert.True(b.Top > a.Top, "the second child must be in a lower row");
    }

    [Fact]
    public void LayoutMdi_WithNoChildren_DoesNotThrow()
    {
        using var client = new MdiClient();

        var ex = Record.Exception(() => client.LayoutMdi(MdiLayout.Cascade));

        Assert.Null(ex);
    }

    [Fact]
    public void NestedControlCollection_ConstructsForItsOwner()
    {
        using var client = new MdiClient();

        var collection = new MdiClient.ControlCollection(client);

        Assert.NotNull(collection);
    }
}
