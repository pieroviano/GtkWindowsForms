// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;

namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// Chooses the web engine behind
/// <see cref="T:System.Windows.Forms.WebBrowser" />.
/// </summary>
/// <remarks>
/// Backends register themselves rather than being named here, so that an engine which cannot be
/// referenced from a
/// <c>
/// netstandard2.0
/// </c> library - WebView2, which ships Windows-only assemblies - can live in a companion package and
/// still be picked first on the platform it belongs to. Every probe is guarded: a backend whose native
/// library is missing must make the factory fall through to the next candidate, never fault the control
/// being constructed.
/// </remarks>
public static class WebViewBackendFactory
{
    private static readonly List<Registration> Registrations = new List<Registration>();

    private static readonly object SyncRoot = new object();

    /// <summary>
    /// Gets the URI a navigation falls back to when the caller names none - the same
    /// <c>
    /// about:blank
    /// </c> real WinForms uses.
    /// </summary>
    /// <value>
    /// The blank URI.
    /// </value>
    public static Uri BlankUri
    {
        get;
    } = new Uri("about:blank");

    /// <summary>
    /// Creates the highest-priority backend whose probe succeeds.
    /// </summary>
    /// <returns>
    /// A backend. Never
    /// <c>
    /// null
    /// </c>: when no engine is available the result is a
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.NullWebViewBackend" />.
    /// </returns>
    public static IWebViewBackend Create()
    {
        Registration[] candidates;
        lock (SyncRoot)
        {
            candidates = Registrations.OrderByDescending(registration => registration.Priority)
                .ThenBy(registration => registration.Name, StringComparer.Ordinal)
                .ToArray();
        }

        foreach (var candidate in candidates)
        {
            IWebViewBackend? backend;
            try
            {
                backend = candidate.Create();
            }
            catch (Exception exception)
            {
                // A probe that throws means "this engine is not usable here" - a missing native library,
                // a runtime that is not installed, a platform mismatch. That is exactly the case the
                // fallback chain exists for, so it must not escape into the control's constructor.
                GtkLogger.Debug($"Web view backend '{candidate.Name}' is unavailable.", exception);
                continue;
            }

            if (backend != null)
            {
                return backend;
            }

            GtkLogger.Debug($"Web view backend '{candidate.Name}' declined.");
        }

        return new NullWebViewBackend();
    }

    /// <summary>
    /// Determines whether any registered backend is currently able to produce an engine.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> when a real engine would be chosen.
    /// </returns>
    public static bool IsAnyEngineAvailable()
    {
        using (var backend = Create())
        {
            return backend.IsFunctional;
        }
    }

    /// <summary>
    /// Registers a backend, replacing any registration already held under the same name.
    /// </summary>
    /// <param name="name">
    /// A stable name for the backend, used for logging and for replacing a registration.
    /// </param>
    /// <param name="priority">
    /// Higher is tried first. The Windows backend registers above the WebKitGTK one so that it wins where
    /// both could load.
    /// </param>
    /// <param name="create">
    /// Produces the backend, or returns
    /// <c>
    /// null
    /// </c> when it is not usable on this machine.
    /// </param>
    public static void Register(string name, int priority, Func<IWebViewBackend?> create)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (create == null)
        {
            throw new ArgumentNullException(nameof(create));
        }

        lock (SyncRoot)
        {
            Registrations.RemoveAll(registration => string.Equals(registration.Name, name, StringComparison.Ordinal));
            Registrations.Add(new Registration(name, priority, create));
        }
    }

    /// <summary>
    /// Removes a registration.
    /// </summary>
    /// <param name="name">
    /// The name the backend registered under.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when a registration was removed.
    /// </returns>
    public static bool Unregister(string name)
    {
        lock (SyncRoot)
        {
            return Registrations.RemoveAll(registration =>
                string.Equals(registration.Name, name, StringComparison.Ordinal)) > 0;
        }
    }

    /// <summary>
    /// Drops every registration. Exists for tests, which must be able to establish a known set of
    /// candidates without depending on what the running machine happens to have installed.
    /// </summary>
    internal static void Clear()
    {
        lock (SyncRoot)
        {
            Registrations.Clear();
        }
    }

    /// <summary>
    /// Lists the registered backend names, highest priority first.
    /// </summary>
    /// <returns>
    /// The names.
    /// </returns>
    internal static IReadOnlyList<string> RegisteredNames()
    {
        lock (SyncRoot)
        {
            return Registrations.OrderByDescending(registration => registration.Priority)
                .ThenBy(registration => registration.Name, StringComparer.Ordinal)
                .Select(registration => registration.Name)
                .ToArray();
        }
    }

    private sealed class Registration
    {
        internal Registration(string name, int priority, Func<IWebViewBackend?> create)
        {
            Name = name;
            Priority = priority;
            Create = create;
        }

        internal Func<IWebViewBackend?> Create
        {
            get;
        }

        internal string Name
        {
            get;
        }

        internal int Priority
        {
            get;
        }
    }
}
