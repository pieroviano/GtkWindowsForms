/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Text.RegularExpressions;

namespace System.Windows.Forms
{
    /// <summary>
    /// Uses a mask to distinguish between proper and improper user input.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class MaskedTextBox : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MaskedTextBox" /> class using defaults.
        /// </summary>
        public MaskedTextBox() : base()
        {
            self.AddClass("MaskedTextBox");
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MaskedTextBox" /> class using the specified input mask.
        /// </summary>
        /// <param name="mask">A
        /// <see cref="System.String" /> representing the input mask. The initial value of the
        /// <see cref="System.Windows.Forms.MaskedTextBox.Mask" /> property.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="mask" /> is null.
        /// </exception>
        internal MaskedTextBox(string cssClass) : base()
        {
            self.AddClass(cssClass);
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }
        /// <summary>
        /// Gets or sets the text as it is currently displayed to the user.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> containing the text currently displayed by the control. The default is an empty string.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string Text { get { return self.Text; } set { IsMasking = false; self.Text = value ?? ""; IsMasking = true; } }
        private void Control_Shown(object sender, EventArgs e)
        {
            if (_PasswordChar != '\0')
            {
                self.Visibility = false;
            }
            else if (!string.IsNullOrWhiteSpace(Mask))
            {//按 format formatted assignment
                string txt = Regex.Match(Text, "\\d").Value;
                int windex = -1;
                Text = Regex.Replace(Mask, "\\d", (Match m) =>
                {
                    windex++;
                    if (txt.Length > windex)
                    {
                        return txt[windex].ToString();
                    }
                    else
                    {
                        return "_";
                    }
                });
            }
        }
        string correctText;
        private void Control_TextInserted(object o, Gtk.TextInsertedArgs args)
        {
            if (self.IsRealized && isBackspace == false)
            {
                int position = args.Position;
                string new_text = args.NewText;
                if (IsMaskPassword == true)
                {
                    if (new_text.Length > 1 || self.Text.Length != correctText.Length + 1)
                    {
                        if (correctText != null)
                            self.Text = correctText;
                    }
                    else if (correctText.Length > position && new_text.Length == 1)
                    {
                        if (IsNumberText(correctText.Substring(position - 1, 1)) && IsNumberText(new_text))
                        {
                            //正常
                            self.DeleteText(position, position + 1);
                        }
                        else
                        {
                            self.Text = correctText;
                        }
                    }
                    else
                    {
                        self.DeleteText(position - 1, position);
                    }
                }
            }
            isBackspace = false;
            correctText = self.Text;
        }
        bool isBackspace = false;
        private void Control_Backspace(object sender, EventArgs e)
        {
            if (IsMaskPassword == true)
            {
                // format formatting mask， change numbers only
                int position = self.CursorPosition;
                if (self.Text.Length + 1 == correctText.Length) // delete one character
                {
                    isBackspace = true;
                    if (IsNumberChar(correctText[position]))
                    {
                        self.InsertText("_", ref position);
                    }
                    else
                    {
                        self.InsertText(correctText[position].ToString(), ref position);
                    }
                }
                else if (self.Text.Length + 1 < correctText.Length) // select multiple characters for deletion
                {
                    self.Text = correctText;
                }
            }
        }

        private bool IsNumberText(string text)
        {
            foreach (char w in text)
            {
                if (!IsNumberChar(w))
                    return false;
            }
            return true;
        }
        private bool IsNumberChar(char w)
        {
            return (char.IsNumber(w) || w == '_');
        }
        /// <summary>
        /// Gets or sets the input mask to use at run time.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> representing the current mask. The default value is the empty string which allows any input.
        /// </returns>
        /// <exception cref="System.ArgumentException">The string supplied to the
        /// <see cref="System.Windows.Forms.MaskedTextBox.Mask" /> property is not a valid mask. Invalid masks include masks containing non-printable characters.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public string Mask { get; set; }

        private char _PasswordChar;
        /// <summary>
        /// Gets or sets the character to be displayed in substitute for user input.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Char" /> value used as the password character.
        /// </returns>
        /// <exception cref="System.ArgumentException">The character specified when setting this property is not a valid password character, as determined by the
        /// <see cref="System.ComponentModel.MaskedTextProvider.IsValidPasswordChar(System.Char)" /> method of the
        /// <see cref="System.ComponentModel.MaskedTextProvider" /> class.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The password character specified is the same as the current prompt character,
        /// <see cref="System.Windows.Forms.MaskedTextBox.PromptChar" /> The two are required to be different.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override char PasswordChar { get => _PasswordChar; set { _PasswordChar = value; self.InvisibleChar = value; } }
        /// <summary>
        /// Gets or sets the data type used to verify the data input by the user.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> representing the data type used in validation. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Type ValidatingType { get; set; }
        /// <summary>
        /// Gets or sets a value that determines whether literals and prompt characters are included in the formatted string.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.MaskFormat" /> values. The default is
        /// <see cref="System.Windows.Forms.MaskFormat.IncludeLiterals" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Property set with a
        /// <see cref="System.Windows.Forms.MaskFormat" /> value that is not valid.
        /// </exception>
        public MaskFormat TextMaskFormat { get; set; }

        internal bool IsMasking = true;
        private bool IsMaskPassword
        {
            get { return !string.IsNullOrWhiteSpace(Mask) && IsMasking; }
        }
    }
}
