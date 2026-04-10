using System;
using System.Drawing.Text;

namespace System.Drawing;

/// <summary>
/// Encapsulates text layout information (such as alignment, orientation and tab stops) display manipulations (such as ellipsis insertion and national digit substitution) and OpenType features. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class StringFormat : MarshalByRefObject, ICloneable, IDisposable
{
    private StringAlignment _alignment = StringAlignment.Near;
    private StringAlignment _lineAlignment = StringAlignment.Near;
    private StringFormatFlags _formatFlags = StringFormatFlags.None;
    private HotkeyPrefix _hotkeyPrefix = HotkeyPrefix.None;
    private StringTrimming _trimming = StringTrimming.Character;

    private int _digitLanguage;
    private StringDigitSubstitute _digitMethod = StringDigitSubstitute.None;

    private float _firstTabOffset;
    private float[] _tabStops = Array.Empty<float>();

    private CharacterRange[] _ranges = Array.Empty<CharacterRange>();

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    public StringFormat()
    {
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.StringFormat" /> object from the specified existing
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <param name="format">
    /// The
    /// <see cref="T:System.Drawing.StringFormat" /> object from which to initialize the new
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is
    /// <see langword="null" />.
    /// </exception>
    public StringFormat(StringFormat format)
    {
        if (format == null)
            throw new ArgumentNullException(nameof(format));

        _alignment = format._alignment;
        _lineAlignment = format._lineAlignment;
        _formatFlags = format._formatFlags;
        _hotkeyPrefix = format._hotkeyPrefix;
        _trimming = format._trimming;

        _digitLanguage = format._digitLanguage;
        _digitMethod = format._digitMethod;

        _firstTabOffset = format._firstTabOffset;
        _tabStops = (float[])format._tabStops.Clone();
        _ranges = (CharacterRange[])format._ranges.Clone();
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.StringFormat" /> object with the specified
    /// <see cref="T:System.Drawing.StringFormatFlags" /> enumeration.
    /// </summary>
    public StringFormat(StringFormatFlags options)
    {
        _formatFlags = options;
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.StringFormat" /> object with the specified
    /// <see cref="T:System.Drawing.StringFormatFlags" /> enumeration and language.
    /// </summary>
    public StringFormat(StringFormatFlags options, int language)
    {
        _formatFlags = options;
        _digitLanguage = language;
    }

    /// <summary>
    /// Gets or sets horizontal alignment of the string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.StringAlignment" /> enumeration that specifies the horizontal  alignment of the string.
    /// </returns>
    public StringAlignment Alignment
    {
        get => _alignment;
        set => _alignment = value;
    }

    /// <summary>
    /// Gets the language that is used when local digits are substituted for western digits.
    /// </summary>
    /// <returns>
    /// A National Language Support (NLS) language identifier that identifies the language that will be used when local digits are substituted for western digits. You can pass the
    /// <see cref="T:System.Globalization.CultureInfo.LCID" /> property of a
    /// <see cref="T:System.Globalization.CultureInfo" /> object as the NLS language identifier. For example, suppose you create a
    /// <see cref="T:System.Globalization.CultureInfo" /> object by passing the string "ar-EG" to a
    /// <see cref="T:System.Globalization.CultureInfo" /> constructor. If you pass the
    /// <see cref="T:System.Globalization.CultureInfo.LCID" /> property of that
    /// <see cref="T:System.Globalization.CultureInfo" /> object along with
    /// <see cref="T:System.Drawing.StringDigitSubstitute.Traditional" /> to the
    /// <see cref="T:System.Drawing.StringFormat.SetDigitSubstitution(System.Int32,System.Drawing.StringDigitSubstitute)" /> method, then Arabic-Indic digits will be substituted for western digits at display time.
    /// </returns>
    public int DigitSubstitutionLanguage => _digitLanguage;

    /// <summary>
    /// Gets the method to be used for digit substitution.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.StringDigitSubstitute" /> enumeration value that specifies how to substitute characters in a string that cannot be displayed because they are not supported by the current font.
    /// </returns>
    public StringDigitSubstitute DigitSubstitutionMethod => _digitMethod;

    /// <summary>
    /// Gets or sets a
    /// <see cref="T:System.Drawing.StringFormatFlags" /> enumeration that contains formatting information.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.StringFormatFlags" /> enumeration that contains formatting information.
    /// </returns>
    public StringFormatFlags FormatFlags
    {
        get => _formatFlags;
        set => _formatFlags = value;
    }

    /// <summary>
    /// Gets a generic default
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <returns>
    /// The generic default
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </returns>
    public static StringFormat GenericDefault => new StringFormat();

    /// <summary>
    /// Gets a generic typographic
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <returns>
    /// A generic typographic
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </returns>
    public static StringFormat GenericTypographic =>
        new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox);

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Drawing.Text.HotkeyPrefix" /> object for this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Text.HotkeyPrefix" /> object for this
    /// <see cref="T:System.Drawing.StringFormat" /> object, the default is
    /// <see cref="T:System.Drawing.Text.HotkeyPrefix.None" />.
    /// </returns>
    public HotkeyPrefix HotkeyPrefix
    {
        get => _hotkeyPrefix;
        set => _hotkeyPrefix = value;
    }

    /// <summary>
    /// Gets or sets the vertical alignment of the string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.StringAlignment" /> enumeration that represents the vertical line alignment.
    /// </returns>
    public StringAlignment LineAlignment
    {
        get => _lineAlignment;
        set => _lineAlignment = value;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Drawing.StringTrimming" /> enumeration for this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.StringTrimming" /> enumeration that indicates how text drawn with this
    /// <see cref="T:System.Drawing.StringFormat" /> object is trimmed when it exceeds the edges of the layout rectangle.
    /// </returns>
    public StringTrimming Trimming
    {
        get => _trimming;
        set => _trimming = value;
    }

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.StringFormat" /> object this method creates.
    /// </returns>
    public object Clone()
    {
        return new StringFormat(this);
    }

    /// <summary>
    /// Releases all resources used by this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    public void Dispose()
    {
        // No unmanaged resources in this implementation
    }

    /// <summary>
    /// Gets the tab stops for this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <param name="firstTabOffset">
    /// The number of spaces between the beginning of a text line and the first tab stop.
    /// </param>
    /// <returns>
    /// An array of distances (in number of spaces) between tab stops.
    /// </returns>
    public float[] GetTabStops(out float firstTabOffset)
    {
        firstTabOffset = _firstTabOffset;
        return (float[])_tabStops.Clone();
    }

    /// <summary>
    /// Specifies the language and method to be used when local digits are substituted for western digits.
    /// </summary>
    /// <param name="language">
    /// A National Language Support (NLS) language identifier that identifies the language that will be used when local digits are substituted for western digits. You can pass the
    /// <see cref="T:System.Globalization.CultureInfo.LCID" /> property of a
    /// <see cref="T:System.Globalization.CultureInfo" /> object as the NLS language identifier. For example, suppose you create a
    /// <see cref="T:System.Globalization.CultureInfo" /> object by passing the string "ar-EG" to a
    /// <see cref="T:System.Globalization.CultureInfo" /> constructor. If you pass the
    /// <see cref="T:System.Globalization.CultureInfo.LCID" /> property of that
    /// <see cref="T:System.Globalization.CultureInfo" /> object along with
    /// <see cref="T:System.Drawing.StringDigitSubstitute.Traditional" /> to the
    /// <see cref="T:System.Drawing.StringFormat.SetDigitSubstitution(System.Int32,System.Drawing.StringDigitSubstitute)" /> method, then Arabic-Indic digits will be substituted for western digits at display time.
    /// </param>
    /// <param name="substitute">
    /// An element of the
    /// <see cref="T:System.Drawing.StringDigitSubstitute" /> enumeration that specifies how digits are displayed.
    /// </param>
    public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
    {
        _digitLanguage = language;
        _digitMethod = substitute;
    }

    /// <summary>
    /// Specifies an array of
    /// <see cref="T:System.Drawing.CharacterRange" /> structures that represent the ranges of characters measured by a call to the
    /// <see cref="T:System.Drawing.Graphics.MeasureCharacterRanges(System.String,System.Drawing.Font,System.Drawing.RectangleF,System.Drawing.StringFormat)" /> method.
    /// </summary>
    /// <param name="ranges">
    /// An array of
    /// <see cref="T:System.Drawing.CharacterRange" /> structures that specifies the ranges of characters measured by a call to the
    /// <see cref="T:System.Drawing.Graphics.MeasureCharacterRanges(System.String,System.Drawing.Font,System.Drawing.RectangleF,System.Drawing.StringFormat)" /> method.
    /// </param>
    /// <exception cref="T:System.OverflowException">
    /// More than 32 character ranges are set.
    /// </exception>
    public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
    {
        if (ranges == null)
            throw new ArgumentNullException(nameof(ranges));

        if (ranges.Length > 32)
            throw new OverflowException("More than 32 character ranges are set.");

        _ranges = (CharacterRange[])ranges.Clone();
    }

    /// <summary>
    /// Sets tab stops for this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </summary>
    /// <param name="firstTabOffset">
    /// The number of spaces between the beginning of a line of text and the first tab stop.
    /// </param>
    /// <param name="tabStops">
    /// An array of distances between tab stops in the units specified by the
    /// <see cref="T:System.Drawing.Graphics.PageUnit" /> property.
    /// </param>
    public void SetTabStops(float firstTabOffset, float[] tabStops)
    {
        _firstTabOffset = firstTabOffset;
        _tabStops = tabStops != null ? (float[])tabStops.Clone() : Array.Empty<float>();
    }

    /// <summary>
    /// Converts this
    /// <see cref="T:System.Drawing.StringFormat" /> object to a human-readable string.
    /// </summary>
    /// <returns>
    /// A string representation of this
    /// <see cref="T:System.Drawing.StringFormat" /> object.
    /// </returns>
    public override string ToString()
    {
        return $"StringFormat [Alignment={_alignment}, LineAlignment={_lineAlignment}, FormatFlags={_formatFlags}]";
    }
}