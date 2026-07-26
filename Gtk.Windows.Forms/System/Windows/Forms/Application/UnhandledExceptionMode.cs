namespace System.Windows.Forms;

/// <summary>
/// Defines where a Windows Forms application should send unhandled exceptions.
/// </summary>
public enum UnhandledExceptionMode
{
    /// <summary>
    /// Route the exception to <see cref="E:System.Windows.Forms.Application.ThreadException" /> unless
    /// the application has opted out. This is the default.
    /// </summary>
    Automatic = 0,

    /// <summary>
    /// Never route the exception; let it propagate out of the message loop and terminate the process.
    /// </summary>
    ThrowException = 1,

    /// <summary>
    /// Always route the exception to <see cref="E:System.Windows.Forms.Application.ThreadException" />,
    /// even when no handler is attached.
    /// </summary>
    CatchException = 2,
}
