// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gtk.Windows.Forms.WebBrowsing;

/// <summary>
/// Names the kinds of DOM change a
/// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation" /> can carry.
/// </summary>
public enum DomMutationKind
{
    /// <summary>
    /// Set an attribute named by
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Name" /> to
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Value" />.
    /// </summary>
    SetAttribute,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// innerHTML
    /// </c>.
    /// </summary>
    SetInnerHtml,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// innerText
    /// </c>.
    /// </summary>
    SetInnerText,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// outerHTML
    /// </c>.
    /// </summary>
    SetOuterHtml,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// outerText
    /// </c>.
    /// </summary>
    SetOuterText,

    /// <summary>
    /// Assign the node's inline
    /// <c>
    /// style
    /// </c>.
    /// </summary>
    SetStyle,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// tabIndex
    /// </c>.
    /// </summary>
    SetTabIndex,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// disabled
    /// </c> flag.
    /// </summary>
    SetDisabled,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// scrollLeft
    /// </c>.
    /// </summary>
    SetScrollLeft,

    /// <summary>
    /// Assign the node's
    /// <c>
    /// scrollTop
    /// </c>.
    /// </summary>
    SetScrollTop,

    /// <summary>
    /// Give the node keyboard focus.
    /// </summary>
    Focus,

    /// <summary>
    /// Take keyboard focus away from the node.
    /// </summary>
    Blur,

    /// <summary>
    /// Scroll the node into view.
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Flag" /> selects top alignment.
    /// </summary>
    ScrollIntoView,

    /// <summary>
    /// Dispatch the event named by
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Name" /> at the node.
    /// </summary>
    RaiseEvent,

    /// <summary>
    /// Append the node named by
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.RelatedHandle" /> to the target's
    /// children.
    /// </summary>
    AppendChild,

    /// <summary>
    /// Insert
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.RelatedHandle" /> into the target
    /// before
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.ReferenceHandle" />.
    /// </summary>
    InsertBefore,

    /// <summary>
    /// Create an element whose tag is
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Name" />, detached from the tree, and
    /// register it under
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.TargetHandle" />.
    /// </summary>
    CreateElement,

    /// <summary>
    /// Write
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Value" /> through
    /// <c>
    /// document.write
    /// </c>.
    /// </summary>
    Write,

    /// <summary>
    /// Assign a document-level property named by
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Name" /> - the title, the cookie, or
    /// one of the legacy colour properties.
    /// </summary>
    SetDocumentProperty
}

/// <summary>
/// One posted change to the live page.
/// </summary>
/// <remarks>
/// Mutations are fire-and-forget by design (see
/// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.Post(Gtk.Windows.Forms.WebBrowsing.DomMutation)" />):
/// a WinForms setter must not block the UI thread waiting for the web engine's content process to
/// acknowledge it.
/// </remarks>
public sealed class DomMutation
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation" /> class.
    /// </summary>
    /// <param name="kind">
    /// What the mutation does.
    /// </param>
    /// <param name="targetHandle">
    /// The node the mutation applies to.
    /// </param>
    public DomMutation(DomMutationKind kind, int targetHandle)
    {
        Kind = kind;
        TargetHandle = targetHandle;
    }

    /// <summary>
    /// Gets or sets the mutation's boolean argument -
    /// <c>
    /// disabled
    /// </c> for
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutationKind.SetDisabled" />, top alignment for
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutationKind.ScrollIntoView" />.
    /// </summary>
    /// <value>
    /// The boolean argument.
    /// </value>
    public bool Flag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the mutation's integer argument.
    /// </summary>
    /// <value>
    /// The integer argument.
    /// </value>
    public int IntValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets what the mutation does.
    /// </summary>
    /// <value>
    /// The mutation kind.
    /// </value>
    public DomMutationKind Kind
    {
        get;
    }

    /// <summary>
    /// Gets or sets the name the mutation acts on - an attribute name, an event name, a tag name or a
    /// document property name, depending on
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomMutation.Kind" />.
    /// </summary>
    /// <value>
    /// The name, or
    /// <c>
    /// null
    /// </c> when the kind carries none.
    /// </value>
    public string? Name
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a second node the mutation acts on - the node being appended or inserted.
    /// </summary>
    /// <value>
    /// The related node handle, or
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int RelatedHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets or sets the node an insertion is positioned against.
    /// </summary>
    /// <value>
    /// The reference node handle, or
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int ReferenceHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets the node the mutation applies to.
    /// </summary>
    /// <value>
    /// The target node handle.
    /// </value>
    public int TargetHandle
    {
        get;
    }

    /// <summary>
    /// Gets or sets the mutation's string argument.
    /// </summary>
    /// <value>
    /// The string argument, or
    /// <c>
    /// null
    /// </c> when the kind carries none.
    /// </value>
    public string? Value
    {
        get;
        set;
    }
}

/// <summary>
/// Carries one DOM event from the page up to the
/// <c>
/// Html*
/// </c> layer.
/// </summary>
/// <remarks>
/// This is deliberately not
/// <see cref="T:System.Windows.Forms.HtmlElementEventArgs" />: that type exposes
/// <see cref="!:System.Windows.Forms.HtmlElementEventArgs.FromElement" /> and
/// <see cref="!:System.Windows.Forms.HtmlElementEventArgs.ToElement" /> as
/// <see cref="!:System.Windows.Forms.HtmlElement" />, and an element cannot be built without the
/// <see cref="!:System.Windows.Forms.HtmlDocument" /> that owns it - which the bridge has no knowledge of.
/// The bridge therefore reports handles, and the
/// <c>
/// Html*
/// </c> layer resolves them against its own document.
/// </remarks>
public sealed class DomEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomEventArgs" /> class.
    /// </summary>
    /// <param name="nodeHandle">
    /// The node the event was dispatched at.
    /// </param>
    /// <param name="eventType">
    /// The DOM event type, without the
    /// <c>
    /// on
    /// </c> prefix.
    /// </param>
    public DomEventArgs(int nodeHandle, string eventType)
    {
        NodeHandle = nodeHandle;
        EventType = eventType ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the alt key was down.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when alt was down.
    /// </value>
    public bool AltKeyPressed
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets which mouse buttons were down.
    /// </summary>
    /// <value>
    /// The buttons.
    /// </value>
    public MouseButtons Buttons
    {
        get;
        set;
    } = MouseButtons.None;

    /// <summary>
    /// Gets or sets the mouse position in client coordinates.
    /// </summary>
    /// <value>
    /// The client mouse position.
    /// </value>
    public Point ClientMousePosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control key was down.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when control was down.
    /// </value>
    public bool CtrlKeyPressed
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the DOM event type, without the
    /// <c>
    /// on
    /// </c> prefix.
    /// </summary>
    /// <value>
    /// The event type.
    /// </value>
    public string EventType
    {
        get;
    }

    /// <summary>
    /// Gets or sets the node the pointer came from.
    /// </summary>
    /// <value>
    /// The handle, or
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int FromHandle
    {
        get;
        set;
    } = DomNode.NoHandle;

    /// <summary>
    /// Gets or sets the key code of the key involved.
    /// </summary>
    /// <value>
    /// The key code.
    /// </value>
    public int KeyPressedCode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the mouse position in screen coordinates.
    /// </summary>
    /// <value>
    /// The mouse position.
    /// </value>
    public Point MousePosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the node the event was dispatched at.
    /// </summary>
    /// <value>
    /// The node handle.
    /// </value>
    public int NodeHandle
    {
        get;
    }

    /// <summary>
    /// Gets or sets the mouse position relative to the target node.
    /// </summary>
    /// <value>
    /// The offset mouse position.
    /// </value>
    public Point OffsetMousePosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the shift key was down.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when shift was down.
    /// </value>
    public bool ShiftKeyPressed
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the node the pointer moved to.
    /// </summary>
    /// <value>
    /// The handle, or
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomNode.NoHandle" />.
    /// </value>
    public int ToHandle
    {
        get;
        set;
    } = DomNode.NoHandle;
}

/// <summary>
/// The seam the
/// <c>
/// Html*
/// </c> types sit on: the JavaScript-synthesised DOM that replaces mono's in-process
/// <c>
/// Mono.WebBrowser.DOM.IDocument
/// </c> /
/// <c>
/// IElement
/// </c> /
/// <c>
/// IWindow
/// </c>.
/// </summary>
/// <remarks>
/// WebKitGTK and WebView2 both run the page in a separate process, so there is no in-process DOM object
/// to hand out and no way to read one synchronously. Reads therefore come from
/// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.Snapshot" /> and writes are posted.
/// </remarks>
public interface IHtmlDomBridge
{
    /// <summary>
    /// Gets the last snapshot pulled from the page.
    /// </summary>
    /// <value>
    /// The snapshot. Never
    /// <c>
    /// null
    /// </c>; possibly
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.DomSnapshot.Empty" />.
    /// </value>
    DomSnapshot Snapshot
    {
        get;
    }

    /// <summary>
    /// Invokes a member on a live node.
    /// </summary>
    /// <param name="nodeHandle">
    /// The node to invoke on.
    /// </param>
    /// <param name="member">
    /// The member name.
    /// </param>
    /// <param name="args">
    /// The arguments. Only values the engine's script bridge can marshal - strings, numbers and booleans -
    /// survive the crossing.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the answer, not the call itself: the page has already been asked.
    /// </param>
    /// <returns>
    /// The value the member returned, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args, CancellationToken cancellationToken);

    /// <summary>
    /// Posts a mutation to the page and applies it optimistically to
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.Snapshot" />, so that a value written
    /// through a WinForms setter reads back immediately through the matching getter.
    /// </summary>
    /// <param name="mutation">
    /// The mutation to apply.
    /// </param>
    void Post(DomMutation mutation);

    /// <summary>
    /// Re-pulls the snapshot from the page.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancels the wait for the new snapshot.
    /// </param>
    /// <returns>
    /// A task that completes once
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.Snapshot" /> has been replaced and
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.SnapshotUpdated" /> raised.
    /// </returns>
    Task RefreshAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Occurs when an event fires in the page on a node the bridge is listening to.
    /// </summary>
    event EventHandler<DomEventArgs> DomEvent;

    /// <summary>
    /// Occurs when
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.IHtmlDomBridge.Snapshot" /> has been replaced.
    /// </summary>
    event EventHandler SnapshotUpdated;
}
