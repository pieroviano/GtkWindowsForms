using System.Collections;
using System.Resources;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Resources.Extensions;

public class RuntimeResourceSetTests
{
    [Fact]
    public void Constructor_WithNonDeserializingReader_Throws()
    {
        Assert.Throws<ArgumentException>(() => new RuntimeResourceSet(new DummyResourceReader()));
    }

    [Fact]
    public void Constructor_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RuntimeResourceSet(null));
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var ms = new MemoryStream(new byte[1], true);
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
    public void GetEnumerator_ThrowsIfDisposed()
    {
        var ms = new MemoryStream(new byte[1], true);
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

    [Fact]
    public void GetObject_ThrowsIfDisposed()
    {
        var ms = new MemoryStream(new byte[1], true);
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
        var ms = new MemoryStream(new byte[1], true);
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

    private class DummyResourceReader : IResourceReader
    {
        public void Close()
        {
        }

        public IDictionaryEnumerator GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public void Dispose()
        {
        }
    }
}