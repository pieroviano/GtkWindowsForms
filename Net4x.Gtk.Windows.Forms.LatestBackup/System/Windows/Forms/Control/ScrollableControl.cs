using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// Defines a base class for controls that support auto-scrolling behavior.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [Designer("System.Windows.Forms.Design.ScrollableControlDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class ScrollableControl : Control, IArrangedElement, IComponent, IDisposable
    {

        protected const int ScrollStateAutoScrolling = 1;

        protected const int ScrollStateFullDrag = 16;

        protected const int ScrollStateHScrollVisible = 2;

        protected const int ScrollStateUserHasScrolled = 8;

        protected const int ScrollStateVScrollVisible = 4;

        private bool _AutoScroll;

        private IScrollableBoxBase scrollbase;

        public virtual event ScrollEventHandler Scroll
        {
            add { if (scrollbase != null) { scrollbase.Scroll += value; } }
            remove { if (scrollbase != null) { scrollbase.Scroll -= value; } }
        }
        public ScrollableControl() : base()
        {
            scrollbase = GtkControl as IScrollableBoxBase;
        }
        public virtual bool AutoScroll
        {
            get => _AutoScroll;
            set
            {
                _AutoScroll = value;
                if (AutoSize == false)
                    if (scrollbase != null) { scrollbase.AutoScroll = value; }
            }
        }

        public Size AutoScrollMargin { get; set; }

        public Size AutoScrollMinSize { get; set; }

        public Point AutoScrollPosition { get; set; }

        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                if (value == true)
                {
                    if (scrollbase != null) { scrollbase.AutoScroll = false; }
                }
                else
                {
                    if (scrollbase != null) { scrollbase.AutoScroll = _AutoScroll; }
                }
            }
        }

        public override Rectangle DisplayRectangle { get; }
        protected bool HScroll { get => scrollbase.HScroll; set => scrollbase.HScroll = value; }

        //public VScrollProperties VerticalScroll { get; }
        //public DockPaddingEdges DockPadding { get; }
        //public HScrollProperties HorizontalScroll { get; }

        protected bool VScroll { get => scrollbase.VScroll; set => scrollbase.VScroll = value; }
        public void SetScrolledWindow(IScrollableBoxBase scrolledwindow)
        {
            scrollbase = scrolledwindow;
        }
    }
}
