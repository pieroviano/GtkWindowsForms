using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Sweeps the <c>OnXxx</c> raisers of <see cref="ToolStripItem"/> (via
/// <see cref="ToolStripButton"/>) and asserts each raises its same-named event exactly once with the
/// item as sender — the same contract already proven for <see cref="Control"/> and
/// <see cref="DataGridView"/>.</summary>
/// <remarks>Discovery must not construct GTK widgets, so the theory data is plain strings and the
/// item is created inside the test body.</remarks>
[Collection("NonParallelCollection")]
public class ToolStripItemEventRaisingFunctionalTests
{
    private static IEnumerable<MethodInfo> Raisers()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        foreach (var method in typeof(ToolStripButton).GetMethods(flags))
        {
            if (!method.Name.StartsWith("On", StringComparison.Ordinal) || method.Name.Length <= 2)
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(EventArgs))
            {
                continue;
            }

            if (typeof(ToolStripButton).GetEvent(method.Name.Substring(2), flags) == null)
            {
                continue;
            }

            yield return method;
        }
    }

    public static TheoryData<string> RaiserNames()
    {
        var data = new TheoryData<string>();
        foreach (var raiser in Raisers())
        {
            data.Add(raiser.Name);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(RaiserNames))]
    public void EachRaiserRaisesItsEventExactlyOnce(string raiserName)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        using var item = new ToolStripButton();

        var raiser = Raisers().First(m => m.Name == raiserName);
        var eventInfo = typeof(ToolStripButton).GetEvent(raiserName.Substring(2), flags)!;

        var fired = 0;
        object? sender = null;

        var invoke = eventInfo.EventHandlerType!.GetMethod("Invoke")!;
        var senderParam = Expression.Parameter(invoke.GetParameters()[0].ParameterType, "s");
        var argsParam = Expression.Parameter(invoke.GetParameters()[1].ParameterType, "a");
        Action<object?> record = s => { fired++; sender = s; };
        var body = Expression.Invoke(Expression.Constant(record),
            Expression.Convert(senderParam, typeof(object)));
        var handler = Expression.Lambda(eventInfo.EventHandlerType!, body, senderParam, argsParam).Compile();

        eventInfo.AddEventHandler(item, handler);
        try
        {
            raiser.Invoke(item, new object[] { EventArgs.Empty });
        }
        catch (TargetInvocationException ex)
        {
            throw new Xunit.Sdk.XunitException(
                $"{raiserName} threw {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
        }

        Assert.True(fired == 1, $"{raiserName} should raise {eventInfo.Name} exactly once but raised it {fired} times");
        Assert.Same(item, sender);
    }

    [Fact]
    public void TheSweepCoversAMeaningfulShareOfTheRaisers()
    {
        Assert.True(Raisers().Count() >= 10,
            $"expected at least 10 EventArgs raisers on ToolStripItem but found {Raisers().Count()}");
    }
}
