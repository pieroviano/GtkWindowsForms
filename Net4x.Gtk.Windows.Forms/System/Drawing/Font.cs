using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Drawing
{

    /// <summary>
    /// Defines a particular format for text, including font face, size, and style attributes.
    /// This class cannot be inherited.
    /// </summary>
    [TypeConverter("System.Drawing.FontConverter, System.Windows.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    public sealed class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
    {

        /// <summary>
        /// Initializes a new System.Drawing.Font that uses the specified existing System.Drawing.Font
        /// and System.Drawing.FontStyle enumeration.
        /// <param name="prototype">The existing System.Drawing.Font from which to create the new System.Drawing.Font.</param>
        /// <param name="newStyle">The System.Drawing.FontStyle to apply to the new System.Drawing.Font. Multiple values of the System.Drawing.FontStyle enumeration can be combined with the OR operator. </summary></param>
        /// </summary>
        public Font(Font prototype, FontStyle newStyle) : this(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, prototype.GdiCharSet, prototype.GdiVerticalFont)
        {

        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
        {

        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize) : this(new FontFamily(familyName), emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size and style.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.</param>
        /// <param name="T:System.ArgumentNullException">family is null. </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style) : this(family, emSize, style, GraphicsUnit.Point, 1, false) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size and unit. The style
        /// is set to System.Drawing.FontStyle.Regular.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, FontStyle.Regular, unit, 1, false)
        {

        
        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size and style.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style) : this(new FontFamily(familyName), emSize, style, GraphicsUnit.Point, 1, false)
        {
            this.FontFamily = new FontFamily(familyName);
            this.Size = emSize;
            this.Style = style;
        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size and unit. Sets the
        /// style to System.Drawing.FontStyle.Regular.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentNullException">family is null.</param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize, GraphicsUnit unit) : this(family, emSize, FontStyle.Regular, unit, 1, false)
        {

        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size, style, and unit.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, style, unit, 1, false)
        {
        }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size, style, and unit.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.</param>
        /// <param name="T:System.ArgumentNullException">family is null. </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit) : this(family, emSize, style, unit, 1, false) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size, style, unit, and
        /// character set.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="gdiCharSet">A System.Byte that specifies a GDI character set to use for the new font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.</param>
        /// <param name="T:System.ArgumentNullException">family is null. </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(family, emSize, style, unit, gdiCharSet, false) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size, style, unit, and
        /// character set.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="gdiCharSet">A System.Byte that specifies a GDI character set to use for this font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(new FontFamily(familyName), emSize, style, unit, gdiCharSet, false) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using the specified size, style, unit,
        /// and character set.
        /// <param name="familyName">A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="gdiCharSet">A System.Byte that specifies a GDI character set to use for this font.</param>
        /// <param name="gdiVerticalFont">A Boolean value indicating whether the new System.Drawing.Font is derived from a GDI vertical font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number. </summary></param>
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) :this(new FontFamily(familyName), emSize, style, unit, gdiCharSet, gdiVerticalFont) { }

        /// <summary>
        /// Initializes a new System.Drawing.Font using a specified size, style, unit, and
        /// character set.
        /// <param name="family">The System.Drawing.FontFamily of the new System.Drawing.Font.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
        /// <param name="style">The System.Drawing.FontStyle of the new font.</param>
        /// <param name="unit">The System.Drawing.GraphicsUnit of the new font.</param>
        /// <param name="gdiCharSet">A System.Byte that specifies a GDI character set to use for this font.</param>
        /// <param name="gdiVerticalFont">A Boolean value indicating whether the new font is derived from a GDI vertical font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.</param>
        /// <param name="T:System.ArgumentNullException">family is null </summary></param>
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) {
            this.Name = family?.Name;
            this.FontFamily = family;
            this.Size = emSize;
            this.Style = style;
            this.Unit = unit;
            this.GdiCharSet = gdiCharSet;
            this.GdiVerticalFont = gdiVerticalFont;
        }


        /// <summary>
        /// Gets a value that indicates whether this font has the italic style applied.
        /// Return result:
        /// true to indicate this font has the italic style applied; otherwise, false.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Italic { get => Style.HasFlag(FontStyle.Italic); }

        /// <summary>
        /// Gets a byte value that specifies the GDI character set that this System.Drawing.Font
        /// uses.
        /// Return result:
        /// A byte value that specifies the GDI character set that this System.Drawing.Font
        /// uses. The default is 1.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte GdiCharSet { get; }

        /// <summary>
        /// Gets a Boolean value that indicates whether this System.Drawing.Font is derived
        /// from a GDI vertical font.
        /// Return result:
        /// true if this System.Drawing.Font is derived from a GDI vertical font; otherwise,
        /// false.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GdiVerticalFont { get; }

        /// <summary>
        /// Gets the line spacing of this font.
        /// Return result:
        /// The line spacing, in pixels, of this font.
        /// </summary>
        [Browsable(false)]
        public int Height { get; }

        /// <summary>
        /// Gets a value indicating whether the font is a member of System.Drawing.SystemFonts.
        /// Return result:
        /// true if the font is a member of System.Drawing.SystemFonts; otherwise, false.
        /// The default is false.
        /// </summary>
        [Browsable(false)]
        public bool IsSystemFont { get; }

        /// <summary>
        /// Gets the face name of this System.Drawing.Font.
        /// Return result:
        /// A string representation of the face name of this System.Drawing.Font.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name { get; }

        /// <summary>
        /// Gets the name of the system font if the System.Drawing.Font.IsSystemFont property
        /// returns true.
        /// Return result:
        /// The name of the system font, if System.Drawing.Font.IsSystemFont returns true;
        /// otherwise, an empty string ("").
        /// </summary>
        [Browsable(false)]
        public string SystemFontName { get; }

        /// <summary>
        /// Gets the em-size of this System.Drawing.Font measured in the units specified
        /// by the System.Drawing.Font.Unit property.
        /// Return result:
        /// The em-size of this System.Drawing.Font.
        /// </summary>
        public float Size { get; }

        /// <summary>
        /// Gets the em-size, in points, of this System.Drawing.Font.
        /// Return result:
        /// The em-size, in points, of this System.Drawing.Font.
        /// </summary>
        [Browsable(false)]
        public float SizeInPoints { get; }

        /// <summary>
        /// Gets a value that indicates whether this System.Drawing.Font specifies a horizontal
        /// line through the font.
        /// Return result:
        /// true if this System.Drawing.Font has a horizontal line through it; otherwise,
        /// false.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Strikeout { get; }

        /// <summary>
        /// Gets style information for this System.Drawing.Font.
        /// Return result:
        /// A System.Drawing.FontStyle enumeration that contains style information for this
        /// System.Drawing.Font.
        /// </summary>
        [Browsable(false)]
        public FontStyle Style { get; }

        /// <summary>
        /// Gets the System.Drawing.FontFamily associated with this System.Drawing.Font.
        /// Return result:
        /// The System.Drawing.FontFamily associated with this System.Drawing.Font.
        /// </summary>
        [Browsable(false)]
        public FontFamily FontFamily { get; }

        /// <summary>
        /// Gets the name of the font originally specified.
        /// Return result:
        /// The string representing the name of the font originally specified.
        /// </summary>
        [Browsable(false)]
        public string OriginalFontName { get; }

        /// <summary>
        /// Gets a value that indicates whether this System.Drawing.Font is bold.
        /// Return result:
        /// true if this System.Drawing.Font is bold; otherwise, false.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Bold { get=> Style.HasFlag(FontStyle.Bold); }

        /// <summary>
        /// Gets the unit of measure for this System.Drawing.Font.
        /// Return result:
        /// A System.Drawing.GraphicsUnit that represents the unit of measure for this System.Drawing.Font.
        /// </summary>
        public GraphicsUnit Unit { get; }

        /// <summary>
        /// Gets a value that indicates whether this System.Drawing.Font is underlined.
        /// Return result:
        /// true if this System.Drawing.Font is underlined; otherwise, false.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Underline { get => Style.HasFlag(FontStyle.Underline); }


        /// <summary>
        /// Creates a System.Drawing.Font from the specified Windows handle to a device context.
        /// <param name="hdc">A handle to a device context.</param>
        /// <param name="Return result">The System.Drawing.Font this method creates.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">The font for the specified device context is not a TrueType font. </summary></param>
        /// </summary>
        public static Font FromHdc(IntPtr hdc) { return new Font("Arial", 12); }

        /// <summary>
        /// Creates a System.Drawing.Font from the specified Windows handle.
        /// <param name="hfont">A Windows handle to a GDI font.</param>
        /// <param name="Return result">The System.Drawing.Font this method creates.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">hfont points to an object that is not a TrueType font. </summary></param>
        /// </summary>
        public static Font FromHfont(IntPtr hfont) { return new Font("Arial", 12); }

        /// <summary>
        /// Creates a System.Drawing.Font from the specified GDI logical font (LOGFONT) structure.
        /// <param name="lf">An System.Object that represents the GDI LOGFONT structure from which to create the System.Drawing.Font.</param>
        /// <param name="hdc">A handle to a device context that contains additional information about the lf structure.</param>
        /// <param name="Return result">The System.Drawing.Font that this method creates.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentException">The font is not a TrueType font. </summary></param>
        /// </summary>
        public static Font FromLogFont(object lf, IntPtr hdc) { return new Font("Arial", 12); }

        /// <summary>
        /// Creates a System.Drawing.Font from the specified GDI logical font (LOGFONT) structure.
        /// <param name="lf">An System.Object that represents the GDI LOGFONT structure from which to create the System.Drawing.Font.</param>
        /// <param name="Return result">The System.Drawing.Font that this method creates. </summary></param>
        /// </summary>
        public static Font FromLogFont(object lf) { return new Font("Arial", 12); }

        /// <summary>
        /// Creates an exact copy of this System.Drawing.Font.
        /// Return result:
        /// The System.Drawing.Font this method creates, cast as an System.Object.
        /// </summary>
        public object Clone() { return null; }

        /// <summary>
        /// Releases all resources used by this System.Drawing.Font.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Indicates whether the specified object is a System.Drawing.Font and has the same
        /// System.Drawing.Font.FontFamily, System.Drawing.Font.GdiVerticalFont, System.Drawing.Font.GdiCharSet,
        /// System.Drawing.Font.Style, System.Drawing.Font.Size, and System.Drawing.Font.Unit
        /// property values as this System.Drawing.Font.
        /// <param name="obj">The object to test.</param>
        /// <param name="Return result">true if the obj parameter is a System.Drawing.Font and has the same System.Drawing.Font.FontFamily, System.Drawing.Font.GdiVerticalFont, System.Drawing.Font.GdiCharSet, System.Drawing.Font.Style, System.Drawing.Font.Size, and System.Drawing.Font.Unit property values as this System.Drawing.Font; otherwise, false. </summary></param>
        /// </summary>
        public override bool Equals(object obj) { return Equals(obj as Font); }

        /// <summary>
        /// Gets the hash code for this System.Drawing.Font.
        /// Return result:
        /// The hash code for this System.Drawing.Font.
        /// </summary>
        public override int GetHashCode() { return base.GetHashCode(); }

        /// <summary>
        /// Returns the height, in pixels, of this System.Drawing.Font when drawn to a device
        /// with the specified vertical resolution.
        /// <param name="dpi">The vertical resolution, in dots per inch, used to calculate the height of the font.</param>
        /// <param name="Return result">The height, in pixels, of this System.Drawing.Font. </summary></param>
        /// </summary>
        public float GetHeight(float dpi) { return Height; }

        /// <summary>
        /// Returns the line spacing, in the current unit of a specified System.Drawing.Graphics,
        /// of this font.
        /// <param name="graphics">A System.Drawing.Graphics that holds the vertical resolution, in dots per inch, of the display device as well as settings for page unit and page scale.</param>
        /// <param name="Return result">The line spacing, in pixels, of this font.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentNullException">graphics is null. </summary></param>
        /// </summary>
        public float GetHeight(Graphics graphics) { return Height; }

        /// <summary>
        /// Returns the line spacing, in pixels, of this font.
        /// Return result:
        /// The line spacing, in pixels, of this font.
        /// </summary>
        public float GetHeight() { return Height; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           
        }


        /// <summary>
        /// Returns a handle to this System.Drawing.Font.
        /// Return result:
        /// A Windows handle to this System.Drawing.Font.
        /// <exception cref="System.ComponentModel.Win32Exception">The operation was unsuccessful. </summary></exception>
        /// </summary>
        public IntPtr ToHfont() { return IntPtr.Zero; }

        /// <summary>
        /// Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.
        /// <param name="logFont">An System.Object to represent the LOGFONT structure that this method creates.</param>
        /// <param name="graphics">A System.Drawing.Graphics that provides additional information for the LOGFONT structure.</param>
        /// <param name="Exception"></param>
        /// <param name="T:System.ArgumentNullException">graphics is null. </summary></param>
        /// </summary>
        public void ToLogFont(object logFont, Graphics graphics) { }

        /// <summary>
        /// Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.
        /// <param name="logFont">An System.Object to represent the LOGFONT structure that this method creates. </summary></param>
        /// </summary>
        public void ToLogFont(object logFont) { }

        /// <summary>
        /// Returns a human-readable string representation of this System.Drawing.Font.
        /// Return result:
        /// A string that represents this System.Drawing.Font.
        /// </summary>
        public override string ToString() { return "Font"; }
    }
}