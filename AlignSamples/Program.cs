using Align.Common;
using Prepare.Common.Alignment;
using Prepare.Common.IO;
using Prepare.Common.Languages;
using Prepare.Common.Logging;
using PrepareProjects;

namespace AlignSamples;

/// <summary>
/// Post-pipeline tool: forward-ports the latest gitee upstream onto the generated <b>sample apps</b>
/// (<c>GtkWinFormsApp</c>, <c>GtkSystemWinFormsApp11</c>) via a transformed-space delta merge —
/// <c>fs(base)</c> vs <c>fs(target)</c>, applied onto the live samples. The counterpart of AlignProjects,
/// whose scope deliberately excludes the samples. See <c>AlignSamples-Plan.md</c>.
///
/// Beyond porting upstream changes it preserves the samples' globalization: new Chinese is keyed and
/// queued for translation before the transform runs (Phase 2), and the culture-awareness invariant is
/// verified after the merge.
/// </summary>
public static class Program
{
    private const string ToolName = "AlignSamples";
    private const string SubmoduleName = "gtksystem-windows-forms";

    /// <summary>Repo-relative directories this tool aligns - the two upstream-derived samples.</summary>
    private static readonly string[] Scope =
        SampleTransformPipeline.Samples.Select(s => s.TargetProjectName).ToArray();

    public static async Task<int> Main()
    {
        // Resolve paths against the exe location (repo root is 4 levels up), so the tool works
        // regardless of the current working directory.
        var repoRoot = Path.GetFullPath(Environment.GetEnvironmentVariable("ALIGN_REPO_ROOT")
                                        ?? Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
        var resourcesDir = Path.Combine(repoRoot, "Resources");
        var checkpointFile = Path.Combine(repoRoot, ToolName, "last-aligned.txt");

        // --- Configuration (env-var overridable) ---
        // Base = the last commit this tool already absorbed. Its own checkpoint, separate from
        // AlignProjects', so the samples may lag or lead the libraries without blocking either.
        var baseCommit = Environment.GetEnvironmentVariable("ALIGNSAMPLES_BASE_COMMIT")
                         ?? AlignmentCheckpoint.Read(checkpointFile, SourceRepositorySync.GenerationBaseCommit);
        var targetRef = Environment.GetEnvironmentVariable("ALIGNSAMPLES_TARGET_COMMIT")
                        ?? SourceRepositorySync.DefaultTrackingBranch;
        var sourceDir = Environment.GetEnvironmentVariable("ALIGN_SOURCE_DIR")
                        ?? Path.Combine(repoRoot, SubmoduleName);

        // Resolve the ref to a real commit: a submodule on a detached HEAD frequently has no local branch
        // of that name (only origin/<name>), and the checkpoint must record the commit absorbed, not a
        // branch that keeps moving.
        var targetCommit = GitRepository.ResolveCommit(sourceDir, targetRef);
        if (targetCommit == null)
        {
            RedConsole.Write($"Cannot resolve target '{targetRef}' in {sourceDir} - is the submodule initialized and fetched?");
            return 5;
        }

        Console.WriteLine("AlignSamples - align the sample apps with the latest gitee upstream");
        Console.WriteLine($"  base commit   : {baseCommit}");
        Console.WriteLine($"  target commit : {targetCommit} ({targetRef})");
        Console.WriteLine($"  source dir    : {sourceDir}");
        Console.WriteLine($"  scope         : {string.Join(", ", Scope)}");

        WarnIfAheadOfLibraries(repoRoot, baseCommit);

        var driver = new AlignmentDriver(BuildOptions(repoRoot, checkpointFile));

        // Determinism self-check (§3.1): fs must reproduce byte for byte, or the delta carries transform
        // noise indistinguishable from real upstream change.
        var selfCheck = Environment.GetEnvironmentVariable("ALIGNSAMPLES_SELFCHECK");
        if (!string.IsNullOrEmpty(selfCheck))
        {
            return await driver.Producer.RunDeterminismCheckAsync(selfCheck == "1" ? targetCommit : selfCheck);
        }

        // --- Phase 2: globalization enrichment, then the manual pause ---
        // Runs BEFORE the transform: fs can only produce clean English-with-keys output for both snapshots
        // if every Chinese string already has a (possibly blank) slot to be translated in.
        ConsoleColor.Green.Write("Enriching the sample resources with new Chinese...", true);
        var enrichment = SampleGlobalizationEnricher.Enrich(repoRoot, UpstreamSampleDirs(sourceDir));
        Console.WriteLine($"  proposed resource keys : {enrichment.NewKeys.Count}");
        Console.WriteLine($"  blank dictionary entries: {enrichment.NewDictionaryEntries.Count}");
        Console.WriteLine($"  data files to translate : {enrichment.DataFiles.Count}");
        ManualTranslationPause.RunForSamples(enrichment, resourcesDir);
        CommitEnrichedResources(repoRoot, enrichment);

        // --- Phase 3: the alignment driver ---
        // ALIGNSAMPLES_BASE_TREE/_THEIRS_TREE feed pre-produced trees in (test seam); otherwise the driver
        // runs fs twice in isolated staging roots.
        return await driver.RunAsync(baseCommit, targetCommit,
            Environment.GetEnvironmentVariable("ALIGNSAMPLES_BASE_TREE"),
            Environment.GetEnvironmentVariable("ALIGNSAMPLES_THEIRS_TREE"),
            Environment.GetEnvironmentVariable("ALIGNSAMPLES_PROVENANCE"));
    }

    /// <summary>The sample-specific half of the alignment: scope, the transform <c>fs</c>, and the culture check.</summary>
    private static AlignmentOptions BuildOptions(string repoRoot, string checkpointFile) => new()
    {
        RepoRoot = repoRoot,
        ToolName = ToolName,
        Scope = Scope,
        CheckpointFile = checkpointFile,
        SubmoduleName = SubmoduleName,
        // Its own throwaway branches, so a pending AlignProjects run is never disturbed.
        BranchPrefix = "align-samples",
        ConflictLogName = "conflicts-samples.log",
        // fs, with the network-dependent GtkSharp bump off (see SampleTransformPipeline).
        Transform = stagingRoot => SampleTransformPipeline.RunAllSampleTransformsAsync(stagingRoot),
        // §3.7, samples-only: the merge is not "clean" if it left the samples less culture-aware.
        PostMergeChecks = () => SampleCultureVerifier.Verify(Scope.Select(s => Path.Combine(repoRoot, s)))
    };

    /// <summary>
    /// Commits the enriched overlay files (and only those) before the merge. The 3-way merge needs a clean
    /// working tree, and these dictionaries are fork state the delta must not carry: leaving them
    /// uncommitted would mix hand-filled translations into the cherry-pick. Nothing else is staged - the
    /// commit is scoped by pathspec.
    /// </summary>
    private static void CommitEnrichedResources(string repoRoot, SampleEnrichmentResult enrichment)
    {
        if (!enrichment.HasWork)
        {
            return;
        }

        var paths = new List<string>
        {
            $"Resources/{SampleGlobalizationEnricher.ResourceBaseName}.xml",
            $"Resources/{SampleGlobalizationEnricher.ResourceBaseName}.zh-Hans.xml",
            $"Resources/{SampleGlobalizationEnricher.DictionaryName}.xml"
        };

        foreach (var data in enrichment.DataFiles)
        {
            paths.Add("Resources/" + Path.GetFileName(data.ChineseFile));
            paths.Add("Resources/" + Path.GetFileName(data.EnglishSibling));
        }

        if (GitRepository.CommitPaths(repoRoot, "AlignSamples: enrich sample translations", paths))
        {
            ConsoleColor.Green.Write("Committed the enriched sample dictionaries.", true);
        }
        else
        {
            Console.WriteLine("Enriched resources already committed - nothing to do.");
        }
    }

    /// <summary>The upstream sample directories the enrichment scan reads.</summary>
    private static IEnumerable<string> UpstreamSampleDirs(string sourceDir) =>
        SampleTransformPipeline.Samples.Select(s => Path.Combine(sourceDir, "Samples", s.SourceProjectName));

    /// <summary>
    /// Ordering guard (recommend, don't enforce): upstream sample changes may depend on library APIs that
    /// AlignProjects has not ported yet. A warning, never an error - the post-align build is the hard net.
    /// </summary>
    private static void WarnIfAheadOfLibraries(string repoRoot, string samplesBaseCommit)
    {
        var librariesCheckpoint = Path.Combine(repoRoot, "AlignProjects", "last-aligned.txt");
        if (!File.Exists(librariesCheckpoint))
        {
            return;
        }

        var libraries = AlignmentCheckpoint.Read(librariesCheckpoint, string.Empty);
        if (libraries.Length == 0 || string.Equals(libraries, samplesBaseCommit, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ConsoleColor.Yellow.Write(
            $"Note: AlignProjects is at {Shorten(libraries)} while the samples start from {Shorten(samplesBaseCommit)}.", true);
        Console.WriteLine("  Run AlignProjects first if the upstream sample changes need new library APIs.");
    }

    private static string Shorten(string commit) => commit.Length > 10 ? commit.Substring(0, 10) : commit;
}
