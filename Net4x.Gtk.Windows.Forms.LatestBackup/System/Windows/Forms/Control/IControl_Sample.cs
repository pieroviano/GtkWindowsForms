using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Sample minimal control interface used for lightweight control wrappers and tests.
/// Documents a subset of common control events, properties and methods.
/// </summary>
public interface IControl_Sample
{
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
    /// Occurs when UI cue states (keyboard cues) change.
    /// </summary>
    event UICuesEventHandler ChangeUICues;

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    event EventHandler Click;

    /// <summary>
    /// Occurs when the ClientSize changes.
    /// </summary>
    event EventHandler ClientSizeChanged;

    /// <summary>
    /// Occurs when the ContextMenuStrip property value changes.
    /// </summary>
    event EventHandler ContextMenuStripChanged;

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
    /// Occurs when an object is dragged out of the control's bounds.
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
    /// Occurs when the user requests help for the control.
    /// </summary>
    event HelpEventHandler HelpRequested;

    /// <summary>
    /// Occurs when the ImeMode property value changes.
    /// </summary>
    event EventHandler ImeModeChanged;

    /// <summary>
    /// Occurs when a key is pressed while the control has focus.
    /// </summary>
    event KeyEventHandler KeyDown;

    /// <summary>
    /// Occurs when a key press yields a character.
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
    /// Occurs when the Margin property value changes.
    /// </summary>
    event EventHandler MarginChanged;

    /// <summary>
    /// Occurs when the mouse capture state changes.
    /// </summary>
    event EventHandler MouseCaptureChanged;

    /// <summary>
    /// Occurs when the control is clicked with the mouse.
    /// </summary>
    event MouseEventHandler MouseClick;

    /// <summary>
    /// Occurs when the control is double-clicked with the mouse.
    /// </summary>
    event MouseEventHandler MouseDoubleClick;

    /// <summary>
    /// Occurs when a mouse button is pressed while the cursor is over the control.
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
    /// Occurs when a mouse button is released while the cursor is over the control.
    /// </summary>
    event MouseEventHandler MouseUp;

    /// <summary>
    /// Occurs when the control is moved.
    /// </summary>
    event EventHandler Move;

    /// <summary>
    /// Occurs when the Padding property value changes.
    /// </summary>
    event EventHandler PaddingChanged;

    /// <summary>
    /// Occurs when the control needs to be painted.
    /// </summary>
    event PaintEventHandler Paint;

    /// <summary>
    /// Occurs when the Parent property value changes.
    /// </summary>
    event EventHandler ParentChanged;

    /// <summary>
    /// Occurs when a key is pressed and can be previewed.
    /// </summary>
    event PreviewKeyDownEventHandler PreviewKeyDown;

    /// <summary>
    /// Occurs when accessibility help is requested for the control.
    /// </summary>
    event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

    /// <summary>
    /// Occurs during a drag operation to determine whether the drag should continue.
    /// </summary>
    event QueryContinueDragEventHandler QueryContinueDrag;

    /// <summary>
    /// Occurs when the Region property value changes.
    /// </summary>
    event EventHandler RegionChanged;

    /// <summary>
    /// Occurs when RightToLeft property changes.
    /// </summary>
    event EventHandler RightToLeftChanged;

    /// <summary>
    /// Occurs when the Size property changes.
    /// </summary>
    event EventHandler SizeChanged;

    /// <summary>
    /// Occurs when the control style changes.
    /// </summary>
    event EventHandler StyleChanged;

    /// <summary>
    /// Occurs when system colors change.
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
    /// Occurs when the control is validated.
    /// </summary>
    event EventHandler Validated;

    /// <summary>
    /// Occurs when validating the control; allows cancellation.
    /// </summary>
    event CancelEventHandler Validating;

    /// <summary>
    /// Occurs when the Visible property value changes.
    /// </summary>
    event EventHandler VisibleChanged;

    /// <summary>
    /// Gets or sets the accessible description for the control.
    /// </summary>
    string AccessibleDescription { get; set; }

    /// <summary>
    /// Gets or sets the accessible name for the control.
    /// </summary>
    string AccessibleName { get; set; }

    /// <summary>
    /// Gets or sets the accessible role for the control.
    /// </summary>
    AccessibleRole AccessibleRole { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control accepts drag-and-drop.
    /// </summary>
    bool AllowDrop { get; set; }

    /// <summary>
    /// Gets or sets the anchor styles for the control.
    /// </summary>
    AnchorStyles Anchor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control automatically sizes to its contents.
    /// </summary>
    bool AutoSize { get; set; }

    /// <summary>
    /// Gets or sets the background image for the control.
    /// </summary>
    //Color BackColor { get; set; }
    Image BackgroundImage { get; set; }

    /// <summary>
    /// Gets or sets the background image layout style.
    /// </summary>
    ImageLayout BackgroundImageLayout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether causes validation on focus change.
    /// </summary>
    bool CausesValidation { get; set; }

    /// <summary>
    /// Gets or sets the context menu strip associated with the control.
    /// </summary>
    ContextMenuStrip ContextMenuStrip { get; set; }

    /// <summary>
    /// Gets or sets the cursor used by the control.
    /// </summary>
    Cursor Cursor { get; set; }

    /// <summary>
    /// Gets or sets how the control is docked within its parent.
    /// </summary>
    DockStyle Dock { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the font for the control.
    /// </summary>
    Font Font { get; set; }

    /// <summary>
    /// Gets or sets the height of the control.
    /// </summary>
    //Color ForeColor { get; set; }
    int Height { get; set; }

    /// <summary>
    /// Gets or sets the IME mode for the control.
    /// </summary>
    ImeMode ImeMode { get; set; }

    /// <summary>
    /// Gets or sets the maximum size the control can be resized to.
    /// </summary>
    //int Left { get; set; }
    //Point Location { get; set; }
    //Padding Margin { get; set; }
    Size MaximumSize { get; set; }

    /// <summary>
    /// Gets or sets the minimum size the control can be resized to.
    /// </summary>
    Size MinimumSize { get; set; }

    /// <summary>
    /// Gets or sets the padding within the control.
    /// </summary>
    // string Name { get; set; }
    Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets the RightToLeft setting for the control.
    /// </summary>
    //Control Parent { get; set; }
    RightToLeft RightToLeft { get; set; }

    /// <summary>
    /// Gets or sets the tab index of the control.
    /// </summary>
    //Size Size { get; set; }
    int TabIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is included in tab navigation.
    /// </summary>
    bool TabStop { get; set; }

    /// <summary>
    /// Gets or sets user-defined data associated with the control.
    /// </summary>
    object Tag { get; set; }

    /// <summary>
    /// Gets or sets the top coordinate of the control relative to its container.
    /// </summary>
    //string Text { get; set; }
    int Top { get; set; }

    /// <summary>
    /// Gets or sets whether the control should use the visual style background color.
    /// </summary>
    bool UseVisualStyleBackColor { get; set; }

    /// <summary>
    /// Gets or sets whether the wait cursor should be shown for the control.
    /// </summary>
    bool UseWaitCursor { get; set; }

    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    //bool Visible { get; set; }
    int Width { get; set; }

    /// <summary>
    /// Brings the control to the front of the Z-order.
    /// </summary>
    void BringToFront();

    /// <summary>
    /// Determines whether the specified control is contained within this control.
    /// </summary>
    /// <param name="ctl">Control to test.
    /// </param>
    /// <returns>True if contained; otherwise false.
    /// </returns>
    bool Contains(Control ctl);

    /// <summary>
    /// Creates the control and any child controls.
    /// </summary>
    void CreateControl();

    /// <summary>
    /// Creates a Graphics object for drawing to the control.
    /// </summary>
    /// <returns>A Graphics instance for the control.
    /// </returns>
    Graphics CreateGraphics();

    /// <summary>
    /// Initiates a drag-and-drop operation.
    /// </summary>
    /// <param name="data">Data to drag.
    /// </param>
    /// <param name="allowedEffects">Allowed drag-drop effects.
    /// </param>
    /// <returns>The drag operation result.
    /// </returns>
    DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);

    /// <summary>
    /// Draws the control into the specified bitmap.
    /// </summary>
    /// <param name="bitmap">Destination bitmap.
    /// </param>
    /// <param name="targetBounds">Area within bitmap to draw.
    /// </param>
    void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);

    /// <summary>
    /// Gets the parent form that contains this control.
    /// </summary>
    /// <returns>The parent Form or null.
    /// </returns>
    Form FindForm();

    /// <summary>
    /// Returns the child control at the specified point.
    /// </summary>
    /// <param name="pt">Point in client coordinates.
    /// </param>
    /// <returns>Child control or null.
    /// </returns>
    Control GetChildAtPoint(Point pt);

    /// <summary>
    /// Returns the child control at the specified point with skip options.
    /// </summary>
    /// <param name="pt">Point in client coordinates.
    /// </param>
    /// <param name="skipValue">Children to skip.
    /// </param>
    /// <returns>Child control or null.
    /// </returns>
    Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue);

    /// <summary>
    /// Returns the container control associated with this control.
    /// </summary>
    /// <returns>An IContainerControl instance or null.
    /// </returns>
    IContainerControl GetContainerControl();

    /// <summary>
    /// Gets the next control in the tab order.
    /// </summary>
    /// <param name="ctl">Starting control.
    /// </param>
    /// <param name="forward">True to move forward; false to move backward.
    /// </param>
    /// <returns>The next control or null.
    /// </returns>
    Control GetNextControl(Control ctl, bool forward);

    /// <summary>
    /// Hides the control.
    /// </summary>
    void Hide();

    /// <summary>
    /// Invalidates the entire control and causes a repaint.
    /// </summary>
    void Invalidate();

    /// <summary>
    /// Invalidates the control and optionally its children.
    /// </summary>
    /// <param name="invalidateChildren">True to invalidate child controls as well.
    /// </param>
    void Invalidate(bool invalidateChildren);

    /// <summary>
    /// Invalidates the specified rectangle of the control.
    /// </summary>
    /// <param name="rc">Rectangle to invalidate.
    /// </param>
    void Invalidate(Rectangle rc);

    /// <summary>
    /// Invalidates the specified rectangle of the control and optionally its children.
    /// </summary>
    /// <param name="rc">Rectangle to invalidate.
    /// </param>
    /// <param name="invalidateChildren">True to invalidate child controls as well.
    /// </param>
    void Invalidate(Rectangle rc, bool invalidateChildren);

    /// <summary>
    /// Invalidates the specified region of the control.
    /// </summary>
    /// <param name="region">Region to invalidate.
    /// </param>
    void Invalidate(Region region);

    /// <summary>
    /// Invalidates the specified region of the control and optionally its children.
    /// </summary>
    /// <param name="region">Region to invalidate.
    /// </param>
    /// <param name="invalidateChildren">True to invalidate child controls as well.
    /// </param>
    void Invalidate(Region region, bool invalidateChildren);

    /// <summary>
    /// Executes a delegate on the control's thread and returns its result.
    /// </summary>
    /// <param name="method">Delegate to invoke.
    /// </param>
    /// <returns>Result of the delegate.
    /// </returns>
    object Invoke(Delegate method);

    /// <summary>
    /// Executes a delegate on the control's thread with arguments and returns its result.
    /// </summary>
    /// <param name="method">Delegate to invoke.
    /// </param>
    /// <param name="args">Arguments for the delegate.
    /// </param>
    /// <returns>Result of the delegate.
    /// </returns>
    object Invoke(Delegate method, params object[] args);

    /// <summary>
    /// Converts a logical value to device units (int).
    /// </summary>
    /// <param name="value">Logical value.
    /// </param>
    /// <returns>Device units value.
    /// </returns>
    int LogicalToDeviceUnits(int value);

    /// <summary>
    /// Converts a logical size to device units.
    /// </summary>
    /// <param name="value">Logical size.
    /// </param>
    /// <returns>Device size in device units.
    /// </returns>
    Size LogicalToDeviceUnits(Size value);

    /// <summary>
    /// Forces the control to perform layout logic.
    /// </summary>
    void PerformLayout();

    /// <summary>
    /// Forces the control to perform layout logic for a specific affected control/property.
    /// </summary>
    /// <param name="affectedControl">Control affected by the layout.
    /// </param>
    /// <param name="affectedProperty">Property name that changed.
    /// </param>
    void PerformLayout(Control affectedControl, string affectedProperty);

    /// <summary>
    /// Converts a screen point to client coordinates.
    /// </summary>
    /// <param name="p">Point in screen coordinates.
    /// </param>
    /// <returns>Point in client coordinates.
    /// </returns>
    Point PointToClient(Point p);

    /// <summary>
    /// Converts a client point to screen coordinates.
    /// </summary>
    /// <param name="p">Point in client coordinates.
    /// </param>
    /// <returns>Point in screen coordinates.
    /// </returns>
    Point PointToScreen(Point p);

    /// <summary>
    /// Preprocesses a message and returns whether it was handled.
    /// </summary>
    /// <param name="msg">Message to preprocess.
    /// </param>
    /// <returns>True if processed; otherwise false.
    /// </returns>
    bool PreProcessMessage(ref Message msg);

    /// <summary>
    /// Converts a rectangle from screen to client coordinates.
    /// </summary>
    /// <param name="r">Rectangle in screen coordinates.
    /// </param>
    /// <returns>Rectangle in client coordinates.
    /// </returns>
    Rectangle RectangleToClient(Rectangle r);

    /// <summary>
    /// Converts a rectangle from client to screen coordinates.
    /// </summary>
    /// <param name="r">Rectangle in client coordinates.
    /// </param>
    /// <returns>Rectangle in screen coordinates.
    /// </returns>
    Rectangle RectangleToScreen(Rectangle r);

    /// <summary>
    /// Forces the control to repaint itself.
    /// </summary>
    void Refresh();

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
    /// <param name="performLayout">True to force a layout immediately.
    /// </param>
    void ResumeLayout(bool performLayout);

    /// <summary>
    /// Scales a logical bitmap to device pixels.
    /// </summary>
    /// <param name="logicalBitmap">Bitmap to scale (in/out parameter).
    /// </param>
    void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap);

    /// <summary>
    /// Selects the control.
    /// </summary>
    void Select();

    /// <summary>
    /// Selects the next control in the tab order.
    /// </summary>
    /// <param name="ctl">Starting control.
    /// </param>
    /// <param name="forward">True to move forward; false to move backward.
    /// </param>
    /// <param name="tabStopOnly">True to consider only tab stop controls.
    /// </param>
    /// <param name="nested">True to include nested child controls.
    /// </param>
    /// <param name="wrap">True to wrap to the first control when the end is reached.
    /// </param>
    /// <returns>True if another control was selected; otherwise false.
    /// </returns>
    bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);

    /// <summary>
    /// Sends the control to the back of the Z-order.
    /// </summary>
    void SendToBack();

    /// <summary>
    /// Sets the bounds of the control.
    /// </summary>
    /// <param name="x">Left coordinate.
    /// </param>
    /// <param name="y">Top coordinate.
    /// </param>
    /// <param name="width">Width.
    /// </param>
    /// <param name="height">Height.
    /// </param>
    void SetBounds(int x, int y, int width, int height);

    /// <summary>
    /// Sets the bounds of the control with specified bounds flags.
    /// </summary>
    /// <param name="x">Left coordinate.
    /// </param>
    /// <param name="y">Top coordinate.
    /// </param>
    /// <param name="width">Width.
    /// </param>
    /// <param name="height">Height.
    /// </param>
    /// <param name="specified">Which bounds components to set.
    /// </param>
    void SetBounds(int x, int y, int width, int height, BoundsSpecified specified);

    /// <summary>
    /// Shows the control.
    /// </summary>
    void Show();

    /// <summary>
    /// Suspends layout logic for the control.
    /// </summary>
    void SuspendLayout();

    /// <summary>
    /// Updates the control and processes pending paint requests.
    /// </summary>
    void Update();

}