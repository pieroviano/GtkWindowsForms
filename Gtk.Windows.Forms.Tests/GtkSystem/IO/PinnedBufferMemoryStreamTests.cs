namespace Gtk.IO;

public class PinnedBufferMemoryStreamTests
{
    [Fact]
    public void Constructor_InitializesStreamCorrectly()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };

        using var stream = new PinnedBufferMemoryStream(data);

        Assert.Equal(data.Length, stream.Length);
        Assert.Equal(0, stream.Position);
        Assert.True(stream.CanRead);
        Assert.False(stream.CanWrite);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var data = new byte[] { 1, 2, 3 };

        var stream = new PinnedBufferMemoryStream(data);

        stream.Dispose();
        stream.Dispose(); // should not throw

        // If no exception, test passes
    }

    [Fact]
    public void Dispose_ReleasesResources_StreamBecomesUnusable()
    {
        var data = new byte[] { 1, 2, 3 };

        var stream = new PinnedBufferMemoryStream(data);
        stream.Dispose();

        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }

    [Fact]
    public void LargeBuffer_ReadsCorrectly()
    {
        var data = new byte[10_000];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i % 256);
        }

        using var stream = new PinnedBufferMemoryStream(data);

        var buffer = new byte[data.Length];
        var read = stream.Read(buffer, 0, buffer.Length);

        Assert.Equal(data.Length, read);
        Assert.Equal(data, buffer);
    }

    [Fact]
    public void Read_ReturnsCorrectData()
    {
        var data = Enumerable.Range(0, 10).Select(i => (byte)i).ToArray();

        using var stream = new PinnedBufferMemoryStream(data);
        var buffer = new byte[10];

        var bytesRead = stream.Read(buffer, 0, buffer.Length);

        Assert.Equal(data.Length, bytesRead);
        Assert.Equal(data, buffer);
    }

    [Fact]
    public void ReadByte_ReadsSequentially()
    {
        var data = new byte[] { 10, 20, 30 };

        using var stream = new PinnedBufferMemoryStream(data);

        Assert.Equal(10, stream.ReadByte());
        Assert.Equal(20, stream.ReadByte());
        Assert.Equal(30, stream.ReadByte());
        Assert.Equal(-1, stream.ReadByte()); // end of stream
    }

    [Fact]
    public void Seek_WorksCorrectly()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };

        using var stream = new PinnedBufferMemoryStream(data);

        stream.Seek(2, SeekOrigin.Begin);
        Assert.Equal(3, stream.ReadByte());

        stream.Seek(-1, SeekOrigin.End);
        Assert.Equal(5, stream.ReadByte());
    }

    [Fact]
    public void Stream_ReflectsUnderlyingArrayChanges()
    {
        var data = new byte[] { 1, 2, 3 };

        using var stream = new PinnedBufferMemoryStream(data);

        data[1] = 99;

        stream.Position = 0;
        var buffer = new byte[3];
#pragma warning disable CA2022
        stream.Read(buffer, 0, 3);
#pragma warning restore CA2022

        Assert.Equal(new byte[] { 1, 99, 3 }, buffer);
    }
}