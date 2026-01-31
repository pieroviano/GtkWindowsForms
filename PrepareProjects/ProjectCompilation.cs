using System.Diagnostics;

namespace PrepareProjects;

public static class ProjectCompilation
{
    public static void RestoreProject(string csprojPath)
    {
        if (!File.Exists(csprojPath))
        {
            throw new FileNotFoundException(
                "Cannot run dotnet restore. Project file not found.",
                csprojPath);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore \"{csprojPath}\"",
                WorkingDirectory = Path.GetDirectoryName(csprojPath)!,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        Console.WriteLine(output);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet restore failed:{Environment.NewLine}{error}");
        }
    }
}