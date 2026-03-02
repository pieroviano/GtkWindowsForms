using XmlDocSync.Formatters;

namespace Prepare.Tests;

public class XmlDocumentationFormatterTests
{
    [Fact]
    public void Format_ShouldFormatSingleLineBlock()
    {
        var input = """
                    /// <summary><b>Test</b></summary>
                    public class A {}
                    """;

        var result = XmlDocumentationFormatter.Format(input, new[]
        {
            ("</summary></param>", "</param></summary>"),
            ("<devdov>", "&lt;devdov&gt;"),
        });

        Assert.Contains("/// <summary>", result);
        Assert.Contains("///   <b>Test</b>", result);
        Assert.Contains("/// </summary>", result);
    }

    [Fact]
    public void Format_ShouldPreserveIndentation()
    {
        var input = """
                        /// <summary><b>Test</b></summary>
                        public class A {}
                    """;

        var result = XmlDocumentationFormatter.Format(input, new[]
        {
            ("</summary></param>", "</param></summary>"),
            ("<devdov>", "&lt;devdov&gt;"),
        });

        Assert.Contains("    /// <summary>", result);
        Assert.Contains("    ///   <b>Test</b>", result);
    }

    [Fact]
    public void Format_ShouldHandleMultipleBlocks()
    {
        var input = """
                    /// <summary>One</summary>
                    public class A {}

                    /// <summary>Two</summary>
                    public class B {}
                    """;

        var result = XmlDocumentationFormatter.Format(input, new[]
        {
            ("</summary></param>", "</param></summary>"),
            ("<devdov>", "&lt;devdov&gt;"),
        });

        Assert.Contains("/// <summary>", result);
        Assert.Contains("One", result);
        Assert.Contains("Two", result);
    }

    [Fact]
    public void Format_ShouldLeaveNonXmlDocLinesUntouched()
    {
        var input = """
                    // normal comment
                    public class A {}
                    """;

        var result = XmlDocumentationFormatter.Format(input, new[]
        {
            ("</summary></param>", "</param></summary>"),
            ("<devdov>", "&lt;devdov&gt;"),
        });

        Assert.Equal(input.Replace("\r\n", "\n"), result?.Replace("\r\n", "\n"));
    }

    [Fact]
    public void Format_ShouldHandleMultilineXmlInsideBlock()
    {
        var input = """
                    /// <summary>
                    /// <b>Test</b>
                    /// </summary>
                    public class A {}
                    """;

        var result = XmlDocumentationFormatter.Format(input, new[]
        {
            ("</summary></param>", "</param></summary>"),
            ("<devdov>", "&lt;devdov&gt;"),
        });

        Assert.Contains("/// <summary>", result);
        Assert.Contains("///   <b>Test</b>", result);
        Assert.Contains("/// </summary>", result);
    }
}
