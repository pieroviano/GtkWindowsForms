using System.Text;
using System.Xml.Linq;
using ResxProcessor.Tests.Internals;

namespace ResxProcessor.Tests;

/// <summary>
/// Behavioural tests for the resx rewriting this tool performs during every pipeline run.
/// <para>
/// The tool stamps each <c>.resx</c> with a <c>this_FileName</c> entry that the runtime
/// <c>ResourceManager</c> later reads to locate the file a resource came from. It rewrites files in
/// place, across the whole tree, unattended - so the cases worth pinning down are the ones where a wrong
/// answer corrupts a resx rather than failing: idempotency, nested paths, files it must leave alone, and
/// content it must not disturb.
/// </para>
/// </summary>
public class ProcessResxBehaviourTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "resxproc-" + Guid.NewGuid().ToString("N"));

    public ProcessResxBehaviourTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, true);
        }
    }

    private string WriteResx(string relativePath, params XElement[] content)
    {
        var full = Path.Combine(_root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        new XDocument(new XElement("root", content)).Save(full);

        return full;
    }

    private static string? StampOf(string resxPath) =>
        (string?)XDocument.Load(resxPath).Root!
            .Elements("data")
            .FirstOrDefault(e => (string?)e.Attribute("name") == "this_FileName")
            ?.Element("value");

    private static int StampCount(string resxPath) =>
        XDocument.Load(resxPath).Root!
            .Elements("data")
            .Count(e => (string?)e.Attribute("name") == "this_FileName");

    [Fact]
    public void ProcessResx_RunTwice_DoesNotStampTwice()
    {
        // The pipeline runs the tool on every build, so a second pass over an already-stamped tree is the
        // normal case, not the exception. Duplicate <data name="this_FileName"> entries are a resx error.
        var resx = WriteResx("sample.resx");

        Program.ProcessResx(_root, resx);
        Program.ProcessResx(_root, resx);

        Assert.Equal(1, StampCount(resx));
    }

    [Fact]
    public void ProcessResx_NestedFile_RecordsThePathRelativeToTheRoot()
    {
        var resx = WriteResx(Path.Combine("Forms", "Dialogs", "Nested.resx"));

        Program.ProcessResx(_root, resx);

        Assert.Equal(Path.Combine("Forms", "Dialogs", "Nested.resx"), StampOf(resx));
    }

    [Fact]
    public void ProcessResx_PreservesExistingEntries()
    {
        var resx = WriteResx("keep.resx",
            new XElement("data",
                new XAttribute("name", "Greeting"),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", "Hello")),
            new XElement("data",
                new XAttribute("name", "Farewell"),
                new XElement("value", "Bye")));

        Program.ProcessResx(_root, resx);

        var root = XDocument.Load(resx).Root!;
        Assert.Equal("Hello", (string?)root.Elements("data")
            .First(e => (string?)e.Attribute("name") == "Greeting").Element("value"));
        Assert.Equal("Bye", (string?)root.Elements("data")
            .First(e => (string?)e.Attribute("name") == "Farewell").Element("value"));
        Assert.Equal(3, root.Elements("data").Count());
    }

    [Fact]
    public void ProcessResx_PreservesResHeaders()
    {
        // Losing the resheaders would make the file unreadable to the resx tooling entirely.
        var resx = WriteResx("headers.resx",
            new XElement("resheader",
                new XAttribute("name", "resmimetype"),
                new XElement("value", "text/microsoft-resx")),
            new XElement("resheader",
                new XAttribute("name", "version"),
                new XElement("value", "2.0")));

        Program.ProcessResx(_root, resx);

        var root = XDocument.Load(resx).Root!;
        Assert.Equal(2, root.Elements("resheader").Count());
        Assert.Equal("text/microsoft-resx", (string?)root.Elements("resheader")
            .First(e => (string?)e.Attribute("name") == "resmimetype").Element("value"));
    }

    [Fact]
    public void ProcessResx_DoesNotOverwriteAnExistingStamp()
    {
        // A hand-maintained or previously stamped value wins; silently rewriting it would change which
        // file the runtime looks for.
        var resx = WriteResx("stamped.resx",
            new XElement("data",
                new XAttribute("name", "this_FileName"),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", "custom/path.resx")));

        Program.ProcessResx(_root, resx);

        Assert.Equal("custom/path.resx", StampOf(resx));
        Assert.Equal(1, StampCount(resx));
    }

    [Fact]
    public void ProcessResx_NonResxXml_IsLeftUntouched()
    {
        var notResx = Path.Combine(_root, "notaresx.resx");
        new XDocument(new XElement("configuration", new XElement("appSettings"))).Save(notResx);
        var before = File.ReadAllText(notResx);

        Program.ProcessResx(_root, notResx);

        Assert.Equal(before, File.ReadAllText(notResx));
    }

    [Fact]
    public void ProcessResx_StampIsMarkedSpacePreserve()
    {
        // Without xml:space="preserve" a path with leading or trailing space would come back trimmed.
        var resx = WriteResx("space.resx");

        Program.ProcessResx(_root, resx);

        var stamp = XDocument.Load(resx).Root!
            .Elements("data").First(e => (string?)e.Attribute("name") == "this_FileName");
        Assert.Equal("preserve", (string?)stamp.Attribute(XNamespace.Xml + "space"));
    }

    [Fact]
    public void DoWork_StampsEveryResxInTheTree()
    {
        var top = WriteResx("Top.resx");
        var nested = WriteResx(Path.Combine("Sub", "Deep", "Nested.resx"));

        var exit = Program.DoWork(new[] { _root }, new TestProcesses(0));

        Assert.Equal(0, exit);
        Assert.Equal("Top.resx", StampOf(top));
        Assert.Equal(Path.Combine("Sub", "Deep", "Nested.resx"), StampOf(nested));
    }

    [Fact]
    public void DoWork_IgnoresFilesThatAreNotResx()
    {
        var resx = WriteResx("Real.resx");
        var other = Path.Combine(_root, "Notes.txt");
        File.WriteAllText(other, "untouched", Encoding.UTF8);

        Program.DoWork(new[] { _root }, new TestProcesses(0));

        Assert.NotNull(StampOf(resx));
        Assert.Equal("untouched", File.ReadAllText(other));
    }

    [Fact]
    public void DoWork_AcceptsAQuotedFolderArgument()
    {
        // The pipeline passes the folder quoted, which is why DoWork trims quotes at all.
        WriteResx("Quoted.resx");

        var exit = Program.DoWork(new[] { "\"" + _root + "\"" }, new TestProcesses(0));

        Assert.Equal(0, exit);
        Assert.NotNull(StampOf(Path.Combine(_root, "Quoted.resx")));
    }

    [Fact]
    public void DoWork_EmptyTree_Succeeds()
    {
        Assert.Equal(0, Program.DoWork(new[] { _root }, new TestProcesses(0)));
    }
}
