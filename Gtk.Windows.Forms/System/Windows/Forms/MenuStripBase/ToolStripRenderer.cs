// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Handles the painting functionality for <see cref="T:System.Windows.Forms.ToolStrip" /> objects.
/// On this GTK layer the actual chrome is drawn by the theme, so the base render methods are no-ops; the
/// public Draw methods and the protected OnRender override points are provided so custom renderers compile
/// and their overrides are invoked.
/// </summary>
public abstract class ToolStripRenderer
{
    /// <summary>Draws the Arrow. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderArrow(System.Windows.Forms.ToolStripArrowRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripArrowRenderEventArgs" /> that contains the event data.</param>
    public void DrawArrow(ToolStripArrowRenderEventArgs e) => OnRenderArrow(e);

    /// <summary>Raises the Arrow render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripArrowRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
    }

    /// <summary>Draws the ToolStripBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    public void DrawToolStripBackground(ToolStripRenderEventArgs e) => OnRenderToolStripBackground(e);

    /// <summary>Raises the ToolStripBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
    }

    /// <summary>Draws the Grip. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderGrip(System.Windows.Forms.ToolStripGripRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripGripRenderEventArgs" /> that contains the event data.</param>
    public void DrawGrip(ToolStripGripRenderEventArgs e) => OnRenderGrip(e);

    /// <summary>Raises the Grip render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripGripRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
    }

    /// <summary>Draws the ItemBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderItemBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawItemBackground(ToolStripItemRenderEventArgs e) => OnRenderItemBackground(e);

    /// <summary>Raises the ItemBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the ImageMargin. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderImageMargin(System.Windows.Forms.ToolStripRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    public void DrawImageMargin(ToolStripRenderEventArgs e) => OnRenderImageMargin(e);

    /// <summary>Raises the ImageMargin render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
    }

    /// <summary>Draws the LabelBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderLabelBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawLabelBackground(ToolStripItemRenderEventArgs e) => OnRenderLabelBackground(e);

    /// <summary>Raises the LabelBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the ButtonBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawButtonBackground(ToolStripItemRenderEventArgs e) => OnRenderButtonBackground(e);

    /// <summary>Raises the ButtonBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the ToolStripBorder. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderToolStripBorder(System.Windows.Forms.ToolStripRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    public void DrawToolStripBorder(ToolStripRenderEventArgs e) => OnRenderToolStripBorder(e);

    /// <summary>Raises the ToolStripBorder render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
    }

    /// <summary>Draws the DropDownButtonBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderDropDownButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawDropDownButtonBackground(ToolStripItemRenderEventArgs e) => OnRenderDropDownButtonBackground(e);

    /// <summary>Raises the DropDownButtonBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the OverflowButtonBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderOverflowButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawOverflowButtonBackground(ToolStripItemRenderEventArgs e) => OnRenderOverflowButtonBackground(e);

    /// <summary>Raises the OverflowButtonBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the ItemImage. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderItemImage(System.Windows.Forms.ToolStripItemImageRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemImageRenderEventArgs" /> that contains the event data.</param>
    public void DrawItemImage(ToolStripItemImageRenderEventArgs e) => OnRenderItemImage(e);

    /// <summary>Raises the ItemImage render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemImageRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
    }

    /// <summary>Draws the ItemCheck. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderItemCheck(System.Windows.Forms.ToolStripItemImageRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemImageRenderEventArgs" /> that contains the event data.</param>
    public void DrawItemCheck(ToolStripItemImageRenderEventArgs e) => OnRenderItemCheck(e);

    /// <summary>Raises the ItemCheck render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemImageRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
    }

    /// <summary>Draws the ItemText. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderItemText(System.Windows.Forms.ToolStripItemTextRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemTextRenderEventArgs" /> that contains the event data.</param>
    public void DrawItemText(ToolStripItemTextRenderEventArgs e) => OnRenderItemText(e);

    /// <summary>Raises the ItemText render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemTextRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
    }

    /// <summary>Draws the MenuItemBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderMenuItemBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawMenuItemBackground(ToolStripItemRenderEventArgs e) => OnRenderMenuItemBackground(e);

    /// <summary>Raises the MenuItemBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the SplitButton. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderSplitButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawSplitButton(ToolStripItemRenderEventArgs e) => OnRenderSplitButtonBackground(e);

    /// <summary>Raises the SplitButton render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the ToolStripStatusLabelBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderToolStripStatusLabelBackground(System.Windows.Forms.ToolStripItemRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    public void DrawToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) => OnRenderToolStripStatusLabelBackground(e);

    /// <summary>Raises the ToolStripStatusLabelBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripItemRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>Draws the StatusStripSizingGrip. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderStatusStripSizingGrip(System.Windows.Forms.ToolStripRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    public void DrawStatusStripSizingGrip(ToolStripRenderEventArgs e) => OnRenderStatusStripSizingGrip(e);

    /// <summary>Raises the StatusStripSizingGrip render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
    }

    /// <summary>Draws the Separator. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderSeparator(System.Windows.Forms.ToolStripSeparatorRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripSeparatorRenderEventArgs" /> that contains the event data.</param>
    public void DrawSeparator(ToolStripSeparatorRenderEventArgs e) => OnRenderSeparator(e);

    /// <summary>Raises the Separator render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripSeparatorRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
    }

    /// <summary>Draws the ToolStripPanelBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderToolStripPanelBackground(System.Windows.Forms.ToolStripPanelRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripPanelRenderEventArgs" /> that contains the event data.</param>
    public void DrawToolStripPanelBackground(ToolStripPanelRenderEventArgs e) => OnRenderToolStripPanelBackground(e);

    /// <summary>Raises the ToolStripPanelBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripPanelRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
    {
    }

    /// <summary>Draws the ToolStripContentPanelBackground. Raises <see cref="M:System.Windows.Forms.ToolStripRenderer.OnRenderToolStripContentPanelBackground(System.Windows.Forms.ToolStripContentPanelRenderEventArgs)" />.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripContentPanelRenderEventArgs" /> that contains the event data.</param>
    public void DrawToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) => OnRenderToolStripContentPanelBackground(e);

    /// <summary>Raises the ToolStripContentPanelBackground render event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripContentPanelRenderEventArgs" /> that contains the event data.</param>
    protected virtual void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
    {
    }
}
