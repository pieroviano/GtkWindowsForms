using XmlDocSync.Services;

namespace Prepare.Tests;

public class TypeSplitServiceTests
{
    [Fact]
    public void ProcessFilesInDirectory_SkipsBaseAndChildFiles()
    {
        // Arrange: create temp directory
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Create files
            var files = new[]
            {
                "ImageList.cs",
                "ImageList.Original.cs",
                "ImageList.OriginalOptions.cs",
                "Other.cs"
            };

            foreach (var file in files)
            {
                File.WriteAllText(Path.Combine(tempDir, file), "public class " + Path.GetFileNameWithoutExtension(file) + " {}");
            }

            // Group files as IGrouping<string?, string>
            var csFiles = Directory.GetFiles(tempDir, "*.cs", SearchOption.TopDirectoryOnly).ToList();
            var filesByDir = csFiles.GroupBy(Path.GetDirectoryName);

            var processedFiles = new List<string>();

            // Act: call ProcessFilesInDirectory with a custom processFiles action that records processed files
            TypeSplitService.ProcessFilesInDirectory(filesByDir, filesToProcess =>
            {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                processedFiles.AddRange(filesToProcess.Select(Path.GetFileName).Where(i => i != null));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            });

            // Assert: only "Other.cs" should be processed
            Assert.Single(processedFiles);
            Assert.Contains("Other.cs", processedFiles);

            // None of the ImageList files should be processed
            Assert.DoesNotContain("ImageList.cs", processedFiles);
            Assert.DoesNotContain("ImageList.Original.cs", processedFiles);
            Assert.DoesNotContain("ImageList.OriginalOptions.cs", processedFiles);
        }
        finally
        {
            // Cleanup temp directory
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}