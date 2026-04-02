#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace System.Windows.Forms.Collection;

[Collection("NonParallelCollection")]
public class ControlCollectionTests
{
    public ControlCollectionTests()
    {
        Application.Init();
    }

    private Control CreateControl(string? name = null)
    {
        return new Label { Name = name ?? Guid.NewGuid().ToString() };
    }

    [Fact]
    public void Add_ShouldAddControl()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var child = CreateControl("child1");

        collection.Add(child);

        Assert.True(collection.Contains(child));
        Assert.Equal(0, collection.IndexOf(child));
    }

    [Fact]
    public void Add_Null_ShouldNotThrow()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner) { (Control?)null };

        Assert.Empty(collection);
    }

    [Fact]
    public void AddRange_ShouldAddAllControls()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        var c2 = CreateControl("c2");

        collection.AddRange(c1, c2);

        Assert.Equal(2, collection.Count);
        Assert.True(collection.Contains(c1));
        Assert.True(collection.Contains(c2));
    }

    [Fact]
    public void AddRange_Null_ShouldThrow()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        Assert.Throws<ArgumentNullException>(() => collection.AddRange(null));
    }

    [Fact]
    public void ContainsKey_ShouldWork()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var child = CreateControl("test");
        collection.Add(child);

        Assert.True(collection.ContainsKey("test"));
        Assert.False(collection.ContainsKey("missing"));
    }

    [Fact]
    public void IndexOfKey_ShouldReturnCorrectIndex()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        var c2 = CreateControl("c2");

        collection.AddRange(c1, c2);

        Assert.Equal(1, collection.IndexOfKey("c2"));
        Assert.Equal(-1, collection.IndexOfKey("missing"));
    }

    [Fact]
    public void Find_ShouldReturnMatchingControls()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("same");
        var c2 = CreateControl("same");

        collection.AddRange(c1, c2);

        var result = collection.Find("same", false);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void Remove_ShouldRemoveControl()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var child = CreateControl("child");
        collection.Add(child);

        collection.Remove(child);

        Assert.False(collection.Contains(child));
    }

    [Fact]
    public void RemoveAt_ShouldRemoveCorrectItem()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        var c2 = CreateControl("c2");

        collection.AddRange(c1, c2);

        collection.RemoveAt(0);

        Assert.False(collection.Contains(c1));
        Assert.True(collection.Contains(c2));
    }

    [Fact]
    public void RemoveByKey_ShouldRemoveCorrectControl()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        collection.Add(c1);

        collection.RemoveByKey("c1");

        Assert.Empty(collection);
    }

    [Fact]
    public void Clear_ShouldRemoveAll()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner)
        {
            CreateControl(),
            CreateControl()
        };

        collection.Clear();

        Assert.Empty(collection);
    }

    [Fact]
    public void Clone_ShouldCopyElements()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        collection.Add(c1);

        var clone = (Control.ControlCollection)collection.Clone();

        Assert.Equal(collection.Count, clone.Count);
        Assert.True(clone.Contains(c1));
    }

    [Fact]
    public void GetChildIndex_ShouldReturnCorrectIndex()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");
        collection.Add(c1);

        var index = collection.GetChildIndex(c1);

        Assert.Equal(0, index);
    }

    [Fact]
    public void GetChildIndex_NotFound_ShouldThrow()
    {
        var owner = new Control();
        var collection = new Control.ControlCollection(owner);

        var c1 = CreateControl("c1");

        Assert.Throws<ArgumentException>(() => collection.GetChildIndex(c1));
    }
}