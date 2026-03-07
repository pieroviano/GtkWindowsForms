// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.Layout" />
    /// event. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public sealed class LayoutEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LayoutEventArgs" /> class with the specified component and property affected.
        /// </summary>
        /// <param name="affectedComponent">The
        /// <see cref="System.ComponentModel.Component" /> affected by the layout change.
        /// </param>
        /// <param name="affectedProperty">The property affected by the layout change.
        /// </param>
        public LayoutEventArgs(IComponent affectedComponent, string affectedProperty)
        {
            AffectedComponent = affectedComponent;
            AffectedProperty = affectedProperty;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LayoutEventArgs" /> class with the specified control and property affected.
        /// </summary>
        /// <param name="affectedControl">The
        /// <see cref="System.Windows.Forms.Control" /> affected by the layout change.
        /// </param>
        /// <param name="affectedProperty">The property affected by the layout change.
        /// </param>
        public LayoutEventArgs(Control affectedControl, string affectedProperty) : this((IComponent)affectedControl, affectedProperty)
        {
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.ComponentModel.Component" /> affected by the layout change.
        /// </summary>
        /// <returns>An
        /// <see cref="System.ComponentModel.IComponent" /> representing the
        /// <see cref="System.ComponentModel.Component" /> affected by the layout change.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IComponent AffectedComponent { get; }

        /// <summary>
        /// Gets the child control affected by the change.
        /// </summary>
        /// <returns>The child
        /// <see cref="System.Windows.Forms.Control" /> affected by the change.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Control AffectedControl => AffectedComponent as Control;

        /// <summary>
        /// Gets the property affected by the change.
        /// </summary>
        /// <returns>The property affected by the change.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string AffectedProperty { get; }
    }
}
