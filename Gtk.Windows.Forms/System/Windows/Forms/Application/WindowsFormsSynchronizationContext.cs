// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.Windows.Forms;

/// <summary>Provides a synchronization context for Windows Forms that marshals callbacks onto the GTK main
/// loop, so async continuations resume on the UI thread.</summary>
public sealed class WindowsFormsSynchronizationContext : SynchronizationContext, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.WindowsFormsSynchronizationContext" /> class.</summary>
    public WindowsFormsSynchronizationContext() { }

    /// <summary>Gets or sets a value indicating whether the context is automatically installed.</summary>
    public static bool AutoInstall { get; set; } = true;

    /// <summary>Dispatches a synchronous message: runs the callback on the GTK main loop and waits.</summary>
    /// <param name="d">The delegate to invoke.</param>
    /// <param name="state">The state passed to the delegate.</param>
    public override void Send(SendOrPostCallback d, object? state)
    {
        if (d == null) { return; }
        var done = new ManualResetEventSlim(false);
        Exception? error = null;
        Gtk.Application.Invoke((_, _) =>
        {
            try { d(state); } catch (Exception ex) { error = ex; } finally { done.Set(); }
        });
        done.Wait();
        if (error != null) { throw error; }
    }

    /// <summary>Dispatches an asynchronous message: posts the callback to the GTK main loop.</summary>
    /// <param name="d">The delegate to invoke.</param>
    /// <param name="state">The state passed to the delegate.</param>
    public override void Post(SendOrPostCallback d, object? state)
    {
        if (d == null) { return; }
        Gtk.Application.Invoke((_, _) => d(state));
    }

    /// <summary>Creates a copy of this synchronization context.</summary>
    /// <returns>A new <see cref="T:System.Windows.Forms.WindowsFormsSynchronizationContext" />.</returns>
    public override SynchronizationContext CreateCopy() => new WindowsFormsSynchronizationContext();

    /// <summary>Uninstalls the current Windows Forms synchronization context, restoring the default.</summary>
    public static void Uninstall() => SetSynchronizationContext(new SynchronizationContext());

    /// <summary>Releases the resources used by this context.</summary>
    public void Dispose() { }
}
