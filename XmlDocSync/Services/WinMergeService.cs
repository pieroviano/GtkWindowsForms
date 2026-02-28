using System.Diagnostics;

namespace XmlDocSync.Services;

public static class WinMergeService
{
    public static void CompareWithWinMerge(string file, string oldFile, string newFile)
    {
        string file1= Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetFileName(file),".old.cs"));
        string file2= Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetFileName(file), ".new.cs"));
        File.WriteAllText(file1, oldFile);
        File.WriteAllText(file2, newFile);
        var winMergePath = @"C:\Program Files\WinMerge\WinMergeU.exe";

        var psi = new ProcessStartInfo
        {
            FileName = winMergePath,
            Arguments = $"\"{file1}\" \"{file2}\"",
            UseShellExecute = true
        };

        var process = Process.Start(psi);
        process.WaitForExit();
        File.Delete(file1);
        File.Delete(file2);
    }
}