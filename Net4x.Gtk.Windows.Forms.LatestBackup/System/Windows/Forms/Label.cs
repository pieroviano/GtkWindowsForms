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
    /// Represents a standard Windows label.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class Label : Control
    {

        public readonly LabelBase self = new LabelBase();
        private System.Drawing.ContentAlignment textAlign;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Label" /> class.
        /// </summary>
        public Label() : base()
        {

        }

        public override object GtkControl => self;

        public override RightToLeft RightToLeft { get { return self.Direction == Gtk.TextDirection.Rtl ? RightToLeft.Yes : RightToLeft.No; } set { self.Direction = value == RightToLeft.Yes ? Gtk.TextDirection.Rtl : Gtk.TextDirection.Ltr; } }

        /// <returns>
        /// The text associated with this control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string Text { get => self.Text; set { self.Text = value; } }

        /// <summary>
        /// Gets or sets the alignment of text in the label.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.ContentAlignment" /> values. The default is
        /// <see cref="System.Drawing.ContentAlignment.TopLeft" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Drawing.ContentAlignment" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Drawing.ContentAlignment TextAlign
        {
            get { return textAlign; }
            set
            {
                textAlign = value;
                if (value == System.Drawing.ContentAlignment.TopLeft)
                {
                    self.Xalign = 0.0f;
                    self.Yalign = 0.0f;
                }
                else if (value == System.Drawing.ContentAlignment.TopCenter)
                {
                    self.Xalign = 0.5f;
                    self.Yalign = 0.0f;
                }
                else if (value == System.Drawing.ContentAlignment.TopRight)
                {
                    self.Xalign = 1.0f;
                    self.Yalign = 0.0f;
                }
                else if (value == System.Drawing.ContentAlignment.MiddleLeft)
                {
                    self.Xalign = 0.0f;
                    self.Yalign = 0.5f;
                }
                else if (value == System.Drawing.ContentAlignment.MiddleCenter)
                {
                    self.Xalign = 0.5f;
                    self.Yalign = 0.5f;
                }
                else if (value == System.Drawing.ContentAlignment.MiddleRight)
                {
                    self.Xalign = 1.0f;
                    self.Yalign = 0.5f;
                }
                else if (value == System.Drawing.ContentAlignment.BottomLeft)
                {
                    self.Xalign = 0.0f;
                    self.Yalign = 1.0f;
                }
                else if (value == System.Drawing.ContentAlignment.BottomCenter)
                {
                    self.Xalign = 0.5f;
                    self.Yalign = 1.0f;
                }
                else if (value == System.Drawing.ContentAlignment.BottomRight)
                {
                    self.Xalign = 1.0f;
                    self.Yalign = 1.0f;
                }

            }
        }
    }
}
