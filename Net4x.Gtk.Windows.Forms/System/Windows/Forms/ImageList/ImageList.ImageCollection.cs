// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {

        /// <summary>
        /// Class ImageCollection. This class cannot be inherited.
        /// Implements the <see cref="IList" />
        /// </summary>
        /// <seealso cref="IList" />
        public sealed partial class ImageCollection : IList
        {
            /// <summary>
            /// The image information collection
            /// </summary>
            private readonly List<ImageInfo> _imageInfoCollection = new List<ImageInfo>();

            // Indicates whether images are added in a batch.
            /// <summary>
            /// The is batch add
            /// </summary>
            private bool _isBatchAdd;

            /// <summary>
            /// The last accessed index
            /// </summary>
            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int _lastAccessedIndex = -1;

            /// <summary>
            /// The owner
            /// </summary>
            private readonly ImageList _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageCollection"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            internal ImageCollection(ImageList owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
            /// </summary>
            /// <value>The count.</value>
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
            /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
            public bool Empty => Count == 0;

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public bool IsReadOnly => false;

            /// <summary>
            /// Returns the keys in the image list - images without keys return String.Empty.
            /// </summary>
            /// <value>The keys.</value>
            public StringCollection Keys
            {
                get
                {
                    // pass back a copy of the current state.
                    StringCollection keysCollection = new StringCollection();

                    for (int i = 0; i < _imageInfoCollection.Count; i++)
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

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.
            /// </summary>
            /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
            bool IList.IsFixedSize => false;

            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
            /// </summary>
            /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
            bool ICollection.IsSynchronized => false;

            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
            /// </summary>
            /// <value>The synchronize root.</value>
            object ICollection.SyncRoot => this;

            /// <summary>
            /// Gets or sets the <see cref="Image"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>Image.</returns>
            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image this[int index]
            {
                get
                {
                    return _owner._originals[index]._image as Image;
                }
                set
                {
                    if ((_imageInfoCollection.Count > index))
                    {
                        Bitmap bitmap = value as Bitmap;
                        _owner._originals[index] = new Original(bitmap.Clone(), OriginalOptions.OwnsImage);
                    }
                }
            }

            /// <summary>
            /// Retrieves the child control with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Nullable{Image}.</returns>
            public Image? this[string key]
            {
                get
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }
                    int index = IndexOfKey(key);
                    if (!IsValidIndex(index))
                    {
                        return null;
                    }

                    return this[index];
                }
            }

            /// <summary>
            /// Gets or sets the System.Nullable&lt;object&gt; at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>System.Nullable.</returns>
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
            /// <param name="key">The key.</param>
            /// <param name="image">The image.</param>
            public void Add(string key, Image image)
            {
                // Store off the name.
                ImageInfo imageInfo = new ImageInfo()
                {
                    Name = key
                };
                // Add the image to the IList
                Original original = new Original(image, OriginalOptions.Default);
                Add(original, imageInfo);
            }

            /// <summary>
            /// Adds an icon to the end of the image list with a key accessor.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="icon">The icon.</param>
            public void Add(string key, Icon icon)
            {
                // Store off the name.
                ImageInfo imageInfo = new ImageInfo()
                {
                    Name = key
                };

                // Add the image to the IList
                Original original = new Original(icon, OriginalOptions.Default);
                Add(original, imageInfo);
            }

            /// <summary>
            /// Adds the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public void Add(Icon value)
            {
                // Don't clone it now is a breaking change, so we have to keep track of this specific icon and dispose that
                Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null);
            }

            /// <summary>
            /// Add the given image to the ImageList.
            /// </summary>
            /// <param name="value">The value.</param>
            public void Add(Image value)
            {

                Original original = new Original(value, OriginalOptions.Default);
                Add(original, null);
            }

            /// <summary>
            /// Add the given image to the ImageList, using the given color
            /// to generate the mask. The number of images to add is inferred from
            /// the width of the given image.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="transparentColor">Color of the transparent.</param>
            /// <returns>System.Int32.</returns>
            public int Add(Image value, Color transparentColor)
            {

                Original original = new Original(value, OriginalOptions.CustomTransparentColor, transparentColor);

                return Add(original, null);
            }

            /// <summary>
            /// Adds the range.
            /// </summary>
            /// <param name="images">The images.</param>
            public void AddRange(params Image[] images)
            {
                foreach (Image image in images)
                {
                    Add(image);
                }
            }

            /// <summary>
            /// Add an image strip the given image to the ImageList. A strip is a single Image
            /// which is treated as multiple images arranged side-by-side.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>System.Int32.</returns>
            public int AddStrip(Image value)
            {
                int nImages = value.Width / _owner.ImageSize.Width;

                Original original = new Original(value, OriginalOptions.ImageStrip, nImages);

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
            /// <param name="image">The image.</param>
            /// <returns><c>true</c> if [contains] [the specified image]; otherwise, <c>false</c>.</returns>
            /// <exception cref="System.NotSupportedException"></exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool Contains(Image image) => throw new NotSupportedException();

            /// <summary>
            /// Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
            public bool ContainsKey(string key) => IsValidIndex(IndexOfKey(key));

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
            public IEnumerator GetEnumerator()
            {
                Image[] images = new Image[Count];
                for (int i = 0; i < images.Length; ++i)
                {
                    images[i] = _owner.GetBitmap(i);
                }

                return images.GetEnumerator();
            }

            /// <summary>
            /// Indexes the of.
            /// </summary>
            /// <param name="image">The image.</param>
            /// <returns>System.Int32.</returns>
            /// <exception cref="System.NotSupportedException"></exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int IndexOf(Image image) => throw new NotSupportedException();

            /// <summary>
            /// The zero-based index of the first occurrence of value within the entire CollectionBase,
            /// if found; otherwise, -1.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Int32.</returns>
            public int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    // We don't support empty or null keys.
                    return -1;
                }

                return _imageInfoCollection.FindIndex(o => o.Name == key);
            }

            /// <summary>
            /// Removes the specified image.
            /// </summary>
            /// <param name="image">The image.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Image image) => throw new NotSupportedException();

            /// <summary>
            /// Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            public void RemoveAt(int index)
            {
                if (_imageInfoCollection is null)
                    return;
                if (index >= 0 && index < _imageInfoCollection.Count)
                {
                    _imageInfoCollection.RemoveAt(index);
                    _owner._originals.RemoveAt(index);
                    _owner.OnChangeHandle(EventArgs.Empty);
                }
            }

            /// <summary>
            /// Removes the child control with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            public void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            /// Please make sure to copy the related images into the directoryResources下， keep the file name consistent withname一致
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="name">The name.</param>
            public void SetKeyName(int index, string name)
            {
                int len = _imageInfoCollection.Count;
                if (len > index)
                {
                    _imageInfoCollection[index].Name = name;
                    _owner._originals[index] = new Original(_owner.GetOriginalImage(name), OriginalOptions.OwnsImage);
                }
                else
                {
                    for (int i = len; i < index + 1; i++)
                    {
                        this.Add(name, _owner.GetOriginalImage(name));
                    }
                }
            }

            /// <summary>
            /// Resets the keys.
            /// </summary>
            internal void ResetKeys()
            {
                _imageInfoCollection.Clear();

                for (int i = 0; i < Count; i++)
                {
                    _imageInfoCollection.Add(new ImageInfo());
                }
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
            /// </summary>
            /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"></see>.</param>
            /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.</returns>
            int IList.Add(object? value)
            {
                Add((Image)value);

                return Count - 1;
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.
            /// </summary>
            /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"></see>.</param>
            /// <returns>true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.</returns>
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

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="dest">The dest.</param>
            /// <param name="index">The index.</param>
            void ICollection.CopyTo(Array dest, int index)
            {
                for (int i = 0; i < Count; ++i)
                {
                    dest.SetValue(_owner.GetBitmap(i), index++);
                }
            }

            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.
            /// </summary>
            /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"></see>.</param>
            /// <returns>The index of <paramref name="value">value</paramref> if found in the list; otherwise, -1.</returns>
            int IList.IndexOf(object? value)
            {
                if (value is Image image)
                {
                    return IndexOf(image);

                }

                return -1;
            }

            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which value should be inserted.</param>
            /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList"></see>.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            void IList.Insert(int index, object? value) => throw new NotSupportedException();

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.
            /// </summary>
            /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"></see>.</param>
            void IList.Remove(object? value)
            {
                if (value is Image image)
                {
                    Remove(image);

                    _owner.OnChangeHandle(EventArgs.Empty);
                }
            }

            /// <summary>
            /// Adds the specified original.
            /// </summary>
            /// <param name="original">The original.</param>
            /// <param name="imageInfo">The image information.</param>
            /// <returns>System.Int32.</returns>
            private int Add(Original original, ImageInfo? imageInfo)
            {
                _imageInfoCollection.Add(imageInfo ?? new ImageInfo());
                _owner._originals.Add(original);
                _owner.OnChangeHandle(EventArgs.Empty);

                return _imageInfoCollection.Count - 1;
            }

            /// <summary>
            /// Determines if the index is valid for the collection.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns><c>true</c> if [is valid index] [the specified index]; otherwise, <c>false</c>.</returns>
            private bool IsValidIndex(int index) => index >= 0 && index < Count;

        }
    }
}
