namespace Align.Common;

/// <summary>
/// Everything that distinguishes one alignment tool from another: what is merged (<see cref="Scope"/>),
/// how the transformed trees are produced (<see cref="Transform"/>), where the checkpoint lives, and the
/// labels/branch names used in reports. The driver itself (<see cref="AlignmentDriver"/>) is identical
/// for AlignProjects (Gtk.* libraries) and AlignSamples (the sample apps) - see
/// <c>AlignSamples-Plan.md</c> Phase 0.
/// </summary>
public sealed class AlignmentOptions
{
    /// <summary>Repository root the live <c>OURS</c> tree lives in (also the git working directory).</summary>
    public required string RepoRoot { get; init; }

    /// <summary>Tool name for report headers, the conflict log and the checkpoint comment (e.g. "AlignProjects").</summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Repo-relative directories the merge covers - and the ONLY paths the delta may ever replace
    /// (e.g. <c>Gtk.Windows.Forms</c> + <c>Gtk.Drawing</c>, or the two sample apps).
    /// </summary>
    public required IReadOnlyList<string> Scope { get; init; }

    /// <summary>Full path of this tool's committed <c>last-aligned.txt</c> checkpoint (per-tool, never shared).</summary>
    public required string CheckpointFile { get; init; }

    /// <summary>
    /// Runs the transform against a staging root: the delegate receives the staging root path and must
    /// produce the <see cref="Scope"/> directories inside it. Keeps the driver independent of which
    /// transform (<c>f</c> or <c>fs</c>) is being run.
    /// </summary>
    public required Func<string, Task> Transform { get; init; }

    /// <summary>Name of the upstream submodule directory the staging worktrees are checked out from.</summary>
    public string SubmoduleName { get; init; } = "gtksystem-windows-forms";

    /// <summary>
    /// This tool's own directory name under the repo root - holds <c>build/</c> (the produced BASE/THEIRS
    /// trees) and the checkpoint. Never mirrored into a staging root. Defaults to <see cref="ToolName"/>.
    /// </summary>
    public string? ToolDirectoryName { get; init; }

    /// <summary>Prefix of the throwaway merge branches (<c>&lt;prefix&gt;/base</c>, <c>&lt;prefix&gt;/theirs</c>).</summary>
    public string BranchPrefix { get; init; } = "align";

    /// <summary>Name of the solution-directory log listing conflicts that need hand resolution.</summary>
    public string ConflictLogName { get; init; } = "conflicts.log";

    /// <summary>
    /// Abort (exit 2) when the structural preflight finds layout differences the rename/move/split
    /// reducer cannot explain. Off by default: <c>OURS = f(base) + patches</c>, and patches legitimately
    /// rename, move, split and delete files in both the libraries and the samples, so residual layout
    /// drift is normal and only worth reporting. The 3-way merge plus the conflict pause are the real
    /// safety net.
    /// </summary>
    public bool AbortOnStructuralDrift { get; init; }

    /// <summary>
    /// Optional provenance source used when the transform emits no <c>provenance.tsv</c>: receives the
    /// produced THEIRS tree and returns generated-file → upstream-source-file. Tool-specific because the
    /// upstream layout differs (library <c>Source/...</c> vs <c>Samples/...</c>).
    /// </summary>
    public Func<string, IReadOnlyDictionary<string, string>>? ProvenanceFallback { get; init; }

    /// <summary>
    /// Optional extra post-merge verification (AlignSamples' culture-completeness check). Returns the
    /// failures to report; a non-empty result makes the run exit non-zero, exactly like residual Chinese.
    /// </summary>
    public Func<IReadOnlyList<string>>? PostMergeChecks { get; init; }

    /// <summary>The tool directory's full path (<see cref="ToolDirectoryName"/> under <see cref="RepoRoot"/>).</summary>
    public string ToolDirectory => Path.Combine(RepoRoot, ToolDirectoryName ?? ToolName);

    /// <summary>Where the produced BASE/THEIRS trees are kept - stable, short paths the operator can diff.</summary>
    public string BuildDirectory => Path.Combine(ToolDirectory, "build");

    /// <summary>Dedicated output directory for the determinism self-check, so a pending alignment's trees survive.</summary>
    public string SelfCheckDirectory => Path.Combine(ToolDirectory, "build-selfcheck");
}
