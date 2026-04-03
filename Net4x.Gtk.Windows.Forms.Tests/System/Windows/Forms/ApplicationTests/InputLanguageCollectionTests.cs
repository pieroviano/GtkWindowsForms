namespace System.Windows.Forms.ApplicationTests;

public class InputLanguageCollectionTests
{
    private InputLanguage CreateLang(int handleValue)
        => new InputLanguage((IntPtr)handleValue);

    private InputLanguageCollection CreateCollection(params InputLanguage[] langs)
        => new InputLanguageCollection(langs);

    [Fact]
    public void Constructor_AddsAllItems()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1, l2);

        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void Indexer_ReturnsCorrectItem()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1, l2);

        Assert.Equal(l1, collection[0]);
        Assert.Equal(l2, collection[1]);
    }

    [Fact]
    public void Indexer_InvalidIndex_Throws()
    {
        var collection = CreateCollection(CreateLang(1));

        Assert.Throws<ArgumentOutOfRangeException>(() => collection[5]);
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var l1 = CreateLang(1);
        var collection = CreateCollection(l1);

        Assert.True(collection.Contains(l1));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1);

        Assert.False(collection.Contains(l2));
    }

    [Fact]
    public void IndexOf_ExistingItem_ReturnsIndex()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1, l2);

        Assert.Equal(1, collection.IndexOf(l2));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsMinusOne()
    {
        var collection = CreateCollection(CreateLang(1));

        var missing = CreateLang(999);

        Assert.Equal(-1, collection.IndexOf(missing));
    }

    [Fact]
    public void CopyTo_CopiesItemsCorrectly()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1, l2);
        var array = new InputLanguage[2];

        collection.CopyTo(array, 0);

        Assert.Equal(l1, array[0]);
        Assert.Equal(l2, array[1]);
    }

    [Fact]
    public void CopyTo_WithOffset_WorksCorrectly()
    {
        var l1 = CreateLang(1);
        var l2 = CreateLang(2);

        var collection = CreateCollection(l1, l2);
        var array = new InputLanguage[5];

        collection.CopyTo(array, 2);

        Assert.Null(array[0]);
        Assert.Null(array[1]);
        Assert.Equal(l1, array[2]);
        Assert.Equal(l2, array[3]);
    }

    [Fact]
    public void CopyTo_ArrayTooSmall_Throws()
    {
        var collection = CreateCollection(CreateLang(1), CreateLang(2));
        var array = new InputLanguage[1];

        Assert.Throws<ArgumentException>(() => collection.CopyTo(array, 0));
    }
}