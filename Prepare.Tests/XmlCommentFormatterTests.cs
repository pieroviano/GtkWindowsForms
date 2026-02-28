using XmlDocSync.Formatters;
using Xunit;
using XmlDocSync.Services;

namespace Prepare.Tests;

public class XmlCommentFormatterTests
{
    [Fact]
    public void FormatXmlComment_NullInput_ReturnsNull()
    {
        string result = XmlCommentFormatter.FormatXmlComment(null);
        Assert.Null(result);
    }

    [Fact]
    public void FormatXmlComment_SplitsInlineTextAfterOpenTag()
    {
        string input =
            @"/// <returns>A value
/// </returns>";

        string expected =
            @"/// <returns>
/// A value
/// </returns>";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_MergesConsecutiveLinesUnder80Chars()
    {
        string input =
            @"/// <summary>
/// Gets
/// the value.
/// </summary>";

        string expected =
            @"/// <summary>
/// Gets the value.
/// </summary>";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_DoesNotMergeLinesExceeding80Chars()
    {
        string longText = new string('a', 75);
        string input =
            $@"/// <summary>
/// {longText}
/// more
/// </summary>";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        // Should not merge because it would exceed 80 chars
        Assert.Contains(Normalize($@"/// {longText}"), result.TrimEnd('\n'));
        Assert.Contains("/// more", result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_PreservesSelfClosingTags()
    {
        string input =
            @"/// <summary>
/// Hello
/// <see cref=""T:System.String"" /> world.
/// </summary>";

        string expected =
            @"/// <summary>
/// Hello <see cref=""T:System.String"" /> world.
/// </summary>";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_StopsMergingAtOpenOrCloseTags()
    {
        string input =
            @"/// <returns>
/// A
/// <see cref=""T:System.Drawing.Color"" />
/// </returns>
/// More text";

        string expected =
            @"/// <returns>
/// A <see cref=""T:System.Drawing.Color"" />
/// </returns>
/// More text";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_FullExample_ComplexCase()
    {
        string input =
            @"/// <summary> Gets or sets the color of this
/// <see cref=""T:System.Drawing.SolidBrush"" /> object.
/// </summary>
/// <returns>
/// A
/// <see cref=""T:System.Drawing.Color"" /> structure that represents the color of this brush.
/// </returns>
/// <exception cref=""T:System.ArgumentException"">
/// The
/// <see cref=""P:System.Drawing.SolidBrush.Color"" /> property is 
/// set on an immutable
/// <see cref=""T:System.Drawing.SolidBrush"" />.
/// </exception>";

        string expected =
            @"/// <summary>
/// Gets or sets the color of this <see cref=""T:System.Drawing.SolidBrush"" /> object.
/// </summary>
/// <returns>
/// A <see cref=""T:System.Drawing.Color"" /> structure that represents the color of this brush.
/// </returns>
/// <exception cref=""T:System.ArgumentException"">
/// The <see cref=""P:System.Drawing.SolidBrush.Color"" /> property is
/// set on an immutable <see cref=""T:System.Drawing.SolidBrush"" />.
/// </exception>";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        expected = Normalize(expected);
        result = result.TrimEnd('\n');
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatXmlComment_NormalLinesRemainUnchanged()
    {
        string input =
            @"/// Just a comment
/// Another line";

        string result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(input), result.TrimEnd('\n'));
    }

    private static string Normalize(string s) =>
        s is null ? null : s.Replace("\r\n", "\n").Replace("\r", "\n");

}
