using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the collection of groups within a
    /// <see cref="T:System.Windows.Forms.ListView" /> control.
    /// </summary>
    [ListBindable(false)]
    public class ListViewGroupCollection : List<ListViewGroup>
    {
        /// <summary>
        /// The list view
        /// </summary>
        ListView _listView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewGroupCollection"/> class.
        /// </summary>
        /// <param name="listView">The list view.</param>
        internal ListViewGroupCollection(ListView listView)
        {
            _listView = listView;

        }

        /// <summary>
        /// Gets or sets the <see cref="ListViewGroup"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>ListViewGroup.</returns>
        public ListViewGroup this[string key]
        {
            get
            {
                return base.Find(w => w.Name == key);
            }
            set
            {
                if (base.FindIndex(w => w.Name == key) > -1)
                {
                    base[FindIndex(w => w.Name == key)] = value;
                }
            }
        }

        /// <summary>
        /// Adds the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>System.Int32.</returns>
        public new int Add(ListViewGroup group)
        {
            AddCore(group);

            return Count;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="headerText">The header text.</param>
        /// <returns>ListViewGroup.</returns>
        public ListViewGroup Add(string key, string headerText)
        {
            ListViewGroup group = new ListViewGroup(key, headerText);
            this.Add(group);

            return group;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="groups">The groups.</param>
        public void AddRange(ListViewGroup[] groups)
        {
            foreach (ListViewGroup group in groups)
            {
                AddCore(group);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="groups">The groups.</param>
        public void AddRange(ListViewGroupCollection groups)
        {
            foreach (ListViewGroup group in groups)
            {
                AddCore(group);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            _listView.NativeGroupsClear();
            base.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return base.FindIndex(w => w.Name == name) > -1;
        }

        /// <summary>
        /// Adds the core.
        /// </summary>
        /// <param name="group">The group.</param>
        private void AddCore(ListViewGroup group)
        {
            if (this.Exists(m => m.Name == group.Name))
            {
                return;
            }
            group.ListView = _listView;
            base.Add(group);

            _listView.NativeGroupAdd(group, -1);
        }

    }
}
