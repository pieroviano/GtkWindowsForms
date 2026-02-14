
using Gdk;
using System.Drawing.Text;
using System.Linq;

namespace System.Drawing
{
    //
    // Summary:
    //     Defines a group of type faces having a similar basic design and certain variations
    //     in styles. This class cannot be inherited.
    public sealed class FontFamily : MarshalByRefObject, IDisposable
    {
        //
        // Summary:
        //     Initializes a new System.Drawing.FontFamily from the specified generic font family.
        //
        // Parameter:
        //   genericFamily:
        //     The System.Drawing.Text.GenericFontFamilies from which to create the new System.Drawing.FontFamily.
        public FontFamily(GenericFontFamilies genericFamily) : this(genericFamily.ToString(), null)
        {
        }
        //
        // Summary:
        //     Initializes a new System.Drawing.FontFamily with the specified name.
        //
        // Parameter:
        //   name:
        //     The name of the new System.Drawing.FontFamily.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     name is an empty string (""). -or- name specifies a font that is not installed
        //     on the computer running the application. -or- name specifies a font that is not
        //     a TrueType font.
        public FontFamily(string name) : this(name, null) { }
        //
        // Summary:
        //     Initializes a new System.Drawing.FontFamily in the specified System.Drawing.Text.FontCollection
        //     with the specified name.
        //
        // Parameter:
        //   name:
        //     A System.String that represents the name of the new System.Drawing.FontFamily.
        //
        //   fontCollection:
        //     The System.Drawing.Text.FontCollection that contains this System.Drawing.FontFamily.
        //
        // Exception:
        //   T:System.ArgumentException:
        //     name is an empty string (""). -or- name specifies a font that is not installed
        //     on the computer running the application. -or- name specifies a font that is not
        //     a TrueType font.
        public FontFamily(string name, FontCollection fontCollection) {
            this.Name = name;
        }


        //
        // Summary:
        //     Gets a generic sans serif System.Drawing.FontFamily object.
        //
        // Return result:
        //     A System.Drawing.FontFamily object that represents a generic sans serif font.
        public static FontFamily GenericSansSerif { get => new FontFamily(GenericFontFamilies.SansSerif); }
        //
        // Summary:
        //     Gets a generic monospace System.Drawing.FontFamily.
        //
        // Return result:
        //     A System.Drawing.FontFamily that represents a generic monospace font.
        public static FontFamily GenericMonospace { get => new FontFamily(GenericFontFamilies.Monospace); }
        //
        // Summary:
        //     Returns an array that contains all the System.Drawing.FontFamily objects associated
        //     with the current graphics context.
        //
        // Return result:
        //     An array of System.Drawing.FontFamily objects associated with the current graphics
        //     context.
        public static FontFamily[] Families { get => Array.ConvertAll(PangoHelper.ContextGet().Families, o => new FontFamily(o.Name)); }

        //
        // Summary:
        //     Gets a generic serif System.Drawing.FontFamily.
        //
        // Return result:
        //     A System.Drawing.FontFamily that represents a generic serif font.
        public static FontFamily GenericSerif { get => new FontFamily(GenericFontFamilies.Serif); }
        //
        // Summary:
        //     Gets the name of this System.Drawing.FontFamily.
        //
        // Return result:
        //     A System.String that represents the name of this System.Drawing.FontFamily.
        public string Name { get; set; }

        //
        // Summary:
        //     Returns an array that contains all the System.Drawing.FontFamily objects available
        //     for the specified graphics context.
        //
        // Parameter:
        //   graphics:
        //     The System.Drawing.Graphics object from which to return System.Drawing.FontFamily
        //     objects.
        //
        // Return result:
        //     An array of System.Drawing.FontFamily objects available for the specified System.Drawing.Graphics
        //     object.
        //
        // Exception:
        //   T:System.ArgumentNullException:
        //     graphics is null.
        [Obsolete("Do not use method GetFamilies, use property Families instead")]
        public static FontFamily[] GetFamilies(Graphics graphics) { return new FontFamily[0]; }
        //
        // Summary:
        //     Releases all resources used by this System.Drawing.FontFamily.
        public void Dispose() { }
        //
        // Summary:
        //     Indicates whether the specified object is a System.Drawing.FontFamily and is
        //     identical to this System.Drawing.FontFamily.
        //
        // Parameter:
        //   obj:
        //     The object to test.
        //
        // Return result:
        //     true if obj is a System.Drawing.FontFamily and is identical to this System.Drawing.FontFamily;
        //     otherwise, false.
        public override bool Equals(object obj) { return Equals(obj as FontFamily); }
        //
        // Summary:
        //     Returns the cell ascent, in design units, of the System.Drawing.FontFamily of
        //     the specified style.
        //
        // Parameter:
        //   style:
        //     A System.Drawing.FontStyle that contains style information for the font.
        //
        // Return result:
        //     The cell ascent for this System.Drawing.FontFamily that uses the specified System.Drawing.FontStyle.
        public int GetCellAscent(FontStyle style) { return 0; }
        //
        // Summary:
        //     Returns the cell descent, in design units, of the System.Drawing.FontFamily of
        //     the specified style.
        //
        // Parameter:
        //   style:
        //     A System.Drawing.FontStyle that contains style information for the font.
        //
        // Return result:
        //     The cell descent metric for this System.Drawing.FontFamily that uses the specified
        //     System.Drawing.FontStyle.
        public int GetCellDescent(FontStyle style) { return 0; }
        //
        // Summary:
        //     Gets the height, in font design units, of the em square for the specified style.
        //
        // Parameter:
        //   style:
        //     The System.Drawing.FontStyle for which to get the em height.
        //
        // Return result:
        //     The height of the em square.
        public int GetEmHeight(FontStyle style) { return 0; }
        //
        // Summary:
        //     Gets a hash code for this System.Drawing.FontFamily.
        //
        // Return result:
        //     The hash code for this System.Drawing.FontFamily.
        public override int GetHashCode() { return Name.GetHashCode(); }
        //
        // Summary:
        //     Returns the line spacing, in design units, of the System.Drawing.FontFamily of
        //     the specified style. The line spacing is the vertical distance between the base
        //     lines of two consecutive lines of text.
        //
        // Parameter:
        //   style:
        //     The System.Drawing.FontStyle to apply.
        //
        // Return result:
        //     The distance between two consecutive lines of text.
        public int GetLineSpacing(FontStyle style) { return 0; }
        //
        // Summary:
        //     Returns the name, in the specified language, of this System.Drawing.FontFamily.
        //
        // Parameter:
        //   language:
        //     The language in which the name is returned.
        //
        // Return result:
        //     A System.String that represents the name, in the specified language, of this
        //     System.Drawing.FontFamily.
        public string GetName(int language) { return Name; }
        //
        // Summary:
        //     Indicates whether the specified System.Drawing.FontStyle enumeration is available.
        //
        // Parameter:
        //   style:
        //     The System.Drawing.FontStyle to test.
        //
        // Return result:
        //     true if the specified System.Drawing.FontStyle is available; otherwise, false.
        public bool IsStyleAvailable(FontStyle style) { return false; }
        //
        // Summary:
        //     Converts this System.Drawing.FontFamily to a human-readable string representation.
        //
        // Return result:
        //     The string that represents this System.Drawing.FontFamily.
        public override string ToString()
        {
            return Name;
        }
    }
}