using ResxProcessor;
using Xunit;

namespace ResxProcessor.Tests;

/// <summary>
/// Tests the consumer-project conversion: stripping the Windows platform moniker from the target
/// framework and removing the real Windows Desktop WinForms opt-in. The transform is textual and must
/// be minimal (leave everything else untouched) and idempotent.
/// </summary>
public class CsprojConverterTests
{
    private const string WindowsProject = """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
""";

    [Fact]
    public void Transform_StripsWindowsMoniker_AndRemovesUseWindowsForms()
    {
        var result = CsprojConverter.Transform(WindowsProject, out var changed);

        Assert.True(changed);
        Assert.Contains("<TargetFramework>net10.0</TargetFramework>", result);
        Assert.DoesNotContain("-windows", result);
        Assert.DoesNotContain("UseWindowsForms", result);
        // Untouched content survives.
        Assert.Contains("<OutputType>WinExe</OutputType>", result);
        Assert.Contains("<Nullable>enable</Nullable>", result);
    }

    [Fact]
    public void Transform_StripsVersionedWindowsMoniker()
    {
        var result = CsprojConverter.Transform(
            "<Project><PropertyGroup><TargetFramework>net10.0-windows10.0.19041.0</TargetFramework></PropertyGroup></Project>",
            out var changed);

        Assert.True(changed);
        Assert.Contains("<TargetFramework>net10.0</TargetFramework>", result);
    }

    [Fact]
    public void Transform_StripsEachMoniker_InTargetFrameworks()
    {
        var result = CsprojConverter.Transform(
            "<TargetFrameworks>net8.0-windows;net10.0-windows</TargetFrameworks>",
            out var changed);

        Assert.True(changed);
        Assert.Equal("<TargetFrameworks>net8.0;net10.0</TargetFrameworks>", result);
    }

    [Fact]
    public void Transform_IsIdempotent()
    {
        var once = CsprojConverter.Transform(WindowsProject, out _);
        var twice = CsprojConverter.Transform(once, out var changedSecond);

        Assert.False(changedSecond);
        Assert.Equal(once, twice);
    }

    [Fact]
    public void Transform_LeavesCrossPlatformProjectUnchanged()
    {
        const string plain = """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <UseWindowsForms>false</UseWindowsForms>
  </PropertyGroup>
</Project>
""";

        var result = CsprojConverter.Transform(plain, out var changed);

        Assert.False(changed);
        Assert.Equal(plain, result);
    }

    [Fact]
    public void Transform_PreservesOtherElementsAndIndentation()
    {
        var result = CsprojConverter.Transform(WindowsProject, out _);

        // The UseWindowsForms line is removed whole, leaving no blank gap before Nullable.
        Assert.Contains("    <TargetFramework>net10.0</TargetFramework>\n    <Nullable>enable</Nullable>",
            result.Replace("\r\n", "\n"));
    }
}
