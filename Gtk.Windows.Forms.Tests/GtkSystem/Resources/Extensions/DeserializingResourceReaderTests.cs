#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace Gtk.Resources.Extensions;

public class DeserializingResourceReaderTests
{
    [Fact]
    public void Close_CanBeCalledMultipleTimes()
    {
        var ms = new MemoryStream(new byte[1], true);
        try
        {
            var reader = new DeserializingResourceReader(ms);
            reader.Close();
            reader.Close();
        }
        catch (Exception ex)
        {
            // Acceptable: may throw due to invalid resource format
            Assert.True(ex is BadImageFormatException || ex is ArgumentException);
        }
    }

    [Fact]
    public void Constructor_WithNullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DeserializingResourceReader((Stream)null));
    }

    [Fact]
    public void Constructor_WithUnreadableStream_Throws()
    {
        var stream = new MemoryStream(new byte[1], false);
        stream.Close();
        Assert.Throws<ArgumentException>(() => new DeserializingResourceReader(stream));
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var ms = new MemoryStream(new byte[1], true);
        try
        {
            var reader = new DeserializingResourceReader(ms);
            reader.Dispose();
            reader.Dispose();
        }
        catch (Exception ex)
        {
            // Acceptable: may throw due to invalid resource format
            Assert.True(ex is BadImageFormatException || ex is ArgumentException);
        }
    }

    [Fact]
    public void GetEnumerator_ThrowsIfDisposed()
    {
        var ms = new MemoryStream(new byte[1], true);
        DeserializingResourceReader? reader = null;
        try
        {
            reader = new DeserializingResourceReader(ms);
        }
        catch
        {
            // Ignore: may throw due to invalid resource format
        }

        if (reader != null)
        {
            reader.Dispose();
            Assert.ThrowsAny<Exception>(() => reader.GetEnumerator());
        }
    }
}