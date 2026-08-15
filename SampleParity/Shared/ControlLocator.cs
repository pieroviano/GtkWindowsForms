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
    /// Enumerates every control reachable from a form, including those a container keeps outside its
    /// <c>Controls</c> collection.
    /// </summary>
    /// <param name="form">The form to walk.</param>
    /// <returns>Every reachable control.</returns>
    /// <remarks>
    /// Used for <em>lookup</em>, not for the structural census. A <c>SplitContainer</c> keeps its content in
    /// <c>Panel1</c>/<c>Panel2</c> and a <c>TabControl</c> in <c>TabPages</c>, and neither shim surfaces
    /// those through <c>Controls</c> - so most of Form1 and Form4 is unaddressable without following them.
    /// The census deliberately does not, because walking that deep also enumerates the internal sub-widgets
    /// of composite controls, which differ between the two engines for reasons that are not defects.
    /// </remarks>
    public static IEnumerable<Control> AllReachableControls(Form form)
    {
        return WalkDeep(form.Controls);
    }

    private static IEnumerable<Control> WalkDeep(Control.ControlCollection? controls)
    {
        if (controls == null)
        {
            yield break;
        }

        foreach (var entry in (System.Collections.IEnumerable)controls)
        {
            if (entry is not Control child)
            {
                continue;
            }

            yield return child;

            foreach (var descendant in WalkDeep(child.Controls))
            {
                yield return descendant;
            }

            foreach (var descendant in WalkSideContainers(child))
            {
                yield return descendant;
            }
        }
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

        foreach (var entry in (System.Collections.IEnumerable)controls)
        {
            // Defensive: a collection may hold something that is not a Control (or a null), and a hard cast
            // in the enumerator would abort the whole scenario with an InvalidCastException.
            if (entry is not Control child)
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

    /// <summary>
    /// Follows containers that hold their children somewhere other than <c>Controls</c>.
    /// </summary>
    /// <param name="control">The control to inspect.</param>
    /// <returns>The children reachable only through those properties, and their descendants.</returns>
    /// <remarks>
    /// A <c>SplitContainer</c> keeps its content in <c>Panel1</c>/<c>Panel2</c> and a <c>TabControl</c> in
    /// <c>TabPages</c>; neither shim surfaces those through <c>Controls</c>, so a plain recursive walk stops
    /// at the container and everything inside it is invisible. That is a shared limitation rather than a
    /// port regression - it behaves identically on both engines - but without following these the catalogue
    /// could not address most of Form1 and Form4 at all.
    /// </remarks>
    private static IEnumerable<Control> WalkSideContainers(Control control)
    {
        foreach (var name in new[] { "Panel1", "Panel2" })
        {
            var property = control.GetType().GetProperty(name);

            if (property == null || !typeof(Control).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            Control? panel = null;
            try
            {
                panel = property.GetValue(control) as Control;
            }
            catch (Exception)
            {
                // A container that cannot produce its panel simply contributes nothing.
            }

            if (panel == null)
            {
                continue;
            }

            yield return panel;

            foreach (var descendant in WalkDeep(panel.Controls))
            {
                yield return descendant;
            }
        }

        foreach (var page in TabPagesOf(control))
        {
            yield return page;

            foreach (var descendant in WalkDeep(page.Controls))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<Control> TabPagesOf(Control control)
    {
        var property = control.GetType().GetProperty("TabPages");
        if (property == null)
        {
            yield break;
        }

        object? value = null;
        try
        {
            value = property.GetValue(control);
        }
        catch (Exception)
        {
            yield break;
        }

        if (value is not System.Collections.IEnumerable pages)
        {
            yield break;
        }

        foreach (var page in pages)
        {
            if (page is Control child)
            {
                yield return child;
            }
        }
    }

    private static Control? FindControlByName(Control.ControlCollection? controls, string name)
    {
        return WalkDeep(controls).FirstOrDefault(c => Matches(c.Name, name));
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
