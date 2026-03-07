// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    internal interface IOleDragClient
    {

        /// <summary>
        /// Checks if the client is read only.  That is, if components can
        /// be added or removed from the designer.
        /// </summary>
        bool CanModifyComponents { get; }

        IComponent Component { get; }

        /// <summary>
        /// Retrieves the control view instance for the designer that
        /// is hosting the drag.
        /// </summary>
        bool AddComponent(IComponent component, string name, bool firstAdd);

        /// <summary>
        /// Retrieves the control view instance for the given component.
        /// For Win32 designer, this will often be the component itself.
        /// </summary>
        Control GetControlForComponent(object component);

        /// <summary>
        /// Retrieves the control view instance for the designer that
        /// is hosting the drag.
        /// </summary>
        Control GetDesignerControl();

        /// <summary>
        /// Checks if it is valid to drop this type of a component on this client.
        /// </summary>
        bool IsDropOk(IComponent component);
    }
}
