using Xunit;
using XmlDocSync;

namespace Prepare.Tests;

public class XmlDocFormatterTests
{
    [Fact]
    public void SplitsOpeningTagWithText_And_ReplacesSpacesWithTab()
    {
        var input = string.Join("\n", new[]
        {
            "    /// <filterpriority>1",
            "    /// </filterpriority>"
        });

        var expected = string.Join("\n", new[]
        {
            "    /// <filterpriority>",
            "    /// 1",
            "    /// </filterpriority>"
        });

        var output = XmlDocFormatter.Format(input);

        Assert.Equal(expected, output);
    }

    [Fact]
    public void ConvertsPlainTextLine_ToTabIndented()
    {
        var input = "    /// Some description text";

        var expected = "    /// Some description text";

        var output = XmlDocFormatter.Format(input);

        Assert.Equal(expected, output);
    }

    [Fact]
    public void PreservesIndentation()
    {
        var input = "\t\t/// <tag>value";

        var expected = string.Join("\n", new[]
        {
            "\t\t/// <tag>",
            "\t\t/// value"
        });

        var output = XmlDocFormatter.Format(input);

        Assert.Equal(expected, output);
    }

    [Fact]
    public void NonXmlLines_AreUnchanged()
    {
        var input = string.Join("\n", new[]
        {
            "namespace Test",
            "{",
            "    public class C {}",
            "}"
        });

        var output = XmlDocFormatter.Format(input);

        Assert.Equal(input, output);
    }
}