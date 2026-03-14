// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
/// The ParentControlDesigner class builds on the ControlDesigner.  It adds the ability
/// to manipulate child components, and provides a selection UI handler for all
/// components it contains.
/// </summary>
public partial class ParentControlDesigner : ControlDesigner, IOleDragClient
{
    /// <summary>
    /// Checks if the client is read only.  That is, if components can
    /// be added or removed from the designer.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance can modify components; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public bool CanModifyComponents => throw new NotImplementedException();

    /// <summary>
    /// Retrieves the control view instance for the designer that is hosting the drag.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="firstAdd">
    /// if set to
    /// <c>
    /// true
    /// </c>first add.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public bool AddComponent(IComponent component, string name, bool firstAdd)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the control view instance for the given component.
    /// For Win32 designer, this will often be the component itself.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <returns>
    /// Control.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Control GetControlForComponent(object component)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the control view instance for the designer that is hosting the drag.
    /// </summary>
    /// <returns>
    /// Control.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Control GetDesignerControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if it is valid to drop this type of a component on this client.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if is drop ok the specified component; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public bool IsDropOk(IComponent component)
    {
        throw new NotImplementedException();
    }

}