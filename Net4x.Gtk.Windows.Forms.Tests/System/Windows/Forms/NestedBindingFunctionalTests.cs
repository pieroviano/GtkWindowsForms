using System.ComponentModel;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for binding to a <em>nested</em> (dotted) data member — e.g.
/// <c>DataBindings.Add("Text", customer, "Address.City")</c>. WinForms resolves the path segment
/// ("Address") to an intermediate object and binds the field ("City") on it, both directions.
/// EMULATION-GAPS §5.6 flags binding depth as "value flow fixed, depth unverified"; these exercise the
/// intermediate-traversal path the single-level tests never reach.
/// </summary>
[Collection("NonParallelCollection")]
public class NestedBindingFunctionalTests
{
    static NestedBindingFunctionalTests()
    {
        // Seed GTK (Application.Init runs from Form's static ctor) so constructing a TextBox in isolation
        // does not hit the control-before-Form Gtk-CRITICAL crash.
        using var _ = new Form();
    }

    private sealed class Address
    {
        public string? City { get; set; }
        public int Zip { get; set; }
    }

    private sealed class Customer
    {
        public string? Name { get; set; }
        public Address? Address { get; set; } = new();
    }

    [Fact]
    public void ReadValue_TraversesTheIntermediateObject()
    {
        using var box = new TextBox();
        var customer = new Customer { Address = new Address { City = "Rome" } };
        var binding = new Binding("Text", customer, "Address.City");
        box.DataBindings.Add(binding);

        binding.ReadValue();

        Assert.Equal("Rome", box.Text);
    }

    [Fact]
    public void WriteValue_UpdatesThePropertyOnTheIntermediateObject()
    {
        using var box = new TextBox();
        var customer = new Customer { Address = new Address { City = "Rome" } };
        var binding = new Binding("Text", customer, "Address.City");
        box.DataBindings.Add(binding);

        box.Text = "Milan";
        binding.WriteValue();

        Assert.Equal("Milan", customer.Address!.City);
    }

    [Fact]
    public void WriteValue_ConvertsToTheNestedPropertyType()
    {
        using var box = new TextBox();
        var customer = new Customer { Address = new Address { Zip = 1 } };
        var binding = new Binding("Text", customer, "Address.Zip", true);
        box.DataBindings.Add(binding);

        box.Text = "20100";
        binding.WriteValue();

        Assert.Equal(20100, customer.Address!.Zip);
    }

    [Fact]
    public void ReadValue_TracksAReplacedIntermediateObject()
    {
        using var box = new TextBox();
        var customer = new Customer { Address = new Address { City = "Rome" } };
        var binding = new Binding("Text", customer, "Address.City");
        box.DataBindings.Add(binding);

        binding.ReadValue();
        Assert.Equal("Rome", box.Text);

        // Swap the whole intermediate object — a re-read must follow the new one.
        customer.Address = new Address { City = "Turin" };
        binding.ReadValue();

        Assert.Equal("Turin", box.Text);
    }
}
