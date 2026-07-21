using System.Drawing.Text;
using Gdk;

namespace System.Drawing;

/// <summary>
/// Defines a group of type faces having a similar basic design and certain variations in styles. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class FontFamily : MarshalByRefObject, IDisposable
{

    private static readonly Dictionary<string, string> fontLanguages = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new System.Drawing.FontFamily from the specified generic font family.
    /// </summary>
    /// <param name="genericFamily">
    /// The System.Drawing.Text.GenericFontFamilies from which to create the new System.Drawing.FontFamily.
    /// </param>
    public FontFamily(GenericFontFamilies genericFamily) : this(genericFamily.ToString(), null)
    {
    }

    /// <summary>
    /// Initializes a new System.Drawing.FontFamily with the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the new System.Drawing.FontFamily.
    /// </param>
    public FontFamily(string? name) : this(name, null) { }

    /// <summary>
    /// Initializes a new System.Drawing.FontFamily in the specified System.Drawing.Text.FontCollection
    /// with the specified name.
    /// </summary>
    /// <param name="name">
    /// A System.String that represents the name of the new System.Drawing.FontFamily.
    /// </param>
    /// <param name="fontCollection">
    /// The System.Drawing.Text.FontCollection that contains this System.Drawing.FontFamily.
    /// </param>
    public FontFamily(string? name, FontCollection? fontCollection)
    {
        if (name != null && fontLanguages.TryGetValue(name, out string enname))
            this.Name = enname;
        else
            this.Name = name;
    }

    static FontFamily()
    {
        if (fontLanguages.Count == 0)
        {
            // Keys are the native Chinese font names a caller may pass to Font/FontFamily; values are the
            // canonical family names GTK/Pango knows. These keys MUST stay in Chinese - they are runtime
            // lookup data, not UI text. (The generation pipeline translates Chinese literals, which both
            // emptied this table and collapsed the two distinct Microsoft JhengHei keys into a duplicate
            // that threw from the static ctor; kept correct here via patch.)
            fontLanguages.Add("宋体", "SimSun");
            fontLanguages.Add("黑体", "SimHei");
            fontLanguages.Add("微软雅黑", "Microsoft Yahei");
            fontLanguages.Add("微软正黑", "Microsoft JhengHei");
            fontLanguages.Add("微軟正黑體", "Microsoft JhengHei");
            fontLanguages.Add("楷体", "KaiTi");
            fontLanguages.Add("新宋体", "NSimSun");
            fontLanguages.Add("仿宋", "FangSong");
            fontLanguages.Add("標楷體", "BiauKai");
            fontLanguages.Add("新細明體", "PMingLiU");
            fontLanguages.Add("細明體", "MingLiU");
            //macos
            fontLanguages.Add("苹方", "PingFang SC");
            fontLanguages.Add("华文黑体", "STHeiti");
            fontLanguages.Add("华文楷体", "STKaiti");
            fontLanguages.Add("华文宋体", "STSong");
            fontLanguages.Add("华文仿宋", "STFangsong");
            fontLanguages.Add("华文中宋", "STZhongsong");
            fontLanguages.Add("华文琥珀", "STHupo");
            fontLanguages.Add("华文新魏", "STXinwei");
            fontLanguages.Add("华文隶书", "STLiti");
            fontLanguages.Add("华文行楷", "STXingkai");
            //open
            fontLanguages.Add("思源黑体", "Source Han Sans CN");
            fontLanguages.Add("思源宋体", "Source Han Serif SC");
            fontLanguages.Add("文泉驿微米黑", "WenQuanYi Micro Hei");
        }
    }

    /// <summary>
    /// Returns an array that contains all the System.Drawing.FontFamily objects associated
    /// with the current graphics context.
    /// </summary>
    /// <returns>
    /// An array of System.Drawing.FontFamily objects associated with the current graphics context.
    /// </returns>
    public static FontFamily[] Families { get => Array.ConvertAll(PangoHelper.ContextGet().Families, o => new FontFamily(o.Name)); }

    /// <summary>
    /// Gets a generic monospace System.Drawing.FontFamily.
    /// </summary>
    /// <returns>
    /// A System.Drawing.FontFamily that represents a generic monospace font.
    /// </returns>
    public static FontFamily GenericMonospace { get => new FontFamily(GenericFontFamilies.Monospace); }

    /// <summary>
    /// Gets a generic sans serif System.Drawing.FontFamily object.
    /// </summary>
    /// <returns>
    /// A System.Drawing.FontFamily object that represents a generic sans serif font.
    /// </returns>
    public static FontFamily GenericSansSerif { get => new FontFamily(GenericFontFamilies.SansSerif); }

    /// <summary>
    /// Gets a generic serif System.Drawing.FontFamily.
    /// </summary>
    /// <returns>
    /// A System.Drawing.FontFamily that represents a generic serif font.
    /// </returns>
    public static FontFamily GenericSerif { get => new FontFamily(GenericFontFamilies.Serif); }

    /// <summary>
    /// Gets the name of this System.Drawing.FontFamily.
    /// </summary>
    /// <returns>
    /// A System.String that represents the name of this System.Drawing.FontFamily.
    /// </returns>
    public string? Name
    {
        get;
        set;
    }

    /// <summary>
    /// Releases all resources used by this System.Drawing.FontFamily.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// Indicates whether the specified object is a System.Drawing.FontFamily and is
    /// identical to this System.Drawing.FontFamily.
    /// </summary>
    /// <param name="obj">
    /// The object to test.
    /// </param>
    /// <returns>
    /// true if obj is a System.Drawing.FontFamily and is identical to this System.Drawing.FontFamily; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj == this)
        {
            return true;
        }

        if (obj is not FontFamily fontFamily)
        {
            return false;
        }
        return fontFamily.Name == this.Name;
    }
    /// <summary>
    /// Returns the cell ascent, in design units, of the System.Drawing.FontFamily of
    /// the specified style.
    /// </summary>
    /// <param name="style">
    /// A System.Drawing.FontStyle that contains style information for the font.
    /// </param>
    /// <returns>
    /// The cell ascent for this System.Drawing.FontFamily that uses the specified System.Drawing.FontStyle.
    /// </returns>
    public int GetCellAscent(FontStyle style) { return 0; }

    /// <summary>
    /// Returns the cell descent, in design units, of the System.Drawing.FontFamily of
    /// the specified style.
    /// </summary>
    /// <param name="style">
    /// A System.Drawing.FontStyle that contains style information for the font.
    /// </param>
    /// <returns>
    /// The cell descent metric for this System.Drawing.FontFamily that uses the specified System.Drawing.FontStyle.
    /// </returns>
    public int GetCellDescent(FontStyle style) { return 0; }

    /// <summary>
    /// Gets the height, in font design units, of the em square for the specified style.
    /// </summary>
    /// <param name="style">
    /// The System.Drawing.FontStyle for which to get the em height.
    /// </param>
    /// <returns>
    /// The height of the em square.
    /// </returns>
    public int GetEmHeight(FontStyle style) { return 0; }

    /// <summary>
    /// Returns an array that contains all the System.Drawing.FontFamily objects available
    /// for the specified graphics context.
    /// </summary>
    /// <param name="graphics">
    /// The System.Drawing.Graphics object from which to return System.Drawing.FontFamily objects.
    /// </param>
    /// <returns>
    /// An array of System.Drawing.FontFamily objects available for the specified System.Drawing.Graphics object.
    /// </returns>
    [Obsolete("Do not use method GetFamilies, use property Families instead")]
    public static FontFamily[] GetFamilies(Graphics graphics) { return new FontFamily[0]; }

    /// <summary>
    /// Gets a hash code for this System.Drawing.FontFamily.
    /// </summary>
    /// <returns>
    /// The hash code for this System.Drawing.FontFamily.
    /// </returns>
    public override int GetHashCode() { return (Name ?? string.Empty).GetHashCode(); }

    /// <summary>
    /// Returns the line spacing, in design units, of the System.Drawing.FontFamily of
    /// the specified style. The line spacing is the vertical distance between the base
    /// lines of two consecutive lines of text.
    /// </summary>
    /// <param name="style">
    /// The System.Drawing.FontStyle to apply.
    /// </param>
    /// <returns>
    /// The distance between two consecutive lines of text.
    /// </returns>
    public int GetLineSpacing(FontStyle style) { return 0; }

    /// <summary>
    /// Returns the name, in the specified language, of this System.Drawing.FontFamily.
    /// </summary>
    /// <param name="language">
    /// The language in which the name is returned.
    /// </param>
    /// <returns>
    /// A System.String that represents the name, in the specified language, of this System.Drawing.FontFamily.
    /// </returns>
    public string? GetName(int language) { return Name; }

    /// <summary>
    /// Indicates whether the specified System.Drawing.FontStyle enumeration is available.
    /// </summary>
    /// <param name="style">
    /// The System.Drawing.FontStyle to test.
    /// </param>
    /// <returns>
    /// true if the specified System.Drawing.FontStyle is available; otherwise, false.
    /// </returns>
    public bool IsStyleAvailable(FontStyle style) { return true; }

    /// <summary>
    /// Converts this System.Drawing.FontFamily to a human-readable string representation.
    /// </summary>
    /// <returns>
    /// The string that represents this System.Drawing.FontFamily.
    /// </returns>
    public override string? ToString()
    {
        return $"[{GetType().Name}: Name={Name}]";
    }
}