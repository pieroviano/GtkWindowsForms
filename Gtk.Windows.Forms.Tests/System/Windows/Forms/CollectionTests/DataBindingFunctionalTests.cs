using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms.CollectionTests;

/// <summary>Behavioural tests for the data-binding plumbing: <see cref="ListBindingHelper"/> must report the
/// right list, item type and item properties for each shape of data source; <see cref="CurrencyManager"/>
/// must act as a cursor over that list; and a <see cref="Binding"/> must push values into the bound control
/// property and back.</summary>
[Collection("NonParallelCollection")]
public class DataBindingFunctionalTests
{
    public class Person
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }

    public class Department
    {
        public string Title { get; set; } = string.Empty;

        public List<Person> Staff { get; set; } = [];
    }

    private static List<Person> People() =>
    [
        new() { Name = "Carol", Age = 42 },
        new() { Name = "Alice", Age = 31 },
        new() { Name = "Bob", Age = 55 }
    ];

    private static DataTable PeopleTable()
    {
        var table = new DataTable("People");
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("Carol", 42);
        table.Rows.Add("Alice", 31);

        return table;
    }

    // ---- ListBindingHelper ------------------------------------------------

    [Fact]
    public void GetList_OfAList_ReturnsTheListItself()
    {
        var people = People();

        Assert.Same(people, ListBindingHelper.GetList(people));
    }

    [Fact]
    public void GetList_OfANonList_ReturnsTheObjectItself()
    {
        var person = new Person { Name = "Solo" };

        Assert.Same(person, ListBindingHelper.GetList(person));
    }

    [Fact]
    public void GetList_WithADataMember_ReturnsThatMembersList()
    {
        var department = new Department { Title = "Ops", Staff = People() };

        var list = ListBindingHelper.GetList(department, "Staff");

        Assert.Same(department.Staff, list);
    }

    [Fact]
    public void GetList_WithAnEmptyDataMember_ReturnsTheSourceList()
    {
        var people = People();

        Assert.Same(people, ListBindingHelper.GetList(people, string.Empty));
    }

    [Fact]
    public void GetListItemType_OfATypedList_IsTheElementType()
    {
        Assert.Equal(typeof(Person), ListBindingHelper.GetListItemType(People()));
    }

    [Fact]
    public void GetListItemType_OfANonList_IsTheObjectsOwnType()
    {
        Assert.Equal(typeof(Person), ListBindingHelper.GetListItemType(new Person()));
    }

    [Fact]
    public void GetListItemType_WithADataMember_IsTheMembersElementType()
    {
        var department = new Department { Staff = People() };

        Assert.Equal(typeof(Person), ListBindingHelper.GetListItemType(department, "Staff"));
    }

    [Fact]
    public void GetListItemProperties_OfATypedList_DescribesTheItemProperties()
    {
        var properties = ListBindingHelper.GetListItemProperties(People());

        Assert.NotNull(properties);
        Assert.NotNull(properties!["Name"]);
        Assert.NotNull(properties["Age"]);
        Assert.Equal(typeof(int), properties["Age"]!.PropertyType);
    }

    [Fact]
    public void GetListItemProperties_OfADataTable_DescribesTheColumns()
    {
        using var table = PeopleTable();

        var properties = ListBindingHelper.GetListItemProperties(table);

        Assert.NotNull(properties);
        Assert.NotNull(properties!["Name"]);
        Assert.NotNull(properties["Age"]);
    }

    [Fact]
    public void GetListName_OfADataView_IsTheTableName()
    {
        // DataView is the ITypedList in ADO.NET, so it is what reports the underlying table name.
        using var table = PeopleTable();

        Assert.Equal("People", ListBindingHelper.GetListName(table.DefaultView, null));
    }

    [Fact]
    public void GetListName_OfANonTypedList_IsTheTypeName()
    {
        using var table = PeopleTable();

        Assert.Equal(nameof(DataTable), ListBindingHelper.GetListName(table, null));
    }

    [Fact]
    public void GetListName_OfATypedList_IsTheItemTypeName()
    {
        Assert.Equal(nameof(Person), ListBindingHelper.GetListName(People(), null));
    }

    // ---- CurrencyManager --------------------------------------------------

    private static CurrencyManager Manager(object source)
    {
        var context = new BindingContext();
        var manager = context[source];

        return Assert.IsType<CurrencyManager>(manager);
    }

    [Fact]
    public void CurrencyManager_CountAndListMatchTheSource()
    {
        var people = People();
        var manager = Manager(people);

        Assert.Equal(3, manager.Count);
        Assert.Same(people, manager.List);
    }

    [Fact]
    public void CurrencyManager_StartsOnTheFirstItem()
    {
        var manager = Manager(People());

        Assert.Equal(0, manager.Position);
        Assert.Equal("Carol", ((Person)manager.Current!).Name);
    }

    [Fact]
    public void CurrencyManager_PositionMovesCurrent()
    {
        var manager = Manager(People());

        manager.Position = 2;

        Assert.Equal(2, manager.Position);
        Assert.Equal("Bob", ((Person)manager.Current!).Name);
    }

    [Fact]
    public void CurrencyManager_PositionIsClampedToTheList()
    {
        var manager = Manager(People());

        manager.Position = 99;
        Assert.Equal(2, manager.Position);

        manager.Position = -1;
        Assert.Equal(0, manager.Position);
    }

    [Fact]
    public void CurrencyManager_RemoveAtShrinksTheList()
    {
        var people = People();
        var manager = Manager(people);

        manager.RemoveAt(0);

        Assert.Equal(2, manager.Count);
        Assert.Equal("Alice", people[0].Name);
    }

    [Fact]
    public void CurrencyManager_GetItemPropertiesDescribesTheItems()
    {
        var manager = Manager(People());

        var properties = manager.GetItemProperties();

        Assert.NotNull(properties);
        Assert.NotNull(properties!["Name"]);
    }

    [Fact]
    public void CurrencyManager_SuspendAndResumeBindingFlipTheFlag()
    {
        var manager = Manager(People());

        manager.SuspendBinding();
        Assert.True(manager.IsBindingSuspended, "SuspendBinding should suspend the manager");

        manager.ResumeBinding();
        Assert.False(manager.IsBindingSuspended, "ResumeBinding should resume the manager");
    }

    [Fact]
    public void CurrencyManager_PositionChangedFiresOnMove()
    {
        var manager = Manager(People());
        var fired = 0;
        manager.PositionChanged += (_, _) => fired++;

        manager.Position = 1;

        Assert.True(fired > 0, "moving the position should raise PositionChanged");
    }

    [Fact]
    public void BindingContext_ReturnsTheSameManagerForTheSameSource()
    {
        var people = People();
        var context = new BindingContext();

        var first = context[people];
        var second = context[people];

        Assert.Same(first, second);
        Assert.True(context.Contains(people), "the context should report the source it manages");
    }

    // ---- Binding ----------------------------------------------------------

    [Fact]
    public void Binding_RemembersItsPropertyAndMember()
    {
        var people = People();
        var binding = new Binding("Text", people, "Name");

        Assert.Equal("Text", binding.PropertyName);
        Assert.Equal("Name", binding.BindingMemberInfo?.BindingField);
    }

    [Fact]
    public void Binding_ExposesTheDataSourceItWasGiven()
    {
        var people = People();
        var binding = new Binding("Text", people, "Name");

        Assert.Same(people, binding.DataSource);
    }

    [Fact]
    public void Binding_UpdateModesRoundTrip()
    {
        var binding = new Binding("Text", People(), "Name")
        {
            ControlUpdateMode = ControlUpdateMode.Never,
            DataSourceUpdateMode = DataSourceUpdateMode.Never,
            NullValue = "(none)",
            DataSourceNullValue = DBNull.Value,
            FormatString = "X",
            FormattingEnabled = true
        };

        Assert.Equal(ControlUpdateMode.Never, binding.ControlUpdateMode);
        Assert.Equal(DataSourceUpdateMode.Never, binding.DataSourceUpdateMode);
        Assert.Equal("(none)", binding.NullValue);
        Assert.Equal(DBNull.Value, binding.DataSourceNullValue);
        Assert.Equal("X", binding.FormatString);
        Assert.True(binding.FormattingEnabled);
    }

    [Fact]
    public void Binding_IsNotBoundUntilItIsAddedToAControl()
    {
        var binding = new Binding("Text", People(), "Name");

        Assert.False(binding.IsBinding, "an unattached binding is not active");
        Assert.Null(binding.Control);
    }

    [Fact]
    public void AddingABindingToAControl_AttachesItToThatControl()
    {
        using var box = new TextBox();
        var people = People();

        var bindings = Assert.IsType<ControlBindingsCollection>(box.DataBindings);
        bindings.Add(new Binding("Text", people, "Name"));

        Assert.Single(bindings);
        Assert.Same(box, bindings[0].Control);
    }

    [Fact]
    public void ControlBindingsCollection_AddByMemberNameCreatesTheBinding()
    {
        using var box = new TextBox();
        var people = People();

        var bindings = Assert.IsType<ControlBindingsCollection>(box.DataBindings);

        var binding = bindings.Add("Text", people, "Name");

        Assert.Equal("Text", binding.PropertyName);
        Assert.Same(box, binding.Control);
    }

    [Fact]
    public void ControlBindingsCollection_RemoveDropsTheBinding()
    {
        using var box = new TextBox();
        var bindings = Assert.IsType<ControlBindingsCollection>(box.DataBindings);
        var binding = bindings.Add("Text", People(), "Name");

        bindings.Remove(binding);

        Assert.Empty(bindings);
    }

    [Fact]
    public void ControlBindingsCollection_ClearRemovesEveryBinding()
    {
        using var box = new TextBox();
        var bindings = Assert.IsType<ControlBindingsCollection>(box.DataBindings);
        bindings.Add("Text", People(), "Name");

        bindings.Clear();

        Assert.Empty(bindings);
    }

    [Fact]
    public void BindingMemberInfo_SplitsPathAndField()
    {
        var info = new BindingMemberInfo("Staff.Name");

        Assert.Equal("Staff", info.BindingPath);
        Assert.Equal("Name", info.BindingField);
        Assert.Equal("Staff.Name", info.BindingMember);
    }

    [Fact]
    public void BindingMemberInfo_WithNoPath_HasOnlyAField()
    {
        var info = new BindingMemberInfo("Name");

        Assert.Equal(string.Empty, info.BindingPath);
        Assert.Equal("Name", info.BindingField);
    }
}
