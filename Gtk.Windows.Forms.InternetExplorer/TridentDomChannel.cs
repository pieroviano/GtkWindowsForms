using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Gtk.Windows.Forms.WebBrowsing;
using Microsoft.CSharp.RuntimeBinder;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Drives the shared DOM bridge script inside MSHTML and reads values back out of it.
/// </summary>
/// <remarks>
/// The bridge script is the same one the WebKitGTK and WebView2 backends inject. Trident has no host
/// message channel like <c>chrome.webview.postMessage</c>, so values are pulled rather than pushed: the
/// script is injected with <c>IHTMLWindow2.execScript</c>, and a global function is then called through the
/// window's script dispatch (<c>IHTMLDocument2.Script</c>), whose return value crosses back as a VARIANT.
/// Everything runs synchronously on the STA thread the control lives on; the asynchronous signatures are
/// kept so the backend presents the same seam as the out-of-process engines.
///
/// One consequence: the bridge's delegated DOM events are not delivered on this backend - there is no
/// callback channel into the host without an <c>IDocHostUIHandler.GetExternal</c> sink. Document reads,
/// mutations and script invocation all work; raised <c>HtmlElement</c> events do not. Recorded in
/// EMULATION-GAPS.md.
/// </remarks>
internal sealed class TridentDomChannel
{
    private readonly Func<object?> _browser;

    private readonly ComDispatcher _com;

    internal TridentDomChannel(Func<object?> browser, ComDispatcher com)
    {
        _browser = browser;
        _com = com;
    }

    private static string Bootstrap =>
        DomBridgeScript.Source + "\n" +
        "function __gtkwfEval(code){try{return JSON.stringify(eval(code));}catch(e){return 'null';}}\n" +
        "function __gtkwfSnapshot(){try{return " + DomBridgeScript.Global + ".snapshot();}catch(e){return '';}}";

    /// <summary>Injects the bridge script into the current document. Called after each document completes.</summary>
    internal void Inject()
    {
        _com.Send(() =>
        {
            try
            {
                dynamic? window = CurrentWindow();
                window?.execScript(Bootstrap, "JavaScript");
            }
            catch (Exception exception) when (exception is COMException or RuntimeBinderException)
            {
                global::System.Diagnostics.GtkLogger.Debug(
                    "Could not inject the DOM bridge into the IE document.", exception);
            }
        });
    }

    /// <summary>Evaluates a script expression and returns its value in the shapes the other backends produce.</summary>
    internal Task<object?> EvaluateAsync(string script, CancellationToken cancellationToken)
    {
        var json = _com.Send(() => InvokeNamed("__gtkwfEval", script)) as string;
        object? value = string.IsNullOrEmpty(json) || string.Equals(json, "null", StringComparison.Ordinal)
            ? null
            : DomJson.Parse(json!);
        return Task.FromResult(value);
    }

    /// <summary>Pulls a fresh DOM snapshot as the JSON the base reader expects.</summary>
    internal Task<string?> SnapshotJsonAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_com.Send(() => InvokeNamed("__gtkwfSnapshot")) as string);
    }

    /// <summary>Runs a statement for its effect, discarding any result (used for DOM mutations).</summary>
    internal void Execute(string script)
    {
        _com.Send(() =>
        {
            try
            {
                dynamic? window = CurrentWindow();
                window?.execScript(script, "JavaScript");
            }
            catch (Exception exception) when (exception is COMException or RuntimeBinderException)
            {
                // The document was replaced or is not ready; the mutation is dropped (fire-and-forget).
            }
        });
    }

    private object? CurrentWindow()
    {
        var browser = _browser();
        if (browser == null)
        {
            return null;
        }

        dynamic b = browser;
        var document = b.Document;
        return document?.parentWindow;
    }

    private object? InvokeNamed(string name, params object[] args)
    {
        try
        {
            var browser = _browser();
            if (browser == null)
            {
                return null;
            }

            dynamic b = browser;
            object? document = b.Document;
            object? script = document == null ? null : ((dynamic)document).Script;
            if (script == null)
            {
                return null;
            }

            return script.GetType().InvokeMember(
                name, BindingFlags.InvokeMethod, null, script, args, CultureInfo.InvariantCulture);
        }
        catch (Exception exception) when (
            exception is COMException or MissingMethodException or TargetInvocationException or
                RuntimeBinderException)
        {
            // The bridge is not injected yet, or the document went away: no value to return.
            return null;
        }
    }
}
