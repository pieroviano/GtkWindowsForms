using Prepare.Common.IO;

namespace PrepareProjects.Preparation.Directories;

public static class DrawingService
{
    public static void SeparateDrawing(string gtkDrawing)
    {
        var resourcesNet4xGtkDrawingsCsproj = @$"..\..\..\..\Resources\{gtkDrawing}.csproj";
        var net4xGtkDrawings = @$"..\..\..\..\{gtkDrawing}";
        var net4xGtkDrawingsSystem = @$"..\..\..\..\{gtkDrawing}\System";
        var net4xGtkDrawingsSystemDrawing = @$"{net4xGtkDrawingsSystem}\Drawing";
        const string net4xGtkWindowsFormsSystemDrawing = @"..\..\..\..\Net4x.Gtk.Windows.Forms\System\Drawing";

        if (Directory.Exists(net4xGtkDrawings))
        {
            Directory.Delete(net4xGtkDrawings, true);
        }

        Directory.CreateDirectory(net4xGtkDrawings);
        Directory.CreateDirectory(net4xGtkDrawingsSystem);
        Directory.CreateDirectory(net4xGtkDrawingsSystemDrawing);
        DirectoryMover.MoveAllExcept(
            sourceRoot: net4xGtkWindowsFormsSystemDrawing,
            destinationRoot: net4xGtkDrawingsSystemDrawing,
            excludedFolderFullPath: @$"{net4xGtkWindowsFormsSystemDrawing}\Printing"
        );

        LogUtility.FileCopyAndLog(resourcesNet4xGtkDrawingsCsproj, @$"{net4xGtkDrawings}\{gtkDrawing}.csproj");
    }
}
