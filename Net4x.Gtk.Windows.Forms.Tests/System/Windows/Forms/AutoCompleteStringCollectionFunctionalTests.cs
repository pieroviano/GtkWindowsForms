namespace System.Windows.Forms;

using System.ComponentModel;

/// <summary>
/// Verifies <see cref="AutoCompleteStringCollection" /> — the string list that drives auto-completion on
/// text controls. It is a pure model type (no GTK), so these assertions need no form.
/// </summary>
public class AutoCompleteStringCollectionFunctionalTests
{
    [Fact]
    public void ImplementsIList()
    {
        Assert.True(typeof(System.Collections.IList).IsAssignableFrom(typeof(AutoCompleteStringCollection)));
    }

    [Fact]
    public void Add_IndexOf_Contains_RoundTrip()
    {
        var c = new AutoCompleteStringCollection();
        Assert.Equal(0, c.Add("one"));
        Assert.Equal(1, c.Add("two"));

        Assert.Equal(2, c.Count);
        Assert.True(c.Contains("two"));
        Assert.Equal(1, c.IndexOf("two"));
        Assert.Equal("one", c[0]);
    }

    [Fact]
    public void AddRange_AddsAll()
    {
        var c = new AutoCompleteStringCollection();
        c.AddRange(new[] { "a", "b", "c" });
        Assert.Equal(3, c.Count);
        Assert.Equal("c", c[2]);
    }

    [Fact]
    public void Remove_And_Clear()
    {
        var c = new AutoCompleteStringCollection();
        c.AddRange(new[] { "x", "y", "z" });
        c.Remove("y");
        Assert.Equal(2, c.Count);
        Assert.False(c.Contains("y"));

        c.Clear();
        Assert.Equal(0, c.Count);
    }

    [Fact]
    public void CollectionChanged_FiresOnAddAndRemove()
    {
        var c = new AutoCompleteStringCollection();
        var actions = new System.Collections.Generic.List<CollectionChangeAction>();
        c.CollectionChanged += (s, e) => actions.Add(e.Action);

        c.Add("item");                 // Add
        c.Remove("item");              // Remove

        Assert.Contains(CollectionChangeAction.Add, actions);
        Assert.Contains(CollectionChangeAction.Remove, actions);
    }

    [Fact]
    public void Enumeration_YieldsItems()
    {
        var c = new AutoCompleteStringCollection();
        c.AddRange(new[] { "p", "q" });

        var seen = new System.Collections.Generic.List<string>();
        foreach (string s in c)
        {
            seen.Add(s);
        }

        Assert.Equal(new[] { "p", "q" }, seen);
    }
}
