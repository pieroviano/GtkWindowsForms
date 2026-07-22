using System.Reflection;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Every <c>OnXxx</c> method on <see cref="Control"/> exists to raise the matching <c>Xxx</c> event;
/// the prepare pipeline even has a Roslyn rewrite that enforces it. These tests hold that contract for the
/// whole family at once, and pin down the argument-carrying ones individually.</summary>
[Collection("NonParallelCollection")]
public class ControlEventRaisingFunctionalTests
{
    /// <summary>Exposes the protected raisers so a test can call them.</summary>
    private sealed class ProbeControl : Panel
    {
        public void Raise(MethodInfo raiser, object args) => raiser.Invoke(this, new[] { args });
    }

    /// <summary>
    /// The raisers that cannot be driven from a bare unit test, with the reason. Everything else must fire.
    /// </summary>
    private static readonly Dictionary<string, string> NotDrivable = new()
    {
        ["OnPaint"] = "needs a live Cairo context supplied by the GTK draw signal",
        ["OnPaintBackground"] = "needs a live Cairo context supplied by the GTK draw signal"
    };

    /// <summary>Every protected OnXxx(EventArgs) raiser paired with the event it must raise.</summary>
    public static TheoryData<string> EventArgsRaisers()
    {
        var data = new TheoryData<string>();
        foreach (var name in RaiserNames())
        {
            data.Add(name);
        }

        return data;
    }

    private static IEnumerable<string> RaiserNames()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        foreach (var method in typeof(Control).GetMethods(flags))
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

            if (NotDrivable.ContainsKey(method.Name))
            {
                continue;
            }

            // Only those that have a same-named event to raise.
            if (typeof(Control).GetEvent(method.Name.Substring(2), flags) == null)
            {
                continue;
            }

            yield return method.Name;
        }
    }

    [Theory]
    [MemberData(nameof(EventArgsRaisers))]
    public void EachOnMethodRaisesItsEvent(string raiserName)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var control = new ProbeControl();
        var raiser = typeof(Control).GetMethod(raiserName, flags, null, new[] { typeof(EventArgs) }, null);
        Assert.NotNull(raiser);

        var eventInfo = typeof(Control).GetEvent(raiserName.Substring(2), flags);
        Assert.NotNull(eventInfo);

        var fired = 0;
        var handler = new EventHandler((_, _) => fired++);
        eventInfo!.AddEventHandler(control, handler);

        try
        {
            control.Raise(raiser!, EventArgs.Empty);
        }
        catch (TargetInvocationException ex)
        {
            throw new Xunit.Sdk.XunitException(
                $"{raiserName} threw {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
        }

        Assert.True(fired == 1, $"{raiserName} should raise {eventInfo.Name} exactly once but raised it {fired} times");
    }

    [Fact]
    public void TheSweepCoversAMeaningfulNumberOfRaisers()
    {
        // Guards against the reflection filter silently matching nothing and the theory passing vacuously.
        Assert.True(RaiserNames().Count() >= 25,
            $"expected the OnXxx sweep to cover at least 25 raisers but found {RaiserNames().Count()}");
    }

    [Fact]
    public void RemovingTheHandlerStopsTheEvent()
    {
        var control = new ProbeControl();
        var fired = 0;
        void Handler(object? sender, EventArgs e) => fired++;

        control.Click += Handler;
        control.Click -= Handler;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnClick", flags, null, new[] { typeof(EventArgs) }, null);
        control.Raise(raiser!, EventArgs.Empty);

        Assert.Equal(0, fired);
    }

    [Fact]
    public void TheSenderIsTheControlThatRaisedTheEvent()
    {
        var control = new ProbeControl();
        object? sender = null;
        control.Click += (s, _) => sender = s;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnClick", flags, null, new[] { typeof(EventArgs) }, null);
        control.Raise(raiser!, EventArgs.Empty);

        Assert.Same(control, sender);
    }

    [Fact]
    public void SeveralHandlersOnOneEventAllRun()
    {
        var control = new ProbeControl();
        var first = 0;
        var second = 0;
        control.Click += (_, _) => first++;
        control.Click += (_, _) => second++;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnClick", flags, null, new[] { typeof(EventArgs) }, null);
        control.Raise(raiser!, EventArgs.Empty);

        Assert.Equal(1, first);
        Assert.Equal(1, second);
    }

    [Fact]
    public void OnKeyDown_PassesTheKeyDataThrough()
    {
        var control = new ProbeControl();
        KeyEventArgs? received = null;
        control.KeyDown += (_, e) => received = e;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnKeyDown", flags, null, new[] { typeof(KeyEventArgs) }, null);
        raiser!.Invoke(control, new object[] { new KeyEventArgs(Keys.Control | Keys.S) });

        Assert.NotNull(received);
        Assert.Equal(Keys.Control | Keys.S, received!.KeyData);
        Assert.True(received.Control, "the Control modifier should be reported");
    }

    [Fact]
    public void OnKeyPress_PassesTheCharacterThrough()
    {
        var control = new ProbeControl();
        KeyPressEventArgs? received = null;
        control.KeyPress += (_, e) => received = e;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnKeyPress", flags, null, new[] { typeof(KeyPressEventArgs) }, null);
        raiser!.Invoke(control, new object[] { new KeyPressEventArgs('q') });

        Assert.NotNull(received);
        Assert.Equal('q', received!.KeyChar);
    }

    [Fact]
    public void OnMouseDown_PassesTheButtonAndPositionThrough()
    {
        var control = new ProbeControl();
        MouseEventArgs? received = null;
        control.MouseDown += (_, e) => received = e;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnMouseDown", flags, null, new[] { typeof(MouseEventArgs) }, null);
        raiser!.Invoke(control, new object[] { new MouseEventArgs(MouseButtons.Right, 2, 30, 40, 0) });

        Assert.NotNull(received);
        Assert.Equal(MouseButtons.Right, received!.Button);
        Assert.Equal(2, received.Clicks);
        Assert.Equal(30, received.X);
        Assert.Equal(40, received.Y);
    }

    [Fact]
    public void OnControlAdded_ReportsTheChildThatWasAdded()
    {
        var control = new ProbeControl();
        Control? added = null;
        control.ControlAdded += (_, e) => added = e.Control;

        var child = new Button();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnControlAdded", flags, null, new[] { typeof(ControlEventArgs) }, null);
        raiser!.Invoke(control, new object[] { new ControlEventArgs(child) });

        Assert.Same(child, added);
    }

    [Fact]
    public void OnControlRemoved_ReportsTheChildThatWasRemoved()
    {
        var control = new ProbeControl();
        Control? removed = null;
        control.ControlRemoved += (_, e) => removed = e.Control;

        var child = new Button();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var raiser = typeof(Control).GetMethod("OnControlRemoved", flags, null, new[] { typeof(ControlEventArgs) }, null);
        raiser!.Invoke(control, new object[] { new ControlEventArgs(child) });

        Assert.Same(child, removed);
    }
}
