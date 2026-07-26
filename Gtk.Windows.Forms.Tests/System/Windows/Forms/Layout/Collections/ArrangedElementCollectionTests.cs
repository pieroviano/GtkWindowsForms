using System.Collections;
using System.ComponentModel;
using System.Drawing;
#pragma warning disable xUnit2013

namespace System.Windows.Forms.Layout.Collections;

public class ArrangedElementCollectionTests
{
    internal class TestElement : IArrangedElement
    {
        public string Name { get; }

        public TestElement(string name) => Name = name;

        public override string ToString() => Name;
        public void Dispose() => throw new NotImplementedException();

        public ISite? Site { get; set; }
        public event EventHandler? Disposed = (_, _) => {};
        public Rectangle Bounds { get; set; }
        public ArrangedElementCollection? Children { get; set; }
        public IArrangedElement? Container { get; set; }
        public Rectangle DisplayRectangle { get; set; }
        public bool ParticipatesInLayout { get; set; }
        public PropertyStore? Properties { get; set; }
        public Size GetPreferredSize(Size proposedSize) => throw new NotImplementedException();

        public void PerformLayout(IArrangedElement affectedElement, string? propertyName) => throw new NotImplementedException();

        public void SetBounds(Rectangle bounds, BoundsSpecified specified) => throw new NotImplementedException();
    }

    private static ArrangedElementCollection Create(params TestElement[] elements)
    {
        var list = new System.Collections.Generic.List<IArrangedElement>(elements);
        return new ArrangedElementCollection(list);
    }

    [Fact]
    public void Count_Returns_NumberOfElements()
    {
        var col = Create(new TestElement("A"), new TestElement("B"));

        Assert.Equal(2, col.Count);
    }

    [Fact]
    public void IList_Add_AddsElement()
    {
        IList col = Create();

        var el = new TestElement("A");
        col.Add(el);

        Assert.Equal(1, col.Count);
        Assert.Same(el, col[0]);
    }

    [Fact]
    public void IList_Remove_RemovesElement()
    {
        var el = new TestElement("A");
        IList col = Create(el);

        col.Remove(el);

        Assert.Empty(col);
    }

    [Fact]
    public void IList_IndexOf_ReturnsCorrectIndex()
    {
        var a = new TestElement("A");
        var b = new TestElement("B");

        IList col = Create(a, b);

        Assert.Equal(1, col.IndexOf(b));
    }

    [Fact]
    public void IList_Contains_Works()
    {
        var a = new TestElement("A");
        IList col = Create(a);

        Assert.True(col.Contains(a));
        Assert.False(col.Contains(new TestElement("B")));
    }

    [Fact]
    public void CopyTo_CopiesElements()
    {
        var a = new TestElement("A");
        var b = new TestElement("B");

        var col = Create(a, b);

        var array = new object[2];
        col.CopyTo(array, 0);

        Assert.Equal(new object[] { a, b }, array);
    }

    [Fact]
    public void Equals_SameElements_ReturnsTrue()
    {
        var a = new TestElement("A");
        var b = new TestElement("B");

        var col1 = Create(a, b);
        var col2 = Create(a, b);

        Assert.True(col1.Equals(col2));
    }

    [Fact]
    public void Equals_DifferentElements_ReturnsFalse()
    {
        var col1 = Create(new TestElement("A"));
        var col2 = Create(new TestElement("B"));

        Assert.False(col1.Equals(col2));
    }

    [Fact]
    public void GetEnumerator_IteratesInOrder()
    {
        var a = new TestElement("A");
        var b = new TestElement("B");

        var col = Create(a, b);

        var items = col.Cast<object>().ToArray();

        Assert.Equal(new object[] { a, b }, items);
    }

    [Fact]
    public void GetHashCode_EqualCollections_SameHash()
    {
        var a = new TestElement("A");

        var col1 = Create(a);
        var col2 = Create(a);

        Assert.Equal(col1.GetHashCode(), col2.GetHashCode());
    }

    [Fact]
    public void IList_Indexer_Set_Throws()
    {
        IList col = Create(new TestElement("A"));

        Assert.Throws<NotSupportedException>(() => col[0] = new TestElement("B"));
    }

    [Fact]
    public void IList_Insert_Throws()
    {
        IList col = Create();

        Assert.Throws<NotSupportedException>(() => col.Insert(0, new TestElement("A")));
    }
}