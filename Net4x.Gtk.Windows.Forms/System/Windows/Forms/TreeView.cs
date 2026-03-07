/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gdk;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays a hierarchical collection of labeled items, each represented by a
    /// <see cref="System.Windows.Forms.TreeNode" />
    /// </summary>
    /// <filterpriority> 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class TreeView : ScrollableControl
    {

        public readonly TreeViewBase self = new TreeViewBase();

        internal TreeNode root;

        private bool _checkBoxs;

        private ImageList _imageList;

        private Gtk.TreeStore _store;

        private TreeViewCancelEventArgs cancelEventArgs = null;

        private CellRendererToggle renderercheckbox;

        private CellRendererIcon rendererPixbuf;

        public event TreeViewEventHandler AfterCollapse;

        public event TreeViewEventHandler AfterExpand;

        public event TreeViewEventHandler AfterSelect;

        public event TreeViewCancelEventHandler BeforeSelect;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeView" /> class.
        /// </summary>
        public TreeView() : base()
        {
            root = new TreeNode(this);
            root.Index = "-1";
            root.Name = "__root";
            _store = new Gtk.TreeStore(typeof(string), typeof(bool), typeof(int), typeof(string));
            self.TreeView.Selection.Mode = Gtk.SelectionMode.Single;
            self.TreeView.Model = _store;
            self.TreeView.Realized += TreeView_Realized;
            self.TreeView.Selection.Changed += Selection_Changed;
            self.TreeView.RowCollapsed += TreeView_RowCollapsed;
            self.TreeView.RowExpanded += TreeView_RowExpanded;
            this.BorderStyle = BorderStyle.Fixed3D;

            Gtk.TreeViewColumn column = new Gtk.TreeViewColumn();
            column.Title = System.Properties.Resources.Gtk_Tree_directory;

            renderercheckbox = new CellRendererToggle();
            renderercheckbox.Activatable = true;
            renderercheckbox.IsExpanded = true;
            renderercheckbox.Toggled += CellName_Toggled;

            renderercheckbox.Visible = false;
            column.PackStart(renderercheckbox, false);
            column.AddAttribute(renderercheckbox, "active", 1);

            rendererPixbuf = new CellRendererIcon(this);
            rendererPixbuf.IsExpanded = true;
            rendererPixbuf.Visible = false;
            column.PackStart(rendererPixbuf, false);
            Gtk.CellRendererText renderertext = new Gtk.CellRendererText();
            renderertext.IsExpanded = true;
            renderertext.PlaceholderText = "---";
            column.PackStart(renderertext, true);
            column.AddAttribute(renderertext, "text", 0);
            self.TreeView.AppendColumn(column);
        }
        /// <summary>
        /// Gets or sets a value indicating whether check boxes are displayed next to the tree nodes in the tree view control.
        /// </summary>
        /// <returns>true if a check box is displayed next to each tree node in the tree view control; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool CheckBoxes
        {
            get => _checkBoxs;
            set
            {
                _checkBoxs = value;
                renderercheckbox.Visible = _checkBoxs == true;
            }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the image-list index value of the default image that is displayed by the tree nodes.
        /// </summary>
        /// <returns>A zero-based index that represents the position of an
        /// <see cref="System.Drawing.Image" /> in an
        /// <see cref="System.Windows.Forms.ImageList" /> The default is zero.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified index is less than 0.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int ImageIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the key of the default image for each node in the
        /// <see cref="System.Windows.Forms.TreeView" /> control when it is in an unselected state.
        /// </summary>
        /// <returns>The key of the default image shown for each node
        /// <see cref="System.Windows.Forms.TreeView" /> control when the node is in an unselected state.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageKey { get; set; }

        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ImageList" /> that contains the
        /// <see cref="System.Drawing.Image" /> objects that are used by the tree nodes.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ImageList" /> that contains the
        /// <see cref="System.Drawing.Image" /> objects that are used by the tree nodes. The default value is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ImageList ImageList
        {
            get => _imageList;
            set
            {
                _imageList = value;
                rendererPixbuf.Visible = _imageList != null;
                if (_imageList != null)
                {
                    Gtk.TreeViewColumn column = self.TreeView.Columns[0];
                    column.AddAttribute(rendererPixbuf, "pixbufkey", 3);
                    column.AddAttribute(rendererPixbuf, "pixbufindex", 2);
                }
            }
        }

        /// <summary>
        /// Gets the collection of tree nodes that are assigned to the tree view control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TreeNodeCollection" /> that represents the tree nodes assigned to the tree view control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeNodeCollection Nodes
        {
            get
            {
                return root.Nodes;
            }
        }

        /// <summary>
        /// Gets or sets the delimiter string that the tree node path uses.
        /// </summary>
        /// <returns>The delimiter string that the tree node
        /// <see cref="System.Windows.Forms.TreeNode.FullPath" /> property uses. The default is the backslash character (\).
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue("\\")]
        public string PathSeparator
        {
            get;
            set;
        } = "\\";

        /// <summary>
        /// Gets or sets the image list index value of the image that is displayed when a tree node is selected.
        /// </summary>
        /// <returns>A zero-based index value that represents the position of an
        /// <see cref="System.Drawing.Image" /> in an
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </returns>
        /// <exception cref="System.ArgumentException">The index assigned value is less than zero.
        /// </exception>
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
        /// Gets or sets the key of the default image shown when a
        /// <see cref="System.Windows.Forms.TreeNode" /> is in a selected state.
        /// </summary>
        /// <returns>The key of the default image shown when a
        /// <see cref="System.Windows.Forms.TreeNode" /> is in a selected state.
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

        public object SelectedItem
        {
            get
            {
                return SelectedNode.Text;
            }
        }

        /// <summary>
        /// Gets or sets the tree node that is currently selected in the tree view control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TreeNode" /> that is currently selected in the tree view control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TreeNode SelectedNode
        {
            get
            {
                TreePath[] paths = self.TreeView.Selection.GetSelectedRows();
                if (paths.Length > 0)
                {
                    TreeNode result = null;
                    GetNodeChild(root, paths.Last().Indices, ref result);
                    return result;
                }
                else { return null; }
            }
            set
            {
                if (value == null)
                    self.TreeView.Selection.UnselectAll();
                else
                {
                    TreePath path = _store.GetPath(value.TreeIter);
                    if (path != null)
                    {
                        self.TreeView.ExpandToPath(path);
                        self.TreeView.Selection.SelectPath(path);
                    }
                }
            }
        }

        public object SelectedValue
        {
            get
            {
                return SelectedNode.Text;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether lines are drawn between tree nodes in the tree view control.
        /// </summary>
        /// <returns>true if lines are drawn between tree nodes in the tree view control; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowLines { get => self.TreeView.EnableTreeLines; set { self.TreeView.EnableTreeLines = true; self.TreeView.EnableGridLines = Gtk.TreeViewGridLines.Horizontal; } }

        public bool ShowNodeToolsTips { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether plus-sign (+) and minus-sign (-) buttons are displayed next to tree nodes that contain child tree nodes.
        /// </summary>
        /// <returns>true if plus sign and minus sign buttons are displayed next to tree nodes that contain child tree nodes; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowPlusMinus { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether lines are drawn between the tree nodes that are at the root of the tree view.
        /// </summary>
        /// <returns>true if lines are drawn between the tree nodes that are at the root of the tree view; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowRootLines { get; set; } = true;

        public int StateImageIndex { get; set; }

        public string StateImageKey { get; set; }

        /// <summary>
        /// Gets or sets the first fully-visible tree node in the tree view control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TreeNode" /> that represents the first fully-visible tree node in the tree view control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TreeNode TopNode
        {
            get
            {
                return root;
            }
            set
            {

            }
        }

        internal Gtk.TreeStore Store { get { return _store; } }

        public void Clear()
        {
            Store.Clear();
        }

        /// <summary>
        /// Collapses all the tree nodes.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void CollapseAll()
        {
            self.TreeView.CollapseAll();
        }

        /// <summary>
        /// Expands all the tree nodes.
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
            self.TreeView.ExpandAll();
        }
        internal bool GetNodeChecked(TreeNode node)
        {
            var val = Store.GetValue(node.TreeIter, 1);

            return val == null ? false : (bool)val;
        }

        internal bool GetNodeExpanded(TreeNode node)
        {
            return self.TreeView.GetRowExpanded(_store.GetPath(node.TreeIter));
        }

        internal bool GetNodeSelected(TreeNode node)
        {
            Gtk.TreePath[] treePaths = self.TreeView.Selection.GetSelectedRows();
            foreach (Gtk.TreePath treePath in treePaths)
            {
                if (Store.GetIter(out Gtk.TreeIter iter, treePath))
                {
                    if (iter.Equals(node.TreeIter))
                        return true;
                }
            }

            return false;
        }

        internal void InsertNode(int position, TreeNode node, TreeNode parent)
        {
            if (node != null && parent != null)
            {
                var iter = Store.InsertWithValues(parent.TreeIter, position, node.Text, node.Checked, node.ImageIndex, node.ImageKey);
                Gtk.TreePath path = Store.GetPath(iter);
                node.Index = string.Join(",", path.Indices);
                node.TreeIter = iter;
                foreach (TreeNode node2 in node.Nodes)
                    LoadNodeValue(node2, node.TreeIter);
            }
        }

        internal void LoadNodeValue(TreeNode node, TreeIter parent)
        {
            TreeIter iter = parent.Equals(TreeIter.Zero) ? Store.AppendValues(node.Text, node.Checked, node.ImageIndex, node.ImageKey) : Store.AppendValues(parent, node.Text, node.Checked, node.ImageIndex, node.ImageKey);
            TreePath path = Store.GetPath(iter);
            node.Index = string.Join(",", path.Indices);
            node.TreeIter = iter;
            foreach (TreeNode child in node.Nodes)
            {
                LoadNodeValue(child, iter);
            }
        }

        internal void NativeNodeChecked(TreeNode node, bool isChecked)
        {
            if (node != null)
            {
                _store.SetValue(node.TreeIter, 1, isChecked);
            }
        }

        internal void NativeNodeImage(TreeNode node, int index)
        {
            if (node != null)
            {
                _store.SetValue(node.TreeIter, 2, index);
            }
        }

        internal void NativeNodeImage(TreeNode node, string key)
        {
            if (node != null)
            {
                _store.SetValue(node.TreeIter, 3, key);
            }
        }

        internal void NativeNodeSelected(TreeNode node, bool isSelected)
        {
            if (node != null)
            {
                this.SelectedNode = node;
            }
        }

        internal void NativeNodeText(TreeNode node, string text)
        {
            if (node != null)
            {
                _store.SetValue(node.TreeIter, 0, text);
            }
        }

        internal void RemoveNode(TreeNode node)
        {
            _store.Remove(ref node.TreeIter);
        }

        internal void SetCollapseNode(TreeNode node)
        {

            self.TreeView.CollapseRow(_store.GetPath(node.TreeIter));
        }

        internal void SetExpandNode(TreeNode node, bool all)
        {
            self.TreeView.ExpandRow(_store.GetPath(node.TreeIter), all);
        }
        protected override void SetStyle(Widget widget)
        {
            self.TreeView.Name = this.Name;
            base.SetStyle(self.TreeView);
        }
        private void CellName_Toggled(object o, ToggledArgs args)
        {
            TreePath path = new TreePath(args.Path);
            var model = _store;
            model.GetIter(out TreeIter iter, path);
            bool val = (bool)(model.GetValue(iter, 1));
            model.SetValue(iter, 1, val == false);
        }

        private void GetNodeChild(TreeNode node, int[] indices, ref TreeNode result)
        {
            string nodeIndex = string.Join(",", indices);
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Index == nodeIndex)
                {
                    result = child;
                    return;
                }
                else if (nodeIndex.Length >= child.Index.Length)
                {
                    GetNodeChild(child, indices, ref result);
                }
            }
        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            if (BeforeSelect != null || AfterSelect != null)
            {
                Gtk.TreeSelection selection = sender as Gtk.TreeSelection;
                TreePath[] paths = selection.GetSelectedRows();
                if (paths.Length > 0)
                {
                    TreePath currpath = paths.Last();
                    TreeNode result = null;
                    GetNodeChild(root, currpath.Indices, ref result);
                    cancelEventArgs = new TreeViewCancelEventArgs(result, false, TreeViewAction.ByMouse);

                    BeforeSelect?.Invoke(this, cancelEventArgs);
                    if (cancelEventArgs == null || cancelEventArgs.Cancel == false)
                    {
                        AfterSelect?.Invoke(this, new TreeViewEventArgs(result));
                    }
                    else
                    {
                        selection.UnselectPath(currpath);
                    }
                }
            }
        }

        private void TreeView_Realized(object sender, EventArgs e)
        {
            if (ImageList != null)
            {
                Gtk.TreeViewColumn column = ((Gtk.TreeView)sender).Columns[0];
                if (string.IsNullOrWhiteSpace(ImageKey))
                {
                    System.Drawing.Image image = ImageList.GetBitmap(ImageIndex);
                    if (image != null) rendererPixbuf.Pixbuf = image.Pixbuf;
                }
                else
                {
                    System.Drawing.Image image = ImageList.GetBitmap(ImageKey);
                    if (image != null) rendererPixbuf.Pixbuf = image.Pixbuf;
                }
            }
        }

        private void TreeView_RowCollapsed(object o, RowCollapsedArgs args)
        {
            if (AfterCollapse != null && ((Gtk.TreeView)o).IsVisible)
            {
                TreeNode result = null;
                GetNodeChild(root, args.Path.Indices, ref result);
                AfterCollapse(this, new TreeViewEventArgs(result, TreeViewAction.Collapse));
            }
        }

        private void TreeView_RowExpanded(object o, RowExpandedArgs args)
        {
            if (AfterExpand != null && ((Gtk.TreeView)o).IsVisible)
            {
                TreeNode result = null;
                GetNodeChild(root, args.Path.Indices, ref result);
                AfterExpand(this, new TreeViewEventArgs(result, TreeViewAction.Expand));
            }
        }
        private class CellRendererIcon : Gtk.CellRendererPixbuf
        {

            public TreeView _treeView;
            public CellRendererIcon(TreeView treeView)
            {
                _treeView = treeView;
            }
            [Property("pixbufindex")]
            public int PixbufIndex
            {
                set
                {
                    if (value < _treeView.ImageList.Images.Count)
                        this.Pixbuf = _treeView.ImageList.Images[value].Pixbuf;
                }
            }

            [Property("pixbufkey")]
            public string PixbufKey
            {
                set
                {

                    if (string.IsNullOrWhiteSpace(value) == false && _treeView.ImageList.Images.ContainsKey(value))
                        this.Pixbuf = _treeView.ImageList.Images[value].Pixbuf;
                }
            }
        }
    }
}
