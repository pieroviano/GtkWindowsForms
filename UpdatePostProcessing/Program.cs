using Prepare.Common.IO;

namespace UpdatePostProcessing;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Missing csproj path argument.");
            return 1;
        }
        var csprojPath = args[0];
        return ProcessFileUpdatesProcessor.ProcessFilesUpdates(csprojPath, BackupUtility.LatestBackup);
    }
}