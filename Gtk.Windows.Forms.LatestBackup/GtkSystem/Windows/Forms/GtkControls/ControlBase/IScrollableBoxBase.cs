using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// IScrollableBoxBase is the base interface for scrollable box controls, such as
/// <see cref="!:ScrollableBox" /> and
/// <see cref="!:ScrollableBox2" />.
/// </summary>
public interface IScrollableBoxBase
{
    /// <summary>
    /// Occurs when scroll.
    /// </summary>
    event System.Windows.Forms.ScrollEventHandler Scroll;

    /// <summary>
    /// Gets or sets a value indicating whether automatic scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool AutoScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the child.
    /// </summary>
    /// <value>
    /// The child.
    /// </value>
    Widget Child { get; }

    /// <summary>
    /// Gets or sets the height request.
    /// </summary>
    /// <value>
    /// The height request.
    /// </value>
    int HeightRequest
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether  horizontal scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if  horizontal scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool HScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether vertical scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if vertical scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool VScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width request.
    /// </summary>
    /// <value>
    /// The width request.
    /// </value>
    int WidthRequest
    {
        get;
        set;
    }

    /// <summary>
    /// Adds the specified child.
    /// </summary>
    /// <param name="child">
    /// The child.
    /// </param>
    void Add(Widget child);

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    void AddClass(string cssClass);

}