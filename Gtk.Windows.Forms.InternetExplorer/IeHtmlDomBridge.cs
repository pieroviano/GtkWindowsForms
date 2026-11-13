using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Gtk.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// The DOM bridge over a Trident document: the same injected script the other backends use, driven through
/// <see cref="TridentDomChannel" /> rather than an out-of-process script channel.
/// </summary>
internal sealed class IeHtmlDomBridge : IHtmlDomBridge
{
    internal IeHtmlDomBridge(InternetExplorerBackend backend, ComDispatcher com)
    {
        Channel = new TridentDomChannel(() => backend.Browser, com);
    }

    /// <summary>Gets the channel the backend also uses for <c>InvokeScript</c> and snapshot refreshes.</summary>
    internal TridentDomChannel Channel { get; }

    /// <inheritdoc />
    public DomSnapshot Snapshot { get; private set; } = DomSnapshot.Empty;

    /// <inheritdoc />
    public async Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args,
        CancellationToken cancellationToken)
    {
        var script = string.Format(CultureInfo.InvariantCulture, "{0}.invoke({1}, {2}, {3})",
            DomBridgeScript.Global, nodeHandle.ToString(CultureInfo.InvariantCulture),
            JavaScriptLiteral.Quote(member), JavaScriptLiteral.Array(args));

        return await Channel.EvaluateAsync(script, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Post(DomMutation mutation)
    {
        if (mutation == null)
        {
            throw new ArgumentNullException(nameof(mutation));
        }

        var request = string.Format(CultureInfo.InvariantCulture,
            "{{ kind: {0}, target: {1}, related: {2}, reference: {3}, name: {4}, value: {5}, intValue: {6}, flag: {7} }}",
            JavaScriptLiteral.Quote(mutation.Kind.ToString()),
            mutation.TargetHandle.ToString(CultureInfo.InvariantCulture),
            mutation.RelatedHandle.ToString(CultureInfo.InvariantCulture),
            mutation.ReferenceHandle.ToString(CultureInfo.InvariantCulture),
            JavaScriptLiteral.Quote(mutation.Name),
            JavaScriptLiteral.Quote(mutation.Value),
            mutation.IntValue.ToString(CultureInfo.InvariantCulture),
            mutation.Flag ? "true" : "false");

        // Fire-and-forget: Post is called from synchronous WinForms setters.
        Channel.Execute(DomBridgeScript.Global + ".mutate(" + request + ")");
    }

    /// <inheritdoc />
    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var json = await Channel.SnapshotJsonAsync(cancellationToken).ConfigureAwait(false);
        Replace(DomSnapshot.FromJson(json));
    }

    internal void RaiseDomEvent(DomEventArgs e) => DomEvent?.Invoke(this, e);

    internal void Replace(DomSnapshot snapshot)
    {
        Snapshot = snapshot;
        SnapshotUpdated?.Invoke(this, global::System.EventArgs.Empty);
    }

    /// <inheritdoc />
    public event global::System.EventHandler<DomEventArgs>? DomEvent;

    /// <inheritdoc />
    public event global::System.EventHandler? SnapshotUpdated;
}
