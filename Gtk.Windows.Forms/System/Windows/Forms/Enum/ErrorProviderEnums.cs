namespace System.Windows.Forms;

/// <summary>
/// Specifies where the error icon appears relative to the control it belongs to.
/// </summary>
/// <remarks>
/// Part of the <see cref="T:System.Windows.Forms.ErrorProvider" /> API, which was missing entirely.
/// </remarks>
public enum ErrorIconAlignment
{
    /// <summary>The icon appears above and to the left of the control.</summary>
    TopLeft,

    /// <summary>The icon appears above and to the right of the control.</summary>
    TopRight,

    /// <summary>The icon appears to the left of the control, vertically centred.</summary>
    MiddleLeft,

    /// <summary>The icon appears to the right of the control, vertically centred.</summary>
    MiddleRight,

    /// <summary>The icon appears below and to the left of the control.</summary>
    BottomLeft,

    /// <summary>The icon appears below and to the right of the control.</summary>
    BottomRight
}

/// <summary>
/// Specifies how the error icon blinks when an error is set.
/// </summary>
public enum ErrorBlinkStyle
{
    /// <summary>The icon blinks when the error is first set and when it changes.</summary>
    BlinkIfDifferentError,

    /// <summary>The icon always blinks when the error is set.</summary>
    AlwaysBlink,

    /// <summary>The icon never blinks.</summary>
    NeverBlink
}
