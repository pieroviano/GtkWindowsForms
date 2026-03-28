using System.Text;

namespace System.Resources;

public class ResXFileRefTests
{
    [Fact]
    public void Ctor_ShouldSetProperties()
    {
        var sut = new System.Resources.ResXFileRef("file.txt", "System.String");

        Assert.Equal("file.txt", sut.FileName);
        Assert.Equal("System.String", sut.TypeName);
        Assert.Null(sut.TextFileEncoding);
    }

    [Fact]
    public void Ctor_WithEncoding_ShouldSetEncoding()
    {
        var encoding = Encoding.UTF8;

        var sut = new System.Resources.ResXFileRef("file.txt", "System.String", encoding);

        Assert.Equal(encoding, sut.TextFileEncoding);
    }

    [Fact]
    public void Ctor_NullFileName_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new System.Resources.ResXFileRef(null, "type"));
    }

    [Fact]
    public void Ctor_NullTypeName_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new System.Resources.ResXFileRef("file", null));
    }

    [Fact]
    public void ToString_NoEncoding_ShouldFormatCorrectly()
    {
        var sut = new System.Resources.ResXFileRef("file.txt", "System.String");

        var result = sut.ToString();

        Assert.Equal("file.txt;System.String", result);
    }

    [Fact]
    public void ToString_WithEncoding_ShouldIncludeEncoding()
    {
        var sut = new System.Resources.ResXFileRef("file.txt", "System.String", Encoding.UTF8);

        var result = sut.ToString();

        Assert.Equal($"file.txt;System.String;{Encoding.UTF8.WebName}", result);
    }

    [Fact]
    public void ToString_FileNameWithSemicolon_ShouldBeQuoted()
    {
        var sut = new System.Resources.ResXFileRef("file;name.txt", "System.String");

        var result = sut.ToString();

        Assert.Equal("\"file;name.txt\";System.String", result);
    }

    [Fact]
    public void ToString_FileNameWithQuote_ShouldBeQuoted()
    {
        var sut = new System.Resources.ResXFileRef("file\"name.txt", "System.String");

        var result = sut.ToString();

        Assert.Equal("\"file\"name.txt\";System.String", result);
    }

    [Fact]
    public void Clone_ShouldReturnNewInstanceWithSameValues()
    {
        var sut = new System.Resources.ResXFileRef("file.txt", "System.String", Encoding.UTF8);

        var clone = sut.Clone();

        Assert.NotSame(sut, clone);
        Assert.Equal(sut.FileName, clone.FileName);
        Assert.Equal(sut.TypeName, clone.TypeName);
        Assert.Equal(sut.TextFileEncoding, clone.TextFileEncoding);
    }

    [Fact]
    public void MakeFilePathRelative_EmptyBasePath_ShouldDoNothing()
    {
        var sut = new System.Resources.ResXFileRef("folder/file.txt", "System.String");

        sut.MakeFilePathRelative(null);

        Assert.Equal("folder/file.txt", sut.FileName);
    }

    [Fact]
    public void MakeFilePathRelative_ShouldConvertToRelative()
    {
        var basePath = "/home/user/project";
        var filePath = "/home/user/project/resources/file.txt";

        var sut = new System.Resources.ResXFileRef(filePath, "System.String");

        sut.MakeFilePathRelative(basePath);

        Assert.Contains("file.txt", sut.FileName);
        Assert.DoesNotContain(basePath, sut.FileName);
    }

    [Fact]
    public void MakeFilePathRelative_WhenSamePath_ShouldReturnEmpty()
    {
        var basePath = "/home/user/project";

        var sut = new System.Resources.ResXFileRef(basePath, "System.String");

        sut.MakeFilePathRelative(basePath);

        Assert.Equal(string.Empty, sut.FileName);
    }
}