namespace System.Windows.Forms;

using System.Collections.Generic;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

/// <summary>
/// Verifies the third sweep batch: the accessibility/system enums, the designer/editing interfaces,
/// <see cref="FlowLayoutSettings" />/<see cref="LayoutSettings" />, <see cref="ImeModeConversion" /> and the
/// now-public <see cref="ListBindingHelper" />. All are model/enum/interface types with no GTK widgets.
/// </summary>
public class SupportTypesBatchCFunctionalTests
{
    [Fact]
    public void Enums_HaveExpectedValues()
    {
        Assert.Equal(1, (int)AccessibleNavigation.Up);
        Assert.Equal(8, (int)AccessibleNavigation.LastChild);

        Assert.True(typeof(AccessibleSelection).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(16, (int)AccessibleSelection.RemoveSelection);

        Assert.Equal(2, (int)BootMode.FailSafeWithNetwork);
        Assert.Equal(1, (int)PowerState.Hibernate);
        Assert.Equal(3, (int)ScreenOrientation.Angle270);
        Assert.Equal(9, (int)SecurityIDType.Computer);
        Assert.Equal(3, (int)StructFormat.Auto);

        Assert.True(typeof(ToolStripItemDesignerAvailability).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(15, (int)ToolStripItemDesignerAvailability.All);
    }

    [Fact]
    public void Interfaces_AreDefined()
    {
        Assert.True(typeof(ICommandExecutor).IsInterface);
        Assert.True(typeof(IFileReaderService).IsInterface);
        Assert.True(typeof(IDataGridViewEditingCell).IsInterface);
        Assert.True(typeof(IUIService).IsInterface);
        Assert.True(typeof(IWindowsFormsEditorService).IsInterface);
        Assert.True(typeof(IComponentEditorPageSite).IsInterface);
        Assert.NotNull(typeof(ICommandExecutor).GetMethod("Execute"));
    }

    [Fact]
    public void FlowLayoutSettings_Behaviour()
    {
        var s = (FlowLayoutSettings)Activator.CreateInstance(typeof(FlowLayoutSettings), nonPublic: true)!;
        Assert.IsAssignableFrom<LayoutSettings>(s);
        Assert.NotNull(s.LayoutEngine);
        Assert.IsAssignableFrom<LayoutEngine>(s.LayoutEngine);

        Assert.Equal(FlowDirection.LeftToRight, s.FlowDirection);
        Assert.True(s.WrapContents);
        s.FlowDirection = FlowDirection.TopDown;
        s.WrapContents = false;
        Assert.Equal(FlowDirection.TopDown, s.FlowDirection);
        Assert.False(s.WrapContents);

        var child = new object();
        Assert.False(s.GetFlowBreak(child));
        s.SetFlowBreak(child, true);
        Assert.True(s.GetFlowBreak(child));
    }

    [Fact]
    public void ImeModeConversion_Defaults()
    {
        Assert.False(ImeModeConversion.IsCurrentConversionTableSupported);
        Assert.NotNull(ImeModeConversion.ImeModeConversionBits);
    }

    [Fact]
    public void ListBindingHelper_IsPublic_AndResolvesLists()
    {
        Assert.True(typeof(ListBindingHelper).IsPublic);

        var list = new List<int> { 1, 2, 3 };
        Assert.Same(list, ListBindingHelper.GetList(list));
        Assert.Equal(typeof(int), ListBindingHelper.GetListItemType(list));

        var arr = new[] { "a", "b" };
        Assert.Equal(typeof(string), ListBindingHelper.GetListItemType(arr));
    }
}
