using Prepare.Common.IO;
using Prepare.Common.Services;

namespace UpdatePostProcessing;

public class Program
{
    private static int processFilesUpdates;

    public static int Main(string[] args)
    {
        foreach (var csprojPath in args)
        {
            processFilesUpdates = FileUpdatesService.Process(csprojPath, BackupUtility.LatestBackup);
            if (processFilesUpdates != 0)
            {
                return processFilesUpdates;
            }
        }

        return 0;
    }
}
