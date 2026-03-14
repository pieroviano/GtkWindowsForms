using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

/// <summary>
/// Class PropertyGrid.
/// Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" />
/// Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
public partial class PropertyGrid
{
    /// <summary>
    /// Class PropertyTabCollection.
    /// Implements the
    /// <see cref="T:System.Collections.ICollection" />
    /// </summary>
    /// <seealso cref="T:System.Collections.ICollection" />
    public class PropertyTabCollection : ICollection
    {
        /// <summary>
        /// The owner property grid
        /// </summary>
        private readonly PropertyGrid _ownerPropertyGrid;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.PropertyGrid.PropertyTabCollection" /> class.
        /// </summary>
        /// <param name="ownerPropertyGrid">The owner property grid.
        /// </param>
        internal PropertyTabCollection(PropertyGrid ownerPropertyGrid)
        {
            _ownerPropertyGrid = ownerPropertyGrid;
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>The count.
        /// </value>
        public int Count
        {
            get
            {
                if (_ownerPropertyGrid is null)
                {
                    return 0;
                }

                return _ownerPropertyGrid._tabs.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the
        /// <see cref="T:System.Collections.ICollection">
        /// </see> is synchronized (thread safe).
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is synchronized; otherwise,
        /// <c>false
        /// </c>.
        /// </value>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>The synchronize root.
        /// </value>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.Design.PropertyTab" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.
        /// </param>
        /// <returns>PropertyTab.
        /// </returns>
        public PropertyTab this[int index]
        {
            get
            {
                return _ownerPropertyGrid._tabs[index].Tab;
            }
        }

        /// <summary>
        /// Adds the type of the tab.
        /// </summary>
        /// <param name="propertyTabType">Type of the property tab.
        /// </param>
        public void AddTabType(Type propertyTabType)
        {

        }

        /// <summary>
        /// Adds the type of the tab.
        /// </summary>
        /// <param name="propertyTabType">Type of the property tab.
        /// </param>
        /// <param name="tabScope">The tab scope.
        /// </param>
        public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
        {

        }

        /// <summary>
        /// Clears the specified tab scope.
        /// </summary>
        /// <param name="tabScope">The tab scope.
        /// </param>
        public void Clear(PropertyTabScope tabScope)
        {

        }

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
            if (_ownerPropertyGrid is null)
            {
                return Array.Empty<PropertyTab>().GetEnumerator();
            }

            return _ownerPropertyGrid._tabs.Select(i => i.Tab).GetEnumerator();
        }

        /// <summary>
        /// Removes the type of the tab.
        /// </summary>
        /// <param name="propertyTabType">Type of the property tab.
        /// </param>
        public void RemoveTabType(Type propertyTabType)
        {

        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="dest">The dest.
        /// </param>
        /// <param name="index">The index.
        /// </param>
        void ICollection.CopyTo(Array dest, int index)
        {
            if (_ownerPropertyGrid is null)
            {
                return;
            }

            if (_ownerPropertyGrid._tabs.Count > 0)
            {
                Array.Copy(
                    _ownerPropertyGrid._tabs.Select(i => i.Tab).ToArray(),
                    0,
                    dest,
                    index,
                    _ownerPropertyGrid._tabs.Count);
            }
        }

    }
}