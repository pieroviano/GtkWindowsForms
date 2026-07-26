using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms.DataGridViewTests;

/// <summary>Every <c>OnXxx</c> raiser on <see cref="DataGridView"/> exists to raise the matching event —
/// the same contract already held for <see cref="Control"/>, and one the prepare pipeline's Roslyn rewrite
/// is supposed to enforce. The grid declares ~116 raisers over a handful of argument shapes, so this sweep
/// drives every raiser whose argument it can construct and asserts the matching event fires exactly once
/// with the grid as sender.</summary>
/// <remarks>Discovery must not construct GTK widgets (a MemberData that does crashes the test host), so
/// the theory data is derived from parameter *types* only; the grid is created inside the test body.</remarks>
[Collection("NonParallelCollection")]
public class DataGridViewEventRaisingFunctionalTests
{
    /// <summary>The argument shapes the sweep knows how to construct.</summary>
    private static readonly HashSet<string> SupportedArgTypes = new()
    {
        nameof(EventArgs),
        nameof(DataGridViewCellEventArgs),
        nameof(DataGridViewColumnEventArgs),
        nameof(DataGridViewRowEventArgs),
        nameof(DataGridViewCellMouseEventArgs)
    };

    private static object ArgumentFor(Type type, DataGridView grid) => type.Name switch
    {
        nameof(EventArgs) => EventArgs.Empty,
        nameof(DataGridViewCellEventArgs) => new DataGridViewCellEventArgs(0, 0),
        nameof(DataGridViewColumnEventArgs) => new DataGridViewColumnEventArgs(grid.Columns[0]),
        nameof(DataGridViewRowEventArgs) => new DataGridViewRowEventArgs(grid.Rows[0]),
        nameof(DataGridViewCellMouseEventArgs) => new DataGridViewCellMouseEventArgs(
            0, 0, 1, 1, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0)),
        _ => throw new InvalidOperationException($"No factory for {type.Name}")
    };

    private static IEnumerable<MethodInfo> Raisers()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        foreach (var method in typeof(DataGridView).GetMethods(flags))
        {
            if (!method.Name.StartsWith("On", StringComparison.Ordinal) || method.Name.Length <= 2)
            {
                continue;
            }

            if (method.GetParameters().Length != 1)
            {
                continue;
            }

            // Only raisers with a same-named event on the grid are part of the contract.
            if (typeof(DataGridView).GetEvent(method.Name.Substring(2), flags) == null)
            {
                continue;
            }

            yield return method;
        }
    }

    /// <summary>Raiser names paired with their argument type — plain strings, no widgets at discovery.</summary>
    public static TheoryData<string, string> RaiserNames()
    {
        var data = new TheoryData<string, string>();
        foreach (var raiser in Raisers())
        {
            var argType = raiser.GetParameters()[0].ParameterType.Name;
            if (SupportedArgTypes.Contains(argType))
            {
                data.Add(raiser.Name, argType);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(RaiserNames))]
    public void EachRaiserRaisesItsEventExactlyOnce(string raiserName, string argTypeName)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");

        var raiser = Raisers().First(m =>
            m.Name == raiserName && m.GetParameters()[0].ParameterType.Name == argTypeName);
        var eventInfo = typeof(DataGridView).GetEvent(raiserName.Substring(2), flags)!;

        var fired = 0;
        object? sender = null;

        // Build a handler of the exact delegate type: (object s, TArgs a) => record(s).
        var invoke = eventInfo.EventHandlerType!.GetMethod("Invoke")!;
        var senderParam = Expression.Parameter(invoke.GetParameters()[0].ParameterType, "s");
        var argsParam = Expression.Parameter(invoke.GetParameters()[1].ParameterType, "a");
        Action<object?> record = s => { fired++; sender = s; };
        var body = Expression.Invoke(Expression.Constant(record),
            Expression.Convert(senderParam, typeof(object)));
        var handler = Expression.Lambda(eventInfo.EventHandlerType!, body, senderParam, argsParam).Compile();

        eventInfo.AddEventHandler(grid, handler);
        try
        {
            raiser.Invoke(grid, new[] { ArgumentFor(raiser.GetParameters()[0].ParameterType, grid) });
        }
        catch (TargetInvocationException ex)
        {
            throw new Xunit.Sdk.XunitException(
                $"{raiserName} threw {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
        }

        Assert.True(fired == 1, $"{raiserName} should raise {eventInfo.Name} exactly once but raised it {fired} times");
        Assert.Same(grid, sender);
    }

    [Fact]
    public void TheSweepCoversAMeaningfulShareOfTheRaisers()
    {
        // Guards against the argument factory silently matching nothing and the theory passing vacuously.
        var covered = Raisers().Count(r => SupportedArgTypes.Contains(r.GetParameters()[0].ParameterType.Name));

        Assert.True(covered >= 60,
            $"expected the sweep to reach at least 60 of the grid's raisers but only {covered} have constructible arguments");
    }
}
