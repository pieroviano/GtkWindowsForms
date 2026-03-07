/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a control consisting of a movable bar that divides a container's display area into two resizable panels.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class SplitContainer : ContainerControl
    {

        public int _SplitterDistance;

        public readonly SplitContainerBase self = new SplitContainerBase();

        private FixedPanel _fixedPanel = FixedPanel.Panel1;

        private SplitterPanel _panel1;

        private SplitterPanel _panel2;

        private int _SplitterWidth;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> class.
        /// </summary>
        public SplitContainer() : base()
        {
            _panel1 = new SplitterPanel(this);
            _panel1.contaner.Name = "Child1";
            _panel2 = new SplitterPanel(this);
            _panel2.contaner.Name = "Child2";
            self.Pack1(_panel1.Widget, false, true);
            self.Pack2(_panel2.Widget, true, true);
        }
        /// <summary>
        /// Gets or sets which
        /// <see cref="System.Windows.Forms.SplitContainer" /> panel remains the same size when the container is resized.
        /// </summary>
        /// <returns>One of the values of
        /// <see cref="System.Windows.Forms.FixedPanel" /> The default value is None.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.FixedPanel" /> values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        public FixedPanel FixedPanel
        {
            get => _fixedPanel;
            set
            {
                _fixedPanel = value;
                bool resize = value == FixedPanel.Panel2;
                ((Paned.PanedChild)self[_panel1.Widget]).Resize = resize;
                ((Paned.PanedChild)self[_panel2.Widget]).Resize = !resize;
            }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets a value indicating the horizontal or vertical orientation of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> panels.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Orientation" /> values. The default is Vertical.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.Orientation" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Orientation Orientation
        {
            get { return self.Orientation == Gtk.Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal; }
            set
            {
                self.Orientation = value == Orientation.Horizontal ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal;
            }
        }

        /// <summary>
        /// Gets the left or top panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" />, depending on
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" />
        /// </summary>
        /// <returns>If
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" /> is Vertical, the left panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> If
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" /> is Horizontal, the top panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public SplitterPanel Panel1
        {
            get
            {
                return _panel1;
            }
            set
            {
                _panel1 = value;
            }
        }

        /// <summary>
        /// Gets the right or bottom panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" />, depending on
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" />
        /// </summary>
        /// <returns>If
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" /> is Vertical, the right panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> If
        /// <see cref="System.Windows.Forms.SplitContainer.Orientation" /> is Horizontal, the bottom panel of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public SplitterPanel Panel2
        {
            get
            {
                return _panel2;
            }
            set
            {
                _panel2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the location of the splitter, in pixels, from the left or top edge of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the location of the splitter, in pixels, from the left or top edge of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> The default value is 50 pixels.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than zero.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The value is incompatible with the orientation.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SplitterDistance { get => self.Position + 5; set { _SplitterDistance = Math.Max(1, value - 5); self.Position = _SplitterDistance; } }

        /// <summary>
        /// Gets or sets a value representing the increment of splitter movement in pixels.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the increment of splitter movement in pixels. The default value is one pixel.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than one.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public int SplitterIncrement { get; set; }

        /// <summary>
        /// Gets or sets the width of the splitter in pixels.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the width of the splitter, in pixels. The default is four pixels.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than one or is incompatible with the orientation.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SplitterWidth { get { return _SplitterWidth; } set { _SplitterWidth = value; self.WideHandle = value > 2; } }
    }

}
