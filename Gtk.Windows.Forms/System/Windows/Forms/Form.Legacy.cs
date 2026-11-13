// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Gtk.Windows.Forms.Controls.ControlBase;

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
    /// <remarks>
    /// Sets <see cref="P:System.Windows.Forms.Control.Location" /> to the centered top-left, clamped to the
    /// working area's origin, exactly as <c>System.Windows.Forms.Form.CenterToScreen</c> does. (Gtk4 leaves
    /// the final on-screen placement to the window manager, the same repo-wide limitation that affects every
    /// Location assignment; this method still performs the WinForms computation and updates Location.)
    /// </remarks>
    protected void CenterToScreen()
    {
        var screen = Widget != null ? Screen.FromControl(this) : Screen.PrimaryScreen;
        if (screen != null)
        {
            var area = screen.WorkingArea;
            Location = new Point(
                Math.Max(area.X, area.X + (area.Width - Width) / 2),
                Math.Max(area.Y, area.Y + (area.Height - Height) / 2));
        }

        _centerRequest = CenterRequest.Screen;
        _centerOwner = null;
        RememberSettleCentering(CenterRequest.Screen, null);
        ApplyNativeCentering();
    }

    /// <summary>Centers the position of the form within the bounds of its parent form.</summary>
    /// <remarks>
    /// Centers on the owning form (<see cref="Owner" />, falling back to <see cref="ParentForm" />); with no
    /// owner it centers on the screen, matching <c>System.Windows.Forms.Form.CenterToParent</c>.
    /// </remarks>
    protected void CenterToParent()
    {
        var owner = Owner ?? ParentForm;
        if (owner == null)
        {
            CenterToScreen();
            return;
        }

        Location = new Point(
            owner.Left + (owner.Width - Width) / 2,
            owner.Top + (owner.Height - Height) / 2);

        _centerRequest = CenterRequest.Parent;
        _centerOwner = owner;
        RememberSettleCentering(CenterRequest.Parent, owner);
        ApplyNativeCentering();
    }

    private enum CenterRequest
    {
        None,
        Screen,
        Parent
    }

    private CenterRequest _centerRequest;
    private Form? _centerOwner;

    // The last center the form was asked for, kept sticky so the content-fit growth watch (EnsureContentFits)
    // can re-apply it while the window is still growing and once more when it settles. A single 150ms re-center
    // fired while the window was mid-growth, leaving the final (larger) window anchored at the opening size's
    // top-left - visibly off-center up and to the left.
    private CenterRequest _settleCenterMode;
    private Form? _settleCenterOwner;

    /// <summary>Records the center to re-apply as the window's content-driven size settles.</summary>
    private void RememberSettleCentering(CenterRequest mode, Form? owner)
    {
        _settleCenterMode = mode;
        _settleCenterOwner = owner;
    }

    /// <summary>
    /// Re-applies the remembered center. Called as the window grows to fit its content and once when that
    /// growth settles, so the final on-screen size is the one that ends up centered. No-op if no center was
    /// requested or the form is gone.
    /// </summary>
    internal void ReapplySettleCentering()
    {
        if (_settleCenterMode == CenterRequest.None || IsDisposed)
        {
            return;
        }

        _centerRequest = _settleCenterMode;
        _centerOwner = _settleCenterOwner;
        ApplyNativeCentering();
    }

    /// <summary>
    /// Applies a pending center natively. Gtk4 cannot position a top-level window through the managed API, so
    /// the actual move is done against the backend window (Win32 / X11 / macOS - see
    /// <see cref="NativeWindowPlacement" />). This is called both immediately from CenterToScreen/CenterToParent
    /// (effective when the native window already exists) and from the shown handler, so a center requested
    /// before the window was realized (e.g. from an <c>OnLoad</c> override, exactly as in classic WinForms)
    /// still takes effect once the window is up.
    /// </summary>
    internal void ApplyNativeCentering()
    {
        var applied = _centerRequest switch
        {
            CenterRequest.Screen => NativeWindowPlacement.CenterOnScreen(self),
            CenterRequest.Parent => NativeWindowPlacement.CenterOverOwner(self, _centerOwner?.self),
            _ => true
        };

        if (applied)
        {
            _centerRequest = CenterRequest.None;
            _centerOwner = null;
        }
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
