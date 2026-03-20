// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

internal abstract class ArrangedElement : Component, IArrangedElement
{

    private Rectangle _bounds = Rectangle.Empty;

    private IArrangedElement? _parent;

    private readonly PropertyStore _propertyStore = new();  // Contains all properties that are not always set.

    private BitVector32 _state;

    private static readonly int s_stateVisible = BitVector32.CreateMask();

    public virtual event LayoutEventHandler? Layout;

    internal ArrangedElement()
    {
        Padding = DefaultPadding;
        Margin = DefaultMargin;
        _state[s_stateVisible] = true;
    }

    public Rectangle Bounds
    {
        get
        {
            return _bounds;
        }
    }

    public virtual Rectangle DisplayRectangle
    {
        get
        {
            var displayRectangle = Bounds;

            return displayRectangle;
        }
    }

    public IntPtr Handle => throw new NotImplementedException();

    public abstract LayoutEngine LayoutEngine
    {
        get;
    }

    public Padding Margin
    {
        get;

        set;
    }

    public virtual Padding Padding
    {
        get;

        set;
    }

    public virtual IArrangedElement? Parent
    {
        get
        {
            return _parent;
        }

        set
        {
            _parent = value;
        }
    }

    public virtual bool ParticipatesInLayout
    {
        get
        {
            return Visible;
        }
    }

    public virtual bool Visible
    {
        get;

        set;
    }

    ArrangedElementCollection IArrangedElement.Children
    {
        get { return GetChildren(); }
    }

    IArrangedElement? IArrangedElement.Container
    {
        get { return GetContainer(); }
    }

    PropertyStore IArrangedElement.Properties
    {
        get
        {
            return Properties;
        }
    }

    protected virtual Padding DefaultMargin
    {
        get { return Padding.Empty; }
    }

    protected virtual Padding DefaultPadding
    {
        get { return Padding.Empty; }
    }

    private PropertyStore Properties
    {
        get
        {
            return _propertyStore;
        }
    }

    public virtual Size GetPreferredSize(Size constrainingSize)
    {
        return constrainingSize;
    }

    public virtual void PerformLayout(IArrangedElement container, string? propertyName)

    {
        OnLayout(new LayoutEventArgs(container, propertyName));
    }

    public void SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        // in this case the parent is telling us to refresh our bounds - don't
        // call PerformLayout
        SetBoundsCore(bounds, specified);
    }

    protected abstract ArrangedElementCollection GetChildren();

    protected abstract IArrangedElement? GetContainer();

    protected virtual void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {

    }

    protected virtual void OnLayout(LayoutEventArgs e)
    {
        Layout?.Invoke(this, e);
    }

    protected virtual void SetBoundsCore(Rectangle bounds, BoundsSpecified specified)
    {
        if (bounds != _bounds)
        {
            var oldBounds = _bounds;

            _bounds = bounds;
            OnBoundsChanged(oldBounds, bounds);
        }
    }

}