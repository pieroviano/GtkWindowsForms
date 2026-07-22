using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the binding pipeline left unestablished by the earlier value-flow
/// pass: <see cref="Binding"/> formatting/parsing (FormatString, FormatInfo, NullValue, the Format
/// and Parse events, DataSourceUpdateMode) and the <see cref="BindingSource"/> /
/// <see cref="CurrencyManager"/> transaction surface (AddNew/AddingNew/CancelEdit, Find, sorting,
/// related sources, list-change reactions).</summary>
[Collection("NonParallelCollection")]
public class BindingPipelineFunctionalTests
{
    private sealed class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public double Price { get; set; }

        public List<Order> Orders { get; set; } = new();
    }

    private sealed class Order
    {
        public string Item { get; set; } = string.Empty;
    }

    // ---- Binding: formatting ---------------------------------------------

    [Fact]
    public void ReadValueAppliesTheFormatString()
    {
        using var box = new TextBox();
        var person = new Person { Price = 1234.5 };
        box.DataBindings.Add(new Binding("Text", person, "Price", true,
            DataSourceUpdateMode.OnValidation, null, "N2", CultureInfo.InvariantCulture));

        box.DataBindings[0].ReadValue();

        Assert.Equal("1,234.50", box.Text);
    }

    [Fact]
    public void ANullSourceValueBecomesTheNullValueAndRoundTripsBack()
    {
        using var box = new TextBox();
        var person = new Person { Name = null };
        var binding = new Binding("Text", person, "Name", true,
            DataSourceUpdateMode.OnValidation, "n/a");
        box.DataBindings.Add(binding);

        binding.ReadValue();
        Assert.Equal("n/a", box.Text);

        person.Name = "set";
        binding.ReadValue();
        Assert.Equal("set", box.Text);

        box.Text = "n/a";
        binding.WriteValue();
        Assert.Null(person.Name);
    }

    [Fact]
    public void TheFormatEventCanReplaceTheOutgoingValue()
    {
        using var box = new TextBox();
        var person = new Person { Name = "raw" };
        var binding = new Binding("Text", person, "Name", true);
        binding.Format += (_, e) => e.Value = "formatted";
        box.DataBindings.Add(binding);

        binding.ReadValue();

        Assert.Equal("formatted", box.Text);
    }

    [Fact]
    public void TheParseEventCanReplaceTheIncomingValue()
    {
        using var box = new TextBox();
        var person = new Person { Name = "before" };
        var binding = new Binding("Text", person, "Name", true);
        binding.Parse += (_, e) => e.Value = "parsed";
        box.DataBindings.Add(binding);

        box.Text = "typed";
        binding.WriteValue();

        Assert.Equal("parsed", person.Name);
    }

    [Fact]
    public void WriteValueConvertsTheTextToThePropertyType()
    {
        using var box = new TextBox();
        var person = new Person { Age = 1 };
        var binding = new Binding("Text", person, "Age", true);
        box.DataBindings.Add(binding);

        box.Text = "42";
        binding.WriteValue();

        Assert.Equal(42, person.Age);
    }

    // ---- Binding: DataSourceUpdateMode ------------------------------------

    [Fact]
    public void OnPropertyChangedWritesBackImmediately()
    {
        using var box = new TextBox();
        var person = new Person { Name = "old" };
        box.DataBindings.Add(new Binding("Text", person, "Name", true,
            DataSourceUpdateMode.OnPropertyChanged));

        box.Text = "typed";

        Assert.Equal("typed", person.Name);
    }

    [Fact]
    public void UpdateModeNeverOnlyWritesExplicitly()
    {
        using var box = new TextBox();
        var person = new Person { Name = "old" };
        var binding = new Binding("Text", person, "Name", true, DataSourceUpdateMode.Never);
        box.DataBindings.Add(binding);

        box.Text = "typed";
        Assert.Equal("old", person.Name);

        binding.WriteValue();
        Assert.Equal("typed", person.Name);
    }

    // ---- BindingSource: AddNew / transactions -----------------------------

    [Fact]
    public void AddNewCreatesAnInstanceOfTheItemType()
    {
        var people = new List<Person> { new() { Name = "existing" } };
        using var source = new BindingSource { DataSource = people };

        var added = source.AddNew();

        Assert.IsType<Person>(added);
        Assert.Equal(2, source.Count);
        Assert.Same(added, source.Current);
    }

    [Fact]
    public void AddingNewCanSupplyTheInstance()
    {
        var custom = new Person { Name = "custom" };
        using var source = new BindingSource { DataSource = new List<Person>() };
        source.AddingNew += (_, e) => e.NewObject = custom;

        var added = source.AddNew();

        Assert.Same(custom, added);
        Assert.Same(custom, source[0]);
    }

    [Fact]
    public void CancelEditRemovesThePendingNewItem()
    {
        using var source = new BindingSource { DataSource = new List<Person> { new() } };

        source.AddNew();
        Assert.Equal(2, source.Count);

        source.CancelEdit();
        Assert.Single(source);
    }

    // ---- BindingSource: Find / sort / related -----------------------------

    [Fact]
    public void FindLocatesARowInASearchableIBindingListSource()
    {
        // WinForms Find requires a searchable IBindingList underneath (DataView is one; a plain
        // List<T> throws NotSupportedException).
        using var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add("Alice");
        table.Rows.Add("Bob");
        using var source = new BindingSource { DataSource = table };

        Assert.Equal(1, source.Find("Name", "Bob"));

        using var plain = new BindingSource { DataSource = new List<Person> { new() { Name = "Alice" } } };
        Assert.Throws<NotSupportedException>(() => plain.Find("Name", "Alice"));
    }

    [Fact]
    public void ASortStringOrdersADataViewBackedSource()
    {
        using var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add("Bravo");
        table.Rows.Add("Alpha");
        table.Rows.Add("Charlie");
        using var source = new BindingSource { DataSource = table };

        source.Sort = "Name DESC";

        Assert.Equal("Charlie", ((DataRowView)source[0])["Name"]);
        Assert.Equal("Alpha", ((DataRowView)source[2])["Name"]);
    }

    [Fact]
    public void ARelatedBindingSourceFollowsTheParentCurrency()
    {
        var people = new List<Person>
        {
            new() { Name = "first", Orders = { new Order { Item = "a1" }, new Order { Item = "a2" } } },
            new() { Name = "second", Orders = { new Order { Item = "b1" } } }
        };
        using var parent = new BindingSource { DataSource = people };
        using var child = new BindingSource(parent, "Orders");

        Assert.Equal(2, child.Count);
        Assert.Equal("a1", ((Order)child[0]!).Item);

        parent.Position = 1;

        Assert.Single(child);
        Assert.Equal("b1", ((Order)child[0]!).Item);
    }

    [Fact]
    public void AnEnumerableDataSourceIsMaterialised()
    {
        static IEnumerable<Person> Generate()
        {
            yield return new Person { Name = "one" };
            yield return new Person { Name = "two" };
        }

        using var source = new BindingSource { DataSource = Generate() };

        Assert.Equal(2, source.Count);
        Assert.Equal("one", ((Person)source[0]!).Name);
    }

    // ---- CurrencyManager --------------------------------------------------

    [Fact]
    public void PositionMovesRaiseTheEventsAndCurrentTracks()
    {
        using var source = new BindingSource
        {
            DataSource = new List<Person> { new() { Name = "a" }, new() { Name = "b" } }
        };
        var manager = source.CurrencyManager!;
        Assert.NotNull(manager);
        var positionChanges = 0;
        var currentChanges = 0;
        manager.PositionChanged += (_, _) => positionChanges++;
        manager.CurrentChanged += (_, _) => currentChanges++;

        manager.Position = 1;

        Assert.Equal(1, manager.Position);
        Assert.Equal("b", ((Person)manager.Current!).Name);
        Assert.True(positionChanges >= 1);
        Assert.True(currentChanges >= 1);
    }

    [Fact]
    public void RemovingTheLastItemClampsThePosition()
    {
        using var source = new BindingSource
        {
            DataSource = new List<Person> { new() { Name = "a" }, new() { Name = "b" } }
        };
        var manager = source.CurrencyManager!;
        Assert.NotNull(manager);
        manager.Position = 1;

        source.RemoveAt(1);

        Assert.Equal(0, manager.Position);
        Assert.Equal(1, manager.Count);
    }

    [Fact]
    public void AFixedSizeSourceForbidsAddingAndRemoving()
    {
        using var listSource = new BindingSource { DataSource = new List<Person> { new() } };
        using var arraySource = new BindingSource { DataSource = new[] { new Person() } };

        Assert.True(listSource.CurrencyManager!.AllowAdd);
        Assert.True(listSource.CurrencyManager!.AllowRemove);
        Assert.False(arraySource.CurrencyManager!.AllowAdd);
        Assert.False(arraySource.CurrencyManager!.AllowRemove);
        Assert.True(arraySource.CurrencyManager!.AllowEdit);
    }
}
