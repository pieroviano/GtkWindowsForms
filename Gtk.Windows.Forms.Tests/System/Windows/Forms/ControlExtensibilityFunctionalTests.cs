namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the <see cref="Control" /> extensibility surface: right-to-left alignment translation, the
/// keyboard-command routing chain (<c>ProcessKeyMessage</c>/<c>ProcessCmdKey</c>) and the parent-change
/// notification override points. A <see cref="Panel" />-derived probe exposes the protected members; it is
/// created inside a realized form (form first so GTK is initialized), with no clipboard interaction.
/// </summary>
[Collection("NonParallelCollection")]
public class ControlExtensibilityFunctionalTests
{
    private sealed class KeyProbe : Panel
    {
        public bool KeyDownFired;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyDownFired = true;
            base.OnKeyDown(e);
        }

        public bool RunProcessKeyMessage(ref Message m) => ProcessKeyMessage(ref m);

        public bool RunProcessCmdKey(ref Message m, Keys k) => ProcessCmdKey(ref m, k);

        public HorizontalAlignment Rtl(HorizontalAlignment a) => RtlTranslateAlignment(a);

        public gtk::System.Drawing.ContentAlignment RtlContent(gtk::System.Drawing.ContentAlignment a)
            => RtlTranslateAlignment(a);
    }

    private sealed class CmdKeyParent : Panel
    {
        public Keys LastKey;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            LastKey = keyData;
            return true; // claim every command key
        }
    }

    [Fact]
    public void Extensibility_Behaves()
    {
        var form = new Form();
        var parent = new CmdKeyParent();
        var probe = new KeyProbe();
        form.Controls.Add(parent);
        parent.Controls.Add(probe);

        form.Load += (s, e) =>
        {
            try
            {
                // --- RtlTranslate flips left/right only when RightToLeft is Yes ---
                probe.RightToLeft = RightToLeft.No;
                Assert.Equal(HorizontalAlignment.Left, probe.Rtl(HorizontalAlignment.Left));

                probe.RightToLeft = RightToLeft.Yes;
                Assert.Equal(HorizontalAlignment.Right, probe.Rtl(HorizontalAlignment.Left));
                Assert.Equal(HorizontalAlignment.Left, probe.Rtl(HorizontalAlignment.Right));
                Assert.Equal(HorizontalAlignment.Center, probe.Rtl(HorizontalAlignment.Center));
                Assert.Equal(gtk::System.Drawing.ContentAlignment.TopRight,
                    probe.RtlContent(gtk::System.Drawing.ContentAlignment.TopLeft));

                // --- ProcessKeyMessage raises OnKeyDown for a WM_KEYDOWN message ---
                var keyMsg = Message.Create(probe.Handle, 0x0100, (IntPtr)(int)Keys.A, IntPtr.Zero);
                probe.RunProcessKeyMessage(ref keyMsg);
                Assert.True(probe.KeyDownFired);

                // --- ProcessCmdKey forwards to the parent, which handles it ---
                var cmdMsg = Message.Create(probe.Handle, 0x0100, (IntPtr)(int)(Keys.Control | Keys.S), IntPtr.Zero);
                var handled = probe.RunProcessCmdKey(ref cmdMsg, Keys.Control | Keys.S);
                Assert.True(handled);
                Assert.Equal(Keys.Control | Keys.S, parent.LastKey);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}
