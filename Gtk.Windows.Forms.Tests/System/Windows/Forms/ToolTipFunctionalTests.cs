using Color = global::System.Drawing.Color;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="ToolTip"/>. It is an extender provider: the caption it is given
/// for a control must come back for that control and no other, and clearing must actually forget it.</summary>
[Collection("NonParallelCollection")]
public class ToolTipFunctionalTests
{
    [Fact]
    public void SetToolTip_ThenGetToolTip_ReturnsTheSameCaption()
    {
        using var tip = new ToolTip();
        var button = new Button();

        tip.SetToolTip(button, "explain me");

        Assert.Equal("explain me", tip.GetToolTip(button));
    }

    [Fact]
    public void GetToolTip_ForAnUnknownControl_IsEmptyNotTheOtherControlsCaption()
    {
        using var tip = new ToolTip();
        var known = new Button();
        var unknown = new Button();
        tip.SetToolTip(known, "explain me");

        var caption = tip.GetToolTip(unknown);

        Assert.True(string.IsNullOrEmpty(caption),
            $"a control that was never given a tooltip should have none, but got '{caption}'");
    }

    [Fact]
    public void SetToolTip_Twice_ReplacesTheCaption()
    {
        using var tip = new ToolTip();
        var button = new Button();

        tip.SetToolTip(button, "first");
        tip.SetToolTip(button, "second");

        Assert.Equal("second", tip.GetToolTip(button));
    }

    [Fact]
    public void RemoveAll_ForgetsEveryCaption()
    {
        using var tip = new ToolTip();
        var a = new Button();
        var b = new Button();
        tip.SetToolTip(a, "a");
        tip.SetToolTip(b, "b");

        tip.RemoveAll();

        Assert.True(string.IsNullOrEmpty(tip.GetToolTip(a)), "RemoveAll should forget the first caption");
        Assert.True(string.IsNullOrEmpty(tip.GetToolTip(b)), "RemoveAll should forget the second caption");
    }

    [Fact]
    public void CanExtend_IsTrueForControlsOnly()
    {
        using var tip = new ToolTip();

        Assert.True(tip.CanExtend(new Button()), "a ToolTip extends controls");
        Assert.False(tip.CanExtend("not a control"), "a ToolTip does not extend arbitrary objects");
    }

    [Fact]
    public void DelayProperties_RoundTrip()
    {
        using var tip = new ToolTip
        {
            AutomaticDelay = 700,
            AutoPopDelay = 4000,
            InitialDelay = 300,
            ReshowDelay = 120
        };

        Assert.Equal(700, tip.AutomaticDelay);
        Assert.Equal(4000, tip.AutoPopDelay);
        Assert.Equal(300, tip.InitialDelay);
        Assert.Equal(120, tip.ReshowDelay);
    }

    [Fact]
    public void AppearanceProperties_RoundTrip()
    {
        using var tip = new ToolTip
        {
            Active = false,
            BackColor = Color.LightYellow,
            ForeColor = Color.DarkSlateGray,
            IsBalloon = true,
            OwnerDraw = true,
            ShowAlways = true,
            StripAmpersands = true,
            ToolTipIcon = ToolTipIcon.Warning,
            ToolTipTitle = "Heads up",
            UseAnimation = false,
            UseFading = false,
            Tag = "payload"
        };

        Assert.False(tip.Active);
        Assert.Equal(Color.LightYellow, tip.BackColor);
        Assert.Equal(Color.DarkSlateGray, tip.ForeColor);
        Assert.True(tip.IsBalloon);
        Assert.True(tip.OwnerDraw);
        Assert.True(tip.ShowAlways);
        Assert.True(tip.StripAmpersands);
        Assert.Equal(ToolTipIcon.Warning, tip.ToolTipIcon);
        Assert.Equal("Heads up", tip.ToolTipTitle);
        Assert.False(tip.UseAnimation);
        Assert.False(tip.UseFading);
        Assert.Equal("payload", tip.Tag);
    }

    [Fact]
    public void SetToolTip_WithNullCaption_ClearsIt()
    {
        using var tip = new ToolTip();
        var button = new Button();
        tip.SetToolTip(button, "something");

        tip.SetToolTip(button, null);

        Assert.True(string.IsNullOrEmpty(tip.GetToolTip(button)),
            "setting a null caption should clear the tooltip");
    }

    [Fact]
    public void GetToolTip_ForNull_DoesNotThrow()
    {
        using var tip = new ToolTip();

        var caption = tip.GetToolTip(null);

        Assert.True(string.IsNullOrEmpty(caption), "a null control has no tooltip");
    }
}
