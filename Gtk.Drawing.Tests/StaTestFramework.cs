using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

// GTK is single-threaded and, on Windows, GDK4 calls OleInitialize (for OLE drag-and-drop / clipboard),
// which REQUIRES the thread to be an STA. The default xUnit/vstest host runs test cases on MTA thread-pool
// threads, so any test that initialises GDK crashes the host with
//   "Gdk-ERROR: OleInitialize failed - COM runtime already initialized ... with an incompatible apartment model".
// This custom test framework runs the entire assembly's test cases on ONE dedicated STA thread with a
// single-threaded SynchronizationContext, which satisfies both GTK's single-thread rule and GDK4's STA
// requirement. No per-test attributes are needed.
[assembly: Xunit.TestFramework("GtkWinForms.Tests.StaTestFramework", "Gtk.Drawing.Tests")]

namespace GtkWinForms.Tests;

/// <summary>
/// xUnit test framework that executes all test cases on a single dedicated STA thread so GTK/GDK 4 can be
/// initialised and driven from one apartment-correct thread (see the file header for why).
/// </summary>
public sealed class StaTestFramework : XunitTestFramework
{
    /// <summary>Initializes a new instance of the <see cref="StaTestFramework"/> class.</summary>
    /// <param name="messageSink">The diagnostic message sink supplied by xUnit.</param>
    public StaTestFramework(IMessageSink messageSink) : base(messageSink)
    {
    }

    /// <inheritdoc />
    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new StaExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
}

/// <summary>
/// Runs the assembly's test cases on a dedicated STA thread pumped by a single-threaded
/// <see cref="SynchronizationContext"/>.
/// </summary>
public sealed class StaExecutor : XunitTestFrameworkExecutor
{
    /// <summary>Initializes a new instance of the <see cref="StaExecutor"/> class.</summary>
    /// <param name="assemblyName">The test assembly name.</param>
    /// <param name="sourceInformationProvider">The source information provider.</param>
    /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
    public StaExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider,
        IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
    }

    /// <inheritdoc />
    protected override void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            var previous = SynchronizationContext.Current;
            var context = new PumpingSyncContext();
            SynchronizationContext.SetSynchronizationContext(context);
            try
            {
                using var runner = new XunitTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink,
                    executionMessageSink, executionOptions);
                var task = runner.RunAsync();
                task.ContinueWith(_ => context.Complete(), CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                context.Pump();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        });

        thread.Name = "GTK-STA-Test-Thread";
        // GDK4 on Windows calls OleInitialize, which requires an STA thread. STA is a Windows-only COM
        // concept: SetApartmentState(STA) throws PlatformNotSupportedException on Linux/macOS, where the
        // single dedicated thread alone already satisfies GTK's single-thread rule.
        if (OperatingSystem.IsWindows())
        {
            thread.SetApartmentState(ApartmentState.STA);
        }
        thread.IsBackground = false;
        thread.Start();
        thread.Join();

        if (failure != null)
        {
            throw failure;
        }
    }

    /// <summary>
    /// A minimal single-threaded synchronization context: continuations posted from async xUnit code are
    /// queued and drained on the owning (STA) thread, so the whole test run stays on one thread.
    /// </summary>
    private sealed class PumpingSyncContext : SynchronizationContext
    {
        private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _queue = new();

        /// <inheritdoc />
        public override void Post(SendOrPostCallback d, object? state) => _queue.Add((d, state));

        /// <inheritdoc />
        public override void Send(SendOrPostCallback d, object? state)
        {
            // Called on the pumping thread itself in practice; run inline to avoid deadlock.
            d(state);
        }

        /// <summary>Drains queued callbacks on the current thread until <see cref="Complete"/> is called.</summary>
        public void Pump()
        {
            foreach (var (callback, state) in _queue.GetConsumingEnumerable())
            {
                callback(state);
            }
        }

        /// <summary>Signals that no more callbacks will be posted, ending <see cref="Pump"/>.</summary>
        public void Complete() => _queue.CompleteAdding();
    }
}
