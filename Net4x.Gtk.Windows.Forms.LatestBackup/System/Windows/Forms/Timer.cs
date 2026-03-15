/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
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

    readonly System.Timers.Timer TimersTimer = new Timers.Timer();

    /// <summary>
    /// Occurs when [tick.
    /// </summary>
    public event EventHandler Tick;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Timer" /> class.
    /// </summary>
    public Timer()
    {
        TimersTimer.Elapsed += TimersTimer_Elapsed;
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
    public virtual bool Enabled { get => TimersTimer.Enabled; set => TimersTimer.Enabled = value; }

    /// <summary>
    /// Gets or sets the interval.
    /// </summary>
    /// <value>
    /// The interval.
    /// </value>
    [DefaultValue(100)]
    public int Interval { get => (int)TimersTimer.Interval; set => TimersTimer.Interval = value; }

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
    public object Tag { get; set; }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        TimersTimer.Start();
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        TimersTimer.Stop();
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() { return "System.Windows.Forms.Timer"; }

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
        TimersTimer.Dispose();
        base.Dispose();
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    protected new void Dispose()
    {
        Dispose(true);
    }

    private void TimersTimer_Elapsed(object sender, Timers.ElapsedEventArgs e)
    {
        if (Tick != null)
            Gtk.Application.Invoke(Tick);
    }

}