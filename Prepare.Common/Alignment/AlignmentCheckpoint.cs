namespace Prepare.Common.Alignment;

/// <summary>
/// Persists the last upstream commit that AlignProjects has absorbed (the "checkpoint"), so each
/// run aligns only <c>base..target</c> incrementally. Stored as a small text file (first non-empty,
/// non-comment line = the commit id). Reads fall back to a bootstrap commit when the file is absent
/// (the first run). See <c>AlignProjects-Plan.md</c> §3.2 / §3.8.
/// </summary>
public static class AlignmentCheckpoint
{
    /// <summary>
    /// Returns the last-aligned commit from <paramref name="checkpointFilePath"/>, or
    /// <paramref name="fallbackCommit"/> when the file is missing/empty. Lines starting with '#' are ignored.
    /// </summary>
    public static string Read(string checkpointFilePath, string fallbackCommit)
    {
        if (File.Exists(checkpointFilePath))
        {
            foreach (var raw in File.ReadAllLines(checkpointFilePath))
            {
                var line = raw.Trim();
                if (line.Length > 0 && !line.StartsWith("#"))
                {
                    return line;
                }
            }
        }

        return fallbackCommit;
    }

    /// <summary>
    /// Writes <paramref name="commit"/> as the new checkpoint, creating the directory if needed.
    /// <paramref name="toolName"/> only names the owning tool in the header comment - each alignment
    /// tool keeps its own checkpoint file, so the two never share state.
    /// </summary>
    public static void Write(string checkpointFilePath, string commit, string toolName = "AlignProjects")
    {
        var dir = Path.GetDirectoryName(checkpointFilePath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(checkpointFilePath,
            $"# {toolName} last-aligned upstream commit (auto-managed)" + Environment.NewLine +
            commit.Trim() + Environment.NewLine);
    }
}
