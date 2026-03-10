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

        internal bool IsMasking = true;

        string correctText;

        bool isBackspace = false;

        private char _PasswordChar;

        public MaskedTextBox() : base()
        {
            self.AddClass("MaskedTextBox");
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }

        internal MaskedTextBox(string cssClass) : base()
        {
            self.AddClass(cssClass);
            self.Backspace += Control_Backspace;
            self.TextInserted += Control_TextInserted;
            self.Shown += Control_Shown;
        }

        public string Mask { get; set; }

        public override char PasswordChar { get => _PasswordChar; set { _PasswordChar = value; self.InvisibleChar = value; } }

        public override string Text { get { return self.Text; } set { IsMasking = false; self.Text = value ?? ""; IsMasking = true; } }

        public MaskFormat TextMaskFormat { get; set; }

        public Type ValidatingType { get; set; }

        private bool IsMaskPassword
        {
            get { return !string.IsNullOrWhiteSpace(Mask) && IsMasking; }
        }

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

        private bool IsNumberChar(char w)
        {
            return (char.IsNumber(w) || w == '_');
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

    }
}
