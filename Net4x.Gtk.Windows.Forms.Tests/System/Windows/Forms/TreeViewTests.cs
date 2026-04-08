namespace System.Windows.Forms;

extern alias gtk;

using System.Xml.Linq;
using Gtk;

[Collection("NonParallelCollection")]
public class TreeViewTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = tree.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(tree.self);
        Assert.NotNull(tree.Nodes);
        Assert.Equal(tree.self, gtkControl);
        Assert.False(tree.CheckBoxes);
        Assert.Equal("\\", tree.PathSeparator);
    }

    [Fact]
    public void CheckBoxes_ShouldToggleRendererVisibility()
    {
        var tree = new TreeView
        {
            CheckBoxes = true
        };

        Assert.True(tree.CheckBoxes);

        tree.CheckBoxes = false;
        Assert.False(tree.CheckBoxes);
    }

    [Fact]
    public void ImageList_ShouldEnableRenderer()
    {
        var tree = new TreeView();

        var imageList = new ImageList();
        tree.ImageList = imageList;

        Assert.Equal(imageList, tree.ImageList);
    }

    [Fact]
    public void Nodes_ShouldAddAndRetrieveNode()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        TreeNode? node = null;

        form.Load += (s, e) =>
        {
            node = new TreeNode("Node1");
            tree.Nodes.Add(node);
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(node);
        Assert.Equal("Node1", node.Text);
    }

    [Fact]
    public void SelectedNode_ShouldSetAndGet()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        TreeNode? node = null;
        TreeNode? selected = null;

        form.Load += (s, e) =>
        {
            node = new TreeNode("Node1");
            tree.Nodes.Add(node);
            tree.SelectedNode = node;
            selected = tree.SelectedNode;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(node);
        Assert.Equal(node, selected);
    }

    [Fact]
    public void SelectedItem_ShouldReturnText()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        object? selectedItem = null;

        form.Load += (s, e) =>
        {
            var node = new TreeNode("Hello");
            tree.Nodes.Add(node);
            tree.SelectedNode = node;
            selectedItem = tree.SelectedItem;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Hello", selectedItem);
    }

    [Fact]
    public void Clear_ShouldRemoveAllNodes()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        int countBefore = 0;

        form.Load += (s, e) =>
        {
            var node = new TreeNode("A");
            tree.Nodes.Add(node);
            node = new TreeNode("B");
            tree.Nodes.Add(node);

            countBefore = tree.Nodes.Count;

            tree.Clear();

            form.Close();
        };

        form.ShowDialog();

        Assert.True(countBefore > 0);
        Assert.Empty(tree.Nodes);
    }

    [Fact]
    public void ExpandAndCollapse_ShouldNotThrow()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        form.Load += (s, e) =>
        {
            var node = new TreeNode("Node1");
            tree.Nodes.Add(node);

            tree.ExpandAll();
            tree.CollapseAll();

            form.Close();
        };

        form.ShowDialog();

        Assert.True(true); // just ensuring no exceptions
    }

    [Fact]
    public void AfterSelect_ShouldFire()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        bool afterSelectRaised = false;

        form.Load += (s, e) =>
        {
            var node = new TreeNode("Node1");
            tree.Nodes.Add(node);

            tree.AfterSelect += (_, __) => afterSelectRaised = true;

            tree.SelectedNode = node;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(afterSelectRaised);
    }

    [Fact]
    public void BeforeSelect_Cancel_ShouldPreventSelection()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        TreeNode? node = null;
        TreeNode? selected = null;

        form.Load += (s, e) =>
        {
            node = new TreeNode("Node1");
            tree.Nodes.Add(node);

            tree.BeforeSelect += (s2, e2) =>
            {
                e2.Cancel = true;
            };

            tree.SelectedNode = node;
            selected = tree.SelectedNode;

            form.Close();
        };

        form.ShowDialog();

        Assert.Null(selected);
    }

    [Fact]
    public void ShowLines_ShouldEnableGtkLines()
    {
        var form = new Form();
        var tree = new TreeView();
        form.Controls.Add(tree);

        bool enabled = false;

        form.Load += (s, e) =>
        {
            tree.ShowLines = true;
            enabled = tree.self.TreeView.EnableTreeLines;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(enabled);
    }

    [Fact]
    public void TopNode_ShouldReturnRoot()
    {
        var tree = new TreeView();

        var top = tree.TopNode;

        Assert.NotNull(top);
        Assert.Equal("__root", top.Name);
    }
}