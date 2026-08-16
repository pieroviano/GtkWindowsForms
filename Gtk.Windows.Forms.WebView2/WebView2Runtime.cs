using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// The WebView2 runtime this package needs, and the opt-in that lets an application install it.
/// </summary>
/// <remarks>
/// The opt-in lives here and not on <see cref="T:System.Windows.Forms.WebBrowser" /> deliberately: that
/// type is a WinForms compatibility surface, and a static that real WinForms does not have would be a
/// gratuitous divergence. Companion packages are where this library keeps its own additions.
/// </remarks>
public static class WebView2Runtime
{
    /// <summary>The Evergreen bootstrapper Microsoft publishes at a stable URL.</summary>
    private const string BootstrapperUrl = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

    private static bool _bootstrapAttempted;

    /// <summary>
    /// Gets or sets a value indicating whether this package may download and run Microsoft's Evergreen
    /// WebView2 bootstrapper when the runtime is missing.
    /// </summary>
    /// <value>
    /// <c>false</c> by default: nothing is downloaded unasked. Setting it to <c>true</c> is an
    /// application's statement that fetching an installer from Microsoft on its users' machines is
    /// acceptable; the library will not decide that on its behalf.
    /// </value>
    public static bool AllowRuntimeBootstrap
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether a WebView2 runtime is installed.
    /// </summary>
    /// <value>
    /// <c>true</c> when a browser version can be read.
    /// </value>
    public static bool IsRuntimeAvailable => InstalledVersion != null;

    /// <summary>
    /// Gets the installed runtime's version.
    /// </summary>
    /// <value>
    /// The version string, or <c>null</c> when no runtime is installed.
    /// </value>
    public static string? InstalledVersion
    {
        get
        {
            try
            {
                var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                return string.IsNullOrEmpty(version) ? null : version;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                return null;
            }
            catch (DllNotFoundException)
            {
                // WebView2Loader.dll is not next to the application. Same outcome as no runtime.
                return null;
            }
            catch (BadImageFormatException)
            {
                // A WebView2Loader.dll of the wrong architecture. Again: unusable, not fatal.
                return null;
            }
        }
    }

    /// <summary>
    /// Runs the Evergreen bootstrapper, if the application opted in and the runtime is missing.
    /// </summary>
    /// <returns>
    /// <c>true</c> when a runtime is available afterwards. <c>false</c> when the application did not opt
    /// in, when the download or the installer failed, or when the user declined the installer's prompt.
    /// </returns>
    /// <remarks>
    /// Attempted at most once per process: a bootstrapper that failed once will fail again, and retrying
    /// on every control that is constructed would download the same installer repeatedly.
    /// </remarks>
    public static async Task<bool> TryBootstrapAsync()
    {
        if (IsRuntimeAvailable)
        {
            return true;
        }

        if (!AllowRuntimeBootstrap || _bootstrapAttempted)
        {
            return false;
        }

        _bootstrapAttempted = true;

        var installer = Path.Combine(Path.GetTempPath(),
            "MicrosoftEdgeWebview2Setup-" + Guid.NewGuid().ToString("N") + ".exe");

        try
        {
            using (var client = new global::System.Net.Http.HttpClient())
            {
                var payload = await client.GetByteArrayAsync(BootstrapperUrl).ConfigureAwait(false);
#if NET462
                File.WriteAllBytes(installer, payload);
#else
                await using (var file = File.Create(installer))
                {
                    await file.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
                }
#endif
            }

            using (var process = Process.Start(new ProcessStartInfo(installer) { UseShellExecute = true }))
            {
                process?.WaitForExit();
            }

            return IsRuntimeAvailable;
        }
        catch (Exception exception) when (exception is global::System.Net.Http.HttpRequestException
                                              or IOException
                                              or UnauthorizedAccessException
                                              or global::System.ComponentModel.Win32Exception)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "The WebView2 Evergreen bootstrapper could not be downloaded or run.", exception);
            return false;
        }
        finally
        {
            try
            {
                if (File.Exists(installer))
                {
                    File.Delete(installer);
                }
            }
            catch (IOException)
            {
                // A temporary file the installer still holds open. Not worth failing over.
            }
        }
    }
}
