// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

/// <summary>
/// Represents a node of a
/// <see cref="T:System.Windows.Forms.TreeView" />
/// </summary>
public class TreeNode : ICloneable, ISerializable, IEquatable<TreeNode>
{

    internal Gtk.TreeIter TreeIter = Gtk.TreeIter.Zero;

    internal TreeView? treeView;

    private int _imageIndex;

    private string? _imageKey;

    private bool _IsChecked;

    private bool _IsSelected;

    private string? _text;

    // format， union of indexes at all levels：0,1,2,3....

    private string index = "";

    private readonly TreeNodeCollection nodes;

    private TreeNode? parent;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    public TreeNode()
    {
        nodes = new TreeNodeCollection(this);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public TreeNode(string? text) : this()
    {
        Text = text;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="children">
    /// The children.
    /// </param>
    public TreeNode(string text, TreeNode[] children) : this()
    {
        Text = text;
        Nodes.AddRange(children);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public TreeNode(TreeView view) : this()
    {
        this.treeView = view;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    public TreeNode(TreeNode node) : this()
    {
        this.parent = node;
        this.treeView = node.TreeView;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNode" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="pImageIndex">
    /// Index of the p image.
    /// </param>
    /// <param name="pSelectedImageIndex">
    /// Index of the p selected image.
    /// </param>
    public TreeNode(string text, int pImageIndex, int pSelectedImageIndex) : this(text)
    {
        ImageIndex = pImageIndex;
        SelectedImageIndex = pSelectedImageIndex;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.TreeNode" /> is checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
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
    /// Gets or sets the full path.
    /// </summary>
    /// <value>
    /// The full path.
    /// </value>
    public string FullPath
    {
        get
        {
            var paths=new List<string?>();
            GetFullPath(this, paths);

            return string.Join("/", paths);
        }
        set { }
    }

    /// <summary>
    /// Gets or sets the index of the image.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    public int ImageIndex
    {
        get => _imageIndex;
        set { _imageIndex = value; TreeView?.NativeNodeImage(this, value); }
    }

    /// <summary>
    /// Gets or sets the image key.
    /// </summary>
    /// <value>
    /// The image key.
    /// </value>
    public string? ImageKey
    {
        get => _imageKey;
        set { _imageKey = value; TreeView?.NativeNodeImage(this, value); }
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public string Index
    {
        get { return index; }
        internal set { index = value ?? ""; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is expanded.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is expanded; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsExpanded
    {
        get
        {
            return TreeView?.GetNodeExpanded(this) == true;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
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
    /// Gets the level.
    /// </summary>
    /// <value>
    /// The level.
    /// </value>
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
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string? Name
    {
        get;
        set;
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
            return nodes;
        }
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    public TreeNode? Parent
    {
        get { return parent; }

        internal set { parent = value; }
    }

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
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    public object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public string? Text
    {
        get => _text;
        set
        {
            _text = value;
            TreeView?.NativeNodeText(this, value);
        }
    }

    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    /// <value>
    /// The tool tip text.
    /// </value>
    public string? ToolTipText
    {
        get;
        set;
    }

    internal TreeView? TreeView
    {
        get
        {
            if (treeView == null)
            { treeView = this.parent?.TreeView; }

            return treeView;
        }
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object? Clone()
    {
        return null;
    }

    /// <summary>
    /// Collapses this instance.
    /// </summary>
    public void Collapse()
    {
        TreeView?.SetCollapseNode(this);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// true if the current object is equal to the
    /// <paramref name="other">
    /// other
    /// </paramref> parameter; otherwise, false.
    /// </returns>
    public bool Equals([AllowNull] TreeNode other)
    {
        return other != null && other.Index == this.Index && other.Name == this.Name && other.Text == this.Text && other.Level == this.Level;
    }

    /// <summary>
    /// Expands this instance.
    /// </summary>
    public void Expand()
    {
        TreeView?.SetExpandNode(this, false);
    }

    /// <summary>
    /// Expands all.
    /// </summary>
    public void ExpandAll()
    {
        TreeView?.SetExpandNode(this, true);
    }

    /// <summary>
    /// Populates a
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">
    /// The
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> to populate with data.
    /// </param>
    /// <param name="context">
    /// The destination (see
    /// <see cref="T:System.Runtime.Serialization.StreamingContext">
    /// </see>) for this serialization.
    /// </param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    public void Remove()
    {
        TreeView?.RemoveNode(this);
    }

    /// <summary>
    /// Gets the full path.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <param name="paths">
    /// The paths.
    /// </param>
    protected void GetFullPath(TreeNode node, List<string?> paths)
    {
        while (true)
        {
            paths.Add(node.Text);
            if (node.Parent != null && node.Parent.index != "-1")
            {
                node = node.Parent;
                continue;
            }

            break;
        }
    }
}