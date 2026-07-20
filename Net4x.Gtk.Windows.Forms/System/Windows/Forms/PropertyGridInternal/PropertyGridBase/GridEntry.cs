// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal;

public partial class GridEntry : GridItem, ITypeDescriptorContext
{

    public GridEntry? _parent;

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

    public IContainer Container => throw new NotImplementedException();

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
                        items.Add(new GridEntry(this, GridItemType.Property, 2, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property });
                    }
                }
            }

            return new GridItemCollection(items.ToArray());
        }
    }

    public override GridItemType GridItemType { get; }

    public object Instance => throw new NotImplementedException();

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

    public override PropertyDescriptor? PropertyDescriptor { get; }

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

    public object GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public void OnComponentChanged()
    {
        throw new NotImplementedException();
    }

    public bool OnComponentChanging()
    {
        throw new NotImplementedException();
    }

    public override bool Select()
    {
        return false;
    }

    public override void SetValue(object value)
    {
        PropertyInfo?.SetValue(OwnerObj, value);
    }
}