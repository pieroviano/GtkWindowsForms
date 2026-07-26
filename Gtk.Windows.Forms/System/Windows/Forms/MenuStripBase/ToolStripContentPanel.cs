// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Used to group collections of aligned controls hosted in the central area of a
/// <see cref="T:System.Windows.Forms.ToolStripContainer" />.
/// </summary>
[DesignerCategory("Component")]
public class ToolStripContentPanel : Panel
{
    private ToolStripRenderer? _renderer;
    private ToolStripRenderMode _renderMode = ToolStripRenderMode.ManagerRenderMode;

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripContentPanel" /> class.</summary>
    public ToolStripContentPanel() : base()
    {
    }

    /// <summary>Occurs when the value of the <see cref="P:System.Windows.Forms.ToolStripContentPanel.Renderer" /> property changes.</summary>
    public event EventHandler? RendererChanged;

    /// <summary>Gets or sets a <see cref="T:System.Windows.Forms.ToolStripRenderer" /> used to customize the appearance of the content panel.</summary>
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

    /// <summary>Gets or sets the painting styles applied to the content panel.</summary>
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

    /// <summary>Raises the <see cref="E:System.Windows.Forms.ToolStripContentPanel.RendererChanged" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnRendererChanged(EventArgs e) => RendererChanged?.Invoke(this, e);
}
