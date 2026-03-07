using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides the base functionality for creating data-related collections in the
    /// <see cref="System.Windows.Forms" /> namespace.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class BaseCollection : MarshalByRefObject, ICollection, IEnumerable
    {
        public BaseCollection()
        {

        }
        /// <summary>
        /// Gets the total number of elements in the collection.
        /// </summary>
        /// <returns>The total number of elements in the collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual int Count => List!.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>This property is always false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the
        /// <see cref="System.Collections.ICollection" /> is synchronized.
        /// </summary>
        /// <returns>This property always returns false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="System.Windows.Forms.BaseCollection" />
        /// </summary>
        /// <returns>An object that can be used to synchronize the
        /// <see cref="System.Windows.Forms.BaseCollection" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object SyncRoot => this;
        /// <summary>
        /// Gets the list of elements contained in the
        /// <see cref="System.Windows.Forms.BaseCollection" /> instance.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.ArrayList" /> containing the elements of the collection. This property returns null unless overridden in a derived class.
        /// </returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ArrayList? List => null;
        /// <summary>
        /// Copies all the elements of the current one-dimensional
        /// <see cref="System.Array" /> to the specified one-dimensional
        /// <see cref="System.Array" /> starting at the specified destination
        /// <see cref="System.Array" /> index.
        /// </summary>
        /// <param name="ar">The one-dimensional
        /// <see cref="System.Array" /> that is the destination of the elements copied from the current Array.
        /// </param>
        /// <param name="index">The zero-based relative index in
        /// <paramref name="ar" /> at which copying begins.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void CopyTo(Array ar, int index) => List!.CopyTo(ar, index);

        /// <summary>
        /// Gets the object that enables iterating through the members of the collection.
        /// </summary>
        /// <returns>An object that implements the
        /// <see cref="System.Collections.IEnumerator" /> interface.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual IEnumerator GetEnumerator() => List!.GetEnumerator();
    }
}
