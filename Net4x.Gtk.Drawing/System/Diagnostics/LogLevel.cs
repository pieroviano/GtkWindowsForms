namespace System.Diagnostics;

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