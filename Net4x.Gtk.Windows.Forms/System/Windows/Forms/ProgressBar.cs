/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    [DefaultProperty("Value")]
	public class ProgressBar : Control
    {
        private ProgressBarBase self = new ProgressBarBase();
        public override object GtkControl => self;
        public ProgressBar():base()
        {
            self.Realized += Control_Realized;
        }

        private void Control_Realized(object sender, EventArgs e)
        {
            self.MaxValue = Maximum;
            self.MinValue = Minimum;
            self.Value = Value;
        }

        public ProgressBarStyle Style { get; set; }
        [DefaultValue(100)]
		public int MarqueeAnimationSpeed { get; set; } = 100;
        [DefaultValue(100)]
		public int Maximum { get; set; } = 100;
        [DefaultValue(0)]
		public int Minimum { get; set; } = 0;
        public new Padding Padding { get; set; }
        [DefaultValue(10)]
        public int Step { get; set; }
        [DefaultValue(0)]
        public int Value { get => (int)self.Value; set => self.Value = value; }
        public void Increment(int value)
        {

        }
 
        public void PerformStep()
		{
	
		}
	}
}