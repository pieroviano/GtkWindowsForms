// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Colour-keyed window transparency - <c>Form.TransparencyKey</c> and <c>Form.AllowTransparency</c>.
/// Neither member existed, so the standard shaped-window idiom
/// (<c>BackColor = X; TransparencyKey = X;</c>) did not compile.
/// </summary>
public partial class Form
{
    private Color _transparencyKey = Color.Empty;

    private bool _allowTransparency;

    /// <summary>
    /// Gets or sets a value indicating whether the form can be made transparent - through
    /// <see cref="P:System.Windows.Forms.Form.Opacity" /> or
    /// <see cref="P:System.Windows.Forms.Form.TransparencyKey" />.
    /// </summary>
    /// <remarks>
    /// Assigning a non-empty <see cref="P:System.Windows.Forms.Form.TransparencyKey" /> turns this on, as
    /// it does in WinForms. Turning it off clears the key.
    /// </remarks>
    [DefaultValue(false)]
    public bool AllowTransparency
    {
        get => _allowTransparency;
        set
        {
            if (_allowTransparency == value)
            {
                return;
            }

            _allowTransparency = value;
            if (!value)
            {
                _transparencyKey = Color.Empty;
            }

            ApplyTransparency();
        }
    }

    /// <summary>
    /// Gets or sets the colour that will represent transparent areas of the form.
    /// </summary>
    /// <value>
    /// The colour to treat as transparent, or <see cref="F:System.Drawing.Color.Empty" /> - the default -
    /// for an opaque form.
    /// </value>
    /// <remarks>
    /// <para>
    /// What is emulated: the idiom this member exists for, a form whose background colour *is* the key, so
    /// the window's own background becomes invisible and only the controls on it are drawn. The window is
    /// given an RGBA visual and its background is painted with zero alpha.
    /// </para>
    /// <para>
    /// What is not: per-pixel colour keying of arbitrary content - a key that matches a colour inside a
    /// child control or a background image leaves those pixels opaque - and click-through, which in
    /// WinForms lets the mouse pass through keyed areas. GTK expresses both through an input shape that
    /// would have to be recomputed from the rendered pixels on every paint. Documented deviation.
    /// </para>
    /// <para>
    /// Transparency also needs a compositing window manager. Without one the key is remembered and
    /// reported, but the window stays opaque; <see cref="P:System.Windows.Forms.Form.IsTransparencySupported" />
    /// says which case you are in.
    /// </para>
    /// </remarks>
    public Color TransparencyKey
    {
        get => _transparencyKey;
        set
        {
            if (_transparencyKey == value)
            {
                return;
            }

            _transparencyKey = value;
            if (!value.IsEmpty)
            {
                _allowTransparency = true;
            }

            ApplyTransparency();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the screen this form is on can actually composite transparency.
    /// </summary>
    /// <remarks>
    /// There is no WinForms equivalent - on Windows the answer is always yes. It is exposed because on
    /// X11 without a compositor, and on some remote sessions, it is no, and a form that silently stays
    /// opaque is otherwise indistinguishable from a bug.
    /// </remarks>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsTransparencySupported
    {
        get
        {
            try
            {
                return self.Screen?.IsComposited == true && self.Screen.RgbaVisual != null;
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug("Unexpected exception querying compositing support", ex);

                return false;
            }
        }
    }

    /// <summary>
    /// Pushes the current transparency settings to the window: an RGBA visual and an alpha-zero background
    /// while a key matching the form's own background colour is set, the normal visual and background
    /// otherwise.
    /// </summary>
    private void ApplyTransparency()
    {
        try
        {
            var keyed = _allowTransparency && !_transparencyKey.IsEmpty;
            if (keyed && IsTransparencySupported)
            {
                self.Visual = self.Screen.RgbaVisual;
                self.AppPaintable = true;
            }
            else if (!keyed)
            {
                self.AppPaintable = false;
            }

            // The background is what the key can actually make transparent here: when it matches, the
            // override paints nothing rather than a colour.
            self.Override.TransparencyKey = keyed ? _transparencyKey : null;
            if (self.IsRealized)
            {
                self.QueueDraw();
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception applying the form transparency key", ex);
        }
    }
}
