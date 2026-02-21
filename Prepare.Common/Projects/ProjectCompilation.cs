using System.Diagnostics;

namespace Prepare.Common.Projects;

public static class ProjectCompilation
{
    public static void RestoreSolution(string solutionPath)
    {
        RunDotNet(solutionPath, "restore");
    }

    public static void BuildSolution(string solutionPath)
    {
        RunDotNet(solutionPath, "build");
    }

    private static void RunDotNet(string projectSolutionPath, object? whatToDo)
    {
        if (!File.Exists(projectSolutionPath))
        {
            throw new FileNotFoundException(
                "Cannot run dotnet restore. Project file not found.",
                projectSolutionPath);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{whatToDo} \"{projectSolutionPath}\"",
                WorkingDirectory = Path.GetDirectoryName(projectSolutionPath)!,
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
            var message = $"dotnet {whatToDo} failed:{Environment.NewLine}{output}{Environment.NewLine}{error}";
            Console.WriteLine(message);
            throw new InvalidOperationException(message);
        }
    }
}