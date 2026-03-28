using System.Collections;

namespace System.Resources;

public class ResXResourceReaderTests
{
    private const string SimpleResx = "<?xml version='1.0' encoding='utf-8'?>\n<root>\n  <resheader name='resmimetype'>\n    <value>text/microsoft-resx</value>\n  </resheader>\n  <resheader name='version'>\n    <value>2.0</value>\n  </resheader>\n  <resheader name='reader'>\n    <value>System.Resources.ResXResourceReader</value>\n  </resheader>\n  <resheader name='writer'>\n    <value>System.Resources.ResXResourceWriter</value>\n  </resheader>\n  <data name='TestString' xml:space='preserve'>\n    <value>Hello World</value>\n  </data>\n</root>";

    [Fact]
    public void FromFileContents_ShouldReadStringResource()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        var dict = new Dictionary<string, object>();
        foreach (DictionaryEntry entry in reader)
        {
            dict.Add((string)entry.Key, entry.Value);
        }
        Assert.True(dict.ContainsKey("TestString"));
        Assert.Equal("Hello World", dict["TestString"]);
    }

    [Fact]
    public void GetEnumerator_ShouldReturnDictionaryEnumerator()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("TestString", enumerator.Key);
        Assert.Equal("Hello World", enumerator.Value);
    }

    [Fact]
    public void GetMetadataEnumerator_ShouldReturnEmpty_WhenNoMetadata()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        var enumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(enumerator);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void BasePath_SetAndGet_Works()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        reader.BasePath = "/tmp";
        Assert.Equal("/tmp", reader.BasePath);
    }

    [Fact]
    public void UseResXDataNodes_SetAndGet_Works()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        reader.UseResXDataNodes = true;
        Assert.True(reader.UseResXDataNodes);
    }

    [Fact]
    public void Close_Dispose_DoesNotThrow()
    {
        var reader = ResXResourceReader.FromFileContents(SimpleResx);
        reader.Close();
        ((IDisposable)reader).Dispose();
    }

    [Fact]
    public void InvalidResx_ThrowsArgumentException()
    {
        var invalidResx = "<root><data></data></root>";
        Assert.Throws<ArgumentException>(() => {
            var reader = ResXResourceReader.FromFileContents(invalidResx);
            foreach (DictionaryEntry entry in reader) { }
        });
    }
}