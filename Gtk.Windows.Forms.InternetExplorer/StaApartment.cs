using System;
using System.Collections.Concurrent;
using System.Threading;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// A dedicated single-threaded apartment with its own message pump, used to host the WebBrowser ActiveX
/// control when the application's GTK thread is not itself an STA.
/// </summary>
/// <remarks>
/// OLE in-place activation requires an STA, and the control's window has to be pumped on the thread that
/// created it. When the GTK main loop runs on an MTA thread, this gives the control a correct home: every
/// COM touch - instantiation, navigation, geometry, teardown - is marshalled here through
/// <see cref="Invoke(System.Action)" />, while the GTK widget is still read on the main thread and only the
/// resulting bounds crossed over. The common case (a normal <c>[STAThread]</c> WinForms host) does not use
/// this at all; the control then lives directly on the GTK thread.
///
/// The pump is a plain work queue drained on a wait handle rather than a Win32 message loop: the control's
/// own window messages are dispatched by the thread between work items because this is a real STA, which is
/// what OLE needs. This keeps the apartment free of a dependency on user32 message APIs.
/// </remarks>
internal sealed class StaApartment : IDisposable
{
    private readonly BlockingCollection<global::System.Action> _work =
        new BlockingCollection<global::System.Action>();

    private readonly Thread _thread;

    private volatile bool _disposed;

    internal StaApartment()
    {
        _thread = new Thread(Pump)
        {
            IsBackground = true,
            Name = "GtkWinForms IE STA"
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
    }

    /// <summary>Runs an action on the apartment thread and waits for it to finish, rethrowing its exception.</summary>
    internal void Invoke(global::System.Action action)
    {
        if (_disposed)
        {
            return;
        }

        if (Thread.CurrentThread == _thread)
        {
            action();
            return;
        }

        using (var done = new ManualResetEventSlim(false))
        {
            Exception? failure = null;
            _work.Add(() =>
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    failure = exception;
                }
                finally
                {
                    done.Set();
                }
            });

            done.Wait();
            if (failure != null)
            {
                throw new global::System.Reflection.TargetInvocationException(failure);
            }
        }
    }

    /// <summary>Runs a function on the apartment thread and returns its result.</summary>
    internal T Invoke<T>(global::System.Func<T> func)
    {
        var result = default(T)!;
        // A block-bodied lambda so this binds to Invoke(Action), not back to this generic overload.
        Invoke(() => { result = func(); });
        return result;
    }

    /// <summary>Queues an action on the apartment thread without waiting.</summary>
    internal void Post(global::System.Action action)
    {
        if (!_disposed)
        {
            _work.Add(action);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _work.CompleteAdding();
        if (_thread.IsAlive && Thread.CurrentThread != _thread)
        {
            _thread.Join(TimeSpan.FromSeconds(2));
        }

        _work.Dispose();
    }

    private void Pump()
    {
        try
        {
            foreach (var item in _work.GetConsumingEnumerable())
            {
                item();
            }
        }
        catch (InvalidOperationException)
        {
            // The collection was completed while blocked; the apartment is shutting down.
        }
    }
}
