/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements a timer that raises an event at user-defined intervals. This timer is optimized for use in Windows Forms applications and must be used in a window.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DefaultEvent("Tick")]
    [DefaultProperty("Interval")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public class Timer : Component
    {

        readonly System.Timers.Timer TimersTimer = new Timers.Timer();

        public event EventHandler Tick;

        public Timer()
        {
            TimersTimer.Elapsed += TimersTimer_Elapsed;
        }

        public Timer(IContainer container) : this()
        {
        }

        [DefaultValue(false)]

        public virtual bool Enabled { get => TimersTimer.Enabled; set => TimersTimer.Enabled = value; }

        [DefaultValue(100)]
        public int Interval { get => (int)TimersTimer.Interval; set => TimersTimer.Interval = value; }

        [Bindable(true)]
        [DefaultValue(null)]
        [Localizable(false)]
        [TypeConverter(typeof(StringConverter))]

        public object Tag { get; set; }

        public void Start()
        {
            TimersTimer.Start();
        }

        public void Stop()
        {
            TimersTimer.Stop();
        }

        public override string ToString() { return "System.Windows.Forms.Timer"; }

        protected override void Dispose(bool disposing)
        {
            TimersTimer.Dispose();
            base.Dispose();
        }

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
}
