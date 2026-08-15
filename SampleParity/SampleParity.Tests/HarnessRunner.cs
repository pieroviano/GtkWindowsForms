using System.Diagnostics;

namespace SampleParity.Tests;

/// <summary>
/// Runs a harness executable and reads back the trace it produced.
/// </summary>
/// <remarks>
/// Child processes, not references: the two harnesses bind different <c>System.Windows.Forms</c>
/// implementations that declare the same type names, and GTK 3 and GTK 4 cannot be loaded into one process
/// at all. See <c>SampleParityUITests-Plan.md</c> §0 B1.
/// </remarks>
internal static class HarnessRunner
{
    /// <summary>How long a harness may run before it is considered hung.</summary>
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

    /// <summary>Runs a harness and returns its trace.</summary>
    /// <param name="harness">The harness directory name, e.g. <c>Gtk3Harness</c>.</param>
    /// <param name="assemblyName">Its assembly name, e.g. <c>SampleParity.Gtk3Harness</c>.</param>
    /// <returns>The parsed trace records.</returns>
    /// <exception cref="InvalidOperationException">The harness is missing, hung, or failed.</exception>
    public static IReadOnlyList<TraceRecord> Run(string harness, string assemblyName)
    {
        var exe = Locate(harness, assemblyName);
        var output = Path.Combine(Path.GetTempPath(), $"sampleparity-{harness}-{Guid.NewGuid():N}.tsv");

        var info = new ProcessStartInfo(exe)
        {
            WorkingDirectory = Path.GetDirectoryName(exe)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        info.ArgumentList.Add("--out");
        info.ArgumentList.Add(output);

        using var process = Process.Start(info)
                            ?? throw new InvalidOperationException($"Could not start {exe}.");

        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit((int)Timeout.TotalMilliseconds))
        {
            // A harness that hangs - typically on a modal dialog with no one to dismiss it - must not wedge
            // the whole run.
            TryKill(process);

            throw new InvalidOperationException(
                $"{harness} did not finish within {Timeout.TotalMinutes} minutes and was killed.");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"{harness} exited with {process.ExitCode}.{Environment.NewLine}{stdout.Result}{stderr.Result}");
        }

        if (!File.Exists(output))
        {
            throw new InvalidOperationException($"{harness} wrote no trace to {output}.");
        }

        try
        {
            return File.ReadAllLines(output)
                .Select(TraceRecord.Parse)
                .Where(r => r != null)
                .Select(r => r!)
                .ToList();
        }
        finally
        {
            TryDelete(output);
        }
    }

    /// <summary>
    /// Finds a harness executable relative to the repository root.
    /// </summary>
    /// <param name="harness">The harness directory name.</param>
    /// <param name="assemblyName">Its assembly name.</param>
    /// <returns>The full path.</returns>
    /// <exception cref="InvalidOperationException">It has not been built.</exception>
    private static string Locate(string harness, string assemblyName)
    {
        var root = RepositoryRoot();
        var configuration = IsDebug() ? "Debug" : "Release";
        var exeName = OperatingSystem.IsWindows() ? assemblyName + ".exe" : assemblyName;

        var path = Path.Combine(root, "SampleParity", harness, "bin", configuration, "net8.0", exeName);

        if (!File.Exists(path))
        {
            throw new InvalidOperationException(
                $"{harness} has not been built ({path} is missing). Build the solution first - the harnesses "
                + "are separate executables, deliberately not project-referenced by this test assembly.");
        }

        return path;
    }

    /// <summary>Walks up from the test assembly to the directory holding the product solution.</summary>
    /// <returns>The repository root.</returns>
    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Gtk.Windows.Forms.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    private static void TryKill(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (Exception)
        {
            // Already gone; nothing to do.
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception)
        {
            // A leftover temp file is not worth failing a test over.
        }
    }
}
