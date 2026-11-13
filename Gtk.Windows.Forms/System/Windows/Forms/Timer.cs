/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Implements a timer that raises an event at user-defined intervals. This timer is optimized for use in Windows Forms applications and must be used in a window.
/// </summary>
[DefaultEvent("Tick")]
[DefaultProperty("Interval")]
[ToolboxItemFilter("System.Windows.Forms")]
public class Timer : Component
{

    private protected EventHandler? _onTimer;

    private bool _enabled;

    private uint _interval = 100;

    private readonly object _syncObj = new();

    private uint _timerId;

    private bool isDisposed;

    /// <summary>
    /// Occurs when tick.
    /// </summary>
    public virtual event EventHandler? Tick
    {
        add => _onTimer += value;
        remove => _onTimer -= value;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Timer" /> class.
    /// </summary>
    public Timer() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Timer" /> class.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    public Timer(IContainer container) : this()
    {
        container?.Add(this);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.Timer" /> is enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if enabled; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool Enabled
    {
        get => _enabled;
        set
        {
            lock (_syncObj)
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (value)
                    {
                        _timerId = GLib.Timeout.Add(_interval, () =>
                        {
                            OnTick(EventArgs.Empty);

                            return true;
                        });
                    }
                    else
                    {
                        if (_timerId != 0)
                            GLib.Timeout.Remove(_timerId);
                        _timerId = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the interval.
    /// </summary>
    /// <value>
    /// The interval.
    /// </value>
    [DefaultValue(100)]
    public int Interval
    {
        get => (int)_interval;
        set
        {
            if (_interval != value)
            {
                _interval = (uint)value;
                if (_enabled)
                {
                    Enabled = false;
                    Enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    [Bindable(true)]
    [DefaultValue(null)]
    [Localizable(false)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        Enabled = true;
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        Enabled = false;
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"{base.ToString()}, Interval: {Interval}";

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.ComponentModel.Component">
    /// </see> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
        Enabled = false;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Raises the Tick event.
    /// </summary>
    protected virtual void OnTick(System.EventArgs e)
    {
        // Raise unconditionally: the OnXxx contract (and real .NET) is that invoking OnTick fires Tick.
        // Enabled-gating happens at the scheduling level - a stopped timer removes its GLib timeout, so
        // OnTick is not called during normal operation anyway. (Upstream added an `if (_enabled)` guard
        // that swallowed the event when OnTick is invoked directly.)
        _onTimer?.Invoke(this, e);
    }
}