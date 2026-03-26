extern alias gtk;

using LogLevel = gtk::System.Diagnostics.LogLevel;
using GtkLogger = gtk::System.Diagnostics.GtkLogger;

namespace System.Diagnostics;

public class GtkLoggerTests
{
    private string? _capturedMessage;
    private LogLevel _capturedLevel;
    private Exception? _capturedException;

    public GtkLoggerTests()
    {
        // Reset logger before each test
        GtkLogger.SetCustomLogAction(null);
        GtkLogger.IsEnabled = true;
        GtkLogger.MinimumLogLevel = LogLevel.Trace;

        _capturedMessage = null;
        _capturedException = null;

        GtkLogger.SetCustomLogAction((msg, lvl, ex) =>
        {
            _capturedMessage = msg;
            _capturedLevel = lvl;
            _capturedException = ex;
        });
    }

    [Fact]
    public void Debug_Should_Log_When_Level_Allows()
    {
        GtkLogger.MinimumLogLevel = LogLevel.Trace;

        GtkLogger.Debug("test");

        Assert.NotNull(_capturedMessage);
        Assert.Contains("test", _capturedMessage);
        Assert.Equal(LogLevel.Debug, _capturedLevel);
    }

    [Fact]
    public void Should_Not_Log_When_Below_MinimumLevel()
    {
        GtkLogger.MinimumLogLevel = LogLevel.Error;

        GtkLogger.Info("ignored");

        Assert.Null(_capturedMessage);
    }

    [Fact]
    public void Should_Not_Log_When_Disabled()
    {
        GtkLogger.IsEnabled = false;

        GtkLogger.Error("test");

        Assert.Null(_capturedMessage);
    }

    [Fact]
    public void Error_Should_Log_Exception()
    {
        var ex = new InvalidOperationException("boom");

        GtkLogger.Error("failure", ex);

        Assert.NotNull(_capturedMessage);
        Assert.Contains("failure", _capturedMessage);
        Assert.Contains("InvalidOperationException", _capturedMessage);
        Assert.Equal(ex, _capturedException);
    }

    [Fact]
    public void Exception_Method_Should_Default_Message()
    {
        var ex = new Exception("err");

        GtkLogger.Exception(ex);

        Assert.Contains("An exception occurred", _capturedMessage);
    }

    [Fact]
    public void Exception_Method_Should_Use_Custom_Message()
    {
        var ex = new Exception("err");

        GtkLogger.Exception(ex, "custom");

        Assert.Contains("custom", _capturedMessage);
    }

    [Fact]
    public void Should_Log_Inner_Exceptions()
    {
        var inner = new Exception("inner");
        var outer = new Exception("outer", inner);

        GtkLogger.Error("fail", outer);

        Assert.Contains("Inner Exception", _capturedMessage);
        Assert.Contains("inner", _capturedMessage);
    }

    [Fact]
    public void Info_And_Information_Should_Be_Equivalent()
    {
        GtkLogger.Info("info");

        var first = _capturedMessage;

        GtkLogger.Information("info");

        var second = _capturedMessage;

        Assert.NotNull(first);
        Assert.NotNull(second);
    }

    [Fact]
    public void Warn_And_Warning_Should_Be_Equivalent()
    {
        GtkLogger.Warn("warn");
        var firstLevel = _capturedLevel;

        GtkLogger.Warning("warn");
        var secondLevel = _capturedLevel;

        Assert.Equal(firstLevel, secondLevel);
        Assert.Equal(LogLevel.Warning, firstLevel);
    }

    [Fact]
    public void Fatal_Should_Map_To_Critical()
    {
        GtkLogger.Fatal("fatal");

        Assert.Equal(LogLevel.Critical, _capturedLevel);
    }

    [Fact]
    public void Trace_Should_Log_With_Trace_Level()
    {
        GtkLogger.Trace("trace");

        Assert.Equal(LogLevel.Trace, _capturedLevel);
    }

    [Fact]
    public void Should_Use_Custom_Log_Action()
    {
        var called = false;

        GtkLogger.SetCustomLogAction((msg, lvl, ex) =>
        {
            called = true;
        });

        GtkLogger.Info("test");

        Assert.True(called);
    }

    [Fact]
    public void Should_Fallback_To_Trace_When_No_Custom_Action()
    {
        GtkLogger.SetCustomLogAction(null);

        using var listener = new TestTraceListener();
        Trace.Listeners.Add(listener);

        GtkLogger.Info("trace test");

        Trace.Listeners.Remove(listener);

        Assert.Contains("trace test", listener.Output);
    }

    private class TestTraceListener : TraceListener
    {
        public string Output { get; private set; } = "";

        public override void Write(string? message)
        {
            Output += message;
        }

        public override void WriteLine(string? message)
        {
            Output += message + Environment.NewLine;
        }
    }
}