// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Creates a container within which other controls (typically tool strips) can share horizontal or vertical
/// space and be arranged into rows.
/// </summary>
[DesignerCategory("Component")]
public class ToolStripPanel : ContainerControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly PanelBase self = new();

    private ToolStripRenderer? _renderer;
    private ToolStripRenderMode _renderMode = ToolStripRenderMode.ManagerRenderMode;
    private Orientation _orientation = Orientation.Horizontal;

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripPanel" /> class.</summary>
    public ToolStripPanel() : base()
    {
    }

    /// <summary>Occurs when the value of the <see cref="P:System.Windows.Forms.ToolStripPanel.Renderer" /> property changes.</summary>
    public event EventHandler? RendererChanged;

    /// <summary>Gets the GTK control.</summary>
    public override object GtkControl => self;

    /// <summary>Gets or sets a value indicating the horizontal or vertical orientation of the panel.</summary>
    public Orientation Orientation
    {
        get => _orientation;
        set => _orientation = value;
    }

    /// <summary>Gets or sets a <see cref="T:System.Windows.Forms.ToolStripRenderer" /> used to customize the appearance of the panel.</summary>
    public ToolStripRenderer Renderer
    {
        get => _renderer ?? ToolStripManager.Renderer;
        set
        {
            if (_renderer != value)
            {
                _renderer = value;
                _renderMode = ToolStripRenderMode.Custom;
                OnRendererChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>Gets or sets the painting styles applied to the panel.</summary>
    public ToolStripRenderMode RenderMode
    {
        get => _renderMode;
        set
        {
            _renderMode = value;
            if (value != ToolStripRenderMode.Custom)
            {
                _renderer = null;
            }
        }
    }

    /// <summary>Gets an array of <see cref="T:System.Windows.Forms.ToolStripPanelRow" /> objects that represent the rows in the panel.</summary>
    public ToolStripPanelRow[] Rows => [new ToolStripPanelRow(this)];

    /// <summary>Raises the <see cref="E:System.Windows.Forms.ToolStripPanel.RendererChanged" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnRendererChanged(EventArgs e) => RendererChanged?.Invoke(this, e);
}
