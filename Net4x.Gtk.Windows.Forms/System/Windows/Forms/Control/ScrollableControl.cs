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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ScrollableControl" /> class.
        /// </summary>
        public ScrollableControl() : base()
        {
            scrollbase = GtkControl as IScrollableBoxBase;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.
        /// </summary>
        /// <returns>true if the container enables auto-scrolling; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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

        /// <summary>
        /// Gets or sets the size of the auto-scroll margin.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that represents the height and width of the auto-scroll margin in pixels.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The
        /// <see cref="System.Drawing.Size.Height" /> or
        /// <see cref="System.Drawing.Size.Width" /> value assigned is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public Size AutoScrollMargin { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the auto-scroll.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that determines the minimum size of the virtual area through which the user can scroll.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Size AutoScrollMinSize { get; set; }

        /// <summary>
        /// Gets or sets the location of the auto-scroll position.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> that represents the auto-scroll position in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        /// <summary>
        /// Gets the rectangle that represents the virtual display area of the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the display area of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override Rectangle DisplayRectangle { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the horizontal scroll bar is visible.
        /// </summary>
        /// <returns>true if the horizontal scroll bar is visible; otherwise, false.
        /// </returns>
        protected bool HScroll { get => scrollbase.HScroll; set => scrollbase.HScroll = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the vertical scroll bar is visible.
        /// </summary>
        /// <returns>true if the vertical scroll bar is visible; otherwise, false.
        /// </returns>
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
