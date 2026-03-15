using System;
using System.IO;
using System.Linq;
using Xunit;
using XmlDocSync.Cleaning;

namespace XmlDocSync.Tests;

public class XmlDocDuplicateCleanerTests
{
    [Fact]
    public void RemoveRepeatedXmlDocBlocks_Should_Remove_Duplicates_From_All_Test_Files()
    {
        var folder = Path.Combine("..", "..", "..", "XmlDocDuplicateClean");

        var files = Directory.GetFiles(folder, "*.cs");

        Assert.NotEmpty(files);

        foreach (var file in files)
        {
            var original = File.ReadAllText(file);

            var cleaned = XmlDocDuplicateCleaner.RemoveRepeatedXmlDocBlocks(original);

            // Running cleaner twice should produce the same result
            var cleanedTwice = XmlDocDuplicateCleaner.RemoveRepeatedXmlDocBlocks(cleaned);

            Assert.Equal(cleaned, cleanedTwice);
        }
    }

    [Fact]
    public void RemoveRepeatedXmlDocBlocks_Should_Remove_Consecutive_Duplicate_Block()
    {
        var input = """
                    /// <summary>
                    /// Test
                    /// </summary>
                    /// <summary>
                    /// Test
                    /// </summary>
                    public class A {}
                    """;

        var result = XmlDocDuplicateCleaner.RemoveRepeatedXmlDocBlocks(input);

        var expected = """
                       /// <summary>
                       /// Test
                       /// </summary>
                       public class A {}
                       """.Replace("\r\n", "\n");

        Assert.Equal(expected, result.Replace("\r\n", "\n"));
    }
}