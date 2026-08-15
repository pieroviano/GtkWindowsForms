namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// A container that keeps its children in a more specific collection must still expose them through
/// <see cref="Control.Controls" />.
/// </summary>
/// <remarks>
/// Found by the Gtk3/Gtk4 sample parity suite: walking <c>Controls</c> recursively reported 54 controls on
/// the reference build but only 29 on the port, because <see cref="TableLayoutPanel" /> declared its
/// <c>Controls</c> property with <c>new</c> and the base kept a second, empty collection. Anything reaching
/// a container through a <see cref="Control" /> reference - a recursive search, serialization, a designer -
/// silently skipped everything nested inside it.
/// </remarks>
[Collection("NonParallelCollection")]
public class ContainerControlsExposureTests
{
    // Constructing these controls builds GTK widgets, which needs the type system initialised.
    public ContainerControlsExposureTests() => Application.Init();

    [Fact]
    public void TableLayoutPanel_ExposesItsChildrenThroughTheBaseControlsProperty()
    {
        var table = new TableLayoutPanel { ColumnCount = 2, RowCount = 1 };
        var child = new Button { Name = "nested" };

        table.Controls.Add(child, 0, 0);

        // Reached as a Control, exactly as a generic tree walk would.
        Control asControl = table;

        Assert.NotNull(asControl.Controls);
        Assert.Equal(1, asControl.Controls!.Count);
        Assert.Same(child, asControl.Controls[0]);
    }

    [Fact]
    public void TableLayoutPanel_TypedAndBaseCollectionsAreTheSameInstance()
    {
        // The two views must not be allowed to drift apart again; same instance is the invariant that
        // guarantees it.
        var table = new TableLayoutPanel();
        Control asControl = table;

        Assert.Same(table.Controls, asControl.Controls);
    }

    [Fact]
    public void TabPage_ExposesItsChildrenThroughTheBaseControlsProperty()
    {
        var page = new TabPage("page");
        var child = new Button { Name = "onPage" };

        page.Controls.Add(child);

        Control asControl = page;

        Assert.NotNull(asControl.Controls);
        Assert.Contains(child, asControl.Controls!.Cast<Control>());
    }

    [Fact]
    public void NestedContainers_AreFullyWalkable()
    {
        // The shape the parity suite tripped over: controls inside panels inside a table.
        var table = new TableLayoutPanel { ColumnCount = 1, RowCount = 1 };
        var panel = new Panel { Name = "panel" };
        var deep = new Button { Name = "deep" };

        panel.Controls.Add(deep);
        table.Controls.Add(panel, 0, 0);

        Assert.Equal(2, Walk(table).Count());
        Assert.Contains(Walk(table), c => ReferenceEquals(c, deep));
    }

    private static IEnumerable<Control> Walk(Control root)
    {
        if (root.Controls == null)
        {
            yield break;
        }

        foreach (Control child in root.Controls)
        {
            yield return child;

            foreach (var descendant in Walk(child))
            {
                yield return descendant;
            }
        }
    }
}
