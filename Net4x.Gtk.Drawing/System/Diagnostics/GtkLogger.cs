using System.Text;

namespace System.Diagnostics;

/// <summary>
/// Provides logging functionality similar to Serilog, Microsoft.Extensions.Logging, or Log4Net.
/// By default, uses Trace.Write to output log messages.
/// </summary>
public static class GtkLogger
{
    /// <summary>
    /// Custom action for logging. If set, overrides the default Trace.Write behavior.
    /// </summary>
    private static Action<string, LogLevel, Exception?>? _customLogAction;

    /// <summary>
    /// Indicates whether logging is enabled.
    /// </summary>
    private static bool _isEnabled = true;

    /// <summary>
    /// The minimum log level. Messages below this level will not be logged.
    /// </summary>
    private static LogLevel _minimumLogLevel;

    /// <summary>
    /// Static constructor to initialize the minimum log level from environment variable.
    /// Reads the "MinimumLogLevel" environment variable and defaults to Information if not set.
    /// </summary>
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

    /// <summary>
    /// Gets or sets whether logging is enabled.
    /// </summary>
    public static bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    /// <summary>
    /// Gets or sets the minimum log level. Messages below this level will not be logged.
    /// </summary>
    public static LogLevel MinimumLogLevel
    {
        get => _minimumLogLevel;
        set => _minimumLogLevel = value;
    }

    /// <summary>
    /// Logs a critical-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Critical(string message, Exception? exception = null)
    {
        Log(LogLevel.Critical, message, exception);
    }

    /// <summary>
    /// Logs a debug-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Debug(string message, Exception? exception = null)
    {
        Log(LogLevel.Debug, message, exception);
    }

    /// <summary>
    /// Logs an error-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Error(string message, Exception? exception = null)
    {
        Log(LogLevel.Error, message, exception);
    }

    /// <summary>
    /// Logs an exception with error level.
    /// </summary>
    /// <param name="exception">The exception to log.
    /// </param>
    /// <param name="message">Optional message to accompany the exception.
    /// </param>
    public static void Exception(Exception exception, string? message = null)
    {
        var logMessage = string.IsNullOrEmpty(message)
            ? "An exception occurred"
            : message;
        Log(LogLevel.Error, logMessage, exception);
    }

    /// <summary>
    /// Logs a fatal-level message (alias for Critical).
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Fatal(string message, Exception? exception = null)
    {
        Log(LogLevel.Critical, message, exception);
    }

    /// <summary>
    /// Logs an information-level message (alias for Information).
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Info(string message, Exception? exception = null)
    {
        Log(LogLevel.Information, message, exception);
    }

    /// <summary>
    /// Logs an information-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Information(string message, Exception? exception = null)
    {
        Log(LogLevel.Information, message, exception);
    }

    /// <summary>
    /// Sets a custom log action to override the default Trace.Write behavior.
    /// </summary>
    /// <param name="logAction">Custom action that receives the message, log level, and optional exception.
    /// </param>
    public static void SetCustomLogAction(Action<string, LogLevel, Exception?>? logAction)
    {
        _customLogAction = logAction;
    }

    /// <summary>
    /// Logs a trace-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Trace(string message, Exception? exception = null)
    {
        Log(LogLevel.Trace, message, exception);
    }

    /// <summary>
    /// Logs a warning-level message (alias for Warning).
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Warn(string message, Exception? exception = null)
    {
        Log(LogLevel.Warning, message, exception);
    }

    /// <summary>
    /// Logs a warning-level message.
    /// </summary>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    public static void Warning(string message, Exception? exception = null)
    {
        Log(LogLevel.Warning, message, exception);
    }

    /// <summary>
    /// Formats the log message with timestamp, level, message, and optional exception details.
    /// </summary>
    /// <param name="level">The log level.
    /// </param>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    /// <returns>The formatted log message.
    /// </returns>
    private static string FormatMessage(LogLevel level, string? message, Exception? exception)
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

    /// <summary>
    /// Gets the string representation of a log level.
    /// </summary>
    /// <param name="level">The log level.
    /// </param>
    /// <returns>The string representation.
    /// </returns>
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

    /// <summary>
    /// Core logging method that handles message filtering, formatting, and output.
    /// </summary>
    /// <param name="level">The log level.
    /// </param>
    /// <param name="message">The message to log.
    /// </param>
    /// <param name="exception">Optional exception to include in the log.
    /// </param>
    private static void Log(LogLevel level, string? message, Exception? exception)
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
            Diagnostics.Trace.Write(formattedMessage);
        }
    }
}