using System.Diagnostics;

namespace Net4x.ResxProcessor.Tests.Internals;

internal class TestProcesses : Processes
{
    private int _callCount = 0;
    private readonly int _timesToReturnProcess;

    public TestProcesses(int timesToReturnProcess)
    {
        _timesToReturnProcess = timesToReturnProcess;
    }

    public override Process? IsProcessRunning(string processName)
    {
        _callCount++;

        if (_callCount <= _timesToReturnProcess)
        {
            return Process.GetCurrentProcess(); // safe dummy
        }

        return null;
    }

    protected override void WaitProcessExit(Process process)
    {

    }

}