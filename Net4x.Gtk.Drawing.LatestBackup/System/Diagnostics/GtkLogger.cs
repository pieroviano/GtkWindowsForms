using System.Text;

namespace System.Diagnostics;

public static class GtkLogger
{
    private static Action<string, LogLevel, Exception?>? _customLogAction;

    private static bool _isEnabled = true;

    private static LogLevel _minimumLogLevel;

    // Static constructor to initialize from environment variable
    static GtkLogger()
    {
        var envValue = Environment.GetEnvironmentVariable("MinimumLogLevel");

        if (!string.IsNullOrWhiteSpace(envValue) &&
            Enum.TryParse<LogLevel>(envValue, true, out var parsedLevel))
        {
            _minimumLogLevel = parsedLevel;
        }
        else
        {
            _minimumLogLevel = LogLevel.Information;
        }
    }

    public static bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public static LogLevel MinimumLogLevel
    {
        get => _minimumLogLevel;
        set => _minimumLogLevel = value;
    }

    public static void Critical(string message, Exception? exception = null)
    {
        Log(LogLevel.Critical, message, exception);
    }

    public static void Debug(string message, Exception? exception = null)
    {
        Log(LogLevel.Debug, message, exception);
    }

    public static void Error(string message, Exception? exception = null)
    {
        Log(LogLevel.Error, message, exception);
    }

    public static void Exception(Exception exception, string? message = null)
    {
        var logMessage = string.IsNullOrEmpty(message)
            ? "An exception occurred"
            : message;
        Log(LogLevel.Error, logMessage, exception);
    }

    public static void Fatal(string message, Exception? exception = null)
    {
        Log(LogLevel.Critical, message, exception);
    }

    public static void Info(string message, Exception? exception = null)
    {
        Log(LogLevel.Information, message, exception);
    }

    public static void Information(string message, Exception? exception = null)
    {
        Log(LogLevel.Information, message, exception);
    }

    public static void SetCustomLogAction(Action<string, LogLevel, Exception?>? logAction)
    {
        _customLogAction = logAction;
    }

    public static void Trace(string message, Exception? exception = null)
    {
        Log(LogLevel.Trace, message, exception);
    }

    public static void Warn(string message, Exception? exception = null)
    {
        Log(LogLevel.Warning, message, exception);
    }

    public static void Warning(string message, Exception? exception = null)
    {
        Log(LogLevel.Warning, message, exception);
    }

    private static string FormatMessage(LogLevel level, string message, Exception? exception)
    {
        var sb = new StringBuilder();

        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        sb.Append(" [");
        sb.Append(GetLogLevelString(level));
        sb.Append("] ");
        sb.Append(message);

        if (exception != null)
        {
            sb.AppendLine();
            sb.Append("Exception: ");
            sb.Append(exception.GetType().FullName);
            sb.Append(": ");
            sb.AppendLine(exception.Message);

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);
            }

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                sb.AppendLine("Inner Exception: ");
                sb.Append(innerException.GetType().FullName);
                sb.Append(": ");
                sb.AppendLine(innerException.Message);

                if (!string.IsNullOrEmpty(innerException.StackTrace))
                {
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(innerException.StackTrace);
                }

                innerException = innerException.InnerException;
            }
        }

        sb.AppendLine();

        return sb.ToString();
    }

    private static string GetLogLevelString(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => "UNKNOWN"
        };
    }

    private static void Log(LogLevel level, string message, Exception? exception)
    {
        if (!_isEnabled || level < _minimumLogLevel)
            return;

        var formattedMessage = FormatMessage(level, message, exception);

        if (_customLogAction != null)
        {
            _customLogAction(formattedMessage, level, exception);
        }
        else
        {
            System.Diagnostics.Trace.Write(formattedMessage);
        }
    }
}

/// <summary>
/// Defines logging severity levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Logs that are used for interactive investigation during development.
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Logs that track the general flow of the application.
    /// </summary>
    Information = 2,

    /// <summary>
    /// Logs that highlight an abnormal or unexpected event in the application flow.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Logs that highlight when the current flow of execution is stopped due to a failure.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Logs that describe an unrecoverable application or system crash.
    /// </summary>
    Critical = 5
}