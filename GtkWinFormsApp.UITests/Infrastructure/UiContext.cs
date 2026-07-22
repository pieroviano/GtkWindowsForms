using System.Windows.Forms;

namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// The scripting surface handed to a test body: the form under test plus the API-level "input" verbs
/// (click, type, select) and the screenshot checkpoint. Every verb pumps the GTK loop afterwards so the
/// resulting layout/paint is in place before the next assertion or capture.
/// </summary>
internal sealed class UiContext
{
    internal UiContext(Form form) => Form = form;

    /// <summary>The form under test.</summary>
    internal Form Form { get; }

    /// <summary>Pumps the GTK main loop so pending work settles.</summary>
    internal void Pump(int iterations = 12) => UiDriver.Pump(iterations);

    /// <summary>Clicks a button/control by raising its wired <c>Click</c> handler, then pumps.</summary>
    internal void Click(Control control)
    {
        UiDriver.RaiseProtected(control, "OnClick");
        Pump();
    }

    /// <summary>Sets a text control's value (raising its <c>TextChanged</c> path), then pumps.</summary>
    internal void Type(Control textControl, string text)
    {
        textControl.Text = text;
        Pump();
    }

    /// <summary>Selects an item in a combo box by index, then pumps.</summary>
    internal void Select(ComboBox combo, int index)
    {
        combo.SelectedIndex = index;
        Pump();
    }

    /// <summary>Selects an item in a list box by index, then pumps.</summary>
    internal void Select(ListBox list, int index)
    {
        list.SelectedIndex = index;
        Pump();
    }

    /// <summary>
    /// Captures the form window and compares it to its committed baseline (or updates the baseline when
    /// <c>UPDATE_UI_BASELINES=1</c>). <paramref name="checkpoint"/> names the visual state.
    /// </summary>
    internal void Shot(string checkpoint) => Screenshots.Capture(Form, checkpoint);
}
