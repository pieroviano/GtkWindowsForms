// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents the container that an MDI (multiple-document interface) parent form uses to host its child
/// forms. The type was absent, so <c>form.MdiClient</c>, <c>is MdiClient</c> and <c>LayoutMdi</c> did not
/// compile. It is a scrollable container whose children are the MDI child windows; <see cref="LayoutMdi" />
/// cascades or tiles them.
/// </summary>
public sealed class MdiClient : ScrollableControl
{
    /// <summary>The backing GTK container widget.</summary>
    public readonly PanelBase self = new();

    private readonly Control.ControlCollection _controls;

    /// <summary>Initializes a new instance of the <see cref="MdiClient" /> class.</summary>
    public MdiClient() : base()
    {
        self.Override.sender = this;
        _controls = new Control.ControlCollection(this, self.contaner);
    }

    /// <summary>Gets the collection of MDI child controls hosted by this client.</summary>
    public override Control.ControlCollection Controls => _controls;

    /// <summary>Gets the underlying GTK widget.</summary>
    public override object GtkControl => self;

    /// <summary>Arranges the MDI child forms within the client according to <paramref name="value" />.</summary>
    /// <param name="value">The layout to apply.</param>
    public void LayoutMdi(MdiLayout value)
    {
        if (_controls == null || _controls.Count == 0)
        {
            return;
        }

        var children = new List<Control>();
        foreach (Control child in _controls)
        {
            children.Add(child);
        }

        var area = ClientRectangle;
        if (area.Width <= 0 || area.Height <= 0)
        {
            area = new Rectangle(0, 0, Math.Max(1, Width), Math.Max(1, Height));
        }

        switch (value)
        {
            case MdiLayout.Cascade:
                // Each child steps down and to the right by one title-bar height.
                const int step = 24;
                for (var i = 0; i < children.Count; i++)
                {
                    var offset = i * step;
                    var w = Math.Max(1, area.Width - offset - step);
                    var h = Math.Max(1, area.Height - offset - step);
                    children[i].SetBounds(area.X + offset, area.Y + offset, w, h, BoundsSpecified.All);
                }

                break;

            case MdiLayout.TileHorizontal:
                {
                    var rowHeight = Math.Max(1, area.Height / children.Count);
                    for (var i = 0; i < children.Count; i++)
                    {
                        children[i].SetBounds(area.X, area.Y + i * rowHeight, area.Width, rowHeight, BoundsSpecified.All);
                    }

                    break;
                }

            case MdiLayout.TileVertical:
                {
                    var colWidth = Math.Max(1, area.Width / children.Count);
                    for (var i = 0; i < children.Count; i++)
                    {
                        children[i].SetBounds(area.X + i * colWidth, area.Y, colWidth, area.Height, BoundsSpecified.All);
                    }

                    break;
                }

            case MdiLayout.ArrangeIcons:
                // Minimized-child icon arrangement has no representation on this layer.
                break;
        }
    }

    /// <summary>Represents the collection of controls (MDI child windows) hosted by an <see cref="MdiClient" />.</summary>
    public new class ControlCollection : Control.ControlCollection
    {
        /// <summary>Initializes a new instance for the given owner.</summary>
        /// <param name="owner">The owning <see cref="MdiClient" />.</param>
        public ControlCollection(MdiClient owner) : base(owner)
        {
        }
    }
}
