# Porting mono's `WebBrowser` control into `Gtk.Windows.Forms`

Plan of record. Written 2026-08-16 against branch `gtk4` @ `eb5cd71b`. **Do not start implementing until asked.**

---

## 0. Blocking verifications — do these *before* any code is written

Each of these can invalidate a design decision below. None is a "risk to monitor"; they are gates.

| # | What to verify | How | If it fails |
|---|---|---|---|
| **B1** | `Net4x.WebKitGtkSharp` and `Net4x.JavaScriptCoreSharp` are **published to a feed the repo consumes**, not just present in the local cache. Today they exist only at `~/.nuget/packages/net4x.webkitgtksharp/4.22.4.26228/` and `…/net4x.javascriptcoresharp/4.22.4.26228/`, and `Nuget.Config` feeds are the local `Packages` symlink + nuget.org. | `dotnet nuget list source`; check `Packages/` for the `.nupkg`; try `dotnet add package Net4x.WebKitGtkSharp` in a scratch project. | Publish them from the `GtkSharp` fork (`..\GtkSharp`, `build.cake`) before §3 starts. `WebkitGtkSharp.csproj` currently has `GeneratePackageOnBuild=False` — the cake pipeline packs it. |
| **B2** | The `netstandard2.0` asset really exists in both packages (the main library is `netstandard2.0`). | Confirmed present at `…/net4x.webkitgtksharp/4.22.4.26228/lib/netstandard2.0/WebkitGtkSharp.dll` — re-confirm for whatever version gets pinned. | Backend must move to the companion-package model too, which changes §2 materially. |
| **B3** | A Linux dev/CI box has **WebKitGTK 6.0** (`libwebkitgtk-6.0.so.4`, WebKitGTK ≥ 2.44). This is `webkit2gtk-6.0`/`libwebkitgtk-6.0-4` on Debian/Ubuntu — *not* the `4.0`/`4.1` packages. | `ldconfig -p \| grep -i webkitgtk` inside the target container/box. | Nothing past the fallback path is testable. Add the package to the CI image first (`bitbucket-pipelines.yml`). |
| **B4** | The Windows GTK4 runtime the repo loads **exports `gdk_win32_surface_get_handle`**, and `Gtk.Window.Surface` on Windows is a `GdkWin32Surface`. The whole Windows backend (§7) rests on this. | `dumpbin /exports libgtk-4-1.dll \| findstr win32_surface` against the GTK4 runtime actually deployed. Note `GLibrary.cs:53` still points `SetDllDirectory` at `%LOCALAPPDATA%\Gtk\3.24.24`, a GTK3 path — find where the GTK4 runtime actually lives. | Windows backend is dead as designed; fall back to the placeholder path (§8) on Windows too, and re-open the round-2 hosting question. |
| **B5** | `Microsoft.Web.WebView2` version to pin, and that its assets work for the companion project's TFM. `microsoft.web.webview2` is already in the local cache — check which version and whether it ships `netstandard2.0`/`net462`. | `ls ~/.nuget/packages/microsoft.web.webview2/*/lib/` | Companion project multi-targets (e.g. `netstandard2.0;net8.0-windows`) instead of a single TFM. |
| **B6** | **Licensing.** mono's `mcs/class/System.Windows.Forms/System.Windows.Forms/*.cs` headers carry the MIT permission grant ("Permission is hereby granted, free of charge…", © 2007–2008 Novell, authors Andreia Gaita). The five NuGet packages currently pack `gtksystem-windows-forms/Source/licenses/LICENSE.txt` = **LGPL v3**, and [CLAUDE.md](CLAUDE.md) records that the licence line is unsettled. Importing ~4 200 lines of MIT-licensed Novell code into an LGPL-packaged assembly needs the MIT attribution shipped. | Decide with counsel/upstream; at minimum add a `THIRD-PARTY-NOTICES` entry to the packed content. | Do not merge. This is the one gate that is not technical. |
| **B7** | `Gtk.Widget.ComputePoint` / `TranslateCoordinates`, `Gtk.Window.Surface` / `Gtk.INative.Surface`, `Gdk.Surface.ScaleFactor` are all bound in the fork. | Already confirmed: `GtkSharp/Generated/Gtk/Widget.cs:2483` (`ComputePoint`), `:3426` (`TranslateCoordinates`), `Gtk/Window.cs:1245` (`Surface`), `Gtk/INative.cs:17`, `GdkSharp/Generated/Gdk/Surface.cs:145`. | — (passed) |

---

## 1. Decisions taken, and what each one costs

| # | Decision | Consequence you are accepting |
|---|---|---|
| D1 | **Full mono `Html*` surface, best-effort.** Every member signature from mono's `HtmlDocument`/`HtmlElement`/`HtmlWindow`/`HtmlElementCollection`/`HtmlWindowCollection`/`HtmlHistory` is ported. What JavaScript can express is implemented; the rest throws `NotSupportedException` with a message naming WebKit2's process split. | Large surface (~250 members) where a visible minority is inert. `API-COMPLETENESS.md` gains 17 types but `EMULATION-GAPS.md` gains a long "present but partial" section. Members that cannot work: `DomDocument`/`DomElement`/`DomWindow`/`DomHistory` (no in-process DOM object to hand out), `HtmlWindow.Open*` (no second `WebView` under one `WebBrowser`), `HtmlDocument.GetElementFromPoint` beyond a JS `elementFromPoint` approximation. |
| D2 | **Add a Windows WebView2 backend** alongside WebKitGTK. | Two engines, two DOM-bridge transports, two sets of quirks. Biggest single cost in this plan; see §7 and the airspace consequence in D5. |
| D3 | **`PackageReference Net4x.WebKitGtkSharp`** in the main library (typed bindings, not P/Invoke). | `Gtk.Windows.Forms` gains a hard dependency on `Net4x.WebKitGtkSharp` + its transitive `Net4x.JavaScriptCoreSharp` for **every** consumer, including those who never touch `WebBrowser`. Managed-only weight; the native `libwebkitgtk-6.0` is still resolved lazily and its absence is a runtime fallback, not a load failure. |
| D4 | **Cached state + explicit async siblings.** No nested main loop; synchronous getters never block the UI thread. | Synchronous WinForms getters can return stale or empty values in the window between a navigation and the next snapshot. This is a *documented behavioural deviation* and belongs in `EMULATION-GAPS.md` §2 ("recorded deviations"), not §1. |
| D5 | **Child-HWND overlay** for WebView2, parented to the GTK toplevel's HWND. | Airspace: the browser paints above all GTK content in that window. Overlapping GTK widgets, popovers, menus and `Form` opacity do **not** clip it. Documented as a Windows-only deviation. |
| D6 | **Companion package `Gtk.Windows.Forms.WebView2`** + runtime backend factory. | One more package to version, sign, pack and publish. Main package stays clean for Linux/macOS. Precedent already in the tree: [Gtk.Windows.Extensions](Gtk.Windows.Extensions/Gtk.Windows.Extensions.csproj). |
| D7 | **Placeholder + opt-in bootstrapper** when the WebView2 Runtime is missing. | Nothing is ever downloaded unasked. See §8 for the deviation from the literal option wording (the opt-in switch does **not** go on `System.Windows.Forms.WebBrowser`). |
| D8 | **GTK4 / WebKit-6.0 only.** | The GTK3 tree (`..\Net4x.GtkWindowsForms_Gtk3`) keeps the documented gap. The backend interface is deliberately ABI-agnostic so a `webkit2gtk-4.1` implementation could be added later without touching the public layer. |
| D9 | *(mine, from repo precedent)* **Ship as `Resources/` overlays, not patches.** | These are wholly new files with no upstream counterpart — exactly the `NotifyIconBackend.*` situation described at [ResourcesCopyService.cs:35-37](PrepareProjects/Preparation/Source/ResourcesCopyService.cs#L35-L37). Only edits to *existing* generated files (`GLibrary.cs`) become patches. |
| D10 | *(mine, from repo precedent)* **Async siblings live in `Gtk.Windows.Extensions`, not in the core library.** | That package's own description says it is for "additions to the `System.Windows.Forms` surface that are deliberately not part of the real WinForms API, kept out of the core library so it stays a faithful compatibility layer". `InvokeScriptAsync`/`GetDocumentTextAsync`/`RefreshDomAsync` are precisely that. Cost: consumers who want them add a second package reference. |

---

## 2. Architecture

```
System.Windows.Forms                        (public, mono-derived, in Gtk.Windows.Forms)
├── WebBrowser : WebBrowserBase             ← mono WebBrowser.cs, rewired
├── WebBrowserBase : Control                ← mono WebBrowserBase.cs, ActiveX guts removed
├── WebBrowserSiteBase                      ← mono, inert stub (kept for surface parity)
├── HtmlDocument / HtmlElement / HtmlWindow
│   HtmlElementCollection / HtmlWindowCollection / HtmlHistory
│                                           ← mono, IDocument/IElement/IWindow swapped for the snapshot
└── WebBrowserReadyState / WebBrowserRefreshOption / WebBrowserEncryptionLevel
    HtmlElementInsertionOrientation / HtmlElement*EventArgs / *EventHandler
                                            ← mono, verbatim (all currently MISSING from the tree)

GtkSystem.Windows.Forms.GtkControls.ControlBase
└── WebViewBase : Gtk.Box, IControlGtk      ← the GTK host widget (NOT named WebBrowserBase — see §2.1)

GtkSystem.Windows.Forms.WebBrowsing         (backend seam — replaces Mono.WebBrowser.IWebBrowser)
├── IWebViewBackend                         ← navigation, history, state, script, snapshot
├── IHtmlDomBridge                          ← the JS-synthesised DOM the Html* types read
├── WebViewBackendFactory                   ← probes, registers, falls back
├── WebKitWebViewBackend                    ← Net4x.WebKitGtkSharp (Linux/macOS)
├── NullWebViewBackend                      ← placeholder + honest failure events
└── DomSnapshot / DomNode                   ← the cached document model

Gtk.Windows.Forms.WebView2                  (new companion package, Windows only)
├── WebView2Backend : IWebViewBackend
├── WebView2HostWindow                      ← child-HWND overlay, positioning, clipping, focus
├── GdkWin32Interop                          ← gdk_win32_surface_get_handle P/Invoke
└── WebView2BackendRegistration             ← ModuleInitializer → factory.Register(...)

Gtk.Windows.Extensions                      (existing companion package)
└── WebBrowserAsyncExtensions                ← InvokeScriptAsync, GetDocumentTextAsync, RefreshDomAsync…
```

### 2.1 A naming collision that must not be fumbled

`System.Windows.Forms.WebBrowserBase` is a **public WinForms type** (mono `WebBrowserBase.cs`, real WinForms too). The shim's own convention for GTK host widgets is also `XxxBase`, in `GtkSystem.Windows.Forms.GtkControls.ControlBase` — `PictureBoxBase`, `RichTextBoxBase`, `TextBoxBase`… If the GTK widget were called `WebBrowserBase` the two would be distinguished only by namespace, in files that routinely `using GtkSystem.Windows.Forms.GtkControls.ControlBase;`.

**The GTK host widget is `WebViewBase`.** No exceptions.

### 2.2 The control follows the existing shim pattern exactly

Mirroring [PictureBox.cs:18-38](Gtk.Windows.Forms/System/Windows/Forms/PictureBox.cs#L18-L38) and [PictureBox.cs:97](Gtk.Windows.Forms/System/Windows/Forms/PictureBox.cs#L97):

```csharp
// Gtk.Windows.Forms/System/Windows/Forms/WebBrowser.cs
public partial class WebBrowser : WebBrowserBase
{
    /// <summary>The self</summary>
    public readonly WebViewBase self = new();

    public WebBrowser()
    {
        self.Override.sender = this;
    }

    public override object GtkControl => self;
}
```

`Control.GtkControl` ([Control.cs:1423](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1423)) is what `Widget` ([Control.cs:1949](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1949)) and `ISelf` ([Control.cs:1592](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1592)) resolve through, so overriding it is the whole integration.

---

## 3. What already exists, and what is missing

Present in the tree today (from upstream `gtksystem-windows-forms`, .NET-Foundation-headed, **keep as-is**):

- [WebBrowserNavigatingEventArgs.cs](Gtk.Windows.Forms/System/Windows/Forms/EventArgs/WebBrowserNavigatingEventArgs.cs), `WebBrowserNavigatedEventArgs`, `WebBrowserDocumentCompletedEventArgs`, `WebBrowserProgressChangedEventArgs` and their four handlers under `System/Windows/Forms/EventHandlers/`.

Verified **missing** and to be added (`grep` for `class|enum|delegate <name>` across `Gtk.Windows.Forms/**/*.cs` returns nothing):

`WebBrowser`, `WebBrowserBase`, `WebBrowserSiteBase`, `WebBrowserReadyState`, `WebBrowserRefreshOption`, `WebBrowserEncryptionLevel`, `HtmlDocument`, `HtmlElement`, `HtmlElementCollection`, `HtmlWindow`, `HtmlWindowCollection`, `HtmlHistory`, `HtmlElementEventArgs`, `HtmlElementEventHandler`, `HtmlElementErrorEventArgs`, `HtmlElementErrorEventHandler`, `HtmlElementInsertionOrientation`.

Source of truth for each: `mono/mcs/class/System.Windows.Forms/System.Windows.Forms/<Name>.cs`.

**Unportable by construction:** everything under `mono/mcs/class/Mono.WebBrowser/` — `Mono.Mozilla/*` is XPCOM against `libgluezilla`, a Novell native shim that has not existed for over a decade, and `Mono.WebBrowser/IWebBrowser.cs` + `Mono.WebBrowser/DOM/*` are its interface set. `IWebBrowser` is *read* to derive `IWebViewBackend`; no file from that assembly is copied.

---

## 4. File inventory

### 4.1 New overlay sources (`Resources/` → generated tree)

| New file in `Resources/` | Lands at | Derived from |
|---|---|---|
| `WebBrowser.cs` | `Gtk.Windows.Forms/System/Windows/Forms/WebBrowser.cs` | mono `WebBrowser.cs` (706 lines) |
| `WebBrowserBase.cs` | `…/System/Windows/Forms/WebBrowserBase.cs` | mono `WebBrowserBase.cs` (755) |
| `WebBrowserSiteBase.cs` | `…/System/Windows/Forms/WebBrowserSiteBase.cs` | mono (53) |
| `HtmlDocument.cs` | `…/System/Windows/Forms/HtmlDocument.cs` | mono (538) |
| `HtmlElement.cs` | `…/System/Windows/Forms/HtmlElement.cs` | mono (702) |
| `HtmlElementCollection.cs` | `…/System/Windows/Forms/HtmlElementCollection.cs` | mono (116) |
| `HtmlWindow.cs` | `…/System/Windows/Forms/HtmlWindow.cs` | mono (431) |
| `HtmlWindowCollection.cs` | `…/System/Windows/Forms/HtmlWindowCollection.cs` | mono (87) |
| `HtmlHistory.cs` | `…/System/Windows/Forms/HtmlHistory.cs` | mono (99) |
| `HtmlElementEventArgs.cs` | `…/System/Windows/Forms/EventArgs/` | mono (127) |
| `HtmlElementErrorEventArgs.cs` | `…/System/Windows/Forms/EventArgs/` | mono (65) |
| `HtmlElementEventHandler.cs` | `…/System/Windows/Forms/EventHandlers/` | mono (29) |
| `HtmlElementErrorEventHandler.cs` | `…/System/Windows/Forms/EventHandlers/` | mono (29) |
| `HtmlElementInsertionOrientation.cs` | `…/System/Windows/Forms/Enum/` | mono (39) |
| `WebBrowserReadyState.cs` | `…/System/Windows/Forms/Enum/` | mono (40) |
| `WebBrowserRefreshOption.cs` | `…/System/Windows/Forms/Enum/` | mono (38) |
| `WebBrowserEncryptionLevel.cs` | `…/System/Windows/Forms/Enum/` | mono (42) |
| `WebViewBase.cs` | `…/GtkSystem/Windows/Forms/GtkControls/ControlBase/WebViewBase.cs` | new |
| `IWebViewBackend.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/IWebViewBackend.cs` | new (shaped by mono `IWebBrowser.cs`) |
| `IHtmlDomBridge.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new |
| `DomSnapshot.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new |
| `WebViewBackendFactory.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new |
| `WebKitWebViewBackend.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new |
| `NullWebViewBackend.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new |
| `DomBridgeScript.cs` | `…/GtkSystem/Windows/Forms/WebBrowsing/` | new (the injected JS, as a `const string`) |

The namespace-driven reorganisation in the pipeline ([TransformPipeline.cs:74-87](PrepareProjects/TransformPipeline.cs#L74-L87)) places files by namespace, so the overlay copy target is `GTKControls\<file>` and the final location follows from the `namespace` declaration — exactly how the `NotifyIconBackend.*` overlays reach `System\Windows\Forms\`.

### 4.2 New companion project

```
Gtk.Windows.Forms.WebView2/
├── Gtk.Windows.Forms.WebView2.csproj    (copy Gtk.Windows.Extensions.csproj's shape: signed with
│                                         ../GtkForms.snk, LICENSE.txt packed, PackageOutputPath
│                                         $(SolutionDir)Packages/, InternalsVisibleTo the test project)
├── README.md
├── WebView2Backend.cs
├── WebView2HostWindow.cs
├── GdkWin32Interop.cs
└── WebView2BackendRegistration.cs
```

Hand-maintained (**not** pipeline-generated), like `Gtk.Windows.Extensions`. Must be added to `Gtk.Windows.Forms.sln` (project GUID block near line 29) **and** excluded from `GeneratedTreeGuard`'s purview.

### 4.3 Existing files that change

| File | Change |
|---|---|
| [PrepareProjects/Preparation/Source/ResourcesCopyService.cs:50](PrepareProjects/Preparation/Source/ResourcesCopyService.cs#L50) | new overlay-copy loop (code in §9) |
| [Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/GLibrary.cs:31](Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/GLibrary.cs#L31) | `Library.Webkit` names are GTK3-era `libwebkit2gtk-4.0` — wrong ABI for this branch. → **patch** (§9) |
| [Gtk.Windows.Forms/Gtk.Windows.Forms.csproj](Gtk.Windows.Forms/Gtk.Windows.Forms.csproj) `<ItemGroup>` with `Net4x.GtkSharp` | add `Net4x.WebKitGtkSharp` |
| [Directory.Build.Props:35](Directory.Build.Props#L35) | add `WebKitGtkSharpPackageVersion` mirroring the `GtkSharpPackageVersion` pin |
| [Directory.Nuget.Props:18-26](Directory.Nuget.Props#L18-L26) | add `WebView2Version` |
| [Gtk.Windows.Forms.sln](Gtk.Windows.Forms.sln) | add the companion project + its test project |
| [EMULATION-GAPS.md:23](EMULATION-GAPS.md#L23) | move the entry out of §1 ("will not be implemented") into §2 ("recorded deviations"), rewritten |
| [API-COMPLETENESS.md:63](API-COMPLETENESS.md#L63) | drop the `Html*`/`WebBrowser` row (17) from the non-goals table; regenerate the counts |
| [ReadMe.md:25](ReadMe.md#L25) | rewrite the "Web / HTML hosting" bullet |
| [UpdateHistory.md](UpdateHistory.md) | entry per milestone |
| [bitbucket-pipelines.yml](bitbucket-pipelines.yml) | install `libwebkitgtk-6.0-4` in the Linux image (B3) |

---

## 5. The backend seam

Derived from mono's `Mono.WebBrowser.IWebBrowser` (`mono/mcs/class/Mono.WebBrowser/Mono.WebBrowser/IWebBrowser.cs`, 429 lines) but reshaped for D4 — every operation is fire-and-forget or reads cached state; nothing returns a value that requires a round trip.

```csharp
namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// The engine seam behind <see cref="System.Windows.Forms.WebBrowser"/>. Replaces mono's
/// <c>Mono.WebBrowser.IWebBrowser</c>, whose only implementation was the Mozilla/XPCOM backend.
/// Every member is non-blocking: commands are posted, state is read from the last snapshot.
/// </summary>
public interface IWebViewBackend : IDisposable
{
    /// <summary>The GTK widget that occupies the control's area.</summary>
    Gtk.Widget Widget { get; }

    /// <summary>True when a real engine is behind this backend (false for the placeholder).</summary>
    bool IsFunctional { get; }

    // ---- navigation (all fire-and-forget) ----
    void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders);
    void LoadHtml(string html, Uri? baseUri);
    void GoBack();
    void GoForward();
    void Reload(bool bypassCache);
    void Stop();

    // ---- cached state (never blocks) ----
    bool CanGoBack { get; }
    bool CanGoForward { get; }
    bool IsBusy { get; }
    Uri? Url { get; }
    string DocumentTitle { get; }
    string StatusText { get; }
    int Progress { get; }
    WebBrowserReadyState ReadyState { get; }
    WebBrowserEncryptionLevel EncryptionLevel { get; }

    // ---- settings ----
    bool ContextMenuEnabled { get; set; }
    bool ScrollBarsEnabled { get; set; }
    bool ShortcutsEnabled { get; set; }
    bool ScriptErrorsSuppressed { get; set; }
    bool AllowDrop { get; set; }

    // ---- script + DOM ----
    IHtmlDomBridge Dom { get; }
    Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken);

    // ---- events raised on the GTK main thread ----
    event EventHandler<BackendNavigatingEventArgs> Navigating;   // cancellable
    event EventHandler<BackendNavigatedEventArgs>  Navigated;
    event EventHandler<BackendProgressEventArgs>   ProgressChanged;
    event EventHandler<BackendLoadEventArgs>       LoadFinished; // success or failure
    event EventHandler                             TitleChanged;
    event EventHandler                             StatusChanged;
    event EventHandler<BackendNewWindowEventArgs>  NewWindow;    // cancellable
}
```

`IHtmlDomBridge` is the seam the `Html*` types sit on — the direct replacement for `Mono.WebBrowser.DOM.IDocument`/`IElement`/`IWindow`:

```csharp
public interface IHtmlDomBridge
{
    /// <summary>Last snapshot pulled from the page; never null, possibly empty.</summary>
    DomSnapshot Snapshot { get; }

    /// <summary>Re-pulls the snapshot. Raised as <see cref="SnapshotUpdated"/> when it lands.</summary>
    Task RefreshAsync(CancellationToken cancellationToken);

    /// <summary>Posts a mutation (SetAttribute, InnerHtml=, Focus, ScrollIntoView…) and
    /// optimistically applies it to <see cref="Snapshot"/>.</summary>
    void Post(DomMutation mutation);

    Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args, CancellationToken ct);

    event EventHandler SnapshotUpdated;
    event EventHandler<HtmlElementEventArgs> DomEvent;   // from delegated listeners in the page
}
```

### 5.1 The snapshot, and exactly what "cached" means

After each `LoadFinished`, and on demand via `RefreshAsync`, the bridge evaluates one injected script that walks the document and returns a JSON tree:

```
DomNode { Handle, TagName, Id, Name, Attributes[], InnerText, InnerHtml, OuterHtml,
          Style, TabIndex, Enabled, ClientRect, OffsetRect, ScrollRect, ScrollLeft, ScrollTop,
          ParentHandle, ChildHandles[] }
```

`Handle` is an integer key into a `WeakMap`-backed registry the injected script maintains, so a later `Post`/`InvokeMemberAsync` can address the same live node without re-querying by selector.

Synchronous `Html*` getters read only this snapshot. Setters mutate the snapshot immediately and post the real change. **Documented consequences:**

- Between a navigation completing and the snapshot landing, `WebBrowser.Document` is `null` (matching mono, which also returned `null` until `documentReady`).
- Script that mutates the DOM after the snapshot is invisible until the next `RefreshAsync`.
- `HtmlElement.InvokeMember` (synchronous, returns `object`) cannot return a value under D4. It **posts** the call and returns `null`, with the async sibling (`InvokeMemberAsync`, D10) as the way to get an answer. This is the single most visible deviation and must be called out in the member's XML doc and in `EMULATION-GAPS.md`.

### 5.2 Member mapping (the parts that are not one-liners)

| `WebBrowser` member | WebKitGTK 6.0 | WebView2 | Note |
|---|---|---|---|
| `Navigate(Uri)` | `WebView.LoadUri` (`WebView.cs:3069`) | `CoreWebView2.Navigate` | |
| `Navigate(…, postData, headers)` | `WebView.LoadRequest` + `URIRequest` | `NavigateWithWebResourceRequest` | |
| `DocumentText` set | `WebView.LoadHtml(content, baseUri)` (`:3039`) | `NavigateToString` | |
| `DocumentText` get | snapshot `OuterHtml` of `<html>` | idem | stale-able (D4) |
| `DocumentStream` set | `LoadBytes` (`:3025`) | `NavigateToString` after decode | mono returned `null` on get; keep that |
| `CanGoBack` / `CanGoForward` | `CanGoBack()` / `CanGoForward()` (`:2625`, `:2635`) | `CoreWebView2.CanGoBack/Forward` | |
| `GoBack` / `GoForward` | `GoBack()` / `GoForward()` | idem | |
| `Refresh(WebBrowserRefreshOption)` | `Reload()` / `ReloadBypassCache()` (`:3079`, `:3087`) | `Reload()` | `Completely`/`IfExpired` collapse to the two WebKit modes |
| `Stop` | `StopLoading()` (`:3198`) | `Stop()` | |
| `DocumentTitle` | `WebView.Title` (`:257`) | `CoreWebView2.DocumentTitle` | |
| `Url` | `WebView.Uri` (`:270`) | `CoreWebView2.Source` | |
| `IsBusy` | `WebView.IsLoading` (`:2979`) | `NavigationStarting`/`Completed` tracking | |
| `ReadyState` | `LoadChanged` (`LoadEvent.Started/Redirected/Committed/Finished`) | `NavigationStarting`/`DOMContentLoaded`/`NavigationCompleted` | |
| `Navigating` (cancellable) | `DecidePolicy` + `NavigationPolicyDecision` (`WebView.cs:578`) | `NavigationStarting` `e.Cancel` | |
| `Navigated` | `LoadChanged` == `Committed` | `SourceChanged` | |
| `DocumentCompleted` | `LoadChanged` == `Finished` (`:588`) | `NavigationCompleted` | fire after the snapshot lands, so `Document` is usable in the handler |
| `ProgressChanged` | `notify::estimated-load-progress` (`:122`) | `NavigationCompleted` only | WebView2 has no incremental progress — report 0 then 100 |
| `NewWindow` (cancellable) | `Create` signal (`:608`) — return `null` to refuse | `NewWindowRequested` `e.Handled` | |
| `StatusText` | `MouseTargetChanged` (`:478`) hit-test link URI | `StatusBarTextChanged` | |
| `EncryptionLevel` | `GetTlsInfo` (`:2893`) → map `TlsCertificateFlags` | `CoreWebView2.ServerCertificate` | coarse mapping; document it |
| `IsWebBrowserContextMenuEnabled` | `ContextMenu` signal (`:428`), return `true` to suppress | `AreDefaultContextMenusEnabled` | |
| `ScrollBarsEnabled` | injected CSS (`::-webkit-scrollbar{display:none}`) | idem | no engine setting either side |
| `ScriptErrorsSuppressed` | swallow `console`/`LoadFailed` reporting | `IsScriptEnabled` unaffected; suppress our own surfacing | |
| `ObjectForScripting` | `UserContentManager` script message handler ↔ reflection dispatch | `AddHostObjectToScript` | best-effort; primitives + strings only |
| `Print` | `WebView` `Print` signal / `WebKit.PrintOperation` | `ShowPrintUI` | |
| `ShowPageSetupDialog` / `ShowPrintPreviewDialog` / `ShowPropertiesDialog` / `ShowSaveAsDialog` | GTK dialogs where one exists, else `NotSupportedException` | WebView2 equivalents where they exist | mono threw `NotSupportedException` for most — keep parity where WebKit offers nothing |
| `GoSearch` / `GoHome` | navigate to a configurable default (mono hard-coded Novell's) | idem | expose as `WebBrowserDefaults.HomeUrl`/`SearchUrl` in `Gtk.Windows.Extensions` rather than hard-coding a vendor URL |
| `Version` | assembly version of the backend | idem | |
| `ActiveXInstance`, `AttachInterfaces`, `CreateSink`, `DetachInterfaces`, `DetachSink`, `CreateWebBrowserSiteBase`, `WndProc` | — | — | kept for surface parity, inert; `WndProc` already throws `PlatformNotSupportedException` repo-wide (EMULATION-GAPS §1) |

---

## 6. `WebViewBase` — the GTK host widget

```csharp
namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// The GTK widget behind <see cref="System.Windows.Forms.WebBrowser"/>. Named WebViewBase, not
/// WebBrowserBase: the latter is a public System.Windows.Forms type.
/// </summary>
public sealed class WebViewBase : Gtk.Box, IControlGtk
{
    public GtkControlOverride Override { get; set; }

    internal IWebViewBackend Backend { get; private set; }

    public WebViewBase() : base(Gtk.Orientation.Vertical, 0)
    {
        Override = new GtkControlOverride(this);
        StyleContext.AddClass("WebBrowser");

        Backend = WebViewBackendFactory.Create();     // WebKit → WebView2 → Null
        var child = Backend.Widget;
        child.Hexpand = true;
        child.Vexpand = true;
        this.AddChild(child);                         // Gtk4Compat extension
    }
}
```

`AddChild` is the `Gtk4Compat` extension already used throughout the backend ([Gtk4Compat.cs](Gtk.Windows.Forms/GTKSystem/Gtk4Compat.cs)); `GtkControlOverride` gives the placeholder path a paint surface for free.

---

## 7. The Windows WebView2 backend (D2 + D5)

The hard part. GTK4 has no child windows: only a toplevel has an HWND. So the WebView2 becomes a **child HWND of the GTK toplevel**, manually positioned over the area the `WebViewBase` widget occupies.

### 7.1 Getting the HWND

```csharp
internal static class GdkWin32Interop
{
    // Exported by the GTK4 runtime library on Windows (verify per B4 — the export lives in
    // libgtk-4-1.dll for the MSYS2/gvsbuild layouts; the loader path in GLibrary.cs:53 is stale).
    [DllImport("libgtk-4-1.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gdk_win32_surface_get_handle(IntPtr surface);

    internal static IntPtr GetToplevelHwnd(Gtk.Widget widget)
    {
        // Gtk.Window.Surface (Window.cs:1245) / Gtk.INative.Surface (INative.cs:17)
        if (widget.Root is not Gtk.Window window) return IntPtr.Zero;
        var surface = window.Surface;
        return surface is null ? IntPtr.Zero : gdk_win32_surface_get_handle(surface.Handle);
    }
}
```

### 7.2 Positioning and clipping

`WebView2HostWindow` owns a `CoreWebView2Controller` whose `ParentWindow` is that HWND, and drives `controller.Bounds` from the GTK widget's geometry:

- Origin: `webViewBase.TranslateCoordinates(root, 0, 0, out var x, out var y)` ([Widget.cs:3426](file:///d:/CommonLibrary/GtkSharp/Source/Libs/GtkSharp/Generated/Gtk/Widget.cs)) — or `ComputePoint` (`:2483`) with a `Graphene.Point`; pick whichever the fork's overload set makes cleanest.
- Size: the widget's allocation.
- HiDPI: multiply by `surface.ScaleFactor` ([Surface.cs:145](file:///d:/CommonLibrary/GtkSharp/Source/Libs/GdkSharp/Generated/Gdk/Surface.cs)) — WebView2 bounds are in **physical** pixels, GTK allocations in logical ones. Getting this wrong is the classic "browser is a quarter of the control on a 4K screen" bug.
- Re-drive on: `SizeAllocate`, `Map`, `Unmap`, `notify::scale-factor`, the root window's `notify::default-width/height`, and any ancestor scroll (`Gtk.ScrolledWindow.Vadjustment.ValueChanged`).
- `controller.IsVisible = false` when the widget is unmapped, when its allocation is empty, or when it scrolls out of the viewport. Nothing else clips it — that is D5.

### 7.3 Focus

- GTK `EventControllerFocus.Enter` on `WebViewBase` → `controller.MoveFocus(Programmatic)`.
- WebView2 `MoveFocusRequested` → move GTK focus to the next/previous widget and set `e.Handled = true`, so Tab does not trap inside the browser.

### 7.4 Threading

WebView2's `EnsureCoreWebView2Async` is asynchronous and its callbacks arrive on the UI thread of the HWND's message pump. Under GTK on Windows that pump *is* the GTK main loop, so callbacks are already on the GTK thread — but every event handler must still marshal through `GLib.Idle.Add` before touching GTK widgets, to keep one discipline across both backends. Navigation requests issued before initialisation completes are queued in the backend and replayed on `CoreWebView2Ready` (a `Navigate` that silently vanishes because the control was not ready yet is the failure mode to avoid).

---

## 8. The placeholder path (D7)

`NullWebViewBackend` is chosen by the factory when neither engine loads. It:

- draws a centred, localisable message through the `GtkControlOverride` paint path (resource string in `Properties/Resources.resx` + the five existing locale resx files — the pipeline's culture invariant requires it);
- reports `IsFunctional == false`, `ReadyState == Uninitialized`;
- on `Navigate`, raises `Navigating`, then `LoadFinished` with a failure status, so `WebBrowser` raises `Navigating` → `DocumentCompleted` and applications that drive a state machine off those events keep running;
- leaves `Document` `null`.

**Deviation from the literal round-2 option text:** the bootstrapper opt-in is **not** `System.Windows.Forms.WebBrowser.AllowRuntimeBootstrap`. Putting a non-WinForms static on a reference-assembly type is exactly what `Gtk.Windows.Extensions` exists to prevent. It goes on the companion package instead:

```csharp
namespace Gtk.Windows.Forms.WebView2;

public static class WebView2Runtime
{
    /// <summary>
    /// Opt in to launching Microsoft's Evergreen WebView2 bootstrapper when the runtime is absent.
    /// Default false: nothing is ever downloaded unasked.
    /// </summary>
    public static bool AllowRuntimeBootstrap { get; set; }

    /// <summary>True when a WebView2 runtime is present.</summary>
    public static bool IsRuntimeAvailable { get; }
}
```

If you want it on `WebBrowser` itself despite the above, say so and I will move it.

---

## 9. Pipeline durability

### 9.1 Overlay registration

Append to [ResourcesCopyService.cs](PrepareProjects/Preparation/Source/ResourcesCopyService.cs), after the tray-backend loop that ends at line 50:

```csharp
        // The WebBrowser control, its Html* DOM layer and the engine backends. Overlays rather than a
        // patch for the same reason the tray backends above are: wholly new files with no upstream
        // counterpart. The public types are ported from the mono submodule
        // (mono/mcs/class/System.Windows.Forms/System.Windows.Forms) with the Mozilla/XPCOM backend
        // replaced by IWebViewBackend; the reorganisation step files them by namespace.
        foreach (var webFile in new[]
                 {
                     "WebBrowser.cs", "WebBrowserBase.cs", "WebBrowserSiteBase.cs",
                     "WebBrowserReadyState.cs", "WebBrowserRefreshOption.cs", "WebBrowserEncryptionLevel.cs",
                     "HtmlDocument.cs", "HtmlElement.cs", "HtmlElementCollection.cs",
                     "HtmlWindow.cs", "HtmlWindowCollection.cs", "HtmlHistory.cs",
                     "HtmlElementEventArgs.cs", "HtmlElementErrorEventArgs.cs",
                     "HtmlElementEventHandler.cs", "HtmlElementErrorEventHandler.cs",
                     "HtmlElementInsertionOrientation.cs",
                     "WebViewBase.cs", "IWebViewBackend.cs", "IHtmlDomBridge.cs", "DomSnapshot.cs",
                     "WebViewBackendFactory.cs", "WebKitWebViewBackend.cs", "NullWebViewBackend.cs",
                     "DomBridgeScript.cs"
                 })
        {
            LogUtility.FileCopyAndLog(RepoPaths.At(Path.Combine("Resources", webFile)),
                Path.Combine(targetDir, "GTKControls", webFile));
        }
```

### 9.2 The one patch

`GLibrary.cs:31` names the GTK3-era ABI. On this branch it must be:

```csharp
            [Library.Webkit] = new[] { "libwebkitgtk-6.0-4.dll", "libwebkitgtk-6.0.so.4", "libwebkitgtk-6.0.4.dylib", "webkitgtk-6.0.dll" }
```

Created per [Patches.txt](Patches.txt): `git format-patch -o patches/ -1 HEAD -- :(exclude)*.csproj`, named `0147-Gtk4-name-the-WebKitGTK-6.0-ABI-the-branch-actually-loads.patch`.

This is a genuine library-general fix independent of the port (the entry is wrong today for anything on the gtk4 branch), so it lands as its own commit first.

---

## 10. Tests

Per the standing rule that every fix ships with a test, and that tests assert real behaviour rather than hitting lines. All widget-constructing classes call `Application.Init()` in the constructor and carry `[Collection("NonParallelCollection")]`, matching [ControlExtensionTests.cs:13-17](Gtk.Windows.Forms.Tests/GtkControls/ControlExtensionTests.cs#L13-L17). The solution test run needs `-m:1`.

New, in `Gtk.Windows.Forms.Tests/System/Windows/Forms/`:

| Suite | Oracle |
|---|---|
| `WebBrowserNavigationTests` | With a `FakeWebViewBackend` (NSubstitute is already a dependency): `Navigate` raises `Navigating` → `Navigated` → `DocumentCompleted` **in that order**, once each. Cancelling `Navigating` suppresses the other two and leaves `Url` unchanged. `AllowNavigation=false` blocks the second navigation only. `Url` normalisation (`"example.com"` → `http://example.com/`, `null`/`""` → `about:blank`) matches mono. |
| `WebBrowserStateTests` | `CanGoBack`/`CanGoForward`/`IsBusy`/`DocumentTitle`/`StatusText` delegate to the backend and raise `CanGoBackChanged`/`CanGoForwardChanged`/`DocumentTitleChanged`/`StatusTextChanged` exactly when the backing value changes, not on every poll. `ReadyState` walks `Uninitialized → Loading → Interactive → Complete`. |
| `HtmlDocumentSnapshotTests` | Feed a fixed JSON snapshot (checked in under `Resources/`) into the bridge; assert `GetElementById`, `GetElementsByTagName`, `Body`/`Forms`/`Images`/`Links`, `Title`, `Url`, and that a missing id returns `null` rather than throwing. |
| `HtmlElementTests` | `InnerText`/`InnerHtml`/`OuterHtml`/`GetAttribute`/`TagName`/`Id`/`Name`/`TabIndex`/`Style` read the snapshot; `SetAttribute` updates the snapshot **and** posts exactly one mutation; `Parent`/`FirstChild`/`NextSibling`/`Children` navigate the tree consistently; `Equals`/`GetHashCode`/`==`/`!=` follow mono's identity-by-underlying-node semantics (two `HtmlElement`s over the same handle are equal). |
| `HtmlElementCollectionTests` | `Count`, `this[int]`, `this[string]`, `GetElementsByName`, `GetEnumerator`, and out-of-range/missing-name behaviour. |
| `HtmlUnsupportedMembersTests` | The members that cannot work throw `NotSupportedException` **with a message naming the reason** — `DomDocument`, `DomElement`, `DomWindow`, `DomHistory`, `HtmlWindow.Open`. Guards against a later refactor silently returning `null` instead. |
| `NullWebViewBackendTests` | With no engine: the control constructs, `Navigate` raises `Navigating` then a failed `DocumentCompleted`, `Document` is `null`, `IsFunctional` is false, and nothing throws. |
| `WebViewBackendFactoryTests` | Registration order and fallback: registered Windows backend wins on Windows, WebKit wins when its probe succeeds, `Null` when both fail. Probe failures do not propagate. |
| `WebKitWebViewBackendTests` *(skippable)* | `[SkippableFact]` gated on `libwebkitgtk-6.0` being loadable: real `about:blank` load raises `DocumentCompleted`, `Document.Body` is non-null, a `LoadHtml` round-trip returns the written text through the snapshot. |

New project `Gtk.Windows.Forms.WebView2.Tests` (Windows-gated, `[SkippableFact]` on `WebView2Runtime.IsRuntimeAvailable`): bounds arithmetic under a scale factor of 1 and 2, visibility toggling on unmap, and the queued-navigation replay after `CoreWebView2Ready`. The bounds arithmetic is pure and must be tested **without** a runtime present.

---

## 11. Verification

```sh
# after the GLibrary patch (milestone 1)
dotnet build Gtk.Windows.Forms.sln
dotnet test  Gtk.Windows.Forms.sln -m:1

# after each subsequent milestone
dotnet test Gtk.Windows.Forms.Tests/Gtk.Windows.Forms.Tests.csproj -m:1
dotnet test Gtk.Windows.Forms.WebView2.Tests/Gtk.Windows.Forms.WebView2.Tests.csproj -m:1

# real engines
dotnet run --project GtkWinFormsApp/GtkWinFormsApp.csproj      # Linux: WebKit; Windows: WebView2

# pipeline durability — the real proof the overlays survive regeneration
SkipPush=1 dotnet run --project PrepareProjects/PrepareProjects.csproj
git diff --stat Gtk.Windows.Forms/System/Windows/Forms/WebBrowser.cs   # must be empty
```

A `WebBrowser` demo goes on a **new** form in `GtkWinFormsApp` (`Form6`), not by modifying an existing one — the existing forms are baselined by `GtkWinFormsApp.UITests`/`SampleParity` and changing them re-baselines screenshots for no reason.

Docs regenerated/updated at the end: `API-COMPLETENESS.md` counts, `EMULATION-GAPS.md` §1→§2 move, `ReadMe.md` platform notes, `UpdateHistory.md`.

---

## 12. Milestones (each ends in a commit)

| # | Deliverable | Ends when |
|---|---|---|
| 1 | `GLibrary` WebKit ABI patch | patch file in `patches/`, build+tests green |
| 2 | Seam + placeholder: `IWebViewBackend`, `IHtmlDomBridge`, `DomSnapshot`, `WebViewBackendFactory`, `NullWebViewBackend`, `WebViewBase`; overlay registration in `ResourcesCopyService` | `NullWebViewBackendTests`, `WebViewBackendFactoryTests` green; pipeline round-trip clean |
| 3 | Public API layer: `WebBrowser`, `WebBrowserBase`, `WebBrowserSiteBase`, three enums — over the placeholder backend only | `WebBrowserNavigationTests`, `WebBrowserStateTests` green |
| 4 | `Html*` layer over the snapshot | `HtmlDocumentSnapshotTests`, `HtmlElementTests`, `HtmlElementCollectionTests`, `HtmlUnsupportedMembersTests` green |
| 5 | `WebKitWebViewBackend` + `Net4x.WebKitGtkSharp` reference + the injected DOM script | skippable integration test green on a box with WebKitGTK |
| 6 | `Gtk.Windows.Forms.WebView2` companion package | bounds/visibility tests green; manual run on Windows |
| 7 | `Gtk.Windows.Extensions` async siblings | unit tests green |
| 8 | Sample `Form6`, docs, `UpdateHistory` | full solution build + `dotnet test -m:1` green |

---

## 13. Risks

| Risk | Severity | Mitigation |
|---|---|---|
| **B4 fails** — no HWND from the GTK4 Windows surface | Kills D2 entirely | Verify first (§0). Fallback: Windows uses the placeholder path, same as macOS-without-Homebrew-WebKit. |
| Airspace (D5) reported as a bug by users | Medium, permanent | Document prominently in `EMULATION-GAPS.md` §2 and the package README. No fix exists short of composition hosting, which round 2 rejected. |
| Snapshot staleness (D4) surprises code ported from real WinForms | Medium | Per-member XML docs, a dedicated `EMULATION-GAPS.md` §2 entry, and the async siblings as the documented escape hatch. |
| `InvokeMember`/`InvokeScript` returning `null` breaks ported apps | Medium | Same as above; this is the sharpest edge of D4 and deserves its own paragraph in the README. |
| Hard `Net4x.WebKitGtkSharp` dependency (D3) bloats every consumer | Low–medium | Measured, documented. If it becomes a complaint, the WebKit backend moves to its own companion package exactly like WebView2 — the factory seam makes that a mechanical change, which is a reason to build the seam properly in milestone 2. |
| WebKitGTK absent on user machines (very common on macOS, universal on Windows) | Low | Placeholder path (§8), tested. |
| mono code drifts from the pipeline's formatter/XML-doc rules | Low | `dotnet format` and the XML-doc merge run over the generated tree anyway; the overlays get the same treatment. Budget one pass fixing doc warnings — public members require XML docs and mono's originals have almost none. |
| Licence (B6) | Blocking, non-technical | Resolve before merge. |
| Pipeline regeneration wipes hand edits | Low | Overlays + one patch (§9), verified by the round-trip check in §11. |

---

## 14. Scale

Roughly 4 200 lines of mono source ported (with the DOM plumbing rewritten, not merely re-typed), plus ~2 000 lines of new backend/bridge/host code, plus tests. Milestones 5 and 6 carry nearly all the runtime risk; 2–4 are mechanical once the seam is right.
