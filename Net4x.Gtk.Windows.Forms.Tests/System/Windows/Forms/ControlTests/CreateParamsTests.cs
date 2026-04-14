namespace System.Windows.Forms.ControlTests;

[Collection("NonParallelCollection")]
public class CreateParamsTests
{
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var cp = new CreateParams
        {
            Caption = "Test Caption",
            ClassName = "MyClass",
            ClassStyle = 1,
            ExStyle = 2,
            Height = 100,
            Width = 200,
            X = 10,
            Y = 20,
            Param = new object(),
            Parent = new IntPtr(1234),
            Style = 3
        };

        Assert.Equal("Test Caption", cp.Caption);
        Assert.Equal("MyClass", cp.ClassName);
        Assert.Equal(1, cp.ClassStyle);
        Assert.Equal(2, cp.ExStyle);
        Assert.Equal(100, cp.Height);
        Assert.Equal(200, cp.Width);
        Assert.Equal(10, cp.X);
        Assert.Equal(20, cp.Y);
        Assert.NotNull(cp.Param);
        Assert.Equal(new IntPtr(1234), cp.Parent);
        Assert.Equal(3, cp.Style);
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var cp = new CreateParams();

        Assert.Null(cp.Caption);
        Assert.Null(cp.ClassName);
        Assert.Equal(0, cp.ClassStyle);
        Assert.Equal(0, cp.ExStyle);
        Assert.Equal(0, cp.Height);
        Assert.Equal(0, cp.Width);
        Assert.Equal(0, cp.X);
        Assert.Equal(0, cp.Y);
        Assert.Null(cp.Param);
        Assert.Equal(IntPtr.Zero, cp.Parent);
        Assert.Equal(0, cp.Style);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var cp = new CreateParams
        {
            Caption = "Hello",
            ClassName = "EDIT",
            Style = 0x10,
            ExStyle = 0x20,
            X = 1,
            Y = 2,
            Width = 3,
            Height = 4
        };

        var result = cp.ToString();

        Assert.Equal(
            "CreateParams {'EDIT', 'Hello', 0x10, 0x20, {1, 2, 3, 4}}",
            result);
    }

    [Fact]
    public void ToString_HandlesNullValues()
    {
        var cp = new CreateParams();

        var result = cp.ToString();

        Assert.Equal(
            "CreateParams {'', '', 0x0, 0x0, {0, 0, 0, 0}}",
            result);
    }

    [Fact]
    public void ToString_UsesLowercaseHex()
    {
        var cp = new CreateParams
        {
            Style = 0xABCD,
            ExStyle = 0xEF12
        };

        var result = cp.ToString();

        Assert.Contains("0xabcd", result);
        Assert.Contains("0xef12", result);
    }
}