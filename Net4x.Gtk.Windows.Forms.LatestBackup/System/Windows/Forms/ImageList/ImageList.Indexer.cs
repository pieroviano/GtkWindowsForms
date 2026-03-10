// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
public sealed partial class ImageList
{
    /// <summary>
    /// This class is for classes that want to support both an ImageIndex
    /// and ImageKey. We want to toggle between using keys or indexes.
    /// Default is to use the integer index.
    /// </summary>
    internal class Indexer
    {
        internal const int DefaultIndex = -1;

        // Used by generally across the board to indicate unset image

        internal const string DefaultKey = "";

        // Used by TreeViewImageIndexConverter to show "(none)"
        internal const int NoneIndex = -2;

        private int _index = DefaultIndex;

        private string _key = DefaultKey;

        private bool _useIntegerIndex = true;

        public virtual int ActualIndex
        {
            get
            {
                if (_useIntegerIndex)
                {
                    return Index;
                }
                if (ImageList is null)
                {

                }
                else //  if (ImageList != null)
                {
                    return ImageList.Images.IndexOfKey(Key);
                }

                return DefaultIndex;
            }
        }

        public virtual ImageList? ImageList { get; set; }

        public virtual int Index
        {
            get => _index;
            set
            {
                _key = DefaultKey;
                _index = value;
                _useIntegerIndex = true;
            }
        }

        public virtual string Key
        {
            get => _key;
            set
            {
                _index = DefaultIndex;
                _key = value ?? DefaultKey;
                _useIntegerIndex = false;
            }
        }

    }
}
