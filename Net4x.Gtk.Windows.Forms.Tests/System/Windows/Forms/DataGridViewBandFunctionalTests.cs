using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Pins the DataGridView hierarchy re-parent: <see cref="DataGridViewRow"/> now derives from
/// <see cref="DataGridViewBand"/> → <see cref="DataGridViewElement"/>, and <see cref="DataGridViewCell"/>
/// from <see cref="DataGridViewElement"/>. The leaf types keep their own behaviour via overrides — the
/// point of the change is that the hierarchy exists (so <c>is DataGridViewBand</c> works and the base
/// members are inherited) without disturbing the row's computed <c>State</c> or the cell's computed
/// <c>DataGridView</c>.
/// </summary>
[Collection("NonParallelCollection")]
public class DataGridViewBandFunctionalTests
{
    static DataGridViewBandFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void DataGridViewRow_IsABandAndElement()
    {
        var row = new DataGridViewRow();

        Assert.IsAssignableFrom<DataGridViewBand>(row);
        Assert.IsAssignableFrom<DataGridViewElement>(row);
    }

    [Fact]
    public void DataGridViewCell_IsAnElement()
    {
        var cell = new DataGridViewTextBoxCell();

        Assert.IsAssignableFrom<DataGridViewElement>(cell);
    }

    [Fact]
    public void Row_StateStillComputesFromTheFlags_AfterReparent()
    {
        // The row's State override (derived from the flags) must survive the move onto Element, whose own
        // State is a stored source of truth. Frozen/ReadOnly set must still surface in State.
        var row = new DataGridViewRow { Frozen = true, ReadOnly = true };

        Assert.True(row.State.HasFlag(DataGridViewElementStates.Frozen));
        Assert.True(row.State.HasFlag(DataGridViewElementStates.ReadOnly));
    }

    [Fact]
    public void Row_InheritsTheBandSurface()
    {
        var row = new DataGridViewRow();

        // Members that live on DataGridViewBand and the row did not previously carry.
        row.Tag = "marker";
        Assert.Equal("marker", row.Tag);
        Assert.False(row.HasDefaultCellStyle);

        row.DefaultCellStyle = new DataGridViewCellStyle();
        Assert.True(row.HasDefaultCellStyle);
    }
}
