// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class GridEntry : GridItem, ITypeDescriptorContext
{

    public GridEntry _parent;

    internal object value;

    public GridEntry(GridEntry parent, GridItemType itemtype, int level, string label, object value, string description)
    {
        _parent = parent;
        this.Parent = parent;
        this.GridItemType = itemtype;
        this.Level = level;
        this.Label = label;
        this.value = value;
        this.Description = description;
    }

    protected GridEntry(PropertyGrid ownerGrid, GridEntry parent)
    {
        _parent = parent;
        OwnerGrid = ownerGrid;
    }

    public IContainer Container => throw new NotImplementedException();

    public string Description
    {
        get;
        set;
    }

    public bool Editable
    {
        get;
        set;
    }

    public override GridItemCollection GridItems { get; }

    public override GridItemType GridItemType { get; }

    public object Instance => throw new NotImplementedException();

    public override string Label { get; }

    public int Level
    {
        get;
        set;
    }

    public PropertyGrid OwnerGrid
    {
        get;
        set;
    }

    public override GridItem Parent { get; }

    public override PropertyDescriptor PropertyDescriptor { get; }

    public override object Value { get => value; }

    public Type ValueType
    {
        get;
        set;
    }

    internal System.Reflection.PropertyInfo PropertyInfo
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
}