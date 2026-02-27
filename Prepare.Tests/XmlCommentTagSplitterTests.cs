using XmlDocSync;

namespace Prepare.Tests;

public class XmlCommentTagSplitterTests
{

    [Fact]
    public void Process_ShouldSplit_WhenShortText()
    {
        var input =
            @"    /// <summary>Short
public class MyClass
{
}";

        var expected =
            @"    /// <summary>
    /// Short
public class MyClass
{
}";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldNotSplit_WhenTextIsLongerThanLimit()
    {
        var input =
            @"/// <summary>This is definitely long";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(input.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldNotSplit_WhenRegexNotSatisfied()
    {
        var input =
            @"/// </summary>Short";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(input.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldPreserveIndentation()
    {
        var input =
            @"		/// <tag>Test";

        var expected =
            @"		/// <tag>
		/// Test";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldHandleMultipleOccurrences()
    {
        var input =
            @"/// <a>One
/// <b>Two
/// <c>ThisIsLongerThanTen";

        var expected =
            @"/// <a>
/// One
/// <b>
/// Two
/// <c>ThisIsLongerThanTen";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldTrimTextBeforeInserting()
    {
        var input =
            @"/// <tag>   Trim   ";

        var expected =
            @"/// <tag>
/// Trim";

        var result = XmlCommentTagSplitter.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }
}