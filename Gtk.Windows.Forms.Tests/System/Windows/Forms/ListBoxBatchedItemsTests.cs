namespace System.Windows.Forms;

/// <summary>
/// Covers the captions of <see cref="ListBox" /> items added inside a batch.
/// <para>
/// Every item's caption used to be created hidden while <see cref="ListBox.BeginUpdate" /> was in
/// effect, on the Gtk3 assumption that <c>EndUpdate</c>'s recursive <c>ShowAll()</c> would reveal it
/// again. Gtk4 has no recursive show - its replacement sets <c>Visible</c> on the widget it is called on
/// and nothing else - so those captions stayed hidden for good and the rows rendered blank. Because
/// <see cref="ListBox.ObjectCollection.AddRange(object[])" /> wraps its whole batch in
/// <c>BeginUpdate</c>/<c>EndUpdate</c>, that silently covered every designer-declared item.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class ListBoxBatchedItemsTests
{
    /// <summary>
    /// Initializes GTK before any widget is constructed.
    /// </summary>
    public ListBoxBatchedItemsTests()
    {
        Application.Init();
    }

    [Fact]
    public void AddRange_LeavesEveryItemCaptionVisible()
    {
        var listBox = new ListBox();
        listBox.Items.AddRange(new object[] { "first", "second", "third" });

        Assert.Equal(3, listBox.Items.Count);
        for (var i = 0; i < 3; i++)
        {
            AssertCaptionVisible(listBox, i);
        }
    }

    [Fact]
    public void ItemsAddedBetweenBeginUpdateAndEndUpdate_KeepTheirCaption()
    {
        var listBox = new ListBox();
        listBox.BeginUpdate();
        listBox.Items.Add("batched");
        listBox.Items.Insert(0, "inserted");
        listBox.EndUpdate();

        AssertCaptionVisible(listBox, 0, "inserted");
        AssertCaptionVisible(listBox, 1, "batched");
    }

    [Fact]
    public void ItemAddedOutsideABatch_KeepsTheirCaption()
    {
        var listBox = new ListBox();
        listBox.Items.Add("plain");

        AssertCaptionVisible(listBox, 0, "plain");
    }

    private static void AssertCaptionVisible(ListBox listBox, int index, string? expected = null)
    {
        var row = listBox.self.ListBox.GetRowAtIndex(index);
        Assert.NotNull(row);

        var caption = row.Child as Gtk.Label;
        Assert.True(caption != null, $"row {index} has no caption label");
        Assert.True(caption!.Visible, $"row {index} renders blank - its caption was left hidden");

        if (expected != null)
        {
            Assert.Equal(expected, caption.Text);
        }
    }
}
