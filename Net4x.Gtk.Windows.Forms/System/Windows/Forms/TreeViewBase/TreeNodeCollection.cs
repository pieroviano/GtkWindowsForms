namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="System.Windows.Forms.TreeNode" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TreeNodeCollection : List<TreeNode>
    {
        private TreeNode owner;

        internal TreeNodeCollection(TreeNode owner)
        {
            this.owner = owner;

        }
        /// <summary>
        /// Adds an array of previously created tree nodes to the collection.
        /// </summary>
        /// <param name="nodes">An array of
        /// <see cref="System.Windows.Forms.TreeNode" /> objects representing the tree nodes to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="nodes" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="nodes" />
        /// is the child of another
        /// <see cref="System.Windows.Forms.TreeView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void AddRange(TreeNode[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            foreach (TreeNode node in nodes)
            {
                Add(node);
            }
        }
        public new void AddRange(IEnumerable<TreeNode> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            foreach (TreeNode node in nodes)
            {
                Add(node);
            }
        }
        /// <summary>
        /// Adds a previously created tree node to the end of the tree node collection.
        /// </summary>
        /// <returns>The zero-based index value of the
        /// <see cref="System.Windows.Forms.TreeNode" /> added to the tree node collection.
        /// </returns>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentException">The
        /// <paramref name="node" /> is currently assigned to another
        /// <see cref="System.Windows.Forms.TreeView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Add(TreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            node.Parent = owner;
            node.treeView = owner.TreeView;
            base.Add(node);
            if (owner != null && owner.TreeView != null)
            {
                owner.TreeView.LoadNodeValue(node, owner.TreeIter);
            }
        }
        /// <summary>
        /// Inserts an existing tree node into the tree node collection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the tree node.
        /// </param>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> to insert into the collection.
        /// </param>
        /// <exception cref="System.ArgumentException">The
        /// <paramref name="node" /> is currently assigned to another
        /// <see cref="System.Windows.Forms.TreeView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Insert(int index, TreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (this.Count <= index)
            {
                throw new ArgumentOutOfRangeException("index " + index.ToString());
            }
            base.Insert(index, node);
            if (owner != null && owner.TreeView != null)
            {
                owner.TreeView.InsertNode(index, node, owner);
            }
        }
        /// <summary>
        /// Removes all tree nodes from the collection.
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
            if (owner.TreeView != null)
            {
                foreach (TreeNode node in this)
                    owner.TreeView.RemoveNode(node);
            }
            base.Clear();
        }
        /// <summary>
        /// Removes the specified tree node from the tree node collection.
        /// </summary>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> to remove.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Remove(TreeNode node)
        {
            if (owner.TreeView != null)
                this.owner.TreeView.RemoveNode(node);
            base.Remove(node);
        }
        /// <summary>
        /// Removes a tree node from the tree node collection at a specified index.
        /// </summary>
        /// <param name="index">The index of the
        /// <see cref="System.Windows.Forms.TreeNode" /> to remove.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void RemoveAt(int index)
        {
            if (index < this.Count)
                Remove(this[index]);
        }
    }
}
