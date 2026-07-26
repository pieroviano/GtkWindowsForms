namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the <see cref="UpDownBase" /> base class, the re-parenting of <see cref="NumericUpDown" /> onto
/// it, and the new <see cref="DomainUpDown" /> control. Hierarchy claims are checked by reflection; the
/// value/selection behaviour runs inside a single realized-form lifecycle (form created first so GTK is
/// initialized), with no clipboard interaction.
/// </summary>
[Collection("NonParallelCollection")]
public class UpDownFunctionalTests
{
    [Fact]
    public void Hierarchy_IsFaithfulToWinForms()
    {
        Assert.True(typeof(UpDownBase).IsAssignableFrom(typeof(NumericUpDown)));
        Assert.True(typeof(UpDownBase).IsAssignableFrom(typeof(DomainUpDown)));
        // Real WinForms: UpDownBase : ContainerControl.
        Assert.Equal(typeof(ContainerControl), typeof(UpDownBase).BaseType);
    }

    [Fact]
    public void UpDownBehaviour_Behaves()
    {
        var form = new Form();
        var numeric = new NumericUpDown();
        var domain = new DomainUpDown();
        form.Controls.Add(numeric);
        form.Controls.Add(domain);

        form.Load += (s, e) =>
        {
            try
            {
                // --- NumericUpDown.UpButton/DownButton step by Increment and clamp ---
                numeric.Minimum = 0;
                numeric.Maximum = 10;
                numeric.Increment = 2;
                numeric.Value = 4;
                numeric.UpButton();
                Assert.Equal(6m, numeric.Value);
                numeric.DownButton();
                Assert.Equal(4m, numeric.Value);
                numeric.Value = 10;
                numeric.UpButton(); // clamps at Maximum
                Assert.Equal(10m, numeric.Value);

                // --- DomainUpDown item model + selection ---
                domain.Items.Add("alpha");
                domain.Items.Add("beta");
                domain.Items.Add("gamma");
                Assert.Equal(3, domain.Items.Count);

                domain.SelectedIndex = 0;
                Assert.Equal("alpha", domain.SelectedItem);
                Assert.Equal("alpha", domain.Text);

                domain.DownButton();
                Assert.Equal(1, domain.SelectedIndex);
                Assert.Equal("beta", domain.SelectedItem);

                domain.UpButton();
                Assert.Equal(0, domain.SelectedIndex);

                // --- without Wrap, UpButton stays at the first item ---
                domain.UpButton();
                Assert.Equal(0, domain.SelectedIndex);

                // --- with Wrap, UpButton from the first item goes to the last ---
                domain.Wrap = true;
                domain.UpButton();
                Assert.Equal(2, domain.SelectedIndex);
                Assert.Equal("gamma", domain.SelectedItem);

                // --- Sorted reorders the items ---
                domain.Items.Clear();
                domain.Items.Add("charlie");
                domain.Items.Add("alpha");
                domain.Items.Add("bravo");
                domain.Sorted = true;
                Assert.Equal("alpha", domain.Items[0]);
                Assert.Equal("bravo", domain.Items[1]);
                Assert.Equal("charlie", domain.Items[2]);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}
