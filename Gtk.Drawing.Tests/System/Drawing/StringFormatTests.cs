extern alias gtk;
using CharacterRange = gtk::System.Drawing.CharacterRange;
using HotkeyPrefix = gtk::System.Drawing.Text.HotkeyPrefix;
using StringAlignment = gtk::System.Drawing.StringAlignment;
using StringDigitSubstitute = gtk::System.Drawing.StringDigitSubstitute;
using StringFormat = gtk::System.Drawing.StringFormat;
using StringFormatFlags = gtk::System.Drawing.StringFormatFlags;
using StringTrimming = gtk::System.Drawing.StringTrimming;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class StringFormatTests
{
    #region Constructors

    [Fact]
    public void DefaultConstructor_HasDefaultValues()
    {
        var format = new StringFormat();

        Assert.Equal(StringAlignment.Near, format.Alignment);
        Assert.Equal(StringAlignment.Near, format.LineAlignment);
        Assert.Equal(StringFormatFlags.None, format.FormatFlags);
        Assert.Equal(HotkeyPrefix.None, format.HotkeyPrefix);
        Assert.Equal(StringTrimming.Character, format.Trimming);
    }

    [Fact]
    public void CopyConstructor_CopiesValues()
    {
        var original = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Far,
            FormatFlags = StringFormatFlags.DirectionRightToLeft,
            HotkeyPrefix = HotkeyPrefix.Show,
            Trimming = StringTrimming.EllipsisCharacter
        };

        original.SetDigitSubstitution(123, StringDigitSubstitute.Traditional);
        original.SetTabStops(5, new float[] { 10, 20 });

        var copy = new StringFormat(original);

        Assert.Equal(original.Alignment, copy.Alignment);
        Assert.Equal(original.LineAlignment, copy.LineAlignment);
        Assert.Equal(original.FormatFlags, copy.FormatFlags);
        Assert.Equal(original.HotkeyPrefix, copy.HotkeyPrefix);
        Assert.Equal(original.Trimming, copy.Trimming);
        Assert.Equal(original.DigitSubstitutionLanguage, copy.DigitSubstitutionLanguage);
        Assert.Equal(original.DigitSubstitutionMethod, copy.DigitSubstitutionMethod);
    }

    [Fact]
    public void CopyConstructor_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new StringFormat(null));
    }

    [Fact]
    public void Constructor_WithFlags_SetsFlags()
    {
        var format = new StringFormat(StringFormatFlags.DirectionVertical);

        Assert.Equal(StringFormatFlags.DirectionVertical, format.FormatFlags);
    }

    [Fact]
    public void Constructor_WithFlagsAndLanguage_SetsValues()
    {
        var format = new StringFormat(StringFormatFlags.NoClip, 999);

        Assert.Equal(StringFormatFlags.NoClip, format.FormatFlags);
        Assert.Equal(999, format.DigitSubstitutionLanguage);
    }

    #endregion

    #region Properties

    [Fact]
    public void Alignment_SetGet_Works()
    {
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center
        };

        Assert.Equal(StringAlignment.Center, format.Alignment);
    }

    [Fact]
    public void LineAlignment_SetGet_Works()
    {
        var format = new StringFormat
        {
            LineAlignment = StringAlignment.Far
        };

        Assert.Equal(StringAlignment.Far, format.LineAlignment);
    }

    [Fact]
    public void FormatFlags_SetGet_Works()
    {
        var format = new StringFormat
        {
            FormatFlags = StringFormatFlags.NoWrap
        };

        Assert.Equal(StringFormatFlags.NoWrap, format.FormatFlags);
    }

    [Fact]
    public void HotkeyPrefix_SetGet_Works()
    {
        var format = new StringFormat
        {
            HotkeyPrefix = HotkeyPrefix.Hide
        };

        Assert.Equal(HotkeyPrefix.Hide, format.HotkeyPrefix);
    }

    [Fact]
    public void Trimming_SetGet_Works()
    {
        var format = new StringFormat
        {
            Trimming = StringTrimming.Word
        };

        Assert.Equal(StringTrimming.Word, format.Trimming);
    }

    #endregion

    #region Digit Substitution

    [Fact]
    public void SetDigitSubstitution_SetsValues()
    {
        var format = new StringFormat();

        format.SetDigitSubstitution(123, StringDigitSubstitute.Traditional);

        Assert.Equal(123, format.DigitSubstitutionLanguage);
        Assert.Equal(StringDigitSubstitute.Traditional, format.DigitSubstitutionMethod);
    }

    #endregion

    #region TabStops

    [Fact]
    public void SetAndGetTabStops_Works()
    {
        var format = new StringFormat();

        format.SetTabStops(5, new float[] { 10, 20 });

        var tabs = format.GetTabStops(out float offset);

        Assert.Equal(5, offset);
        Assert.Equal(new float[] { 10, 20 }, tabs);
    }

    [Fact]
    public void SetTabStops_NullArray_UsesEmpty()
    {
        var format = new StringFormat();

        format.SetTabStops(3, null);

        var tabs = format.GetTabStops(out float offset);

        Assert.Equal(3, offset);
        Assert.Empty(tabs);
    }

    #endregion

    #region Character Ranges

    [Fact]
    public void SetMeasurableCharacterRanges_Valid_Works()
    {
        var format = new StringFormat();

        var ranges = new[]
        {
            new CharacterRange(0, 5),
            new CharacterRange(5, 5)
        };

        format.SetMeasurableCharacterRanges(ranges);

        // No exception = success
        Assert.True(true);
    }

    [Fact]
    public void SetMeasurableCharacterRanges_Null_Throws()
    {
        var format = new StringFormat();

        Assert.Throws<ArgumentNullException>(() =>
            format.SetMeasurableCharacterRanges(null));
    }

    [Fact]
    public void SetMeasurableCharacterRanges_TooMany_Throws()
    {
        var format = new StringFormat();

        var ranges = new CharacterRange[33];

        Assert.Throws<OverflowException>(() =>
            format.SetMeasurableCharacterRanges(ranges));
    }

    #endregion

    #region Clone

    [Fact]
    public void Clone_ReturnsDeepCopy()
    {
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center
        };
        format.SetTabStops(5, new float[] { 10 });

        var clone = (StringFormat)format.Clone();

        Assert.NotSame(format, clone);
        Assert.Equal(format.Alignment, clone.Alignment);

        var tabs = clone.GetTabStops(out _);
        Assert.Equal(new float[] { 10 }, tabs);
    }

    #endregion

    #region Static Properties

    [Fact]
    public void GenericDefault_ReturnsNewInstance()
    {
        var a = StringFormat.GenericDefault;
        var b = StringFormat.GenericDefault;

        Assert.NotSame(a, b);
    }

    [Fact]
    public void GenericTypographic_HasExpectedFlags()
    {
        var format = StringFormat.GenericTypographic;

        Assert.True(format.FormatFlags.HasFlag(StringFormatFlags.NoClip));
        Assert.True(format.FormatFlags.HasFlag(StringFormatFlags.FitBlackBox));
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_ReturnsReadableString()
    {
        var format = new StringFormat();

        var result = format.ToString();

        Assert.Contains("StringFormat", result);
    }

    #endregion

    #region Dispose

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var format = new StringFormat();

        format.Dispose();

        Assert.True(true);
    }

    #endregion
}