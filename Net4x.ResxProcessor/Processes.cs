using System.Diagnostics;

namespace Net4x.ResxProcessor;

/// <summary>
/// Provides utility methods for detecting and synchronizing with other running instances of the current process.
/// </summary>
/// <remarks>The Processes class enables applications to check for other running instances of themselves and to
/// wait for those instances to exit before proceeding. This can be useful in scenarios where only one instance should
/// perform a particular operation at a time, or where coordination between multiple instances is required. All methods
/// operate on the current process name and compare process start times to distinguish between instances.</remarks>
public class Processes
{
    /// <summary>
    /// Waits for any other running instances of the current process, with the same name and command-line arguments, to
    /// exit before continuing.
    /// </summary>
    /// <remarks>This method is useful for ensuring that only one instance of a process with the same name and
    /// arguments runs at a time. It blocks execution until all previously started matching processes have exited. If no
    /// such processes are found, the method returns immediately.</remarks>
    /// <returns>true if the method waited for at least one matching process to exit; otherwise, false.</returns>
    public bool WaitSameProcessesStartedEarlier()
    {
        // Specify the process name and command line arguments
        string processName = Process.GetCurrentProcess().ProcessName;

        // Check if another instance of the process is running with the same name and arguments
        var process = IsProcessRunning(processName);
        bool hasWaited = false;
        while (process != null)
        {
            hasWaited = true;
            try
            {
                process.WaitForExit();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            process = IsProcessRunning(processName);
        }
        return hasWaited;
    }

    /// <summary>
    /// Determines whether a process with the specified name is currently running, excluding the current process
    /// instance.
    /// </summary>
    /// <remarks>This method excludes the current process from the search results. If multiple processes with
    /// the specified name are running, the first matching process is returned. The caller is responsible for disposing
    /// the returned <see cref="T:System.Diagnostics.Process"/> object if necessary.</remarks>
    /// <param name="processName">The name of the process to search for. This value is case-insensitive and should not include the file extension.
    /// Cannot be null or empty.</param>
    /// <returns>A <see cref="T:System.Diagnostics.Process"/> object representing a running process with the specified name, or
    /// <see langword="null"/> if no such process is found.</returns>
    public System.Diagnostics.Process? IsProcessRunning(string processName)
    {
        // Get all processes with the specified process name
        var processes = System.Diagnostics.Process.GetProcessesByName(processName);

        // Check each process for matching command line arguments
        System.Diagnostics.Process[]? enumerable = null;
        var currentProcess = Process.GetCurrentProcess();
        try
        {
            enumerable = processes.Where(p => p.Id != currentProcess.Id &&
                                              p.StartTime <= System.Diagnostics.Process.GetCurrentProcess().StartTime).ToArray();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
        }

        if (enumerable is { Length: > 0 })
        {
            foreach (var process in enumerable)
            {
                if (process.StartTime == currentProcess.StartTime &&
                    process.Id > currentProcess.Id)
                {
                    continue;
                }
                return process; // Another instance with the same name and arguments found
            }
        }

        return null; // No matching process found
    }
}
