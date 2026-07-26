namespace System.Windows.Forms;

/// <summary>
/// Provides properties that specify the appearance of
/// <see cref="T:System.Windows.Forms.ButtonBase" /> controls whose
/// <see cref="P:System.Windows.Forms.ButtonBase.FlatStyle" /> is
/// <see cref="F:System.Windows.Forms.FlatStyle.Flat" />. Exposed through
/// <see cref="P:System.Windows.Forms.ButtonBase.FlatAppearance" />.
/// </summary>
public class FlatButtonAppearance
{
    /// <summary>
    /// Gets or sets the color of the border around the button.
    /// </summary>
    public System.Drawing.Color BorderColor { get; set; } = System.Drawing.Color.Empty;

    /// <summary>
    /// Gets or sets the size, in pixels, of the border around the button.
    /// </summary>
    public int BorderSize { get; set; } = 1;

    /// <summary>
    /// Gets or sets the color of the client area of the button when the button is checked and the mouse
    /// pointer is outside the bounds of the control.
    /// </summary>
    public System.Drawing.Color CheckedBackColor { get; set; } = System.Drawing.Color.Empty;

    /// <summary>
    /// Gets or sets the color of the client area of the button when the mouse is pressed within the bounds
    /// of the control.
    /// </summary>
    public System.Drawing.Color MouseDownBackColor { get; set; } = System.Drawing.Color.Empty;

    /// <summary>
    /// Gets or sets the color of the client area of the button when the mouse pointer is within the bounds
    /// of the control.
    /// </summary>
    public System.Drawing.Color MouseOverBackColor { get; set; } = System.Drawing.Color.Empty;
}
