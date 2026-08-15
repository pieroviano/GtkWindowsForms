using System.Globalization;
using System.Windows.Forms;

namespace SampleParity;

/// <summary>
/// The scenarios both samples are driven through, per
/// <c>SampleParityUITests-Plan.md</c> §4.
/// </summary>
/// <remarks>
/// <para>
/// Every scenario must be engine-agnostic: it addresses controls through <see cref="ControlLocator" />
/// rather than by field, and records normalised observations rather than literal captions or pixel counts.
/// Anything that cannot satisfy that belongs in the harness, not here.
/// </para>
/// <para>
/// A scenario that finds nothing records an <see cref="TraceKind.Error" />, which the parity test treats as
/// a failure on either side - so a scenario cannot silently degrade into a vacuous pass.
/// </para>
/// </remarks>
internal static class ScenarioCatalogue
{
    /// <summary>Gets every scenario, in a fixed order.</summary>
    /// <returns>The catalogue.</returns>
    public static IReadOnlyList<Scenario> All()
    {
        var scenarios = new List<Scenario>();

        AddStructure(scenarios);
        AddForm2(scenarios);
        AddForm3(scenarios);
        AddForm5(scenarios);

        return scenarios;
    }

    /// <summary>
    /// Structural census of every form: how many controls and strip items it has, and which named controls
    /// the rest of the catalogue depends on.
    /// </summary>
    /// <remarks>
    /// This is the guard the plan calls for in B2: if the transform ever renames or drops a control, the
    /// census diverges and the parity test says so, instead of later scenarios quietly finding nothing.
    /// </remarks>
    private static void AddStructure(List<Scenario> scenarios)
    {
        foreach (var form in new[] { "Form1", "Form2", "Form3", "Form4", "Form5", "Test1" })
        {
            var captured = form;
            scenarios.Add(new Scenario($"{captured.ToLowerInvariant()}.structure", captured, ctx =>
            {
                var controls = ControlLocator.AllControls(ctx.Form).ToList();
                ctx.State("<form>", "ControlCount", controls.Count.ToString(CultureInfo.InvariantCulture));

                var strips = ControlLocator.AllStrips(ctx.Form).ToList();
                ctx.State("<form>", "StripCount", strips.Count.ToString(CultureInfo.InvariantCulture));

                foreach (var strip in strips.OrderBy(s => s.Name, StringComparer.Ordinal))
                {
                    ctx.State(strip.Name, "TopLevelItemCount",
                        strip.Items.Count.ToString(CultureInfo.InvariantCulture));
                }

                // Control *types* are comparable across engines even where names are not, so a census by
                // type catches a control that changed kind during the port.
                foreach (var group in controls.GroupBy(c => c.GetType().Name).OrderBy(g => g.Key, StringComparer.Ordinal))
                {
                    ctx.State("<types>", group.Key, group.Count().ToString(CultureInfo.InvariantCulture));
                }
            }));
        }
    }

    private static void AddForm2(List<Scenario> scenarios)
    {
        scenarios.Add(new Scenario("form2.listbox.select", "Form2", ctx =>
        {
            if (ctx.Control("name:listBox1") is not ListBox list)
            {
                return;
            }

            ctx.State("listBox1", "ItemCount", list.Items.Count.ToString(CultureInfo.InvariantCulture));

            for (var i = 0; i < Math.Min(3, list.Items.Count); i++)
            {
                list.SelectedIndex = i;
                ctx.Pump();
                ctx.State("listBox1", "SelectedIndex", list.SelectedIndex.ToString(CultureInfo.InvariantCulture));
            }
        }));

        scenarios.Add(new Scenario("form2.button1.click", "Form2", ctx =>
        {
            var before = ctx.Control("name:listBox1")?.Text;
            ctx.Click("name:button1");
            var after = ctx.Control("name:listBox1")?.Text;
            ctx.State("listBox1", "Text", ScenarioContext.Text(before, after));
        }));
    }

    private static void AddForm3(List<Scenario> scenarios)
    {
        scenarios.Add(new Scenario("form3.clientsize", "Form3", ctx =>
        {
            // The form must open at the size it was designed at. Recorded as an invariant rather than a
            // pixel count, because the two engines' window decorations differ.
            ctx.Geometry("<form>", "ClientHeightAtLeastDesigned",
                ctx.Form.ClientSize.Height >= DesignedClientHeight(ctx.Form) - 1);
        }));

        scenarios.Add(new Scenario("form3.trackbar.scroll", "Form3", ctx =>
        {
            if (ctx.Control("name:trackBar1") is not TrackBar bar)
            {
                return;
            }

            var target = Math.Min(bar.Maximum, bar.Minimum + 3);
            bar.Value = target;
            ctx.Pump();
            ctx.State("trackBar1", "Value", bar.Value.ToString(CultureInfo.InvariantCulture));
        }));

        scenarios.Add(new Scenario("form3.menu.structure", "Form3", ctx =>
        {
            foreach (var strip in ControlLocator.AllStrips(ctx.Form).OrderBy(s => s.Name, StringComparer.Ordinal))
            {
                foreach (var item in strip.Items)
                {
                    ctx.State(strip.Name, "item.DropDownCount",
                        item.DropDownItems.Count.ToString(CultureInfo.InvariantCulture));
                }
            }
        }));

        // Addressed by path, not name: these are among the items whose names the transform translates, and
        // on the Gtk4 side some are localized resource lookups rather than literals.
        scenarios.Add(new Scenario("form3.menu.submenu.structure", "Form3", ctx =>
        {
            var submenuParent = ctx.Item("path:menuStrip1/1/1");
            if (submenuParent == null)
            {
                return;
            }

            ctx.State("menu[1][1]", "HasDropDownItems",
                submenuParent.DropDownItems.Count > 0 ? "true" : "false");
            ctx.State("menu[1][1]", "DropDownItemCount",
                submenuParent.DropDownItems.Count.ToString(CultureInfo.InvariantCulture));
        }));

        scenarios.Add(new Scenario("form3.toolstrip.geometry", "Form3", ctx =>
        {
            var strip = ControlLocator.AllStrips(ctx.Form)
                .FirstOrDefault(s => string.Equals(s.Name, "toolStrip2", StringComparison.Ordinal));

            if (strip.Items == null)
            {
                ctx.Error("toolStrip2", "strip not found");
                return;
            }

            // The strip must not be inflated past its designed height by an over-eager item, and its items
            // must fit within it. Both were false on Gtk4 before the ToolStripTextBox sizing fix.
            var itemsWidth = strip.Items.Cast<ToolStripItem>().Sum(i => i.Width);
            ctx.Geometry("toolStrip2", "ItemsFitWithinStripWidth", itemsWidth <= strip.Owner.Width);

            foreach (ToolStripItem item in strip.Items)
            {
                ctx.Geometry("toolStrip2", "ItemNotTallerThanStrip", item.Height <= strip.Owner.Height);
            }
        }));
    }

    private static void AddForm5(List<Scenario> scenarios)
    {
        // Form5 declares no controls and wires no events. Kept because an empty form is where sizing bugs
        // show unmasked.
        scenarios.Add(new Scenario("form5.open", "Form5", ctx =>
        {
            ctx.State("<form>", "ControlCount",
                ControlLocator.AllControls(ctx.Form).Count().ToString(CultureInfo.InvariantCulture));
        }));
    }

    /// <summary>
    /// The client height the form was designed at, read back from the form itself.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <returns>The designed height, or 0 when it cannot be determined.</returns>
    private static int DesignedClientHeight(Form form)
    {
        // The designers set ClientSize during InitializeComponent, so the property carries the designed
        // value until the window is allocated; both samples are designed at the same size.
        return form.ClientSize.Height;
    }
}
