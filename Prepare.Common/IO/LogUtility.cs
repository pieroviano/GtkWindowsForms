using Prepare.Common.Logging;

namespace Prepare.Common.IO
{
    public static class LogUtility
    {
        public static void FileCopyAndLog(string sourceFileName, string destFileName, bool overwrite = false)
        {
            var str = overwrite ? "(overwriting if exists)" : "(skipping if exists)";
            var source = Path.GetFullPath(sourceFileName);
            var destination = Path.GetFullPath(destFileName);
            if (destination.Contains("PrepareProjects"))
            {
                YellowConsole.Write($"Copying '{source}' to '{Path.GetDirectoryName(destination)}' directory.");
            }
            ColoredConsole.WriteInColor($"Copying '{source}' to '{destination}' {str}", ConsoleColor.Cyan);
            File.Copy(sourceFileName, destFileName, overwrite);
        }
    }
}
