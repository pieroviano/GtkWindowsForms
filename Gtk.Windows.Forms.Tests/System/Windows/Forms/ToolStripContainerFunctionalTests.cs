namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the designer-emitted ToolStrip container controls (<see cref="ToolStripContainer" />,
/// <see cref="ToolStripPanel" />, <see cref="ToolStripContentPanel" />, <see cref="ToolStripPanelRow" />).
/// Hierarchy claims are checked by reflection; instance behaviour runs in one realized-form lifecycle
/// (form first so GTK is initialized), with no clipboard interaction.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripContainerFunctionalTests
{
    [Fact]
    public void Hierarchy_MatchesWinForms()
    {
        Assert.True(typeof(ContainerControl).IsAssignableFrom(typeof(ToolStripContainer)));
        Assert.True(typeof(ContainerControl).IsAssignableFrom(typeof(ToolStripPanel)));
        Assert.True(typeof(Panel).IsAssignableFrom(typeof(ToolStripContentPanel)));
        Assert.True(typeof(global::System.ComponentModel.Component).IsAssignableFrom(typeof(ToolStripPanelRow)));
    }

    [Fact]
    public void Container_PanelsAndContentPanel_Behave()
    {
        var form = new Form();
        var container = new ToolStripContainer();
        form.Controls.Add(container);

        form.Load += (s, e) =>
        {
            try
            {
                // ContentPanel and the four edge panels are stable, lazily-created instances.
                var content = container.ContentPanel;
                Assert.NotNull(content);
                Assert.Same(content, container.ContentPanel);

                var top = container.TopToolStripPanel;
                Assert.Same(top, container.TopToolStripPanel);
                Assert.Equal(DockStyle.Top, top.Dock);
                Assert.Equal(Orientation.Horizontal, top.Orientation);

                Assert.Equal(Orientation.Vertical, container.LeftToolStripPanel.Orientation);
                Assert.Equal(DockStyle.Right, container.RightToolStripPanel.Dock);

                // visibility flags default true and round-trip
                Assert.True(container.TopToolStripPanelVisible);
                container.BottomToolStripPanelVisible = false;
                Assert.False(container.BottomToolStripPanelVisible);

                // a panel exposes at least one row whose orientation follows the panel
                var panel = new ToolStripPanel { Orientation = Orientation.Vertical };
                var rows = panel.Rows;
                Assert.NotEmpty(rows);
                Assert.Equal(Orientation.Vertical, rows[0].Orientation);
                Assert.Same(panel, rows[0].ToolStripPanel);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }

    [Fact]
    public void ContentPanel_Renderer_RoundTrips()
    {
        var form = new Form();
        var container = new ToolStripContainer();
        form.Controls.Add(container);

        form.Load += (s, e) =>
        {
            try
            {
                var content = container.ContentPanel;
                Assert.NotNull(content.Renderer); // defaults to ToolStripManager.Renderer

                var custom = new ToolStripProfessionalRenderer();
                content.Renderer = custom;
                Assert.Same(custom, content.Renderer);
                Assert.Equal(ToolStripRenderMode.Custom, content.RenderMode);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}
