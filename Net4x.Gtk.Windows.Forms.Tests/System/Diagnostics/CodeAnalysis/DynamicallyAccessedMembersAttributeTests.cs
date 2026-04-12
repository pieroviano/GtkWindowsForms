extern alias gtk;

using System.Reflection;
#pragma warning disable CS8604 // Possible null reference argument.

namespace System.Diagnostics.CodeAnalysis;

public class DynamicallyAccessedMembersAttributeTests
{
    [Fact]
    public void Constructor_SetsMemberTypes()
    {
        var attr = new gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute(
            gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods);

        Assert.Equal(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, attr.MemberTypes);
    }

    [Fact]
    public void Enum_All_HasExpectedValue()
    {
        Assert.Equal(-1, (int)gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All);
    }

    [Fact]
    public void Enum_None_IsZero()
    {
        Assert.Equal(0, (int)gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.None);
    }

    [Fact]
    public void Enum_Flags_CanBeCombined()
    {
        var combined =
            gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods |
            gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields;

        Assert.True(combined.HasFlag(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods));
        Assert.True(combined.HasFlag(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields));
    }

    [Fact]
    public void Attribute_CanBeApplied_ToClass()
    {
        var type = typeof(TestClass);
        var attr = type.GetCustomAttribute<gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute>();

        Assert.NotNull(attr);
        Assert.Equal(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, attr.MemberTypes);
    }

    [Fact]
    public void Attribute_CanBeApplied_ToMethod()
    {
        var method = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        var attr = method.GetCustomAttribute<gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute>();

        Assert.NotNull(attr);
        Assert.Equal(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields, attr.MemberTypes);
    }

    [gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods)]
    private class TestClass
    {
        [gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute(gtk::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields)]
        public void TestMethod() { }
    }
}