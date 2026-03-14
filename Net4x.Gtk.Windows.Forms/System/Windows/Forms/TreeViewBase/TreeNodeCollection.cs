namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.TreeNode" /> objects.
/// </summary>
public class TreeNodeCollection : List<TreeNode>
{
    /// <summary>
    /// The owner
    /// </summary>
    private readonly TreeNode owner;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNodeCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    internal TreeNodeCollection(TreeNode owner)
    {
        this.owner = owner;

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
            throw new ArgumentNullException("nodes");
        }
        foreach (TreeNode node in nodes)
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
            throw new ArgumentNullException("nodes");
        }
        foreach (TreeNode node in nodes)
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
            foreach (TreeNode node in this)
                owner.TreeView.RemoveNode(node);
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
    /// Removes the specified node.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    public new void Remove(TreeNode node)
    {
        if (owner.TreeView != null)
            this.owner.TreeView.RemoveNode(node);
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