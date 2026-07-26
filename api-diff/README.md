# api-diff — generated data for [API-COMPLETENESS.md](../API-COMPLETENESS.md)

Mechanical public/protected API-surface diff of the shipped assemblies against the real .NET reference
assemblies. **These files are generated** — regenerate rather than hand-edit.

## Files

| Prefix | Comparison |
|---|---|
| `wf.*` | `System.Windows.Forms.dll` + `.Primitives.dll` (WindowsDesktop.App.Ref 10.0.10) vs `Gtk.Windows.Forms.dll` |
| `dr.*` | `System.Drawing.Common.dll` (10.0.6) vs `Gtk.Drawing.dll` |
| `wf48.*` | **net48** `System.Windows.Forms.dll` vs `Gtk.Windows.Forms.dll` |
| `dr48.*` | **net48** `System.Drawing.dll` vs `Gtk.Drawing.dll` |

The `*48` pair is the net48 baseline — it excludes modern additions no ported application can already be
using, and is the truer target (see [API-COMPLETENESS.md](../API-COMPLETENESS.md)). The unsuffixed pair is
the full .NET 10 surface.

| Baseline | Types present | Members present (on present types) |
|---|---|---|
| net48 Forms | 937 / 1034 | 47 056 / 54 529 |
| net10 Forms | 942 / 1081 | 51 887 / 60 356 |
| net48 Drawing | 145 / 189 | 5 475 / 5 543 |
| net10 Drawing | 142 / 183 | 6 507 / 6 604 |

| Suffix | Contents |
|---|---|
| `.missing-types.tsv` | Reference types with no counterpart. `fullName<TAB>kind` |
| `.missing-members.txt` | Per-type missing members, types ordered by gap size |
| `.summary.tsv` | `type<TAB>missingMembers<TAB>totalMembers` |
| `.extra-types.txt` | Types in the shim that the reference assembly does not have |

Member sigs are prefixed `M:` method, `P:` property, `E:` event, `F:` field, `C:` constructor.

## Regenerating

```sh
dotnet build api-diff/tool/ApiDiff.csproj -c Release
dotnet build Gtk.Windows.Forms.sln -c Release   # the assemblies under test

REF=/c/'Program Files'/dotnet/packs/Microsoft.WindowsDesktop.App.Ref/10.0.10/ref/net10.0
IMPL=Gtk.Windows.Forms/bin/Release/netstandard2.0
TOOL=api-diff/tool/bin/Release/net10.0/ApiDiff.exe

"$TOOL" "$REF/System.Windows.Forms.dll" "$IMPL/Gtk.Windows.Forms.dll" \
        "$IMPL" api-diff/wf System.Windows.Forms "$IMPL/Gtk.Drawing.dll"

"$TOOL" ~/.nuget/packages/system.drawing.common/10.0.6/lib/net10.0/System.Drawing.Common.dll \
        "$IMPL/Gtk.Drawing.dll" "$IMPL" api-diff/dr System.Drawing \
        "$IMPL/Gtk.Windows.Forms.dll"
```

For the net48 baseline, point `<refDll>` at
`%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Windows.Forms.dll`
(and `System.Drawing.dll`), writing to prefixes `wf48` / `dr48`.

Usage: `ApiDiff <refDll> <implDll> <implDir> <outPrefix> <nsFilter> [extraImplDlls...]`.

- A `<refName>.Primitives.dll` beside the reference assembly is merged automatically, so type-forwarded
  types (`HighDpiMode`, `Message`) are not reported missing.
- A `.NETFramework` reference path is detected and resolved against `mscorlib` instead of
  `System.Runtime`. The .NET Core ref pack is still loaded (the impl assemblies are `netstandard2.0` and
  need it); assembly paths are deduped by file name so the two packs' `mscorlib.dll` facades do not
  collide.

## Comparison rules

- Public **and protected** members count — WinForms is an inheritance framework.
- **Inherited members count as present** (the whole base chain is walked); constructors are matched
  per-type since they are not inherited. Comparing `DeclaredOnly` inflates the member gap ~3×.
- Signatures match on name + parameter type *names*, so a differently-signatured member reads as
  missing — intentionally, as that is also a compile break.

Not compared: attributes, explicit interface implementations, generic arity, member accessibility
beyond public/protected. See §10 of the analysis for what this method cannot see.
