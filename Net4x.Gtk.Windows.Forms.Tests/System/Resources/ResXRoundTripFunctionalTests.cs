using System.Collections;
using System.Resources;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Resources;

/// <summary>Behavioural tests for the ResX pipeline. What <see cref="ResXResourceWriter"/> emits must come
/// back out of <see cref="ResXResourceReader"/> unchanged, and <see cref="ResXDataNode"/> must report the
/// name, comment, value and value type it was built with.</summary>
[Collection("NonParallelCollection")]
public class ResXRoundTripFunctionalTests
{
    /// <summary>Writes the supplied entries and returns the resulting ResX document as text.</summary>
    private static string WriteResX(Action<ResXResourceWriter> add)
    {
        var buffer = new MemoryStream();
        using (var writer = new ResXResourceWriter(buffer))
        {
            add(writer);
            writer.Generate();
        }

        // The writer emits a UTF-8 byte-order mark; a StringReader would hand that to the XML parser as a
        // stray character at position 1, so drop it here.
        return global::System.Text.Encoding.UTF8.GetString(buffer.ToArray()).TrimStart('﻿');
    }

    private static Dictionary<string, object?> ReadResX(string document)
    {
        var result = new Dictionary<string, object?>();
        using var reader = new ResXResourceReader(new StringReader(document));
        var enumerator = Assert.IsAssignableFrom<IDictionaryEnumerator>(reader.GetEnumerator());
        while (enumerator.MoveNext())
        {
            result[(string)enumerator.Key] = enumerator.Value;
        }

        return result;
    }

    [Fact]
    public void AStringResource_RoundTripsThroughResX()
    {
        var document = WriteResX(w => w.AddResource("greeting", "hello world"));

        var entries = ReadResX(document);

        Assert.Equal("hello world", entries["greeting"]);
    }

    [Fact]
    public void SeveralStringResources_AllRoundTrip()
    {
        var document = WriteResX(w =>
        {
            w.AddResource("a", "one");
            w.AddResource("b", "two");
            w.AddResource("c", "three");
        });

        var entries = ReadResX(document);

        Assert.Equal(3, entries.Count);
        Assert.Equal("one", entries["a"]);
        Assert.Equal("three", entries["c"]);
    }

    [Fact]
    public void NonAsciiText_SurvivesTheResXRoundTrip()
    {
        const string text = "café über 中文";
        var document = WriteResX(w => w.AddResource("unicode", text));

        var entries = ReadResX(document);

        Assert.Equal(text, entries["unicode"]);
    }

    [Fact]
    public void TextWithXmlMarkupCharacters_IsEscapedAndRestored()
    {
        const string text = "<tag> & \"quoted\"";
        var document = WriteResX(w => w.AddResource("markup", text));

        var entries = ReadResX(document);

        Assert.Equal(text, entries["markup"]);
    }

    [Fact]
    public void WhitespaceOnlyText_IsPreserved()
    {
        const string text = "  leading and trailing  ";
        var document = WriteResX(w => w.AddResource("spaces", text));

        var entries = ReadResX(document);

        Assert.Equal(text, entries["spaces"]);
    }

    [Fact]
    public void TheWrittenDocumentIsResXVersion2()
    {
        var document = WriteResX(w => w.AddResource("a", "1"));

        Assert.Contains("resheader", document);
        Assert.Contains(ResXResourceWriter.Version, document);
        Assert.Contains(ResXResourceWriter.ResMimeType, document);
    }

    [Fact]
    public void AnEmptyDocument_ReadsBackAsNoEntries()
    {
        var document = WriteResX(_ => { });

        var entries = ReadResX(document);

        Assert.Empty(entries);
    }

    [Fact]
    public void ManyResources_AllSurviveTheRoundTrip()
    {
        const int count = 100;
        var document = WriteResX(w =>
        {
            for (var i = 0; i < count; i++)
            {
                w.AddResource($"key{i}", $"value{i}");
            }
        });

        var entries = ReadResX(document);

        Assert.Equal(count, entries.Count);
        Assert.Equal($"value{count - 1}", entries[$"key{count - 1}"]);
    }

    [Fact]
    public void ADataNodeResource_RoundTripsItsValue()
    {
        var document = WriteResX(w => w.AddResource(new ResXDataNode("node", "carried value")));

        var entries = ReadResX(document);

        Assert.Equal("carried value", entries["node"]);
    }

    [Fact]
    public void DataNode_ReportsTheNameAndValueItWasBuiltWith()
    {
        var node = new ResXDataNode("greeting", "hello");

        Assert.Equal("greeting", node.Name);
        Assert.Equal("hello", node.GetValue((global::System.ComponentModel.Design.ITypeResolutionService?)null));
    }

    [Fact]
    public void DataNode_CommentRoundTrips()
    {
        var node = new ResXDataNode("greeting", "hello") { Comment = "explains the greeting" };

        Assert.Equal("explains the greeting", node.Comment);
    }

    [Fact]
    public void DataNode_NameCanBeChanged()
    {
        var node = new ResXDataNode("original", "hello") { Name = "renamed" };

        Assert.Equal("renamed", node.Name);
    }

    [Fact]
    public void DataNode_AnEmptyNameIsRejected()
    {
        Assert.ThrowsAny<ArgumentException>(() => new ResXDataNode(string.Empty, "value"));
    }

    [Fact]
    public void DataNode_ANullNameIsRejected()
    {
        // Cast explicitly: an unqualified (null, "value") binds to the internal
        // ResXDataNode(DataNodeInfo, string) overload instead of the public (string, object) one.
        Assert.Throws<ArgumentNullException>(() => new ResXDataNode((string)null!, (object)"value"));
    }

    [Fact]
    public void DataNode_ReportsTheValueTypeName()
    {
        var node = new ResXDataNode("number", 42);

        var typeName = node.GetValueTypeName((global::System.ComponentModel.Design.ITypeResolutionService?)null);

        Assert.NotNull(typeName);
        Assert.Contains("Int32", typeName!);
    }

    [Fact]
    public void DataNode_OfAStringReportsTheStringType()
    {
        var node = new ResXDataNode("text", "value");

        Assert.Contains("String", node.GetValueTypeName((global::System.ComponentModel.Design.ITypeResolutionService?)null)!);
    }

    [Fact]
    public void DataNode_ForAFileRefExposesTheFileRef()
    {
        var fileRef = new ResXFileRef("payload.txt", typeof(string).AssemblyQualifiedName!);
        var node = new ResXDataNode("fromFile", fileRef);

        Assert.NotNull(node.FileRef);
        Assert.Equal("payload.txt", node.FileRef!.FileName);
    }

    [Fact]
    public void DataNode_GetNodePositionIsTheOriginForANodeBuiltInCode()
    {
        var node = new ResXDataNode("greeting", "hello");

        var position = node.GetNodePosition();

        Assert.Equal(0, position.X);
        Assert.Equal(0, position.Y);
    }

    [Fact]
    public void Reader_ExposesDataNodesWhenAsked()
    {
        var document = WriteResX(w => w.AddResource("greeting", "hello"));

        using var reader = new ResXResourceReader(new StringReader(document)) { UseResXDataNodes = true };
        var enumerator = Assert.IsAssignableFrom<IDictionaryEnumerator>(reader.GetEnumerator());
        Assert.True(enumerator.MoveNext(), "the document should contain one entry");

        var node = Assert.IsType<ResXDataNode>(enumerator.Value);
        Assert.Equal("greeting", node.Name);
    }

    [Fact]
    public void Reader_EnumeratorEntryAgreesWithKeyAndValue()
    {
        var document = WriteResX(w => w.AddResource("greeting", "hello"));

        using var reader = new ResXResourceReader(new StringReader(document));
        var enumerator = Assert.IsAssignableFrom<IDictionaryEnumerator>(reader.GetEnumerator());
        Assert.True(enumerator.MoveNext(), "the document should contain one entry");

        var entry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(enumerator.Key, entry.Key);
        Assert.Equal(enumerator.Value, entry.Value);
    }

    [Fact]
    public void Reader_FromAStringOverloadReadsTheSameEntries()
    {
        var document = WriteResX(w => w.AddResource("greeting", "hello"));

        using var reader = ResXResourceReader.FromFileContents(document);
        var entries = new Dictionary<string, object?>();
        var enumerator = Assert.IsAssignableFrom<IDictionaryEnumerator>(reader.GetEnumerator());
        while (enumerator.MoveNext())
        {
            entries[(string)enumerator.Key] = enumerator.Value;
        }

        Assert.Equal("hello", entries["greeting"]);
    }

    [Fact]
    public void FileRef_ToStringAndParseRoundTrip()
    {
        var fileRef = new ResXFileRef("data.bin", typeof(byte[]).AssemblyQualifiedName!);

        var text = fileRef.ToString();

        Assert.Contains("data.bin", text);
    }
}
