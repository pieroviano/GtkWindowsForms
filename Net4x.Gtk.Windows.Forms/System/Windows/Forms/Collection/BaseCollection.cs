using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides the base functionality for creating data-related collections in the
/// <see cref="T:System.Windows.Forms" /> namespace.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class BaseCollection : MarshalByRefObject, ICollection
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BaseCollection" /> class.
    /// </summary>
    public BaseCollection()
    {

    }

    /// <summary>
    /// Gets the total number of elements in a collection.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual int Count => List?.Count ?? 0;

    /// <summary>
    /// Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets a value indicating whether access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see> is synchronized (thread safe).
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see>.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object SyncRoot => this;

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <value>
    /// The list.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual ArrayList List => [];

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="ar">
    /// The ar.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void CopyTo(Array ar, int index) => List.CopyTo(ar, index);

    /// <summary>
    /// Gets an IEnumerator for the collection.
    /// </summary>
    public virtual IEnumerator GetEnumerator() => List.GetEnumerator();

}