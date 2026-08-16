// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using System.Text;

namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// One element of a
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomSnapshot" />: the cached state of a single DOM
/// node, as it was when the snapshot was taken.
/// </summary>
/// <remarks>
/// A node is addressed by its
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.Handle" />, an integer key into a registry the
/// injected page script maintains, so that a later mutation or member invocation reaches the same live
/// node without re-querying it by selector.
/// </remarks>
public sealed class DomNode
{
    /// <summary>
    /// The handle value that means "no node": used by
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.ParentHandle" /> and
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.OffsetParentHandle" /> when there is none.
    /// </summary>
    public const int NoHandle = -1;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode" /> class.
    /// </summary>
    /// <param name="handle">
    /// The registry key that addresses this node in the page.
    /// </param>
    public DomNode(int handle)
    {
        Handle = handle;
        Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ChildHandles = new List<int>();
    }

    /// <summary>
    /// Gets the attributes carried by this node, keyed case-insensitively as HTML attribute names are.
    /// </summary>
    /// <value>
    /// The attribute map. Never
    /// <c>
    /// null
    /// </c>.
    /// </value>
    public Dictionary<string, string> Attributes
    {
        get;
    }

    /// <summary>
    /// Gets the handles of this node's element children, in document order.
    /// </summary>
    /// <value>
    /// The child handles. Never
    /// <c>
    /// null
    /// </c>.
    /// </value>
    public List<int> ChildHandles
    {
        get;
    }

    /// <summary>
    /// Gets or sets the node's client rectangle: its content box, at the origin, as
    /// <c>
    /// clientWidth
    /// </c> /
    /// <c>
    /// clientHeight
    /// </c> report it.
    /// </summary>
    /// <value>
    /// The client rectangle.
    /// </value>
    public Rectangle ClientRect
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the node is enabled - the inverse of the DOM
    /// <c>
    /// disabled
    /// </c> property.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the node is enabled.
    /// </value>
    public bool Enabled
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets the registry key that addresses this node in the page.
    /// </summary>
    /// <value>
    /// The node handle.
    /// </value>
    public int Handle
    {
        get;
    }

    /// <summary>
    /// Gets the value of the node's
    /// <c>
    /// id
    /// </c> attribute.
    /// </summary>
    /// <value>
    /// The identifier, or the empty string when the node carries none.
    /// </value>
    public string Id => GetAttribute("id");

    /// <summary>
    /// Gets or sets the node's inner HTML.
    /// </summary>
    /// <value>
    /// The inner HTML.
    /// </value>
    public string InnerHtml
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the node's inner text.
    /// </summary>
    /// <value>
    /// The inner text.
    /// </value>
    public string InnerText
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets the value of the node's
    /// <c>
    /// name
    /// </c> attribute.
    /// </summary>
    /// <value>
    /// The name, or the empty string when the node carries none.
    /// </value>
    public string Name => GetAttribute("name");

    /// <summary>
    /// Gets or sets the node's offset parent, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.NoHandle" /> when it has none.
    /// </summary>
    /// <value>
    /// The offset parent handle.
    /// </value>
    public int OffsetParentHandle
    {
        get;
        set;
    } = NoHandle;

    /// <summary>
    /// Gets or sets the node's offset rectangle: position and size relative to
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.OffsetParentHandle" />.
    /// </summary>
    /// <value>
    /// The offset rectangle.
    /// </value>
    public Rectangle OffsetRect
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node's outer HTML - the node's own markup, including its tag.
    /// </summary>
    /// <value>
    /// The outer HTML.
    /// </value>
    public string OuterHtml
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the node's outer text. The DOM has no
    /// <c>
    /// outerText
    /// </c> getter outside Internet Explorer, so this mirrors
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.InnerText" />.
    /// </summary>
    /// <value>
    /// The outer text.
    /// </value>
    public string OuterText
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the node's parent, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.NoHandle" /> when it has none.
    /// </summary>
    /// <value>
    /// The parent handle.
    /// </value>
    public int ParentHandle
    {
        get;
        set;
    } = NoHandle;

    /// <summary>
    /// Gets or sets the node's horizontal scroll offset.
    /// </summary>
    /// <value>
    /// The scroll offset, in CSS pixels.
    /// </value>
    public int ScrollLeft
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node's scroll rectangle: the current scroll offset and the full scrollable size.
    /// </summary>
    /// <value>
    /// The scroll rectangle.
    /// </value>
    public Rectangle ScrollRect
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node's vertical scroll offset.
    /// </summary>
    /// <value>
    /// The scroll offset, in CSS pixels.
    /// </value>
    public int ScrollTop
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node's inline style declaration, as its
    /// <c>
    /// style
    /// </c> attribute spells it.
    /// </summary>
    /// <value>
    /// The inline style.
    /// </value>
    public string Style
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the node's tab index.
    /// </summary>
    /// <value>
    /// The tab index.
    /// </value>
    public int TabIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node's tag name, upper-cased as the DOM reports it for HTML documents.
    /// </summary>
    /// <value>
    /// The tag name.
    /// </value>
    public string TagName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Reads one attribute.
    /// </summary>
    /// <param name="attributeName">
    /// The attribute name, matched case-insensitively.
    /// </param>
    /// <returns>
    /// The attribute value, or the empty string when the node does not carry it - which is what
    /// <see cref="!:System.Windows.Forms.HtmlElement.GetAttribute(System.String)" /> returns for a missing
    /// attribute.
    /// </returns>
    public string GetAttribute(string attributeName)
    {
        if (attributeName == null)
        {
            return string.Empty;
        }

        return Attributes.TryGetValue(attributeName, out var value) ? value : string.Empty;
    }

    /// <summary>
    /// Determines whether the node carries an attribute.
    /// </summary>
    /// <param name="attributeName">
    /// The attribute name, matched case-insensitively.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when the attribute is present.
    /// </returns>
    public bool HasAttribute(string attributeName)
    {
        return attributeName != null && Attributes.ContainsKey(attributeName);
    }
}

/// <summary>
/// The cached document model the
/// <c>
/// Html*
/// </c> types read.
/// </summary>
/// <remarks>
/// Every synchronous getter on
/// <see cref="!:System.Windows.Forms.HtmlDocument" /> and
/// <see cref="!:System.Windows.Forms.HtmlElement" /> reads this and nothing else, so no getter can block
/// the UI thread waiting for the web engine's content process. The price is that a snapshot goes stale
/// the moment the page mutates itself; see
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IHtmlDomBridge.RefreshAsync(System.Threading.CancellationToken)" />.
/// </remarks>
public sealed class DomSnapshot
{
    private readonly Dictionary<int, DomNode> _nodes;

    /// <summary>
    /// Initializes a new, empty instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomSnapshot" /> class.
    /// </summary>
    public DomSnapshot()
    {
        _nodes = new Dictionary<int, DomNode>();
    }

    /// <summary>
    /// Gets the empty snapshot: what a bridge reports before any page has loaded.
    /// </summary>
    /// <value>
    /// A shared, empty snapshot.
    /// </value>
    public static DomSnapshot Empty
    {
        get;
    } = new DomSnapshot();

    /// <summary>
    /// Gets or sets the handle of the element that currently has focus.
    /// </summary>
    /// <value>
    /// The active element handle, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int ActiveElementHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets or sets the document's
    /// <c>
    /// alinkColor
    /// </c>, in whatever notation the DOM reported it.
    /// </summary>
    /// <value>
    /// The active link colour.
    /// </value>
    public string ActiveLinkColor
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the document's
    /// <c>
    /// bgColor
    /// </c>, in whatever notation the DOM reported it.
    /// </summary>
    /// <value>
    /// The background colour.
    /// </value>
    public string BackColor
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the handle of the document's
    /// <c>
    /// body
    /// </c> element.
    /// </summary>
    /// <value>
    /// The body handle, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int BodyHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets or sets the document's character set.
    /// </summary>
    /// <value>
    /// The character set name.
    /// </value>
    public string Charset
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the document's cookie string.
    /// </summary>
    /// <value>
    /// The cookie string.
    /// </value>
    public string Cookie
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the document's doctype.
    /// </summary>
    /// <value>
    /// The doctype name.
    /// </value>
    public string DocTypeName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the handle of the document's root element - its
    /// <c>
    /// html
    /// </c> node.
    /// </summary>
    /// <value>
    /// The document element handle, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int DocumentElementHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets or sets the document's domain.
    /// </summary>
    /// <value>
    /// The domain.
    /// </value>
    public string Domain
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the document's
    /// <c>
    /// fgColor
    /// </c>, in whatever notation the DOM reported it.
    /// </summary>
    /// <value>
    /// The foreground colour.
    /// </value>
    public string ForeColor
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the snapshot holds no nodes at all.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when nothing has been captured yet.
    /// </value>
    public bool IsEmpty => _nodes.Count == 0;

    /// <summary>
    /// Gets or sets the document's
    /// <c>
    /// linkColor
    /// </c>, in whatever notation the DOM reported it.
    /// </summary>
    /// <value>
    /// The link colour.
    /// </value>
    public string LinkColor
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets every node in the snapshot, keyed by handle.
    /// </summary>
    /// <value>
    /// The node map.
    /// </value>
    public IReadOnlyDictionary<int, DomNode> Nodes => _nodes;

    /// <summary>
    /// Gets or sets the document's title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the document's URL.
    /// </summary>
    /// <value>
    /// The URL, as text.
    /// </value>
    public string Url
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the document's
    /// <c>
    /// vlinkColor
    /// </c>, in whatever notation the DOM reported it.
    /// </summary>
    /// <value>
    /// The visited link colour.
    /// </value>
    public string VisitedLinkColor
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Parses a snapshot from the JSON the injected page script produces.
    /// </summary>
    /// <param name="json">
    /// The JSON document.
    /// </param>
    /// <returns>
    /// The parsed snapshot, or
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomSnapshot.Empty" /> when
    /// <paramref name="json" /> is blank or is not the object the script emits. Malformed input never
    /// throws: a snapshot is a cache, and a failed refresh must leave the previous one in place rather
    /// than fault the caller.
    /// </returns>
    public static DomSnapshot FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Empty;
        }

        if (!(DomJson.Parse(json!) is Dictionary<string, object?> root))
        {
            return Empty;
        }

        var snapshot = new DomSnapshot
        {
            Title = DomJson.String(root, "title"),
            Url = DomJson.String(root, "url"),
            Charset = DomJson.String(root, "charset"),
            Cookie = DomJson.String(root, "cookie"),
            Domain = DomJson.String(root, "domain"),
            DocTypeName = DomJson.String(root, "docType"),
            BackColor = DomJson.String(root, "backColor"),
            ForeColor = DomJson.String(root, "foreColor"),
            LinkColor = DomJson.String(root, "linkColor"),
            ActiveLinkColor = DomJson.String(root, "activeLinkColor"),
            VisitedLinkColor = DomJson.String(root, "visitedLinkColor"),
            DocumentElementHandle = DomJson.Int(root, "documentElement", DomNode.NoHandle),
            BodyHandle = DomJson.Int(root, "body", DomNode.NoHandle),
            ActiveElementHandle = DomJson.Int(root, "activeElement", DomNode.NoHandle)
        };

        if (root.TryGetValue("nodes", out var nodesValue) && nodesValue is List<object?> nodes)
        {
            foreach (var entry in nodes)
            {
                if (entry is Dictionary<string, object?> nodeObject)
                {
                    var node = ReadNode(nodeObject);
                    if (node != null)
                    {
                        snapshot.Add(node);
                    }
                }
            }
        }

        return snapshot;
    }

    /// <summary>
    /// Adds a node to the snapshot, replacing any node already held under the same handle.
    /// </summary>
    /// <param name="node">
    /// The node to add.
    /// </param>
    public void Add(DomNode node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        _nodes[node.Handle] = node;
    }

    /// <summary>
    /// Enumerates every node below a subtree root, in document order, excluding the root itself.
    /// </summary>
    /// <param name="handle">
    /// The handle of the subtree root.
    /// </param>
    /// <returns>
    /// The descendants. Empty when the handle names no node.
    /// </returns>
    public IEnumerable<DomNode> Descendants(int handle)
    {
        var node = Find(handle);
        if (node == null)
        {
            yield break;
        }

        foreach (var childHandle in node.ChildHandles)
        {
            var child = Find(childHandle);
            if (child == null)
            {
                continue;
            }

            yield return child;

            foreach (var grandChild in Descendants(childHandle))
            {
                yield return grandChild;
            }
        }
    }

    /// <summary>
    /// Looks a node up by handle.
    /// </summary>
    /// <param name="handle">
    /// The node handle.
    /// </param>
    /// <returns>
    /// The node, or
    /// <c>
    /// null
    /// </c> when the snapshot does not hold it.
    /// </returns>
    public DomNode? Find(int handle)
    {
        return _nodes.TryGetValue(handle, out var node) ? node : null;
    }

    /// <summary>
    /// Enumerates the element children of a node, in document order.
    /// </summary>
    /// <param name="handle">
    /// The handle of the parent node.
    /// </param>
    /// <returns>
    /// The children. Empty when the handle names no node.
    /// </returns>
    public IEnumerable<DomNode> GetChildren(int handle)
    {
        var node = Find(handle);
        if (node == null)
        {
            yield break;
        }

        foreach (var childHandle in node.ChildHandles)
        {
            var child = Find(childHandle);
            if (child != null)
            {
                yield return child;
            }
        }
    }

    /// <summary>
    /// Finds the first node carrying an
    /// <c>
    /// id
    /// </c> attribute equal to
    /// <paramref name="id" />.
    /// </summary>
    /// <param name="id">
    /// The identifier, matched case-sensitively as
    /// <c>
    /// document.getElementById
    /// </c> does.
    /// </param>
    /// <returns>
    /// The node, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public DomNode? GetElementById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        foreach (var node in InDocumentOrder())
        {
            if (string.Equals(node.Id, id, StringComparison.Ordinal))
            {
                return node;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds every node under a subtree root whose tag matches
    /// <paramref name="tagName" />.
    /// </summary>
    /// <param name="handle">
    /// The handle of the subtree root. Its own tag is not considered, matching
    /// <c>
    /// Element.getElementsByTagName
    /// </c>.
    /// </param>
    /// <param name="tagName">
    /// The tag name, matched case-insensitively.
    /// <c>
    /// *
    /// </c> matches every element.
    /// </param>
    /// <returns>
    /// The matching nodes, in document order.
    /// </returns>
    public IEnumerable<DomNode> GetElementsByTagName(int handle, string tagName)
    {
        var all = string.Equals(tagName, "*", StringComparison.Ordinal);

        foreach (var node in Descendants(handle))
        {
            if (all || string.Equals(node.TagName, tagName, StringComparison.OrdinalIgnoreCase))
            {
                yield return node;
            }
        }
    }

    /// <summary>
    /// Enumerates every node reachable from the document element, in document order, the document
    /// element included.
    /// </summary>
    /// <returns>
    /// The nodes.
    /// </returns>
    public IEnumerable<DomNode> InDocumentOrder()
    {
        var root = Find(DocumentElementHandle);
        if (root == null)
        {
            yield break;
        }

        yield return root;

        foreach (var node in Descendants(DocumentElementHandle))
        {
            yield return node;
        }
    }

    private static DomNode? ReadNode(Dictionary<string, object?> source)
    {
        var handle = DomJson.Int(source, "handle", DomNode.NoHandle);
        if (handle == DomNode.NoHandle)
        {
            return null;
        }

        var node = new DomNode(handle)
        {
            TagName = DomJson.String(source, "tagName"),
            InnerText = DomJson.String(source, "innerText"),
            InnerHtml = DomJson.String(source, "innerHtml"),
            OuterHtml = DomJson.String(source, "outerHtml"),
            OuterText = DomJson.String(source, "innerText"),
            Style = DomJson.String(source, "style"),
            TabIndex = DomJson.Int(source, "tabIndex", 0),
            Enabled = !DomJson.Bool(source, "disabled"),
            ScrollLeft = DomJson.Int(source, "scrollLeft", 0),
            ScrollTop = DomJson.Int(source, "scrollTop", 0),
            ParentHandle = DomJson.Int(source, "parent", DomNode.NoHandle),
            OffsetParentHandle = DomJson.Int(source, "offsetParent", DomNode.NoHandle),
            ClientRect = new Rectangle(0, 0, DomJson.Int(source, "clientWidth", 0), DomJson.Int(source, "clientHeight", 0)),
            OffsetRect = new Rectangle(DomJson.Int(source, "offsetLeft", 0), DomJson.Int(source, "offsetTop", 0),
                DomJson.Int(source, "offsetWidth", 0), DomJson.Int(source, "offsetHeight", 0)),
            ScrollRect = new Rectangle(DomJson.Int(source, "scrollLeft", 0), DomJson.Int(source, "scrollTop", 0),
                DomJson.Int(source, "scrollWidth", 0), DomJson.Int(source, "scrollHeight", 0))
        };

        if (source.TryGetValue("attributes", out var attributesValue) &&
            attributesValue is Dictionary<string, object?> attributes)
        {
            foreach (var pair in attributes)
            {
                node.Attributes[pair.Key] = pair.Value?.ToString() ?? string.Empty;
            }
        }

        if (source.TryGetValue("children", out var childrenValue) && childrenValue is List<object?> children)
        {
            foreach (var child in children)
            {
                node.ChildHandles.Add(DomJson.ToInt(child, DomNode.NoHandle));
            }
        }

        return node;
    }
}

/// <summary>
/// A minimal JSON reader, enough for the object the injected page script emits.
/// </summary>
/// <remarks>
/// The shipped library is
/// <c>
/// netstandard2.0
/// </c> and deliberately carries no serializer dependency, so this is hand-rolled rather than
/// <c>
/// System.Text.Json
/// </c>: adding a package reference for one internal parser would land on every consumer of the library,
/// including those who never construct a
/// <see cref="!:System.Windows.Forms.WebBrowser" />.
/// </remarks>
internal static class DomJson
{
    /// <summary>
    /// Reads a boolean member.
    /// </summary>
    /// <param name="source">
    /// The object to read from.
    /// </param>
    /// <param name="name">
    /// The member name.
    /// </param>
    /// <returns>
    /// The value, or
    /// <c>
    /// false
    /// </c> when it is absent or is not a boolean.
    /// </returns>
    internal static bool Bool(Dictionary<string, object?> source, string name)
    {
        return source.TryGetValue(name, out var value) && value is bool flag && flag;
    }

    /// <summary>
    /// Reads an integer member.
    /// </summary>
    /// <param name="source">
    /// The object to read from.
    /// </param>
    /// <param name="name">
    /// The member name.
    /// </param>
    /// <param name="fallback">
    /// What to return when the member is absent or is not a number.
    /// </param>
    /// <returns>
    /// The value, or
    /// <paramref name="fallback" />.
    /// </returns>
    internal static int Int(Dictionary<string, object?> source, string name, int fallback)
    {
        return source.TryGetValue(name, out var value) ? ToInt(value, fallback) : fallback;
    }

    /// <summary>
    /// Parses a JSON document.
    /// </summary>
    /// <param name="json">
    /// The document text.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.Collections.Generic.Dictionary`2" /> for an object, a
    /// <see cref="T:System.Collections.Generic.List`1" /> for an array, a
    /// <see cref="T:System.String" />,
    /// <see cref="T:System.Double" /> or
    /// <see cref="T:System.Boolean" /> for a scalar, or
    /// <c>
    /// null
    /// </c> for
    /// <c>
    /// null
    /// </c> and for malformed input.
    /// </returns>
    internal static object? Parse(string json)
    {
        var position = 0;
        try
        {
            var value = ReadValue(json, ref position);
            SkipWhitespace(json, ref position);
            return position == json.Length ? value : null;
        }
        catch (FormatException)
        {
            return null;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    /// <summary>
    /// Reads a string member.
    /// </summary>
    /// <param name="source">
    /// The object to read from.
    /// </param>
    /// <param name="name">
    /// The member name.
    /// </param>
    /// <returns>
    /// The value, or the empty string when the member is absent or is
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    internal static string String(Dictionary<string, object?> source, string name)
    {
        if (!source.TryGetValue(name, out var value) || value == null)
        {
            return string.Empty;
        }

        return value as string ?? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    /// Coerces a parsed JSON value to an integer.
    /// </summary>
    /// <param name="value">
    /// The parsed value.
    /// </param>
    /// <param name="fallback">
    /// What to return when the value is not a number.
    /// </param>
    /// <returns>
    /// The value, or
    /// <paramref name="fallback" />.
    /// </returns>
    internal static int ToInt(object? value, int fallback)
    {
        switch (value)
        {
            case double number:
                return (int)Math.Round(number, MidpointRounding.AwayFromZero);
            case string text when double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed):
                return (int)Math.Round(parsed, MidpointRounding.AwayFromZero);
            default:
                return fallback;
        }
    }

    private static void Expect(string json, ref int position, char expected)
    {
        SkipWhitespace(json, ref position);
        if (position >= json.Length || json[position] != expected)
        {
            throw new FormatException();
        }

        position++;
    }

    private static List<object?> ReadArray(string json, ref int position)
    {
        var items = new List<object?>();
        Expect(json, ref position, '[');
        SkipWhitespace(json, ref position);

        if (position < json.Length && json[position] == ']')
        {
            position++;
            return items;
        }

        while (true)
        {
            items.Add(ReadValue(json, ref position));
            SkipWhitespace(json, ref position);
            if (position >= json.Length)
            {
                throw new FormatException();
            }

            if (json[position] == ',')
            {
                position++;
                continue;
            }

            Expect(json, ref position, ']');
            return items;
        }
    }

    private static object ReadLiteral(string json, ref int position)
    {
        if (string.CompareOrdinal(json, position, "true", 0, 4) == 0)
        {
            position += 4;
            return true;
        }

        if (string.CompareOrdinal(json, position, "false", 0, 5) == 0)
        {
            position += 5;
            return false;
        }

        throw new FormatException();
    }

    private static double ReadNumber(string json, ref int position)
    {
        var start = position;
        while (position < json.Length && "+-0123456789.eE".IndexOf(json[position]) >= 0)
        {
            position++;
        }

        if (!double.TryParse(json.Substring(start, position - start), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var number))
        {
            throw new FormatException();
        }

        return number;
    }

    private static Dictionary<string, object?> ReadObject(string json, ref int position)
    {
        var result = new Dictionary<string, object?>(StringComparer.Ordinal);
        Expect(json, ref position, '{');
        SkipWhitespace(json, ref position);

        if (position < json.Length && json[position] == '}')
        {
            position++;
            return result;
        }

        while (true)
        {
            SkipWhitespace(json, ref position);
            var name = ReadString(json, ref position);
            Expect(json, ref position, ':');
            result[name] = ReadValue(json, ref position);
            SkipWhitespace(json, ref position);

            if (position >= json.Length)
            {
                throw new FormatException();
            }

            if (json[position] == ',')
            {
                position++;
                continue;
            }

            Expect(json, ref position, '}');
            return result;
        }
    }

    private static string ReadString(string json, ref int position)
    {
        Expect(json, ref position, '"');
        var builder = new StringBuilder();

        while (position < json.Length)
        {
            var current = json[position++];

            if (current == '"')
            {
                return builder.ToString();
            }

            if (current != '\\')
            {
                builder.Append(current);
                continue;
            }

            if (position >= json.Length)
            {
                break;
            }

            var escape = json[position++];
            switch (escape)
            {
                case '"':
                    builder.Append('"');
                    break;
                case '\\':
                    builder.Append('\\');
                    break;
                case '/':
                    builder.Append('/');
                    break;
                case 'b':
                    builder.Append('\b');
                    break;
                case 'f':
                    builder.Append('\f');
                    break;
                case 'n':
                    builder.Append('\n');
                    break;
                case 'r':
                    builder.Append('\r');
                    break;
                case 't':
                    builder.Append('\t');
                    break;
                case 'u':
                    if (position + 4 > json.Length ||
                        !int.TryParse(json.Substring(position, 4), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out var code))
                    {
                        throw new FormatException();
                    }

                    builder.Append((char)code);
                    position += 4;
                    break;
                default:
                    throw new FormatException();
            }
        }

        throw new FormatException();
    }

    private static object? ReadValue(string json, ref int position)
    {
        SkipWhitespace(json, ref position);
        if (position >= json.Length)
        {
            throw new FormatException();
        }

        switch (json[position])
        {
            case '{':
                return ReadObject(json, ref position);
            case '[':
                return ReadArray(json, ref position);
            case '"':
                return ReadString(json, ref position);
            case 'n':
                if (string.CompareOrdinal(json, position, "null", 0, 4) != 0)
                {
                    throw new FormatException();
                }

                position += 4;
                return null;
            case 't':
            case 'f':
                return ReadLiteral(json, ref position);
            default:
                return ReadNumber(json, ref position);
        }
    }

    private static void SkipWhitespace(string json, ref int position)
    {
        while (position < json.Length && char.IsWhiteSpace(json[position]))
        {
            position++;
        }
    }
}
