using System;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Routes every COM touch onto the thread the control lives on.
/// </summary>
/// <remarks>
/// With no apartment - the normal case, where the GTK main loop already runs on an STA thread - this is a
/// straight pass-through, so the control lives directly on the GTK thread and there is no marshalling at
/// all. With an apartment, it is the MTA fallback: the work crosses to the dedicated STA thread. Code that
/// touches the control calls <see cref="Send(System.Action)" /> rather than caring which case it is in.
/// </remarks>
internal sealed class ComDispatcher : IDisposable
{
    private readonly StaApartment? _apartment;

    internal ComDispatcher(StaApartment? apartment)
    {
        _apartment = apartment;
    }

    /// <summary>Gets a value indicating whether calls are marshalled to a separate apartment thread.</summary>
    internal bool Marshals => _apartment != null;

    /// <summary>Runs an action on the control's thread and waits for it.</summary>
    internal void Send(global::System.Action action)
    {
        if (_apartment != null)
        {
            _apartment.Invoke(action);
        }
        else
        {
            action();
        }
    }

    /// <summary>Runs a function on the control's thread and returns its result.</summary>
    internal T Send<T>(global::System.Func<T> func) => _apartment != null ? _apartment.Invoke(func) : func();

    /// <inheritdoc />
    public void Dispose() => _apartment?.Dispose();
}
