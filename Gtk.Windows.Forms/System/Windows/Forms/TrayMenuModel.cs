using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// One entry in a <see cref="TrayMenuModel" />: a backend-neutral snapshot of a
/// <see cref="ToolStripItem" />.
/// </summary>
internal sealed class TrayMenuNode
{
    /// <summary>
    /// Gets or sets the identifier assigned by <see cref="TrayMenuModel" />, unique within its model.
    /// </summary>
    /// <value>0 for the synthetic root; a positive depth-first ordinal for every real item.</value>
    public int Id { get; set; }

    /// <summary>Gets or sets the item text, with the WinForms <c>&amp;</c> mnemonic preserved verbatim.</summary>
    /// <remarks>
    /// Mnemonic syntax differs per platform, so conversion happens at emit time in each backend - see
    /// <see cref="TrayMenuModel.ToDBusMenuLabel" /> and <see cref="TrayMenuModel.StripMnemonics" />.
    /// </remarks>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the item is selectable.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether the item is a separator line.</summary>
    public bool IsSeparator { get; set; }

    /// <summary>Gets or sets a value indicating whether the item carries a check mark.</summary>
    public bool IsCheckable { get; set; }

    /// <summary>Gets or sets a value indicating whether the item's check mark is set.</summary>
    public bool Checked { get; set; }

    /// <summary>Gets the child items, empty when the item is not a submenu.</summary>
    public List<TrayMenuNode> Children { get; } = new List<TrayMenuNode>();

    /// <summary>Gets or sets the item this node was built from, or <c>null</c> for the synthetic root.</summary>
    public ToolStripItem? Source { get; set; }

    /// <summary>Gets a value indicating whether this node has children.</summary>
    public bool HasChildren => Children.Count > 0;
}

/// <summary>
/// A backend-neutral snapshot of a <see cref="ContextMenuStrip" />, so that the three platform trays share
/// one interpretation of WinForms menu semantics instead of each re-deriving it.
/// </summary>
/// <remarks>
/// <para>
/// Only the structural essentials survive the translation - text, enabled, checked, separators and
/// submenus. Item images, fonts, colours and owner-draw rendering are deliberately not carried: the
/// <c>com.canonical.dbusmenu</c> and <c>NSMenu</c> models cannot express them faithfully, and a partial
/// implementation that worked on one platform only would be worse than a documented omission.
/// </para>
/// <para>
/// A model is a snapshot, not a live view. Backends rebuild it when the platform asks them to (Windows on
/// right-click, Linux on the dbusmenu <c>AboutToShow</c>, macOS before popping the menu up), so a menu
/// mutated after assignment still renders correctly.
/// </para>
/// </remarks>
internal sealed class TrayMenuModel
{
    private readonly Dictionary<int, TrayMenuNode> _byId = new Dictionary<int, TrayMenuNode>();

    private TrayMenuModel(TrayMenuNode root)
    {
        Root = root;
        Index(root);
    }

    /// <summary>Gets the synthetic root node, whose children are the top-level menu items.</summary>
    public TrayMenuNode Root { get; }

    /// <summary>
    /// Builds a model with no items.
    /// </summary>
    /// <returns>A model whose root has no children.</returns>
    /// <remarks>
    /// Used where a menu is required but none was assigned. It deliberately does not construct a
    /// <see cref="ContextMenuStrip" /> to get there, because that would create Gtk widgets - possibly off
    /// the main loop - purely to represent emptiness.
    /// </remarks>
    public static TrayMenuModel Empty()
    {
        return new TrayMenuModel(new TrayMenuNode { Id = 0 });
    }

    /// <summary>
    /// Builds a model from a <see cref="ContextMenuStrip" />.
    /// </summary>
    /// <param name="menu">The menu to translate, or <c>null</c>.</param>
    /// <returns>The translated model, or <c>null</c> when <paramref name="menu" /> is <c>null</c>.</returns>
    public static TrayMenuModel? From(ContextMenuStrip? menu)
    {
        if (menu == null)
        {
            return null;
        }

        var root = new TrayMenuNode { Id = 0 };
        var nextId = 1;
        AddChildren(root, menu.Items, ref nextId);

        return new TrayMenuModel(root);
    }

    /// <summary>
    /// Finds a node by the identifier a backend received back from the platform.
    /// </summary>
    /// <param name="id">The node identifier.</param>
    /// <returns>The node, or <c>null</c> when the identifier is unknown.</returns>
    public TrayMenuNode? Find(int id)
    {
        return _byId.TryGetValue(id, out var node) ? node : null;
    }

    /// <summary>
    /// Activates the item behind a node, as if the user had clicked it in a WinForms menu.
    /// </summary>
    /// <param name="id">The node identifier reported by the platform.</param>
    /// <remarks>
    /// A no-op for unknown identifiers and for the synthetic root, so a stale identifier from a menu that
    /// has since been rebuilt is ignored rather than misrouted.
    /// </remarks>
    public void Activate(int id)
    {
        var node = Find(id);
        node?.Source?.PerformClick();
    }

    /// <summary>
    /// Converts WinForms mnemonic syntax to the <c>com.canonical.dbusmenu</c> convention.
    /// </summary>
    /// <param name="text">The WinForms item text.</param>
    /// <returns>The label with <c>&amp;</c> mnemonics rewritten as <c>_</c> and literal underscores doubled.</returns>
    /// <remarks>
    /// dbusmenu marks mnemonics with <c>_</c>, so a literal underscore in the text has to be escaped as
    /// <c>__</c> first, or it would silently become a mnemonic. <c>&amp;&amp;</c> is a literal ampersand in
    /// WinForms and collapses to one.
    /// </remarks>
    public static string ToDBusMenuLabel(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(text!.Length + 4);

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (c == '_')
            {
                builder.Append("__");
            }
            else if (c == '&')
            {
                if (i + 1 < text.Length && text[i + 1] == '&')
                {
                    builder.Append('&');
                    i++;
                }
                else
                {
                    builder.Append('_');
                }
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Removes WinForms mnemonic markers, for platforms that have no mnemonic concept.
    /// </summary>
    /// <param name="text">The WinForms item text.</param>
    /// <returns>The text with single <c>&amp;</c> removed and <c>&amp;&amp;</c> collapsed to one.</returns>
    public static string StripMnemonics(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(text!.Length);

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (c == '&')
            {
                if (i + 1 < text.Length && text[i + 1] == '&')
                {
                    builder.Append('&');
                    i++;
                }

                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    private static void AddChildren(TrayMenuNode parent, ToolStripItemCollection? items, ref int nextId)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item == null || !item.Visible)
            {
                // Only dbusmenu has a "visible" property; Win32 menus and NSMenu do not. Dropping the item
                // is the one behaviour all three can agree on.
                continue;
            }

            var node = new TrayMenuNode
            {
                Id = nextId++,
                Source = item,
                Text = item.Text ?? string.Empty,
                Enabled = item.Enabled,
                IsSeparator = item is ToolStripSeparator,
            };

            if (item is ToolStripMenuItem menuItem)
            {
                // An item is checkable when it says so (CheckOnClick) or when it currently carries a check
                // state - the latter covers items whose check mark the application drives itself, which have
                // no other way to advertise that they are checkable.
                node.IsCheckable = menuItem.CheckOnClick || menuItem.CheckState != CheckState.Unchecked;
                node.Checked = menuItem.CheckState == CheckState.Checked;
            }

            parent.Children.Add(node);

            if (!node.IsSeparator)
            {
                AddChildren(node, item.DropDownItems, ref nextId);
            }
        }
    }

    private void Index(TrayMenuNode node)
    {
        _byId[node.Id] = node;

        foreach (var child in node.Children)
        {
            Index(child);
        }
    }
}
