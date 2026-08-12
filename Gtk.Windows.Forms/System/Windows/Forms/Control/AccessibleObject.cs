namespace System.Windows.Forms;

/// <summary>
/// Provides information that accessibility applications use to adjust an application's user interface (UI) for users with impairments.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
/// <remarks>
/// This was an empty class and <see cref="P:System.Windows.Forms.Control.AccessibilityObject" /> always
/// returned <c>null</c>, so the accessibility API was present in name only - nothing reached a screen
/// reader on any platform. It is now backed by the GTK widget's ATK peer, which is what AT-SPI (Orca,
/// and the accessibility bridges on Windows and macOS) actually reads.
/// <para>
/// Scope: name, description, role, value, bounds and parent/child navigation are wired. The richer
/// UI Automation pattern surface of modern WinForms is not emulated.
/// </para>
/// </remarks>
public class AccessibleObject
{
    /// <summary>
    /// The ATK peer this object reflects, when it was created for a live widget.
    /// </summary>
    // When this object belongs to a Control, name/description/role are served from and written to that
    // control's WinForms state (as real WinForms' ControlAccessibleObject does), which is what the values
    // round-trip through - GTK4 exposes no getter for the accessible label/description. The control's setters
    // additionally push the values onto the widget's Gtk.Accessible interface for AT-SPI. The _name/
    // _description fields back the ownerless path (the public parameterless ctor, used by subclasses that
    // call base.Name = ...).
    private readonly Control? _owner;
    private string? _name;
    private string? _description;

    /// <summary>
    /// The widget the peer belongs to, used for bounds. GtkSharp does not project the
    /// <c>Atk.Component</c> interface, so extents come from the widget's allocation instead.
    /// </summary>
    private readonly Gtk.Widget? _widget;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.AccessibleObject" /> class
    /// that is not attached to a widget.
    /// </summary>
    public AccessibleObject()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.AccessibleObject" /> class
    /// backed by the supplied ATK peer.
    /// </summary>
    /// <param name="owner">
    /// The control this accessible object reflects.
    /// </param>
    internal AccessibleObject(Control owner)
    {
        _owner = owner;
        _widget = owner.Widget;
    }

    /// <summary>
    /// Gets the location and size of the accessible object, in screen coordinates.
    /// </summary>
    public virtual Drawing.Rectangle Bounds
    {
        get
        {
            if (_widget == null)
            {
                return Drawing.Rectangle.Empty;
            }

            var allocation = _widget.Allocation;
            var x = allocation.X;
            var y = allocation.Y;

            // Translate to screen coordinates when the widget has a window to translate against.
            if (_widget.IsRealized)
            {
                _widget.GetOrigin(out var originX, out var originY);
                x += originX;
                y += originY;
            }

            return new Drawing.Rectangle(x, y, allocation.Width, allocation.Height);
        }
    }

    /// <summary>
    /// Gets or sets the object description.
    /// </summary>
    public virtual string? Description
    {
        get => _owner != null ? _owner.AccessibleDescription : _description;
        set
        {
            if (_owner != null)
            {
                _owner.AccessibleDescription = value;
            }
            else
            {
                _description = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public virtual string? Name
    {
        // Coalesce to empty for an owner with no name set, matching WinForms (AccessibleName defaults to "").
        get => _owner != null ? (_owner.AccessibleName ?? string.Empty) : _name;
        set
        {
            if (_owner != null)
            {
                _owner.AccessibleName = value;
            }
            else
            {
                _name = value;
            }
        }
    }

    /// <summary>
    /// Gets the parent of this accessible object.
    /// </summary>
    public virtual AccessibleObject? Parent => _owner?.Parent?.AccessibilityObject;

    /// <summary>
    /// Gets the role of this accessible object.
    /// </summary>
    public virtual AccessibleRole Role => _owner?.AccessibleRole ?? AccessibleRole.None;

    /// <summary>
    /// Retrieves the accessible child at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the child.
    /// </param>
    /// <returns>
    /// The child, or <c>null</c> when the index is out of range.
    /// </returns>
    public virtual AccessibleObject? GetChild(int index)
    {
        var controls = _owner?.Controls;
        if (controls == null || index < 0 || index >= controls.Count)
        {
            return null;
        }

        return (controls[index] as Control)?.AccessibilityObject;
    }

    /// <summary>
    /// Retrieves the number of accessible children.
    /// </summary>
    /// <returns>
    /// The child count.
    /// </returns>
    public virtual int GetChildCount() => _owner?.Controls?.Count ?? 0;

    /// <summary>
    /// Returns the name of this accessible object.
    /// </summary>
    /// <returns>
    /// A string describing this object.
    /// </returns>
    public override string ToString() => $"{nameof(AccessibleObject)}: Name = {Name}";
}
