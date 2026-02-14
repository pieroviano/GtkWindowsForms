using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Drawing
{
    //
    // Summary:
    //     Defines a particular format for text, including font face, size, and style attributes.
    //     This class cannot be inherited.
    [TypeConverter("System.Drawing.FontConverter, System.Windows.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")]
    public sealed class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
    {
        //
        // Summary:
        //     Initializes a new System.Drawing.Font that uses the specified existing System.Drawing.Font
        //     and System.Drawing.FontStyle enumeration.
        //
        // Parameter:
        //   prototype:
        //     The existing System.Drawing.Font from which to create the new System.Drawing.Font.
        //
        //   newStyle:
        //     The System.Drawing.FontStyle to apply to the new System.Drawing.Font. Multiple
        //     values of the System.Drawing.FontStyle enumeration can be combined with the OR
        //     operator.
        public Font(Font prototype, FontStyle newStyle) : this(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, prototype.GdiCharSet, prototype.GdiVerticalFont)
        {

        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size, in points, of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
        {

        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size, in points, of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity or is not a valid number.
        public Font(string familyName, float emSize) : this(new FontFamily(familyName), emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size and style.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size, in points, of the new font.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        //
        //   T:System.ArgumentNullException:
        //     family is null.
        public Font(FontFamily family, float emSize, FontStyle style) : this(family, emSize, style, GraphicsUnit.Point, 1, false) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size and unit. The style
        //     is set to System.Drawing.FontStyle.Regular.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(string familyName, float emSize, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, FontStyle.Regular, unit, 1, false)
        {

        
        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size and style.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size, in points, of the new font.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(string familyName, float emSize, FontStyle style) : this(new FontFamily(familyName), emSize, style, GraphicsUnit.Point, 1, false)
        {
            this.FontFamily = new FontFamily(familyName);
            this.Size = emSize;
            this.Style = style;
        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size and unit. Sets the
        //     style to System.Drawing.FontStyle.Regular.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        // Exception:
        //   T:System.ArgumentNullException:
        //     family is null.
        //
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(FontFamily family, float emSize, GraphicsUnit unit) : this(family, emSize, FontStyle.Regular, unit, 1, false)
        {

        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size, style, and unit.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity or is not a valid number.
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, style, unit, 1, false)
        {
        }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size, style, and unit.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        //
        //   T:System.ArgumentNullException:
        //     family is null.
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit) : this(family, emSize, style, unit, 1, false) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size, style, unit, and
        //     character set.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        //   gdiCharSet:
        //     A System.Byte that specifies a GDI character set to use for the new font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        //
        //   T:System.ArgumentNullException:
        //     family is null.
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(family, emSize, style, unit, gdiCharSet, false) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size, style, unit, and
        //     character set.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        //   gdiCharSet:
        //     A System.Byte that specifies a GDI character set to use for this font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(new FontFamily(familyName), emSize, style, unit, gdiCharSet, false) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using the specified size, style, unit,
        //     and character set.
        //
        // Parameter:
        //   familyName:
        //     A string representation of the System.Drawing.FontFamily for the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        //   gdiCharSet:
        //     A System.Byte that specifies a GDI character set to use for this font.
        //
        //   gdiVerticalFont:
        //     A Boolean value indicating whether the new System.Drawing.Font is derived from
        //     a GDI vertical font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) :this(new FontFamily(familyName), emSize, style, unit, gdiCharSet, gdiVerticalFont) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.Font using a specified size, style, unit, and
        //     character set.
        //
        // Parameter:
        //   family:
        //     The System.Drawing.FontFamily of the new System.Drawing.Font.
        //
        //   emSize:
        //     The em-size of the new font in the units specified by the unit parameter.
        //
        //   style:
        //     The System.Drawing.FontStyle of the new font.
        //
        //   unit:
        //     The System.Drawing.GraphicsUnit of the new font.
        //
        //   gdiCharSet:
        //     A System.Byte that specifies a GDI character set to use for this font.
        //
        //   gdiVerticalFont:
        //     A Boolean value indicating whether the new font is derived from a GDI vertical
        //     font.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.
        //
        //   T:System.ArgumentNullException:
        //     family is null
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) {
            this.Name = family?.Name;
            this.FontFamily = family;
            this.Size = emSize;
            this.Style = style;
            this.Unit = unit;
            this.GdiCharSet = gdiCharSet;
            this.GdiVerticalFont = gdiVerticalFont;
        }

        //
        // Summary:
        //     Gets a value that indicates whether this font has the italic style applied.
        //
        // Return result:
        //     true to indicate this font has the italic style applied; otherwise, false.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Italic { get => Style.HasFlag(FontStyle.Italic); }
        //
        // Summary:
        //     Gets a byte value that specifies the GDI character set that this System.Drawing.Font
        //     uses.
        //
        // Return result:
        //     A byte value that specifies the GDI character set that this System.Drawing.Font
        //     uses. The default is 1.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte GdiCharSet { get; }
        //
        // Summary:
        //     Gets a Boolean value that indicates whether this System.Drawing.Font is derived
        //     from a GDI vertical font.
        //
        // Return result:
        //     true if this System.Drawing.Font is derived from a GDI vertical font; otherwise,
        //     false.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GdiVerticalFont { get; }
        //
        // Summary:
        //     Gets the line spacing of this font.
        //
        // Return result:
        //     The line spacing, in pixels, of this font.
        [Browsable(false)]
        public int Height { get; }
        //
        // Summary:
        //     Gets a value indicating whether the font is a member of System.Drawing.SystemFonts.
        //
        // Return result:
        //     true if the font is a member of System.Drawing.SystemFonts; otherwise, false.
        //     The default is false.
        [Browsable(false)]
        public bool IsSystemFont { get; }
        //
        // Summary:
        //     Gets the face name of this System.Drawing.Font.
        //
        // Return result:
        //     A string representation of the face name of this System.Drawing.Font.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name { get; }
        //
        // Summary:
        //     Gets the name of the system font if the System.Drawing.Font.IsSystemFont property
        //     returns true.
        //
        // Return result:
        //     The name of the system font, if System.Drawing.Font.IsSystemFont returns true;
        //     otherwise, an empty string ("").
        [Browsable(false)]
        public string SystemFontName { get; }
        //
        // Summary:
        //     Gets the em-size of this System.Drawing.Font measured in the units specified
        //     by the System.Drawing.Font.Unit property.
        //
        // Return result:
        //     The em-size of this System.Drawing.Font.
        public float Size { get; }
        //
        // Summary:
        //     Gets the em-size, in points, of this System.Drawing.Font.
        //
        // Return result:
        //     The em-size, in points, of this System.Drawing.Font.
        [Browsable(false)]
        public float SizeInPoints { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this System.Drawing.Font specifies a horizontal
        //     line through the font.
        //
        // Return result:
        //     true if this System.Drawing.Font has a horizontal line through it; otherwise,
        //     false.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Strikeout { get; }
        //
        // Summary:
        //     Gets style information for this System.Drawing.Font.
        //
        // Return result:
        //     A System.Drawing.FontStyle enumeration that contains style information for this
        //     System.Drawing.Font.
        [Browsable(false)]
        public FontStyle Style { get; }
        //
        // Summary:
        //     Gets the System.Drawing.FontFamily associated with this System.Drawing.Font.
        //
        // Return result:
        //     The System.Drawing.FontFamily associated with this System.Drawing.Font.
        [Browsable(false)]
        public FontFamily FontFamily { get; }
        //
        // Summary:
        //     Gets the name of the font originally specified.
        //
        // Return result:
        //     The string representing the name of the font originally specified.
        [Browsable(false)]
        public string OriginalFontName { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this System.Drawing.Font is bold.
        //
        // Return result:
        //     true if this System.Drawing.Font is bold; otherwise, false.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Bold { get=> Style.HasFlag(FontStyle.Bold); }
        //
        // Summary:
        //     Gets the unit of measure for this System.Drawing.Font.
        //
        // Return result:
        //     A System.Drawing.GraphicsUnit that represents the unit of measure for this System.Drawing.Font.
        public GraphicsUnit Unit { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this System.Drawing.Font is underlined.
        //
        // Return result:
        //     true if this System.Drawing.Font is underlined; otherwise, false.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Underline { get => Style.HasFlag(FontStyle.Underline); }

        //
        // Summary:
        //     Creates a System.Drawing.Font from the specified Windows handle to a device context.
        //
        // Parameter:
        //   hdc:
        //     A handle to a device context.
        //
        // Return result:
        //     The System.Drawing.Font this method creates.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     The font for the specified device context is not a TrueType font.
        public static Font FromHdc(IntPtr hdc) { return new Font("Arial", 12); }
        //
        // Summary:
        //     Creates a System.Drawing.Font from the specified Windows handle.
        //
        // Parameter:
        //   hfont:
        //     A Windows handle to a GDI font.
        //
        // Return result:
        //     The System.Drawing.Font this method creates.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     hfont points to an object that is not a TrueType font.
        public static Font FromHfont(IntPtr hfont) { return new Font("Arial", 12); }
        //
        // Summary:
        //     Creates a System.Drawing.Font from the specified GDI logical font (LOGFONT) structure.
        //
        // Parameter:
        //   lf:
        //     An System.Object that represents the GDI LOGFONT structure from which to create
        //     the System.Drawing.Font.
        //
        //   hdc:
        //     A handle to a device context that contains additional information about the lf
        //     structure.
        //
        // Return result:
        //     The System.Drawing.Font that this method creates.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     The font is not a TrueType font.
        public static Font FromLogFont(object lf, IntPtr hdc) { return new Font("Arial", 12); }
        //
        // Summary:
        //     Creates a System.Drawing.Font from the specified GDI logical font (LOGFONT) structure.
        //
        // Parameter:
        //   lf:
        //     An System.Object that represents the GDI LOGFONT structure from which to create
        //     the System.Drawing.Font.
        //
        // Return result:
        //     The System.Drawing.Font that this method creates.
        public static Font FromLogFont(object lf) { return new Font("Arial", 12); }
        //
        // Summary:
        //     Creates an exact copy of this System.Drawing.Font.
        //
        // Return result:
        //     The System.Drawing.Font this method creates, cast as an System.Object.
        public object Clone() { return null; }
        //
        // Summary:
        //     Releases all resources used by this System.Drawing.Font.
        public void Dispose() { }
        //
        // Summary:
        //     Indicates whether the specified object is a System.Drawing.Font and has the same
        //     System.Drawing.Font.FontFamily, System.Drawing.Font.GdiVerticalFont, System.Drawing.Font.GdiCharSet,
        //     System.Drawing.Font.Style, System.Drawing.Font.Size, and System.Drawing.Font.Unit
        //     property values as this System.Drawing.Font.
        //
        // Parameter:
        //   obj:
        //     The object to test.
        //
        // Return result:
        //     true if the obj parameter is a System.Drawing.Font and has the same System.Drawing.Font.FontFamily,
        //     System.Drawing.Font.GdiVerticalFont, System.Drawing.Font.GdiCharSet, System.Drawing.Font.Style,
        //     System.Drawing.Font.Size, and System.Drawing.Font.Unit property values as this
        //     System.Drawing.Font; otherwise, false.
        public override bool Equals(object obj) { return Equals(obj as Font); }
        //
        // Summary:
        //     Gets the hash code for this System.Drawing.Font.
        //
        // Return result:
        //     The hash code for this System.Drawing.Font.
        public override int GetHashCode() { return base.GetHashCode(); }
        //
        // Summary:
        //     Returns the height, in pixels, of this System.Drawing.Font when drawn to a device
        //     with the specified vertical resolution.
        //
        // Parameter:
        //   dpi:
        //     The vertical resolution, in dots per inch, used to calculate the height of the
        //     font.
        //
        // Return result:
        //     The height, in pixels, of this System.Drawing.Font.
        public float GetHeight(float dpi) { return Height; }
        //
        // Summary:
        //     Returns the line spacing, in the current unit of a specified System.Drawing.Graphics,
        //     of this font.
        //
        // Parameter:
        //   graphics:
        //     A System.Drawing.Graphics that holds the vertical resolution, in dots per inch,
        //     of the display device as well as settings for page unit and page scale.
        //
        // Return result:
        //     The line spacing, in pixels, of this font.
        //
        // Exception:
        //   T:System.ArgumentNullException:
        //     graphics is null.
        public float GetHeight(Graphics graphics) { return Height; }
        //
        // Summary:
        //     Returns the line spacing, in pixels, of this font.
        //
        // Return result:
        //     The line spacing, in pixels, of this font.
        public float GetHeight() { return Height; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           
        }

        //
        // Summary:
        //     Returns a handle to this System.Drawing.Font.
        //
        // Return result:
        //     A Windows handle to this System.Drawing.Font.
        //
        // Exception:
        //   T:System.ComponentModel.Win32Exception:
        //     The operation was unsuccessful.
        public IntPtr ToHfont() { return IntPtr.Zero; }
        //
        // Summary:
        //     Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.
        //
        // Parameter:
        //   logFont:
        //     An System.Object to represent the LOGFONT structure that this method creates.
        //
        //   graphics:
        //     A System.Drawing.Graphics that provides additional information for the LOGFONT
        //     structure.
        //
        // Exception:
        //   T:System.ArgumentNullException:
        //     graphics is null.
        public void ToLogFont(object logFont, Graphics graphics) { }
        //
        // Summary:
        //     Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.
        //
        // Parameter:
        //   logFont:
        //     An System.Object to represent the LOGFONT structure that this method creates.
        public void ToLogFont(object logFont) { }
        //
        // Summary:
        //     Returns a human-readable string representation of this System.Drawing.Font.
        //
        // Return result:
        //     A string that represents this System.Drawing.Font.
        public override string ToString() { return "Font"; }
    }
}