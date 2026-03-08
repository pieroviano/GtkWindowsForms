using System.Diagnostics;
using Prepare.Common.Content;

namespace XmlDocSync.Services;

public static class WinMergeService
{
    public static void CompareWithWinMerge(string file, string oldFile, string newFile)
    {
        var file1= Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetFileName(file),".old.cs"));
        var file2= Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetFileName(file), ".new.cs"));
        CsFile.WriteAllText(file1, oldFile);
        CsFile.WriteAllText(file2, newFile);
        var winMergePath = @"C:\Program Files\WinMerge\WinMergeU.exe";

        var psi = new ProcessStartInfo
        {
            FileName = winMergePath,
            Arguments = $"\"{file1}\" \"{file2}\"",
            UseShellExecute = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();
        File.Delete(file1);
        File.Delete(file2);
    }
}
