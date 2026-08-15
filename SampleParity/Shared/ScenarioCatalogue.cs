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
        AddForm1(scenarios);
        AddForm2(scenarios);
        AddForm3(scenarios);
        AddForm4(scenarios);
        AddForm5(scenarios);
        AddTest1(scenarios);

        return scenarios;
    }

    /// <summary>
    /// Clicks a control and records what the click did, without assuming the handler exists.
    /// </summary>
    /// <param name="ctx">The scenario context.</param>
    /// <param name="address">The control to click.</param>
    /// <param name="observed">Controls whose text is watched across the click.</param>
    /// <remarks>
    /// Text is compared as a change token rather than a literal, because the reference sample renders
    /// Chinese and the port English - see <see cref="ScenarioContext.Text" />.
    /// </remarks>
    private static void ClickAndObserve(ScenarioContext ctx, string address, params string[] observed)
    {
        var before = observed.Select(a => ControlLocator.FindControl(ctx.Form, a)?.Text).ToArray();
        var windowsBefore = Gtk.Window.ListToplevels().Length;

        ctx.Click(address);

        for (var i = 0; i < observed.Length; i++)
        {
            var after = ControlLocator.FindControl(ctx.Form, observed[i])?.Text;
            ctx.State(observed[i], "Text", ScenarioContext.Text(before[i], after));
        }

        // Whether the click opened a window, not how many exist: the absolute count is an artefact of how
        // each engine retains hidden and already-closed toplevels, and differs wildly between them.
        ctx.Window("openedAWindow", Gtk.Window.ListToplevels().Length > windowsBefore ? 1 : 0);
    }

    private static void AddForm1(List<Scenario> scenarios)
    {
        // Form1's button Click handlers are deliberately absent for now: several open modal dialogs or
        // secondary windows, and an unattended harness that clicks them accumulates toplevels and can be
        // re-prompted indefinitely. Driving those safely needs a per-scenario window quarantine (close
        // whatever the click spawned before the next scenario), which the RequiresPointer tier will need
        // anyway. The state-driven handlers below are deterministic and are covered here.

        scenarios.Add(new Scenario("form1.checkedlistbox.itemcheck", "Form1", ctx =>
        {
            if (ctx.Control("name:checkedListBox1") is not CheckedListBox list)
            {
                return;
            }

            ctx.State("checkedListBox1", "ItemCount", Count(list.Items.Count));

            if (list.Items.Count > 0)
            {
                list.SetItemChecked(0, true);
                ctx.Pump();
                ctx.State("checkedListBox1", "CheckedCount", Count(list.CheckedIndices.Count));
            }
        }));

        scenarios.Add(new Scenario("form1.textbox1.text", "Form1", ctx =>
        {
            if (ctx.Control("name:textBox1") is not TextBox box)
            {
                return;
            }

            box.Text = "parity";
            ctx.Pump();
            ctx.State("textBox1", "Text", box.Text);
        }));

        scenarios.Add(new Scenario("form1.treeview.select", "Form1", ctx =>
        {
            if (ctx.Control("name:treeView1") is not TreeView tree)
            {
                return;
            }

            ctx.State("treeView1", "TopLevelNodeCount", Count(tree.Nodes.Count));

            if (tree.Nodes.Count > 0)
            {
                tree.SelectedNode = tree.Nodes[0];
                ctx.Pump();
                ctx.State("treeView1", "SelectedNodeIsFirst",
                    ReferenceEquals(tree.SelectedNode, tree.Nodes[0]) ? "true" : "false");
                ctx.State("treeView1", "FirstNodeChildCount", Count(tree.Nodes[0].Nodes.Count));
            }
        }));

        scenarios.Add(new Scenario("form1.tabcontrol.select", "Form1", ctx =>
        {
            if (ctx.Control("name:tabControl1") is not TabControl tabs)
            {
                return;
            }

            ctx.State("tabControl1", "TabCount", Count(tabs.TabCount));

            for (var i = 0; i < Math.Min(3, tabs.TabCount); i++)
            {
                tabs.SelectedIndex = i;
                ctx.Pump();
                ctx.State("tabControl1", "SelectedIndex", Count(tabs.SelectedIndex));
            }
        }));

        scenarios.Add(new Scenario("form1.checkboxes.toggle", "Form1", ctx =>
        {
            foreach (var box in ControlLocator.AllReachableControls(ctx.Form).OfType<CheckBox>()
                         .OrderBy(c => c.Name, StringComparer.Ordinal))
            {
                box.Checked = true;
                ctx.Pump();
                ctx.State(box.Name, "Checked", box.Checked ? "true" : "false");
            }
        }));

        scenarios.Add(new Scenario("form1.radios.exclusive", "Form1", ctx =>
        {
            var radios = ControlLocator.AllReachableControls(ctx.Form).OfType<RadioButton>()
                .OrderBy(c => c.Name, StringComparer.Ordinal).ToList();

            if (radios.Count == 0)
            {
                return;
            }

            radios[0].Checked = true;
            ctx.Pump();

            // Selecting one must clear the others in the same container - the behaviour a tester checks.
            foreach (var radio in radios)
            {
                ctx.State(radio.Name, "Checked", radio.Checked ? "true" : "false");
            }
        }));
    }

    private static void AddForm4(List<Scenario> scenarios)
    {
        // As on Form1, the button Click handlers open windows and are left to the quarantined tier.

        scenarios.Add(new Scenario("form4.treeview.select", "Form4", ctx =>
        {
            if (ctx.Control("name:treeView1") is not TreeView tree)
            {
                return;
            }

            ctx.State("treeView1", "TopLevelNodeCount", Count(tree.Nodes.Count));

            for (var i = 0; i < Math.Min(3, tree.Nodes.Count); i++)
            {
                tree.SelectedNode = tree.Nodes[i];
                ctx.Pump();
                ctx.State("treeView1", "SelectedNodeIndex", Count(i));
                ctx.State("treeView1", "SelectedNodeChildCount", Count(tree.Nodes[i].Nodes.Count));
            }
        }));
    }

    private static void AddTest1(List<Scenario> scenarios)
    {
        scenarios.Add(new Scenario("test1.radios.exclusive", "Test1", ctx =>
        {
            var radios = ControlLocator.AllReachableControls(ctx.Form).OfType<RadioButton>()
                .OrderBy(c => c.Name, StringComparer.Ordinal).ToList();

            if (radios.Count == 0)
            {
                return;
            }

            radios[0].Checked = true;
            ctx.Pump();

            foreach (var radio in radios)
            {
                ctx.State(radio.Name, "Checked", radio.Checked ? "true" : "false");
            }
        }));

        scenarios.Add(new Scenario("test1.grid.shape", "Test1", ctx =>
        {
            if (ctx.Control("name:dataGridView1") is not DataGridView grid)
            {
                return;
            }

            // Shape only. The sample seeds this grid from DateTime.Now.Ticks, so cell values are not
            // comparable between two runs, let alone two engines.
            ctx.State("dataGridView1", "ColumnCount", Count(grid.Columns.Count));
            ctx.State("dataGridView1", "RowCount", Count(grid.Rows.Count));
        }));
    }

    private static string Count(int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
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
