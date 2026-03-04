using System.Diagnostics;
using Prepare.Common.IO;
using Prepare.Common.Models;
using Prepare.Common.Services;
using Prepare.Common.Text;

namespace Prepare.Common.Extensions;

internal static class FileItemExtensions
{
    public static void CompareFiles(this List<FileItem> items, string csprojDirectory, string[] files)
    {
        for (var index = 0; index < files.Length; index++)
        {
            if (index % 100 == 0)
            {
                ConsoleColor.White.Write($"[{index + (index == 0 ? 1 : 0)}/{files.Length}]", true);
            }

            var file = files[index];
            var fileItem = items.GetItem(file, out var fileName);
            if (fileItem == null)
            {
                continue;
            }

            var oldContent = fileItem.OldSource;
            var replacements = fileItem.GetReplacements(csprojDirectory, fileName, oldContent);
            if (replacements != null)
            {
                replacements = replacements.Where(r =>
                    r.NewText != r.OldText).ToList();
                if (replacements.Count > 0)
                {
                    var persister = new PostProcessingPersister();
                    persister.PersistReplacements(replacements, fileItem);
                }
            }
        }
    }

    public static List<TextReplacement>? GetReplacements(this FileItem item,
        string csprojDirectory,
        string fileName, string contents)
    {
        if (!SourcePathService.GetCurrentNewFullPath(csprojDirectory, item, out var currentNew))
        {
            return null;
        }

        var currentOld = SourcePathService.GetCurrentOldFullPath(fileName, contents);

        if (RunGitDiff(currentOld, currentNew, out var output, out var error))
        {
            return null;
        }

        var currentDifferences = output + error;

        if (!string.IsNullOrWhiteSpace(currentDifferences))
        {
            var parser = new GitDiffParser();
            var replacements = parser.Parse(currentDifferences);
            return replacements;
        }

        ConsoleColor.White.Write($"No differences for {fileName}", true);
        return null;
    }

    private static bool RunGitDiff(string currentNew, string currentOld, out string? output, out string? error)
    {
        // Run git diff
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"diff --no-index \"{currentNew}\" \"{currentOld}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);

        if (process == null)
        {
            output = null;
            error = null;
            return true;
        }

        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        process.WaitForExit();
        return false;
    }
}
