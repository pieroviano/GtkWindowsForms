using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    public interface IControl
    {

        event EventHandler AutoSizeChanged;

        event EventHandler BackColorChanged;

        event EventHandler BackgroundImageChanged;

        event EventHandler BackgroundImageLayoutChanged;

        event EventHandler BindingContextChanged;

        event EventHandler CausesValidationChanged;

        event UICuesEventHandler ChangeUICues;

        event EventHandler Click;

        event EventHandler ClientSizeChanged;

        event EventHandler ContextMenuStripChanged;

        event ControlEventHandler ControlAdded;

        event ControlEventHandler ControlRemoved;

        event EventHandler CursorChanged;

        event EventHandler DockChanged;

        event EventHandler DoubleClick;

        event EventHandler DpiChangedAfterParent;

        event EventHandler DpiChangedBeforeParent;

        event DragEventHandler DragDrop;

        event DragEventHandler DragEnter;

        event EventHandler DragLeave;

        event DragEventHandler DragOver;

        event EventHandler EnabledChanged;

        event EventHandler Enter;

        event EventHandler FontChanged;

        event EventHandler ForeColorChanged;

        event GiveFeedbackEventHandler GiveFeedback;

        event EventHandler GotFocus;

        event EventHandler HandleCreated;

        event EventHandler HandleDestroyed;

        event HelpEventHandler HelpRequested;

        event EventHandler ImeModeChanged;

        event InvalidateEventHandler Invalidated;

        event KeyEventHandler KeyDown;

        event KeyPressEventHandler KeyPress;

        event KeyEventHandler KeyUp;

        event LayoutEventHandler Layout;

        event EventHandler Leave;

        event EventHandler LocationChanged;

        event EventHandler LostFocus;

        event EventHandler MarginChanged;

        event EventHandler MouseCaptureChanged;

        event MouseEventHandler MouseClick;

        event MouseEventHandler MouseDoubleClick;

        event MouseEventHandler MouseDown;

        event EventHandler MouseEnter;

        event EventHandler MouseHover;

        event EventHandler MouseLeave;

        event MouseEventHandler MouseMove;

        event MouseEventHandler MouseUp;

        event MouseEventHandler MouseWheel;

        event EventHandler Move;

        event EventHandler PaddingChanged;

        event PaintEventHandler Paint;

        event EventHandler ParentChanged;

        event PreviewKeyDownEventHandler PreviewKeyDown;

        event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

        event QueryContinueDragEventHandler QueryContinueDrag;

        event EventHandler RegionChanged;

        event EventHandler Resize;

        event EventHandler RightToLeftChanged;

        event EventHandler SizeChanged;

        event EventHandler StyleChanged;

        event EventHandler SystemColorsChanged;

        event EventHandler TabIndexChanged;

        event EventHandler TabStopChanged;

        event EventHandler TextChanged;

        event EventHandler Validated;

        event CancelEventHandler Validating;

        event EventHandler VisibleChanged;
        /// <summary>
        /// Gets the accessibility object.
        /// </summary>
        /// <value>
        /// The accessibility object.
        /// </value>
        AccessibleObject AccessibilityObject { get; }

        /// <summary>
        /// Gets or sets the accessible default action description.
        /// </summary>
        /// <value>
        /// The accessible default action description.
        /// </value>
        string AccessibleDefaultActionDescription { get; set; }

        /// <summary>
        /// Gets or sets the accessible description.
        /// </summary>
        /// <value>
        /// The accessible description.
        /// </value>
        string AccessibleDescription { get; set; }

        /// <summary>
        /// Gets or sets the name of the accessible.
        /// </summary>
        /// <value>
        /// The name of the accessible.
        /// </value>
        string AccessibleName { get; set; }

        /// <summary>
        /// Gets or sets the accessible role.
        /// </summary>
        /// <value>
        /// The accessible role.
        /// </value>
        AccessibleRole AccessibleRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allows drop.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if allows drop; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool AllowDrop { get; set; }

        /// <summary>
        /// Gets or sets the anchor.
        /// </summary>
        /// <value>
        /// The anchor.
        /// </value>
        AnchorStyles Anchor { get; set; }

        /// <summary>
        /// Gets or sets the automatic scroll offset.
        /// </summary>
        /// <value>
        /// The automatic scroll offset.
        /// </value>
        Point AutoScrollOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic size.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if automatic size; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool AutoSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the background image layout.
        /// </summary>
        /// <value>
        /// The background image layout.
        /// </value>
        ImageLayout BackgroundImageLayout { get; set; }

        /// <summary>
        /// Gets or sets the binding context.
        /// </summary>
        /// <value>
        /// The binding context.
        /// </value>
        BindingContext BindingContext { get; set; }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        int Bottom { get; }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance can focus.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance can focus; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool CanFocus { get; }

        /// <summary>
        /// Gets a value indicating whether this instance can select.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance can select; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool CanSelect { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is capture.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if capture; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Capture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether causes validation.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if causes validation; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool CausesValidation { get; set; }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>
        /// The name of the company.
        /// </value>
        string CompanyName { get; }

        /// <summary>
        /// Gets a value indicating whether contains focus.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if contains focus; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool ContainsFocus { get; }

        /// <summary>
        /// Gets or sets the context menu strip.
        /// </summary>
        /// <value>
        /// The context menu strip.
        /// </value>
        ContextMenuStrip ContextMenuStrip { get; set; }

        /// <summary>
        /// Gets the controls.
        /// </summary>
        /// <value>
        /// The controls.
        /// </value>
        Control.ControlCollection Controls { get; }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is created.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if created; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Created { get; }

        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>
        /// The cursor.
        /// </value>
        Cursor Cursor { get; set; }

        /// <summary>
        /// Gets the data bindings.
        /// </summary>
        /// <value>
        /// The data bindings.
        /// </value>
        ControlBindingsCollection DataBindings { get; }

        /// <summary>
        /// Gets the device dpi.
        /// </summary>
        /// <value>
        /// The device dpi.
        /// </value>
        int DeviceDpi { get; }

        /// <summary>
        /// Gets the display rectangle.
        /// </summary>
        /// <value>
        /// The display rectangle.
        /// </value>
        Rectangle DisplayRectangle { get; }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is disposing.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if disposing; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Disposing { get; }

        /// <summary>
        /// Gets or sets the dock.
        /// </summary>
        /// <value>
        /// The dock.
        /// </value>
        DockStyle Dock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is enabled.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if enabled; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is focused.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if focused; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Focused { get; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        Font Font { get; set; }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>
        /// The color of the fore.
        /// </value>
        Color ForeColor { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance has children; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        // IntPtr Handle { get; }
        bool HasChildren { get; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets the IME mode.
        /// </summary>
        /// <value>
        /// The IME mode.
        /// </value>
        ImeMode ImeMode { get; set; }

        /// <summary>
        /// Gets a value indicating whether invoke required.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if invoke required; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool InvokeRequired { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is accessible.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is accessible; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool IsAccessible { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is disposed; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is handle created.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is handle created; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool IsHandleCreated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is mirrored.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is mirrored; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool IsMirrored { get; }

        /// <summary>
        /// Gets the layout engine.
        /// </summary>
        /// <value>
        /// The layout engine.
        /// </value>
        LayoutEngine LayoutEngine { get; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        int Left { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        Point Location { get; set; }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        Padding Margin { get; set; }

        /// <summary>
        /// Gets or sets the maximum size.
        /// </summary>
        /// <value>
        /// The maximum size.
        /// </value>
        Size MaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum size.
        /// </summary>
        /// <value>
        /// The minimum size.
        /// </value>
        Size MinimumSize { get; set; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        //string Name { get; set; }
        Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        Control Parent { get; set; }

        /// <summary>
        /// Gets the size of the preferred.
        /// </summary>
        /// <value>
        /// The size of the preferred.
        /// </value>
        Size PreferredSize { get; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        string ProductName { get; }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        string ProductVersion { get; }

        /// <summary>
        /// Gets a value indicating whether recreating handle.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if recreating handle; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool RecreatingHandle { get; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        Region Region { get; set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        int Right { get; }

        /// <summary>
        /// Gets or sets the right to left.
        /// </summary>
        /// <value>
        /// The right to left.
        /// </value>
        RightToLeft RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        /// <value>
        /// The site.
        /// </value>
        ISite Site { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        Size Size { get; set; }

        /// <summary>
        /// Gets or sets the index of the tab.
        /// </summary>
        /// <value>
        /// The index of the tab.
        /// </value>
        int TabIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tab stop.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if tab stop; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool TabStop { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        object Tag { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        int Top { get; set; }

        /// <summary>
        /// Gets the top level control.
        /// </summary>
        /// <value>
        /// The top level control.
        /// </value>
        Control TopLevelControl { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [use visual style back color].
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if [use visual style back color]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool UseVisualStyleBackColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use wait cursor.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if use wait cursor; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool UseWaitCursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="System.Windows.Forms.IControl" /> is visible.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if visible; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool Visible { get; set; }

        /// <summary>
        /// widget unregister the contained methods
        /// </summary>
        //widget unregister the contained methods
        Gtk.Widget Widget { get; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the window target.
        /// </summary>
        /// <value>
        /// The window target.
        /// </value>
        IWindowTarget WindowTarget { get; set; }
        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.
        /// </param>
        /// <returns>
        /// </returns>
        IAsyncResult BeginInvoke(Delegate method);

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.
        /// </param>
        /// <param name="args">The arguments.
        /// </param>
        /// <returns>
        /// </returns>
        IAsyncResult BeginInvoke(Delegate method, params object[] args);

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.
        /// </param>
        /// <returns>
        /// </returns>
        IAsyncResult BeginInvoke(Action method);

        /// <summary>
        /// Brings to front.
        /// </summary>
        void BringToFront();

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="ctl">The control.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if [ontains the specified control; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        bool Contains(Control ctl);

        /// <summary>
        /// Creates the control.
        /// </summary>
        void CreateControl();

        /// <summary>
        /// Creates the graphics.
        /// </summary>
        /// <returns>
        /// </returns>
        Graphics CreateGraphics();

        /// <summary>
        /// Does the drag drop.
        /// </summary>
        /// <param name="data">The data.
        /// </param>
        /// <param name="allowedEffects">The allowed effects.
        /// </param>
        /// <returns>
        /// </returns>
        DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);

        /// <summary>
        /// Draws to bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.
        /// </param>
        /// <param name="targetBounds">The target bounds.
        /// </param>
        void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);

        /// <summary>
        /// Ends the invoke.
        /// </summary>
        /// <param name="asyncResult">The asynchronous result.
        /// </param>
        /// <returns>
        /// </returns>
        object EndInvoke(IAsyncResult asyncResult);

        /// <summary>
        /// Finds the form.
        /// </summary>
        /// <returns>
        /// </returns>
        Form FindForm();

        /// <summary>
        /// Focuses this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        bool Focus();

        /// <summary>
        /// Gets the child at point.
        /// </summary>
        /// <param name="pt">The pt.
        /// </param>
        /// <returns>
        /// </returns>
        Control GetChildAtPoint(Point pt);

        /// <summary>
        /// Gets the child at point.
        /// </summary>
        /// <param name="pt">The pt.
        /// </param>
        /// <param name="skipValue">The skip value.
        /// </param>
        /// <returns>
        /// </returns>
        Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue);

        /// <summary>
        /// Gets the container control.
        /// </summary>
        /// <returns>
        /// </returns>
        IContainerControl GetContainerControl();

        /// <summary>
        /// Gets the next control.
        /// </summary>
        /// <param name="ctl">The control.
        /// </param>
        /// <param name="forward">if set to
        /// <c>
        /// true
        /// </c> [forward].
        /// </param>
        /// <returns>
        /// </returns>
        Control GetNextControl(Control ctl, bool forward);

        /// <summary>
        /// Gets the size of the preferred.
        /// </summary>
        /// <param name="proposedSize">Size of the proposed.
        /// </param>
        /// <returns>
        /// </returns>
        Size GetPreferredSize(Size proposedSize);

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        // void Hide();
        void Invalidate();

        /// <summary>
        /// Invalidates the specified invalidate children.
        /// </summary>
        /// <param name="invalidateChildren">if set to
        /// <c>
        /// true
        /// </c> [invalidate children].
        /// </param>
        void Invalidate(bool invalidateChildren);

        /// <summary>
        /// Invalidates the specified rc.
        /// </summary>
        /// <param name="rc">The rc.
        /// </param>
        void Invalidate(Rectangle rc);

        /// <summary>
        /// Invalidates the specified rc.
        /// </summary>
        /// <param name="rc">The rc.
        /// </param>
        /// <param name="invalidateChildren">if set to
        /// <c>
        /// true
        /// </c> [invalidate children].
        /// </param>
        void Invalidate(Rectangle rc, bool invalidateChildren);

        /// <summary>
        /// Invalidates the specified region.
        /// </summary>
        /// <param name="region">The region.
        /// </param>
        void Invalidate(Region region);

        /// <summary>
        /// Invalidates the specified region.
        /// </summary>
        /// <param name="region">The region.
        /// </param>
        /// <param name="invalidateChildren">if set to
        /// <c>
        /// true
        /// </c> [invalidate children].
        /// </param>
        void Invalidate(Region region, bool invalidateChildren);

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.
        /// </param>
        /// <returns>
        /// </returns>
        object Invoke(Delegate method);

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.
        /// </param>
        /// <param name="args">The arguments.
        /// </param>
        /// <returns>
        /// </returns>
        object Invoke(Delegate method, params object[] args);

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <typeparam name="TEntry">The type of the entry.
        /// </typeparam>
        /// <param name="method">The method.
        /// </param>
        /// <returns>
        /// </returns>
        TEntry Invoke<TEntry>(Func<TEntry> method);

        /// <summary>
        /// Logicals to device units.
        /// </summary>
        /// <param name="value">The value.
        /// </param>
        /// <returns>
        /// </returns>
        int LogicalToDeviceUnits(int value);

        /// <summary>
        /// Logicals to device units.
        /// </summary>
        /// <param name="value">The value.
        /// </param>
        /// <returns>
        /// </returns>
        Size LogicalToDeviceUnits(Size value);

        /// <summary>
        /// Performs the layout.
        /// </summary>
        void PerformLayout();

        /// <summary>
        /// Performs the layout.
        /// </summary>
        /// <param name="affectedControl">The affected control.
        /// </param>
        /// <param name="affectedProperty">The affected property.
        /// </param>
        void PerformLayout(Control affectedControl, string affectedProperty);

        /// <summary>
        /// Points to client.
        /// </summary>
        /// <param name="p">The p.
        /// </param>
        /// <returns>
        /// </returns>
        Point PointToClient(Point p);

        /// <summary>
        /// Points to screen.
        /// </summary>
        /// <param name="p">The p.
        /// </param>
        /// <returns>
        /// </returns>
        Point PointToScreen(Point p);

        /// <summary>
        /// Pres the process control message.
        /// </summary>
        /// <param name="msg">The MSG.
        /// </param>
        /// <returns>
        /// </returns>
        PreProcessControlState PreProcessControlMessage(ref Message msg);

        /// <summary>
        /// Pres the process message.
        /// </summary>
        /// <param name="msg">The MSG.
        /// </param>
        /// <returns>
        /// </returns>
        bool PreProcessMessage(ref Message msg);

        /// <summary>
        /// Rectangles to client.
        /// </summary>
        /// <param name="r">The r.
        /// </param>
        /// <returns>
        /// </returns>
        Rectangle RectangleToClient(Rectangle r);

        /// <summary>
        /// Rectangles to screen.
        /// </summary>
        /// <param name="r">The r.
        /// </param>
        /// <returns>
        /// </returns>
        Rectangle RectangleToScreen(Rectangle r);

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Resets the color of the back.
        /// </summary>
        void ResetBackColor();

        /// <summary>
        /// Resets the bindings.
        /// </summary>
        void ResetBindings();

        /// <summary>
        /// Resets the cursor.
        /// </summary>
        void ResetCursor();

        /// <summary>
        /// Resets the font.
        /// </summary>
        void ResetFont();

        /// <summary>
        /// Resets the color of the fore.
        /// </summary>
        void ResetForeColor();

        /// <summary>
        /// Resets the IME mode.
        /// </summary>
        void ResetImeMode();

        /// <summary>
        /// Resets the right to left.
        /// </summary>
        void ResetRightToLeft();

        /// <summary>
        /// Resets the text.
        /// </summary>
        void ResetText();

        /// <summary>
        /// Resumes the layout.
        /// </summary>
        void ResumeLayout();

        /// <summary>
        /// Resumes the layout.
        /// </summary>
        /// <param name="performLayout">if set to
        /// <c>
        /// true
        /// </c> [perform layout].
        /// </param>
        void ResumeLayout(bool performLayout);

        /// <summary>
        /// Scales the specified ratio.
        /// </summary>
        /// <param name="ratio">The ratio.
        /// </param>
        void Scale(float ratio);

        /// <summary>
        /// Scales the specified dx.
        /// </summary>
        /// <param name="dx">The dx.
        /// </param>
        /// <param name="dy">The dy.
        /// </param>
        void Scale(float dx, float dy);

        /// <summary>
        /// Scales the specified factor.
        /// </summary>
        /// <param name="factor">The factor.
        /// </param>
        void Scale(SizeF factor);

        /// <summary>
        /// Scales the bitmap logical to device.
        /// </summary>
        /// <param name="logicalBitmap">The logical bitmap.
        /// </param>
        void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap);

        /// <summary>
        /// Selects this instance.
        /// </summary>
        void Select();

        /// <summary>
        /// Selects the next control.
        /// </summary>
        /// <param name="ctl">The control.
        /// </param>
        /// <param name="forward">if set to
        /// <c>
        /// true
        /// </c> [forward].
        /// </param>
        /// <param name="tabStopOnly">if set to
        /// <c>
        /// true
        /// </c> [tab stop only].
        /// </param>
        /// <param name="nested">if set to
        /// <c>
        /// true
        /// </c> [nested].
        /// </param>
        /// <param name="wrap">if set to
        /// <c>
        /// true
        /// </c> [wrap].
        /// </param>
        /// <returns>
        /// </returns>
        bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);

        /// <summary>
        /// Sends to back.
        /// </summary>
        void SendToBack();

        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <param name="x">The x.
        /// </param>
        /// <param name="y">The y.
        /// </param>
        /// <param name="width">The width.
        /// </param>
        /// <param name="height">The height.
        /// </param>
        void SetBounds(int x, int y, int width, int height);

        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <param name="x">The x.
        /// </param>
        /// <param name="y">The y.
        /// </param>
        /// <param name="width">The width.
        /// </param>
        /// <param name="height">The height.
        /// </param>
        /// <param name="specified">The specified.
        /// </param>
        void SetBounds(int x, int y, int width, int height, BoundsSpecified specified);

        /// <summary>
        /// Suspends the layout.
        /// </summary>
        // void Show();
        void SuspendLayout();

        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update();
    }
}
