// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides colors used for Microsoft Office display elements. The values are derived from the current
/// system colors; on this GTK layer actual chrome is drawn by the theme, so these serve source that
/// reads the table rather than pixel-exact Office rendering.
/// </summary>
public class ProfessionalColorTable
{
    /// <summary>Gets or sets a value indicating whether rounded edges are used for menus and tool strips.</summary>
    public bool UseSystemColors { get; set; }

    /// <summary>Gets the color used to render the ButtonSelectedHighlight element.</summary>
    public virtual Color ButtonSelectedHighlight => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonSelectedHighlightBorder element.</summary>
    public virtual Color ButtonSelectedHighlightBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonPressedHighlight element.</summary>
    public virtual Color ButtonPressedHighlight => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonPressedHighlightBorder element.</summary>
    public virtual Color ButtonPressedHighlightBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonCheckedHighlight element.</summary>
    public virtual Color ButtonCheckedHighlight => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonCheckedHighlightBorder element.</summary>
    public virtual Color ButtonCheckedHighlightBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonPressedBorder element.</summary>
    public virtual Color ButtonPressedBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonSelectedBorder element.</summary>
    public virtual Color ButtonSelectedBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonCheckedGradientBegin element.</summary>
    public virtual Color ButtonCheckedGradientBegin => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonCheckedGradientMiddle element.</summary>
    public virtual Color ButtonCheckedGradientMiddle => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonCheckedGradientEnd element.</summary>
    public virtual Color ButtonCheckedGradientEnd => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonSelectedGradientBegin element.</summary>
    public virtual Color ButtonSelectedGradientBegin => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonSelectedGradientMiddle element.</summary>
    public virtual Color ButtonSelectedGradientMiddle => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonSelectedGradientEnd element.</summary>
    public virtual Color ButtonSelectedGradientEnd => SystemColors.Highlight;

    /// <summary>Gets the color used to render the ButtonPressedGradientBegin element.</summary>
    public virtual Color ButtonPressedGradientBegin => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonPressedGradientMiddle element.</summary>
    public virtual Color ButtonPressedGradientMiddle => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ButtonPressedGradientEnd element.</summary>
    public virtual Color ButtonPressedGradientEnd => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the CheckBackground element.</summary>
    public virtual Color CheckBackground => SystemColors.Highlight;

    /// <summary>Gets the color used to render the CheckSelectedBackground element.</summary>
    public virtual Color CheckSelectedBackground => SystemColors.Highlight;

    /// <summary>Gets the color used to render the CheckPressedBackground element.</summary>
    public virtual Color CheckPressedBackground => SystemColors.Highlight;

    /// <summary>Gets the color used to render the GripDark element.</summary>
    public virtual Color GripDark => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the GripLight element.</summary>
    public virtual Color GripLight => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ImageMarginGradientBegin element.</summary>
    public virtual Color ImageMarginGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ImageMarginGradientMiddle element.</summary>
    public virtual Color ImageMarginGradientMiddle => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ImageMarginGradientEnd element.</summary>
    public virtual Color ImageMarginGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the ImageMarginRevealedGradientBegin element.</summary>
    public virtual Color ImageMarginRevealedGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ImageMarginRevealedGradientMiddle element.</summary>
    public virtual Color ImageMarginRevealedGradientMiddle => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ImageMarginRevealedGradientEnd element.</summary>
    public virtual Color ImageMarginRevealedGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the MenuStripGradientBegin element.</summary>
    public virtual Color MenuStripGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the MenuStripGradientEnd element.</summary>
    public virtual Color MenuStripGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the MenuItemSelected element.</summary>
    public virtual Color MenuItemSelected => SystemColors.Highlight;

    /// <summary>Gets the color used to render the MenuItemBorder element.</summary>
    public virtual Color MenuItemBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the MenuBorder element.</summary>
    public virtual Color MenuBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the MenuItemSelectedGradientBegin element.</summary>
    public virtual Color MenuItemSelectedGradientBegin => SystemColors.Highlight;

    /// <summary>Gets the color used to render the MenuItemSelectedGradientEnd element.</summary>
    public virtual Color MenuItemSelectedGradientEnd => SystemColors.Highlight;

    /// <summary>Gets the color used to render the MenuItemPressedGradientBegin element.</summary>
    public virtual Color MenuItemPressedGradientBegin => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the MenuItemPressedGradientMiddle element.</summary>
    public virtual Color MenuItemPressedGradientMiddle => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the MenuItemPressedGradientEnd element.</summary>
    public virtual Color MenuItemPressedGradientEnd => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the RaftingContainerGradientBegin element.</summary>
    public virtual Color RaftingContainerGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the RaftingContainerGradientEnd element.</summary>
    public virtual Color RaftingContainerGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the SeparatorDark element.</summary>
    public virtual Color SeparatorDark => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the SeparatorLight element.</summary>
    public virtual Color SeparatorLight => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the StatusStripGradientBegin element.</summary>
    public virtual Color StatusStripGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the StatusStripGradientEnd element.</summary>
    public virtual Color StatusStripGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the ToolStripBorder element.</summary>
    public virtual Color ToolStripBorder => SystemColors.ControlDark;

    /// <summary>Gets the color used to render the ToolStripDropDownBackground element.</summary>
    public virtual Color ToolStripDropDownBackground => SystemColors.Window;

    /// <summary>Gets the color used to render the ToolStripGradientBegin element.</summary>
    public virtual Color ToolStripGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ToolStripGradientMiddle element.</summary>
    public virtual Color ToolStripGradientMiddle => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ToolStripGradientEnd element.</summary>
    public virtual Color ToolStripGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the ToolStripContentPanelGradientBegin element.</summary>
    public virtual Color ToolStripContentPanelGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ToolStripContentPanelGradientEnd element.</summary>
    public virtual Color ToolStripContentPanelGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the ToolStripPanelGradientBegin element.</summary>
    public virtual Color ToolStripPanelGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the ToolStripPanelGradientEnd element.</summary>
    public virtual Color ToolStripPanelGradientEnd => SystemColors.Control;

    /// <summary>Gets the color used to render the OverflowButtonGradientBegin element.</summary>
    public virtual Color OverflowButtonGradientBegin => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the OverflowButtonGradientMiddle element.</summary>
    public virtual Color OverflowButtonGradientMiddle => SystemColors.ControlLightLight;

    /// <summary>Gets the color used to render the OverflowButtonGradientEnd element.</summary>
    public virtual Color OverflowButtonGradientEnd => SystemColors.Control;
}
