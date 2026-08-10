/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using Image = System.Drawing.Image;

namespace System.Windows.Forms;

/// <summary>
/// Displays a hierarchical collection of labeled items, each represented by a
/// <see cref="T:System.Windows.Forms.TreeNode" />
/// </summary>
[DesignerCategory("Component")]
public partial class TreeView : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly TreeViewBase self = new();

    internal TreeNode root;

    private bool _checkBoxs;

    private ImageList? _imageList;

    private readonly Gtk.TreeStore _store;

    private TreeViewCancelEventArgs? cancelEventArgs = null;

    private bool Is_TreeView_Realized;

    private readonly CellRendererToggle renderercheckbox;

    private readonly CellRendererIcon rendererPixbuf;

    /// <summary>
    /// Occurs after collapse.
    /// </summary>
    public virtual event TreeViewEventHandler? AfterCollapse;

    /// <summary>
    /// Occurs after expand.
    /// </summary>
    public virtual event TreeViewEventHandler? AfterExpand;

    /// <summary>
    /// Occurs after select.
    /// </summary>
    public virtual event TreeViewEventHandler? AfterSelect;

    /// <summary>
    /// Occurs before select.
    /// </summary>
    public virtual event TreeViewCancelEventHandler? BeforeSelect;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeView" /> class.
    /// </summary>
    public TreeView() : base()
    {
        self.Override.sender = this;
        root = new TreeNode(this)
        {
            Name = "__root__"
        };
        _store = new Gtk.TreeStore(typeof(string), typeof(bool), typeof(int), typeof(string));
        self.TreeView.Selection.Mode = Gtk.SelectionMode.Single;
        self.TreeView.Model = _store;
        self.TreeView.Realized += TreeView_Realized;
        self.TreeView.Selection.Changed += Selection_Changed;
        self.TreeView.RowCollapsed += TreeView_RowCollapsed;
        self.TreeView.RowExpanded += TreeView_RowExpanded;
        this.BorderStyle = BorderStyle.Fixed3D;

        var column = new Gtk.TreeViewColumn
        {
            Title = System.Properties.Resources.Gtk_Tree_directory
        };

        renderercheckbox = new CellRendererToggle
        {
            Activatable = true,
            IsExpanded = true
        };
        renderercheckbox.Toggled += CellName_Toggled;

        renderercheckbox.Visible = false;
        column.PackStart(renderercheckbox, false);
        column.AddAttribute(renderercheckbox, "active", 1);

        rendererPixbuf = new CellRendererIcon(this)
        {
            IsExpanded = true,
            Visible = false
        };
        column.PackStart(rendererPixbuf, false);
        var renderertext = new Gtk.CellRendererText
        {
            IsExpanded = true,
            PlaceholderText = "---"
        };
        column.PackStart(renderertext, true);
        column.AddAttribute(renderertext, "text", 0);
        self.TreeView.AppendColumn(column);
    }

    /// <summary>
    /// Gets or sets a value indicating whether check boxes.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if check boxes; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool CheckBoxes
    {
        get => _checkBoxs;
        set
        {
            _checkBoxs = value;
            renderercheckbox.Visible = _checkBoxs == true;
        }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the index of the image.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    public int ImageIndex
    {
        get;
        set;
    } = -1;

    /// <summary>
    /// Gets or sets the image key.
    /// </summary>
    /// <value>
    /// The image key.
    /// </value>
    public string? ImageKey
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image list.
    /// </summary>
    /// <value>
    /// The image list.
    /// </value>
    public ImageList? ImageList
    {
        get => _imageList;
        set
        {
            _imageList = value;
            rendererPixbuf.Visible = _imageList != null;
            if (_imageList != null)
            {
                var column = self.TreeView.Columns[0];
                column.AddAttribute(rendererPixbuf, "pixbufkey", 3);
                column.AddAttribute(rendererPixbuf, "pixbufindex", 2);
            }
        }
    }

    /// <summary>
    /// Gets the nodes.
    /// </summary>
    /// <value>
    /// The nodes.
    /// </value>
    public TreeNodeCollection Nodes
    {
        get
        {
            return root.Nodes;
        }
    }

    /// <summary>
    /// Gets or sets the path separator.
    /// </summary>
    /// <value>
    /// The path separator.
    /// </value>
    [DefaultValue("\\")]
    public string PathSeparator
    {
        get;
        set;
    } = "\\";

    /// <summary>
    /// Gets or sets the index of the selected image.
    /// </summary>
    /// <value>
    /// The index of the selected image.
    /// </value>
    public int SelectedImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected image key.
    /// </summary>
    /// <value>
    /// The selected image key.
    /// </value>
    public string? SelectedImageKey
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    public object? SelectedItem
    {
        get
        {
            return SelectedNode?.Text;
        }
    }

    /// <summary>
    /// Gets or sets the selected node.
    /// </summary>
    /// <value>
    /// The selected node.
    /// </value>
    public TreeNode? SelectedNode
    {
        get
        {
            var paths = self.TreeView.Selection.GetSelectedRows();
            if (paths.Length > 0)
            {
                TreeNode? result = null;
                if (_store.GetIter(out var iter, paths.Last()))
                    GetNodeChild(root, iter, ref result);

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
                var path = _store.GetPath(value.TreeIter);
                if (path != null)
                {
                    self.TreeView.ExpandToPath(path);
                    self.TreeView.Selection.SelectPath(path);
                }
            }
        }
    }

    /// <summary>
    /// Gets the selected value.
    /// </summary>
    /// <value>
    /// The selected value.
    /// </value>
    public object? SelectedValue
    {
        get
        {
            return SelectedNode?.Text;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether show lines.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show lines; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowLines
    {
        get => self.TreeView.EnableTreeLines;
        // The setter ignored its argument and always switched lines on, so ShowLines = false did nothing.
        set
        {
            self.TreeView.EnableTreeLines = value;
            self.TreeView.EnableGridLines = value ? Gtk.TreeViewGridLines.Horizontal : Gtk.TreeViewGridLines.None;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether show node tools tips.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show node tools tips; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowNodeToolsTips
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show plus minus.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show plus minus; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowPlusMinus
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether show root lines.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show root lines; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowRootLines
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the index of the state image.
    /// </summary>
    /// <value>
    /// The index of the state image.
    /// </value>
    public int StateImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the state image key.
    /// </summary>
    /// <value>
    /// The state image key.
    /// </value>
    public string? StateImageKey
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the top node.
    /// </summary>
    /// <value>
    /// The top node.
    /// </value>
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

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        Store.Clear();
        Nodes.Clear();
    }

    /// <summary>
    /// Collapses all.
    /// </summary>
    public void CollapseAll()
    {
        self.TreeView.CollapseAll();
    }

    /// <summary>
    /// Expands all.
    /// </summary>
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
        var treePaths = self.TreeView.Selection.GetSelectedRows();
        foreach (var treePath in treePaths)
        {
            if (Store.GetIter(out var iter, treePath))
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
            node.TreeIter = iter;
            foreach (var node2 in node.Nodes)
                LoadNodeValue(node2, node.TreeIter);
        }
    }

    internal void LoadNodeValue(TreeNode node, TreeIter parent)
    {
        var iter = parent.Equals(TreeIter.Zero) ? Store.AppendValues(node.Text, node.Checked, node.ImageIndex, node.ImageKey) : Store.AppendValues(parent, node.Text, node.Checked, node.ImageIndex, node.ImageKey);
        node.TreeIter = iter;
        foreach (var child in node.Nodes)
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

    internal void NativeNodeImage(TreeNode node, string? key)
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

    internal void NativeNodeText(TreeNode node, string? text)
    {
        if (node != null)
        {
            _store.SetValue(node.TreeIter, 0, text);
        }
    }

    internal void NativeRemoveNode(TreeNode node)
    {
        var iter = node.TreeIter;
        if (!TreeIter.Zero.Equals(iter))
            _store.Remove(ref iter);
    }

    internal void SetCollapseNode(TreeNode node)
    {
        if (!TreeIter.Zero.Equals(node.TreeIter))
            self.TreeView.CollapseRow(_store.GetPath(node.TreeIter));
    }

    internal void SetExpandNode(TreeNode node, bool all)
    {
        if (!TreeIter.Zero.Equals(node.TreeIter))
            self.TreeView.ExpandRow(_store.GetPath(node.TreeIter), all);
    }

    /// <summary>
    /// Raises the AfterCollapse event.
    /// </summary>
    protected virtual void OnAfterCollapse(System.Windows.Forms.TreeViewEventArgs e)
    {
        AfterCollapse?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AfterExpand event.
    /// </summary>
    protected virtual void OnAfterExpand(System.Windows.Forms.TreeViewEventArgs e)
    {
        AfterExpand?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AfterSelect event.
    /// </summary>
    protected virtual void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
    {
        AfterSelect?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BeforeSelect event.
    /// </summary>
    protected virtual void OnBeforeSelect(System.Windows.Forms.TreeViewCancelEventArgs e)
    {
        BeforeSelect?.Invoke(this, e);
    }

    /// <summary>
    /// Applies styling to the GTK widget based on control properties.
    /// </summary>
    /// <param name="widget">
    /// GTK widget to style.
    /// </param>
    protected override void SetStyle(Widget widget)
    {
        self.TreeView.Name = this.Name;
        base.SetStyle(self.TreeView);
    }

    private void CellName_Toggled(object o, ToggledArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _store;
        model.GetIter(out var iter, path);
        var val = (bool)(model.GetValue(iter, 1));
        model.SetValue(iter, 1, val == false);
    }

    private void GetNodeChild(TreeNode node, TreeIter iter, ref TreeNode? result)
    {
        foreach (var child in node.Nodes)
        {
            if (iter.Equals(child.TreeIter))
            {
                result = child;
                return;
            }
            else
            {
                GetNodeChild(child, iter, ref result);
            }
        }
    }

    private void Selection_Changed(object sender, EventArgs e)
    {
        if (BeforeSelect != null || AfterSelect != null)
        {
            var selection = sender as Gtk.TreeSelection;
            var paths = selection?.GetSelectedRows();
            if (paths?.Length > 0)
            {
                var currpath = paths.Last();
                TreeNode? result = null;
                if (_store.GetIter(out var iter, currpath))
                    GetNodeChild(root, iter, ref result);
                cancelEventArgs = new TreeViewCancelEventArgs(result, false, TreeViewAction.ByMouse);

                OnBeforeSelect(cancelEventArgs);
                if (cancelEventArgs == null || cancelEventArgs.Cancel == false)
                {
                    OnAfterSelect(new TreeViewEventArgs(result));
                }
                else
                {
                    selection?.UnselectPath(currpath);
                }
            }
        }
    }

    private void TreeView_Realized(object sender, EventArgs e)
    {
        if (!Is_TreeView_Realized)
        {
            Is_TreeView_Realized = true;
            if (ImageList != null && ImageList.Images.Count > 0)
            {
                var column = ((Gtk.TreeView)sender).Columns[0];
                if (!string.IsNullOrWhiteSpace(ImageKey))
                {
                    System.Drawing.Image? image = ImageList.GetBitmap(ImageKey);
                    if (image != null) rendererPixbuf.Pixbuf = image.Pixbuf;
                }
                else if (ImageIndex >= 0)
                {
                    Image? image = ImageList.GetBitmap(ImageIndex);
                    if (image != null) rendererPixbuf.Pixbuf = image.Pixbuf;
                }
            }
        }
    }

    private void TreeView_RowCollapsed(object o, RowCollapsedArgs args)
    {
        if (AfterCollapse != null && ((Gtk.TreeView)o).IsVisible)
        {
            TreeNode? result = null;
            if (_store.GetIter(out var iter, args.Path))
                GetNodeChild(root, iter, ref result);
            OnAfterCollapse(new TreeViewEventArgs(result, TreeViewAction.Collapse));
        }
    }

    private void TreeView_RowExpanded(object o, RowExpandedArgs args)
    {
        if (AfterExpand != null && ((Gtk.TreeView)o).IsVisible)
        {
            TreeNode? result = null;
            if (_store.GetIter(out var iter, args.Path))
                GetNodeChild(root, iter, ref result);
            OnAfterExpand(new TreeViewEventArgs(result, TreeViewAction.Expand));
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.Windows.Forms.TreeView" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        root.Nodes.Clear();
        Store.Clear();
        Store.Dispose();
        base.Dispose(disposing);
    }

    private class CellRendererIcon : Gtk.CellRendererPixbuf
    {
        /// <summary>
        /// The tree view
        /// </summary>
        public readonly TreeView _treeView;

        static CellRendererIcon()
        {
            typeof(CellRendererIcon).GetMembers();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.TreeView.CellRendererIcon" /> class.
        /// </summary>
        /// <param name="treeView">
        /// The tree view.
        /// </param>
        public CellRendererIcon(TreeView treeView)
        {
            _treeView = treeView;
        }

        /// <summary>
        /// Sets the index of the pixbuf.
        /// </summary>
        /// <value>
        /// The index of the pixbuf.
        /// </value>
        [Property("pixbufindex")]
        public int PixbufIndex
        {
            set
            {
                if (value < _treeView.ImageList?.Images.Count && _treeView.ImageList.Images[value]?.Pixbuf != null)
                    this.Pixbuf = _treeView.ImageList.Images[value]?.Pixbuf;
            }
        }

        /// <summary>
        /// Sets the pixbuf key.
        /// </summary>
        /// <value>
        /// The pixbuf key.
        /// </value>
        [Property("pixbufkey")]
        public string PixbufKey
        {
            set
            {

                if (string.IsNullOrWhiteSpace(value) == false && (_treeView.ImageList?.Images.ContainsKey(value) ?? false) && _treeView.ImageList.Images[value]?.Pixbuf != null)
                    this.Pixbuf = _treeView.ImageList.Images[value]?.Pixbuf;
            }
        }
    }
}