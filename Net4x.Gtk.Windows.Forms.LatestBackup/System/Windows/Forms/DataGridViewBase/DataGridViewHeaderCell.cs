using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Contains functionality common to row header cells and column header cells.
/// </summary>
public class DataGridViewHeaderCell : DataGridViewCell
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewHeaderCell" /> class.
    /// </summary>
    public DataGridViewHeaderCell()
    {

    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is displayed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if displayed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public override bool Displayed
    {
        get;
    }

    /// <summary>
    /// Gets the type of the formatted value.
    /// </summary>
    /// <value>
    /// The type of the formatted value.
    /// </value>
    public override Type FormattedValueType
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is frozen.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if frozen; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public override bool Frozen
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool ReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is resizable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if resizable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public override bool Resizable
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool Selected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public override Type ValueType
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public override bool Visible
    {
        get
        {
            return true;
        }
    }

    internal Bitmap FlipXPThemesBitmap
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the state of the button.
    /// </summary>
    /// <value>
    /// The state of the button.
    /// </value>
    protected ButtonState ButtonState
    {
        get;
    }

}