using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays a single column header in a
    /// <see cref="T:System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultProperty("Text")]
    public class ColumnHeader : Component, ICloneable
    {
        internal int _index;

        internal string _name;

        internal string _text;

        internal int _width = 120;

        public ColumnHeader()
        {

        }

        public ColumnHeader(int imageIndex)
        {
            ImageIndex = imageIndex;
        }

        public ColumnHeader(string imageKey)
        {
            ImageKey = imageKey;
        }

        public ColumnHeader(string text, int width)
        {
            Text = text;
            Width = width;
        }

        [Localizable(true)]

        public int DisplayIndex { get; set; }

        [DefaultValue(-1)]

        public int ImageIndex { get; set; } = -1;

        public string ImageKey { get; set; }

        [Browsable(false)]
        public ImageList ImageList
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        [Browsable(false)]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        [Browsable(false)]
        public ListView ListView
        {
            [CompilerGenerated]
            get;
        }

        [Browsable(false)]

        public string Name
        {
            get;
            set;
        }

        [Localizable(false)]
        [Bindable(true)]

        [DefaultValue(null)]

        public object Tag
        {
            get;
            set;
        }

        [Localizable(true)]

        public string Text
        {
            get;
            set;
        }

        [Localizable(true)]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get;
            set;
        }

        [Localizable(true)]
        [DefaultValue(60)]
        public int Width
        {
            get;
            set;
        } = 100;

        public object Clone()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {

        }

    }
}
