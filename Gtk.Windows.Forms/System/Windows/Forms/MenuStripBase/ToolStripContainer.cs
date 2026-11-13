// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Provides panels on each side of the form and a central content panel into which tool strips and other
/// controls can be arranged and docked. This type is emitted by the designer for multi-toolbar forms.
/// </summary>
[DesignerCategory("Component")]
public class ToolStripContainer : ContainerControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly PanelBase self = new();

    private ToolStripContentPanel? _contentPanel;
    private ToolStripPanel? _top;
    private ToolStripPanel? _bottom;
    private ToolStripPanel? _left;
    private ToolStripPanel? _right;

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripContainer" /> class.</summary>
    public ToolStripContainer() : base()
    {
        TopToolStripPanelVisible = true;
        BottomToolStripPanelVisible = true;
        LeftToolStripPanelVisible = true;
        RightToolStripPanelVisible = true;
    }

    /// <summary>Gets the GTK control.</summary>
    public override object GtkControl => self;

    /// <summary>Gets the central <see cref="T:System.Windows.Forms.ToolStripContentPanel" /> of the container.</summary>
    public ToolStripContentPanel ContentPanel => _contentPanel ??= new ToolStripContentPanel();

    /// <summary>Gets the top <see cref="T:System.Windows.Forms.ToolStripPanel" /> of the container.</summary>
    public ToolStripPanel TopToolStripPanel => _top ??= new ToolStripPanel { Dock = DockStyle.Top, Orientation = Orientation.Horizontal };

    /// <summary>Gets the bottom <see cref="T:System.Windows.Forms.ToolStripPanel" /> of the container.</summary>
    public ToolStripPanel BottomToolStripPanel => _bottom ??= new ToolStripPanel { Dock = DockStyle.Bottom, Orientation = Orientation.Horizontal };

    /// <summary>Gets the left <see cref="T:System.Windows.Forms.ToolStripPanel" /> of the container.</summary>
    public ToolStripPanel LeftToolStripPanel => _left ??= new ToolStripPanel { Dock = DockStyle.Left, Orientation = Orientation.Vertical };

    /// <summary>Gets the right <see cref="T:System.Windows.Forms.ToolStripPanel" /> of the container.</summary>
    public ToolStripPanel RightToolStripPanel => _right ??= new ToolStripPanel { Dock = DockStyle.Right, Orientation = Orientation.Vertical };

    /// <summary>Gets or sets a value indicating whether the top panel is visible.</summary>
    public bool TopToolStripPanelVisible { get; set; }

    /// <summary>Gets or sets a value indicating whether the bottom panel is visible.</summary>
    public bool BottomToolStripPanelVisible { get; set; }

    /// <summary>Gets or sets a value indicating whether the left panel is visible.</summary>
    public bool LeftToolStripPanelVisible { get; set; }

    /// <summary>Gets or sets a value indicating whether the right panel is visible.</summary>
    public bool RightToolStripPanelVisible { get; set; }
}
