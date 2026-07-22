using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms.CollectionTests;

/// <summary>Behavioural tests for the sorting, filtering and related-source features of
/// <see cref="BindingSource"/>. Sorting must actually reorder what the cursor walks, filtering must actually
/// hide rows, and a related source must follow its parent's current item.</summary>
[Collection("NonParallelCollection")]
public class BindingSourceSortFilterFunctionalTests
{
    private static DataTable PeopleTable()
    {
        var table = new DataTable("People");
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("Carol", 42);
        table.Rows.Add("Alice", 31);
        table.Rows.Add("Bob", 55);

        return table;
    }

    private static string NameAt(BindingSource source, int index) => (string)((DataRowView)source[index])["Name"];

    [Fact]
    public void Sort_ReordersTheItemsAscending()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        source.Sort = "Name ASC";

        Assert.Equal("Alice", NameAt(source, 0));
        Assert.Equal("Bob", NameAt(source, 1));
        Assert.Equal("Carol", NameAt(source, 2));
    }

    [Fact]
    public void Sort_ReordersTheItemsDescending()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        source.Sort = "Name DESC";

        Assert.Equal("Carol", NameAt(source, 0));
        Assert.Equal("Alice", NameAt(source, 2));
    }

    [Fact]
    public void Sort_RoundTripsTheExpression()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Sort = "Age DESC" };

        Assert.Equal("Age DESC", source.Sort);
    }

    [Fact]
    public void Sort_ByANumericColumn_OrdersNumerically()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        source.Sort = "Age ASC";

        Assert.Equal("Alice", NameAt(source, 0));
        Assert.Equal("Carol", NameAt(source, 1));
        Assert.Equal("Bob", NameAt(source, 2));
    }

    [Fact]
    public void IsSorted_ReportsWhetherASortIsApplied()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        Assert.False(source.IsSorted, "an unsorted source should not report itself sorted");

        source.Sort = "Name ASC";

        Assert.True(source.IsSorted, "a sorted source should report itself sorted");
    }

    [Fact]
    public void SortPropertyAndDirection_DescribeTheAppliedSort()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Sort = "Name DESC" };

        Assert.NotNull(source.SortProperty);
        Assert.Equal("Name", source.SortProperty!.Name);
        Assert.Equal(ListSortDirection.Descending, source.SortDirection);
    }

    [Fact]
    public void RemovingTheSort_RestoresTheOriginalOrder()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Sort = "Name ASC" };

        source.RemoveSort();

        Assert.False(source.IsSorted, "after RemoveSort the source should not report itself sorted");
        Assert.Equal("Carol", NameAt(source, 0));
    }

    [Fact]
    public void ApplySort_ByPropertyDescriptor_OrdersTheItems()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);
        var properties = ((ITypedList)source.List).GetItemProperties(null);

        source.ApplySort(properties["Name"]!, ListSortDirection.Ascending);

        Assert.Equal("Alice", NameAt(source, 0));
    }

    [Fact]
    public void Filter_HidesTheRowsThatDoNotMatch()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        source.Filter = "Age > 40";

        Assert.Equal(2, source.Count);
        Assert.DoesNotContain("Alice", Enumerable.Range(0, source.Count).Select(i => NameAt(source, i)));
    }

    [Fact]
    public void Filter_RoundTripsTheExpression()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Filter = "Age > 40" };

        Assert.Equal("Age > 40", source.Filter);
    }

    [Fact]
    public void RemoveFilter_BringsBackEveryRow()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Filter = "Age > 40" };

        source.RemoveFilter();

        Assert.Equal(3, source.Count);
    }

    [Fact]
    public void SupportsFlags_AreTrueForADataViewBackedSource()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        Assert.True(source.SupportsSorting, "a DataView supports sorting");
        Assert.True(source.SupportsFiltering, "a DataView supports filtering");
        Assert.True(source.SupportsSearching, "a DataView supports searching");
    }

    [Fact]
    public void ARelatedSource_FollowsTheParentsCurrentItem()
    {
        using var parentTable = new DataTable("Parents");
        parentTable.Columns.Add("Id", typeof(int));
        parentTable.Rows.Add(1);
        parentTable.Rows.Add(2);

        using var childTable = new DataTable("Children");
        childTable.Columns.Add("ParentId", typeof(int));
        childTable.Columns.Add("Name", typeof(string));
        childTable.Rows.Add(1, "first child");
        childTable.Rows.Add(2, "second child");
        childTable.Rows.Add(2, "third child");

        using var set = new DataSet();
        set.Tables.Add(parentTable);
        set.Tables.Add(childTable);
        set.Relations.Add("ParentChild", parentTable.Columns["Id"]!, childTable.Columns["ParentId"]!);

        using var parents = new BindingSource(set, "Parents");
        using var children = new BindingSource(parents, "ParentChild");

        Assert.Single(children);

        parents.MoveNext();

        Assert.Equal(2, children.Count);
    }

    [Fact]
    public void DataMember_SelectsTheTableFromADataSet()
    {
        using var table = PeopleTable();
        using var set = new DataSet();
        set.Tables.Add(table);

        using var source = new BindingSource(set, "People");

        Assert.Equal(3, source.Count);
        Assert.Equal("People", source.DataMember);
    }

    [Fact]
    public void SortingDoesNotChangeTheUnderlyingTableOrder()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null) { Sort = "Name ASC" };

        Assert.Equal("Carol", table.Rows[0]["Name"]);
        Assert.Equal("Alice", NameAt(source, 0));
    }

    [Fact]
    public void SortingKeepsTheCursorOnAValidPosition()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);
        source.MoveLast();

        source.Sort = "Name ASC";

        Assert.InRange(source.Position, 0, source.Count - 1);
        Assert.NotNull(source.Current);
    }

    [Fact]
    public void FilteringAdjustsThePositionToStayInRange()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);
        source.MoveLast();

        source.Filter = "Age > 40";

        Assert.InRange(source.Position, 0, source.Count - 1);
    }
}
