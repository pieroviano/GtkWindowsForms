# Plan: Internet Explorer (Trident) backend for `WebBrowser` on Windows

Give `System.Windows.Forms.WebBrowser` an **IE `SHDocVw.WebBrowser` ActiveX** engine on Windows, shipped as a
new opt-in companion package, mirroring `Gtk.Windows.Forms.WebView2`.

## Decisions (confirmed)

| # | Decision | Consequence |
|---|----------|-------------|
| 1 | **IE is a fallback below WebView2** | Register at **priority 75** (WebView2=100, WebKitGTK=50). WebView2 wins when installed; IE is used when it is not. |
| 2 | **Full DOM + script** | Must drive the injected `DomBridgeScript` inside MSHTML and provide a host callback channel for DOM events — the hardest part (IE has no `postMessage`-to-host). Phased below. |
| 3 | **Auto IE11 standards mode** | On first use write `HKCU\...\FEATURE_BROWSER_EMULATION\<host.exe>=11001`. No admin rights. |
| 4 | **New companion package** | `Gtk.Windows.Forms.InternetExplorer`, `net462;net8.0-windows`, self-registers via `[ModuleInitializer]`. The `netstandard2.0` base cannot host COM. |

## BLOCKING verifications — settle these before writing code

1. **Host thread apartment must be STA.** OLE in-place activation of an ActiveX control requires the hosting
   thread to be STA (`OleInitialize`). Verify the GTK main loop thread is STA in the consuming app — check the
   entry point (`VbNetToCSharp.UI`) `Main` for `[STAThread]`, and whether GtkSharp's `Application.Run` keeps the
   same thread. **If the main thread is MTA, this cannot work** without marshalling the control onto a dedicated
   STA thread (large, out of scope). This is the single highest risk.
2. **Process bitness vs. IE registry.** `FEATURE_BROWSER_EMULATION` is read by `iexplore`/`MSHTML` per the host
   executable name; the value is the same for 32/64-bit, but confirm the host `.exe` name resolution
   (`Process.GetCurrentProcess().MainModule.ModuleName`) matches what we write.
3. **IE/Trident present.** `mshtml.dll` + the `WebBrowser` ActiveX (CLSID `8856F961-340A-11D0-A96B-00C04FD705A2`)
   exist on all supported Windows (IE11 / "IE mode" shim) — but confirm on the target image. Probe = `Type.GetTypeFromCLSID` + instantiate guarded; decline (return `null`) on failure so the factory falls through.
4. **GTK airspace limitation applies identically** to WebView2: the ActiveX paints in a child HWND over the GTK
   toplevel, above all GTK content; not clipped by overlapping widgets/popovers. Record in `EMULATION-GAPS.md`.

## Where it plugs in (existing seam — no base changes for Phase 1)

- Control delegates everything to `IWebViewBackend`: `Gtk.Windows.Forms\System\Windows\Forms\WebBrowser.cs`
  (e.g. `Navigate` → `Backend.Navigate`, `WebBrowser.cs:703`; DOM via `Backend.Dom`, `WebBrowser.cs:163`).
- Factory picks the highest-priority backend whose probe succeeds:
  `Gtk.Windows.Forms\GTKSystem\Windows\Forms\WebBrowsing\WebViewBackendFactory.cs:71`.
- Backend contract: `...\WebBrowsing\IWebViewBackend.cs` — note `Widget` (`IWebViewBackend.cs:472`) must return a
  `Gtk.Widget`; the HWND view is hosted over that widget's allocation.
- Reference implementation to mirror: `Gtk.Windows.Forms.WebView2\` — `WebView2Backend.cs`,
  `WebView2HostWindow.cs`, `GdkWin32Interop.cs`, `WebView2BackendRegistration.cs`, `.csproj`.

## New package layout — `Gtk.Windows.Forms.InternetExplorer\` (each file ≤200 lines)

| File | Responsibility |
|------|----------------|
| `Gtk.Windows.Forms.InternetExplorer.csproj` | `net462;net8.0-windows`, strong-named, `GeneratePackageOnBuild`, `PackageOutputPath=$(SolutionDir)Packages/`, `ProjectReference` to base, `Net4x.CleanCache`. Clone of the WebView2 csproj minus the WebView2 PackageReference. |
| `InternetExplorerBackendRegistration.cs` | `[ModuleInitializer]` → `WebViewBackendFactory.Register("InternetExplorer", 75, InternetExplorerBackend.TryCreate)`. Clone of `WebView2BackendRegistration.cs`. |
| `GdkWin32Interop.cs` | Copy verbatim (same `gdk_win32_surface_get_handle` resolution + `GetToplevelHandle`). |
| `InternetExplorerBackend.cs` | `IWebViewBackend` impl: placeholder `DrawingArea`, command queue, event plumbing, DOM bridge. Mirrors `WebView2Backend.cs`. |
| `AxWebBrowserHost.cs` | Owns the child HWND + OLE in-place activation of the ActiveX; geometry tick (33 ms) like `WebView2HostWindow.cs`. |
| `OleHostSite.cs` | `IOleClientSite` + `IOleInPlaceSite` + `IOleInPlaceFrame` + `IDocHostUIHandler`. The site the control activates into. |
| `OleInterop.cs` | `[ComImport]` interface decls + P/Invokes (`OleInitialize`, `OleCreate`/`CoCreateInstance`, `RECT`, `IOleObject`, `IWebBrowser2`). May split into 2 files to stay ≤200 lines. |
| `WebBrowserEventsSink.cs` | `DWebBrowserEvents2` connection-point sink → raises `Navigating/Navigated/LoadFinished/ProgressChanged/NewWindow/StatusChanged/TitleChanged`. |
| `TridentDomChannel.cs` | Phase 2/3: drives `DomBridgeScript` in MSHTML and the `window.external` callback for DOM events. |
| `BrowserEmulation.cs` | Writes `FEATURE_BROWSER_EMULATION=11001` under HKCU for the host exe (decision 3). |
| `IeBounds.cs` | Copy of `WebView2Bounds.cs` (DPI-scaled bounds + visibility). |
| `README.md`, licence/third-party `None` includes | Package metadata, like WebView2. |

## Hosting model (the core of Phase 1)

Mirror `WebView2HostWindow` but, because the ActiveX control has **no built-in "parent HWND + bounds" hosting**:

1. `GdkWin32Interop.GetToplevelHandle(anchor)` → parent HWND (same as WebView2, `WebView2HostWindow.cs:123`).
2. `CreateWindowEx` a child `STATIC`/custom-class HWND parented to the toplevel; this is the ActiveX container.
3. `OleCreate`/`CoCreateInstance` the `WebBrowser` control (CLSID above) → `IOleObject`.
4. `IOleObject.SetClientSite(OleHostSite)` → `OleSetContainedObject(TRUE)` → `DoVerb(OLEIVERB_INPLACEACTIVATE,
   …, hwndChild, &rect)`. The site's `GetWindow` returns the child HWND; `GetWindowContext` returns the frame.
5. Query `IWebBrowser2` for commands; advise `DWebBrowserEvents2` via the connection point.
6. Geometry tick (33 ms) re-reads `anchor.Allocation` + `ComputePoint` origin (as `WebView2HostWindow.cs:163`)
   and `SetWindowPos` the child HWND + `IWebBrowser2.Width/Height`.

### `IWebViewBackend` mapping (Phase 1)

| Member | IE implementation |
|--------|-------------------|
| `Navigate` | `IWebBrowser2.Navigate2(url, flags, frame, postData, headers)` |
| `GoBack/GoForward/Reload/Stop` | `GoBack()/GoForward()/Refresh2()/Stop()` |
| `Url` | `IWebBrowser2.LocationURL` |
| `DocumentTitle` | `DWebBrowserEvents2.TitleChange` → cached |
| `CanGoBack/Forward` | tracked from `BeforeNavigate2`/history depth (IE exposes no direct getter) |
| `EncryptionLevel` | `https` → `Bit128`, else `Insecure` (same shortcut as WebView2, `WebView2Backend.cs:188`) |
| `LoadHtml` | `Navigate2("about:blank")` then write via `IHTMLDocument2.write` |
| events | from `WebBrowserEventsSink` |

## Full DOM + script (decision 2) — phased, higher risk

- **Phase 2 — read-only DOM + `InvokeScript`:** after `DocumentComplete`, inject `DomBridgeScript.Source`
  (`...\WebBrowsing\DomBridgeScript.cs`, referenced by both existing backends) into MSHTML via
  `IHTMLWindow2.execScript`. Implement `InvokeScriptAsync` by invoking a JS function through `IHTMLWindow2`'s
  `IDispatch` and reading its return (IE returns values from `IDispatch::Invoke`, unlike bare `execScript`).
  `IHtmlDomBridge.RefreshAsync` calls `…snapshot()` the same way WebView2 does (`WebView2Backend.cs:76`).
- **Phase 3 — DOM events + mutations:** the bridge posts events to the host. WebView2 uses
  `chrome.webview.postMessage`; IE has no equivalent, so provide `window.external` via
  `IDocHostUIHandler.GetExternal` returning a `[ComVisible]` dispatch object the script calls; marshal those
  callbacks to `IHtmlDomBridge.RaiseDomEvent`. `Post(DomMutation)` runs the mutation script via `execScript`.

Each phase lands independently so Phase 1 (display + navigation + events) is usable even if 2/3 slip.

## Tests (`Gtk.Windows.Forms.InternetExplorer.Tests`, mirrors the WebView2 test project)

- **Registration/priority (no IE needed):** after the module initializer runs, `RegisteredNames()` lists
  `InternetExplorer` **below** `WebView2` and **above** `WebKitGTK` — pins decision 1. Fails without the fix.
- **Probe declines off-Windows / when Trident absent:** `TryCreate()` returns `null` (guarded), factory yields
  `NullWebViewBackend`.
- **`BrowserEmulation`:** writes the value under a redirected test HKCU key; asserts `11001` for the host exe.
- **Bounds math:** `IeBounds.Compute` DPI cases (copy of the WebView2 bounds tests).
- **STA-gated functional smoke (Windows + IE only):** construct backend, realize anchor in a `ShowDialog`,
  `Navigate("about:blank")`, assert `DocumentCompleted` fires and `Url`/`ReadyState` update. Skipped when not
  STA / no Trident, like the existing `WebBrowserEngineIntegrationTests`.

## Wiring the consumer

The UI opts in with a `PackageReference Include="Gtk.Windows.Forms.InternetExplorer"`. With WebView2 also
referenced, WebView2 still wins (priority). To actually get IE in the UI, either don't reference WebView2 or
remove its runtime — document this in the package README so the fallback semantics are not surprising.

## Risks

| Risk | Mitigation |
|------|------------|
| **Host thread is MTA** → OLE hosting fails | Blocking check #1; if MTA, stop and report — do not silently degrade. |
| IE/Trident removed on future Windows | Probe declines → WebKitGTK/placeholder; never faults the control. |
| ActiveX paints over GTK content (airspace) | Same known deviation as WebView2; documented. |
| Phase 2/3 DOM fidelity under IE quirks | IE11 emulation (decision 3) reduces this; snapshot bridge is best-effort and already tolerant in the other backends. |
| COM lifetime/leaks (connection points, sites) | Deterministic `Dispose`: unadvise sink, `DoVerb(HIDE)`, `Close(OLECLOSE_NOSAVE)`, release, `DestroyWindow`. |
| Security: Trident is EOL | It is the user's explicit, opt-in choice; the package README states the security posture plainly. |

## Out of scope

- Printing/dialog parity beyond what `IWebBrowser2.ExecWB` trivially offers (keep current `NotSupportedException`s).
- Hosting on a dedicated STA thread if the app is MTA (would be a separate, larger effort).
- Changing the default engine selection for existing WebView2 users.
