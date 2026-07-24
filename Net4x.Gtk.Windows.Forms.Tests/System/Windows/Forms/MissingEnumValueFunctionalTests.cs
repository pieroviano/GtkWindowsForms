namespace System.Windows.Forms;

/// <summary>
/// Pins the underlying integer values of the compatibility enums added for source compatibility. Enum
/// values are API — code compares them against Win32 constants and persists them in resources — so these
/// assert the exact values from the real <c>System.Windows.Forms</c> reference assembly, several of which
/// are non-sequential (<see cref="ArrowDirection" />, <see cref="SearchDirectionHint" />). No GTK widgets
/// are created, so this runs without a realized form.
/// </summary>
public class MissingEnumValueFunctionalTests
{
    [Fact]
    public void CharacterCasing_Values()
    {
        Assert.Equal(0, (int)CharacterCasing.Normal);
        Assert.Equal(1, (int)CharacterCasing.Upper);
        Assert.Equal(2, (int)CharacterCasing.Lower);
    }

    [Fact]
    public void ScrollBars_Values()
    {
        Assert.Equal(0, (int)ScrollBars.None);
        Assert.Equal(1, (int)ScrollBars.Horizontal);
        Assert.Equal(2, (int)ScrollBars.Vertical);
        Assert.Equal(3, (int)ScrollBars.Both);
    }

    [Fact]
    public void TabAppearance_Values()
    {
        Assert.Equal(0, (int)TabAppearance.Normal);
        Assert.Equal(1, (int)TabAppearance.Buttons);
        Assert.Equal(2, (int)TabAppearance.FlatButtons);
    }

    [Fact]
    public void FrameStyle_Values()
    {
        Assert.Equal(0, (int)FrameStyle.Dashed);
        Assert.Equal(1, (int)FrameStyle.Thick);
    }

    [Fact]
    public void LinkBehavior_Values()
    {
        Assert.Equal(0, (int)LinkBehavior.SystemDefault);
        Assert.Equal(1, (int)LinkBehavior.AlwaysUnderline);
        Assert.Equal(2, (int)LinkBehavior.HoverUnderline);
        Assert.Equal(3, (int)LinkBehavior.NeverUnderline);
    }

    [Fact]
    public void InsertKeyMode_Values()
    {
        Assert.Equal(0, (int)InsertKeyMode.Default);
        Assert.Equal(1, (int)InsertKeyMode.Insert);
        Assert.Equal(2, (int)InsertKeyMode.Overwrite);
    }

    [Fact]
    public void ItemBoundsPortion_Values()
    {
        Assert.Equal(0, (int)ItemBoundsPortion.Entire);
        Assert.Equal(1, (int)ItemBoundsPortion.Icon);
        Assert.Equal(2, (int)ItemBoundsPortion.Label);
        Assert.Equal(3, (int)ItemBoundsPortion.ItemOnly);
    }

    [Fact]
    public void ColumnHeaderAutoResizeStyle_Values()
    {
        Assert.Equal(0, (int)ColumnHeaderAutoResizeStyle.None);
        Assert.Equal(1, (int)ColumnHeaderAutoResizeStyle.HeaderSize);
        Assert.Equal(2, (int)ColumnHeaderAutoResizeStyle.ColumnContent);
    }

    [Fact]
    public void ScrollButton_Values()
    {
        Assert.Equal(0, (int)ScrollButton.Up);
        Assert.Equal(1, (int)ScrollButton.Down);
        Assert.Equal(2, (int)ScrollButton.Left);
        Assert.Equal(3, (int)ScrollButton.Right);
        Assert.Equal(0, (int)ScrollButton.Min);
        Assert.Equal(3, (int)ScrollButton.Max);
    }

    [Fact]
    public void ArrowDirection_Values()
    {
        // Non-sequential in real WinForms.
        Assert.Equal(0, (int)ArrowDirection.Left);
        Assert.Equal(1, (int)ArrowDirection.Up);
        Assert.Equal(16, (int)ArrowDirection.Right);
        Assert.Equal(17, (int)ArrowDirection.Down);
    }

    [Fact]
    public void SearchDirectionHint_Values()
    {
        // Virtual-key values in real WinForms, not 1..4.
        Assert.Equal(37, (int)SearchDirectionHint.Left);
        Assert.Equal(38, (int)SearchDirectionHint.Up);
        Assert.Equal(39, (int)SearchDirectionHint.Right);
        Assert.Equal(40, (int)SearchDirectionHint.Down);
    }

    [Fact]
    public void MenuMerge_Values()
    {
        Assert.Equal(0, (int)MenuMerge.Add);
        Assert.Equal(1, (int)MenuMerge.Replace);
        Assert.Equal(2, (int)MenuMerge.MergeItems);
        Assert.Equal(3, (int)MenuMerge.Remove);
    }

    [Fact]
    public void DockingBehavior_Values()
    {
        Assert.Equal(0, (int)DockingBehavior.Never);
        Assert.Equal(1, (int)DockingBehavior.Ask);
        Assert.Equal(2, (int)DockingBehavior.AutoDock);
    }
}
