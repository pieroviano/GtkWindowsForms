using System.Collections;
using System.Linq;

namespace System.Windows.Forms.Collection;

public class BaseCollectionTests
{
    private class TestCollection : System.Windows.Forms.BaseCollection
    {
        private readonly ArrayList _list;

        public TestCollection(ArrayList list = null)
        {
            _list = list;
        }

        protected override ArrayList List => _list;
    }

    [Fact]
    public void Count_WhenListIsNull_ReturnsZero()
    {
        var sut = new TestCollection(null);

        var result = sut.Count;

        Assert.Equal(0, result);
    }

    [Fact]
    public void Count_WhenListHasItems_ReturnsCorrectCount()
    {
        var list = new ArrayList { 1, 2, 3 };
        var sut = new TestCollection(list);

        var result = sut.Count;

        Assert.Equal(3, result);
    }

    [Fact]
    public void IsReadOnly_ShouldAlwaysReturnFalse()
    {
        var sut = new TestCollection();

        Assert.False(sut.IsReadOnly);
    }

    [Fact]
    public void IsSynchronized_ShouldAlwaysReturnFalse()
    {
        var sut = new TestCollection();

        Assert.False(sut.IsSynchronized);
    }

    [Fact]
    public void SyncRoot_ShouldReturnSameInstance()
    {
        var sut = new TestCollection();

        var syncRoot = sut.SyncRoot;

        Assert.Same(sut, syncRoot);
    }

    [Fact]
    public void CopyTo_ShouldCopyElementsToArray()
    {
        var list = new ArrayList { 1, 2, 3 };
        var sut = new TestCollection(list);
        var array = new object[3];

        sut.CopyTo(array, 0);

        Assert.Equal(new object[] { 1, 2, 3 }, array);
    }

    [Fact]
    public void CopyTo_WithIndex_ShouldOffsetCorrectly()
    {
        var list = new ArrayList { 1, 2 };
        var sut = new TestCollection(list);
        var array = new object[5];

        sut.CopyTo(array, 2);

        Assert.Equal(null, array[0]);
        Assert.Equal(null, array[1]);
        Assert.Equal(1, array[2]);
        Assert.Equal(2, array[3]);
    }

    [Fact]
    public void GetEnumerator_WhenListIsNull_ShouldThrow()
    {
        var sut = new TestCollection(null);

        Assert.Throws<NullReferenceException>(() => sut.GetEnumerator());
    }
}