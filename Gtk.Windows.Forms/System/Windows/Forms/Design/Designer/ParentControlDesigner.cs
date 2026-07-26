// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <summary>
/// The ParentControlDesigner class builds on the ControlDesigner.  It adds the ability
/// to manipulate child components, and provides a selection UI handler for all
/// components it contains.
/// </summary>
public class ParentControlDesigner : ControlDesigner, IOleDragClient
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
    public bool CanModifyComponents
    {
        get
        {
            // Basic implementation: allow modification if we have a Control as root
            return Component is Control;
        }
    }

    public override IComponent? Component { get; set; }

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
    public bool AddComponent(IComponent component, string name, bool firstAdd)
    {
        if (!CanModifyComponents || component is null)
        {
            return false;
        }

        if (Component is not Control parent)
        {
            return false;
        }

        if (component is Control control)
        {
            // Assign name if possible
            if (!string.IsNullOrEmpty(name))
            {
                control.Name = name;
            }

            parent.Controls?.Add(control);

            // Basic positioning for first add
            if (firstAdd)
            {
                control.Location = new System.Drawing.Point(10, 10);
            }

            return true;
        }

        return false;
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
    public Control GetControlForComponent(object component)
    {
        if (component is Control control)
        {
            return control;
        }

        throw new ArgumentException(@"Component is not a Control.", nameof(component));
    }

    /// <summary>
    /// Retrieves the control view instance for the designer that is hosting the drag.
    /// </summary>
    /// <returns>
    /// Control.
    /// </returns>
    public Control GetDesignerControl()
    {
        if (Component is Control control)
        {
            return control;
        }

        throw new InvalidOperationException("Designer is not associated with a Control.");
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
    public bool IsDropOk(IComponent component)
    {
        if (!CanModifyComponents || component is null)
        {
            return false;
        }

        // Only allow dropping controls onto controls
        return component is Control;
    }
}