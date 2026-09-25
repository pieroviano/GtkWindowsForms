namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.TreeNode" /> objects.
/// </summary>
public class TreeNodeCollection : List<TreeNode>
{
    private readonly TreeNode owner;

    internal TreeNodeCollection(TreeNode owner)
    {
        this.owner = owner;

    }

    /// <summary>
    /// The nodes whose <see cref="TreeNode.Name" /> matches <paramref name="key" />, ignoring case.
    /// </summary>
    /// <param name="key">The name to look for.</param>
    /// <param name="searchAllChildren">Whether to search the nodes below these as well.</param>
    public TreeNode[] Find(string key, bool searchAllChildren)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var found = new List<TreeNode>();
        Find(this, key, searchAllChildren, found);
        return found.ToArray();
    }

    private static void Find(IEnumerable<TreeNode> nodes, string key, bool searchAllChildren, List<TreeNode> found)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Name, key, StringComparison.OrdinalIgnoreCase))
            {
                found.Add(node);
            }
            if (searchAllChildren)
            {
                Find(node.Nodes, key, true, found);
            }
        }
    }

    /// <summary>
    /// Adds the specified node.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// node
    /// </exception>
    public new void Add(TreeNode node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
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
    /// Adds a new node carrying the specified text and returns it.
    /// </summary>
    /// <param name="text">The label of the new node.</param>
    /// <returns>The node that was added.</returns>
    public TreeNode Add(string text)
    {
        // The string overloads and key lookups of the WinForms TreeNodeCollection were missing, so
        // ordinary WinForms code such as tree.Nodes.Add("text") did not compile against this shim.
        var node = new TreeNode(text);
        Add(node);

        return node;
    }

    /// <summary>
    /// Adds a new node with the specified key and text and returns it.
    /// </summary>
    /// <param name="key">The name used to look the node up.</param>
    /// <param name="text">The label of the new node.</param>
    /// <returns>The node that was added.</returns>
    public TreeNode Add(string key, string text)
    {
        var node = new TreeNode(text) { Name = key };
        Add(node);

        return node;
    }

    /// <summary>
    /// Gets the first node whose <see cref="P:System.Windows.Forms.TreeNode.Name" /> matches the key,
    /// or <see langword="null" /> when there is none. Keys are matched case-insensitively.
    /// </summary>
    /// <param name="key">The name to look up.</param>
    public virtual TreeNode? this[string key]
    {
        get
        {
            var index = IndexOfKey(key);

            return index == -1 ? null : this[index];
        }
    }

    /// <summary>
    /// Returns the index of the first node with the specified key, or -1 when there is none.
    /// </summary>
    /// <param name="key">The name to look up.</param>
    public virtual int IndexOfKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return -1;
        }

        return FindIndex(n => string.Equals(n.Name, key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Indicates whether a node with the specified key is present.
    /// </summary>
    /// <param name="key">The name to look up.</param>
    public virtual bool ContainsKey(string? key) => IndexOfKey(key) != -1;

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="nodes">
    /// The nodes.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// nodes
    /// </exception>
    public void AddRange(TreeNode[] nodes)
    {
        if (nodes == null)
        {
            throw new ArgumentNullException(nameof(nodes));
        }
        foreach (var node in nodes)
        {
            Add(node);
        }
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="nodes">
    /// The nodes.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// nodes
    /// </exception>
    public new void AddRange(IEnumerable<TreeNode> nodes)
    {
        if (nodes == null)
        {
            throw new ArgumentNullException(nameof(nodes));
        }
        foreach (var node in nodes)
        {
            Add(node);
        }
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public new void Clear()
    {
        if (owner.TreeView != null)
        {
            foreach (var node in this)
                owner.TreeView.NativeRemoveNode(node);
        }
        base.Clear();
    }

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// node
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// index " + index.ToString()
    /// </exception>
    public new void Insert(int index, TreeNode node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
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
    /// Removes the specified node.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    public new void Remove(TreeNode node)
    {
        if (owner.TreeView != null)
            this.owner.TreeView.NativeRemoveNode(node);
        base.Remove(node);
    }

    /// <summary>
    /// Removes at.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the element to remove.
    /// </param>
    public new void RemoveAt(int index)
    {
        if (index < this.Count)
            Remove(this[index]);
    }
}