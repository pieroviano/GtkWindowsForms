/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows rich text box control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class RichTextBox : ScrollableControl
    {

        public readonly RichTextBoxBase self = new RichTextBoxBase();

        public override event EventHandler TextChanged;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.RichTextBox" /> class.
        /// </summary>
        public RichTextBox() : base()
        {
            self.TextView.Buffer.Changed += Buffer_Changed;
            this.BorderStyle = BorderStyle.Fixed3D;
        }

        public override object GtkControl => self;

        public string[] Lines
        {
            get
            {
                return self.TextView.Buffer.Text.Split([Environment.NewLine], StringSplitOptions.None);
            }
            set
            {
                if (value is null || value.Length == 0)
                {
                    Text = string.Empty;
                }
                else
                {
                    Text = string.Join(Environment.NewLine, value);
                }
            }
        }

        public virtual bool ReadOnly { get { return self.TextView.CanFocus; } set { self.TextView.CanFocus = value; } }

        /// <summary>
        /// Gets or sets the number of characters selected in control.
        /// </summary>
        /// <returns>The number of characters selected in the text box.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [System.ComponentModel.Browsable(false)]
        public virtual int SelectionLength
        {
            get
            {
                self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end);

                return end.Offset - start.Offset;
            }
            set
            {
                Select(SelectionStart, value);
            }
        }

        public int SelectionStart
        {
            get
            {
                self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end);

                return start.Offset;
            }
            set { Select(value, SelectionLength); }
        }

        /// <summary>
        /// Gets or sets the current text in the rich text box.
        /// </summary>
        /// <returns>The text displayed in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string Text { get => self.TextView.Buffer.Text; set => self.TextView.Buffer.Text = value; }
        public void AppendText(string text)
        {
            var enditer = self.TextView.Buffer.EndIter;
            self.TextView.Buffer.Insert(ref enditer, text);
        }

        public void InsertTextAtCursor(string text)
        {
            if (text == null) return;
            self.TextView.Buffer.InsertAtCursor(text);
        }

        public void Select(int start, int length)
        {
            TextIter startiter = self.TextView.Buffer.GetIterAtOffset(start);
            TextIter enditer = self.TextView.Buffer.GetIterAtOffset(start + length);
            self.TextView.Buffer.SelectRange(startiter, enditer);
        }
        protected override void SetStyle(Widget widget)
        {
            self.TextView.Name = this.Name;
            base.SetStyle(self.TextView);
        }
        private void Buffer_Changed(object sender, EventArgs e)
        {
            if (TextChanged != null && self.IsVisible)
            {
                TextChanged(this, e);
            }
        }
    }
}
