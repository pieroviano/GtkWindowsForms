using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class EventRewriterTests
{
    private string Rewrite(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();

        var rewriter = new EventRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    [Fact]
    public void AddsVirtualToPublicEvent_InAbstractClass()
    {
        var input = @"
public abstract class TestClass
{
    public event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        Assert.Contains("public virtual event System.EventHandler MyEvent;", output);
    }

    [Fact]
    public void DoesNotAddsVirtualToOverrideEvent_InClass()
    {
        var input = @"
public class TestClass: BaseClass
{
    public override event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        Assert.DoesNotContain("virtual", output);
    }

    [Fact]
    public void AddsVirtualToPublicEvent_InNonSealedClass()
    {
        var input = @"
public class TestClass
{
    public event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        Assert.Contains("public virtual event System.EventHandler MyEvent;", output);
    }

    [Fact]
    public void DoesNotModifySealedClass()
    {
        var input = @"
public sealed class TestClass
{
    public event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        Assert.DoesNotContain("virtual event", output);
    }

    [Fact]
    public void DoesNotModifyNonPublicEvent()
    {
        var input = @"
public class TestClass
{
    private event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        Assert.DoesNotContain("virtual event", output);
    }

    [Fact]
    public void DoesNotDuplicateVirtualKeyword()
    {
        var input = @"
public class TestClass
{
    public virtual event System.EventHandler MyEvent;
}";

        var output = Rewrite(input);

        var count = output.Split("virtual").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void LeavesClassWithoutEventsUnchanged()
    {
        var input = @"
public class TestClass
{
    public void DoSomething() {}
}";

        var output = Rewrite(input);

        Assert.DoesNotContain("virtual", output);
    }
}