// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms
{
    /// <summary>
    /// A collection that stores
    /// <see cref="System.Windows.Forms.InputLanguage" /> objects.
    /// </summary>
    public class InputLanguageCollection : ReadOnlyCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of
        /// <see cref='InputLanguageCollection'/>
        /// containing any array of
        /// <see cref='InputLanguage'/> objects.
        /// </summary>
        internal InputLanguageCollection(InputLanguage[] value)
        {
            InnerList.AddRange(value);
        }

        /// <summary>
        /// Represents the entry at the specified index of the
        /// <see cref='InputLanguage'/> .
        /// </summary>
        public InputLanguage this[int index]
        {
            get
            {
                return ((InputLanguage)(InnerList[index]));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.InputLanguageCollection" /> contains the specified
        /// <see cref="System.Windows.Forms.InputLanguage" />
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.InputLanguage" /> is contained in the collection; otherwise, false.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.InputLanguage" /> to locate.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Contains(InputLanguage value)
        {
            return InnerList.Contains(value);
        }

        /// <summary>
        /// Copies the
        /// <see cref="System.Windows.Forms.InputLanguageCollection" /> values to a one-dimensional
        /// <see cref="System.Array" /> at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the values copied from
        /// <see cref="System.Windows.Forms.InputLanguageCollection" />
        /// </param>
        /// <param name="index">The index in
        /// <paramref name="array" /> where copying begins.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> specifies a multidimensional array.-or- The number of elements in the
        /// <see cref="System.Windows.Forms.InputLanguageCollection" /> is greater than the available space between the
        /// <paramref name="index" /> and the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" />
        /// is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than the lower bound of
        /// <paramref name="array" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void CopyTo(InputLanguage[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        /// <summary>
        /// Returns the index of an
        /// <see cref="System.Windows.Forms.InputLanguage" /> in the
        /// <see cref="System.Windows.Forms.InputLanguageCollection" />
        /// </summary>
        /// <returns>The index of the
        /// <see cref="System.Windows.Forms.InputLanguage" /> in the
        /// <see cref="System.Windows.Forms.InputLanguageCollection" />, if found; otherwise, -1.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.InputLanguage" /> to locate.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public int IndexOf(InputLanguage value)
        {
            return InnerList.IndexOf(value);
        }
    }
}
