// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides static access to the colors used by <see cref="T:System.Windows.Forms.ToolStripProfessionalRenderer" />.</summary>
public static class ProfessionalColors
{
    private static readonly ProfessionalColorTable Table = new();

    /// <summary>Gets the ButtonSelectedHighlight color.</summary>
    public static Color ButtonSelectedHighlight => Table.ButtonSelectedHighlight;

    /// <summary>Gets the ButtonSelectedHighlightBorder color.</summary>
    public static Color ButtonSelectedHighlightBorder => Table.ButtonSelectedHighlightBorder;

    /// <summary>Gets the ButtonPressedHighlight color.</summary>
    public static Color ButtonPressedHighlight => Table.ButtonPressedHighlight;

    /// <summary>Gets the ButtonPressedHighlightBorder color.</summary>
    public static Color ButtonPressedHighlightBorder => Table.ButtonPressedHighlightBorder;

    /// <summary>Gets the ButtonCheckedHighlight color.</summary>
    public static Color ButtonCheckedHighlight => Table.ButtonCheckedHighlight;

    /// <summary>Gets the ButtonCheckedHighlightBorder color.</summary>
    public static Color ButtonCheckedHighlightBorder => Table.ButtonCheckedHighlightBorder;

    /// <summary>Gets the ButtonPressedBorder color.</summary>
    public static Color ButtonPressedBorder => Table.ButtonPressedBorder;

    /// <summary>Gets the ButtonSelectedBorder color.</summary>
    public static Color ButtonSelectedBorder => Table.ButtonSelectedBorder;

    /// <summary>Gets the ButtonCheckedGradientBegin color.</summary>
    public static Color ButtonCheckedGradientBegin => Table.ButtonCheckedGradientBegin;

    /// <summary>Gets the ButtonCheckedGradientMiddle color.</summary>
    public static Color ButtonCheckedGradientMiddle => Table.ButtonCheckedGradientMiddle;

    /// <summary>Gets the ButtonCheckedGradientEnd color.</summary>
    public static Color ButtonCheckedGradientEnd => Table.ButtonCheckedGradientEnd;

    /// <summary>Gets the ButtonSelectedGradientBegin color.</summary>
    public static Color ButtonSelectedGradientBegin => Table.ButtonSelectedGradientBegin;

    /// <summary>Gets the ButtonSelectedGradientMiddle color.</summary>
    public static Color ButtonSelectedGradientMiddle => Table.ButtonSelectedGradientMiddle;

    /// <summary>Gets the ButtonSelectedGradientEnd color.</summary>
    public static Color ButtonSelectedGradientEnd => Table.ButtonSelectedGradientEnd;

    /// <summary>Gets the ButtonPressedGradientBegin color.</summary>
    public static Color ButtonPressedGradientBegin => Table.ButtonPressedGradientBegin;

    /// <summary>Gets the ButtonPressedGradientMiddle color.</summary>
    public static Color ButtonPressedGradientMiddle => Table.ButtonPressedGradientMiddle;

    /// <summary>Gets the ButtonPressedGradientEnd color.</summary>
    public static Color ButtonPressedGradientEnd => Table.ButtonPressedGradientEnd;

    /// <summary>Gets the CheckBackground color.</summary>
    public static Color CheckBackground => Table.CheckBackground;

    /// <summary>Gets the CheckSelectedBackground color.</summary>
    public static Color CheckSelectedBackground => Table.CheckSelectedBackground;

    /// <summary>Gets the CheckPressedBackground color.</summary>
    public static Color CheckPressedBackground => Table.CheckPressedBackground;

    /// <summary>Gets the GripDark color.</summary>
    public static Color GripDark => Table.GripDark;

    /// <summary>Gets the GripLight color.</summary>
    public static Color GripLight => Table.GripLight;

    /// <summary>Gets the ImageMarginGradientBegin color.</summary>
    public static Color ImageMarginGradientBegin => Table.ImageMarginGradientBegin;

    /// <summary>Gets the ImageMarginGradientMiddle color.</summary>
    public static Color ImageMarginGradientMiddle => Table.ImageMarginGradientMiddle;

    /// <summary>Gets the ImageMarginGradientEnd color.</summary>
    public static Color ImageMarginGradientEnd => Table.ImageMarginGradientEnd;

    /// <summary>Gets the ImageMarginRevealedGradientBegin color.</summary>
    public static Color ImageMarginRevealedGradientBegin => Table.ImageMarginRevealedGradientBegin;

    /// <summary>Gets the ImageMarginRevealedGradientMiddle color.</summary>
    public static Color ImageMarginRevealedGradientMiddle => Table.ImageMarginRevealedGradientMiddle;

    /// <summary>Gets the ImageMarginRevealedGradientEnd color.</summary>
    public static Color ImageMarginRevealedGradientEnd => Table.ImageMarginRevealedGradientEnd;

    /// <summary>Gets the MenuStripGradientBegin color.</summary>
    public static Color MenuStripGradientBegin => Table.MenuStripGradientBegin;

    /// <summary>Gets the MenuStripGradientEnd color.</summary>
    public static Color MenuStripGradientEnd => Table.MenuStripGradientEnd;

    /// <summary>Gets the MenuItemSelected color.</summary>
    public static Color MenuItemSelected => Table.MenuItemSelected;

    /// <summary>Gets the MenuItemBorder color.</summary>
    public static Color MenuItemBorder => Table.MenuItemBorder;

    /// <summary>Gets the MenuBorder color.</summary>
    public static Color MenuBorder => Table.MenuBorder;

    /// <summary>Gets the MenuItemSelectedGradientBegin color.</summary>
    public static Color MenuItemSelectedGradientBegin => Table.MenuItemSelectedGradientBegin;

    /// <summary>Gets the MenuItemSelectedGradientEnd color.</summary>
    public static Color MenuItemSelectedGradientEnd => Table.MenuItemSelectedGradientEnd;

    /// <summary>Gets the MenuItemPressedGradientBegin color.</summary>
    public static Color MenuItemPressedGradientBegin => Table.MenuItemPressedGradientBegin;

    /// <summary>Gets the MenuItemPressedGradientMiddle color.</summary>
    public static Color MenuItemPressedGradientMiddle => Table.MenuItemPressedGradientMiddle;

    /// <summary>Gets the MenuItemPressedGradientEnd color.</summary>
    public static Color MenuItemPressedGradientEnd => Table.MenuItemPressedGradientEnd;

    /// <summary>Gets the RaftingContainerGradientBegin color.</summary>
    public static Color RaftingContainerGradientBegin => Table.RaftingContainerGradientBegin;

    /// <summary>Gets the RaftingContainerGradientEnd color.</summary>
    public static Color RaftingContainerGradientEnd => Table.RaftingContainerGradientEnd;

    /// <summary>Gets the SeparatorDark color.</summary>
    public static Color SeparatorDark => Table.SeparatorDark;

    /// <summary>Gets the SeparatorLight color.</summary>
    public static Color SeparatorLight => Table.SeparatorLight;

    /// <summary>Gets the StatusStripGradientBegin color.</summary>
    public static Color StatusStripGradientBegin => Table.StatusStripGradientBegin;

    /// <summary>Gets the StatusStripGradientEnd color.</summary>
    public static Color StatusStripGradientEnd => Table.StatusStripGradientEnd;

    /// <summary>Gets the ToolStripBorder color.</summary>
    public static Color ToolStripBorder => Table.ToolStripBorder;

    /// <summary>Gets the ToolStripDropDownBackground color.</summary>
    public static Color ToolStripDropDownBackground => Table.ToolStripDropDownBackground;

    /// <summary>Gets the ToolStripGradientBegin color.</summary>
    public static Color ToolStripGradientBegin => Table.ToolStripGradientBegin;

    /// <summary>Gets the ToolStripGradientMiddle color.</summary>
    public static Color ToolStripGradientMiddle => Table.ToolStripGradientMiddle;

    /// <summary>Gets the ToolStripGradientEnd color.</summary>
    public static Color ToolStripGradientEnd => Table.ToolStripGradientEnd;

    /// <summary>Gets the ToolStripContentPanelGradientBegin color.</summary>
    public static Color ToolStripContentPanelGradientBegin => Table.ToolStripContentPanelGradientBegin;

    /// <summary>Gets the ToolStripContentPanelGradientEnd color.</summary>
    public static Color ToolStripContentPanelGradientEnd => Table.ToolStripContentPanelGradientEnd;

    /// <summary>Gets the ToolStripPanelGradientBegin color.</summary>
    public static Color ToolStripPanelGradientBegin => Table.ToolStripPanelGradientBegin;

    /// <summary>Gets the ToolStripPanelGradientEnd color.</summary>
    public static Color ToolStripPanelGradientEnd => Table.ToolStripPanelGradientEnd;

    /// <summary>Gets the OverflowButtonGradientBegin color.</summary>
    public static Color OverflowButtonGradientBegin => Table.OverflowButtonGradientBegin;

    /// <summary>Gets the OverflowButtonGradientMiddle color.</summary>
    public static Color OverflowButtonGradientMiddle => Table.OverflowButtonGradientMiddle;

    /// <summary>Gets the OverflowButtonGradientEnd color.</summary>
    public static Color OverflowButtonGradientEnd => Table.OverflowButtonGradientEnd;
}
