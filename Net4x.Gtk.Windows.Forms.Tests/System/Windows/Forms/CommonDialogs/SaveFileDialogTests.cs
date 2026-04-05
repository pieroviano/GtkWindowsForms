using System;
using System.IO;
using Gtk;
using Xunit;

namespace System.Windows.Forms.CommonDialogs
{
    public class SaveFileDialogTests
    {
        // Minimal concrete SaveFileDialog so tests do not open native UI.
        internal class TestSaveFileDialog : SaveFileDialog
        {
            protected override bool RunDialog(IWin32Window? owner) => false;
        }

        [Fact]
        public void CheckWriteAccess_DefaultsToTrue_AndSetterIsNoOp()
        {
            var dlg = new TestSaveFileDialog();

            // Implementation always returns true
            Assert.True(dlg.CheckWriteAccess);

            // Even if we attempt to set to false, implementation returns true
            dlg.CheckWriteAccess = false;
            Assert.True(dlg.CheckWriteAccess);
        }

        [Fact]
        public void OpenFile_ThrowsArgumentNullException_WhenFileNameIsNullOrEmpty()
        {
            var dlg = new TestSaveFileDialog();

            // default FileName is null -> should throw
            Assert.Throws<ArgumentNullException>(() => dlg.OpenFile());

            // empty string also should throw
            dlg.FileName = string.Empty;
            Assert.Throws<ArgumentNullException>(() => dlg.OpenFile());
        }

        [Fact]
        public void OpenFile_CreatesFileStream_ForSpecifiedFile()
        {
            var dlg = new TestSaveFileDialog();
            var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");

            try
            {
                dlg.FileName = tmp;

                using var stream = dlg.OpenFile();
                Assert.NotNull(stream);
                Assert.True(stream.CanWrite);
                Assert.True(File.Exists(tmp));
                Assert.Equal(0, new FileInfo(tmp).Length);
            }
            finally
            {
                try { File.Delete(tmp); } catch { }
            }
        }

        [Fact]
        public void ShowDialog_SetsActionTypeToSave_AndReturnsCancel_WhenRunDialogReturnsFalse()
        {
            var dlg = new TestSaveFileDialog();

            var result = dlg.ShowDialog(null);

            // RunDialog overridden to return false -> ShowDialog should map to Cancel
            Assert.Equal(DialogResult.Cancel, result);

            // Internal ActionType should have been set to Save by ShowDialog
            var asFileDialog = (FileDialog)dlg;
            Assert.Equal(FileChooserAction.Save, asFileDialog.ActionType);
        }
    }
}