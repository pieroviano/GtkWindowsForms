extern alias gtk;

using FontCollection = gtk::System.Drawing.Text.FontCollection;
using FontFamily = gtk::System.Drawing.FontFamily;

namespace System.Drawing.Text;

public class FontCollectionTests
{
    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var collection = new TestFontCollection([]);
        collection.Dispose();
    }

    [Fact]
    public void DisposeBool_CanBeCalled()
    {
        var collection = new TestFontCollection([]);
        collection.CallDisposeBool(true);
        collection.CallDisposeBool(false);
    }

    [Fact]
    public void Families_ReturnsAssignedArray()
    {
        var families = new[] { new FontFamily("Arial"), new FontFamily("Times New Roman") };
        var collection = new TestFontCollection(families);
        Assert.Equal(families, collection.Families);
    }

    private class TestFontCollection : FontCollection
    {
        public TestFontCollection(FontFamily[] families)
        {
            Families = families;
        }

        public new FontFamily[] Families { get; }

        public void CallDisposeBool(bool disposing) => Dispose(disposing);
    }
}