namespace System.Windows.Forms.ApplicationTests;

[Collection("NonParallelCollection")]
public class FormCollectionTests
{
    private static Form CreateForm(string name)
    {
        return new Form { Name = name };
    }

    [Fact]
    public void Indexer_ByName_ReturnsMatchingForm()
    {
        var collection = new FormCollection();
        var form = CreateForm("TestForm");

        collection.Add(form);

        var result = collection["TestForm"];

        Assert.Equal(form, result);
    }

    [Fact]
    public void Indexer_ByName_IsCaseInsensitive()
    {
        var collection = new FormCollection();
        var form = CreateForm("TestForm");

        collection.Add(form);

        var result = collection["testform"];

        Assert.Equal(form, result);
    }

    [Fact]
    public void Indexer_ByName_ReturnsFirstMatch_WhenMultiple()
    {
        var collection = new FormCollection();
        var form1 = CreateForm("Same");
        var form2 = CreateForm("Same");

        collection.Add(form1);
        collection.Add(form2);

        var result = collection["Same"];

        Assert.Equal(form1, result);
    }

    [Fact]
    public void Indexer_ByName_ReturnsNull_WhenNotFound()
    {
        var collection = new FormCollection();

        var result = collection["Missing"];

        Assert.Null(result);
    }

    [Fact]
    public void Indexer_ByName_ReturnsNull_WhenNameIsNull()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        collection.Add(form);

        var result = collection[null!];

        Assert.Null(result);
    }

    [Fact]
    public void Indexer_ByIndex_ReturnsCorrectForm()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        collection.Add(form);

        var result = collection[0];

        Assert.Equal(form, result);
    }

    [Fact]
    public void Indexer_ByIndex_Throws_WhenOutOfRange()
    {
        var collection = new FormCollection();

        Assert.Throws<System.ArgumentOutOfRangeException>(() =>
        {
            var _ = collection[0];
        });
    }

    [Fact]
    public void Add_AddsFormToCollection()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        collection.Add(form);

        Assert.True(collection.Contains(form));
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenNotPresent()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        Assert.False(collection.Contains(form));
    }

    [Fact]
    public void Remove_RemovesFormFromCollection()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        collection.Add(form);
        collection.Remove(form);

        Assert.False(collection.Contains(form));
    }

    [Fact]
    public void Remove_NonExisting_DoesNotThrow()
    {
        var collection = new FormCollection();
        var form = CreateForm("Test");

        var ex = Record.Exception(() => collection.Remove(form));

        Assert.Null(ex);
    }

    [Fact]
    public void MultipleForms_CanBeAddedAndRetrievedByIndex()
    {
        var collection = new FormCollection();
        var f1 = CreateForm("A");
        var f2 = CreateForm("B");

        collection.Add(f1);
        collection.Add(f2);

        Assert.Equal(f1, collection[0]);
        Assert.Equal(f2, collection[1]);
    }
}