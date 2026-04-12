extern alias gtk;

using FrameDimension = gtk::System.Drawing.Imaging.FrameDimension;

namespace System.Drawing.Imaging;

public class FrameDimensionTests
{
    [Fact]
    public void Ctor_SetsGuidCorrectly()
    {
        var guid = Guid.NewGuid();

        var fd = new FrameDimension(guid);

        Assert.Equal(guid, fd.Guid);
    }

    [Fact]
    public void StaticProperties_ReturnSameInstances()
    {
        Assert.Same(FrameDimension.Page, FrameDimension.Page);
        Assert.Same(FrameDimension.Resolution, FrameDimension.Resolution);
        Assert.Same(FrameDimension.Time, FrameDimension.Time);
    }

    [Fact]
    public void StaticProperties_HaveExpectedGuids()
    {
        Assert.Equal(
            new Guid("{7462dc86-6180-4c7e-8e3f-ee7333a7a483}"),
            FrameDimension.Page.Guid);

        Assert.Equal(
            new Guid("{84236f7b-3bd3-428f-8dab-4ea1439ca315}"),
            FrameDimension.Resolution.Guid);

        Assert.Equal(
            new Guid("{6aedbd6d-3fb5-418a-83a6-7f45229dc872}"),
            FrameDimension.Time.Guid);
    }

    [Fact]
    public void Equals_ReturnsTrue_ForSameGuid()
    {
        var guid = Guid.NewGuid();

        var fd1 = new FrameDimension(guid);
        var fd2 = new FrameDimension(guid);

        Assert.True(fd1.Equals(fd2));
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentGuid()
    {
        var fd1 = new FrameDimension(Guid.NewGuid());
        var fd2 = new FrameDimension(Guid.NewGuid());

        Assert.False(fd1.Equals(fd2));
    }

    [Fact]
    public void Equals_ReturnsFalse_ForNull()
    {
        var fd = new FrameDimension(Guid.NewGuid());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.False(fd.Equals(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentType()
    {
        var fd = new FrameDimension(Guid.NewGuid());

        Assert.False(fd.Equals("not a FrameDimension"));
    }

    [Fact]
    public void GetHashCode_MatchesGuidHashCode()
    {
        var guid = Guid.NewGuid();
        var fd = new FrameDimension(guid);

        Assert.Equal(guid.GetHashCode(), fd.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsTime_ForTimeInstance()
    {
        Assert.Equal("Time", FrameDimension.Time.ToString());
    }

    [Fact]
    public void ToString_ReturnsResolution_ForResolutionInstance()
    {
        Assert.Equal("Resolution", FrameDimension.Resolution.ToString());
    }

    [Fact]
    public void ToString_ReturnsPage_ForPageInstance()
    {
        Assert.Equal("Page", FrameDimension.Page.ToString());
    }

    [Fact]
    public void ToString_ReturnsFormattedString_ForCustomInstance()
    {
        var guid = Guid.NewGuid();
        var fd = new FrameDimension(guid);

        var result = fd.ToString();

        Assert.Contains(guid.ToString(), result);
        Assert.StartsWith("[FrameDimension:", result);
    }
}