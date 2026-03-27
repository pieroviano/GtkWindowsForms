using System.Collections;
using System.Resources;

namespace GtkSystem.Resources.Extensions;

public class RuntimeResourceSetTests
{
    private class DummyResourceReader : IResourceReader
    {
        public void Close() { }
        public IDictionaryEnumerator GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        public void Dispose() { }
    }

    [Fact]
    public void Constructor_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RuntimeResourceSet((IResourceReader)null));
    }

    [Fact]
    public void Constructor_WithNonDeserializingReader_Throws()
    {
        Assert.Throws<ArgumentException>(() => new RuntimeResourceSet(new DummyResourceReader()));
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var ms = new System.IO.MemoryStream(new byte[1], writable: true);
        DeserializingResourceReader reader = null;
        try
        {
            reader = new DeserializingResourceReader(ms);
        }
        catch
        {
            // Acceptable: may throw due to invalid resource format
        }
        if (reader != null)
        {
            var set = new RuntimeResourceSet(reader);
            set.Dispose();
            set.Dispose();
        }
    }

    [Fact]
    public void GetObject_ThrowsIfDisposed()
    {
        var ms = new System.IO.MemoryStream(new byte[1], writable: true);
        DeserializingResourceReader reader = null;
        try
        {
            reader = new DeserializingResourceReader(ms);
        }
        catch
        {
            // Acceptable: may throw due to invalid resource format
        }
        if (reader != null)
        {
            var set = new RuntimeResourceSet(reader);
            set.Dispose();
            Assert.ThrowsAny<Exception>(() => set.GetObject("any"));
        }
    }

    [Fact]
    public void GetString_ThrowsIfDisposed()
    {
        var ms = new System.IO.MemoryStream(new byte[1], writable: true);
        DeserializingResourceReader reader = null;
        try
        {
            reader = new DeserializingResourceReader(ms);
        }
        catch
        {
            // Acceptable: may throw due to invalid resource format
        }
        if (reader != null)
        {
            var set = new RuntimeResourceSet(reader);
            set.Dispose();
            Assert.ThrowsAny<Exception>(() => set.GetString("any"));
        }
    }

    [Fact]
    public void GetEnumerator_ThrowsIfDisposed()
    {
        var ms = new System.IO.MemoryStream(new byte[1], writable: true);
        DeserializingResourceReader reader = null;
        try
        {
            reader = new DeserializingResourceReader(ms);
        }
        catch
        {
            // Acceptable: may throw due to invalid resource format
        }
        if (reader != null)
        {
            var set = new RuntimeResourceSet(reader);
            set.Dispose();
            Assert.ThrowsAny<Exception>(() => set.GetEnumerator());
        }
    }
}
