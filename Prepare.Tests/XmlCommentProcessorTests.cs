using XmlDocSync;

namespace Prepare.Tests;

public class XmlCommentProcessorTests
{

    [Fact]
    public void Process_ShouldMergeShortXmlCommentLine()
    {
        var input =
            @"/// This is a summary
/// short
public class MyClass
{
}";

        var expected =
            @"/// This is a summary short
public class MyClass
{
}";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldNotMerge_WhenTextIsLongerThanLimit()
    {
        var input =
            @"/// This is a summary
/// this text is long enough
public class MyClass
{
}";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(input.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldNotMerge_WhenLineIsXmlElement()
    {
        var input =
            @"/// <summary>
/// short
/// </summary>";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(input.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldNotMerge_WhenPreviousLineIsNotXmlComment()
    {
        var input =
            @"// normal comment
/// short";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(input.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldTrimBeforeMerging()
    {
        var input =
            @"/// Hello
///   test   
public class MyClass
{
}";

        var expected =
            @"/// Hello test
public class MyClass
{
}";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }

    [Fact]
    public void Process_ShouldHandleMultipleSequentialMerges()
    {
        var input =
            @"/// Hello
/// a
/// b
/// c
public class MyClass
{
}";

        var expected =
            @"/// Hello a b c
public class MyClass
{
}";

        var result = XmlCommentProcessor.Process(input);

        Assert.Equal(expected.Replace("\r\n", "\n"), result);
    }
}