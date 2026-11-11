using System;
using Microsoft.Win32;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Opts the hosted WebBrowser ActiveX control into a modern rendering mode.
/// </summary>
/// <remarks>
/// Left alone, an embedded WebBrowser control renders as Internet Explorer 7 in quirks mode, which breaks
/// most pages written this decade. Writing the host executable's name under
/// <c>FEATURE_BROWSER_EMULATION</c> switches it to IE11 edge mode. The value is written under
/// <c>HKEY_CURRENT_USER</c>, so no administrator rights are needed, and it affects only this user's run of
/// this executable.
/// </remarks>
internal static class BrowserEmulation
{
    /// <summary>IE11 forced standards (edge) mode - the value an embedded control reads for "render like IE11".</summary>
    internal const int Ie11EdgeMode = 11001;

    private const string FeatureKey =
        @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

    private static bool _applied;

    /// <summary>
    /// Writes the emulation value for the current host executable, once per process.
    /// </summary>
    /// <remarks>
    /// Best effort: a locked-down profile may deny the write, in which case the control simply renders in
    /// its default mode rather than faulting. The write is skipped when the value is already correct, so a
    /// repeated run does not churn the registry.
    /// </remarks>
    internal static void EnsureApplied()
    {
        if (_applied)
        {
            return;
        }

        _applied = true;
        TrySet(HostExecutableName(), Ie11EdgeMode);
    }

    /// <summary>
    /// Writes the emulation value for a named executable under <c>HKEY_CURRENT_USER</c>. Separated from
    /// <see cref="EnsureApplied" /> and named rather than tied to the host process so tests can write and read
    /// back a throwaway executable name. <c>RegistryKey</c> is deliberately not named in the signature: the
    /// base library ships its own <c>Microsoft.Win32.RegistryKey</c> shim, which would make the type
    /// ambiguous here.
    /// </summary>
    /// <param name="executableName">The host executable's file name, e.g. <c>app.exe</c>.</param>
    /// <param name="value">The emulation value to write.</param>
    /// <returns><c>true</c> when the value was written or already present.</returns>
    internal static bool Write(string executableName, int value)
    {
        if (string.IsNullOrEmpty(executableName))
        {
            return false;
        }

        using (var key = Registry.CurrentUser.CreateSubKey(FeatureKey))
        {
            if (key == null)
            {
                return false;
            }

            if (key.GetValue(executableName) is int existing && existing == value)
            {
                return true;
            }

            key.SetValue(executableName, value, RegistryValueKind.DWord);
            return true;
        }
    }

    /// <summary>The registry sub-path under which emulation values live, for tests that read values back.</summary>
    internal static string FeatureSubKeyPath => FeatureKey;

    private static string HostExecutableName()
    {
        try
        {
            var module = global::System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName;
            return string.IsNullOrEmpty(module) ? "dotnet.exe" : module!;
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            return "dotnet.exe";
        }
    }

    private static void TrySet(string executableName, int value)
    {
        try
        {
            Write(executableName, value);
        }
        catch (Exception exception) when (
            exception is System.Security.SecurityException or UnauthorizedAccessException or
                global::System.IO.IOException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "Could not set FEATURE_BROWSER_EMULATION; the WebBrowser control will render in its default mode.",
                exception);
        }
    }
}
