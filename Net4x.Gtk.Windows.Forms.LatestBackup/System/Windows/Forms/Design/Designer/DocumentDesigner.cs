// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides a designer that extends the ScrollableControlDesigner and implements
/// IRootDesigner.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms")]
public partial class DocumentDesigner : ScrollableControlDesigner, IRootDesigner, IToolboxUser, IOleDragClient
{

    internal static IDesignerSerializationManager manager;

    private const string AxClipFormat = "CLSID";

    // ActiveX support
    //
    private static readonly Guid htmlDesignTime = new Guid("73CEF3DD-AE85-11CF-A406-00AA00C00940");

    private readonly bool initializing;   // is the designer initializing?

    // used to keep the state of the tab order view
    //

    private readonly bool queriedTabOrder;

    private readonly MenuCommand tabOrderCommand;

    private readonly bool trayAutoArrange;

    private readonly int trayHeight = 80;

    private readonly bool trayLargeIcon;

    private readonly bool trayLayoutSuspended;

    /// <summary>
    /// Gets the set of technologies that this designer can support for its display.
    /// </summary>
    /// <value>
    /// The supported technologies.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public ViewTechnology[] SupportedTechnologies => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the specified tool is supported by the current designer.
    /// </summary>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to be tested for toolbox support.
    /// </param>
    /// <returns>
    /// true if the tool is supported by the toolbox and can be enabled; false if the document designer does not know how to use the tool.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public bool GetToolSupported(ToolboxItem tool)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a view object for the specified view technology.
    /// </summary>
    /// <param name="technology">
    /// A
    /// <see cref="T:System.ComponentModel.Design.ViewTechnology">
    /// </see> that indicates a particular view technology.
    /// </param>
    /// <returns>
    /// An object that represents the view for this designer.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public object GetView(ViewTechnology technology)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Selects the specified tool.
    /// </summary>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to select.
    /// </param>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public void ToolPicked(ToolboxItem tool)
    {
        throw new NotImplementedException();
    }

}