// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>Provides pop-up or online Help for controls. On the GTK layer the Help associations are stored;
/// invoking Help is a no-op boundary.</summary>
[ProvideProperty("HelpString", typeof(Control))]
[ProvideProperty("HelpKeyword", typeof(Control))]
[ProvideProperty("HelpNavigator", typeof(Control))]
[ProvideProperty("ShowHelp", typeof(Control))]
public class HelpProvider : Component, IExtenderProvider
{
    private readonly Dictionary<Control, string> strings = new();
    private readonly Dictionary<Control, string> keywords = new();
    private readonly Dictionary<Control, HelpNavigator> navigators = new();
    private readonly Dictionary<Control, bool> showHelp = new();

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.HelpProvider" /> class.</summary>
    public HelpProvider() { }

    /// <summary>Gets or sets a value specifying the name of the Help file associated with this provider.</summary>
    public virtual string? HelpNamespace { get; set; }

    /// <summary>Gets or sets user-defined data associated with this provider.</summary>
    public object? Tag { get; set; }

    /// <summary>Determines whether this provider can offer an extender property to the specified target.</summary>
    /// <param name="target">The target object.</param>
    /// <returns><see langword="true" /> if the target is a <see cref="T:System.Windows.Forms.Control" />.</returns>
    public virtual bool CanExtend(object target) => target is Control;

    /// <summary>Gets the Help keyword associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <returns>The keyword, or <see langword="null" />.</returns>
    public virtual string? GetHelpKeyword(Control ctl) => keywords.TryGetValue(ctl, out var v) ? v : null;

    /// <summary>Gets the Help navigator associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <returns>The navigator.</returns>
    public virtual HelpNavigator GetHelpNavigator(Control ctl) => navigators.TryGetValue(ctl, out var v) ? v : HelpNavigator.AssociateIndex;

    /// <summary>Gets the Help string associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <returns>The Help string, or <see langword="null" />.</returns>
    public virtual string? GetHelpString(Control ctl) => strings.TryGetValue(ctl, out var v) ? v : null;

    /// <summary>Gets a value indicating whether Help is displayed for the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <returns><see langword="true" /> if Help is shown; otherwise <see langword="false" />.</returns>
    public virtual bool GetShowHelp(Control ctl) => showHelp.TryGetValue(ctl, out var v) && v;

    /// <summary>Specifies the Help string associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <param name="helpString">The Help string.</param>
    public virtual void SetHelpString(Control ctl, string helpString) { strings[ctl] = helpString; showHelp[ctl] = true; }

    /// <summary>Specifies the Help keyword associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <param name="keyword">The keyword.</param>
    public virtual void SetHelpKeyword(Control ctl, string keyword) { keywords[ctl] = keyword; showHelp[ctl] = true; }

    /// <summary>Specifies the Help navigator associated with the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <param name="navigator">The navigator.</param>
    public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator) { navigators[ctl] = navigator; showHelp[ctl] = true; }

    /// <summary>Specifies whether Help is displayed for the control.</summary>
    /// <param name="ctl">The control.</param>
    /// <param name="value">Whether to show Help.</param>
    public virtual void SetShowHelp(Control ctl, bool value) { showHelp[ctl] = value; }

    /// <summary>Resets the Help display for the control.</summary>
    /// <param name="ctl">The control.</param>
    public virtual void ResetShowHelp(Control ctl) { showHelp.Remove(ctl); }

    /// <summary>Returns a string that represents this provider.</summary>
    /// <returns>A string.</returns>
    public override string ToString() => base.ToString() + ", HelpNamespace: " + HelpNamespace;
}
