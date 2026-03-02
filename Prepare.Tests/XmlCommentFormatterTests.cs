using XmlDocSync.Formatters;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Prepare.Tests;

public class XmlCommentFormatterTests
{
    [Fact]
    public void FormatXmlComment_NullInput_ReturnsNull()
    {
        var result = XmlCommentFormatter.FormatXmlComment(null);
        Assert.Null(result);
    }

    [Fact]
    public void FormatXmlComment_SplitsInlineTextAfterOpenTag()
    {
        var input =
            @"/// <returns>A value
/// </returns>";

        var expected =
            @"/// <returns>
/// A value
/// </returns>";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_MergesConsecutiveLinesUnder80Chars()
    {
        var input =
            @"/// <summary>
/// Gets
/// the value.
/// </summary>";

        var expected =
            @"/// <summary>
/// Gets the value.
/// </summary>";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_DoesNotMergeLinesExceeding80Chars()
    {
        var longText = new string('a', 75);
        var input =
            $@"/// <summary>
/// {longText}
/// more
/// </summary>";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        // Should not merge because it would exceed 80 chars
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Contains(Normalize($@"/// {longText}"), result.TrimEnd('\n'));
#pragma warning restore CS8604 // Possible null reference argument.
        Assert.Contains("/// more", result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_PreservesSelfClosingTags()
    {
        var input =
            @"/// <summary>
/// Hello
/// <see cref=""T:System.String"" /> world.
/// </summary>";

        var expected =
            @"/// <summary>
/// Hello <see cref=""T:System.String"" /> world.
/// </summary>";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_StopsMergingAtOpenOrCloseTags()
    {
        var input =
            @"/// <returns>
/// A
/// <see cref=""T:System.Drawing.Color"" />
/// </returns>
/// More text";

        var expected =
            @"/// <returns>
/// A <see cref=""T:System.Drawing.Color"" />
/// </returns>
/// More text";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(expected), result.TrimEnd('\n'));
    }

    [Fact]
    public void FormatXmlComment_FullExample_ComplexCase()
    {
        var input =
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

        var expected =
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

        var result = XmlCommentFormatter.FormatXmlComment(input);

        expected = Normalize(expected);
        result = result.TrimEnd('\n');
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatXmlComment_NormalLinesRemainUnchanged()
    {
        var input =
            @"/// Just a comment
/// Another line";

        var result = XmlCommentFormatter.FormatXmlComment(input);

        Assert.Equal(Normalize(input), result.TrimEnd('\n'));
    }

    private static string? Normalize(string? s) =>
        s?.Replace("\r\n", "\n").Replace("\r", "\n");

}
