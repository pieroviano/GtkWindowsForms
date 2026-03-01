using System.Diagnostics;
using Prepare.Common.IO;

namespace Prepare.Common.Text;

public static class DotnetFormatter
{
    public static async Task<int> FormatAsync(
        string projectPath,
        string? editorConfigPath = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
            throw new ArgumentException("Project path is required.", nameof(projectPath));

        var arguments = $"format \"{projectPath}\"";

        if (!string.IsNullOrWhiteSpace(editorConfigPath))
        {
#pragma warning disable CS8604 // Possible null reference argument.
            LogUtility.FileCopyAndLog(editorConfigPath, Path.Combine(Path.GetDirectoryName(projectPath), ".editorconfig"));
#pragma warning restore CS8604 // Possible null reference argument.
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = processStartInfo
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet format failed with exit code {process.ExitCode}.\nError:\n{error}");
        }

        return process.ExitCode;
    }
}
