namespace System.Windows.Forms;

using System.Threading;
using System.Windows.Forms.Layout;

/// <summary>
/// Verifies the system/feature/help support types: <see cref="PowerStatus" />, <see cref="OSFeature" />/
/// <see cref="FeatureSupport" />, <see cref="Help" />/<see cref="HelpProvider" />,
/// <see cref="WindowsFormsSynchronizationContext" /> and the relocated
/// <see cref="System.Windows.Forms.Layout.LayoutEngine" />. Most are model/static types with no widgets; the
/// control-keyed HelpProvider methods run in a realized form.
/// </summary>
[Collection("NonParallelCollection")]
public class SystemFeatureTypesFunctionalTests
{
    [Fact]
    public void PowerStatus_DefaultsToUnknownNoBattery()
    {
        var p = (PowerStatus)Activator.CreateInstance(typeof(PowerStatus), nonPublic: true)!;
        Assert.Equal(PowerLineStatus.Unknown, p.PowerLineStatus);
        Assert.Equal(BatteryChargeStatus.NoSystemBattery, p.BatteryChargeStatus);
        Assert.Equal(-1, p.BatteryLifeRemaining);
    }

    [Fact]
    public void OSFeature_FeatureSupport_Behaviour()
    {
        Assert.NotNull(OSFeature.Feature);
        Assert.Same(OSFeature.Feature, OSFeature.Feature);
        Assert.Null(OSFeature.Feature.GetVersionPresent(OSFeature.Themes));
        Assert.False(OSFeature.Feature.IsPresent(OSFeature.LayeredWindows));
        Assert.False(OSFeature.Feature.IsPresent(SystemParameter.DropShadow));

        Assert.False(FeatureSupport.IsPresent("SomeType", "SomeConst"));
        Assert.IsAssignableFrom<IFeatureSupport>(OSFeature.Feature);
    }

    [Fact]
    public void Help_StaticMethods_DoNotThrow()
    {
        // Displaying help is a documented no-op boundary on GTK; the methods must exist and be safe to call.
        Help.ShowHelp(null!, "help.chm");
        Help.ShowHelpIndex(null!, "help.chm");
        Help.ShowHelp(null!, "help.chm", HelpNavigator.Index);
    }

    [Fact]
    public void HelpProvider_StoresAssociations()
    {
        var hp = new HelpProvider { HelpNamespace = "app.chm", Tag = 7 };
        Assert.Equal("app.chm", hp.HelpNamespace);
        Assert.Equal(7, hp.Tag);
        Assert.False(hp.CanExtend(new object()));

        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);
        form.Load += (s, e) =>
        {
            try
            {
                Assert.True(hp.CanExtend(label));
                Assert.False(hp.GetShowHelp(label));

                hp.SetHelpString(label, "press F1");
                Assert.Equal("press F1", hp.GetHelpString(label));
                Assert.True(hp.GetShowHelp(label));

                hp.SetHelpKeyword(label, "topic1");
                Assert.Equal("topic1", hp.GetHelpKeyword(label));

                hp.SetShowHelp(label, false);
                Assert.False(hp.GetShowHelp(label));
            }
            finally
            {
                form.Close();
            }
        };
        form.ShowDialog();
        form.Dispose();
    }

    [Fact]
    public void WindowsFormsSynchronizationContext_Structure()
    {
        using var ctx = new WindowsFormsSynchronizationContext();
        Assert.IsAssignableFrom<SynchronizationContext>(ctx);
        Assert.NotSame(ctx, ctx.CreateCopy());
        Assert.IsType<WindowsFormsSynchronizationContext>(ctx.CreateCopy());

        var prev = WindowsFormsSynchronizationContext.AutoInstall;
        WindowsFormsSynchronizationContext.AutoInstall = false;
        Assert.False(WindowsFormsSynchronizationContext.AutoInstall);
        WindowsFormsSynchronizationContext.AutoInstall = prev;
    }

    private sealed class SpyLayoutEngine : LayoutEngine
    {
        public bool LaidOut;
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            LaidOut = true;
            return base.Layout(container, layoutEventArgs);
        }
    }

    [Fact]
    public void LayoutEngine_IsInTheLayoutNamespace_AndOverridable()
    {
        Assert.Equal("System.Windows.Forms.Layout", typeof(LayoutEngine).Namespace);

        var engine = new SpyLayoutEngine();
        engine.InitLayout(new object(), BoundsSpecified.All);
        Assert.False(engine.Layout(new object(), new LayoutEventArgs(null, "Bounds")));
        Assert.True(engine.LaidOut);
    }
}
