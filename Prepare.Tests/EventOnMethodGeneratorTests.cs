using XmlDocSync.Roslyn;

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

        Assert.Contains("protected virtual void OnMyClick(System.EventArgs e)", result);
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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

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

        var result = EventOnMethodGenerator.Process(source, EventOnMethodGenerator.DefaultMap);

        Assert.Contains("System.EventArgs", result);
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
