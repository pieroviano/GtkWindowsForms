using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the three remaining minor helpers that were the last non-non-goal gaps:
/// <see cref="DataGridViewColumnDesignTimeVisibleAttribute"/>, <see cref="DataGridViewCellStyleConverter"/>
/// and <see cref="ToolStripDropDownItemAccessibleObject"/>. They do not break ordinary application code but
/// close the type surface a designer or custom column/converter reference needs.
/// </summary>
[Collection("NonParallelCollection")]
public class DesignTimeHelpersFunctionalTests
{
    static DesignTimeHelpersFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void ColumnDesignTimeVisibleAttribute_HasTheStandardInstances()
    {
        Assert.True(DataGridViewColumnDesignTimeVisibleAttribute.Yes.Visible);
        Assert.False(DataGridViewColumnDesignTimeVisibleAttribute.No.Visible);
        Assert.Same(DataGridViewColumnDesignTimeVisibleAttribute.Yes, DataGridViewColumnDesignTimeVisibleAttribute.Default);

        Assert.False(new DataGridViewColumnDesignTimeVisibleAttribute(false).Visible);
        Assert.True(new DataGridViewColumnDesignTimeVisibleAttribute().Visible);
        Assert.Equal(DataGridViewColumnDesignTimeVisibleAttribute.No, new DataGridViewColumnDesignTimeVisibleAttribute(false));
        Assert.True(DataGridViewColumnDesignTimeVisibleAttribute.Yes.IsDefaultAttribute());
    }

    [Fact]
    public void CellStyleConverter_IsATypeConverter_AdvertisingInstanceDescriptor()
    {
        var converter = new DataGridViewCellStyleConverter();

        Assert.IsAssignableFrom<TypeConverter>(converter);
        Assert.True(converter.CanConvertTo(null, typeof(InstanceDescriptor)));
    }

    [Fact]
    public void DropDownItemAccessibleObject_ExposesTheOwnersRoleAndName()
    {
        using var owner = new ToolStripDropDownButton("Menu");
        var accessible = new ToolStripDropDownItemAccessibleObject(owner);

        Assert.IsAssignableFrom<AccessibleObject>(accessible);
        Assert.Equal(AccessibleRole.MenuItem, accessible.Role);
        Assert.Equal("Menu", accessible.Name);
    }

    [Fact]
    public void DropDownItemAccessibleObject_NullOwner_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripDropDownItemAccessibleObject(null!));
    }
}
