# Quarantined patches

Files here are **not** applied by the pipeline: `GitPatchApplier` globs `patches/*.patch`
(non-recursive), and these carry a `.corrupt` extension besides.

A patch lands here when it is damaged beyond repair. Leaving it in the series is worse than
removing it: `git apply` is atomic per patch, so a corrupt file contributes nothing and only adds a
red line to the end of a pipeline run that is easy to stop reading.

---

## `0006-Cleaned-code.patch.corrupt`

**What is wrong.** The file is not a patch. It is a *rendered dump* of one: it begins with two junk
lines — `File: patches\0006-Cleaned-code.patch` and a markdown code fence — carries CRLF endings
where every sibling has LF, and has lost most of its content in transit:

| | |
|---|---|
| Files listed in its diffstat | **130** |
| `diff --git` blocks actually present | **12** |
| Hunk headers truncated (e.g. `@@ -307`, `@@ -546,7 +546`) | 7 |

Surviving hunks are damaged too — one adds `for (var i = 0; i < typeCount; i++) ;`, a loop whose body
block was elided.

**Why it was not repaired.** Truncated hunk headers can be recomputed from their bodies, but ~90% of
the file diffs are simply absent, and the commit named in its header
(`c801188f845bc15905fb20c2e7e7acb26106d12e`) is not in this repository's history, so
`git format-patch` cannot regenerate it. Reconstructing the missing 118 file diffs by inference would
mean guessing, and applying a half-reconstructed patch is worse than applying none.

**What was lost.** The patch was a cleanup pass over the resource-reader code. Its changes are absent
from the generated tree, which is how the corruption was found — for example
`GTKSystem/IO/Resources/BinaryReaderExtensions.cs` and `PinnedBufferMemoryStream.cs` still declare
`namespace GtkSystem.IO;` where the patch moved them to `GtkSystem.IO.Resources`, and
`DeserializingResourceReader` still calls its local `num` rather than `typeCodeOrIndex`. None of it is
behavioural; it is naming and namespace tidying that never reached the library.

**To restore it**, redo the cleanup against the current generated tree and capture a fresh patch with
the [Patches.txt](../../Patches.txt) recipe. Do not try to salvage this file.

**How this is prevented from recurring.** `PatchSeriesIntegrityTests` in `Prepare.Tests` asserts that
every file in `patches/` is a well-formed mailbox patch whose diffstat entry count matches the number
of file diffs it actually contains, and that every hunk header is complete. That test fails on this
file, which is why it is quarantined here rather than left in the series.
