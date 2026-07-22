namespace System.Windows.Forms;

/// <summary>
/// Specifies the border style for a button control.
/// </summary>
/// <remarks>
/// This enumeration was missing, so <see cref="T:System.Windows.Forms.ControlPaint" />'s border overloads
/// could not be expressed.
/// </remarks>
public enum ButtonBorderStyle
{
    /// <summary>No border.</summary>
    None,

    /// <summary>A dotted-line border.</summary>
    Dotted,

    /// <summary>A dashed border.</summary>
    Dashed,

    /// <summary>A solid border.</summary>
    Solid,

    /// <summary>A sunken border.</summary>
    Inset,

    /// <summary>A raised border.</summary>
    Outset
}
