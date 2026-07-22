using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms.CollectionTests;

/// <summary>Behavioural tests for <see cref="BindingSource"/>. A BindingSource is a cursor over a list:
/// navigation must move Position and Current together, mutation must be visible through both the source and
/// the underlying list, and sorting/filtering/searching must actually reorder or locate items.</summary>
[Collection("NonParallelCollection")]
public class BindingSourceFunctionalTests
{
    public class Person
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public override string ToString() => Name;
    }

    private static List<Person> People() =>
    [
        new() { Name = "Carol", Age = 42 },
        new() { Name = "Alice", Age = 31 },
        new() { Name = "Bob", Age = 55 }
    ];

    private static BindingSource Bound() => new(People(), null);

    [Fact]
    public void Count_ReflectsTheBoundList()
    {
        using var source = Bound();

        Assert.Equal(3, source.Count);
    }

    [Fact]
    public void Indexer_ReturnsTheItemAtThatPosition()
    {
        using var source = Bound();

        Assert.Equal("Carol", ((Person)source[0]).Name);
        Assert.Equal("Bob", ((Person)source[2]).Name);
    }

    [Fact]
    public void Position_StartsAtTheFirstItem_AndCurrentAgrees()
    {
        using var source = Bound();

        Assert.Equal(0, source.Position);
        Assert.Equal("Carol", ((Person)source.Current!).Name);
    }

    [Fact]
    public void MoveNext_AdvancesPositionAndCurrentTogether()
    {
        using var source = Bound();

        source.MoveNext();

        Assert.Equal(1, source.Position);
        Assert.Equal("Alice", ((Person)source.Current!).Name);
    }

    [Fact]
    public void MoveLast_And_MoveFirst_JumpToTheEnds()
    {
        using var source = Bound();

        source.MoveLast();
        Assert.Equal(2, source.Position);
        Assert.Equal("Bob", ((Person)source.Current!).Name);

        source.MoveFirst();
        Assert.Equal(0, source.Position);
        Assert.Equal("Carol", ((Person)source.Current!).Name);
    }

    [Fact]
    public void MovePrevious_StepsBack()
    {
        using var source = Bound();
        source.MoveLast();

        source.MovePrevious();

        Assert.Equal(1, source.Position);
    }

    [Fact]
    public void Position_IsClampedToTheListBounds()
    {
        using var source = Bound();

        source.Position = 99;
        Assert.Equal(2, source.Position);

        source.Position = -5;
        Assert.Equal(0, source.Position);
    }

    [Fact]
    public void MoveNext_AtTheEnd_StaysAtTheLastItem()
    {
        using var source = Bound();
        source.MoveLast();

        source.MoveNext();

        Assert.Equal(2, source.Position);
    }

    [Fact]
    public void Add_AppendsToTheUnderlyingList()
    {
        var people = People();
        using var source = new BindingSource(people, null);

        source.Add(new Person { Name = "Dave", Age = 20 });

        Assert.Equal(4, source.Count);
        Assert.Equal(4, people.Count);
        Assert.Equal("Dave", people[3].Name);
    }

    [Fact]
    public void Insert_PlacesTheItemAtThatPosition()
    {
        var people = People();
        using var source = new BindingSource(people, null);

        source.Insert(1, new Person { Name = "Dave" });

        Assert.Equal(4, source.Count);
        Assert.Equal("Dave", people[1].Name);
    }

    [Fact]
    public void Remove_DropsTheItem()
    {
        var people = People();
        using var source = new BindingSource(people, null);
        var alice = people[1];

        source.Remove(alice);

        Assert.Equal(2, source.Count);
        Assert.DoesNotContain(alice, people);
    }

    [Fact]
    public void RemoveAt_DropsThatPosition()
    {
        var people = People();
        using var source = new BindingSource(people, null);

        source.RemoveAt(0);

        Assert.Equal(2, source.Count);
        Assert.Equal("Alice", people[0].Name);
    }

    [Fact]
    public void RemoveCurrent_DropsTheItemUnderTheCursor()
    {
        var people = People();
        using var source = new BindingSource(people, null);
        source.MoveNext();

        source.RemoveCurrent();

        Assert.Equal(2, source.Count);
        Assert.DoesNotContain(people, p => p.Name == "Alice");
    }

    [Fact]
    public void Clear_EmptiesTheList()
    {
        var people = People();
        using var source = new BindingSource(people, null);

        source.Clear();

        Assert.Empty(source);
        Assert.Empty(people);
    }

    [Fact]
    public void Contains_And_IndexOf_LocateItems()
    {
        var people = People();
        using var source = new BindingSource(people, null);
        var bob = people[2];

        Assert.True(source.Contains(bob), "an item in the list should be contained");
        Assert.Equal(2, source.IndexOf(bob));
        Assert.False(source.Contains(new Person { Name = "Nobody" }), "a foreign item should not be contained");
    }

    [Fact]
    public void CopyTo_CopiesEveryItem()
    {
        using var source = Bound();
        var target = new Person[3];

        source.CopyTo(target, 0);

        Assert.Equal("Carol", target[0].Name);
        Assert.Equal("Bob", target[2].Name);
    }

    [Fact]
    public void Enumerator_YieldsEveryItemInOrder()
    {
        using var source = Bound();

        var names = new List<string>();
        foreach (Person p in source)
        {
            names.Add(p.Name);
        }

        Assert.Equal(new[] { "Carol", "Alice", "Bob" }, names);
    }

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

    [Fact]
    public void Find_ByPropertyName_ReturnsTheMatchingIndex()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        var index = source.Find("Name", "Bob");

        Assert.Equal(2, index);
    }

    [Fact]
    public void Find_ByPropertyName_ReturnsMinusOneWhenAbsent()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        Assert.Equal(-1, source.Find("Name", "Nobody"));
    }

    [Fact]
    public void Find_OnAListThatCannotSearch_ThrowsAsTheContractRequires()
    {
        // BindingList<T> does not implement FindCore, so searching it is unsupported - the same contract
        // the framework BindingSource follows.
        using var source = new BindingSource(new BindingList<Person>(People()), null);

        Assert.Throws<NotSupportedException>(() => source.Find("Name", "Bob"));
    }

    [Fact]
    public void BoundToADataTable_TheCursorWalksTheRows()
    {
        using var table = PeopleTable();
        using var source = new BindingSource(table, null);

        Assert.Equal(3, source.Count);

        source.MoveNext();

        Assert.Equal(1, source.Position);
        Assert.Equal("Alice", ((DataRowView)source.Current!)["Name"]);
    }

    [Fact]
    public void DataSource_RoundTripsAndResetsTheCursor()
    {
        using var source = Bound();
        source.MoveLast();

        var replacement = new List<Person> { new() { Name = "Zoe" } };
        source.DataSource = replacement;

        Assert.Same(replacement, source.DataSource);
        Assert.Single(source);
        Assert.Equal(0, source.Position);
    }

    [Fact]
    public void List_ExposesTheUnderlyingList()
    {
        var people = People();
        using var source = new BindingSource(people, null);

        Assert.Equal(3, source.List.Count);
        Assert.Equal("Carol", ((Person)source.List[0]!).Name);
    }

    [Fact]
    public void SupportsFlags_ReportTheCapabilitiesOfThisImplementation()
    {
        using var source = Bound();

        Assert.True(source.SupportsChangeNotification, "a BindingSource always raises change notifications");
        Assert.False(source.IsReadOnly, "a list-backed BindingSource is writable");
        Assert.False(source.IsFixedSize, "a list-backed BindingSource can grow");
    }

    [Fact]
    public void AllowEditAndAllowRemove_AreTrueForAWritableList()
    {
        using var source = Bound();

        Assert.True(source.AllowEdit, "a writable list allows editing");
        Assert.True(source.AllowRemove, "a writable list allows removal");
    }

    [Fact]
    public void AddNew_AppendsANewItemAndMakesItCurrent()
    {
        using var source = new BindingSource(new BindingList<Person>(People()), null);

        var created = source.AddNew();

        Assert.NotNull(created);
        Assert.Equal(4, source.Count);
        Assert.Same(created, source.Current);
    }

    [Fact]
    public void RaiseListChangedEvents_RoundTrips()
    {
        using var source = Bound();

        source.RaiseListChangedEvents = false;
        Assert.False(source.RaiseListChangedEvents);

        source.RaiseListChangedEvents = true;
        Assert.True(source.RaiseListChangedEvents);
    }

    [Fact]
    public void ListChanged_FiresWhenAnItemIsAdded()
    {
        using var source = new BindingSource(new BindingList<Person>(People()), null);
        var fired = 0;
        source.ListChanged += (_, _) => fired++;

        source.Add(new Person { Name = "Dave" });

        Assert.True(fired > 0, "adding an item should raise ListChanged");
    }

    [Fact]
    public void PositionChanged_FiresWhenTheCursorMoves()
    {
        using var source = Bound();
        var fired = 0;
        source.PositionChanged += (_, _) => fired++;

        source.MoveNext();

        Assert.True(fired > 0, "moving the cursor should raise PositionChanged");
    }

    [Fact]
    public void CurrentChanged_FiresWhenTheCursorMoves()
    {
        using var source = Bound();
        var fired = 0;
        source.CurrentChanged += (_, _) => fired++;

        source.MoveNext();

        Assert.True(fired > 0, "moving the cursor should raise CurrentChanged");
    }

    [Fact]
    public void Current_IsNullForAnEmptySource()
    {
        using var source = new BindingSource(new List<Person>(), null);

        Assert.Null(source.Current);
        Assert.Empty(source);
    }

    [Fact]
    public void CurrencyManager_TracksTheSamePosition()
    {
        using var source = Bound();
        source.MoveNext();

        Assert.NotNull(source.CurrencyManager);
        Assert.Equal(source.Position, source.CurrencyManager!.Position);
    }

    [Fact]
    public void ResetBindings_DoesNotDisturbTheContents()
    {
        using var source = Bound();

        source.ResetBindings(false);

        Assert.Equal(3, source.Count);
        Assert.Equal(0, source.Position);
    }

    [Fact]
    public void EndEdit_And_CancelEdit_LeaveTheListIntact()
    {
        using var source = Bound();

        source.EndEdit();
        source.CancelEdit();

        Assert.Equal(3, source.Count);
    }
}
