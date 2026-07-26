using System.Collections;
using GtkSystem.Resources.Extensions;

namespace GtkWinForms.Tests.GtkSystem.Resources.Extensions;

/// <summary>Behavioural tests for the binary resource pipeline. What
/// <see cref="PreserializedResourceWriter"/> writes must come back out of
/// <see cref="DeserializingResourceReader"/> and <see cref="RuntimeResourceSet"/> unchanged — that
/// round-trip is the whole point of the format, and the existing tests never performed one.</summary>
[Collection("NonParallelCollection")]
public class ResourceRoundTripFunctionalTests
{
    /// <summary>Writes the supplied entries and hands back a rewound stream holding the .resources blob.</summary>
    private static MemoryStream Write(Action<PreserializedResourceWriter> add)
    {
        // The writer closes the stream it was given, so write into a scratch buffer and hand back a fresh
        // stream over the bytes. ToArray still works on a closed MemoryStream; Length does not.
        var payload = new MemoryStream();
        using (var writer = new PreserializedResourceWriter(payload))
        {
            add(writer);
            writer.Generate();
        }

        return new MemoryStream(payload.ToArray());
    }

    private static Dictionary<string, object?> ReadAll(Stream stream)
    {
        var result = new Dictionary<string, object?>();
        using var reader = new DeserializingResourceReader(stream);
        var enumerator = reader.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result[(string)enumerator.Key] = enumerator.Value;
        }

        return result;
    }

    [Fact]
    public void AStringResource_RoundTrips()
    {
        using var stream = Write(w => w.AddResource("greeting", "hello world"));

        var entries = ReadAll(stream);

        Assert.Equal("hello world", entries["greeting"]);
    }

    [Fact]
    public void SeveralStringResources_AllRoundTrip()
    {
        using var stream = Write(w =>
        {
            w.AddResource("first", "one");
            w.AddResource("second", "two");
            w.AddResource("third", "three");
        });

        var entries = ReadAll(stream);

        Assert.Equal(3, entries.Count);
        Assert.Equal("one", entries["first"]);
        Assert.Equal("two", entries["second"]);
        Assert.Equal("three", entries["third"]);
    }

    [Fact]
    public void AnEmptyStringValue_RoundTrips()
    {
        using var stream = Write(w => w.AddResource("blank", string.Empty));

        var entries = ReadAll(stream);

        Assert.Equal(string.Empty, entries["blank"]);
    }

    [Fact]
    public void NonAsciiText_RoundTripsIntact()
    {
        const string text = "café über 中文";
        using var stream = Write(w => w.AddResource("unicode", text));

        var entries = ReadAll(stream);

        Assert.Equal(text, entries["unicode"]);
    }

    [Fact]
    public void AByteArrayResource_RoundTrips()
    {
        var payload = new byte[] { 1, 2, 3, 250, 251 };
        using var stream = Write(w => w.AddResource("blob", payload));

        var entries = ReadAll(stream);

        Assert.Equal(payload, Assert.IsType<byte[]>(entries["blob"]));
    }

    [Theory]
    [InlineData(42)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void AnIntegerResource_RoundTrips(int value)
    {
        using var stream = Write(w => w.AddResource("number", value));

        var entries = ReadAll(stream);

        Assert.Equal(value, entries["number"]);
    }

    [Fact]
    public void PrimitiveResourcesOfSeveralTypes_KeepTheirTypes()
    {
        using var stream = Write(w =>
        {
            w.AddResource("flag", true);
            w.AddResource("count", 7);
            w.AddResource("ratio", 1.5d);
            w.AddResource("letter", 'x');
            w.AddResource("big", 9_000_000_000L);
        });

        var entries = ReadAll(stream);

        Assert.Equal(true, entries["flag"]);
        Assert.Equal(7, entries["count"]);
        Assert.Equal(1.5d, entries["ratio"]);
        Assert.Equal('x', entries["letter"]);
        Assert.Equal(9_000_000_000L, entries["big"]);
    }

    [Fact]
    public void KeysAreEnumeratedForEveryResourceWritten()
    {
        using var stream = Write(w =>
        {
            w.AddResource("a", "1");
            w.AddResource("b", "2");
        });

        var entries = ReadAll(stream);

        Assert.Contains("a", entries.Keys);
        Assert.Contains("b", entries.Keys);
    }

    [Fact]
    public void ARewrittenStream_ProducesAReadableResourceSet()
    {
        using var stream = Write(w => w.AddResource("greeting", "hello"));

        using var reader = new DeserializingResourceReader(stream);
        using var set = new RuntimeResourceSet(reader);

        Assert.Equal("hello", set.GetString("greeting"));
    }

    [Fact]
    public void ResourceSet_LooksUpByNameAndReportsMissingKeysAsNull()
    {
        using var stream = Write(w =>
        {
            w.AddResource("known", "value");
        });

        using var reader = new DeserializingResourceReader(stream);
        using var set = new RuntimeResourceSet(reader);

        Assert.Equal("value", set.GetString("known"));
        Assert.Null(set.GetString("absent"));
    }

    [Fact]
    public void ResourceSet_GetObjectReturnsTheSameValueAsGetString()
    {
        using var stream = Write(w => w.AddResource("known", "value"));

        using var reader = new DeserializingResourceReader(stream);
        using var set = new RuntimeResourceSet(reader);

        Assert.Equal(set.GetString("known"), set.GetObject("known"));
    }

    [Fact]
    public void ResourceSet_CaseInsensitiveLookupFindsTheKey()
    {
        using var stream = Write(w => w.AddResource("Known", "value"));

        using var reader = new DeserializingResourceReader(stream);
        using var set = new RuntimeResourceSet(reader);

        Assert.Equal("value", set.GetString("known", true));
    }

    [Fact]
    public void ResourceSet_EnumeratesEveryEntry()
    {
        using var stream = Write(w =>
        {
            w.AddResource("a", "1");
            w.AddResource("b", "2");
        });

        using var reader = new DeserializingResourceReader(stream);
        using var set = new RuntimeResourceSet(reader);

        var seen = new List<string>();
        var enumerator = set.GetEnumerator();
        while (enumerator.MoveNext())
        {
            seen.Add((string)enumerator.Key);
        }

        Assert.Equal(2, seen.Count);
        Assert.Contains("a", seen);
        Assert.Contains("b", seen);
    }

    [Fact]
    public void Reader_EnumeratorCanBeWalkedTwiceAfterReset()
    {
        using var stream = Write(w => w.AddResource("a", "1"));
        using var reader = new DeserializingResourceReader(stream);

        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext(), "the first pass should yield the entry");
        var firstKey = enumerator.Key;

        enumerator.Reset();
        Assert.True(enumerator.MoveNext(), "after Reset the enumerator should yield the entry again");

        Assert.Equal(firstKey, enumerator.Key);
    }

    [Fact]
    public void Reader_EnumeratorEntryMatchesKeyAndValue()
    {
        using var stream = Write(w => w.AddResource("a", "1"));
        using var reader = new DeserializingResourceReader(stream);

        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext(), "there should be an entry to read");

        var entry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(enumerator.Key, entry.Key);
        Assert.Equal(enumerator.Value, entry.Value);
    }

    [Fact]
    public void Writer_AddingTheSameNameTwiceIsRejected()
    {
        var payload = new MemoryStream();
        using var writer = new PreserializedResourceWriter(payload);
        writer.AddResource("duplicate", "first");

        Assert.ThrowsAny<ArgumentException>(() => writer.AddResource("duplicate", "second"));
    }

    [Fact]
    public void Writer_ANullNameIsRejected()
    {
        var payload = new MemoryStream();
        using var writer = new PreserializedResourceWriter(payload);

        Assert.Throws<ArgumentNullException>(() => writer.AddResource(null!, "value"));
    }

    [Fact]
    public void AnEmptyResourceFile_ReadsBackAsNoEntries()
    {
        using var stream = Write(_ => { });

        var entries = ReadAll(stream);

        Assert.Empty(entries);
    }

    [Fact]
    public void ManyResources_AllSurviveTheRoundTrip()
    {
        const int count = 250;
        using var stream = Write(w =>
        {
            for (var i = 0; i < count; i++)
            {
                w.AddResource($"key{i}", $"value{i}");
            }
        });

        var entries = ReadAll(stream);

        Assert.Equal(count, entries.Count);
        Assert.Equal("value0", entries["key0"]);
        Assert.Equal($"value{count - 1}", entries[$"key{count - 1}"]);
    }
}
