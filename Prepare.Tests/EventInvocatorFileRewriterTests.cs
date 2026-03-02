using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class EventInvocatorFileRewriterTests : IDisposable
{
    private readonly string _tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.cs");

    // Create a unique temp file for each test run

    [Fact]
    public void RewriteEventInvocations_ValidOnMethod_RewritesBody()
    {
        // Arrange
        var originalCode = @"
namespace TestNamespace;
public class TestClass {
    public event EventHandler<MouseEventArgs> MouseClick;
    protected virtual void OnMouseClick(MouseEventArgs e)
    {
        // Old logic
    }
}";
        File.WriteAllText(_tempFilePath, originalCode);

        // Act
        EventInvocatorFileRewriter.RewriteEventInvocations(_tempFilePath);

        // Assert
        var updatedCode = File.ReadAllText(_tempFilePath);
        Assert.Contains("MouseClick?.Invoke(this, e);", updatedCode);
        Assert.Contains("// Old logic", updatedCode);
    }

    [Fact]
    public void RewriteEventInvocations_MethodDoesNotMatchPattern_DoesNotChangeFile()
    {
        // Arrange: Method starts with 'On' but parameter doesn't end in 'EventArgs'
        var originalCode = @"
public class TestClass {
    protected void OnStatusUpdate(string message) { }
}";
        File.WriteAllText(_tempFilePath, originalCode);

        // Act
        EventInvocatorFileRewriter.RewriteEventInvocations(_tempFilePath);

        // Assert
        var updatedCode = File.ReadAllText(_tempFilePath);
        Assert.Equal(originalCode, updatedCode);
    }

    [Fact]
    public void RewriteEventInvocations_NoFileExists_DoesNotThrow()
    {
        // Act & Assert
        var nonExistentPath = "non_existent_file.cs";
        var exception = Record.Exception(() => EventInvocatorFileRewriter.RewriteEventInvocations(nonExistentPath));
        Assert.Null(exception);
    }

    [Fact]
    public void RewriteEventInvocations_HandlesMultipleMembers()
    {
        // Arrange
        var originalCode = @"
public class MyControl {
    protected void OnClick(ClickEventArgs e) { }
    protected void OnHover(HoverEventArgs ev) { }
}";
        File.WriteAllText(_tempFilePath, originalCode);

        // Act
        EventInvocatorFileRewriter.RewriteEventInvocations(_tempFilePath);

        // Assert
        var updatedCode = File.ReadAllText(_tempFilePath);
        Assert.Contains("Click?.Invoke(this, e);", updatedCode);
        Assert.Contains("Hover?.Invoke(this, ev);", updatedCode);
    }

    public void Dispose()
    {
        // Cleanup the temp file after each test
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }
}
