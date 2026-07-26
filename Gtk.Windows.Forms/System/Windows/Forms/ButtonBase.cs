/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 */

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality common to button controls
/// (<see cref="T:System.Windows.Forms.Button" />, <see cref="T:System.Windows.Forms.CheckBox" /> and
/// <see cref="T:System.Windows.Forms.RadioButton" />). This mirrors the real
/// <c>System.Windows.Forms.ButtonBase</c> so that source written against the shared surface — and type
/// tests such as <c>control is ButtonBase</c> — compile and behave.
/// </summary>
/// <remarks>
/// <see cref="P:System.Windows.Forms.ButtonBase.TextAlign" /> is wired to the underlying widget through
/// <see cref="M:System.Windows.Forms.ButtonBase.SetContentAlignmentCore(System.Single,System.Single)" />.
/// The remaining appearance properties (<see cref="P:System.Windows.Forms.ButtonBase.FlatStyle" />,
/// <see cref="P:System.Windows.Forms.ButtonBase.FlatAppearance" />,
/// <see cref="P:System.Windows.Forms.ButtonBase.UseVisualStyleBackColor" /> and the others) are stored for
/// source compatibility; rendering defers to the active GTK theme rather than emulating Windows visual
/// styles, so their visual effect is a documented deviation from desktop WinForms.
/// </remarks>
[DesignerCategory("Component")]
public abstract class ButtonBase : Control, IButtonControl
{
    private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
    private FlatButtonAppearance? _flatAppearance;
    private bool _isDefault;

    /// <summary>
    /// Gets or sets the alignment of the text on the button control.
    /// </summary>
    public virtual ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            _textAlign = value;
            SetContentAlignmentCore(HorizontalFactor(value), VerticalFactor(value));
        }
    }

    /// <summary>
    /// Gets the appearance of the border and the colors used to indicate check state and mouse state when
    /// <see cref="P:System.Windows.Forms.ButtonBase.FlatStyle" /> is
    /// <see cref="F:System.Windows.Forms.FlatStyle.Flat" />.
    /// </summary>
    public FlatButtonAppearance FlatAppearance => _flatAppearance ??= new FlatButtonAppearance();

    /// <summary>
    /// Gets or sets the flat style appearance of the button control.
    /// </summary>
    public FlatStyle FlatStyle { get; set; } = FlatStyle.Standard;

    /// <summary>
    /// Gets or sets a value indicating whether the ellipsis character (...) appears at the right edge of the
    /// control, denoting that the text extends beyond the specified length of the control.
    /// </summary>
    public bool AutoEllipsis { get; set; }

    /// <summary>
    /// Gets or sets the position of text and image relative to each other.
    /// </summary>
    public TextImageRelation TextImageRelation { get; set; } = TextImageRelation.Overlay;

    /// <summary>
    /// Gets or sets a value indicating whether the button uses the mnemonic (access key) contained in its
    /// text.
    /// </summary>
    public bool UseMnemonic { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to use the classic (GDI+) or the compatibility text renderer.
    /// Stored for compatibility only.
    /// </summary>
    public bool UseCompatibleTextRendering { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the button control is the default button. In WinForms this is
    /// set by a hosting form through its accept button.
    /// </summary>
    protected virtual bool IsDefault
    {
        get => _isDefault;
        set => _isDefault = value;
    }

    /// <summary>
    /// Applies a horizontal and vertical alignment factor (each 0.0, 0.5 or 1.0) to the underlying widget.
    /// Implemented per control against its GTK alignment properties.
    /// </summary>
    /// <param name="xAlign">The horizontal alignment factor.</param>
    /// <param name="yAlign">The vertical alignment factor.</param>
    protected abstract void SetContentAlignmentCore(float xAlign, float yAlign);

    /// <summary>
    /// Maps the horizontal component of a <see cref="T:System.Drawing.ContentAlignment" /> to a 0.0–1.0
    /// factor (left, center, right).
    /// </summary>
    /// <param name="alignment">The content alignment.</param>
    /// <returns>The horizontal alignment factor.</returns>
    protected static float HorizontalFactor(ContentAlignment alignment)
    {
        const ContentAlignment right = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
        const ContentAlignment center = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
        if ((alignment & right) != 0)
        {
            return 1.0f;
        }

        if ((alignment & center) != 0)
        {
            return 0.5f;
        }

        return 0.0f;
    }

    /// <summary>
    /// Maps the vertical component of a <see cref="T:System.Drawing.ContentAlignment" /> to a 0.0–1.0 factor
    /// (top, middle, bottom).
    /// </summary>
    /// <param name="alignment">The content alignment.</param>
    /// <returns>The vertical alignment factor.</returns>
    protected static float VerticalFactor(ContentAlignment alignment)
    {
        const ContentAlignment bottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
        const ContentAlignment middle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
        if ((alignment & bottom) != 0)
        {
            return 1.0f;
        }

        if ((alignment & middle) != 0)
        {
            return 0.5f;
        }

        return 0.0f;
    }
}
