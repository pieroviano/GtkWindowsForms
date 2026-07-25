using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Value tests for the nine <c>DataGridView*</c> enums that were absent. The underlying integer values must
/// match real WinForms exactly — designer serialization, resx round-trips and interop compare against them,
/// so a wrong number is a silent data-corruption bug, not just a cosmetic mismatch. The expected values are
/// the ground truth dumped from the net48 reference assembly (two of them, AutoSizeRowMode and EditMode, do
/// not follow the "declaration order" intuition).
/// </summary>
public class DataGridViewEnumValuesTests
{
    [Fact]
    public void RowHeadersWidthSizeMode_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewRowHeadersWidthSizeMode.EnableResizing);
        Assert.Equal(1, (int)DataGridViewRowHeadersWidthSizeMode.DisableResizing);
        Assert.Equal(2, (int)DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        Assert.Equal(3, (int)DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
        Assert.Equal(4, (int)DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader);
    }

    [Fact]
    public void AutoSizeRowMode_HasReferenceValues()
    {
        // Note the non-obvious numbering: these are bit-derived, not declaration order.
        Assert.Equal(3, (int)DataGridViewAutoSizeRowMode.AllCells);
        Assert.Equal(2, (int)DataGridViewAutoSizeRowMode.AllCellsExceptHeader);
        Assert.Equal(1, (int)DataGridViewAutoSizeRowMode.RowHeader);
    }

    [Fact]
    public void CellBorderStyle_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewCellBorderStyle.Custom);
        Assert.Equal(1, (int)DataGridViewCellBorderStyle.Single);
        Assert.Equal(2, (int)DataGridViewCellBorderStyle.Raised);
        Assert.Equal(3, (int)DataGridViewCellBorderStyle.Sunken);
        Assert.Equal(4, (int)DataGridViewCellBorderStyle.None);
        Assert.Equal(5, (int)DataGridViewCellBorderStyle.SingleVertical);
        Assert.Equal(6, (int)DataGridViewCellBorderStyle.RaisedVertical);
        Assert.Equal(7, (int)DataGridViewCellBorderStyle.SunkenVertical);
        Assert.Equal(8, (int)DataGridViewCellBorderStyle.SingleHorizontal);
        Assert.Equal(9, (int)DataGridViewCellBorderStyle.RaisedHorizontal);
        Assert.Equal(10, (int)DataGridViewCellBorderStyle.SunkenHorizontal);
    }

    [Fact]
    public void ClipboardCopyMode_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewClipboardCopyMode.Disable);
        Assert.Equal(1, (int)DataGridViewClipboardCopyMode.EnableWithAutoHeaderText);
        Assert.Equal(2, (int)DataGridViewClipboardCopyMode.EnableWithoutHeaderText);
        Assert.Equal(3, (int)DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText);
    }

    [Fact]
    public void ComboBoxDisplayStyle_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewComboBoxDisplayStyle.ComboBox);
        Assert.Equal(1, (int)DataGridViewComboBoxDisplayStyle.DropDownButton);
        Assert.Equal(2, (int)DataGridViewComboBoxDisplayStyle.Nothing);
    }

    [Fact]
    public void EditMode_HasReferenceValues()
    {
        // EditOnEnter is 0 here, not EditOnKeystrokeOrF2 — worth pinning.
        Assert.Equal(0, (int)DataGridViewEditMode.EditOnEnter);
        Assert.Equal(1, (int)DataGridViewEditMode.EditOnKeystroke);
        Assert.Equal(2, (int)DataGridViewEditMode.EditOnKeystrokeOrF2);
        Assert.Equal(3, (int)DataGridViewEditMode.EditOnF2);
        Assert.Equal(4, (int)DataGridViewEditMode.EditProgrammatically);
    }

    [Fact]
    public void HeaderBorderStyle_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewHeaderBorderStyle.Custom);
        Assert.Equal(1, (int)DataGridViewHeaderBorderStyle.Single);
        Assert.Equal(2, (int)DataGridViewHeaderBorderStyle.Raised);
        Assert.Equal(3, (int)DataGridViewHeaderBorderStyle.Sunken);
        Assert.Equal(4, (int)DataGridViewHeaderBorderStyle.None);
    }

    [Fact]
    public void HitTestType_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewHitTestType.None);
        Assert.Equal(1, (int)DataGridViewHitTestType.Cell);
        Assert.Equal(2, (int)DataGridViewHitTestType.ColumnHeader);
        Assert.Equal(3, (int)DataGridViewHitTestType.RowHeader);
        Assert.Equal(4, (int)DataGridViewHitTestType.TopLeftHeader);
        Assert.Equal(5, (int)DataGridViewHitTestType.HorizontalScrollBar);
        Assert.Equal(6, (int)DataGridViewHitTestType.VerticalScrollBar);
    }

    [Fact]
    public void ImageCellLayout_HasReferenceValues()
    {
        Assert.Equal(0, (int)DataGridViewImageCellLayout.NotSet);
        Assert.Equal(1, (int)DataGridViewImageCellLayout.Normal);
        Assert.Equal(2, (int)DataGridViewImageCellLayout.Stretch);
        Assert.Equal(3, (int)DataGridViewImageCellLayout.Zoom);
    }
}
