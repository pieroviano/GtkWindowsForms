# CsToCpp — Plan: convert C# WinForms projects to native cross-platform C++ (GTK4)

Goal: a tool that turns a C# project (console or WinForms) into a **CMake C++20 project** that uses
`std::shared_ptr`/`std::weak_ptr` and standard types, and links against:

- a **C++ runtime** that supplies the BCL subset the code actually uses;
- a **native WinForms library**, produced by running the same converter over this repo's
  `Gtk.Windows.Forms` + `Gtk.Drawing` (`gtk4` branch, GtkSharp4 → GTK4).

## Decisions (agreed)

| # | Decision | Consequence |
|---|---|---|
| D1 | Front-end = **Roslyn source** (`SemanticModel`), not IL | Readable C++, names/comments kept; source is required (no binary-only deps without a mapping) |
| D2 | Memory = **`shared_ptr` everywhere + `weak_ptr` inference**, plus a debug leak/cycle report | No GC; cycles the inference misses leak (found by the debug report, fixed by a rule, never by hand-editing output) |
| D3 | UI = **transpile Gtk.Windows.Forms/Gtk.Drawing**; GtkSharp4 calls target a **GtkSharp-shaped C++ facade generated from GIR** over the GTK4 C API | Library fixes in C# flow through by re-conversion; the facade has to reproduce GtkSharp4's API shape (metadata renames + hand-written partials) |
| D4 | **New repo** (working name `Net4x.CsToCpp`), **C++20**, **CMake + vcpkg** | Coroutines/concepts/`std::format` are available; floor is MSVC 19.30 / GCC 11 / Clang 14 / Apple Clang 15 |

General rule (from CLAUDE.md): generated C++ is **never hand-edited**. Every defect is fixed in the
converter, the runtime, the facade generator or the C# source layer (`patches/`), as a general rule,
never one shaped to fit a sample.

---

## 0. Blocking verifications (before any code)

| # | Check | How | If it fails |
|---|---|---|---|
| V1 | Gtk.Windows.Forms + Gtk.Drawing compile with **zero errors** in an `MSBuildWorkspace` (a full semantic model is needed) | Small Roslyn console app: `OpenProjectAsync` → `GetCompilationAsync().GetDiagnostics()` on the `gtk4` branch | Fix the build first; the converter refuses to convert a project that has errors |
| V2 | **GIR files** for the exact GTK4/Pango/GdkPixbuf/GLib/WebKit versions GtkSharp4 `4.22.4` binds (`Directory.Nuget.Props:16`) are available | Locate `Gtk-4.0.gir` etc. (vcpkg/MSYS2/distro `gir1.2-*`); diff the version against the GtkSharp4 generator inputs | Pin GTK to the GIR version that is available, then rebuild GtkSharp4 against it |
| V3 | The **GtkSharp4 generator metadata** (renames, `.metadata` fixups) and the **hand-written partials** of Net4x.GtkSharp are accessible | Inspect the Net4x.GtkSharp repo (cake build) | Without them the facade's names won't match → hand-maintained rename table |
| V4 | **Licence**: upstream and packages are **LGPL v3** (see CLAUDE.md). The converted C++ is a derivative work | Confirm with upstream | The native Forms lib must ship as an LGPL shared library (dynamic linking) |
| V5 | **vcpkg `gtk` (4.x) + `libadwaita`-free build** works on Windows (MSVC), Linux and macOS | CI matrix spike: a C++ "hello GtkWindow" | Fall back to MSYS2 (Windows) / Homebrew (macOS) GTK |
| V6 | The input is the **generated** `Gtk.Windows.Forms/` tree at a pinned commit (after `PrepareProjects` → `AlignProjects`) | Record the commit SHA in the CsToCpp repo (`forms/SOURCE_COMMIT`) | — |

---

## 1. Architecture

```
 C# project (.csproj)                 Gtk.Windows.Forms + Gtk.Drawing (C#, gtk4 branch)
        │                                         │
        ▼                                         ▼
 ┌──────────────────────── CsToCpp converter (C#, Roslyn) ─────────────────────┐
 │ load → analyze (partials, weak-refs, reflection reach, unsupported) → emit │
 └────────┬──────────────────────────────────────────────┬────────────────────┘
          ▼                                              ▼
   app/ (C++20, CMake)  ──links──►  forms/ (C++ lib, LGPL)  ──►  facade/ (GtkSharp-shaped C++, generated from GIR)
          │                                  │                           │
          └──────────────► runtime/ (C++ BCL subset) ◄───────────────────┘
                                                                         ▼
                                                    GTK4 / GDK / GSK / Cairo / Pango / GLib (C API)
```

### Repo layout (`Net4x.CsToCpp`)

| Path | Lang | Content |
|---|---|---|
| `converter/CsToCpp.Core` | C# net10.0 | Roslyn analysis + C++ emitter |
| `converter/CsToCpp.Cli` | C# | `cs2cpp convert <csproj> -o <dir>` |
| `converter/CsToCpp.GirFacadeGen` | C# | GIR → GtkSharp-shaped C++ facade |
| `converter/CsToCpp.Tests` | C# xUnit | Unit tests for the converter (`InternalsVisibleTo` in the csproj) |
| `runtime/` | C++20 | `cs2cpp_runtime` CMake target (namespaces `System::*`, `cs::` helpers) |
| `facade/generated`, `facade/manual` | C++20 | Generated GTK/GDK/GSK/Pango/GLib/GdkPixbuf/WebKit facade + hand-written Cairo & GtkSharp partial equivalents |
| `forms/` | C++20 | **Generated** output for Gtk.Drawing + Gtk.Windows.Forms (never hand-edited) |
| `corpus/` | C# + expected output | Differential test corpus |
| `tests/` | C++ | GoogleTest: runtime tests + transpiled xUnit tests |

---

## 2. Type & language mapping

### 2.1 Types

| C# | C++ | Notes |
|---|---|---|
| reference type `T` | `cs::ref<T>`: thin wrapper over `std::shared_ptr<T>` | Its `operator->` throws `NullReferenceException` on null; `nullptr` ↔ `null` |
| weak field (inferred, §3) | `cs::weak<T>` over `std::weak_ptr<T>` | Reads `lock()`; when expired it acts as `null` |
| `class` | `class X : public Base` | Root `System::Object : std::enable_shared_from_this<Object>` |
| `interface` | abstract struct, **no** `Object` base (avoids diamonds) | Casts via `std::dynamic_pointer_cast`; `cs::as_object(i)` when an interface ref is used as `Object` |
| `struct` | plain C++ value struct | `default(T)` → value-initialisation; a value-type field needs a complete type → structs are ordered topologically |
| `enum` / `[Flags]` | `enum class` + generated bit operators | `ToString()` via generated name tables |
| `int/long/short/byte/...` | `int32_t/int64_t/int16_t/uint8_t/...` | Signed overflow wraps: `-fwrapv` (GCC/Clang); `checked` → runtime helpers that throw `OverflowException` |
| `char` | `char16_t` | |
| `string` | `System::String`: immutable `std::u16string`, shared | UTF-16 index semantics are preserved; converted to UTF-8 at the facade boundary |
| `decimal` | `System::Decimal` (runtime, 96-bit) | |
| `T?` (value) | `std::optional<T>` | |
| `T[]`, `T[,]` | `cs::ref<cs::array<T>>` (`std::vector`), `cs::array2<T>` | Bounds-checked |
| `List<T>`, `Dictionary<K,V>`, `HashSet<T>` … | runtime classes over `std::vector` / insertion-ordered hash map | Keeps .NET enumeration order and `InvalidOperationException` on modification while enumerating |
| `object` boxing | `cs::box<T>` → `cs::ref<Object>` | `unbox` with type checking |
| delegate type | `cs::delegate<R(Args...)>`: multicast list of `(target identity, std::function)` | Identity makes `-=` work |
| `event` | `cs::event<D>` | 1285 event declarations in the Forms lib |

### 2.2 Constructs

| C# | C++ |
|---|---|
| Constructors | **Two-phase**: `cs::make<T>(args)` = `std::make_shared<T>()` + `obj->__ctor(args)`. `shared_from_this()` is valid inside the body (`Control.cs:679` stores `this`; `Button.cs:26` subscribes in the ctor). Virtual calls in a ctor dispatch to the derived type, as in C# |
| Field initialisers / base chaining | `__init()` then `Base::__ctor(...)` in C# order |
| Static ctor / static fields | Lazy guard (`std::call_once`) on first access to the type; this preserves the `Application` static ctor that boots GTK (`Application.cs:713`) and avoids static-init-order bugs |
| Properties / indexers | `get_X()` / `set_X(v)`; indexer → `get_Item`/`set_Item` |
| `partial` classes | Merged into one class |
| Extension methods | Free functions; call sites are rewritten |
| Overloads | Roslyn resolves them; the emitter adds **explicit casts** at every call site so C++ picks the same overload |
| Generics | Templates (in headers); constraints → concepts. **Generic virtual methods** → diagnostic plus type-erased fallback (`cs::ref<Object>` + cast) |
| `ref`/`out`/`in` | `T&` / `T&` / `const T&` |
| `try/catch/finally`, filters | `throw cs::exception_box{ref<Exception>}`; `finally` → scope guard; `when` → catch + condition + rethrow |
| `using` / `IDisposable` | RAII guard calling `Dispose()`; destructors don't call `Dispose` unless a finalizer exists |
| Finalizer `~T()` | C++ destructor (resurrection is not supported → diagnostic) |
| `lock` | Per-object `std::recursive_mutex` from a lazily created monitor table (12 files) |
| `async`/`await` | C++20 coroutine `cs::Task<T>`, continuations posted to `SynchronizationContext` (GLib main loop) |
| `yield` | C++20 generator coroutine implementing `IEnumerable<T>` |
| LINQ | Runtime lazy `Enumerable` operators, limited to the ones the usage report lists |
| Lambdas | `std::function`; capture of `this` follows the weak rules (§3) |
| `unsafe`, pointers, `fixed` | Raw pointers; `fixed` → no-op pin (9 files) |
| P/Invoke `DllImport` | Direct C calls; GTK ones are routed to the facade; OS backends (`NotifyIconBackend.Win32/.Quartz/.StatusNotifier`) are kept behind `#ifdef` |
| Reflection | §2.3 |
| `dynamic`, `Emit`, `BinaryFormatter` | Not supported → diagnostic. `.resx` is pre-compiled at build time (§5), so `BinaryFormatter` isn't needed |

### 2.3 Reflection-lite

There are 32 files / 50 call sites in the Forms lib (`TypeConverter`, `ComponentResourceManager.ApplyResources`,
`Activator`). Designer code relies on `ApplyResources(control, "name")`, which sets properties **by name**.

- The analyzer marks types that are *reflection-reachable*: those passed to reflection APIs, all
  `Component`-derived public types, and types named in `.resx`/`TypeConverter` attributes.
- For those types only, the emitter generates `cs::TypeInfo` tables (name, base, properties as
  getter/setter thunks, attributes, factory).
- `Type.GetType("name")` and `Activator.CreateInstance` → a global registry.

---

## 3. Memory model (D2)

### Weak inference rules (applied in order; the first match wins)

| # | Rule | Example in the Forms lib |
|---|---|---|
| W1 | Explicit: a symbol listed in sidecar `cs2cpp.weak.json` (keyed by Roslyn `DocumentationCommentId`), or carrying `[CsToCpp.Weak]` | Escape hatch; the sidecar suits generated code you can't annotate |
| W2 | **Bidirectional association**: `A` holds a collection of `B`, and `B` holds a scalar field typed `A` (or a base of `A`) → the scalar side is weak | `Control.Parent` ↔ `Controls`, `Form.Owner` ↔ `OwnedForms`, `ToolStripItem.Owner` |
| W3 | **Native back-pointer**: a facade object stored in GObject data is weak | `widget.Data["Control"] = this` (`Control.cs:679`, read in `ControlUtility.cs:22`) |
| W4 | **Self-subscription**: `this.<field-path>.Event += <handler capturing this>` captures `this` weakly | `self.Clicked += Self_Clicked` (`Button.cs:26`) |
| W5 | Everything else is strong | — |

### Native lifetime

- Facade wrappers do `g_object_ref_sink`/`g_object_unref` RAII (GtkSharp semantics; see the memory note
  that `Handle != Zero` means the object is alive).
- Signal closures are created with `g_signal_connect_data` and a destroy-notify that frees the `std::function`.
- Open top-level `Form`s are held strongly by `Application::OpenForms` until closed, which matches .NET,
  where the toplevel keeps the form alive.

### Diagnostics (debug builds)

- The emitter generates `__visit_refs(visitor)` per class, used **only** under `CS2CPP_DEBUG_HEAP`.
- A live-object registry records type + creation site. At `Application::Exit` it reports survivors and
  runs a cycle finder that prints each cycle path. Every cycle it finds is fixed by a W1/W2 rule,
  with a corpus test added.

---

## 4. GIR facade (D3)

| Item | Detail |
|---|---|
| Inputs | `Gtk-4.0`, `Gdk-4.0`, `Gsk-4.0`, `GdkPixbuf-2.0`, `Pango-1.0`, `PangoCairo-1.0`, `GLib/GObject/Gio-2.0`, `WebKit-6.0` `.gir` + GtkSharp4 metadata fixups (V2/V3) |
| Output shape | Same namespaces/names as GtkSharp4 (`Gtk::Button`, `Gdk::Pixbuf`, …); GObject properties → `get_X/set_X`; signals → `cs::event<…>` with the GtkSharp `*Args` classes; `GLib::GException` → C++ exception |
| Scope | Only the **symbols the Forms lib actually references** (Roslyn usage scan over `Gtk.*`, `Gdk.*`, `Pango.*`, `GLib.*`, `WebKit.*`: 83/60/18/26/2 files), plus anything converted apps reference |
| Manual | **Cairo** (GtkSharp's Cairo is hand-written Mono.Cairo, not GIR: 19 files / 130 hits) → hand-written C++ over the cairo C API in `facade/manual`; Net4x.GtkSharp's hand-written partials are mirrored there 1:1 |
| Strings | `System::String` ↔ UTF-8 `const char*` conversion at every boundary |
| Shim | `Gtk4Compat` (`Gtk4Compat.cs:20`) is ordinary C# and is converted like the rest of the library |

---

## 5. Converter pipeline

1. **Load**: `MSBuildWorkspace` → `Compilation`; abort on errors (V1).
2. **Analyze**:
   - merge partials;
   - build the type dependency graph (forward declarations, struct ordering);
   - run W1–W4;
   - mark reflection-reachable types;
   - produce the **API usage report** (every BCL/GtkSharp member referenced) → drives the runtime and facade scope;
   - run the unsupported-feature diagnostics (error codes `CS2CPP0xxx`).
3. **Emit**:
   - one `.hpp`/`.cpp` per type and a forward-declaration header per namespace;
   - names that are C++ keywords are escaped;
   - generic code goes to headers;
   - one `CMakeLists.txt` per project; `ProjectReference` → `target_link_libraries`; `PackageReference` → a mapping table (known package → runtime/facade/forms target, unknown → error).
4. **Resources**: `.resx` → C++ blobs + `System::Resources::ResourceManager`/`ComponentResourceManager`
   (this replaces `BinaryFormatter`; it is the native equivalent of what `ResxProcessor` does today).
5. **Format**: `clang-format` the output, so diffs between conversions stay stable.

---

## 6. Runtime scope (C++20, `runtime/`)

Scope = the union of the usage reports from the Forms lib and the target apps. Nothing is written
that no code calls.

| Area | Content | Source |
|---|---|---|
| Core | `Object`, `String`, `Array`, boxing, delegates/events, exceptions, `Math`, `Convert`, primitive `Parse`/`ToString(format, culture)` | Hand-written |
| Collections | Generic + non-generic, `ObservableCollection`, `BindingList` | Transpile the **mono** submodule classes where they are pure managed code; hand-write the intrinsics |
| Text/IO | `StringBuilder`, `Encoding` (UTF-8/16/Latin-1), `File`/`Path`/`Directory`/`Stream` over `std::filesystem` | Mixed |
| Threading | `Thread`, `Monitor`, `Timer`, `SynchronizationContext` → GLib main loop, `Task` coroutines | Hand-written |
| Globalization | `CultureInfo` (invariant + current from OS locale), number/date formatting | Hand-written; ICU optional |
| ComponentModel | `Component`, `Container`, `TypeConverter`, attributes as metadata, `INotifyPropertyChanged` | Transpiled + reflection-lite |
| Diagnostics | `Debug`, `Trace`, `Stopwatch`, `GtkLogger` | Hand-written |

---

## 7. Phases

| Phase | Deliverable | Exit criterion |
|---|---|---|
| P0 | V1–V6 spikes | All checks pass, or each failure has a decision recorded |
| P1 | Converter skeleton + core runtime; console C# corpus (classes, interfaces, generics, exceptions, delegates, strings) | Differential tests: the C# run and the C++ run produce **identical stdout** on 3 OSes |
| P2 | Full mapping §2 + weak inference §3 + debug heap report | The corpus covers every row of §2; **zero leaks** reported on the corpus |
| P3 | GIR facade generator + manual Cairo; usage report from the Forms lib | A transpiled GtkSharp4 sample window (button + drawing area) runs natively |
| P4 | Convert **Gtk.Drawing** → `forms/drawing`; transpile `Gtk.Drawing.Tests` (xUnit → GoogleTest via an `Assert` mapping table) | The transpiled tests pass at the same rate as the C# tests |
| P5 | Convert **Gtk.Windows.Forms** → `forms/winforms`; transpile `Gtk.Windows.Forms.Tests`; convert `GtkWinFormsApp` | Tests at C# parity; the sample runs and the `SampleParity` screenshot comparison (C# vs C++) is within tolerance; debug heap report is clean |
| P6 | End-user CLI: `cs2cpp convert MyApp.csproj -o out/` → CMake project consuming `forms` via `find_package(GtkWinForms)` (CMake package config + vcpkg port) | A fresh WinForms app converts and runs on Windows/Linux/macOS in CI |
| P7 | Sync loop: after each `PrepareProjects` → `AlignProjects` run, re-convert, diff `forms/`, run tests | A CI job re-converts on a new pinned `SOURCE_COMMIT` |

Commit at every phase milestone (CLAUDE.md); every fix ships with a test (corpus case or GoogleTest).

---

## 8. Verification

| What | How |
|---|---|
| Language semantics | Differential corpus: `dotnet run` vs the converted native binary, comparing stdout and exit code |
| Converter units | `CsToCpp.Tests` (xUnit): snippet → expected C++ (snapshot) + compile check |
| Runtime | GoogleTest per area; .NET behaviour as the oracle (formatting, overflow, collection ordering) |
| Library parity | Transpiled `Gtk.Drawing.Tests` / `Gtk.Windows.Forms.Tests` pass at C# parity |
| UI parity | `SampleParity` / `GtkWinFormsApp.UITests`-style screenshot comparison, C# build vs native build |
| Memory | Debug heap report: no survivors and no cycles at exit (corpus + samples); ASan/UBSan job on Linux |
| Platforms | CI matrix: MSVC (Windows), GCC (Linux), Apple Clang (macOS) |

---

## 9. Risks

| Risk | Impact | Mitigation |
|---|---|---|
| Weak inference misses a cycle | Leak | Debug cycle report + W1 sidecar; a corpus test for each pattern |
| Weak inference marks an owning ref weak | Object dies early → `NullReferenceException` (no UB, because `cs::weak` locks) | W2 applies only to the scalar side of a bidirectional association; heap report plus the test suites |
| Reflection-heavy designer code (`ApplyResources`, `TypeConverter`) | Missing metadata → runtime failure | Reachability analysis is generous for `Component` types; the report lists any reflection call site whose target is unresolved |
| GtkSharp4 API shape not fully reproducible from GIR | Converted Forms lib doesn't compile | V3; generate only used symbols; a manual partials folder |
| Template/header bloat, slow builds | Long compile times | Generic code only where needed; precompiled headers; C++20 modules later |
| Semantic gaps (signed overflow, `float.ToString("R")`, `default(T)`, generic virtual methods, finalizer resurrection) | Behaviour differs | `-fwrapv`, runtime formatting ported from .NET, diagnostics for anything unsupported |
| UTF-16 ↔ UTF-8 conversions at the GTK boundary | Performance | Cache UTF-8 in `String` on first conversion |
| Upstream/pipeline drift (patches, AlignProjects) | `forms/` goes stale | P7 sync job pinned to `SOURCE_COMMIT` |
| LGPL v3 (V4) | Distribution constraints | Ship `forms` as a shared library; keep the runtime/converter separately licensed |
