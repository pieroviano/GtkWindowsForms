// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>Collects the flow-related characteristics associated with flow layout panels.</summary>
public class FlowLayoutSettings : LayoutSettings
{
    private static readonly LayoutEngine engine = new LayoutEngine();
    private readonly Dictionary<object, bool> flowBreaks = new();

    internal FlowLayoutSettings() { }

    /// <summary>Gets the layout engine that applies these settings.</summary>
    public override LayoutEngine LayoutEngine => engine;

    /// <summary>Gets or sets a value indicating the flow direction of the layout.</summary>
    public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

    /// <summary>Gets or sets a value indicating whether the contents should wrap.</summary>
    public bool WrapContents { get; set; } = true;

    /// <summary>Sets whether the flow breaks after the specified child.</summary>
    /// <param name="child">The child object.</param>
    /// <param name="value">Whether a flow break follows the child.</param>
    public void SetFlowBreak(object child, bool value) { flowBreaks[child] = value; }

    /// <summary>Gets whether the flow breaks after the specified child.</summary>
    /// <param name="child">The child object.</param>
    /// <returns><see langword="true" /> if a flow break follows the child; otherwise <see langword="false" />.</returns>
    public bool GetFlowBreak(object child) => flowBreaks.TryGetValue(child, out var v) && v;
}
