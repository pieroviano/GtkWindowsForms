using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a user interface for browsing the properties of an object.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    /// Provides a user interface for browsing the properties of an object.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    /// Provides a user interface for browsing the properties of an object.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    /// Provides a user interface for browsing the properties of an object.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public partial class PropertyGrid
    {
        public class PropertyTabCollection : ICollection
        {
            private readonly PropertyGrid _ownerPropertyGrid;

            internal PropertyTabCollection(PropertyGrid ownerPropertyGrid)
            {
                _ownerPropertyGrid = ownerPropertyGrid;
            }

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

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this;

            public PropertyTab this[int index]
            {
                get
                {
                    return _ownerPropertyGrid._tabs[index].Tab;
                }
            }

            public void AddTabType(Type propertyTabType)
            {

            }

            public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
            {

            }

            public void Clear(PropertyTabScope tabScope)
            {

            }

            public IEnumerator GetEnumerator()
            {
                if (_ownerPropertyGrid is null)
                {
                    return Array.Empty<PropertyTab>().GetEnumerator();
                }

                return _ownerPropertyGrid._tabs.Select(i => i.Tab).GetEnumerator();
            }

            public void RemoveTabType(Type propertyTabType)
            {

            }

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
}
