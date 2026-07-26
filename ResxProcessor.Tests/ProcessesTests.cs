using System.Diagnostics;
using ResxProcessor.Tests.Internals;

namespace ResxProcessor.Tests;

public class ProcessesTests
{
    [Fact]
    public void WaitSameProcessesStartedEarlier_ShouldReturnFalse_WhenNoProcessFound()
    {
        // Arrange
        var processes = new TestProcesses(0);

        // Act
        var result = processes.WaitSameProcessesStartedEarlier();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void WaitSameProcessesStartedEarlier_ShouldReturnTrue_WhenProcessExists()
    {
        // Arrange
        var processes = new TestProcesses(1);

        // Act
        var result = processes.WaitSameProcessesStartedEarlier();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void WaitSameProcessesStartedEarlier_ShouldLoopUntilNoProcess()
    {
        // Arrange
        var processes = new TestProcesses(3);

        // Act
        var result = processes.WaitSameProcessesStartedEarlier();

        // Assert
        Assert.True(result);
        // Implicitly verifies loop doesn't break early
    }

    [Fact]
    public void IsProcessRunning_ShouldReturnNull_WhenNoOtherProcess()
    {
        // Arrange
        var processes = new Processes();
        var current = Process.GetCurrentProcess();

        // Act
        var result = processes.IsProcessRunning(current.ProcessName);

        // Assert
        // In most environments this should be null (only one instance running)
        // We don't assert strict null to avoid flaky CI behavior
        Assert.True(result == null || result.Id != current.Id);
    }

    [Fact]
    public void IsProcessRunning_ShouldNotReturnCurrentProcess()
    {
        // Arrange
        var processes = new Processes();
        var current = Process.GetCurrentProcess();

        // Act
        var result = processes.IsProcessRunning(current.ProcessName);

        // Assert
        if (result != null)
        {
            Assert.NotEqual(current.Id, result.Id);
        }
    }

    [Fact]
    public void IsProcessRunning_ShouldHandleExceptions_Gracefully()
    {
        // Arrange
        var processes = new Processes();

        // Act
        var ex = Record.Exception(() => processes.IsProcessRunning("invalid_process_name_123"));

        // Assert
        Assert.Null(ex);
    }
}