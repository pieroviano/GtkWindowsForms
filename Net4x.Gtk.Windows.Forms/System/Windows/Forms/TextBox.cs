/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Atk;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    public partial class TextBox: Control
    {
        public readonly TextBoxBase self = new TextBoxBase();
        public override object GtkControl => self;
        public TextBox() : base()
        {
            self.MaxWidthChars = 1;
            self.WidthChars = 0;
            self.Valign = Gtk.Align.Start;
            self.Halign = Gtk.Align.Start;
            self.Changed += Self_Changed;
            self.TextInserted += Self_TextInserted;
            self.KeyPressEvent += Self_KeyPressEvent;
        }

        private void Self_KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
        {
            if (KeyDown != null)
            {
                if (args.Event is Gdk.EventKey eventkey)
                {
                    Keys keys = (Keys)eventkey.HardwareKeycode;
                    KeyDown(this, new KeyEventArgs(keys));
                }
            }
        }

        public override event KeyEventHandler KeyDown;
        private void Self_TextInserted(object o, TextInsertedArgs args)
        {
            if (KeyDown != null && this.GetType().Name == "TextBox")
            {
                string keytext = args.NewText.ToUpper();
                if (char.IsNumber(args.NewText[0]))
                    keytext = "D" + keytext;
                var keyv = Enum.GetValues(typeof(Keys)).Cast<Keys>().Where(k => {
                    return Enum.GetName(typeof(Keys), k) == keytext;
                });
                foreach (var key in keyv) 
                    KeyDown(this, new KeyEventArgs(key));
            }
        }

        private void Self_Changed(object sender, EventArgs e)
        {
            if (TextChanged != null && self.IsVisible) { TextChanged(this, EventArgs.Empty); }
        }

        public string PlaceholderText { get { return self.PlaceholderText; } set { self.PlaceholderText = value ?? ""; } }
        public override string Text { get { return self.Text; } set { self.Text = value ?? ""; } }
        public virtual char PasswordChar { get => self.InvisibleChar; set { self.InvisibleChar = value; self.Visibility = false; } }
        public virtual bool ReadOnly { get { return self.IsEditable == false; } set { self.IsEditable = value == false;  } }
        public override event EventHandler TextChanged;
        public bool Multiline { get; set; }
        public int MaxLength { get => self.MaxLength; set => self.MaxLength = value; }
        public string[] Lines
        {
            get
            {
                return [self.Text];
            }
            set
            {
                if (value is null || value.Length == 0)
                {
                    Text = string.Empty;
                }
                else
                {
                    Text = string.Join(string.Empty, value);
                }
            }
        }
        public int SelectionStart { 
            get { self.GetSelectionBounds(out int start, out int end); return start; }
            set { Select(value, SelectionLength); }
        }

        [System.ComponentModel.Browsable(false)]
        public virtual int SelectionLength
        {
            get { self.GetSelectionBounds(out int start, out int end); return end - start; }
            set
            {
                Select(SelectionStart, value);
            }
        }
        public void Select(int start, int length)
        {
            self.SelectRegion(start, start + length);
        }
        
        public void InsertTextAtCursor(string text)
        {
            if(text == null) return;
            int posi = self.CursorPosition;
            self.InsertText(text,ref posi);
        }
    }
}