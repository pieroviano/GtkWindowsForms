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
    public class MaskedTextBox : TextBox
    {

        /// <summary>
        /// The is masking
        /// </summary>
        internal bool IsMasking = true;

        /// <summary>
        /// The correct text
        /// </summary>
        string correctText;

        /// <summary>
        /// The is backspace
        /// </summary>
        bool isBackspace = false;

        /// <summary>
        /// The password character
        /// </summary>
        private char _PasswordChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedTextBox"/> class.
        /// </summary>
        public MaskedTextBox() : base()
        {
            self.AddClass("MaskedTextBox");
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedTextBox"/> class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        internal MaskedTextBox(string cssClass) : base()
        {
            self.AddClass(cssClass);
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>The mask.</value>
        public string Mask { get; set; }

        /// <summary>
        /// Gets or sets the password character.
        /// </summary>
        /// <value>The password character.</value>
        public override char PasswordChar { get => _PasswordChar; set { _PasswordChar = value; self.InvisibleChar = value; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public override string Text { get { return self.Text; } set { IsMasking = false; self.Text = value ?? ""; IsMasking = true; } }

        /// <summary>
        /// Gets or sets the text mask format.
        /// </summary>
        /// <value>The text mask format.</value>
        public MaskFormat TextMaskFormat { get; set; }

        /// <summary>
        /// Gets or sets the type of the validating.
        /// </summary>
        /// <value>The type of the validating.</value>
        public Type ValidatingType { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is mask password.
        /// </summary>
        /// <value><c>true</c> if this instance is mask password; otherwise, <c>false</c>.</value>
        private bool IsMaskPassword
        {
            get { return !string.IsNullOrWhiteSpace(Mask) && IsMasking; }
        }

        /// <summary>
        /// Handles the Backspace event of the Control control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Shown event of the Control control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Controls the text inserted.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
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

        /// <summary>
        /// Determines whether [is number character] [the specified w].
        /// </summary>
        /// <param name="w">The w.</param>
        /// <returns><c>true</c> if [is number character] [the specified w]; otherwise, <c>false</c>.</returns>
        private bool IsNumberChar(char w)
        {
            return (char.IsNumber(w) || w == '_');
        }

        /// <summary>
        /// Determines whether [is number text] [the specified text].
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if [is number text] [the specified text]; otherwise, <c>false</c>.</returns>
        private bool IsNumberText(string text)
        {
            foreach (char w in text)
            {
                if (!IsNumberChar(w))
                    return false;
            }

            return true;
        }

    }
}
