namespace System.Windows.Forms.Collection;

public class BindingTests
{
    [Fact]
    public void BindingComplete_Event_Is_Invoked_By_OnBindingComplete()
    {
        var dataSource = new object();
        var b = new TestBinding("P", dataSource, null, true);

        var invoked = false;
        BindingCompleteEventArgs? received = null;
        b.BindingComplete += (s, e) =>
        {
            invoked = true;
            received = e;
        };

        var args = new BindingCompleteEventArgs(b, BindingCompleteState.Success, BindingCompleteContext.ControlUpdate);
        b.InvokeOnBindingComplete(args);

        Assert.True(invoked);
        Assert.Same(args, received);
    }

    [Fact]
    public void Constructor_Sets_PropertyName_And_DataSource_And_BindingMemberInfo()
    {
        var dataSource = new object();
        var b = new TestBinding("Text", dataSource, "Member");

        Assert.Equal("Text", b.PropertyName);
        Assert.Same(dataSource, b.DataSource);
        Assert.NotNull(b.BindingMemberInfo);
    }

    [Fact]
    public void Default_Constructor_Disables_Formatting()
    {
        var dataSource = new object();
        // The 3-arg constructor routes to the overload that sets formattingEnabled = false
        var b = new TestBinding("P", dataSource, null);

        Assert.False(b.FormattingEnabled);
    }

    [Fact]
    public void Format_Event_Is_Invoked_By_OnFormat()
    {
        var dataSource = new object();
        var b = new TestBinding("P", dataSource, null, true);

        var invoked = false;
        ConvertEventArgs received = null!;
        b.Format += (s, e) =>
        {
            invoked = true;
            received = e;
        };

        var args = new ConvertEventArgs("123", typeof(string));
        b.InvokeOnFormat(args);

        Assert.True(invoked);
        Assert.Same(args, received);
    }

    [Fact]
    public void Overload_Allows_Enabling_Formatting()
    {
        var dataSource = new object();
        var b = new TestBinding("P", dataSource, null, true);

        Assert.True(b.FormattingEnabled);
    }

    [Fact]
    public void Parse_Event_Is_Invoked_By_OnParse()
    {
        var dataSource = new object();
        var b = new TestBinding("P", dataSource, null, true);

        var invoked = false;
        ConvertEventArgs received = null!;
        b.Parse += (s, e) =>
        {
            invoked = true;
            received = e;
        };

        var args = new ConvertEventArgs("456", typeof(int));
        b.InvokeOnParse(args);

        Assert.True(invoked);
        Assert.Same(args, received);
    }

    private class TestBinding : Binding
    {
        public TestBinding(string propertyName, object dataSource, string? dataMember)
            : base(propertyName, dataSource, dataMember)
        {
        }

        public TestBinding(string propertyName, object dataSource, string? dataMember, bool formattingEnabled)
            : base(propertyName, dataSource, dataMember, formattingEnabled)
        {
        }

        public void InvokeOnBindingComplete(BindingCompleteEventArgs e) => OnBindingComplete(e);

        // Expose protected behavior for testing
        public void InvokeOnFormat(ConvertEventArgs e) => OnFormat(e);
        public void InvokeOnParse(ConvertEventArgs e) => OnParse(e);
    }
}