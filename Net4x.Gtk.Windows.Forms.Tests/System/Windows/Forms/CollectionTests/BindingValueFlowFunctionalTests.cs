using System.ComponentModel;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms.CollectionTests;

/// <summary>Behavioural tests for the value flow of a <see cref="Binding"/>. A binding exists to move data:
/// the bound control property must receive the source value, and writing the control must push the value
/// back into the source object.</summary>
[Collection("NonParallelCollection")]
public class BindingValueFlowFunctionalTests
{
    public class Person : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        private int _age;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
            }
        }
    }

    private static List<Person> People() =>
    [
        new() { Name = "Carol", Age = 42 },
        new() { Name = "Alice", Age = 31 }
    ];

    private static ControlBindingsCollection BindingsOf(Control control) =>
        Assert.IsType<ControlBindingsCollection>(control.DataBindings);

    [Fact]
    public void BindingACollection_PushesTheFirstItemsValueIntoTheControl()
    {
        using var box = new TextBox();
        var people = People();

        BindingsOf(box).Add("Text", people, "Name");

        Assert.Equal("Carol", box.Text);
    }

    [Fact]
    public void ReadValue_RefreshesTheControlFromTheSource()
    {
        using var box = new TextBox();
        var people = People();
        var binding = BindingsOf(box).Add("Text", people, "Name");

        people[0].Name = "Renamed";
        binding.ReadValue();

        Assert.Equal("Renamed", box.Text);
    }

    [Fact]
    public void WriteValue_PushesTheControlValueBackIntoTheSource()
    {
        using var box = new TextBox();
        var people = People();
        var binding = BindingsOf(box).Add("Text", people, "Name");

        box.Text = "Typed";
        binding.WriteValue();

        Assert.Equal("Typed", people[0].Name);
    }

    [Fact]
    public void MovingTheCursor_UpdatesTheBoundControl()
    {
        using var box = new TextBox();
        var people = People();
        using var source = new BindingSource(people, null);
        BindingsOf(box).Add("Text", source, "Name");

        source.MoveNext();

        Assert.Equal("Alice", box.Text);
    }

    [Fact]
    public void ANumericPropertyIsFormattedIntoAStringControl()
    {
        using var box = new TextBox();
        var people = People();

        BindingsOf(box).Add("Text", people, "Age");

        Assert.Equal("42", box.Text);
    }

    [Fact]
    public void WritingANumericPropertyParsesTheControlText()
    {
        using var box = new TextBox();
        var people = People();
        var binding = BindingsOf(box).Add("Text", people, "Age");

        box.Text = "77";
        binding.WriteValue();

        Assert.Equal(77, people[0].Age);
    }

    [Fact]
    public void ABindingBecomesActiveOnceAdded()
    {
        using var box = new TextBox();
        var binding = BindingsOf(box).Add("Text", People(), "Name");

        Assert.True(binding.IsBinding, "a binding attached to a control should be active");
        Assert.Same(box, binding.Control);
        Assert.NotNull(binding.BindingManagerBase);
    }

    [Fact]
    public void TwoControlsBoundToTheSameSourceBothReceiveTheValue()
    {
        using var first = new TextBox();
        using var second = new TextBox();
        var people = People();
        using var source = new BindingSource(people, null);

        BindingsOf(first).Add("Text", source, "Name");
        BindingsOf(second).Add("Text", source, "Name");

        Assert.Equal("Carol", first.Text);
        Assert.Equal("Carol", second.Text);

        source.MoveNext();

        Assert.Equal("Alice", first.Text);
        Assert.Equal("Alice", second.Text);
    }

    [Fact]
    public void RemovingTheBindingStopsFurtherUpdates()
    {
        using var box = new TextBox();
        var people = People();
        using var source = new BindingSource(people, null);
        var binding = BindingsOf(box).Add("Text", source, "Name");

        BindingsOf(box).Remove(binding);
        source.MoveNext();

        Assert.Equal("Carol", box.Text);
    }

    [Fact]
    public void BindingToASingleObjectPushesItsProperty()
    {
        using var box = new TextBox();
        var person = new Person { Name = "Solo", Age = 20 };

        BindingsOf(box).Add("Text", person, "Name");

        Assert.Equal("Solo", box.Text);
    }

    [Fact]
    public void FormatStringIsAppliedWhenFormattingIsEnabled()
    {
        using var box = new TextBox();
        var person = new Person { Name = "Solo", Age = 5 };
        var binding = new Binding("Text", person, "Age", true, DataSourceUpdateMode.OnValidation, null, "00");

        BindingsOf(box).Add(binding);

        Assert.Equal("05", box.Text);
    }

    [Fact]
    public void NullValueIsSubstitutedWhenTheSourceValueIsNull()
    {
        using var box = new TextBox();
        var person = new Person { Name = null! };
        var binding = new Binding("Text", person, "Name", true, DataSourceUpdateMode.OnValidation, "(none)");

        BindingsOf(box).Add(binding);

        Assert.Equal("(none)", box.Text);
    }

    [Fact]
    public void ControlUpdateModeNever_LeavesTheControlAlone()
    {
        using var box = new TextBox { Text = "untouched" };
        var person = new Person { Name = "Solo" };
        var binding = new Binding("Text", person, "Name") { ControlUpdateMode = ControlUpdateMode.Never };

        BindingsOf(box).Add(binding);

        Assert.Equal("untouched", box.Text);
    }

    [Fact]
    public void TheBindingsCollectionExposesTheBindingByPropertyName()
    {
        using var box = new TextBox();
        var binding = BindingsOf(box).Add("Text", People(), "Name");

        Assert.Same(binding, BindingsOf(box)["Text"]);
    }

    [Fact]
    public void BindingTheSamePropertyTwiceIsRejected()
    {
        using var box = new TextBox();
        BindingsOf(box).Add("Text", People(), "Name");

        Assert.ThrowsAny<ArgumentException>(() => BindingsOf(box).Add("Text", People(), "Name"));
    }

    [Fact]
    public void ClearingTheBindingsLeavesTheControlValueAsItWas()
    {
        using var box = new TextBox();
        var people = People();
        BindingsOf(box).Add("Text", people, "Name");
        var before = box.Text;

        BindingsOf(box).Clear();

        Assert.Empty(BindingsOf(box));
        Assert.Equal(before, box.Text);
    }
}
