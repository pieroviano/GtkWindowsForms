/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Gtk;
using Container = Gtk.Widget;

namespace System.Windows.Forms;

/// <summary>
/// Represents the collection of data bindings for a control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public partial class Control
{
    internal class ArrangedElementWidget : IArrangedElement
    {
        private Widget? _widget;

        public virtual event EventHandler? Disposed;

        internal ArrangedElementWidget(Gtk.Widget widget)
        {
            _widget = widget;
        }

        public Rectangle Bounds
        {
            get
            {
                var widgetAllocation = _widget?.Allocation;
                if (widgetAllocation.HasValue)
                {
                    return new Rectangle(widgetAllocation.Value.X, widgetAllocation.Value.Y, widgetAllocation.Value.Width, widgetAllocation.Value.Height);
                }
                return Rectangle.Empty;
            }
        }

        public ArrangedElementCollection Children => ArrangedElementCollection.Empty;

        public IArrangedElement? Container => null;

        public Rectangle DisplayRectangle
        {
            get
            {
                var widgetAllocation = _widget?.Allocation;
                if (widgetAllocation.HasValue)
                {
                    return new Rectangle(widgetAllocation.Value.X, widgetAllocation.Value.Y, widgetAllocation.Value.Width, widgetAllocation.Value.Height);
                }
                return Rectangle.Empty;
            }
        }

        public bool ParticipatesInLayout => _widget?.Visible ?? false;

        public PropertyStore Properties { get; } = new PropertyStore();

        public ISite? Site
        {
            get => null;
            set { }
        }

        public Widget? GetWidget => _widget;

        public void Dispose()
        {
            _widget?.Dispose();
            _widget = null;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public Size GetPreferredSize(Size proposedSize)
        {
            // Return the widget's requested size or the proposed size as fallback
            if (_widget != null)
            {
                _widget.GetPreferredSize(out var min, out var nat);
                return new Size(nat.Width, nat.Height);
            }
            return proposedSize;
        }

        public void PerformLayout(IArrangedElement affectedElement, string? propertyName)
        {
            // No-op for widget wrapper
        }

        public void SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            if (_widget != null)
            {
                _widget.SetSizeRequest(bounds.Width, bounds.Height);
                _widget.SizeAllocate(new Gdk.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), -1); // Gtk4: SetAllocation -> SizeAllocate
            }
        }
    }
}