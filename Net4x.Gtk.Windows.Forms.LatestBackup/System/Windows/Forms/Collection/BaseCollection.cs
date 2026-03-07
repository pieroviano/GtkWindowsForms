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
        /// Gets the total number of elements in a collection.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual int Count => List!.Count;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public bool IsReadOnly => false;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public bool IsSynchronized => false;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public object SyncRoot => this;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ArrayList? List => null;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public void CopyTo(Array ar, int index) => List!.CopyTo(ar, index);

        /// <summary>
        /// Gets an IEnumerator for the collection.
        /// </summary>
        public virtual IEnumerator GetEnumerator() => List!.GetEnumerator();
    }
}
