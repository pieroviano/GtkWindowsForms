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
