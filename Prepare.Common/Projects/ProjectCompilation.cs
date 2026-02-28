using System.Diagnostics;
using Prepare.Common.Text;

namespace Prepare.Common.Projects;

public static class ProjectCompilation
{
    public static void RestoreSolution(string solutionPath)
    {
        RunDotNet(solutionPath, "restore");
    }

    public static void BuildSolution(string solutionPath)
    {
#if RELEASE
        RunDotNet(solutionPath, "build -c Release");
#else
        RunDotNet(solutionPath, "build");
#endif
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
            ConsoleUtilityEx.WriteInColor(message, ConsoleColor.Red);
            throw new InvalidOperationException(message);
        }
    }
}
