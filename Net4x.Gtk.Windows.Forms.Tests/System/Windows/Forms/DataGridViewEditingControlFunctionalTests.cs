using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the DataGridView editing-control cluster (§4 of API-COMPLETENESS): the
/// IDataGridViewEditingControl interface, its two concrete controls (text box and combo box) and
/// DataGridViewTopLeftHeaderCell. These were absent, so a custom editing control could not be written
/// against the real names. The tests pin the editing-control contract — formatted-value round-trip through
/// the control's text, row-index/value-changed state, and input-key routing — and the header-cell type.
/// </summary>
[Collection("NonParallelCollection")]
public class DataGridViewEditingControlFunctionalTests
{
    static DataGridViewEditingControlFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void TextBoxEditingControl_ImplementsTheInterface()
    {
        using var control = new DataGridViewTextBoxEditingControl();
        Assert.IsAssignableFrom<IDataGridViewEditingControl>(control);
        Assert.IsAssignableFrom<TextBox>(control);
    }

    [Fact]
    public void TextBoxEditingControl_FormattedValueRoundTripsThroughText()
    {
        using var control = new DataGridViewTextBoxEditingControl();
        IDataGridViewEditingControl editing = control;

        editing.EditingControlFormattedValue = "hello";

        Assert.Equal("hello", control.Text);
        Assert.Equal("hello", editing.EditingControlFormattedValue);
        Assert.Equal("hello", editing.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting));
    }

    [Fact]
    public void TextBoxEditingControl_TracksRowIndexAndValueChanged()
    {
        using var control = new DataGridViewTextBoxEditingControl();
        IDataGridViewEditingControl editing = control;

        editing.EditingControlRowIndex = 7;
        editing.EditingControlValueChanged = true;

        Assert.Equal(7, editing.EditingControlRowIndex);
        Assert.True(editing.EditingControlValueChanged);
        Assert.False(editing.RepositionEditingControlOnValueChange);
        Assert.NotNull(editing.EditingPanelCursor);
    }

    [Fact]
    public void TextBoxEditingControl_WantsTheCaretKeys()
    {
        using var control = new DataGridViewTextBoxEditingControl();
        IDataGridViewEditingControl editing = control;

        Assert.True(editing.EditingControlWantsInputKey(Keys.Left, dataGridViewWantsInputKey: true));
        Assert.True(editing.EditingControlWantsInputKey(Keys.Right, dataGridViewWantsInputKey: true));
        // A key the grid wants and the text box does not claim defers to the grid.
        Assert.False(editing.EditingControlWantsInputKey(Keys.Tab, dataGridViewWantsInputKey: true));
    }

    [Fact]
    public void TextBoxEditingControl_ApplyCellStyleDoesNotThrow()
    {
        using var control = new DataGridViewTextBoxEditingControl();
        IDataGridViewEditingControl editing = control;
        var style = new DataGridViewCellStyle();

        var ex = Record.Exception(() => editing.ApplyCellStyleToEditingControl(style));

        Assert.Null(ex);
    }

    [Fact]
    public void ComboBoxEditingControl_ImplementsTheInterfaceAndRoundTripsValue()
    {
        using var control = new DataGridViewComboBoxEditingControl();
        IDataGridViewEditingControl editing = control;

        Assert.IsAssignableFrom<ComboBox>(control);

        editing.EditingControlFormattedValue = "pick";
        Assert.Equal("pick", editing.EditingControlFormattedValue);
    }

    [Fact]
    public void TopLeftHeaderCell_IsAColumnHeaderCell_AndAdjustsBorderToPlaceholder()
    {
        var cell = new DataGridViewTopLeftHeaderCell();
        Assert.IsAssignableFrom<DataGridViewColumnHeaderCell>(cell);

        var input = new DataGridViewAdvancedBorderStyle();
        var placeholder = new DataGridViewAdvancedBorderStyle();

        var result = cell.AdjustCellBorderStyle(input, placeholder, false, false, true, true);

        Assert.Same(placeholder, result);
    }
}
