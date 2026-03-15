// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

/// <summary>
/// Class PropertyGrid. Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" /> Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// Class PropertyGrid. Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" /> Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// Class PropertyGrid. Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" /> Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// Class PropertyGrid. Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" /> Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
public partial class PropertyGrid
{
    private record TabInfo(PropertyTab Tab, PropertyTabScope Scope, ToolStripButton Button)
    {
        public ToolStripButton Button { get; } = Button;

        public PropertyTabScope Scope { get; } = Scope;

        public PropertyTab Tab { get; } = Tab;

        public Type TabType => Tab.GetType();

        public void Deconstruct(out PropertyTab Tab, out PropertyTabScope Scope, out ToolStripButton Button)
        {
            Tab = this.Tab;
            Scope = this.Scope;
            Button = this.Button;
        }

    }
}
