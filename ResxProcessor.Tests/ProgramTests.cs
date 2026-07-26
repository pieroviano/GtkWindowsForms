using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ResxProcessor.Tests.Internals;
using Xunit;

namespace ResxProcessor.Tests;

public class ProgramTests
{
    [Fact]
    public void DoWork_Returns1_WhenFolderDoesNotExist()
    {
        var nonExisting = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var result = Program.DoWork(new[] { nonExisting }, new TestProcesses(0));

        Assert.Equal(1, result);
    }

    [Fact]
    public void ProcessResx_AddsThisFileName_WhenMissing()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(root);

        try
        {
            var resxPath = Path.Combine(root, "sample.resx");
            // minimal valid resx
            var doc = new XDocument(
                new XElement("root",
                    new XElement("data",
                        new XAttribute("name", "Existing"),
                        new XAttribute(XNamespace.Xml + "space", "preserve"),
                        new XElement("value", "v"))));
            doc.Save(resxPath);

            Program.ProcessResx(root, resxPath);

            var loaded = XDocument.Load(resxPath);
            var added = loaded.Root!
                .Elements("data")
                .FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), "this_FileName", StringComparison.Ordinal));

            Assert.NotNull(added);

            var expectedRelative = Path.GetRelativePath(root, resxPath);
            Assert.Equal(expectedRelative, (string?)added.Element("value"));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, true);
        }
    }

    [Fact]
    public void DoWork_ProcessesResx_Using_TestProcesses()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(root);

        try
        {
            var resxPath = Path.Combine(root, "dir.resx");
            var doc = new XDocument(new XElement("root"));
            doc.Save(resxPath);

            // TestProcesses will return a running process for the first call(s) and then null,
            // allowing Program.DoWork to exercise the wait logic without blocking.
            var processes = new TestProcesses(timesToReturnProcess: 1);

            var result = Program.DoWork(new[] { root }, processes);

            Assert.Equal(0, result);

            // Verify resx updated
            var loaded = XDocument.Load(resxPath);
            var added = loaded.Root!
                .Elements("data")
                .FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), "this_FileName", StringComparison.Ordinal));

            Assert.NotNull(added);
            var expectedRelative = Path.GetRelativePath(root, resxPath);
            Assert.Equal(expectedRelative, (string?)added.Element("value"));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, true);
        }
    }
}
