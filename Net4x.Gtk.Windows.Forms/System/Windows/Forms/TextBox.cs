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
    /// <summary>
    /// Represents a Windows text box control.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class TextBox : Control
    {

        public readonly TextBoxBase self = new TextBoxBase();

        public override event KeyEventHandler KeyDown;

        public override event EventHandler TextChanged;

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

        public override object GtkControl => self;

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

        public int MaxLength { get => self.MaxLength; set => self.MaxLength = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a multiline
        /// <see cref="System.Windows.Forms.TextBox" /> control.
        /// </summary>
        /// <returns>true if the control is a multiline
        /// <see cref="System.Windows.Forms.TextBox" /> control; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Multiline { get; set; }

        /// <summary>
        /// Gets or sets the character used to mask characters of a password in a single-line
        /// <see cref="System.Windows.Forms.TextBox" /> control.
        /// </summary>
        /// <returns>The character used to mask characters entered in a single-line
        /// <see cref="System.Windows.Forms.TextBox" /> control. Set the value of this property to 0 (character value) if you do not want the control to mask characters as they are typed. Equals 0 (character value) by default.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual char PasswordChar { get => self.InvisibleChar; set { self.InvisibleChar = value; self.Visibility = false; } }

        public string PlaceholderText { get { return self.PlaceholderText; } set { self.PlaceholderText = value ?? ""; } }

        public virtual bool ReadOnly { get { return self.IsEditable == false; } set { self.IsEditable = value == false; } }

        [System.ComponentModel.Browsable(false)]
        public virtual int SelectionLength
        {
            get
            {
                self.GetSelectionBounds(out int start, out int end);

                return end - start;
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
                self.GetSelectionBounds(out int start, out int end);

                return start;
            }
            set { Select(value, SelectionLength); }
        }

        /// <summary>
        /// Gets or sets the current text in the
        /// <see cref="System.Windows.Forms.TextBox" />
        /// </summary>
        /// <returns>The text displayed in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string Text { get { return self.Text; } set { self.Text = value ?? ""; } }

        public void InsertTextAtCursor(string text)
        {
            if (text == null) return;
            int posi = self.CursorPosition;
            self.InsertText(text, ref posi);
        }

        public void Select(int start, int length)
        {
            self.SelectRegion(start, start + length);
        }

        private void Self_Changed(object sender, EventArgs e)
        {
            if (TextChanged != null && self.IsVisible) { TextChanged(this, EventArgs.Empty); }
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

        private void Self_TextInserted(object o, TextInsertedArgs args)
        {
            if (KeyDown != null && this.GetType().Name == "TextBox")
            {
                string keytext = args.NewText.ToUpper();
                if (char.IsNumber(args.NewText[0]))
                    keytext = "D" + keytext;
                var keyv = Enum.GetValues(typeof(Keys)).Cast<Keys>().Where(k =>
                {
                    return Enum.GetName(typeof(Keys), k) == keytext;
                });
                foreach (var key in keyv)
                    KeyDown(this, new KeyEventArgs(key));
            }
        }

    }
}
