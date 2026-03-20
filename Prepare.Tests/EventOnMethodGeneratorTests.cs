using XmlDocSync.Services;

namespace Prepare.Tests;

public class EventOnMethodGeneratorTests
{
    [Fact]
    public void Generates_OnMethod_For_Public_Event()
    {
        var source = @"
using System;

public class MyButton
{
    public event EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.Contains("protected virtual void OnMyClick(EventArgs e)", result);
        Assert.Contains("MyClick?.Invoke(this, e);", result);
    }

    [Fact]
    public void Generates_OnMethod_For_Public_Virtual_Event()
    {
        var source = @"
using System;

public class MyButton
{
    public virtual event EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.Contains("OnMyClick", result);
    }

    [Fact]
    public void Does_Not_Generate_For_Static_Event()
    {
        var source = @"
using System;

public class MyButton
{
    public static event EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.DoesNotContain("OnMyClick", result);
    }

    [Fact]
    public void Does_Not_Generate_For_Private_Event()
    {
        var source = @"
using System;

public class MyButton
{
    private event EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.DoesNotContain("OnMyClick", result);
    }

    [Fact]
    public void Skips_When_OnMethod_Already_Exists()
    {
        var source = @"
using System;

public class MyButton
{
    public event EventHandler MyClick;

    protected virtual void OnMyClick(System.EventArgs e)
    {
    }
}";

        var result = EventOnMethodGenerator.Process(source, null);

        var count = CountOccurrences(result, "OnMyClick");

        Assert.Equal(1, count);
    }

    [Fact]
    public void Handles_Multiple_Events()
    {
        var source = @"
using System;

public class MyButton
{
    public event EventHandler Click;
    public event EventHandler DoubleClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.Contains("OnClick", result);
        Assert.Contains("OnDoubleClick", result);
    }

    [Fact]
    public void Works_With_Fully_Qualified_Handler_Name()
    {
        var source = @"
public class MyButton
{
    public event System.EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, null);

        Assert.Contains("OnMyClick", result);
    }

    [Fact]
    public void Resolves_Handler_By_Short_Name()
    {
        var source = @"
using System;

public class MyButton
{
    public event EventHandler MyClick;
}";

        var result = EventOnMethodGenerator.Process(source, [], new Dictionary<string, string>()
        {
            { "System.EventHandler", "System.EventArgs" }
        });

        Assert.Contains("System.EventArgs", result);
    }

    [Fact]
    public void Generates_Missing_Events_From_OnMethods()
    {
        var source = @"
using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripDropDownMenu : ToolStripDropDown
{
    public ToolStripDropDownMenu()
    {
    }

    protected void OnFontChanged(EventArgs e)
    {
        FontChanged?.Invoke(this, e);
    }

    protected void OnLayout(LayoutEventArgs e)
    {
        Layout?.Invoke(this, e);
    }

    protected void OnPaintBackground(PaintEventArgs e)
    {
        PaintBackground?.Invoke(this, e);
    }
}";

        var result = EventOnMethodGenerator.Process(source, null);

        // Verify that the three missing events were generated
        Assert.Contains("public event EventHandler<EventArgs> FontChanged", result);
        Assert.Contains("public event EventHandler<LayoutEventArgs> Layout", result);
        Assert.Contains("public event EventHandler<PaintEventArgs> PaintBackground", result);
    }

    [Fact]
    public void Does_Not_Generate_Event_When_Event_Already_Exists()
    {
        var source = @"
using System;

public class MyButton
{
    public event EventHandler MyClick;

    protected void OnMyClick(EventArgs e)
    {
        MyClick?.Invoke(this, e);
    }
}";

        var result = EventOnMethodGenerator.Process(source, null);

        // Should only have one MyClick event (the original one)
        var count = CountOccurrences(result, "event EventHandler");
        Assert.Equal(1, count);
    }

    [Fact]
    public void Does_Not_Duplicate_Event_When_Custom_Accessor_Exists()
    {
        var source = @"
using System;

public class MyControl
{
    private EventHandler? _myEvent;

    public event EventHandler? MyEvent
    {
        add => _myEvent += value;
        remove => _myEvent -= value;
    }

    protected virtual void OnMyEvent(EventArgs e)
    {
        _myEvent?.Invoke(this, e);
    }
}";

        var result = EventOnMethodGenerator.Process(source, null);

        // Should only have one MyEvent declaration (the custom accessor one)
        var eventCount = CountOccurrences(result, "public event EventHandler");
        Assert.Equal(1, eventCount);

        // Should not have a field-like event declaration
        Assert.DoesNotContain("public virtual event EventHandler", result);
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}

