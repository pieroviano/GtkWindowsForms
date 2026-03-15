using XmlDocSync.Services;

namespace Prepare.Tests
{
    public class XmlDocumentationServiceTests
    {
        private const string RoslynFolder = @"..\..\..\Roslyn";
        private const string XmlFile = "Net4x.Gtk.Windows.Forms.xml";

        [Theory]
        [InlineData("TableLayoutPanel.cs", "The self")]
        [InlineData("ColumnWidthChangingEventHandler.cs", "Delegate ColumnWidthChangingEventHandler")]
        [InlineData("Bitmap.cs", "System.Drawing.Bitmap\" /> class.")]
        [InlineData("ToolboxItem.cs", "with the data needed to serialize the target object.")]
        [InlineData("GraphicsPath.cs", "The size points")]
        [InlineData("RichTextBoxBase.cs", "Initializes a new instance of the")]
        public async Task MergeXmlDocumentationAsync_ShouldProcessSingleFile(
            string fileName,
            string expectedText)
        {
            var guidFolder = Path.Combine(AppContext.BaseDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(guidFolder);

            try
            {
                var sourceFolder = Path.GetFullPath(RoslynFolder);

                var xmlSource = Path.Combine(sourceFolder, XmlFile);
                var xmlDest = Path.Combine(guidFolder, XmlFile);
                File.Copy(xmlSource, xmlDest);

                var sourceFile = Directory
                    .GetFiles(sourceFolder, fileName, SearchOption.AllDirectories)
                    .First();

                var destFile = Path.Combine(guidFolder, fileName);
                File.Copy(sourceFile, destFile);

                var result = await XmlDocumentationService.MergeXmlDocumentationAsync(
                    xmlDest,
                    guidFolder,
                    [],
                    false,
                    true);

                Assert.Equal(0, result.Failure);

                var content = result.Contents.First(i => i.Path.EndsWith(fileName)).Content;

                Assert.Contains(expectedText, content);
            }
            finally
            {
                if (Directory.Exists(guidFolder))
                    Directory.Delete(guidFolder, true);
            }
        }
    }
}
