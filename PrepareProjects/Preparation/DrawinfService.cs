using Prepare.Common.IO;

namespace PrepareProjects.Preparation;

public static class DrawinfService
{
    public static void SeparateDrawing()
    {
        const string resourcesNet4xGtkDrawingsCsproj = @"..\..\..\Resources\Net4x.Gtk.Drawings.csproj";
        const string net4xGtkDrawings = @"..\..\..\..\Net4x.Gtk.Drawings";
        const string net4xGtkDrawingsSystem = @"..\..\..\..\Net4x.Gtk.Drawings\System";
        const string net4xGtkDrawingsSystemDrawing = @$"{net4xGtkDrawingsSystem}\Drawing";
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

        File.Copy(resourcesNet4xGtkDrawingsCsproj, @$"{net4xGtkDrawings}\Net4x.Gtk.Drawings.csproj");
    }
}