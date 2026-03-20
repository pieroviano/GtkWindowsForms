using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
/// Displays a single column header in a
/// <see cref="T:System.Windows.Forms.ListView" /> control.
/// </summary>
[DefaultProperty("Text")]
public class ColumnHeader : Component, ICloneable
{
    internal int _index;

    internal string _name;

    internal string _text;

    internal int _width = 120;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnHeader" /> class.
    /// </summary>
    public ColumnHeader()
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnHeader" /> class.
    /// </summary>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    public ColumnHeader(int imageIndex)
    {
        ImageIndex = imageIndex;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnHeader" /> class.
    /// </summary>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    public ColumnHeader(string imageKey)
    {
        ImageKey = imageKey;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnHeader" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    public ColumnHeader(string text, int width)
    {
        Text = text;
        Width = width;
    }

    /// <summary>
    /// Gets or sets the display index.
    /// </summary>
    /// <value>
    /// The display index.
    /// </value>
    [Localizable(true)]
    public int DisplayIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the image.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    [DefaultValue(-1)]
    public int ImageIndex
    {
        get;
        set;
    } = -1;

    /// <summary>
    /// Gets or sets the image key.
    /// </summary>
    /// <value>
    /// The image key.
    /// </value>
    public string ImageKey
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the image list.
    /// </summary>
    /// <value>
    /// The image list.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    [Browsable(false)]
    public ImageList ImageList
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    [Browsable(false)]
    public int Index
    {
        get
        {
            return _index;
        }
    }

    /// <summary>
    /// Gets the ListView.
    /// </summary>
    /// <value>
    /// The ListView.
    /// </value>
    [Browsable(false)]
    public ListView ListView
    {
        [CompilerGenerated]
        get;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [Browsable(false)]
    public string Name
    {
        get;

        set;
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    [Localizable(false)]
    [Bindable(true)]
    [DefaultValue(null)]
    public object Tag
    {
        get;

        set;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    [Localizable(true)]
    public string Text
    {
        get;

        set;
    }

    /// <summary>
    /// Gets or sets the text align.
    /// </summary>
    /// <value>
    /// The text align.
    /// </value>
    [Localizable(true)]
    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment TextAlign
    {
        get;

        set;
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    [Localizable(true)]
    [DefaultValue(60)]
    public int Width
    {
        get;

        set;
    } = 100;

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
        return null;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.ComponentModel.Component">
    /// </see> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {

    }

}