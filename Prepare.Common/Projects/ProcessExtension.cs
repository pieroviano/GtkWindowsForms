using System.Diagnostics;
using System.Text;

namespace Prepare.Common.Projects;

public static class ProcessExtension
{
    public static (string output, string error) StartAndWaitForExit(this Process process, bool printOutput)
    {
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null)
                return;

            outputBuilder.AppendLine(e.Data);

            if (printOutput)
                Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data == null)
                return;

            errorBuilder.AppendLine(e.Data);

            if (printOutput)
                Console.Error.WriteLine(e.Data);
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        return (outputBuilder.ToString(), errorBuilder.ToString());
    }
}
