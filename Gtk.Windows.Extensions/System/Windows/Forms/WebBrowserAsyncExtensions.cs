using System.Threading;
using System.Threading.Tasks;
using Gtk.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// The asynchronous half of the
/// <see cref="WebBrowser" /> API: the members that can return what the page computed.
/// </summary>
/// <remarks>
/// Real WinForms hosted Internet Explorer in-process, so
/// <see cref="HtmlDocument.InvokeScript(string)" /> and
/// <see cref="HtmlElement.InvokeMember(string)" /> could return a value synchronously. WebKitGTK and
/// WebView2 both run the page in a separate process, so the synchronous members post the call and return
/// <c>null</c> rather than blocking the UI thread on it - see EMULATION-GAPS.md. These are the escape
/// hatch, and they live in this package rather than in the core library because real WinForms has no such
/// members and the library stays a faithful compatibility surface.
/// </remarks>
public static class WebBrowserAsyncExtensions
{
    /// <summary>
    /// Reads the markup of the document currently displayed, from the page rather than from the cached
    /// snapshot.
    /// </summary>
    /// <param name="browser">
    /// The control.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the answer.
    /// </param>
    /// <returns>
    /// The document's markup, or the empty string when nothing has loaded.
    /// </returns>
    public static async Task<string> GetDocumentTextAsync(this WebBrowser browser,
        CancellationToken cancellationToken = default)
    {
        if (browser == null)
        {
            throw new ArgumentNullException(nameof(browser));
        }

        var text = await browser.Backend
            .InvokeScriptAsync("document.documentElement ? document.documentElement.outerHTML : ''",
                cancellationToken)
            .ConfigureAwait(false);

        return text as string ?? string.Empty;
    }

    /// <summary>
    /// Calls a method on the underlying DOM node and returns what it produced.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <param name="parameters">
    /// The arguments. Only values the engine's script bridge can marshal - strings, numbers and booleans -
    /// survive the crossing.
    /// </param>
    /// <returns>
    /// The value the member returned, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static Task<object?> InvokeMemberAsync(this HtmlElement element, string methodName,
        params object[] parameters)
    {
        return element.InvokeMemberAsync(methodName, parameters, CancellationToken.None);
    }

    /// <summary>
    /// Calls a method on the underlying DOM node and returns what it produced.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <param name="parameters">
    /// The arguments.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the answer, not the call: the page has already been asked.
    /// </param>
    /// <returns>
    /// The value the member returned, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static Task<object?> InvokeMemberAsync(this HtmlElement element, string methodName,
        object[] parameters, CancellationToken cancellationToken)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return element.Bridge.InvokeMemberAsync(element.Handle, methodName,
            parameters ?? Array.Empty<object>(), cancellationToken);
    }

    /// <summary>
    /// Evaluates a script in the page and returns its value.
    /// </summary>
    /// <param name="browser">
    /// The control.
    /// </param>
    /// <param name="script">
    /// The script source. This is an expression, not a function name: pass
    /// <c>
    /// doThing()
    /// </c>, not
    /// <c>
    /// doThing
    /// </c>.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the answer.
    /// </param>
    /// <returns>
    /// The value the script produced -
    /// <see cref="double" /> for a number,
    /// <see cref="bool" /> for a boolean,
    /// <see cref="string" /> for a string - or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static Task<object?> InvokeScriptAsync(this WebBrowser browser, string script,
        CancellationToken cancellationToken = default)
    {
        if (browser == null)
        {
            throw new ArgumentNullException(nameof(browser));
        }

        return browser.Backend.InvokeScriptAsync(script, cancellationToken);
    }

    /// <summary>
    /// Re-reads the page into the snapshot the
    /// <c>
    /// Html*
    /// </c> types serve.
    /// </summary>
    /// <param name="browser">
    /// The control.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the new snapshot.
    /// </param>
    /// <returns>
    /// A task that completes once
    /// <see cref="WebBrowser.Document" /> reflects the page again.
    /// </returns>
    /// <remarks>
    /// The snapshot is pulled automatically when a load finishes. Call this after the page has changed
    /// itself - from its own script, or through a
    /// <see cref="HtmlElement" /> setter whose effect cascaded - and the change has to be visible to the
    /// synchronous getters.
    /// </remarks>
    public static Task RefreshDomAsync(this WebBrowser browser, CancellationToken cancellationToken = default)
    {
        if (browser == null)
        {
            throw new ArgumentNullException(nameof(browser));
        }

        return browser.Backend.Dom.RefreshAsync(cancellationToken);
    }
}

/// <summary>
/// The pages
/// <see cref="WebBrowser.GoHome" /> and
/// <see cref="WebBrowser.GoSearch" /> navigate to.
/// </summary>
/// <remarks>
/// Real WinForms reads Internet Explorer's registry settings for these, and mono hard-coded its vendor's
/// pages. Neither is right for a cross-platform library, so both default to
/// <c>
/// about:blank
/// </c> and an application that wants those buttons to go somewhere says where.
/// </remarks>
public static class WebBrowserDefaults
{
    /// <summary>
    /// Gets or sets the page
    /// <see cref="WebBrowser.GoHome" /> navigates to.
    /// </summary>
    /// <value>
    /// The home page. Never
    /// <c>
    /// null
    /// </c>; assigning
    /// <c>
    /// null
    /// </c> restores
    /// <c>
    /// about:blank
    /// </c>.
    /// </value>
    public static Uri HomeUrl
    {
        get => WebBrowser.HomeUrl;
        set => WebBrowser.HomeUrl = value ?? WebViewBackendFactory.BlankUri;
    }

    /// <summary>
    /// Gets or sets the page
    /// <see cref="WebBrowser.GoSearch" /> navigates to.
    /// </summary>
    /// <value>
    /// The search page. Never
    /// <c>
    /// null
    /// </c>; assigning
    /// <c>
    /// null
    /// </c> restores
    /// <c>
    /// about:blank
    /// </c>.
    /// </value>
    public static Uri SearchUrl
    {
        get => WebBrowser.SearchUrl;
        set => WebBrowser.SearchUrl = value ?? WebViewBackendFactory.BlankUri;
    }
}
