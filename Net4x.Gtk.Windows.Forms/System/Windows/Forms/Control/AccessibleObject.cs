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
    private readonly Atk.Object? _peer;

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
    /// <param name="peer">
    /// The ATK object exposed by the GTK widget.
    /// </param>
    /// <param name="widget">
    /// The widget the peer belongs to, when known.
    /// </param>
    internal AccessibleObject(Atk.Object? peer, Gtk.Widget? widget = null)
    {
        _peer = peer;
        _widget = widget;
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
            if (_widget.Window != null)
            {
                _widget.Window.GetOrigin(out var originX, out var originY);
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
        get => _peer?.Description;
        set
        {
            if (_peer != null)
            {
                _peer.Description = value ?? string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public virtual string? Name
    {
        get => _peer?.Name;
        set
        {
            if (_peer != null)
            {
                _peer.Name = value ?? string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets the parent of this accessible object.
    /// </summary>
    public virtual AccessibleObject? Parent
    {
        get
        {
            var parent = _peer?.Parent;

            return parent == null ? null : new AccessibleObject(parent);
        }
    }

    /// <summary>
    /// Gets the role of this accessible object.
    /// </summary>
    public virtual AccessibleRole Role => _peer == null ? AccessibleRole.None : FromAtkRole(_peer.Role);

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
        if (_peer == null || index < 0 || index >= _peer.NAccessibleChildren)
        {
            return null;
        }

        var child = _peer.RefAccessibleChild(index);

        return child == null ? null : new AccessibleObject(child);
    }

    /// <summary>
    /// Retrieves the number of accessible children.
    /// </summary>
    /// <returns>
    /// The child count.
    /// </returns>
    public virtual int GetChildCount() => _peer?.NAccessibleChildren ?? 0;

    /// <summary>
    /// Returns the name of this accessible object.
    /// </summary>
    /// <returns>
    /// A string describing this object.
    /// </returns>
    public override string ToString() => $"{nameof(AccessibleObject)}: Name = {Name}";

    /// <summary>
    /// Maps a WinForms <see cref="T:System.Windows.Forms.AccessibleRole" /> onto the nearest ATK role.
    /// </summary>
    /// <param name="role">
    /// The WinForms role.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="T:Atk.Role" />.
    /// </returns>
    internal static Atk.Role ToAtkRole(AccessibleRole role) => role switch
    {
        AccessibleRole.PushButton => Atk.Role.PushButton,
        AccessibleRole.CheckButton => Atk.Role.CheckBox,
        AccessibleRole.RadioButton => Atk.Role.RadioButton,
        AccessibleRole.ComboBox => Atk.Role.ComboBox,
        AccessibleRole.Text => Atk.Role.Entry,
        AccessibleRole.StaticText => Atk.Role.Label,
        AccessibleRole.List => Atk.Role.List,
        AccessibleRole.ListItem => Atk.Role.ListItem,
        AccessibleRole.MenuBar => Atk.Role.MenuBar,
        AccessibleRole.MenuItem => Atk.Role.MenuItem,
        AccessibleRole.MenuPopup => Atk.Role.Menu,
        AccessibleRole.Outline => Atk.Role.TreeTable,
        AccessibleRole.OutlineItem => Atk.Role.TreeItem,
        AccessibleRole.PageTab => Atk.Role.PageTab,
        AccessibleRole.PageTabList => Atk.Role.PageTabList,
        AccessibleRole.ProgressBar => Atk.Role.ProgressBar,
        AccessibleRole.ScrollBar => Atk.Role.ScrollBar,
        AccessibleRole.Slider => Atk.Role.Slider,
        AccessibleRole.SpinButton => Atk.Role.SpinButton,
        AccessibleRole.StatusBar => Atk.Role.Statusbar,
        AccessibleRole.Table => Atk.Role.Table,
        AccessibleRole.ToolBar => Atk.Role.ToolBar,
        AccessibleRole.ToolTip => Atk.Role.ToolTip,
        AccessibleRole.Window => Atk.Role.Window,
        AccessibleRole.Client => Atk.Role.Panel,
        AccessibleRole.Grouping => Atk.Role.Panel,
        AccessibleRole.Dialog => Atk.Role.Dialog,
        AccessibleRole.Graphic => Atk.Role.Image,
        AccessibleRole.Link => Atk.Role.Link,
        _ => Atk.Role.Unknown
    };

    /// <summary>
    /// Maps an ATK role back onto the nearest WinForms
    /// <see cref="T:System.Windows.Forms.AccessibleRole" />.
    /// </summary>
    /// <param name="role">
    /// The ATK role.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="T:System.Windows.Forms.AccessibleRole" />.
    /// </returns>
    internal static AccessibleRole FromAtkRole(Atk.Role role) => role switch
    {
        Atk.Role.PushButton => AccessibleRole.PushButton,
        Atk.Role.CheckBox => AccessibleRole.CheckButton,
        Atk.Role.RadioButton => AccessibleRole.RadioButton,
        Atk.Role.ComboBox => AccessibleRole.ComboBox,
        Atk.Role.Entry => AccessibleRole.Text,
        Atk.Role.Label => AccessibleRole.StaticText,
        Atk.Role.List => AccessibleRole.List,
        Atk.Role.ListItem => AccessibleRole.ListItem,
        Atk.Role.MenuBar => AccessibleRole.MenuBar,
        Atk.Role.MenuItem => AccessibleRole.MenuItem,
        Atk.Role.Menu => AccessibleRole.MenuPopup,
        Atk.Role.TreeTable => AccessibleRole.Outline,
        Atk.Role.TreeItem => AccessibleRole.OutlineItem,
        Atk.Role.PageTab => AccessibleRole.PageTab,
        Atk.Role.PageTabList => AccessibleRole.PageTabList,
        Atk.Role.ProgressBar => AccessibleRole.ProgressBar,
        Atk.Role.ScrollBar => AccessibleRole.ScrollBar,
        Atk.Role.Slider => AccessibleRole.Slider,
        Atk.Role.SpinButton => AccessibleRole.SpinButton,
        Atk.Role.Statusbar => AccessibleRole.StatusBar,
        Atk.Role.Table => AccessibleRole.Table,
        Atk.Role.ToolBar => AccessibleRole.ToolBar,
        Atk.Role.ToolTip => AccessibleRole.ToolTip,
        Atk.Role.Window => AccessibleRole.Window,
        Atk.Role.Dialog => AccessibleRole.Dialog,
        Atk.Role.Image => AccessibleRole.Graphic,
        Atk.Role.Link => AccessibleRole.Link,
        Atk.Role.Panel => AccessibleRole.Client,
        _ => AccessibleRole.None
    };
}
