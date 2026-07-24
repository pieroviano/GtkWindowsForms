// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality required by a spin box (also known as an up-down control), the shared
/// base of <see cref="T:System.Windows.Forms.NumericUpDown" /> and
/// <see cref="T:System.Windows.Forms.DomainUpDown" />. This mirrors the real
/// <c>System.Windows.Forms.UpDownBase</c> so that source written against the shared surface — and type tests
/// such as <c>control is UpDownBase</c> — compile and behave.
/// </summary>
/// <remarks>
/// The value/text semantics differ per control, so <see cref="M:System.Windows.Forms.UpDownBase.UpButton" />
/// and <see cref="M:System.Windows.Forms.UpDownBase.DownButton" /> are abstract and supplied by the derived
/// control. Alignment and interception properties are carried here for source compatibility.
/// </remarks>
[DesignerCategory("Component")]
public abstract class UpDownBase : ContainerControl
{
    private bool _interceptArrowKeys = true;
    private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
    private LeftRightAlignment _upDownAlign = LeftRightAlignment.Right;

    /// <summary>
    /// Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select
    /// values.
    /// </summary>
    public bool InterceptArrowKeys
    {
        get => _interceptArrowKeys;
        set => _interceptArrowKeys = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons
    /// only.
    /// </summary>
    public virtual bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the alignment of the text in the up-down control.
    /// </summary>
    public HorizontalAlignment TextAlign
    {
        get => _textAlign;
        set => _textAlign = value;
    }

    /// <summary>
    /// Gets or sets the alignment of the up and down buttons on the up-down control.
    /// </summary>
    public LeftRightAlignment UpDownAlign
    {
        get => _upDownAlign;
        set => _upDownAlign = value;
    }

    /// <summary>
    /// Gets the height of the up-down control, in pixels.
    /// </summary>
    public int PreferredHeight => Height > 0 ? Height : 23;

    /// <summary>
    /// When overridden in a derived class, increments the value of the up-down control.
    /// </summary>
    public abstract void UpButton();

    /// <summary>
    /// When overridden in a derived class, decrements the value of the up-down control.
    /// </summary>
    public abstract void DownButton();

    /// <summary>
    /// Selects a range of text in the up-down control specifying the starting position and number of
    /// characters to select.
    /// </summary>
    /// <param name="start">The position of the first character to select.</param>
    /// <param name="length">The number of characters to select.</param>
    public virtual void Select(int start, int length)
    {
        if (GtkControl is Gtk.Entry entry)
        {
            entry.SelectRegion(start, start + length);
        }
    }
}
