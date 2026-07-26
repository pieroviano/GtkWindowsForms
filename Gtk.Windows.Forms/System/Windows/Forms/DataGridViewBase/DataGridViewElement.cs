// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The base class of the elements of a <see cref="DataGridView" /> — bands (rows and columns) and cells.
/// It carries the element's <see cref="State" /> and owning <see cref="DataGridView" /> and the protected
/// notification hooks. It was absent, so <c>is DataGridViewElement</c> and the element hierarchy did not
/// exist; leaf types keep their own <see cref="State" />/<see cref="DataGridView" /> behaviour by
/// overriding these virtuals.
/// </summary>
public class DataGridViewElement
{
    /// <summary>Initializes a new instance of the <see cref="DataGridViewElement" /> class.</summary>
    public DataGridViewElement()
    {
    }

    /// <summary>Gets the user-interface state of the element.</summary>
    public virtual DataGridViewElementStates State { get; internal set; }

    /// <summary>Gets the <see cref="DataGridView" /> that owns this element.</summary>
    public virtual DataGridView? DataGridView { get; internal set; }

    /// <summary>Called when the owning <see cref="DataGridView" /> changes.</summary>
    protected virtual void OnDataGridViewChanged()
    {
    }

    /// <summary>Raises the <c>CellClick</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseCellClick(DataGridViewCellEventArgs e)
    {
    }

    /// <summary>Raises the <c>CellContentClick</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseCellContentClick(DataGridViewCellEventArgs e)
    {
    }

    /// <summary>Raises the <c>CellContentDoubleClick</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
    {
    }

    /// <summary>Raises the <c>CellValueChanged</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseCellValueChanged(DataGridViewCellEventArgs e)
    {
    }

    /// <summary>Raises the <c>DataError</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseDataError(DataGridViewDataErrorEventArgs e)
    {
    }

    /// <summary>Raises the <c>MouseWheel</c> event on the owning grid.</summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void RaiseMouseWheel(MouseEventArgs e)
    {
    }
}
