namespace System.Windows.Forms;

using System.Collections.ObjectModel;

/// <summary>
/// Verifies the fourth sweep batch: <see cref="FileDialogCustomPlace" />/
/// <see cref="FileDialogCustomPlacesCollection" />, the static <see cref="ImeContext" /> helper, and
/// <see cref="ThreadExceptionDialog" />. The first three are GTK-free; the dialog is constructed directly
/// (the Form static constructor initializes GTK), without showing it.
/// </summary>
public class SupportTypesBatchDFunctionalTests
{
    [Fact]
    public void FileDialogCustomPlace_ConstructorsRoundTrip()
    {
        var byPath = new FileDialogCustomPlace(@"C:\data");
        Assert.Equal(@"C:\data", byPath.Path);
        Assert.Equal(Guid.Empty, byPath.KnownFolderGuid);

        var guid = Guid.NewGuid();
        var byGuid = new FileDialogCustomPlace(guid);
        Assert.Equal(guid, byGuid.KnownFolderGuid);
        Assert.Equal(string.Empty, byGuid.Path);
        Assert.Contains("C:\\data", byPath.ToString());
    }

    [Fact]
    public void FileDialogCustomPlacesCollection_AddsPathsAndGuids()
    {
        var c = new FileDialogCustomPlacesCollection();
        Assert.IsAssignableFrom<Collection<FileDialogCustomPlace>>(c);

        c.Add(@"C:\one");
        c.Add(Guid.NewGuid());
        Assert.Equal(2, c.Count);
        Assert.Equal(@"C:\one", c[0].Path);
        Assert.NotEqual(Guid.Empty, c[1].KnownFolderGuid);
    }

    [Fact]
    public void ImeContext_StaticHelpers_DefaultAndNoThrow()
    {
        Assert.Equal(ImeMode.NoControl, ImeContext.GetImeMode(IntPtr.Zero));
        Assert.False(ImeContext.IsOpen(IntPtr.Zero));

        // no-ops must not throw
        ImeContext.Enable(IntPtr.Zero);
        ImeContext.Disable(IntPtr.Zero);
        ImeContext.SetImeStatus(ImeMode.On, IntPtr.Zero);
        ImeContext.SetOpenStatus(true, IntPtr.Zero);
    }

    [Fact]
    public void ThreadExceptionDialog_IsFormAndCarriesException()
    {
        var ex = new InvalidOperationException("boom");
        var dialog = new ThreadExceptionDialog(ex);
        try
        {
            Assert.IsAssignableFrom<Form>(dialog);
            Assert.Equal("Unhandled exception", dialog.Text);
            Assert.Contains("InvalidOperationException", dialog.ToString());
        }
        finally
        {
            dialog.Dispose();
        }
    }
}
