using System.Data;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms.DataGridViewTests;

/// <summary>Behavioural tests for <see cref="DataGridView"/> data binding — the control's core purpose.
/// Assigning a <see cref="DataTable"/>, <see cref="DataSet"/>, <see cref="DataView"/> or a typed list must
/// generate columns (respecting <see cref="DataGridView.AutoGenerateColumns"/>) and populate rows with the
/// source values. Binding used to be deferred until the GTK widget was realized, so all of this was
/// unobservable — and wrong in WinForms terms — until the grid was shown.</summary>
[Collection("NonParallelCollection")]
public class DataGridViewDataBindingFunctionalTests
{
    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public bool Active { get; set; }
    }

    private static DataTable PeopleTable()
    {
        var table = new DataTable("People");
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Columns.Add("Active", typeof(bool));
        table.Rows.Add("Carol", 42, true);
        table.Rows.Add("Alice", 31, false);

        return table;
    }

    // ---- DataTable --------------------------------------------------------

    [Fact]
    public void BindingADataTable_GeneratesOneColumnPerDataColumn()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Equal(3, grid.Columns.Count);
        Assert.Equal("Name", grid.Columns[0].Name);
        Assert.Equal("Name", grid.Columns[0].HeaderText);
        Assert.Equal("Name", grid.Columns[0].DataPropertyName);
    }

    [Fact]
    public void BindingADataTable_PopulatesTheRowsWithTheValues()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Carol", grid.Rows[0].Cells[0].Value);
        Assert.Equal(42, grid.Rows[0].Cells[1].Value);
        Assert.Equal("Alice", grid.Rows[1].Cells[0].Value);
    }

    [Fact]
    public void ABooleanColumn_BecomesACheckBoxColumn()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.IsType<DataGridViewCheckBoxColumn>(grid.Columns[2]);
        Assert.Equal(true, grid.Rows[0].Cells[2].Value);
    }

    [Fact]
    public void TheColumnValueTypeFollowsTheDataColumn()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Equal(typeof(string), grid.Columns[0].ValueType);
        Assert.Equal(typeof(int), grid.Columns[1].ValueType);
    }

    [Fact]
    public void WithAutoGenerateColumnsOff_NoColumnsAreInvented()
    {
        using var grid = new DataGridView { AutoGenerateColumns = false };
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Empty(grid.Columns);
        Assert.Empty(grid.Rows);
    }

    [Fact]
    public void WithAutoGenerateColumnsOff_PredefinedColumnsAreFilled()
    {
        using var grid = new DataGridView { AutoGenerateColumns = false };
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "onlyName",
            HeaderText = "Person",
            DataPropertyName = "Name"
        });
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Single(grid.Columns);
        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Carol", grid.Rows[0].Cells[0].Value);
    }

    [Fact]
    public void EachBoundRowCarriesItsDataRowAsTheBoundItem()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        var bound = Assert.IsType<DataRow>(grid.Rows[0].DataBoundItem);
        Assert.Equal("Carol", bound["Name"]);
    }

    // ---- DataSet / DataView ----------------------------------------------

    [Fact]
    public void BindingADataSet_UsesTheFirstTableWhenNoDataMemberIsSet()
    {
        using var grid = new DataGridView();
        using var set = new DataSet();
        set.Tables.Add(PeopleTable());

        grid.DataSource = set;

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Carol", grid.Rows[0].Cells[0].Value);
    }

    [Fact]
    public void BindingADataSet_HonoursTheDataMember()
    {
        using var grid = new DataGridView();
        using var set = new DataSet();
        using var other = new DataTable("Other");
        other.Columns.Add("X", typeof(int));
        other.Rows.Add(1);
        set.Tables.Add(other);
        set.Tables.Add(PeopleTable());

        grid.DataMember = "People";
        grid.DataSource = set;

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal(3, grid.Columns.Count);
    }

    [Fact]
    public void BindingADataView_BindsItsTable()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table.DefaultView;

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Carol", grid.Rows[0].Cells[0].Value);
    }

    // ---- typed lists ------------------------------------------------------

    [Fact]
    public void BindingATypedList_GeneratesColumnsFromTheProperties()
    {
        using var grid = new DataGridView();
        var people = new List<Person>
        {
            new() { Name = "Carol", Age = 42, Active = true },
            new() { Name = "Alice", Age = 31, Active = false }
        };

        grid.DataSource = people;

        Assert.Equal(3, grid.Columns.Count);
        Assert.Contains(grid.Columns, c => c.DataPropertyName == "Name");
        Assert.Contains(grid.Columns, c => c.DataPropertyName == "Age");
    }

    [Fact]
    public void BindingATypedList_PopulatesTheRows()
    {
        using var grid = new DataGridView();
        var people = new List<Person>
        {
            new() { Name = "Carol", Age = 42 },
            new() { Name = "Alice", Age = 31 }
        };

        grid.DataSource = people;

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Carol", grid.Rows[0].Cells["Name"].Value);
        Assert.Equal(31, grid.Rows[1].Cells["Age"].Value);
    }

    [Fact]
    public void BindingATypedList_TheBoundItemIsTheEntity()
    {
        using var grid = new DataGridView();
        var carol = new Person { Name = "Carol", Age = 42 };

        grid.DataSource = new List<Person> { carol };

        Assert.Same(carol, grid.Rows[0].DataBoundItem);
    }

    [Fact]
    public void BindingATypedList_ABooleanPropertyBecomesACheckBoxColumn()
    {
        using var grid = new DataGridView();

        grid.DataSource = new List<Person> { new() { Active = true } };

        var active = grid.Columns["Active"];
        Assert.IsType<DataGridViewCheckBoxColumn>(active);
    }

    // ---- rebinding and lifecycle -----------------------------------------

    [Fact]
    public void RebindingReplacesTheRowsInsteadOfAppending()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;
        grid.DataSource = table;

        Assert.Equal(2, grid.Rows.Count);
    }

    [Fact]
    public void BindingASmallerSourceDropsTheOldRows()
    {
        using var grid = new DataGridView();
        using var big = PeopleTable();
        using var small = new DataTable("One");
        small.Columns.Add("Name", typeof(string));
        small.Rows.Add("Solo");

        grid.DataSource = big;
        grid.DataSource = small;

        Assert.Single(grid.Rows);
        Assert.Equal("Solo", grid.Rows[0].Cells["Name"].Value);
    }

    [Fact]
    public void SettingDataSourceToNullClearsTheRows()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();
        grid.DataSource = table;

        grid.DataSource = null;

        Assert.Empty(grid.Rows);
    }

    [Fact]
    public void DataBindingCompleteFiresWhenTheBindHappens()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();
        var fired = 0;
        grid.DataBindingComplete += (_, _) => fired++;

        grid.DataSource = table;

        Assert.Equal(1, fired);
    }

    [Fact]
    public void DataBindingCompleteDoesNotFireForANullSource()
    {
        using var grid = new DataGridView();
        var fired = 0;
        grid.DataBindingComplete += (_, _) => fired++;

        grid.DataSource = null;

        Assert.Equal(0, fired);
    }

    [Fact]
    public void TheDataSourcePropertyRoundTrips()
    {
        using var grid = new DataGridView();
        using var table = PeopleTable();

        grid.DataSource = table;

        Assert.Same(table, grid.DataSource);
    }

    [Fact]
    public void AnEmptyTableYieldsColumnsButNoRows()
    {
        using var grid = new DataGridView();
        using var table = new DataTable("Empty");
        table.Columns.Add("Name", typeof(string));

        grid.DataSource = table;

        Assert.Single(grid.Columns);
        Assert.Empty(grid.Rows);
    }
}
