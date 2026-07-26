# Plan — remove the `Net4x` prefix from projects, folders, assemblies, and make the pipeline aware

**Goal.** Drop the `Net4x` token everywhere it names something: project folders, `.csproj`
filenames, output assembly names, the two `.sln` files, MSBuild properties, C# namespaces (tests +
tool), the NuGet id that still carries it (`Net4x.ResxProcessor`), the `Resources/` overlay files,
the 132 patch files, and all Markdown. `PrepareProjects`, `AlignProjects`, and `AlignSamples` must
continue to regenerate and align the tree under the new names.

**Decisions taken (from the clarifying round).**

1. **Depth** — rename directories + `.csproj` + `AssemblyName` **and** `RootNamespace` + the `Net4x.*`
   C# namespaces. (In practice the shipped libraries have *empty* `RootNamespace` and no `Net4x.*`
   namespaces, so the namespace half only touches the test projects and `Net4x.ResxProcessor`.)
2. **PackageIds** — the library ids are already `Gtk.*`; additionally rename the one package that still
   carries the token: **`Net4x.ResxProcessor` → `ResxProcessor`** (breaking; needs a version bump + note).
3. **Patches** — physically **rewrite the path prefixes inside all patch files** (no pipeline shim).
4. **Breadth** — remove the exact prefix **`Net4x.`** (with the trailing dot) wherever it names
   something: projects, tests, tools, `.sln`, the `Resources/` overlays, the patches, and namespaces.
   Tokens that contain `Net4x` **without** the dot are **out of scope and left unchanged**: the
   `GtkWindowsFormsVersion` / `Net4xBaseTypesVersion` MSBuild properties, the `Net4xGtkWinform`
   template folder, and pipeline identifiers such as `net4xGtkDrawing`, `Net4xFullPath`,
   `OptimizeNet4xGtkDrawingFileSystem`.

---

## 0. BLOCKING verification — do these before any rename

These are assumptions that, if wrong, invalidate the mechanical steps below. Check each first.

- **B1 — External references to the old `.sln` / folder names.** The primary solution file is renamed
  (`Net4x.Gtk.Windows.Forms.sln` → `Gtk.Windows.Forms.sln`). Grep for it outside the source tree —
  CI/workflow YAML, `*.cmd`/`*.ps1`/`*.sh` launchers, `.vscode/*.json`, `.idea/*`, git hooks,
  `launchSettings.json`, and the `Nuget.Config`/`Packages` symlink target. Anything naming the old
  `.sln` or a project folder must be updated in the same change or the build breaks for CI only.
  ```sh
  git grep -n "Net4x.Gtk.Windows.Forms.sln"
  grep -rIn "Net4x" .github .vscode .idea *.ps1 *.cmd *.sh 2>/dev/null
  ```
- **B2 — Confirm the out-of-scope `Net4x`-without-dot tokens really have no dot.** The prefix removed is
  exactly `Net4x.`; the MSBuild properties and the template folder use `Net4x` glued to the next word, so
  they stay. Sanity-check nothing was missed:
  ```sh
  git grep -n "Net4x\." -- ':(exclude)gtksystem-windows-forms/**' ':(exclude)mono/**'   # in scope
  git grep -n "Net4x[^.]" -- ':(exclude)gtksystem-windows-forms/**' ':(exclude)mono/**' # out of scope — verify each is intentional
  ```
- **B3 — `ResxProcessor` package-id change is a breaking publish.** Its current default id is
  `Net4x.ResxProcessor`. After the rename, first-time consumers get `ResxProcessor`; existing
  consumers keep resolving the old id until they retarget. Requires a package version bump and a
  changelog/README note. Confirm the id `ResxProcessor` is free on the target feed(s).
- **B4 — Run from a clean, known-good pipeline state.** The rename touches the *generated* trees
  (`Net4x.Gtk.Windows.Forms/`, `Net4x.Gtk.Drawing/`) and their `.LatestBackup` snapshots, plus the 132
  patches that target them. Start from a clean `git status` immediately after a successful
  `PrepareProjects` run, so the "regenerate and confirm identical output" check in §12 is meaningful.
- **B5 — Renames are pure prefix drops (no case-only renames)**, so Windows' case-insensitive FS and
  `git mv` are both safe. The one folder that becomes BCL-shaped —
  `Net4x.System.Resources.Extensions` → `System.Resources.Extensions` — is intentional (its
  `AssemblyName` is already `System.Resources.Extensions`); it is a folder name only and does not shadow
  the real assembly.

---

## 1. Canonical name map

Apply this map everywhere. **Longest keys first** when doing textual replacement, so
`…Forms.Tests` is rewritten before `…Forms`.

| Old | New | Kind |
|---|---|---|
| `Net4x.Gtk.Windows.Forms.Tests` | `Gtk.Windows.Forms.Tests` | folder, csproj, assembly, C# namespace root |
| `Net4x.Gtk.Windows.FormsDesigner` | `Gtk.Windows.FormsDesigner` | folder, csproj, assembly (RootNamespace already `Gtk.Windows.FormsDesigner`) |
| `Net4x.Gtk.Windows.Forms` | `Gtk.Windows.Forms` | folder, csproj, assembly |
| `Net4x.Gtk.Windows.Extensions` | `Gtk.Windows.Extensions` | folder, csproj, assembly |
| `Net4x.Gtk.Drawing.Tests` | `Gtk.Drawing.Tests` | folder, csproj, assembly |
| `Net4x.Gtk.Drawing` | `Gtk.Drawing` | folder, csproj, assembly |
| `Net4x.System.Resources.Extensions` | `System.Resources.Extensions` | folder, csproj (assembly already `System.Resources.Extensions`) |
| `Net4x.ResxProcessor.Tests` | `ResxProcessor.Tests` | folder, csproj, assembly, C# namespace root |
| `Net4x.ResxProcessor` | `ResxProcessor` | folder, csproj, assembly, C# namespace root, **PackageId** |
| `Net4x.Gtk.Windows.Forms.LatestBackup` | `Gtk.Windows.Forms.LatestBackup` | generated backup folder (tracked) |
| `Net4x.Gtk.Drawing.LatestBackup` | `Gtk.Drawing.LatestBackup` | generated backup folder (tracked) |
| `Net4x.Gtk.Windows.Forms.sln` | `Gtk.Windows.Forms.sln` | solution file |

**Unchanged on purpose** (no `Net4x.` dotted prefix, or deliberately excluded):

- Tokens without the dot: `GtkWindowsFormsVersion`, `Net4xBaseTypesVersion` (MSBuild properties),
  the `Net4xGtkWinform` template folder, and pipeline identifiers `net4xGtkDrawing`, `Net4xFullPath`,
  `OptimizeNet4xGtkDrawingFileSystem`. *(A string **value** like `net4xGtkDrawing = "Net4x.Gtk.Drawing"`
  still changes — the value carries the dotted prefix even though the identifier does not.)*
- The public API namespaces (`System.Windows.Forms`, `System.Drawing`, `GtkSystem.*`), the
  already-`Gtk.*` PackageIds (`Gtk.Windows.Forms`, `Gtk.Drawing`, `Gtk.Windows.FormsDesigner`,
  `Gtk.System.Resources.Extensions`, `Gtk.Windows.Extensions`), and the `gtksystem-windows-forms` /
  `mono` submodules.

---

## 2. Rename folders and `.csproj` files (git-tracked moves)

Use `git mv` so history follows. For each row of §1 that is a folder+csproj:

```sh
git mv Net4x.Gtk.Windows.Forms                 Gtk.Windows.Forms
git mv Gtk.Windows.Forms/Net4x.Gtk.Windows.Forms.csproj  Gtk.Windows.Forms/Gtk.Windows.Forms.csproj
# …repeat for Drawing, FormsDesigner, Extensions, System.Resources.Extensions,
#   ResxProcessor, and the three *.Tests projects, and the two *.LatestBackup dirs.
git mv Net4x.Gtk.Windows.Forms.sln             Gtk.Windows.Forms.sln
# NOTE: the Net4xGtkWinform template folder has no dotted prefix — leave it.
```

Also rename the two Designer sidecars already present under the (now-renamed) Designer folder if they
still carry the token, and the per-project XML doc files that carry it:

- `Net4x.Gtk.Windows.FormsDesigner/Net4x.Gtk.Windows.FormsDesigner.xml` → `.../Gtk.Windows.FormsDesigner.xml`
- `Net4x.ResxProcessor/Net4x.ResxProcessor.xml` → `ResxProcessor/ResxProcessor.xml`
  (`System.Resources.Extensions/System.Resources.Extensions.xml` is already token-free.)

> The generated in-tree `Gtk.Windows.Forms.props`/`.targets` inside the Forms/Designer folders are
> **already** token-free (see §8's `StructuralMatchReducer`), so they only move with their folder.

---

## 3. Fix identity fields inside `.csproj`

The output assembly name defaults to the project filename, so §2 already renames every output `.dll`
**except** the two projects that pin it explicitly — leave those as-is (they are already token-free):

- `Net4x.System.Resources.Extensions.csproj:8` → `<AssemblyName>System.Resources.Extensions</AssemblyName>` (keep)
- `Net4x.Gtk.Windows.FormsDesigner.csproj:9` → `<RootNamespace>Gtk.Windows.FormsDesigner</RootNamespace>` (keep)

Edits required:

- **`Gtk.Windows.FormsDesigner.csproj:14`** — `<Product>Net4x.Gtk.Windows.FormsDesigner</Product>` →
  `<Product>Gtk.Windows.FormsDesigner</Product>`.
- **`ResxProcessor.csproj`** — add/adjust `<PackageId>ResxProcessor</PackageId>` (was defaulting to
  `Net4x.ResxProcessor`) and bump its package version; add a changelog/README line (decision B3).
- **`Gtk.Windows.Forms.csproj:45,48`** and **`Gtk.Drawing.csproj:45,48,58`** — the
  `InternalsVisibleTo` `AssemblyAttribute` values:
  - `Net4x.Gtk.Windows.Forms.Tests` → `Gtk.Windows.Forms.Tests`
  - `Net4x.Gtk.Windows.Extensions` → `Gtk.Windows.Extensions`
  (Public-key blobs are unchanged — the strong-name key `GtkForms.snk` is not touched.)
- **`<ProjectReference>` paths** across the whole solution graph — every reference that points at a
  renamed folder/csproj. Rewrite mechanically:
  ```sh
  git ls-files '*.csproj' | xargs sed -i \
    -e 's#Net4x\.Gtk\.Windows\.Forms\.Tests#Gtk.Windows.Forms.Tests#g' \
    -e 's#Net4x\.Gtk\.Windows\.FormsDesigner#Gtk.Windows.FormsDesigner#g' \
    -e 's#Net4x\.Gtk\.Windows\.Forms#Gtk.Windows.Forms#g' \
    -e 's#Net4x\.Gtk\.Windows\.Extensions#Gtk.Windows.Extensions#g' \
    -e 's#Net4x\.Gtk\.Drawing\.Tests#Gtk.Drawing.Tests#g' \
    -e 's#Net4x\.Gtk\.Drawing#Gtk.Drawing#g' \
    -e 's#Net4x\.System\.Resources\.Extensions#System.Resources.Extensions#g' \
    -e 's#Net4x\.ResxProcessor\.Tests#ResxProcessor.Tests#g' \
    -e 's#Net4x\.ResxProcessor#ResxProcessor#g'
  ```
  (The ordered longest-first list is reused verbatim in §6 and §7.) Then re-check the two "keep"
  fields above were not clobbered.
- **XML-doc filename references** — `<DocumentationFile>` and any `Net4x.*.xml` mentioned in a csproj
  are covered by the same sed; verify the two Resources overlays it copies (§6).

---

## 4. Solution files

Both `.sln` files list projects by display name **and** relative path. Rewrite both with the same
ordered map:

```sh
sed -i -f rename.sed Gtk.Windows.Forms.sln PrepareProjects.sln   # rename.sed = the 9 s### rules above
```

`Net4x.Gtk.Windows.Forms.sln` (main) has 10 project rows (lines 19–47); `PrepareProjects.sln` has 9
`Net4x` occurrences. Project GUIDs are unchanged — only names and paths. Verify each solution still
opens and all `ProjectSection` build configs resolve.

---

## 5. MSBuild property manifests

The property **names** `GtkWindowsFormsVersion` / `Net4xBaseTypesVersion` have no dotted prefix, so
they are **left unchanged** — `Directory.Nuget.Props:20,26`, `Directory.Build.Props:5`, and every
`$(GtkWindowsFormsVersion)` reference stay as-is. The only edit here is prose:

- **`Directory.Build.Props:34`** — the comment names the project folder
  `Net4x.System.Resources.Extensions` (a dotted prefix, in prose) → update to `System.Resources.Extensions`.
  Re-scan `Directory.*.Props` for any other comment mentioning a renamed folder/csproj/sln and fix the text.

---

## 6. C# namespaces (tests + tool only) and `Resources/` overlays

**Namespaces.** Only three projects declare `namespace Net4x.*` (the shipped libraries do not):

- `Gtk.Windows.Forms.Tests` — ~75 files, `namespace Net4x.Gtk.Windows.Forms.Tests[.*]`
- `ResxProcessor` — 2 files, `namespace Net4x.ResxProcessor`
- `ResxProcessor.Tests` — 4 files, `namespace Net4x.ResxProcessor.Tests[.Internals]`

Rewrite the `namespace`/`using` lines with the §3 ordered sed over those projects' `*.cs`:
```sh
git ls-files 'Gtk.Windows.Forms.Tests/**/*.cs' 'ResxProcessor/**/*.cs' 'ResxProcessor.Tests/**/*.cs' \
  | xargs sed -i -f rename.sed
```
The test files' `using System.Windows.Forms;` etc. are untouched (no `Net4x` token). Confirm no shipped
library file gained a `Net4x` namespace by accident (`git grep -n "namespace Net4x"` → empty).

**`Resources/` overlays** (copied into the generated tree by `ResourcesCopyService`): rename the nine
tracked files and fix any self-referential paths inside them:

| Old | New |
|---|---|
| `Resources/Net4x.Gtk.Drawing.csproj` | `Resources/Gtk.Drawing.csproj` |
| `Resources/Net4x.Gtk.Drawing.xml` | `Resources/Gtk.Drawing.xml` |
| `Resources/Net4x.Gtk.Drawing.LatestFiles.xml` | `Resources/Gtk.Drawing.LatestFiles.xml` |
| `Resources/Net4x.Gtk.Windows.Forms.xml` | `Resources/Gtk.Windows.Forms.xml` |
| `Resources/Net4x.Gtk.Windows.Forms.Files.xml` | `Resources/Gtk.Windows.Forms.Files.xml` |
| `Resources/Net4x.Gtk.Windows.Forms.IntermediateFiles.xml` | `Resources/Gtk.Windows.Forms.IntermediateFiles.xml` |
| `Resources/Net4x.Gtk.Windows.Forms.LatestFiles.xml` | `Resources/Gtk.Windows.Forms.LatestFiles.xml` |
| `Resources/Net4x.Gtk.Windows.Forms.props` | `Resources/Gtk.Windows.Forms.props` |
| `Resources/Net4x.Gtk.Windows.Forms.targets` | `Resources/Gtk.Windows.Forms.targets` |

The `*Files.xml` / `*LatestFiles.xml` provenance manifests contain **repo-relative generated paths**
(`Net4x.Gtk.Windows.Forms/System/...`) — rewrite their contents with the §3 ordered sed as well.
Also the `Prepare.Tests/Roslyn/Net4x.Gtk.Windows.Forms.xml` test fixture → `Gtk.Windows.Forms.xml`.

---

## 7. Patch files — rewrite path prefixes (132 files, ~4202 refs)

Every patch targets paths under the old folders (`diff --git a/Net4x.Gtk.Windows.Forms/… b/…`, `---`,
`+++`, `rename from/to`). Rewrite **only the diff header/path lines** (not arbitrary content that may
legitimately mention `Net4x`), longest-first:

```sh
for p in patches/*.patch; do
  sed -i -E '/^(diff --git |--- |\+\+\+ |rename (from|to) |copy (from|to) )/ {
    s#Net4x\.Gtk\.Windows\.Forms\.Tests/#Gtk.Windows.Forms.Tests/#g
    s#Net4x\.Gtk\.Windows\.FormsDesigner/#Gtk.Windows.FormsDesigner/#g
    s#Net4x\.Gtk\.Windows\.Forms/#Gtk.Windows.Forms/#g
    s#Net4x\.Gtk\.Windows\.Extensions/#Gtk.Windows.Extensions/#g
    s#Net4x\.Gtk\.Drawing\.Tests/#Gtk.Drawing.Tests/#g
    s#Net4x\.Gtk\.Drawing/#Gtk.Drawing/#g
    s#Net4x\.System\.Resources\.Extensions/#System.Resources.Extensions/#g
    s#Net4x\.ResxProcessor\.Tests/#ResxProcessor.Tests/#g
    s#Net4x\.ResxProcessor/#ResxProcessor/#g
  }' "$p"
done
```

A patch whose *body* also adds/edits a namespace line (e.g. a test-project patch) additionally needs
the namespace token rewritten in its `+`/`-` content — handle those by re-running the affected patch's
originating change through `git format-patch` after §6, **or** extend the sed above to the specific
content lines for the handful of test patches. Identify them:
```sh
git grep -ln "namespace Net4x" -- 'patches/*.patch'
```

**Verification is deferred to the pipeline** (§12): patches apply against the *generated* tree during
`GitPatchApplier.CommitAndApplyPatches()`, not the working tree, so the real check is a clean
`PrepareProjects` run producing no patch-apply rejects.

---

## 8. Make `PrepareProjects` aware

The orchestrator has ~78 references — mostly hardcoded folder/file literals, plus a few that need
thought. Rewrite the literals with the §3 ordered sed over `PrepareProjects/**/*.cs`, then hand-fix
these semantic sites:

- **`PrepareWorker.cs:12`** — `public const string net4xGtkDrawing = "Net4x.Gtk.Drawing";` → change the
  **value** to `"Gtk.Drawing"`. The *identifier* `net4xGtkDrawing` has no dotted prefix, so **keep it**
  (leaving its uses on line 34 etc. untouched).
- **`ProjectPreparation.cs:33,39,46`** — the pipeline **renames the upstream csproj** to the Net4x
  name. Change the target to `Gtk.Windows.Forms.csproj` (+`.user`) and the log string on `:46`
  (`… -> Gtk.Windows.Forms.csproj`).
- **`ResGenService.cs:21`**, **`DrawingService.cs:9,13,25,34`**, **`PrepareWorker.cs:213,252,293,297,
  299,301,302,303,305,306,309,311–326,371,373`** — folder/csproj/xml literals; the sed covers them,
  but re-read each to confirm the replacement kept a valid path (several build paths via
  `RepoPaths.At(...)` and `Path.Combine`). `PrepareWorker.cs:252` also references the **solution
  filename** `..\Net4x.Gtk.Windows.Forms.sln` → `..\Gtk.Windows.Forms.sln`.
- **`GtkFoldersService.cs:37`** — backup-folder base name literal `"Net4x.Gtk.Windows.Forms"` →
  `"Gtk.Windows.Forms"`; this is what produces the `*.LatestBackup` dir name, so it must match §2's
  renamed backup folder.
- **`OptimizeNet4xGtkDrawingFileSystem`** (`PrepareWorker.cs:309`) — the *method name* has no dotted
  prefix, so **keep it**. Only the `"Net4x.Gtk.Drawing\..."` path literals in its body change (via the sed).

`Prepare.Common` (~23 refs) — mostly XML-doc comments describing the `Net4x.*` layout, but two are
**behavioural**:

- **`GeneratedTreeGuard.cs:20–21`** — the `GeneratedRoots` array literals `"Net4x.Gtk.Windows.Forms/"`,
  `"Net4x.Gtk.Drawing/"` → `"Gtk.Windows.Forms/"`, `"Gtk.Drawing/"`. (Guards hand-edits to the
  generated trees; must track the new folder names or the guard silently stops protecting them.)
- **`StructuralMatchReducer.cs:46–56` (`NormName`)** — currently strips a `Net4x.` prefix from
  `.props`/`.targets` filenames during alignment. After §6 the overlays are already token-free, so this
  normalisation becomes a **no-op that must be removed** (or repointed), otherwise a future
  `Gtk.Windows.Forms.props` is left unchanged (fine) but the dead branch is misleading. Simplest:
  delete the `StartsWith("Net4x.")` branch, returning `Path.GetFileName(relPath)` directly. Re-read the
  callers to confirm nothing else relied on the stripping.
- **`VersionPrepare.cs:69`** — the string literal `"GtkWindowsFormsVersion"` has **no dotted
  prefix**; the property is not renamed (§5), so **leave this unchanged**.
- **`ConflictReporter.cs` / `FilesXmlProvenance.cs` / `ProvenanceManifest.cs` etc.** — the `Net4xFullPath`
  identifiers have no dotted prefix → **keep them**. Their doc comments contain dotted forms like
  `Net4x.*` / `Net4x.Gtk.Drawing` (prose describing the layout); updating that prose is optional polish,
  not required by the rename.

`Prepare.Tests` (~62 refs) — fixtures and asserted path/name strings. Rewrite with the sed, then run
the suite; several tests assert the *exact* generated folder/csproj/property names and will (correctly)
fail until updated — that failure set is the test oracle that the pipeline changes are complete.

---

## 9. Make `AlignProjects` / `AlignSamples` aware

Their tracked C# source has **no direct `Net4x` literal** — they drive alignment through
`Align.Common` + `Prepare.Common` (whose `GeneratedTreeGuard`, `StructuralMatchReducer`,
`FilesXmlProvenance`, `ProvenanceManifest` are fixed in §8) and through the `Resources/*Files.xml`
manifests (renamed + content-rewritten in §6). So making §6 and §8 correct **is** making
Align-aware. Explicit checks:

- The provenance manifests (`Gtk.Windows.Forms.LatestFiles.xml`, `Gtk.Drawing.LatestFiles.xml`, the
  `Files`/`IntermediateFiles` pair) resolve to the renamed generated roots — verified by
  `FilesXmlProvenance.ProjectForManifest` returning the new folder (`…LatestFiles` → `Gtk.Drawing`).
- `AlignProjects`' checkpoints and `AlignSamples`' resx culture-invariant step reference generated
  paths only through the manifests — no separate literal to change.
- Run each post-pipeline in **dry-run** (or against a scratch worktree) after `PrepareProjects` to
  confirm the 3-way merge still finds `OURS` at the new paths (`StructuralMatch` compares the live
  `Gtk.*` tree). Update `AlignProjects.md` / `AlignSamples.md` prose (§10).

---

## 10. Markdown

Update every doc that names the token (~17 files). Prose + code-fence paths + tables:

`CLAUDE.md`, `ReadMe.md`, `Architecture.md`, `Solution-Projects.md`, `Solution-Review.md`,
`PrepareProjects.md`, `AlignProjects.md`, `AlignSamples.md`, `API-COMPLETENESS.md`,
`EMULATION-GAPS.md`, `api-diff/README.md`, `Resources/README.md`,
`Resources/Gtk.Windows.Forms.Article.md`, `Resources/SystemResourcesExtensions.README.md`, and the
per-project READMEs under the renamed folders (`Gtk.Windows.Extensions/README.md`,
`Gtk.Windows.FormsDesigner/README.md`, `System.Resources.Extensions/README.md`).

- Apply the §3 ordered sed to `*.md` (excluding the submodules), **but review each diff** — Markdown
  prose sometimes discusses the *package* id (`Gtk.Windows.Forms`) vs the *project*; keep those
  correct. In particular `api-diff/README.md` and the `Regenerating` block reference the impl folder
  path `Net4x.Gtk.Windows.Forms/bin/Release/...` → `Gtk.Windows.Forms/bin/...`.
- `CLAUDE.md` — update the "two solutions" table (the `.sln` name), the generated-dir names, and the
  patch recipe path. *(The `GtkWindowsFormsVersion` mention stays — that property is not renamed.)*
- `api-diff/tool/Program.cs` command examples / any embedded default paths (if present) → new folder.
- Add a short "Renamed from `Net4x.*`" note to `ReadMe.md` and the `ResxProcessor` README (package-id
  change, decision B3).

Regenerate the api-diff data (`api-diff/*.summary.tsv` etc. are gitignored) so headline numbers still
match — the rename does not change the surface, but the impl DLL name in the tool invocation does.

---

## 11. Tracked `.LatestBackup` snapshots

`Net4x.Gtk.Windows.Forms.LatestBackup/` and `Net4x.Gtk.Drawing.LatestBackup/` are pipeline-generated
but **committed**. `git mv` them (§2) so the tree stays consistent; the next `PrepareProjects` run
recreates them under the new names via the `GtkFoldersService.cs:37` literal fixed in §8. If you prefer
not to carry generated snapshots through the rename, deleting them and letting the pipeline recreate is
equivalent — decide once and note it in the commit.

---

## 12. Verification (in order)

1. **Solutions load & build:**
   ```sh
   dotnet build Gtk.Windows.Forms.sln -c Release -p:GeneratePackageOnBuild=false
   ```
   Zero missing-project / unresolved-`ProjectReference` errors; output DLLs are `Gtk.Windows.Forms.dll`,
   `Gtk.Drawing.dll`, `ResxProcessor.dll`, etc.
2. **Tests green:**
   ```sh
   dotnet test Gtk.Windows.Forms.sln
   ```
   The `Gtk.Windows.Forms.Tests` / `Gtk.Drawing.Tests` assemblies resolve internals (IVT rename in §3
   correct). Full Forms suite (2523) + Drawing (1216) still pass.
3. **No stray dotted prefix** outside the submodules and this plan file:
   ```sh
   git grep -n "Net4x\." -- ':(exclude)gtksystem-windows-forms/**' ':(exclude)mono/**' \
     ':(exclude)Rename-Remove-Net4x-Prefix-Plan.md'
   ```
   Should return only intentional prose keeps (e.g. layout-describing doc comments) — investigate each
   hit. Bare `Net4x`-without-dot tokens (`GtkWindowsFormsVersion`, `Net4xBaseTypesVersion`,
   `Net4xGtkWinform`, `net4xGtkDrawing`, `Net4xFullPath`, `OptimizeNet4xGtkDrawingFileSystem`) are
   expected to remain:
   ```sh
   git grep -n "Net4x[^.]" -- ':(exclude)gtksystem-windows-forms/**' ':(exclude)mono/**'  # verify all intentional
   ```
4. **Pipeline round-trips:** from clean state run
   ```sh
   SkipPush=1 dotnet run --project PrepareProjects/PrepareProjects.csproj
   ```
   Expect: upstream csproj renamed to `Gtk.Windows.Forms.csproj`; all patches apply with **zero
   rejects**; generated `Gtk.Windows.Forms/` + `Gtk.Drawing/` reproduced; `git status` clean (no
   unexpected diff vs. the renamed committed tree).
5. **Prepare.Tests** pass (path/name assertions updated in §8).
6. **AlignProjects / AlignSamples** dry-run after step 4: 3-way merge locates `OURS` at the new paths;
   `AlignSamples` culture-invariant check still holds.
7. **Pack** the renamed `ResxProcessor` and confirm the `.nupkg` id is `ResxProcessor` and the shipped
   `tools/` payload is intact.

---

## 13. Risks & notes

- **Patch drift (highest risk).** The §7 header-only sed is safe against false positives, but a patch
  whose body edits a `namespace Net4x.*` line (test patches) needs body rewriting too — enumerate with
  `git grep -ln "namespace Net4x" -- 'patches/*.patch'` and handle explicitly. A missed one surfaces as
  a pipeline apply-reject in §12.4, not a silent corruption.
- **Backup-name coupling.** `GtkFoldersService.cs:37`'s base-name literal must match the renamed
  `*.LatestBackup` folder (§2/§11), or the pipeline creates a second backup dir under the old name.
  *(The version-property names are **not** renamed — no `VersionPrepare.cs:69` ↔ props coupling to worry
  about.)*
- **`ResxProcessor` package break (B3).** Downstream consumers of `Net4x.ResxProcessor` must retarget;
  ship a version bump + changelog. If the id must stay for compat, reconsider decision B2/B3 for this one
  package.
- **External tooling (B1).** CI YAML, IDE run configs, and the `Packages` symlink may name the old
  `.sln`/folders; these live outside the build graph and won't fail a local `dotnet build`.
- **`System.Resources.Extensions` folder name** now visually matches the BCL assembly. It is a folder
  only; the csproj still packs as `Gtk.System.Resources.Extensions` and the assembly is
  `System.Resources.Extensions` (unchanged). No functional collision, but note it in `CLAUDE.md` to
  avoid confusion.
- **Do the rename as one atomic commit** (or a tight sequence) on a branch — a half-renamed tree does
  not build, and the pipeline's clean-status guard (`GeneratedTreeGuard`) will refuse to run mid-rename.

---

## 14. Suggested commit sequence (one branch)

1. `git mv` folders/csproj/sln/template + tracked overlays and backups (§2, §6, §11).
2. Textual rewrites: csproj identity + ProjectReferences (§3), solutions (§4), props (§5), namespaces
   (§6), pipeline C# (§8), markdown (§10).
3. Patch-file path rewrites (§7).
4. Build + test + `git grep` clean (§12.1–3); fix stragglers.
5. Pipeline round-trip + Prepare.Tests + Align dry-runs (§12.4–6); fix stragglers.
6. `ResxProcessor` version bump + changelog (§3, B3).

Keep steps 1–3 reviewable as separate commits if desired; do **not** merge until §12 is fully green.
