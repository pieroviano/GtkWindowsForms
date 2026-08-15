using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace SampleParity;

/// <summary>
/// Supplies the sample's forms. Implemented per harness, because the two samples live in different
/// namespaces (<c>GTKWinFormsApp</c> and <c>GtkWinFormsApp</c>) and link different libraries, so the shared
/// catalogue cannot name their types directly.
/// </summary>
internal interface ISampleForms
{
    /// <summary>Gets the logical form names this sample provides.</summary>
    IEnumerable<string> Names { get; }

    /// <summary>Creates a form by logical name.</summary>
    /// <param name="logicalName">One of <see cref="Names" />.</param>
    /// <returns>The form, or <see langword="null" /> when this sample has no such form.</returns>
    Form? Create(string logicalName);

    /// <summary>
    /// Arranges for modal dialogs to dismiss themselves, so an unattended run cannot deadlock.
    /// </summary>
    /// <remarks>
    /// Sample handlers pop message boxes a manual tester would click away - <c>Form2</c>'s
    /// <c>FormClosing</c> is the one that bites, because teardown always triggers it. How the dialog is
    /// defused differs per engine, which is why this is the harness's job and not the catalogue's: the Gtk4
    /// library exposes a replaceable auto-closing MessageBox, while upstream's is a plain static class that
    /// blocks in <c>Gtk.Dialog.Run()</c> and can only be dismissed from a timer.
    /// </remarks>
    void InstallModalGuard();
}

/// <summary>
/// The surface a scenario body drives: input verbs, and the observation verbs that append to the trace.
/// </summary>
internal sealed class ScenarioContext
{
    private readonly TraceWriter _trace;
    private readonly string _scenario;
    private int _step;

    internal ScenarioContext(string scenario, Form form, TraceWriter trace)
    {
        _scenario = scenario;
        Form = form;
        _trace = trace;
    }

    /// <summary>Gets the form under test.</summary>
    public Form Form { get; }

    /// <summary>Pumps the GTK main loop so pending layout, paint and signals settle.</summary>
    /// <param name="iterations">How many times to pump.</param>
    public void Pump(int iterations = 12)
    {
        for (var i = 0; i < iterations; i++)
        {
            Application.DoEvents();
        }
    }

    /// <summary>Records an observation of control state.</summary>
    /// <param name="control">The logical control name.</param>
    /// <param name="property">The property observed.</param>
    /// <param name="value">The value; normalise text through <see cref="Text" /> first.</param>
    public void State(string control, string property, string value)
    {
        _trace.Add(_scenario, _step++, TraceKind.State, control, property, value);
    }

    /// <summary>Records a geometry invariant.</summary>
    /// <param name="control">The logical control name.</param>
    /// <param name="invariant">The invariant's name.</param>
    /// <param name="holds">Whether it holds.</param>
    /// <remarks>
    /// Invariants, not pixel counts: Gtk3 and Gtk4 ship different themes and metrics, so equal numbers are
    /// not achievable across engines and would only produce noise.
    /// </remarks>
    public void Geometry(string control, string invariant, bool holds)
    {
        _trace.Add(_scenario, _step++, TraceKind.Geometry, control, invariant,
            holds ? "true" : "false");
    }

    /// <summary>Records that a window was opened or closed.</summary>
    /// <param name="what">What happened, e.g. <c>opened</c>.</param>
    /// <param name="count">The resulting top-level window count.</param>
    public void Window(string what, int count)
    {
        _trace.Add(_scenario, _step++, TraceKind.Window, Form.GetType().Name, what,
            count.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Records that the scenario could not run as written.</summary>
    /// <param name="control">The control involved.</param>
    /// <param name="detail">What went wrong.</param>
    public void Error(string control, string detail)
    {
        _trace.Add(_scenario, _step++, TraceKind.Error, control, "error", detail);
    }

    /// <summary>
    /// Normalises a caption for cross-engine comparison.
    /// </summary>
    /// <param name="before">The text before the interaction.</param>
    /// <param name="after">The text after it.</param>
    /// <returns>A stable token describing the change rather than the literal text.</returns>
    /// <remarks>
    /// The reference sample renders Chinese and the Gtk4 sample English, so literal captions differ by
    /// design. What is comparable is whether an interaction changed the text, and whether it became empty.
    /// </remarks>
    public static string Text(string? before, string? after)
    {
        if (string.Equals(before, after, StringComparison.Ordinal))
        {
            return "<text:unchanged>";
        }

        return string.IsNullOrEmpty(after) ? "<text:cleared>" : "<text:changed>";
    }

    /// <summary>Finds a control, recording an error and returning null when it is missing.</summary>
    /// <param name="address">A <c>name:</c> address.</param>
    /// <returns>The control, or <see langword="null" />.</returns>
    public Control? Control(string address)
    {
        var control = ControlLocator.FindControl(Form, address);
        if (control == null)
        {
            Error(address, "control not found");
        }

        return control;
    }

    /// <summary>Finds a strip item, recording an error and returning null when it is missing.</summary>
    /// <param name="address">A <c>name:</c> or <c>path:</c> address.</param>
    /// <returns>The item, or <see langword="null" />.</returns>
    public ToolStripItem? Item(string address)
    {
        var item = ControlLocator.FindItem(Form, address);
        if (item == null)
        {
            Error(address, "item not found");
        }

        return item;
    }

    /// <summary>
    /// Clicks a control by raising its wired <c>OnClick</c>, then pumps.
    /// </summary>
    /// <param name="address">A <c>name:</c> address.</param>
    /// <remarks>
    /// The library exposes no public <c>PerformClick</c> on every control, so the protected raiser is
    /// invoked - which runs the real event to wired-handler path, exactly what a click triggers.
    /// </remarks>
    public void Click(string address)
    {
        var control = Control(address);
        if (control == null)
        {
            return;
        }

        RaiseProtected(control, "OnClick");
        Pump();
    }

    /// <summary>Clicks a strip item through its public <c>PerformClick</c>, then pumps.</summary>
    /// <param name="address">A <c>name:</c> or <c>path:</c> address.</param>
    public void ClickItem(string address)
    {
        var item = Item(address);
        if (item == null)
        {
            return;
        }

        item.PerformClick();
        Pump();
    }

    /// <summary>Invokes a protected event raiser such as <c>OnClick</c>.</summary>
    /// <param name="target">The control.</param>
    /// <param name="methodName">The raiser name.</param>
    internal static void RaiseProtected(object target, string methodName)
    {
        var method = FindRaiser(target.GetType(), methodName);
        method?.Invoke(target, new object[] { EventArgs.Empty });
    }

    private static MethodInfo? FindRaiser(Type? type, string name)
    {
        for (var t = type; t != null; t = t.BaseType)
        {
            var method = t.GetMethod(
                name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                null,
                new[] { typeof(EventArgs) },
                null);

            if (method != null)
            {
                return method;
            }
        }

        return null;
    }
}

/// <summary>One scenario: an id, the form it runs against, and the body that drives it.</summary>
internal sealed class Scenario
{
    /// <summary>Initialises a new instance of the <see cref="Scenario" /> class.</summary>
    /// <param name="id">The stable scenario id, used to match traces across engines.</param>
    /// <param name="form">The logical form name.</param>
    /// <param name="body">What the scenario does.</param>
    /// <param name="requiresPointer">Whether it needs real pointer input.</param>
    public Scenario(string id, string form, Action<ScenarioContext> body, bool requiresPointer = false)
    {
        Id = id;
        Form = form;
        Body = body;
        RequiresPointer = requiresPointer;
    }

    /// <summary>Gets the scenario id.</summary>
    public string Id { get; }

    /// <summary>Gets the logical form name.</summary>
    public string Form { get; }

    /// <summary>Gets the scenario body.</summary>
    public Action<ScenarioContext> Body { get; }

    /// <summary>Gets a value indicating whether this scenario needs real pointer input.</summary>
    public bool RequiresPointer { get; }
}
