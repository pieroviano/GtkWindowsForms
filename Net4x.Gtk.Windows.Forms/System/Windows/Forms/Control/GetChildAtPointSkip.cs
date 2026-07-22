namespace System.Windows.Forms;

/// <summary>
/// Specifies which child controls to skip.
/// </summary>
/// <remarks>
/// This was an empty class with no members, so the skip argument of
/// <see cref="M:System.Windows.Forms.Control.GetChildAtPoint(System.Drawing.Point,System.Windows.Forms.GetChildAtPointSkip)" />
/// could not express anything. WinForms defines it as a flags enumeration.
/// </remarks>
[Flags]
public enum GetChildAtPointSkip
{
    /// <summary>
    /// Do not skip any child control.
    /// </summary>
    None = 0,

    /// <summary>
    /// Skip child controls that are not visible.
    /// </summary>
    Invisible = 1,

    /// <summary>
    /// Skip child controls that are disabled.
    /// </summary>
    Disabled = 2,

    /// <summary>
    /// Skip child controls that are transparent.
    /// </summary>
    Transparent = 4
}
