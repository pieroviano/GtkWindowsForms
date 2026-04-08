extern alias gtk;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System;
using System.Reflection;
using Xunit;

namespace System.Windows.Forms.ControlTests;

[Collection("NonParallelCollection")]
public class ButtonTests
{
    [Fact]
    public void Button_GtkControl_And_Self_AreNotNull()
    {
        var form = new Form();
        var button = new Button();
        form.Controls.Add(button);
        object? buttonGtkControl = null;
        ButtonBase? buttonSelf = null;
        form.Load += (s, e) =>
        {
            buttonGtkControl = button.GtkControl;
            buttonSelf = button.self;
            form.Close();
        };
        form.ShowDialog();

        Assert.NotNull(buttonGtkControl);
        Assert.NotNull(buttonSelf);
    }

    [Fact]
    public void Button_Text_SetGet_MapsToUnderlyingLabel()
    {
        Application.Init();
        var button = new Button
        {
            Text = "hello",
            Name = "btn1"
        };

        Assert.Equal("hello", button.Text);
        Assert.Equal("hello", ((Gtk.Label)button.self.Child).Text);
        Assert.Equal("btn1", button.Name);
    }

    [Fact]
    public void Button_RightToLeft_MapsTo_GtkDirection()
    {
        Application.Init();
        var button = new Button
        {
            RightToLeft = RightToLeft.Yes
        };

        Assert.Equal(Gtk.TextDirection.Rtl, button.self.Direction);

        button.RightToLeft = RightToLeft.No;
        Assert.Equal(Gtk.TextDirection.Ltr, button.self.Direction);
    }

    [Fact]
    public void Button_TextAlign_Sets_Xalign_Yalign()
    {
        Application.Init();
        var button = new Button
        {
            TextAlign = gtk::System.Drawing.ContentAlignment.MiddleCenter
        };

        Assert.Equal(0.5f, button.self.Xalign);
        Assert.Equal(0.5f, button.self.Yalign);

        button.TextAlign = gtk::System.Drawing.ContentAlignment.TopLeft;
        Assert.Equal(0.0f, button.self.Xalign);
        Assert.Equal(0.0f, button.self.Yalign);
    }

    [Fact]
    public void OnClick_Raises_Click_Event()
    {
        Application.Init();
        var button = new Button();
        var invoked = false;
        button.Click += (s, e) => invoked = true;

        // Invoke protected OnClick via reflection to simulate a click
        var onClick = typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onClick);

        onClick!.Invoke(button, new object[] { EventArgs.Empty });

        Assert.True(invoked);
    }
}