using Gdk;
using System.Drawing.Text;
using System.Linq;

namespace System.Drawing
{

    /// <summary>
    /// Defines a group of type faces having a similar basic design and certain variations
    /// in styles. This class cannot be inherited.
    /// </summary>
    public sealed class FontFamily : MarshalByRefObject, IDisposable
    {

        /// <summary>
        /// Initializes a new System.Drawing.FontFamily from the specified generic font family.
        /// </summary>
        /// <param name="genericFamily">The System.Drawing.Text.GenericFontFamilies from which to create the new System.Drawing.FontFamily.
        /// </param>
        public FontFamily(GenericFontFamilies genericFamily) : this(genericFamily.ToString(), null)
        {
        }

        /// <summary>
        /// Initializes a new
        /// <see cref="System.Drawing.FontFamily" /> with the specified name.
        /// </summary>
        /// <param name="name">The name of the new
        /// <see cref="System.Drawing.FontFamily" />
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="name" /> is an empty string ("").-or-
        /// <paramref name="name" /> specifies a font that is not installed on the computer running the application.-or-
        /// <paramref name="name" /> specifies a font that is not a TrueType font.
        /// </exception>
        public FontFamily(string name) : this(name, null) { }

        /// <summary>
        /// Initializes a new
        /// <see cref="System.Drawing.FontFamily" /> in the specified
        /// <see cref="System.Drawing.Text.FontCollection" /> with the specified name.
        /// </summary>
        /// <param name="name">
        /// A
        /// <see cref="System.String" /> that represents the name of the new
        /// <see cref="System.Drawing.FontFamily" />
        /// </param>
        /// <param name="fontCollection">
        /// The
        /// <see cref="System.Drawing.Text.FontCollection" /> that contains this
        /// <see cref="System.Drawing.FontFamily" />
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="name" /> is an empty string ("").-or-
        /// <paramref name="name" /> specifies a font that is not installed on the computer running the application.-or-
        /// <paramref name="name" /> specifies a font that is not a TrueType font.
        /// </exception>
        public FontFamily(string name, FontCollection fontCollection)
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns an array that contains all the
        /// <see cref="System.Drawing.FontFamily" /> objects associated with the current graphics context.
        /// </summary>
        /// <returns>An array of
        /// <see cref="System.Drawing.FontFamily" /> objects associated with the current graphics context.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public static FontFamily[] Families { get => Array.ConvertAll(PangoHelper.ContextGet().Families, o => new FontFamily(o.Name)); }

        /// <summary>
        /// Gets a generic monospace
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.FontFamily" /> that represents a generic monospace font.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public static FontFamily GenericMonospace { get => new FontFamily(GenericFontFamilies.Monospace); }

        /// <summary>
        /// Gets a generic sans serif
        /// <see cref="System.Drawing.FontFamily" /> object.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.FontFamily" /> object that represents a generic sans serif font.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public static FontFamily GenericSansSerif { get => new FontFamily(GenericFontFamilies.SansSerif); }

        /// <summary>
        /// Gets a generic serif
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.FontFamily" /> that represents a generic serif font.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public static FontFamily GenericSerif { get => new FontFamily(GenericFontFamilies.Serif); }

        /// <summary>
        /// Gets the name of this
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.String" /> that represents the name of this
        /// <see cref="System.Drawing.FontFamily" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public string Name { get; set; }

        /// <summary>
        /// Releases all resources used by this
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Dispose() { }

        /// <summary>
        /// Indicates whether the specified object is a
        /// <see cref="System.Drawing.FontFamily" /> and is identical to this
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>
        /// true if
        /// <paramref name="obj" /> is a
        /// <see cref="System.Drawing.FontFamily" /> and is identical to this
        /// <see cref="System.Drawing.FontFamily" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool Equals(object obj) { return Equals(obj as FontFamily); }

        /// <summary>
        /// Returns the cell ascent, in design units, of the
        /// <see cref="System.Drawing.FontFamily" /> of the specified style.
        /// </summary>
        /// <returns>
        /// The cell ascent for this
        /// <see cref="System.Drawing.FontFamily" /> that uses the specified
        /// <see cref="System.Drawing.FontStyle" />
        /// </returns>
        /// <param name="style">A
        /// <see cref="System.Drawing.FontStyle" /> that contains style information for the font.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int GetCellAscent(FontStyle style) { return 0; }

        /// <summary>
        /// Returns the cell descent, in design units, of the
        /// <see cref="System.Drawing.FontFamily" /> of the specified style.
        /// </summary>
        /// <returns>
        /// The cell descent metric for this
        /// <see cref="System.Drawing.FontFamily" /> that uses the specified
        /// <see cref="System.Drawing.FontStyle" />
        /// </returns>
        /// <param name="style">A
        /// <see cref="System.Drawing.FontStyle" /> that contains style information for the font.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int GetCellDescent(FontStyle style) { return 0; }

        /// <summary>
        /// Gets the height, in font design units, of the em square for the specified style.
        /// </summary>
        /// <returns>The height of the em square.
        /// </returns>
        /// <param name="style">The
        /// <see cref="System.Drawing.FontStyle" /> for which to get the em height.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int GetEmHeight(FontStyle style) { return 0; }

        /// <summary>
        /// Returns an array that contains all the
        /// <see cref="System.Drawing.FontFamily" /> objects available for the specified graphics context.
        /// </summary>
        /// <returns>
        /// An array of
        /// <see cref="System.Drawing.FontFamily" /> objects available for the specified
        /// <see cref="System.Drawing.Graphics" /> object.
        /// </returns>
        /// <param name="graphics">
        /// The
        /// <see cref="System.Drawing.Graphics" /> object from which to return
        /// <see cref="System.Drawing.FontFamily" /> objects.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="graphics " />isnull.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        [Obsolete("Do not use method GetFamilies, use property Families instead")]
        public static FontFamily[] GetFamilies(Graphics graphics) { return new FontFamily[0]; }

        /// <summary>
        /// Gets a hash code for this
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>The hash code for this
        /// <see cref="System.Drawing.FontFamily" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override int GetHashCode() { return Name.GetHashCode(); }

        /// <summary>
        /// Returns the line spacing, in design units, of the
        /// <see cref="System.Drawing.FontFamily" /> of the specified style. The line spacing is the vertical distance between the base lines of two consecutive lines of text.
        /// </summary>
        /// <returns>The distance between two consecutive lines of text.
        /// </returns>
        /// <param name="style">The
        /// <see cref="System.Drawing.FontStyle" /> to apply.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int GetLineSpacing(FontStyle style) { return 0; }

        /// <summary>
        /// Returns the name, in the specified language, of this
        /// <see cref="System.Drawing.FontFamily" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.String" /> that represents the name, in the specified language, of this
        /// <see cref="System.Drawing.FontFamily" />
        /// </returns>
        /// <param name="language">The language in which the name is returned.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public string GetName(int language) { return Name; }

        /// <summary>
        /// Indicates whether the specified
        /// <see cref="System.Drawing.FontStyle" /> enumeration is available.
        /// </summary>
        /// <returns>true if the specified
        /// <see cref="System.Drawing.FontStyle" /> is available; otherwise, false.
        /// </returns>
        /// <param name="style">The
        /// <see cref="System.Drawing.FontStyle" /> to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsStyleAvailable(FontStyle style) { return false; }

        /// <summary>
        /// Converts this
        /// <see cref="System.Drawing.FontFamily" /> to a human-readable string representation.
        /// </summary>
        /// <returns>The string that represents this
        /// <see cref="System.Drawing.FontFamily" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override string ToString()
        {
            return Name;
        }
    }
}
