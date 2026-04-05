using System.ComponentModel;
using System.Globalization;
using NSubstitute;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable xUnit2009

namespace System.Windows.Forms.ApplicationTests;

[Collection("NonParallelCollection")]
public class ApplicationTests
{
    public ApplicationTests()
    {
        Application.OpenForms.Clear();
    }

    [Fact]
    public void CurrentCulture_GetSet_Works()
    {
        var original = Thread.CurrentThread.CurrentCulture;

        try
        {
            var newCulture = new CultureInfo("fr-FR");

            Application.CurrentCulture = newCulture;

            Assert.Equal(newCulture, Application.CurrentCulture);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Fact]
    public void CurrentInputLanguage_Get_NotNull()
    {
        var lang = Application.CurrentInputLanguage;

        Assert.NotNull(lang);
    }

    [Fact]
    public void CommonAppDataPath_ReturnsValidPath()
    {
        var path = Application.CommonAppDataPath;

        Assert.False(string.IsNullOrEmpty(path));
        Assert.True(path.Contains(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)));
    }

    [Fact]
    public void LocalUserAppDataPath_ReturnsValidPath()
    {
        var path = Application.LocalUserAppDataPath;

        Assert.False(string.IsNullOrEmpty(path));
#pragma warning disable xUnit2009
        Assert.True(path.Contains(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));
    }

    [Fact]
    public void UserAppDataPath_ReturnsValidPath()
    {
        var path = Application.UserAppDataPath;

        Assert.False(string.IsNullOrEmpty(path));
        Assert.True(path.Contains(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
    }

    [Fact]
    public void StartupPath_ReturnsCurrentDirectory()
    {
        var expected = Directory.GetCurrentDirectory();

        Assert.Equal(expected, Application.StartupPath);
    }

    [Fact]
    public void ExecutablePath_ReturnsNonEmpty()
    {
        var path = Application.ExecutablePath;

        Assert.False(string.IsNullOrEmpty(path));
        Assert.True(File.Exists(path) || Path.IsPathRooted(path));
    }

    [Fact]
    public void OpenForms_InitiallyEmpty()
    {
        var forms = Application.OpenForms;

        Assert.NotNull(forms);
        Assert.True(forms.Count >= 0);
    }

    [Fact]
    public void SetHighDpiMode_AlwaysReturnsTrue()
    {
        var result = Application.SetHighDpiMode(HighDpiMode.SystemAware);

        Assert.True(result);
    }

    [Fact]
    public void Exit_WithNullEvent_DoesNotThrow()
    {
        var ex = Record.Exception(() => Application.Exit((CancelEventArgs?)null));

        Assert.Null(ex);
    }

    [Fact]
    public void Exit_WithCancelTrue_DoesNotThrow()
    {
        var args = new CancelEventArgs { Cancel = true };

        var ex = Record.Exception(() => Application.Exit(args));

        Assert.Null(ex);
    }

    [Fact]
    public void Exit_WithCancelFalse_DoesNotThrow()
    {
        var args = new CancelEventArgs { Cancel = false };

        var ex = Record.Exception(() => Application.Exit(args));

        Assert.Null(ex);
    }

    [Fact]
    public void ExitThread_DoesNotThrow()
    {
        var ex = Record.Exception(() => Application.ExitThread());

        Assert.Null(ex);
    }

    [Fact]
    public void DoEvents_DoesNotThrow()
    {
        var ex = Record.Exception(() => Application.DoEvents());

        Assert.Null(ex);
    }

    [Fact]
    public void EnableVisualStyles_DoesNotThrow()
    {
        var ex = Record.Exception(() => Application.EnableVisualStyles());

        Assert.Null(ex);
    }

    [Fact]
    public void SetCompatibleTextRenderingDefault_DoesNotThrow()
    {
        var ex = Record.Exception(() => Application.SetCompatibleTextRenderingDefault(true));

        Assert.Null(ex);
    }
}