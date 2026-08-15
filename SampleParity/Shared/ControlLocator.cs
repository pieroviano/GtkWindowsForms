using System.Windows.Forms;

namespace SampleParity;

/// <summary>A strip control paired with the items it owns.</summary>
internal readonly struct StripHandle
{
    /// <summary>Initialises a new instance of the <see cref="StripHandle" /> struct.</summary>
    /// <param name="owner">The strip control.</param>
    /// <param name="items">Its item collection.</param>
    public StripHandle(Control owner, ToolStripItemCollection items)
    {
        Owner = owner;
        Items = items;
    }

    /// <summary>Gets the strip control.</summary>
    public Control Owner { get; }

    /// <summary>Gets the strip's items.</summary>
    public ToolStripItemCollection Items { get; }

    /// <summary>Gets the strip's name.</summary>
    public string Name => Owner?.Name ?? string.Empty;
}

/// <summary>
/// Resolves the controls and strip items a scenario addresses, on either sample.
/// </summary>
/// <remarks>
/// <para>
/// Two constraints shape this. The submodule sample is vendored upstream and cannot be given
/// <c>InternalsVisibleTo</c> or internalised Designer fields, so its controls are unreachable by field
/// access and must be found by walking the tree. And the sample transform translates Chinese identifiers,
/// so a minority of names differ between the two samples - on the Gtk4 side some are not even literals but
/// localized resource lookups.
/// </para>
/// <para>
/// Hence two addressing modes. <c>name:</c> matches on <see cref="Control.Name" /> and covers the ~205 of
/// ~218 controls whose names are identical on both sides. <c>path:</c> addresses a strip item by its
/// position (<c>path:menuStrip1/1/1/0</c>), which the transform cannot change, and is what the divergent
/// names use.
/// </para>
/// </remarks>
internal static class ControlLocator
{
    /// <summary>
    /// Finds a control on a form.
    /// </summary>
    /// <param name="form">The form to search.</param>
    /// <param name="address">A <c>name:</c> address.</param>
    /// <returns>The control, or <see langword="null" /> when it does not exist.</returns>
    public static Control? FindControl(Form form, string address)
    {
        var name = StripPrefix(address, "name:");

        return FindControlByName(form.Controls, name);
    }

    /// <summary>
    /// Finds a strip item (menu entry, tool button) on a form.
    /// </summary>
    /// <param name="form">The form to search.</param>
    /// <param name="address">A <c>name:</c> or <c>path:</c> address.</param>
    /// <returns>The item, or <see langword="null" /> when it does not exist.</returns>
    public static ToolStripItem? FindItem(Form form, string address)
    {
        if (address.StartsWith("path:", StringComparison.Ordinal))
        {
            return FindItemByPath(form, StripPrefix(address, "path:"));
        }

        var name = StripPrefix(address, "name:");

        foreach (var strip in AllStrips(form))
        {
            var found = FindItemByName(strip.Items, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>Enumerates every control on a form, depth first.</summary>
    /// <param name="form">The form to walk.</param>
    /// <returns>Every control in the tree.</returns>
    public static IEnumerable<Control> AllControls(Form form)
    {
        return Walk(form.Controls);
    }

    /// <summary>
    /// Enumerates every strip on a form, paired with its items.
    /// </summary>
    /// <param name="form">The form to walk.</param>
    /// <returns>Each strip control and the item collection it owns, ordered by name.</returns>
    /// <remarks>
    /// Strips are identified by <em>having an <c>Items</c> collection</em>, not by their base class. The two
    /// shims disagree on the hierarchy - upstream declares <c>MenuStrip : Control</c> while the Gtk4 port
    /// declares <c>MenuStrip : ToolStrip</c> - so an <c>OfType&lt;ToolStrip&gt;()</c> filter silently drops
    /// the menu bar on one side only, which would make every menu scenario pass vacuously there.
    /// </remarks>
    public static IEnumerable<StripHandle> AllStrips(Form form)
    {
        foreach (var control in Walk(form.Controls))
        {
            var items = ItemsOf(control);
            if (items != null)
            {
                yield return new StripHandle(control, items);
            }
        }
    }

    /// <summary>Reads a control's <c>Items</c> collection, when it has one.</summary>
    /// <param name="control">The control to inspect.</param>
    /// <returns>The collection, or <see langword="null" /> when the control is not a strip.</returns>
    private static ToolStripItemCollection? ItemsOf(Control control)
    {
        var property = control.GetType().GetProperty("Items");

        if (property == null || !typeof(ToolStripItemCollection).IsAssignableFrom(property.PropertyType))
        {
            return null;
        }

        try
        {
            return property.GetValue(control) as ToolStripItemCollection;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static IEnumerable<Control> Walk(Control.ControlCollection? controls)
    {
        if (controls == null)
        {
            yield break;
        }

        foreach (Control child in controls)
        {
            if (child == null)
            {
                continue;
            }

            yield return child;

            foreach (var descendant in Walk(child.Controls))
            {
                yield return descendant;
            }
        }
    }

    private static Control? FindControlByName(Control.ControlCollection? controls, string name)
    {
        if (controls == null)
        {
            return null;
        }

        foreach (Control child in controls)
        {
            if (child == null)
            {
                continue;
            }

            if (Matches(child.Name, name))
            {
                return child;
            }

            var found = FindControlByName(child.Controls, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static ToolStripItem? FindItemByName(ToolStripItemCollection? items, string name)
    {
        if (items == null)
        {
            return null;
        }

        foreach (var item in items)
        {
            if (item == null)
            {
                continue;
            }

            if (Matches(item.Name, name))
            {
                return item;
            }

            var found = FindItemByName(item.DropDownItems, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Resolves an item by position, e.g. <c>menuStrip1/1/1/0</c> - the second top-level entry of
    /// <c>menuStrip1</c>, its second child, that child's first child.
    /// </summary>
    private static ToolStripItem? FindItemByPath(Form form, string path)
    {
        var parts = path.Split('/');
        if (parts.Length < 2)
        {
            return null;
        }

        var strip = AllStrips(form).FirstOrDefault(s => Matches(s.Name, parts[0]));
        if (strip.Items == null)
        {
            return null;
        }

        ToolStripItem? current = null;
        var items = strip.Items;

        for (var i = 1; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out var index) || items == null || index < 0 || index >= items.Count)
            {
                return null;
            }

            current = items[index];
            items = current?.DropDownItems;
        }

        return current;
    }

    /// <summary>
    /// Compares names tolerantly.
    /// </summary>
    /// <remarks>
    /// The transform emits a leading space on translated names (<c>" bbMenuToolStripMenuItem"</c>), which is
    /// an artefact rather than an identity, so names are trimmed before comparison.
    /// </remarks>
    private static bool Matches(string? actual, string expected)
    {
        return string.Equals(actual?.Trim(), expected.Trim(), StringComparison.Ordinal);
    }

    private static string StripPrefix(string address, string prefix)
    {
        return address.StartsWith(prefix, StringComparison.Ordinal) ? address.Substring(prefix.Length) : address;
    }
}
