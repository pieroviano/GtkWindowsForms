// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

/// <summary>
/// Provides notifications of toolbox actions to designers which implement this interface.
/// </summary>
public interface IToolboxUser
{
    /// <summary>
    /// Gets a value indicating whether the specified tool is supported by the current designer.
    /// </summary>
    /// <returns>
    /// true if the tool is supported by the toolbox and can be enabled; false if the document designer does not know how to use the tool.
    /// </returns>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to be tested for toolbox support.
    /// </param>
    bool GetToolSupported(ToolboxItem tool);

    /// <summary>
    /// Selects the specified tool.
    /// </summary>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to select.
    /// </param>
    void ToolPicked(ToolboxItem tool);

}