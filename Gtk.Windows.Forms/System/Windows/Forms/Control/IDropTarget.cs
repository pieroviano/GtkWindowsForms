// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines the drag-and-drop notification methods a drop target receives. A control or object that accepts
/// dropped data implements this interface so the drag pipeline can route <c>DragEnter</c>, <c>DragOver</c>,
/// <c>DragDrop</c> and <c>DragLeave</c> notifications to it with WinForms-compatible signatures.
/// </summary>
public interface IDropTarget
{
    /// <summary>Raises the <c>DragEnter</c> event as the pointer enters the target during a drag.</summary>
    /// <param name="e">The drag event data.</param>
    void OnDragEnter(DragEventArgs e);

    /// <summary>Raises the <c>DragLeave</c> event as the pointer leaves the target during a drag.</summary>
    /// <param name="e">The event data.</param>
    void OnDragLeave(EventArgs e);

    /// <summary>Raises the <c>DragDrop</c> event when data is dropped on the target.</summary>
    /// <param name="e">The drag event data.</param>
    void OnDragDrop(DragEventArgs e);

    /// <summary>Raises the <c>DragOver</c> event as the pointer moves over the target during a drag.</summary>
    /// <param name="e">The drag event data.</param>
    void OnDragOver(DragEventArgs e);
}
