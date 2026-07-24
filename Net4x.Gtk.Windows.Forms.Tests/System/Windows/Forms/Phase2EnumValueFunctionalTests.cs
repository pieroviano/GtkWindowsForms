namespace System.Windows.Forms;

/// <summary>
/// Pins the underlying values of the second batch of compatibility enums against the real net48 reference
/// assembly. Includes the <c>[Flags]</c> sets and the non-sequential / negative values that are easy to get
/// wrong (<see cref="RichTextBoxSelectionAttribute.Mixed" /> = -1, <see cref="AutoCompleteSource" />,
/// <see cref="ArrangeDirection" /> where Left == Right == 0). No GTK widgets are created.
/// </summary>
public class Phase2EnumValueFunctionalTests
{
    [Fact]
    public void RichTextBoxStreamType_Values()
    {
        Assert.Equal(0, (int)RichTextBoxStreamType.RichText);
        Assert.Equal(4, (int)RichTextBoxStreamType.UnicodePlainText);
    }

    [Fact]
    public void RichTextBoxScrollBars_Values()
    {
        Assert.Equal(3, (int)RichTextBoxScrollBars.Both);
        Assert.Equal(17, (int)RichTextBoxScrollBars.ForcedHorizontal);
        Assert.Equal(19, (int)RichTextBoxScrollBars.ForcedBoth);
    }

    [Fact]
    public void RichTextBoxFinds_IsFlags()
    {
        Assert.True(typeof(RichTextBoxFinds).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(16, (int)RichTextBoxFinds.Reverse);
        Assert.Equal(RichTextBoxFinds.WholeWord | RichTextBoxFinds.MatchCase, (RichTextBoxFinds)6);
    }

    [Fact]
    public void RichTextBoxSelectionAttribute_HasNegativeMixed()
    {
        Assert.Equal(-1, (int)RichTextBoxSelectionAttribute.Mixed);
        Assert.Equal(0, (int)RichTextBoxSelectionAttribute.None);
        Assert.Equal(1, (int)RichTextBoxSelectionAttribute.All);
    }

    [Fact]
    public void RichTextBoxSelectionTypes_IsFlags()
    {
        Assert.True(typeof(RichTextBoxSelectionTypes).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(8, (int)RichTextBoxSelectionTypes.MultiObject);
    }

    [Fact]
    public void RichTextBoxWordPunctuations_Values()
    {
        Assert.Equal(128, (int)RichTextBoxWordPunctuations.Level1);
        Assert.Equal(896, (int)RichTextBoxWordPunctuations.All);
    }

    [Fact]
    public void RichTextBoxLanguageOptions_IsFlags()
    {
        Assert.True(typeof(RichTextBoxLanguageOptions).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(1, (int)RichTextBoxLanguageOptions.AutoKeyboard);
        Assert.Equal(32, (int)RichTextBoxLanguageOptions.UIFonts);
    }

    [Fact]
    public void TreeViewDrawMode_Values()
    {
        Assert.Equal(0, (int)TreeViewDrawMode.Normal);
        Assert.Equal(2, (int)TreeViewDrawMode.OwnerDrawAll);
    }

    [Fact]
    public void ToolStripDropDownCloseReason_Values()
    {
        Assert.Equal(0, (int)ToolStripDropDownCloseReason.AppFocusChange);
        Assert.Equal(4, (int)ToolStripDropDownCloseReason.CloseCalled);
    }

    [Fact]
    public void ToolStripGripDisplayStyle_Values()
    {
        Assert.Equal(0, (int)ToolStripGripDisplayStyle.Horizontal);
        Assert.Equal(1, (int)ToolStripGripDisplayStyle.Vertical);
    }

    [Fact]
    public void ToolStripManagerRenderMode_Values()
    {
        Assert.Equal(0, (int)ToolStripManagerRenderMode.Custom);
        Assert.Equal(2, (int)ToolStripManagerRenderMode.Professional);
    }

    [Fact]
    public void DataGridViewAutoSizeColumnsMode_NonSequentialValues()
    {
        Assert.Equal(1, (int)DataGridViewAutoSizeColumnsMode.None);
        Assert.Equal(6, (int)DataGridViewAutoSizeColumnsMode.AllCells);
        Assert.Equal(10, (int)DataGridViewAutoSizeColumnsMode.DisplayedCells);
        Assert.Equal(16, (int)DataGridViewAutoSizeColumnsMode.Fill);
    }

    [Fact]
    public void AutoCompleteSource_NonSequentialValues()
    {
        Assert.Equal(1, (int)AutoCompleteSource.FileSystem);
        Assert.Equal(6, (int)AutoCompleteSource.AllUrl);
        Assert.Equal(7, (int)AutoCompleteSource.AllSystemSources);
        Assert.Equal(64, (int)AutoCompleteSource.CustomSource);
        Assert.Equal(256, (int)AutoCompleteSource.ListItems);
    }

    [Fact]
    public void ArrangeDirection_LeftAndRightAreZero()
    {
        Assert.Equal(0, (int)ArrangeDirection.Left);
        Assert.Equal(0, (int)ArrangeDirection.Right);
        Assert.Equal(4, (int)ArrangeDirection.Up);
        Assert.Equal(4, (int)ArrangeDirection.Down);
    }

    [Fact]
    public void ArrangeStartingPosition_Values()
    {
        Assert.Equal(0, (int)ArrangeStartingPosition.BottomLeft);
        Assert.Equal(8, (int)ArrangeStartingPosition.Hide);
    }

    [Fact]
    public void CaptionButton_Values()
    {
        Assert.Equal(0, (int)CaptionButton.Close);
        Assert.Equal(4, (int)CaptionButton.Help);
    }

    [Fact]
    public void MenuGlyph_Values()
    {
        Assert.Equal(0, (int)MenuGlyph.Arrow);
        Assert.Equal(2, (int)MenuGlyph.Bullet);
        Assert.Equal(0, (int)MenuGlyph.Min);
        Assert.Equal(2, (int)MenuGlyph.Max);
    }

    [Fact]
    public void ListViewHitTestLocations_IsFlags_WithExpectedValues()
    {
        Assert.True(typeof(ListViewHitTestLocations).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(1, (int)ListViewHitTestLocations.None);
        Assert.Equal(512, (int)ListViewHitTestLocations.StateImage);
    }

    [Fact]
    public void TreeViewHitTestLocations_IsFlags_WithExpectedValues()
    {
        Assert.True(typeof(TreeViewHitTestLocations).IsDefined(typeof(FlagsAttribute), false));
        Assert.Equal(16, (int)TreeViewHitTestLocations.PlusMinus);
        Assert.Equal(2048, (int)TreeViewHitTestLocations.LeftOfClientArea);
    }
}
