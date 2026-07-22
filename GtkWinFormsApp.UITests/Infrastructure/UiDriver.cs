using System.Reflection;
using System.Windows.Forms;

namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// Drives a real sample form at the WinForms API level (SampleUITests-Plan.md §3.1). The form is shown
/// NON-modally with <see cref="Form.Show()"/> and the GTK loop is pumped with
/// <see cref="Application.DoEvents"/>, so all interaction and assertion happen on the test thread rather
/// than inside a GTK signal callback. That is what keeps an assertion failure a normal xUnit failure
/// instead of an <c>Exception in Gtk# callback delegate</c> that would crash the test host natively.
/// </summary>
internal static class UiDriver
{
    /// <summary>
    /// Constructs <paramref name="create"/>, shows it, runs <paramref name="script"/> against it, then
    /// always tears the form (and any windows it spawned) down. Exceptions from the script propagate.
    /// </summary>
    internal static void Run<TForm>(Func<TForm> create, Action<UiContext> script) where TForm : Form
    {
        UiApp.EnsureInitialized();

        TForm? form = null;
        try
        {
            form = create();
            form.Show();
            Pump(20); // let GTK realize + map + do the first layout/paint pass
            script(new UiContext(form));
        }
        finally
        {
            TearDown(form);
        }
    }

    /// <summary>Pumps the GTK main loop <paramref name="iterations"/> times so pending layout/paint/signals settle.</summary>
    internal static void Pump(int iterations = 12)
    {
        for (var i = 0; i < iterations; i++)
        {
            Application.DoEvents();
        }
    }

    /// <summary>
    /// Raises a control's protected event method (e.g. <c>OnClick</c>). The library exposes no public
    /// <c>PerformClick</c>; invoking the protected raiser runs the real event → wired-handler path
    /// (the sample wires handlers with <c>button.Click += …</c>), which is what a click would trigger.
    /// </summary>
    internal static void RaiseProtected(object target, string methodName)
    {
        var mi = FindEventRaiser(target.GetType(), methodName)
                 ?? throw new MissingMethodException(
                     $"{target.GetType().Name} has no accessible {methodName}(EventArgs).");
        mi.Invoke(target, new object[] { EventArgs.Empty });
    }

    private static MethodInfo? FindEventRaiser(Type? type, string name)
    {
        while (type != null)
        {
            var mi = type.GetMethod(
                name,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                binder: null,
                types: new[] { typeof(EventArgs) },
                modifiers: null);
            if (mi != null)
            {
                return mi;
            }

            type = type.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Closes the form and destroys every remaining top-level GTK window, so a form that opened a child
    /// window (e.g. Form3 → Test1) does not leak realized windows into the next test.
    /// </summary>
    private static void TearDown(Form? form)
    {
        if (form != null)
        {
            try
            {
                form.Close();
            }
            catch
            {
                // best-effort teardown
            }
        }

        Pump(8);

        foreach (var toplevel in Gtk.Window.ListToplevels())
        {
            try
            {
                if (toplevel.IsRealized)
                {
                    toplevel.Destroy();
                }
            }
            catch
            {
                // best-effort teardown
            }
        }

        Pump(4);
    }
}
