// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal;

public partial class GridEntry : GridItem, ITypeDescriptorContext
{

    public GridEntry? _parent;

    private PropertyDescriptor? _descriptor;

    private bool _descriptorResolved;

    public GridEntry(GridEntry? parent, GridItemType itemtype, int level, string? label, object? entryobj, string? description)
    {
        _parent = parent;
        this.Parent = parent;
        this.GridItemType = itemtype;
        this.Level = level;
        this.Label = label;
        this.OwnerObj = entryobj;
        this.Description = description;
    }

    protected GridEntry(PropertyGrid ownerGrid, GridEntry parent)
    {
        _parent = parent;
        OwnerGrid = ownerGrid;
    }

    /// <summary>
    /// Gets the container the object being edited belongs to, or <see langword="null" /> when it belongs
    /// to none. This is the <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> contract: the
    /// container is taken from the edited object's site, falling back to the owning
    /// <see cref="T:System.Windows.Forms.PropertyGrid" />'s site.
    /// </summary>
    public IContainer? Container
    {
        get
        {
            for (var entry = this; entry != null; entry = entry._parent)
            {
                if ((entry.OwnerObj as IComponent)?.Site is { } site)
                {
                    return site.Container;
                }
            }

            return OwnerGrid?.Site?.Container;
        }
    }

    public string? Description
    {
        get;
        set;
    }

    public bool Editable
    {
        get;
        set;
    }

    public override GridItemCollection? GridItems
    {
        get
        {
            var items = new List<GridEntry>();
            var obj = Value;
            var properties = obj?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            if (properties != null)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.CanWrite && property.CanRead)
                    {
                        string? description = "";
                        var attri3 = property.GetCustomAttributes(typeof(DescriptionAttribute));
                        if (attri3 != null && attri3.Count() > 0)
                        {
                            var attribute = (DescriptionAttribute?)attri3.FirstOrDefault();
                            description = attribute?.Description;
                        }
                        items.Add(new GridEntry(this, GridItemType.Property, 2, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property, OwnerGrid = OwnerGrid });
                    }
                }
            }

            return new GridItemCollection(items.ToArray());
        }
    }

    public override GridItemType GridItemType { get; }

    /// <summary>
    /// Gets the object whose property this entry describes - the value owner, which is what
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> consumers (type converters and UI
    /// type editors) expect to receive.
    /// </summary>
    public object? Instance => OwnerObj;

    public override string? Label { get; }

    public int Level
    {
        get;
        set;
    }

    public PropertyGrid? OwnerGrid
    {
        get;
        set;
    }

    public override GridItem? Parent { get; }

    /// <summary>
    /// Gets the descriptor of the property this entry edits, resolved from the owning object through
    /// <see cref="T:System.ComponentModel.TypeDescriptor" />. Category rows - which describe no property -
    /// return <see langword="null" />.
    /// </summary>
    /// <remarks>
    /// This is the descriptor <see cref="M:System.Windows.Forms.PropertyGridInternal.GridEntry.OnComponentChanged" />
    /// and <see cref="M:System.Windows.Forms.PropertyGridInternal.GridEntry.OnComponentChanging" /> report to
    /// an <see cref="T:System.ComponentModel.Design.IComponentChangeService" />, so it has to describe the
    /// same property the entry writes through <see cref="P:System.Windows.Forms.PropertyGridInternal.GridEntry.PropertyInfo" />.
    /// </remarks>
    public override PropertyDescriptor? PropertyDescriptor
    {
        get
        {
            if (_descriptorResolved)
            {
                return _descriptor;
            }

            _descriptorResolved = true;
            var name = PropertyInfo?.Name ?? Label;
            if (OwnerObj != null && !string.IsNullOrEmpty(name) && GridItemType != GridItemType.Category)
            {
                _descriptor = TypeDescriptor.GetProperties(OwnerObj)[name];
            }

            return _descriptor;
        }
    }

    public object? OwnerObj
    {
        get;
        set;
    }

    public override object? Value
    {
        get
        {
            if (PropertyInfo == null)
            {
                return OwnerObj?.GetType().GetProperty(this.Label ?? string.Empty)?.GetValue(OwnerObj);
            }
            else
            {
                return PropertyInfo.GetValue(OwnerObj);
            }
        }
    }

    public Type? ValueType
    {
        get;
        set;
    }

    internal PropertyInfo? PropertyInfo
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the service object of the specified type, as
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> requires. The entry answers for itself,
    /// then defers to the owning <see cref="T:System.Windows.Forms.PropertyGrid" />'s site and finally to the
    /// site of the object being edited.
    /// </summary>
    /// <param name="serviceType">The type of service to retrieve.</param>
    /// <returns>The service, or <see langword="null" /> when no provider supplies it.</returns>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="serviceType" /> is <see langword="null" />.</exception>
    public object? GetService(Type serviceType)
    {
        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(GridEntry))
        {
            return this;
        }

        if (serviceType == typeof(PropertyGrid) && OwnerGrid != null)
        {
            return OwnerGrid;
        }

        return OwnerGrid?.Site?.GetService(serviceType)
            ?? (OwnerObj as IComponent)?.Site?.GetService(serviceType);
    }

    /// <summary>
    /// Notifies the design surface that the property this entry edits has changed, by raising
    /// <see cref="M:System.ComponentModel.Design.IComponentChangeService.OnComponentChanged(System.Object,System.ComponentModel.MemberDescriptor,System.Object,System.Object)" />
    /// when such a service is available. With no design surface attached this is a no-op, which is what
    /// WinForms does at run time.
    /// </summary>
    public void OnComponentChanged()
    {
        if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
        {
            changeService.OnComponentChanged(Instance!, PropertyDescriptor, null, null);
        }
    }

    /// <summary>
    /// Asks the design surface for permission to change the property this entry edits, by raising
    /// <see cref="M:System.ComponentModel.Design.IComponentChangeService.OnComponentChanging(System.Object,System.ComponentModel.MemberDescriptor)" />
    /// when such a service is available.
    /// </summary>
    /// <returns>
    /// <see langword="false" /> when the change was refused - the designer cancelled the checkout -
    /// otherwise <see langword="true" />, including when no design surface is attached.
    /// </returns>
    public bool OnComponentChanging()
    {
        if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
        {
            try
            {
                changeService.OnComponentChanging(Instance!, PropertyDescriptor);
            }
            catch (CheckoutException checkout) when (checkout == CheckoutException.Canceled)
            {
                return false;
            }
        }

        return true;
    }

    public override bool Select()
    {
        return false;
    }

    /// <summary>
    /// Writes <paramref name="value" /> to the property this entry describes, bracketed by the
    /// component-change notifications a design surface expects. A refused change (see
    /// <see cref="M:System.Windows.Forms.PropertyGridInternal.GridEntry.OnComponentChanging" />) leaves the
    /// property untouched.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public override void SetValue(object value)
    {
        if (PropertyInfo == null)
        {
            return;
        }

        if (!OnComponentChanging())
        {
            return;
        }

        PropertyInfo.SetValue(OwnerObj, value);
        OnComponentChanged();
    }
}