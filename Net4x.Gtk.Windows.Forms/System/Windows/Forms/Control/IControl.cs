using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// IControl is the interface implemented by all controls, including forms. It defines the basic properties, methods, and events that are common to all controls.
    /// </summary>
    public interface IControl
    {

        /// <summary>
        /// Occurs when the AutoSize property value changes.
        /// </summary>
        event EventHandler AutoSizeChanged;

        /// <summary>
        /// Occurs when the BackColor property value changes.
        /// </summary>
        event EventHandler BackColorChanged;

        /// <summary>
        /// Occurs when the BackgroundImage property value changes.
        /// </summary>
        event EventHandler BackgroundImageChanged;

        /// <summary>
        /// Occurs when the BackgroundImageLayout property value changes.
        /// </summary>
        event EventHandler BackgroundImageLayoutChanged;

        /// <summary>
        /// Occurs when the BindingContext property value changes.
        /// </summary>
        event EventHandler BindingContextChanged;

        /// <summary>
        /// Occurs when the CausesValidation property value changes.
        /// </summary>
        event EventHandler CausesValidationChanged;

        /// <summary>
        /// Occurs when the control's UI cues (keyboard indicators) change.
        /// </summary>
        event UICuesEventHandler ChangeUICues;

        /// <summary>
        /// Occurs when the control is clicked.
        /// </summary>
        event EventHandler Click;

        /// <summary>
        /// Occurs when the ClientSize property value changes.
        /// </summary>
        event EventHandler ClientSizeChanged;

        /// <summary>
        /// Occurs when the ContextMenuStrip property value changes.
        /// </summary>
        event EventHandler ContextMenuStripChanged;

        /// <summary>
        /// Occurs when a control is added to the Controls collection.
        /// </summary>
        event ControlEventHandler ControlAdded;

        /// <summary>
        /// Occurs when a control is removed from the Controls collection.
        /// </summary>
        event ControlEventHandler ControlRemoved;

        /// <summary>
        /// Occurs when the Cursor property value changes.
        /// </summary>
        event EventHandler CursorChanged;

        /// <summary>
        /// Occurs when the Dock property value changes.
        /// </summary>
        event EventHandler DockChanged;

        /// <summary>
        /// Occurs when the control is double-clicked.
        /// </summary>
        event EventHandler DoubleClick;

        /// <summary>
        /// Occurs after the DPI of the parent has changed.
        /// </summary>
        event EventHandler DpiChangedAfterParent;

        /// <summary>
        /// Occurs before the DPI of the parent changes.
        /// </summary>
        event EventHandler DpiChangedBeforeParent;

        /// <summary>
        /// Occurs when an object is dropped onto the control.
        /// </summary>
        event DragEventHandler DragDrop;

        /// <summary>
        /// Occurs when an object is dragged into the control's bounds.
        /// </summary>
        event DragEventHandler DragEnter;

        /// <summary>
        /// Occurs when an object is dragged outside the control's bounds.
        /// </summary>
        event EventHandler DragLeave;

        /// <summary>
        /// Occurs when an object is dragged over the control.
        /// </summary>
        event DragEventHandler DragOver;

        /// <summary>
        /// Occurs when the Enabled property value changes.
        /// </summary>
        event EventHandler EnabledChanged;

        /// <summary>
        /// Occurs when the control receives input focus.
        /// </summary>
        event EventHandler Enter;

        /// <summary>
        /// Occurs when the Font property value changes.
        /// </summary>
        event EventHandler FontChanged;

        /// <summary>
        /// Occurs when the ForeColor property value changes.
        /// </summary>
        event EventHandler ForeColorChanged;

        /// <summary>
        /// Occurs during a drag-and-drop operation to provide feedback to the source.
        /// </summary>
        event GiveFeedbackEventHandler GiveFeedback;

        /// <summary>
        /// Occurs when the control receives focus.
        /// </summary>
        event EventHandler GotFocus;

        /// <summary>
        /// Occurs when the underlying handle is created.
        /// </summary>
        event EventHandler HandleCreated;

        /// <summary>
        /// Occurs when the underlying handle is destroyed.
        /// </summary>
        event EventHandler HandleDestroyed;

        /// <summary>
        /// Occurs when the user requests help for the control.
        /// </summary>
        event HelpEventHandler HelpRequested;

        /// <summary>
        /// Occurs when the ImeMode property value changes.
        /// </summary>
        event EventHandler ImeModeChanged;

        /// <summary>
        /// Occurs when the control is invalidated and needs repainting.
        /// </summary>
        event InvalidateEventHandler Invalidated;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when a key is pressed and results in a character.
        /// </summary>
        event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        event KeyEventHandler KeyUp;

        /// <summary>
        /// Occurs when the layout of the control changes.
        /// </summary>
        event LayoutEventHandler Layout;

        /// <summary>
        /// Occurs when the control loses input focus.
        /// </summary>
        event EventHandler Leave;

        /// <summary>
        /// Occurs when the Location property value changes.
        /// </summary>
        event EventHandler LocationChanged;

        /// <summary>
        /// Occurs when the control loses focus.
        /// </summary>
        event EventHandler LostFocus;

        /// <summary>
        /// Occurs when the Margin property value changes.
        /// </summary>
        event EventHandler MarginChanged;

        /// <summary>
        /// Occurs when the mouse capture state changes.
        /// </summary>
        event EventHandler MouseCaptureChanged;

        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        event MouseEventHandler MouseClick;

        /// <summary>
        /// Occurs when the control is double-clicked by the mouse.
        /// </summary>
        event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// Occurs when a mouse button is pressed down on the control.
        /// </summary>
        event MouseEventHandler MouseDown;

        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        event EventHandler MouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer hovers over the control.
        /// </summary>
        event EventHandler MouseHover;

        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        event EventHandler MouseLeave;

        /// <summary>
        /// Occurs when the mouse pointer moves over the control.
        /// </summary>
        event MouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when a mouse button is released over the control.
        /// </summary>
        event MouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when the mouse wheel moves while the pointer is over the control.
        /// </summary>
        event MouseEventHandler MouseWheel;

        /// <summary>
        /// Occurs when the control is moved.
        /// </summary>
        event EventHandler Move;

        /// <summary>
        /// Occurs when the Padding property value changes.
        /// </summary>
        event EventHandler PaddingChanged;

        /// <summary>
        /// Occurs when the control needs to repaint itself.
        /// </summary>
        event PaintEventHandler Paint;

        /// <summary>
        /// Occurs when the Parent property value changes.
        /// </summary>
        event EventHandler ParentChanged;

        /// <summary>
        /// Occurs when a key is pressed while focus is on the control and it can be previewed.
        /// </summary>
        event PreviewKeyDownEventHandler PreviewKeyDown;

        /// <summary>
        /// Occurs when accessibility help is requested for the control.
        /// </summary>
        event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

        /// <summary>
        /// Occurs during a drag-and-drop operation to determine whether the drag should continue.
        /// </summary>
        event QueryContinueDragEventHandler QueryContinueDrag;

        /// <summary>
        /// Occurs when the Region property value changes.
        /// </summary>
        event EventHandler RegionChanged;

        /// <summary>
        /// Occurs when the control is resized.
        /// </summary>
        event EventHandler Resize;

        /// <summary>
        /// Occurs when the RightToLeft property value changes.
        /// </summary>
        event EventHandler RightToLeftChanged;

        /// <summary>
        /// Occurs when the Size property value changes.
        /// </summary>
        event EventHandler SizeChanged;

        /// <summary>
        /// Occurs when the control's style changes.
        /// </summary>
        event EventHandler StyleChanged;

        /// <summary>
        /// Occurs when the system colors change.
        /// </summary>
        event EventHandler SystemColorsChanged;

        /// <summary>
        /// Occurs when the TabIndex property value changes.
        /// </summary>
        event EventHandler TabIndexChanged;

        /// <summary>
        /// Occurs when the TabStop property value changes.
        /// </summary>
        event EventHandler TabStopChanged;

        /// <summary>
        /// Occurs when the Text property value changes.
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// Occurs when the control has been validated.
        /// </summary>
        event EventHandler Validated;

        /// <summary>
        /// Occurs when the control is validating, allowing the operation to be canceled.
        /// </summary>
        event CancelEventHandler Validating;

        /// <summary>
        /// Occurs when the Visible property value changes.
        /// </summary>
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
        /// <c>true</c> if allows drop; otherwise, <c>false</c>.
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
        /// <c>true</c> if automatic size; otherwise, <c>false</c>.
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
        /// <c>true</c> if this instance can focus; otherwise, <c>false</c>.
        /// </value>
        bool CanFocus { get; }

        /// <summary>
        /// Gets a value indicating whether this instance can select.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can select; otherwise, <c>false</c>.
        /// </value>
        bool CanSelect { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this IControl has captured the mouse.
        /// </summary>
        /// <value>
        /// <c>true</c> if captured; otherwise, <c>false</c>.
        /// </value>
        bool Capture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether causes validation.
        /// </summary>
        /// <value>
        /// <c>true</c> if causes validation; otherwise, <c>false</c>.
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
        /// <c>true</c> if contains focus; otherwise, <c>false</c>.
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
        /// Gets a value indicating whether this IControl is created.
        /// </summary>
        /// <value>
        /// <c>true</c> if created; otherwise, <c>false</c>.
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
        /// Gets a value indicating whether this IControl is disposing.
        /// </summary>
        /// <value>
        /// <c>true</c> if disposing; otherwise, <c>false</c>.
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
        /// Gets or sets a value indicating whether this IControl is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this IControl is focused.
        /// </summary>
        /// <value>
        /// <c>true</c> if focused; otherwise, <c>false</c>.
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
        /// <c>true</c> if this instance has children; otherwise, <c>false</c>.
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
        /// <c>true</c> if invoke required; otherwise, <c>false</c>.
        /// </value>
        bool InvokeRequired { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is accessible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is accessible; otherwise, <c>false</c>.
        /// </value>
        bool IsAccessible { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is handle created.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is handle created; otherwise, <c>false</c>.
        /// </value>
        bool IsHandleCreated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is mirrored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is mirrored; otherwise, <c>false</c>.
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
        /// <c>true</c> if recreating handle; otherwise, <c>false</c>.
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
        /// <c>true</c> if tab stop; otherwise, <c>false</c>.
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
        /// <c>true</c> if [use visual style back color]; otherwise, <c>false</c>.
        /// </value>
        bool UseVisualStyleBackColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use wait cursor.
        /// </summary>
        /// <value>
        /// <c>true</c> if use wait cursor; otherwise, <c>false</c>.
        /// </value>
        bool UseWaitCursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this IControl is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        bool Visible { get; set; }

        /// <summary>
        /// Gets the underlying Gtk.Widget associated with the control.
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
        /// <param name="method">The delegate to invoke asynchronously.</param>
        /// <returns>An IAsyncResult representing the asynchronous operation.</returns>
        IAsyncResult BeginInvoke(Delegate method);

        /// <summary>
        /// Begins the invoke with arguments.
        /// </summary>
        /// <param name="method">The delegate to invoke asynchronously.</param>
        /// <param name="args">Arguments to pass to the delegate.</param>
        /// <returns>An IAsyncResult representing the asynchronous operation.</returns>
        IAsyncResult BeginInvoke(Delegate method, params object[] args);

        /// <summary>
        /// Begins the invoke with an action.
        /// </summary>
        /// <param name="method">The action to invoke asynchronously.</param>
        /// <returns>An IAsyncResult representing the asynchronous operation.</returns>
        IAsyncResult BeginInvoke(Action method);

        /// <summary>
        /// Brings the control to the front of the z-order.
        /// </summary>
        void BringToFront();

        /// <summary>
        /// Determines whether this instance contains the specified control.
        /// </summary>
        /// <param name="ctl">The control to test for containment.</param>
        /// <returns><c>true</c> if the control is contained; otherwise, <c>false</c>.</returns>
        bool Contains(Control ctl);

        /// <summary>
        /// Creates the control and any child controls.
        /// </summary>
        void CreateControl();

        /// <summary>
        /// Creates a Graphics object for drawing to the control.
        /// </summary>
        /// <returns>A Graphics object.</returns>
        Graphics CreateGraphics();

        /// <summary>
        /// Initiates a drag-and-drop operation.
        /// </summary>
        /// <param name="data">Data to drag.</param>
        /// <param name="allowedEffects">Allowed drag-drop effects.</param>
        /// <returns>The resulting DragDropEffects.</returns>
        DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);

        /// <summary>
        /// Draws the control into the specified bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to draw into.</param>
        /// <param name="targetBounds">Area within the bitmap to draw.</param>
        void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);

        /// <summary>
        /// Completes an asynchronous operation started with BeginInvoke.
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult returned from BeginInvoke.</param>
        /// <returns>The result of the asynchronous operation, if any.</returns>
        object EndInvoke(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves the form that the control is on.
        /// </summary>
        /// <returns>The parent Form, or null if none is found.</returns>
        Form FindForm();

        /// <summary>
        /// Attempts to set input focus to the control.
        /// </summary>
        /// <returns>True if the control received focus; otherwise false.</returns>
        bool Focus();

        /// <summary>
        /// Gets the child control at the specified point.
        /// </summary>
        /// <param name="pt">Point to test, in client coordinates.</param>
        /// <returns>The child control at the specified point, or null if none.</returns>
        Control GetChildAtPoint(Point pt);

        /// <summary>
        /// Gets the child control at the specified point with skip options.
        /// </summary>
        /// <param name="pt">Point to test, in client coordinates.</param>
        /// <param name="skipValue">Specifies which children to skip.</param>
        /// <returns>The child control at the specified point, or null if none.</returns>
        Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue);

        /// <summary>
        /// Returns the container control that the control is associated with.
        /// </summary>
        /// <returns>An IContainerControl or null.</returns>
        IContainerControl GetContainerControl();

        /// <summary>
        /// Gets the next control in the tab order.
        /// </summary>
        /// <param name="ctl">The starting control.</param>
        /// <param name="forward">True to move forward; false to move backward.</param>
        /// <returns>The next control in the tab order, or null if none.</returns>
        Control GetNextControl(Control ctl, bool forward);

        /// <summary>
        /// Returns the preferred size of the control given the proposed size.
        /// </summary>
        /// <param name="proposedSize">The available size for the control.</param>
        /// <returns>The preferred size.</returns>
        Size GetPreferredSize(Size proposedSize);

        /// <summary>
        /// Invalidates the entire control and causes a repaint.
        /// </summary>
        void Invalidate();

        /// <summary>
        /// Invalidates the control and optionally its children.
        /// </summary>
        /// <param name="invalidateChildren">True to invalidate child controls as well.</param>
        void Invalidate(bool invalidateChildren);

        /// <summary>
        /// Invalidates the specified rectangle of the control.
        /// </summary>
        /// <param name="rc">Rectangle to invalidate, in client coordinates.</param>
        void Invalidate(Rectangle rc);

        /// <summary>
        /// Invalidates the specified rectangle of the control and optionally its children.
        /// </summary>
        /// <param name="rc">Rectangle to invalidate, in client coordinates.</param>
        /// <param name="invalidateChildren">True to invalidate child controls as well.</param>
        void Invalidate(Rectangle rc, bool invalidateChildren);

        /// <summary>
        /// Invalidates the specified region of the control.
        /// </summary>
        /// <param name="region">Region to invalidate.</param>
        void Invalidate(Region region);

        /// <summary>
        /// Invalidates the specified region of the control and optionally its children.
        /// </summary>
        /// <param name="region">Region to invalidate.</param>
        /// <param name="invalidateChildren">True to invalidate child controls as well.</param>
        void Invalidate(Region region, bool invalidateChildren);

        /// <summary>
        /// Invokes a delegate on the control's thread and returns its result.
        /// </summary>
        /// <param name="method">Delegate to invoke.</param>
        /// <returns>The result returned by the delegate.</returns>
        object Invoke(Delegate method);

        /// <summary>
        /// Invokes a delegate on the control's thread with arguments and returns its result.
        /// </summary>
        /// <param name="method">Delegate to invoke.</param>
        /// <param name="args">Arguments to pass to the delegate.</param>
        /// <returns>The result returned by the delegate.</returns>
        object Invoke(Delegate method, params object[] args);

        /// <summary>
        /// Invokes a function on the control's thread and returns typed result.
        /// </summary>
        /// <typeparam name="TEntry">Return type.</typeparam>
        /// <param name="method">Function to invoke.</param>
        /// <returns>Result of the function.</returns>
        TEntry Invoke<TEntry>(Func<TEntry> method);

        /// <summary>
        /// Converts a logical unit value to device units.
        /// </summary>
        /// <param name="value">Value in logical units.</param>
        /// <returns>Value in device units.</returns>
        int LogicalToDeviceUnits(int value);

        /// <summary>
        /// Converts a logical size to device units.
        /// </summary>
        /// <param name="value">Size in logical units.</param>
        /// <returns>Size in device units.</returns>
        Size LogicalToDeviceUnits(Size value);

        /// <summary>
        /// Forces the control to perform layout logic.
        /// </summary>
        void PerformLayout();

        /// <summary>
        /// Forces the control to perform layout logic for a specific affected control/property.
        /// </summary>
        /// <param name="affectedControl">Control affected by the layout.</param>
        /// <param name="affectedProperty">Property name that changed.</param>
        void PerformLayout(Control affectedControl, string affectedProperty);

        /// <summary>
        /// Converts a screen point to client coordinates.
        /// </summary>
        /// <param name="p">Point in screen coordinates.</param>
        /// <returns>Point in client coordinates.</returns>
        Point PointToClient(Point p);

        /// <summary>
        /// Converts a client point to screen coordinates.
        /// </summary>
        /// <param name="p">Point in client coordinates.</param>
        /// <returns>Point in screen coordinates.</returns>
        Point PointToScreen(Point p);

        /// <summary>
        /// Preprocesses a Windows message for the control.
        /// </summary>
        /// <param name="msg">Message to preprocess.</param>
        /// <returns>A PreProcessControlState value.</returns>
        PreProcessControlState PreProcessControlMessage(ref Message msg);

        /// <summary>
        /// Preprocesses a message and returns whether it was handled.
        /// </summary>
        /// <param name="msg">Message to preprocess.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        bool PreProcessMessage(ref Message msg);

        /// <summary>
        /// Converts a rectangle from screen coordinates to client coordinates.
        /// </summary>
        /// <param name="r">Rectangle in screen coordinates.</param>
        /// <returns>Rectangle in client coordinates.</returns>
        Rectangle RectangleToClient(Rectangle r);

        /// <summary>
        /// Converts a rectangle from client coordinates to screen coordinates.
        /// </summary>
        /// <param name="r">Rectangle in client coordinates.</param>
        /// <returns>Rectangle in screen coordinates.</returns>
        Rectangle RectangleToScreen(Rectangle r);

        /// <summary>
        /// Causes the control to be redrawn.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Resets the BackColor property to its default value.
        /// </summary>
        void ResetBackColor();

        /// <summary>
        /// Clears all data bindings on the control.
        /// </summary>
        void ResetBindings();

        /// <summary>
        /// Resets the Cursor property to its default value.
        /// </summary>
        void ResetCursor();

        /// <summary>
        /// Resets the Font property to its default value.
        /// </summary>
        void ResetFont();

        /// <summary>
        /// Resets the ForeColor property to its default value.
        /// </summary>
        void ResetForeColor();

        /// <summary>
        /// Resets the IME mode to its default value.
        /// </summary>
        void ResetImeMode();

        /// <summary>
        /// Resets the RightToLeft property to its default value.
        /// </summary>
        void ResetRightToLeft();

        /// <summary>
        /// Resets the Text property to its default value.
        /// </summary>
        void ResetText();

        /// <summary>
        /// Resumes normal layout logic.
        /// </summary>
        void ResumeLayout();

        /// <summary>
        /// Resumes layout logic and optionally forces an immediate layout.
        /// </summary>
        /// <param name="performLayout">True to force a layout immediately.</param>
        void ResumeLayout(bool performLayout);

        /// <summary>
        /// Scales the control by the specified ratio.
        /// </summary>
        /// <param name="ratio">Scale ratio.</param>
        void Scale(float ratio);

        /// <summary>
        /// Scales the control by the specified x and y factors.
        /// </summary>
        /// <param name="dx">Horizontal scale factor.</param>
        /// <param name="dy">Vertical scale factor.</param>
        void Scale(float dx, float dy);

        /// <summary>
        /// Scales the control by the specified factor.
        /// </summary>
        /// <param name="factor">Scale factor.</param>
        void Scale(SizeF factor);

        /// <summary>
        /// Scales a logical bitmap to device pixels.
        /// </summary>
        /// <param name="logicalBitmap">Bitmap to scale (in/out parameter).</param>
        void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap);

        /// <summary>
        /// Selects the control.
        /// </summary>
        void Select();

        /// <summary>
        /// Selects the next control in the tab order.
        /// </summary>
        /// <param name="ctl">Starting control.</param>
        /// <param name="forward">True to move forward; false to move backward.</param>
        /// <param name="tabStopOnly">True to consider only tab stop controls.</param>
        /// <param name="nested">True to include nested child controls.</param>
        /// <param name="wrap">True to wrap to the first control when the end is reached.</param>
        /// <returns>True if another control was selected; otherwise false.</returns>
        bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);

        /// <summary>
        /// Sends the control to the back of the z-order.
        /// </summary>
        void SendToBack();

        /// <summary>
        /// Sets the bounds of the control.
        /// </summary>
        /// <param name="x">Left coordinate.</param>
        /// <param name="y">Top coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void SetBounds(int x, int y, int width, int height);

        /// <summary>
        /// Sets the bounds of the control with specified bounds flags.
        /// </summary>
        /// <param name="x">Left coordinate.</param>
        /// <param name="y">Top coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="specified">Which bounds components to set.</param>
        void SetBounds(int x, int y, int width, int height, BoundsSpecified specified);

        /// <summary>
        /// Suspends layout logic for the control.
        /// </summary>
        void SuspendLayout();

        /// <summary>
        /// Updates the control and processes pending paint requests.
        /// </summary>
        void Update();

    }
}

