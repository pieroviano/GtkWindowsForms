namespace System.Windows.Forms.Misc;

public class ClientUtilsGetWordBoundaryStartTests
{
    [Theory]
    [InlineData("hello", 5, 0)] // full word
    [InlineData("hello world", 11, 6)] // last word
    [InlineData("hello world", 5, 0)] // first word
    public void Returns_StartOfWord(string text, int endIndex, int expected)
    {
        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello!!!", 8, 0)] // word + punctuation
    [InlineData("abc!!!", 6, 0)]
    public void Includes_TrailingNonWordCharacters(string text, int endIndex, int expected)
    {
        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("!!!", 3, 0)] // only punctuation
    [InlineData("   ", 3, 0)] // spaces
    public void NonWordOnly_ReturnsZero(string text, int endIndex, int expected)
    {
        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Mixed_WordAndNonWord_BreaksCorrectly()
    {
        string text = "abc!!!def";
        int endIndex = 9; // end of string

        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        // should select "def"
        Assert.Equal(6, result);
    }

    [Fact]
    public void Mixed_SelectsWordPlusTrailingSymbols()
    {
        string text = "abc!!!";
        int endIndex = text.Length;

        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        // selects entire "abc!!!"
        Assert.Equal(0, result);
    }

    [Fact]
    public void StopsAtStart_WhenFirstCharIsNonWord()
    {
        string text = "!abc";
        int endIndex = text.Length;

        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Handles_EmptyString()
    {
        int result = ClientUtils.GetWordBoundaryStart("", 0);

        Assert.Equal(0, result);
    }

    [Fact]
    public void SurrogateCharacter_StopsProcessing()
    {
        // surrogate pair (emoji)
        string text = "abc🙂def";
        int endIndex = text.Length;

        int result = ClientUtils.GetWordBoundaryStart(text, endIndex);

        // behavior: stops when hitting surrogate → should not crash
        Assert.InRange(result, 0, text.Length);
    }
}