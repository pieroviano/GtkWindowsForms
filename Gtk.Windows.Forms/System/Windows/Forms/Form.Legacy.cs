// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// The pre-2.0 lifecycle events (<see cref="E:System.Windows.Forms.Form.Closing" />,
/// <see cref="E:System.Windows.Forms.Form.Closed" />, <see cref="E:System.Windows.Forms.Form.Activated" />,
/// <see cref="E:System.Windows.Forms.Form.Deactivate" />) plus owned-form management, parent-form access,
/// validation and centering. These mirror <c>System.Windows.Forms.Form</c> so source that predates
/// <c>FormClosing</c>/<c>FormClosed</c> — a very common partial surface — keeps compiling and behaving.
/// </summary>
public partial class Form
{
    private readonly List<Form> _ownedForms = new();

    /// <summary>Occurs when the form is closing. Superseded by <see cref="E:System.Windows.Forms.Form.FormClosing" />.</summary>
    public event CancelEventHandler? Closing;

    /// <summary>Occurs when the form is closed. Superseded by <see cref="E:System.Windows.Forms.Form.FormClosed" />.</summary>
    public event EventHandler? Closed;

    /// <summary>Occurs when the form is activated in code or by the user.</summary>
    public event EventHandler? Activated;

    /// <summary>Occurs when the form loses focus and is no longer the active form.</summary>
    public event EventHandler? Deactivate;

    /// <summary>Gets an array of <see cref="T:System.Windows.Forms.Form" /> objects that represent all forms that are owned by this form.</summary>
    public Form[] OwnedForms => _ownedForms.ToArray();

    /// <summary>Gets the form that owns this form (its parent in an owner relationship), or <see langword="null" />.</summary>
    public Form? ParentForm { get; internal set; }

    /// <summary>Adds an owned form to this form.</summary>
    /// <param name="ownedForm">The form to be added.</param>
    public void AddOwnedForm(Form ownedForm)
    {
        if (ownedForm == null)
        {
            return;
        }

        if (!_ownedForms.Contains(ownedForm))
        {
            _ownedForms.Add(ownedForm);
            ownedForm.ParentForm = this;
        }
    }

    /// <summary>Removes an owned form from this form.</summary>
    /// <param name="ownedForm">The form to be removed.</param>
    public void RemoveOwnedForm(Form ownedForm)
    {
        if (ownedForm != null && _ownedForms.Remove(ownedForm))
        {
            ownedForm.ParentForm = null;
        }
    }

    /// <summary>Validates the last invalidated control and its ancestors up through, but not including, the current control.</summary>
    /// <returns><see langword="true" /> if validation succeeds; otherwise <see langword="false" />.</returns>
    public bool Validate() => true;

    /// <summary>Causes all of the child controls that support validation to validate their data.</summary>
    /// <returns><see langword="true" /> if all validations succeed; otherwise <see langword="false" />.</returns>
    public bool ValidateChildren() => true;

    /// <summary>Causes all of the child controls within the given constraints to validate their data.</summary>
    /// <param name="validationConstraints">Determines which children are validated.</param>
    /// <returns><see langword="true" /> if all validations succeed; otherwise <see langword="false" />.</returns>
    public bool ValidateChildren(ValidationConstraints validationConstraints) => true;

    /// <summary>Centers the position of the form within the bounds of the screen.</summary>
    protected void CenterToScreen()
    {
        self.SetPosition(Gtk.WindowPosition.Center);
    }

    /// <summary>Centers the position of the form within the bounds of its parent form.</summary>
    protected void CenterToParent()
    {
        self.SetPosition(Gtk.WindowPosition.CenterOnParent);
    }

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Closing" /> event.</summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
    protected virtual void OnClosing(CancelEventArgs e) => Closing?.Invoke(this, e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Activated" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnActivated(EventArgs e) => Activated?.Invoke(this, e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Deactivate" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnDeactivate(EventArgs e) => Deactivate?.Invoke(this, e);
}
