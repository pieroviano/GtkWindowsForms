using Prepare.Common.Logging;

namespace Prepare.Common.IO;

/// <summary>
/// Guards the pipeline's hardcoded upstream-path assumptions. When an expected file or folder
/// is missing — e.g. because a newer upstream reorganized the source — it logs a clear, named
/// warning instead of letting the step throw an obscure error or silently no-op. The caller
/// skips the step, so the transform can still run to completion and surface every issue at once.
/// </summary>
public static class PathGuard
{
    /// <summary>Returns true if <paramref name="path"/> is an existing directory; otherwise logs a clear warning and returns false.</summary>
    public static bool RequireDirectory(string path, string step)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        Warn("directory", path, step);
        return false;
    }

    /// <summary>Returns true if <paramref name="path"/> is an existing file; otherwise logs a clear warning and returns false.</summary>
    public static bool RequireFile(string path, string step)
    {
        if (File.Exists(path))
        {
            return true;
        }

        Warn("file", path, step);
        return false;
    }

    private static void Warn(string kind, string path, string step)
    {
        string full;
        try
        {
            full = Path.GetFullPath(path);
        }
        catch
        {
            full = path;
        }

        RedConsole.Write(
            $"[PrepareProjects] Expected {kind} not found for step '{step}': {full}. " +
            "The upstream source may have been reorganized - skipping this step.");
    }
}
