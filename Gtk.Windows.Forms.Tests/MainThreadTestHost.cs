using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using Xunit.Runners;

// Entry point that lets a GTK test assembly run its tests on the PROCESS MAIN THREAD.
//
// Why: on macOS, GTK's Quartz backend drives AppKit, and AppKit only accepts UI calls on the main thread -
// the first GApplication registration aborts with "NSInternalInconsistencyException: API misuse: setting the
// main menu on a non-main thread". Under 'dotnet test' the main thread belongs to vstest's testhost, so
// StaTestFramework has to use a thread of its own and macOS cannot run these tests at all.
//
// How: Microsoft.NET.Test.Sdk already builds .NET test projects as executables (vstest loads them as
// libraries and never calls Main). With <GenerateProgramFile>false</GenerateProgramFile> this file supplies
// Main instead: running the test assembly directly ('dotnet Gtk.Windows.Forms.Tests.dll') runs the xUnit
// engine in-process on a worker thread, while the real main thread drains a queue. StaTestFramework sees
// IsActive and hands its whole test pass to Run(), which executes it on the main thread. Under 'dotnet test'
// nothing here runs and StaTestFramework keeps its dedicated thread, so Windows/Linux CI are unchanged.
//
// Usage: dotnet <TestAssembly>.dll [--filter <text>]
//   --filter keeps only tests whose display name or Class.Method contains <text> (case-insensitive).
// Exit code: 0 = all passed, 1 = failures or runner errors, 2 = the runner itself crashed.
//
// This file is linked into every test project that runs GTK tests; each must set GenerateProgramFile=false
// and reference xunit.runner.utility.

namespace GtkWinForms.Tests;

/// <summary>
/// Runs the containing test assembly with the xUnit engine while keeping the process main thread available
/// for GTK (see the file header).
/// </summary>
public static class MainThreadTestHost
{
    private static BlockingCollection<Action>? _mainThreadQueue;

    /// <summary>Gets a value indicating whether the assembly was started through <see cref="Main"/>.</summary>
    public static bool IsActive => _mainThreadQueue != null;

    /// <summary>
    /// Runs <paramref name="work"/> on the process main thread and blocks until it has finished, rethrowing
    /// anything it threw. Only valid while <see cref="IsActive"/> is true.
    /// </summary>
    /// <param name="work">The work to execute on the main thread.</param>
    public static void Run(Action work)
    {
        var queue = _mainThreadQueue ?? throw new InvalidOperationException(
            "MainThreadTestHost.Run requires the assembly to be started through MainThreadTestHost.Main.");

        Exception? failure = null;
        using var done = new ManualResetEventSlim();
        queue.Add(() =>
        {
            try
            {
                work();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                done.Set();
            }
        });
        done.Wait();

        if (failure != null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    /// <summary>Runs every test in this assembly; see the file header for arguments and exit codes.</summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>The process exit code.</returns>
    public static int Main(string[] args)
    {
        var queue = new BlockingCollection<Action>();
        _mainThreadQueue = queue;

        var exitCode = 2;
        var runnerThread = new Thread(() =>
        {
            try
            {
                exitCode = RunAssembly(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Test runner crashed: " + ex);
                exitCode = 2;
            }
            finally
            {
                queue.CompleteAdding();
            }
        })
        {
            Name = "xUnit-Runner-Thread",
            IsBackground = true,
        };
        runnerThread.Start();

        // The main thread does nothing but execute work handed to it - in practice, StaTestFramework's whole
        // test pass - until the runner thread is done.
        foreach (var action in queue.GetConsumingEnumerable())
        {
            action();
        }

        runnerThread.Join();
        return exitCode;
    }

    private static int RunAssembly(string[] args)
    {
        string? filter = null;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--filter" && i + 1 < args.Length)
            {
                filter = args[++i];
            }
            else
            {
                Console.Error.WriteLine($"Unknown argument '{args[i]}'. Usage: dotnet <TestAssembly>.dll [--filter <text>]");
                return 1;
            }
        }

        var assemblyPath = Assembly.GetEntryAssembly()!.Location;
        var assemblyName = Path.GetFileName(assemblyPath);
        var gate = new object();
        int total = 0, failed = 0, skipped = 0, errors = 0;
        var stopwatch = Stopwatch.StartNew();

        using var finished = new ManualResetEventSlim();
        using var runner = AssemblyRunner.WithoutAppDomain(assemblyPath);

        if (filter != null)
        {
            runner.TestCaseFilter = testCase =>
                testCase.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                $"{testCase.TestMethod.TestClass.Class.Name}.{testCase.TestMethod.Method.Name}"
                    .Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        runner.OnDiscoveryComplete = info =>
        {
            lock (gate)
            {
                Console.WriteLine($"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests in {assemblyName} on the main thread...");
            }
        };
        runner.OnTestFailed = info =>
        {
            lock (gate)
            {
                Console.WriteLine($"  Failed {info.TestDisplayName}");
                Console.WriteLine($"    {info.ExceptionType}: {info.ExceptionMessage}");
                if (!string.IsNullOrEmpty(info.ExceptionStackTrace))
                {
                    Console.WriteLine(info.ExceptionStackTrace);
                }
            }
        };
        runner.OnTestSkipped = info =>
        {
            lock (gate)
            {
                Console.WriteLine($"  Skipped {info.TestDisplayName}: {info.SkipReason}");
            }
        };
        runner.OnErrorMessage = info =>
        {
            lock (gate)
            {
                errors++;
                Console.WriteLine($"  [ERROR] {info.ExceptionType}: {info.ExceptionMessage}");
                if (!string.IsNullOrEmpty(info.ExceptionStackTrace))
                {
                    Console.WriteLine(info.ExceptionStackTrace);
                }
            }
        };
        runner.OnExecutionComplete = info =>
        {
            total = info.TotalTests;
            failed = info.TestsFailed;
            skipped = info.TestsSkipped;
            finished.Set();
        };

        // Theory data is NOT pre-enumerated: discovery runs on this worker thread, and MemberData that builds
        // GTK objects must only be evaluated during execution, i.e. on the main thread.
        runner.Start(new AssemblyRunnerStartOptions { PreEnumerateTheories = false });
        finished.Wait();

        // AssemblyRunner.Dispose throws while the runner is still winding down after ExecutionComplete.
        while (runner.Status != AssemblyRunnerStatus.Idle)
        {
            Thread.Sleep(50);
        }

        var success = failed == 0 && errors == 0;
        Console.WriteLine(
            $"{(success ? "Passed!" : "Failed!")}  - Failed: {failed}, Passed: {total - failed - skipped}, Skipped: {skipped}, " +
            $"Total: {total}, Errors: {errors}, Duration: {stopwatch.Elapsed.TotalSeconds:F0} s - {assemblyName}");
        return success ? 0 : 1;
    }
}
