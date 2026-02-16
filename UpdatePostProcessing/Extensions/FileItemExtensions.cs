using System.Diagnostics;
using UpdatePostProcessing.Model;
using UpdatePostProcessing.Services;

namespace UpdatePostProcessing.Extensions;

internal static class FileItemExtensions
{
    public static string GetContents(this FileItem item, string resourceFile, bool useResourceFile)
    {
        string contents;
        if (useResourceFile)
        {
            contents = File.ReadAllText(resourceFile);
        }
        else
        {
            contents = item.OldSource;
        }
        return contents;
    }

    public static List<TextReplacement>? ProcessItem(this FileItem item,
        string csprojDirectory,
        string fileName, string contents)
    {
        if (!BuildCurrentNew(csprojDirectory, item, out var currentNew))
        {
            return null;
        }

        var currentOld = BuildCurrentOld(fileName, contents);

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
        else
        {
            Console.WriteLine($"No differences for {fileName}");
            return null;
        }
    }

    private static bool BuildCurrentNew(string csprojDirectory, FileItem item, out string currentNew)
    {
        // Build currentNew
        currentNew = Path.Combine(csprojDirectory, item.Destination);

        if (!File.Exists(currentNew))
        {
            Console.WriteLine($"Skipping (not found): {currentNew}");
            return false;
        }

        return true;
    }

    private static string BuildCurrentOld(string fileName, string contents)
    {
        // Build currentOld in %TEMP%
        var tempPath = Path.GetTempPath();
        var currentOld = Path.Combine(tempPath, fileName);

        if (File.Exists(currentOld))
        {
            File.Delete(currentOld);
        }

        File.WriteAllText(currentOld, contents);
        return currentOld;
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