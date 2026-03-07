// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a node of a
    /// <see cref="System.Windows.Forms.TreeView" />
    /// </summary>
    /// <filterpriority> 1
    /// </filterpriority>
    public class TreeNode : ICloneable, ISerializable, IEquatable<TreeNode>
    {

        internal Gtk.TreeIter TreeIter = Gtk.TreeIter.Zero;

        internal TreeView treeView;

        private int _imageIndex;

        private string _imageKey;

        private bool _IsChecked;

        private bool _IsSelected;

        private string _text;

        // format， union of indexes at all levels：0,1,2,3....

        private string index = "";

        private TreeNodeCollection nodes;

        private TreeNode parent;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeNode" /> class.
        /// </summary>
        public TreeNode()
        {
            nodes = new TreeNodeCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeNode" /> class with the specified label text.
        /// </summary>
        /// <param name="text">The label
        /// <see cref="System.Windows.Forms.TreeNode.Text" /> of the new tree node.
        /// </param>
        public TreeNode(string text) : this()
        {
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeNode" /> class with the specified label text and child tree nodes.
        /// </summary>
        /// <param name="text">The label
        /// <see cref="System.Windows.Forms.TreeNode.Text" /> of the new tree node.
        /// </param>
        /// <param name="children">An array of child
        /// <see cref="System.Windows.Forms.TreeNode" /> objects.
        /// </param>
        public TreeNode(string text, TreeNode[] children) : this()
        {
            Text = text;
            Nodes.AddRange(children);
        }

        public TreeNode(TreeView view) : this()
        {
            this.treeView = view;
        }

        public TreeNode(TreeNode node) : this()
        {
            this.parent = node;
            this.treeView = node.TreeView;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeNode" /> class with the specified label text and images to display when the tree node is in a selected and unselected state.
        /// </summary>
        /// <param name="text">The label
        /// <see cref="System.Windows.Forms.TreeNode.Text" /> of the new tree node.
        /// </param>
        /// <param name="imageIndex">The index value of
        /// <see cref="System.Drawing.Image" /> to display when the tree node is unselected.
        /// </param>
        /// <param name="selectedImageIndex">The index value of
        /// <see cref="System.Drawing.Image" /> to display when the tree node is selected.
        /// </param>
        public TreeNode(string text, int pImageIndex, int pSelectedImageIndex) : this(text)
        {
            ImageIndex = pImageIndex;
            SelectedImageIndex = pSelectedImageIndex;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree node is in a checked state.
        /// </summary>
        /// <returns>true if the tree node is in a checked state; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Checked
        {
            get
            {
                if (this.TreeIter.Equals(Gtk.TreeIter.Zero) || this.TreeView == null)
                    return _IsChecked;
                else
                {
                    return this.TreeView.GetNodeChecked(this);
                }
            }
            set { _IsChecked = value; TreeView?.NativeNodeChecked(this, value); }
        }

        /// <summary>
        /// Gets the path from the root tree node to the current tree node.
        /// </summary>
        /// <returns>The path from the root tree node to the current tree node.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The node is not contained in a
        /// <see cref="System.Windows.Forms.TreeView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public string FullPath
        {
            get
            {
                List<string> paths = new List<string>();
                GetFullPath(this, paths);

                return string.Join("/", paths);
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the image list index value of the image displayed when the tree node is in the unselected state.
        /// </summary>
        /// <returns>A zero-based index value that represents the image position in the assigned
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int ImageIndex
        {
            get => _imageIndex;
            set { _imageIndex = value; TreeView?.NativeNodeImage(this, value); }
        }

        /// <summary>
        /// Gets or sets the key for the image associated with this tree node when the node is in an unselected state.
        /// </summary>
        /// <returns>The key for the image associated with this tree node when the node is in an unselected state.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageKey
        {
            get => _imageKey;
            set { _imageKey = value; TreeView?.NativeNodeImage(this, value); }
        }

        /// <summary>
        /// Gets the position of the tree node in the tree node collection.
        /// </summary>
        /// <returns>A zero-based index value that represents the position of the tree node in the
        /// <see cref="System.Windows.Forms.TreeNode.Nodes" /> collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string Index { get { return index; } internal set { index = value ?? ""; } }

        /// <summary>
        /// Gets a value indicating whether the tree node is in the expanded state.
        /// </summary>
        /// <returns>true if the tree node is in the expanded state; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool IsExpanded
        {
            get
            {
                return TreeView?.GetNodeExpanded(this) == true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tree node is in the selected state.
        /// </summary>
        /// <returns>true if the tree node is in the selected state; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool IsSelected
        {
            get
            {
                if (this.TreeIter.Equals(Gtk.TreeIter.Zero) || this.TreeView == null)
                    return _IsSelected;
                else
                {
                    return this.TreeView.GetNodeSelected(this);
                }
            }
            set
            {
                _IsSelected = value;
                TreeView?.NativeNodeSelected(this, value);
            }
        }

        /// <summary>
        /// Gets the zero-based depth of the tree node in the
        /// <see cref="System.Windows.Forms.TreeView" /> control.
        /// </summary>
        /// <returns>The zero-based depth of the tree node in the
        /// <see cref="System.Windows.Forms.TreeView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Level
        {
            get
            {
                if (parent == null)
                    return 0;
                else if (TreeView != null && parent.Equals(TreeView.root))
                    return 0;
                else
                    return parent.Level + 1;
            }
        }

        /// <summary>
        /// Gets or sets the name of the tree node.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that represents the name of the tree node.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets the collection of
        /// <see cref="System.Windows.Forms.TreeNode" /> objects assigned to the current tree node.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TreeNodeCollection" /> that represents the tree nodes assigned to the current tree node.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeNodeCollection Nodes
        {
            get
            {
                return nodes;
            }
        }

        /// <summary>
        /// Gets the parent tree node of the current tree node.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TreeNode" /> that represents the parent of the current tree node.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeNode Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        /// <summary>
        /// Gets or sets the image list index value of the image that is displayed when the tree node is in the selected state.
        /// </summary>
        /// <returns>A zero-based index value that represents the image position in an
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SelectedImageIndex { get; set; }

        /// <summary>
        /// Gets or sets the key of the image displayed in the tree node when it is in a selected state.
        /// </summary>
        /// <returns>The key of the image displayed when the tree node is in a selected state.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string SelectedImageKey { get; set; }

        /// <summary>
        /// Gets or sets the index of the image that is used to indicate the state of the
        /// <see cref="System.Windows.Forms.TreeNode" /> when the parent
        /// <see cref="System.Windows.Forms.TreeView" /> has its
        /// <see cref="System.Windows.Forms.TreeView.CheckBoxes" /> property set to false.
        /// </summary>
        /// <returns>The index of the image that is used to indicate the state of the
        /// <see cref="System.Windows.Forms.TreeNode" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified index is less than -1 or greater than 14.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int StateImageIndex { get; set; }

        /// <summary>
        /// Gets or sets the key of the image that is used to indicate the state of the
        /// <see cref="System.Windows.Forms.TreeNode" /> when the parent
        /// <see cref="System.Windows.Forms.TreeView" /> has its
        /// <see cref="System.Windows.Forms.TreeView.CheckBoxes" /> property set to false.
        /// </summary>
        /// <returns>The key of the image that is used to indicate the state of the
        /// <see cref="System.Windows.Forms.TreeNode" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string StateImageKey { get; set; }

        /// <summary>
        /// Gets or sets the object that contains data about the tree node.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the tree node. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the text displayed in the label of the tree node.
        /// </summary>
        /// <returns>The text displayed in the label of the tree node.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                TreeView?.NativeNodeText(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that appears when the mouse pointer hovers over a
        /// <see cref="System.Windows.Forms.TreeNode" />
        /// </summary>
        /// <returns>Gets the text that appears when the mouse pointer hovers over a
        /// <see cref="System.Windows.Forms.TreeNode" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ToolTipText
        {
            get; set;
        }

        /// <summary>
        /// Gets the parent tree view that the tree node is assigned to.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TreeView" /> that represents the parent tree view that the tree node is assigned to, or null if the node has not been assigned to a tree view.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        internal TreeView TreeView
        {
            get
            {
                if (treeView == null)
                { treeView = this.parent?.TreeView; }

                return treeView;
            }
        }

        /// <summary>
        /// Copies the tree node and the entire subtree rooted at this tree node.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Object" /> that represents the cloned
        /// <see cref="System.Windows.Forms.TreeNode" />
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public object Clone()
        {
            return null;
        }

        /// <summary>
        /// Collapses the tree node.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Collapse()
        {
            TreeView?.SetCollapseNode(this);
        }

        public bool Equals([AllowNull] TreeNode other)
        {
            return other != null && other.Index == this.Index && other.Name == this.Name && other.Text == this.Text && other.Level == this.Level;
        }

        /// <summary>
        /// Expands the tree node.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Expand()
        {
            TreeView?.SetExpandNode(this, false);
        }

        /// <summary>
        /// Expands all the child tree nodes.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ExpandAll()
        {
            TreeView?.SetExpandNode(this, true);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Removes the current tree node from the tree view control.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Remove()
        {
            TreeView?.RemoveNode(this);
        }

        protected void GetFullPath(TreeNode node, List<string> paths)
        {
            paths.Add(node.Text);
            if (node.Parent != null && node.Parent.index != "-1")
            {
                GetFullPath(node.Parent, paths);
            }
        }

    }
}
