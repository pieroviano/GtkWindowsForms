using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the collection of groups within a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ListBindable(false)]
    public class ListViewGroupCollection : List<ListViewGroup>
    {
        ListView _listView;

        internal ListViewGroupCollection(ListView listView)
        {
            _listView = listView;

        }
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
        /// Adds the specified
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to the collection.
        /// </summary>
        /// <returns>The index of the group within the collection, or -1 if the group is already present in the collection.
        /// </returns>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="group" /> contains at least one
        /// <see cref="System.Windows.Forms.ListViewItem" /> that belongs to a
        /// <see cref="System.Windows.Forms.ListView" /> control other than the one that owns this
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new int Add(ListViewGroup group)
        {
            AddCore(group);

            return Count;
        }

        /// <summary>
        /// Adds a new
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to the collection using the specified values to initialize the
        /// <see cref="System.Windows.Forms.ListViewGroup.Name" /> and
        /// <see cref="System.Windows.Forms.ListViewGroup.Header" /> properties
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.ListViewGroup" />
        /// </returns>
        /// <param name="key">The initial value of the
        /// <see cref="System.Windows.Forms.ListViewGroup.Name" /> property for the new group.
        /// </param>
        /// <param name="headerText">The initial value of the
        /// <see cref="System.Windows.Forms.ListViewGroup.Header" /> property for the new group.
        /// </param>
        public ListViewGroup Add(string key, string headerText)
        {
            ListViewGroup group = new ListViewGroup(key, headerText);
            this.Add(group);

            return group;
        }

        /// <summary>
        /// Adds an array of groups to the collection.
        /// </summary>
        /// <param name="groups">An array of type
        /// <see cref="System.Windows.Forms.ListViewGroup" /> that specifies the groups to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="groups" /> contains at least one group with at least one
        /// <see cref="System.Windows.Forms.ListViewItem" /> that belongs to a
        /// <see cref="System.Windows.Forms.ListView" /> control other than the one that owns this
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void AddRange(ListViewGroup[] groups)
        {
            foreach (ListViewGroup group in groups)
            {
                AddCore(group);
            }
        }

        /// <summary>
        /// Adds the groups in an existing
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" /> to the collection.
        /// </summary>
        /// <param name="groups">A
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" /> containing the groups to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="groups" /> contains at least one group with at least one
        /// <see cref="System.Windows.Forms.ListViewItem" /> that belongs to a
        /// <see cref="System.Windows.Forms.ListView" /> control other than the one that owns this
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void AddRange(ListViewGroupCollection groups)
        {
            foreach (ListViewGroup group in groups)
            {
                AddCore(group);
            }
        }

        /// <summary>
        /// Removes all groups from the collection.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Clear()
        {
            _listView.NativeGroupsClear();
            base.Clear();
        }

        public bool Contains(string name)
        {
            return base.FindIndex(w => w.Name == name) > -1;
        }
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
