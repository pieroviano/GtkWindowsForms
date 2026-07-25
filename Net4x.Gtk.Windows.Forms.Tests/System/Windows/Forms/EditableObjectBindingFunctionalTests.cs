using System.ComponentModel;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="IEditableObject"/> transactions under list binding — the last item
/// EMULATION-GAPS §5.6 left "still unestablished". A <see cref="CurrencyManager"/> calls
/// <see cref="IEditableObject.BeginEdit"/> when it lands on an item and, on demand, commits with
/// <see cref="IEditableObject.EndEdit"/> or rolls back with <see cref="IEditableObject.CancelEdit"/>.
/// The oracle is the row's own committed/reverted value, not the call log alone.
/// </summary>
[Collection("NonParallelCollection")]
public class EditableObjectBindingFunctionalTests
{
    static EditableObjectBindingFunctionalTests()
    {
        using var _ = new Form();
    }

    /// <summary>A row that snapshots its value on BeginEdit and restores it on CancelEdit.</summary>
    private sealed class EditableRow : IEditableObject
    {
        private string? _committed;
        private bool _editing;

        public string? Value { get; set; }

        public int BeginCount { get; private set; }
        public int EndCount { get; private set; }
        public int CancelCount { get; private set; }

        public void BeginEdit()
        {
            if (_editing)
            {
                return;
            }

            _editing = true;
            BeginCount++;
            _committed = Value;
        }

        public void EndEdit()
        {
            if (!_editing)
            {
                return;
            }

            _editing = false;
            EndCount++;
            _committed = Value;
        }

        public void CancelEdit()
        {
            if (!_editing)
            {
                return;
            }

            _editing = false;
            CancelCount++;
            Value = _committed;
        }
    }

    [Fact]
    public void LandingOnAnItemBeginsItsEdit()
    {
        var row = new EditableRow { Value = "start" };
        using var source = new BindingSource { DataSource = new List<EditableRow> { row } };

        // Touch the current item so the manager positions onto it.
        _ = source.Current;

        Assert.True(row.BeginCount >= 1, "BeginEdit must be called when the manager lands on the row");
    }

    [Fact]
    public void CancelEditRollsBackTheEditedRow()
    {
        var row = new EditableRow { Value = "original" };
        using var source = new BindingSource { DataSource = new List<EditableRow> { row } };
        _ = source.Current;

        row.Value = "typed but not committed";
        source.CancelEdit();

        Assert.Equal("original", row.Value);
        Assert.True(row.CancelCount >= 1);
    }

    [Fact]
    public void EndEditCommitsTheEditedRow()
    {
        var row = new EditableRow { Value = "original" };
        using var source = new BindingSource { DataSource = new List<EditableRow> { row } };
        _ = source.Current;

        row.Value = "committed";
        source.EndEdit();

        Assert.Equal("committed", row.Value);
        Assert.True(row.EndCount >= 1);

        // After committing, a later cancel must not resurrect the pre-edit value.
        source.CancelEdit();
        Assert.Equal("committed", row.Value);
    }
}
