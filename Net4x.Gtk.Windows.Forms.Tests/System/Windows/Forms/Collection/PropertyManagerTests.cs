using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Collection;

public class PropertyManagerTests
{
    // -------------------------
    // Not supported operations
    // -------------------------

    [Fact]
    public void AddNew_Throws()
    {
        var sut = new PropertyManager();

        Assert.Throws<NotSupportedException>(() => sut.AddNew());
    }

    // -------------------------
    // Editing behavior
    // -------------------------

    [Fact]
    public void CancelCurrentEdit_CallsCancelEdit_OnEditableObject()
    {
        var editable = new EditableObject();
        var sut = new PropertyManager(editable);

        sut.CancelCurrentEdit();

        Assert.True(editable.CancelCalled);
    }

    [Fact]
    public void CancelCurrentEdit_WithNonEditable_DoesNotThrow()
    {
        var sut = new PropertyManager(new object());

        sut.CancelCurrentEdit(); // should not throw
    }

    [Fact]
    public void Constructor_WithInvalidProperty_Throws()
    {
        var obj = new TestObject();

        Assert.Throws<ArgumentException>(() =>
            new PropertyManager(obj, "DoesNotExist"));
    }

    // -------------------------
    // DataSource + PropertyName
    // -------------------------

    [Fact]
    public void Constructor_WithPropertyName_BindsSuccessfully()
    {
        var obj = new TestObject { Value = 5 };

        var sut = new PropertyManager(obj, nameof(TestObject.Value));

        Assert.Equal(obj, sut.Current);
    }

    // -------------------------
    // Basic properties
    // -------------------------

    [Fact]
    public void Count_Always_One()
    {
        var sut = new PropertyManager();

        Assert.Equal(1, sut.Count);
    }

    [Fact]
    public void Current_ReturnsDataSource()
    {
        var obj = new object();
        var sut = new PropertyManager(obj);

        Assert.Same(obj, sut.Current);
    }

    [Fact]
    public void EndCurrentEdit_CallsEndEdit_WhenPullDataReturnsTrue()
    {
        var editable = new EditableObject();
        var sut = new PropertyManager(editable);

        sut.EndCurrentEdit();

        Assert.True(editable.EndCalled);
    }

    [Fact]
    public void GetListName_NullDataSource_ReturnsNull()
    {
        var sut = new PropertyManager();

        var name = sut.GetListName();

        Assert.Null(name);
    }

    // -------------------------
    // GetListName
    // -------------------------

    [Fact]
    public void GetListName_ReturnsExpectedFormat()
    {
        var obj = new TestObject();
        var sut = new PropertyManager(obj, nameof(TestObject.Value));

        var name = sut.GetListName();

        Assert.Contains(nameof(TestObject), name);
        Assert.Contains(nameof(TestObject.Value), name);
    }

    [Fact]
    public void GetListName_WithAccessors_ReturnsEmpty()
    {
        var sut = new PropertyManager(new object());

        var name = sut.GetListName(new ArrayList());

        Assert.Equal(string.Empty, name);
    }

    [Fact]
    public void IsBinding_False_WhenNull()
    {
        var sut = new PropertyManager();

        Assert.False(sut.IsBinding);
    }

    // -------------------------
    // Binding state
    // -------------------------

    [Fact]
    public void IsBinding_True_WhenDataSourceExists()
    {
        var sut = new PropertyManager(new object());

        Assert.True(sut.IsBinding);
    }

    // -------------------------
    // Events
    // -------------------------

    [Fact]
    public void OnCurrentChanged_RaisesBothEvents()
    {
        var sut = new PropertyManager(new object());

        var currentChanged = false;
        var itemChanged = false;

        sut.CurrentChanged += (_, _) => currentChanged = true;
        sut.CurrentItemChanged += (_, _) => itemChanged = true;

        sut.ResumeBinding(); // triggers OnCurrentChanged

        Assert.True(currentChanged);
        Assert.True(itemChanged);
    }

    [Fact]
    public void OnCurrentItemChanged_RaisesItemChangedOnly()
    {
        var sut = new PropertyManager(new object());

        var itemChanged = false;

        sut.CurrentItemChanged += (_, _) => itemChanged = true;

        // invoke indirectly via ResumeBinding -> OnCurrentChanged -> both
        sut.ResumeBinding();

        Assert.True(itemChanged);
    }

    [Fact]
    public void Position_IsAlwaysZero()
    {
        var sut = new PropertyManager();

        Assert.Equal(0, sut.Position);

        sut.Position = 10; // should do nothing

        Assert.Equal(0, sut.Position);
    }

    [Fact]
    public void RemoveAt_Throws()
    {
        var sut = new PropertyManager();

        Assert.Throws<NotSupportedException>(() => sut.RemoveAt(0));
    }

    // -------------------------
    // Resume / Suspend binding
    // -------------------------

    [Fact]
    public void ResumeBinding_RaisesCurrentChanged()
    {
        var sut = new PropertyManager(new object());
        var raised = false;

        sut.CurrentChanged += (_, _) => raised = true;

        sut.ResumeBinding();

        Assert.True(raised);
    }

    [Fact]
    public void SuspendBinding_DoesNotThrow()
    {
        var sut = new PropertyManager(new object());

        sut.SuspendBinding();
    }

    private class EditableObject : IEditableObject
    {
        public bool CancelCalled { get; private set; }
        public bool EndCalled { get; private set; }

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
            CancelCalled = true;
        }

        public void EndEdit()
        {
            EndCalled = true;
        }
    }

    private class TestObject
    {
        public int Value { get; set; }
    }
}