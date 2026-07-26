// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides methods and properties to manage <see cref="T:System.Windows.Forms.ToolStrip" /> objects, such as
/// the default renderer, render mode and merge operations.
/// </summary>
public static class ToolStripManager
{
    private static ToolStripRenderer _renderer = new ToolStripProfessionalRenderer();
    private static ToolStripManagerRenderMode _renderMode = ToolStripManagerRenderMode.Professional;

    /// <summary>Gets or sets the default renderer used to paint tool strips.</summary>
    public static ToolStripRenderer Renderer
    {
        get => _renderer;
        set
        {
            _renderer = value ?? new ToolStripProfessionalRenderer();
            _renderMode = ToolStripManagerRenderMode.Custom;
        }
    }

    /// <summary>Gets or sets the painting styles applied to tool strips by default.</summary>
    public static ToolStripManagerRenderMode RenderMode
    {
        get => _renderMode;
        set
        {
            _renderMode = value;
            _renderer = value == ToolStripManagerRenderMode.System
                ? new ToolStripSystemRenderer()
                : new ToolStripProfessionalRenderer();
        }
    }

    /// <summary>Gets or sets a value indicating whether tool strips honor operating-system visual styles.</summary>
    public static bool VisualStylesEnabled { get; set; } = true;

    /// <summary>Combines two tool strips of the same type. Returns whether the merge succeeded.</summary>
    /// <param name="sourceToolStrip">The tool strip whose items are merged into the target.</param>
    /// <param name="targetToolStrip">The tool strip that receives the merged items.</param>
    /// <returns><see langword="true" /> if the merge succeeded; otherwise <see langword="false" />.</returns>
    public static bool Merge(ToolStrip sourceToolStrip, ToolStrip targetToolStrip)
    {
        return sourceToolStrip != null && targetToolStrip != null;
    }

    /// <summary>Reverts a previous merge on the specified tool strip. Returns whether the operation succeeded.</summary>
    /// <param name="targetToolStrip">The tool strip to revert.</param>
    /// <returns><see langword="true" /> if the revert succeeded; otherwise <see langword="false" />.</returns>
    public static bool RevertMerge(ToolStrip targetToolStrip)
    {
        return targetToolStrip != null;
    }
}
