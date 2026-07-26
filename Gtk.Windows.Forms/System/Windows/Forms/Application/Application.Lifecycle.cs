// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
/// The residual lifecycle surface of <c>System.Windows.Forms.Application</c>: the idle notification,
/// restart, the product/company metadata and the visual-style state.
/// </summary>
public sealed partial class Application
{
    /// <summary>How often idleness is sampled, in milliseconds.</summary>
    private const uint IdlePollInterval = 50;

    private static EventHandler? _idle;

    /// <summary>Whether the idle poll source is installed on the GLib main context.</summary>
    private static bool _idlePolling;

    /// <summary>Whether <see cref="E:System.Windows.Forms.Application.Idle" /> has already been raised for the current lull.</summary>
    private static bool _idleRaised;

    private static bool _useVisualStyles;

    private static VisualStyleState _visualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;

    /// <summary>
    /// Occurs when the application finishes processing and is about to enter the idle state.
    /// </summary>
    /// <remarks>
    /// GTK has no "the queue just drained" notification, so idleness is sampled from the GLib main
    /// context: the event is raised once each time the event queue is found empty and is re-armed as soon
    /// as events appear again - the same once-per-lull contract WinForms offers. The sampling source only
    /// exists while something is subscribed, so an application that never uses <c>Idle</c> pays nothing.
    /// </remarks>
    public static event EventHandler? Idle
    {
        add
        {
            lock (internalSyncObject)
            {
                _idle = (EventHandler?)Delegate.Combine(_idle, value);
                StartIdlePolling();
            }
        }
        remove
        {
            lock (internalSyncObject)
            {
                _idle = (EventHandler?)Delegate.Remove(_idle, value);
            }
        }
    }

    /// <summary>
    /// Installs the idle sampling source, unless it is already running.
    /// </summary>
    private static void StartIdlePolling()
    {
        if (_idlePolling || _idle == null)
        {
            return;
        }

        _idlePolling = true;
        _idleRaised = false;
        GLib.Timeout.Add(IdlePollInterval, PollIdle);
    }

    /// <summary>
    /// One sample: work still queued re-arms the notification, a drained queue raises it once.
    /// </summary>
    /// <returns><see langword="false" /> once nobody is listening any more, which removes the source.</returns>
    private static bool PollIdle()
    {
        if (_idle == null)
        {
            _idlePolling = false;

            return false;
        }

        if (Gtk.Application.EventsPending() || GLib.MainContext.Default.HasPendingEvents)
        {
            _idleRaised = false;
        }
        else if (!_idleRaised)
        {
            _idleRaised = true;
            OnIdle(EventArgs.Empty);
        }

        return true;
    }

    /// <summary>
    /// Raises <see cref="E:System.Windows.Forms.Application.Idle" />, applying the configured
    /// unhandled-exception policy to anything a handler throws - an exception escaping here would
    /// otherwise unwind through GLib's callback marshalling.
    /// </summary>
    /// <param name="e">The event data.</param>
    private static void OnIdle(EventArgs e)
    {
        try
        {
            _idle?.Invoke(null, e);
        }
        catch (Exception ex)
        {
            OnThreadException(ex);
        }
    }

    /// <summary>
    /// Gets the company name associated with the application.
    /// </summary>
    /// <remarks>
    /// Read from the entry assembly's <see cref="T:System.Reflection.AssemblyCompanyAttribute" />, falling
    /// back to the assembly's own name - the same order WinForms uses.
    /// </remarks>
    public static string CompanyName =>
        EntryAssemblyMetadata(a => a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company);

    /// <summary>
    /// Gets the product name associated with the application.
    /// </summary>
    /// <remarks>
    /// Read from the entry assembly's <see cref="T:System.Reflection.AssemblyProductAttribute" />, falling
    /// back to the assembly's own name.
    /// </remarks>
    public static string ProductName =>
        EntryAssemblyMetadata(a => a.GetCustomAttribute<AssemblyProductAttribute>()?.Product);

    /// <summary>
    /// Gets the product version associated with the application.
    /// </summary>
    /// <remarks>
    /// <see cref="T:System.Reflection.AssemblyInformationalVersionAttribute" /> wins, as in WinForms, then
    /// <see cref="T:System.Reflection.AssemblyFileVersionAttribute" />, then the assembly version.
    /// </remarks>
    public static string ProductVersion =>
        EntryAssemblyMetadata(a =>
            a.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? a.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? a.GetName().Version?.ToString());

    /// <summary>
    /// Reads a piece of entry-assembly metadata, falling back to the assembly's simple name and finally to
    /// the empty string, so these properties never return <see langword="null" />.
    /// </summary>
    /// <param name="read">Reads the attribute value from the entry assembly.</param>
    private static string EntryAssemblyMetadata(Func<Assembly, string?> read)
    {
        var entry = Assembly.GetEntryAssembly();
        if (entry == null)
        {
            return string.Empty;
        }

        var value = read(entry);

        return string.IsNullOrWhiteSpace(value) ? entry.GetName().Name ?? string.Empty : value!;
    }

    /// <summary>
    /// Gets a value indicating whether the application is drawing with visual styles.
    /// </summary>
    /// <remarks>
    /// True once <see cref="M:System.Windows.Forms.Application.EnableVisualStyles" /> has been called,
    /// provided <see cref="P:System.Windows.Forms.Application.VisualStyleState" /> still enables the client
    /// area and the theme is available.
    /// </remarks>
    public static bool RenderWithVisualStyles =>
        _useVisualStyles
        && VisualStyleInformation.IsEnabledByUser
        && VisualStyleInformation.IsSupportedByOS
        && (VisualStyleState == VisualStyleState.ClientAreaEnabled
            || VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled);

    /// <summary>
    /// Gets or sets which areas of the application windows are drawn with visual styles.
    /// </summary>
    /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
    /// The value is not one of the <see cref="T:System.Windows.Forms.VisualStyles.VisualStyleState" /> members.
    /// </exception>
    public static VisualStyleState VisualStyleState
    {
        get => _visualStyleState;
        set
        {
            if (value < VisualStyleState.NoneEnabled || value > VisualStyleState.ClientAndNonClientAreasEnabled)
            {
                throw new global::System.ComponentModel.InvalidEnumArgumentException(
                    nameof(value), (int)value, typeof(VisualStyleState));
            }

            _visualStyleState = value;
        }
    }

    /// <summary>
    /// Shuts the application down and starts a new instance of it immediately.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// The application has no entry assembly - it is hosted rather than launched - so there is nothing to
    /// restart.
    /// </exception>
    /// <remarks>
    /// As in WinForms, the message loop is ended first and the replacement process is started afterwards,
    /// with the same command line. A framework-dependent .NET application is relaunched through its host,
    /// since its entry point is a library.
    /// </remarks>
    public static void Restart()
    {
        if (Assembly.GetEntryAssembly() == null)
        {
            throw new NotSupportedException("Application.Restart is only supported for executable applications.");
        }

        var startInfo = BuildRestartStartInfo();

        Exit();

        try
        {
            Diagnostics.Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception restarting the application", ex);

            throw;
        }
    }

    /// <summary>
    /// Works out how to launch this application again: the executable to run and the arguments to repeat.
    /// </summary>
    private static Diagnostics.ProcessStartInfo BuildRestartStartInfo()
    {
        var commandLine = Environment.GetCommandLineArgs();
        var arguments = new List<string>();
        var fileName = ExecutablePath;

        if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            // A framework-dependent application's entry assembly is a library: it has to go back through
            // the host that is running this process.
            var host = Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            arguments.Add(fileName);
            fileName = string.IsNullOrWhiteSpace(host) ? "dotnet" : host!;
        }

        for (var i = 1; i < commandLine.Length; i++)
        {
            arguments.Add(commandLine[i]);
        }

        var startInfo = new Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            Arguments = string.Join(" ", arguments.Select(Quote)),
            UseShellExecute = false,
            WorkingDirectory = StartupPath
        };

        return startInfo;
    }

    /// <summary>
    /// Quotes an argument that contains whitespace so the relaunched process receives it whole.
    /// </summary>
    /// <param name="argument">The argument to quote.</param>
    private static string Quote(string argument) =>
        argument.IndexOf(' ') < 0 || argument.StartsWith("\"", StringComparison.Ordinal)
            ? argument
            : "\"" + argument.Replace("\"", "\\\"") + "\"";
}
