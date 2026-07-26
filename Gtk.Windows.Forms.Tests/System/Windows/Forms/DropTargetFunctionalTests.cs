using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the public <see cref="IDropTarget"/> interface. It existed only as an empty internal marker,
/// so <c>class MyTarget : IDropTarget</c> (the documented way to write a custom drop target) did not
/// compile. It is now public with the four WinForms notification methods; this verifies a type can
/// implement it and be driven polymorphically, and that <see cref="ToolStripItem"/> (which declares it)
/// routes the interface calls to its overridable On* methods and Drag* events.
/// </summary>
[Collection("NonParallelCollection")]
public class DropTargetFunctionalTests
{
    static DropTargetFunctionalTests()
    {
        using var _ = new Form();
    }

    private sealed class RecordingTarget : IDropTarget
    {
        public string Log { get; private set; } = "";

        public void OnDragEnter(DragEventArgs e) => Log += "enter;";
        public void OnDragOver(DragEventArgs e) => Log += "over;";
        public void OnDragDrop(DragEventArgs e) => Log += "drop;";
        public void OnDragLeave(EventArgs e) => Log += "leave;";
    }

    private static DragEventArgs Drag() =>
        new DragEventArgs(null, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

    [Fact]
    public void ACustomTypeCanImplementAndBeDrivenThroughTheInterface()
    {
        IDropTarget target = new RecordingTarget();

        target.OnDragEnter(Drag());
        target.OnDragOver(Drag());
        target.OnDragDrop(Drag());
        target.OnDragLeave(EventArgs.Empty);

        Assert.Equal("enter;over;drop;leave;", ((RecordingTarget)target).Log);
    }

    [Fact]
    public void ToolStripItem_RoutesInterfaceCallsToItsDragEvents()
    {
        using var button = new ToolStripButton("x");
        var log = "";
        button.DragEnter += (_, _) => log += "enter;";
        button.DragOver += (_, _) => log += "over;";
        button.DragDrop += (_, _) => log += "drop;";
        button.DragLeave += (_, _) => log += "leave;";

        var target = (IDropTarget)button;
        target.OnDragEnter(Drag());
        target.OnDragOver(Drag());
        target.OnDragDrop(Drag());
        target.OnDragLeave(EventArgs.Empty);

        Assert.Equal("enter;over;drop;leave;", log);
    }
}
