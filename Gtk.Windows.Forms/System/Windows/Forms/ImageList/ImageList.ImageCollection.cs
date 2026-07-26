// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;

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
    /// Class ImageCollection. This class cannot be inherited. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    public sealed partial class ImageCollection : IList
    {
        private readonly List<ImageInfo> _imageInfoCollection = new();

        // Indicates whether images are added in a batch.
        private readonly bool _isBatchAdd;

        /// A caching mechanism for key accessor
        /// We use an index here rather than control so that we don't have lifetime
        /// issues by holding on to extra references.
        private readonly int _lastAccessedIndex = -1;

        private readonly ImageList _owner;

        internal ImageCollection(ImageList owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Browsable(false)]
        public int Count
        {
            get
            {
                return _imageInfoCollection.Count;
            }
        }

        /// <summary>
        /// Determines if the ImageList has any images, without forcing a handle creation.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if empty; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        public bool Empty => Count == 0;

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> is read-only.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is read only; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns the keys in the image list - images without keys return String.Empty.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public StringCollection Keys
        {
            get
            {
                // pass back a copy of the current state.
                var keysCollection = new StringCollection();

                for (var i = 0; i < _imageInfoCollection.Count; i++)
                {
                    if ((_imageInfoCollection[i] is ImageInfo image) && (image.Name != null) && (image.Name.Length != 0))
                    {
                        keysCollection.Add(image.Name);
                    }
                    else
                    {
                        keysCollection.Add(string.Empty);
                    }
                }

                return keysCollection;
            }
        }

        bool IList.IsFixedSize => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets or sets the
        /// <see cref="T:System.Drawing.Image" /> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// Image.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image? this[int index]
        {
            get
            {
                return _owner._originals?[index]._image as Image;
            }
            set
            {
                if ((_imageInfoCollection.Count > index))
                {
                    var bitmap = value as Bitmap;
                    _owner._originals?[index] = new Original(bitmap?.Clone(), OriginalOptions.OwnsImage);
                }
            }
        }

        /// <summary>
        /// Retrieves the child control with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// System.Nullable{Image}.
        /// </returns>
        public Image? this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }
                var index = IndexOfKey(key);
                if (!IsValidIndex(index))
                {
                    return null;
                }

                return this[index];
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set
            {

            }
        }

        /// <summary>
        /// Adds an image to the end of the image list with a key accessor.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        public void Add(string key, Image? image)
        {
            // Store off the name.
            var imageInfo = new ImageInfo()
            {
                Name = key
            };
            // Add the image to the IList
            var original = new Original(image, OriginalOptions.Default);
            Add(original, imageInfo);
        }

        /// <summary>
        /// Adds an icon to the end of the image list with a key accessor.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public void Add(string key, Icon icon)
        {
            // Store off the name.
            var imageInfo = new ImageInfo()
            {
                Name = key
            };

            // Add the image to the IList
            var original = new Original(icon, OriginalOptions.Default);
            Add(original, imageInfo);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(Icon value)
        {
            // Don't clone it now is a breaking change, so we have to keep track of this specific icon and dispose that
            Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null);
        }

        /// <summary>
        /// Add the given image to the ImageList.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(Image? value)
        {

            var original = new Original(value, OriginalOptions.Default);
            Add(original, null);
        }

        /// <summary>
        /// Add the given image to the ImageList, using the given color
        /// to generate the mask. The number of images to add is inferred from
        /// the width of the given image.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="transparentColor">
        /// Color of the transparent.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int Add(Image value, Color transparentColor)
        {

            var original = new Original(value, OriginalOptions.CustomTransparentColor, transparentColor);

            return Add(original, null);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="images">
        /// The images.
        /// </param>
        public void AddRange(params Image[] images)
        {
            foreach (var image in images)
            {
                Add(image);
            }
        }

        /// <summary>
        /// Add an image strip the given image to the ImageList. A strip is a single Image
        /// which is treated as multiple images arranged side-by-side.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int AddStrip(Image value)
        {
            var nImages = value.Width / _owner.ImageSize.Width;

            var original = new Original(value, OriginalOptions.ImageStrip, nImages);

            return Add(original, null);
        }

        /// <summary>
        /// Remove all images and masks from the ImageList.
        /// </summary>
        public void Clear()
        {
            _owner._originals?.Clear();
            _imageInfoCollection.Clear();
            _owner.OnChangeHandle(EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified image]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Contains(Image image) => throw new NotSupportedException();

        /// <summary>
        /// Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if the specified key contains key; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public bool ContainsKey(string key) => IsValidIndex(IndexOfKey(key));

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            var images = new Image?[Count];
            for (var i = 0; i < images.Length; ++i)
            {
                images[i] = _owner.GetBitmap(i);
            }

            return images.GetEnumerator();
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int IndexOf(Image image) => throw new NotSupportedException();

        /// <summary>
        /// The zero-based index of the first occurrence of value within the entire CollectionBase,
        /// if found; otherwise, -1.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                // We don't support empty or null keys.
                return -1;
            }

            // WinForms matches image keys case-insensitively.
            return _imageInfoCollection.FindIndex(o => string.Equals(o.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Removes the specified image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Remove(Image image) => throw new NotSupportedException();

        /// <summary>
        /// Removes the
        /// <see cref="T:System.Collections.IList">
        /// </see> item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            if (_imageInfoCollection is null)
                return;
            if (index >= 0 && index < _imageInfoCollection.Count)
            {
                _imageInfoCollection.RemoveAt(index);
                _owner._originals?.RemoveAt(index);
                _owner.OnChangeHandle(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes the child control with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void RemoveByKey(string key)
        {
            var index = IndexOfKey(key);
            if (IsValidIndex(index))
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Please make sure to copy the related images into the directoryResources下， keep the file name consistent withname一致
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public void SetKeyName(int index, string name)
        {
            var len = _imageInfoCollection.Count;
            if (len > index)
            {
                _imageInfoCollection[index].Name = name;
                _owner._originals?[index] = new Original(_owner.GetOriginalImage(name), OriginalOptions.OwnsImage);
            }
            else
            {
                for (var i = len; i < index + 1; i++)
                {
                    this.Add(name, _owner.GetOriginalImage(name));
                }
            }
        }

        internal void ResetKeys()
        {
            _imageInfoCollection.Clear();

            for (var i = 0; i < Count; i++)
            {
                _imageInfoCollection.Add(new ImageInfo());
            }
        }

        int IList.Add(object? value)
        {
            Add((Image?)value);

            return Count - 1;
        }

        bool IList.Contains(object? value)
        {
            if (value is Image image)
            {
                return Contains(image);
            }
            else
            {
                return false;
            }
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            for (var i = 0; i < Count; ++i)
            {
                dest.SetValue(_owner.GetBitmap(i), index++);
            }
        }

        int IList.IndexOf(object? value)
        {
            if (value is Image image)
            {
                return IndexOf(image);

            }

            return -1;
        }

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        void IList.Remove(object? value)
        {
            if (value is Image image)
            {
                Remove(image);

                _owner.OnChangeHandle(EventArgs.Empty);
            }
        }

        private int Add(Original original, ImageInfo? imageInfo)
        {
            _imageInfoCollection.Add(imageInfo ?? new ImageInfo());
            _owner._originals?.Add(original);
            _owner.OnChangeHandle(EventArgs.Empty);

            return _imageInfoCollection.Count - 1;
        }

        /// <summary>
        /// Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index) => index >= 0 && index < Count;

    }
}