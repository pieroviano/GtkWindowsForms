using System.Diagnostics;

namespace Prepare.Common.IO;

/// <summary>
/// Guards the one failure mode the generate-from-submodule design creates: a fix hand-edited into a
/// generated tree is <em>silently temporary</em>.
/// <para>
/// The transform rewrites <c>Net4x.Gtk.Windows.Forms/</c> and <c>Net4x.Gtk.Drawing/</c> from the
/// submodule before any patch is applied, so uncommitted edits in those trees are destroyed by the next
/// run without a word. Durable changes belong in <c>patches/</c>, <c>Resources/</c>, a post-processing
/// rule, or the submodule - see Patches.txt.
/// </para>
/// </summary>
public static class GeneratedTreeGuard
{
    /// <summary>The trees the transform regenerates, and therefore may not be hand-edited durably.</summary>
    public static readonly string[] GeneratedRoots =
    {
        "Net4x.Gtk.Windows.Forms/",
        "Net4x.Gtk.Drawing/",
    };

    /// <summary>Set this environment variable to a non-empty value to proceed despite dirty trees.</summary>
    public const string OverrideVariable = "AllowDirtyGeneratedTrees";

    /// <summary>
    /// Extracts, from <c>git status --porcelain</c> output, the paths inside generated trees whose edits
    /// are about to be lost. Project files are excluded: they are maintained directly in this repository
    /// rather than produced by the transform, which is why the patch recipe excludes them too.
    /// </summary>
    /// <param name="porcelain">Raw <c>git status --porcelain</c> output.</param>
    /// <param name="generatedRoots">Repository-relative directory prefixes to police.</param>
    public static IReadOnlyList<string> FindEndangeredEdits(
        string? porcelain,
        IEnumerable<string>? generatedRoots = null)
    {
        var roots = (generatedRoots ?? GeneratedRoots).ToArray();
        var result = new List<string>();

        if (string.IsNullOrWhiteSpace(porcelain))
        {
            return result;
        }

        foreach (var rawLine in porcelain.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            // Porcelain v1: two status characters, a space, then the path.
            if (line.Length < 4)
            {
                continue;
            }

            var path = line.Substring(3).Trim().Replace('\\', '/');

            // A rename is reported as "old -> new"; the destination is the one that exists now.
            var arrow = path.IndexOf(" -> ", StringComparison.Ordinal);
            if (arrow >= 0)
            {
                path = path.Substring(arrow + 4);
            }

            path = path.Trim('"');

            if (path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (roots.Any(root => path.StartsWith(root, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add(path);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks the working tree before the transform runs. Returns <see langword="true" /> when it is safe
    /// to continue.
    /// </summary>
    /// <param name="repoPath">Repository root.</param>
    /// <param name="printOutput">Whether to echo the git invocation.</param>
    public static bool VerifyBeforeTransform(string repoPath, bool printOutput = true)
    {
        var endangered = FindEndangeredEdits(ReadPorcelainStatus(repoPath, printOutput));
        if (endangered.Count == 0)
        {
            return true;
        }

        ConsoleColor.Red.Write(
            $"{endangered.Count} uncommitted change(s) live in generated trees and this run will overwrite them:",
            true);
        foreach (var path in endangered)
        {
            ConsoleColor.Red.Write($"  - {path}", true);
        }

        ConsoleColor.Yellow.Write(
            "Capture them first (see Patches.txt: git format-patch -o patches/ -1 HEAD -- :(exclude)*.csproj), " +
            $"or set {OverrideVariable}=1 to accept the loss.",
            true);

        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(OverrideVariable));
    }

    private static string ReadPorcelainStatus(string repoPath, bool printOutput)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "status --porcelain",
                WorkingDirectory = repoPath,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (printOutput && process.ExitCode != 0)
        {
            ConsoleColor.Red.Write("git status failed; skipping the generated-tree check.", true);
        }

        return process.ExitCode == 0 ? output : string.Empty;
    }
}
