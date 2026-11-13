namespace Gtk.Resources.Extensions;

/// <summary>Round-trips every primitive type code plus byte arrays and streams through
/// <see cref="PreserializedResourceWriter"/> and <see cref="DeserializingResourceReader"/>. The
/// existing tests only exercised construction edge cases, so the whole typed-value decoding side
/// (<c>_LoadObjectV2</c> and friends) was unverified.</summary>
[Collection("NonParallelCollection")]
public class BinaryResourceRoundTripFunctionalTests
{
    private static Dictionary<string, object?> RoundTrip(Action<PreserializedResourceWriter> fill)
    {
        var payload = new MemoryStream();
        using (var writer = new PreserializedResourceWriter(payload))
        {
            fill(writer);
            writer.Generate();
        }

        using var reader = new DeserializingResourceReader(new MemoryStream(payload.ToArray()));
        var result = new Dictionary<string, object?>();
        var enumerator = reader.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result[(string)enumerator.Key!] = enumerator.Value;
        }

        return result;
    }

    [Fact]
    public void StringsAndNullRoundTrip()
    {
        var values = RoundTrip(w =>
        {
            w.AddResource("plain", "hello");
            w.AddResource("empty", string.Empty);
            w.AddResource("unicode", "héllo wörld 你好");
        });

        Assert.Equal("hello", values["plain"]);
        Assert.Equal(string.Empty, values["empty"]);
        Assert.Equal("héllo wörld 你好", values["unicode"]);
    }

    [Fact]
    public void EveryPrimitiveTypeCodeRoundTrips()
    {
        var values = RoundTrip(w =>
        {
            w.AddResource("bool", true);
            w.AddResource("char", 'Ω');
            w.AddResource("byte", (byte)200);
            w.AddResource("sbyte", (sbyte)-5);
            w.AddResource("short", (short)-12345);
            w.AddResource("ushort", (ushort)54321);
            w.AddResource("int", -123456789);
            w.AddResource("uint", 3123456789u);
            w.AddResource("long", -1234567890123L);
            w.AddResource("ulong", 12345678901234567890UL);
            w.AddResource("float", 3.25f);
            w.AddResource("double", -2.5e10);
            w.AddResource("decimal", 123.456m);
            w.AddResource("datetime", new DateTime(2026, 7, 22, 10, 30, 0, DateTimeKind.Utc));
            w.AddResource("timespan", TimeSpan.FromMinutes(90));
        });

        Assert.Equal(true, values["bool"]);
        Assert.Equal('Ω', values["char"]);
        Assert.Equal((byte)200, values["byte"]);
        Assert.Equal((sbyte)-5, values["sbyte"]);
        Assert.Equal((short)-12345, values["short"]);
        Assert.Equal((ushort)54321, values["ushort"]);
        Assert.Equal(-123456789, values["int"]);
        Assert.Equal(3123456789u, values["uint"]);
        Assert.Equal(-1234567890123L, values["long"]);
        Assert.Equal(12345678901234567890UL, values["ulong"]);
        Assert.Equal(3.25f, values["float"]);
        Assert.Equal(-2.5e10, values["double"]);
        Assert.Equal(123.456m, values["decimal"]);
        Assert.Equal(new DateTime(2026, 7, 22, 10, 30, 0, DateTimeKind.Utc), values["datetime"]);
        Assert.Equal(TimeSpan.FromMinutes(90), values["timespan"]);
    }

    [Fact]
    public void ByteArraysRoundTripByValue()
    {
        var payload = new byte[] { 1, 2, 3, 250, 251, 252 };

        var values = RoundTrip(w => w.AddResource("bytes", payload));

        var bytes = Assert.IsType<byte[]>(values["bytes"]);
        Assert.Equal(payload, bytes);
    }

    [Fact]
    public void StreamsComeBackAsReadableStreams()
    {
        var payload = new byte[] { 9, 8, 7, 6 };

        var values = RoundTrip(w => w.AddResource("stream", new MemoryStream(payload)));

        var stream = Assert.IsAssignableFrom<Stream>(values["stream"]);
        var read = new byte[payload.Length];
        Assert.Equal(payload.Length, stream.Read(read, 0, read.Length));
        Assert.Equal(payload, read);
    }

    [Fact]
    public void ManyResourcesAreAllFoundRegardlessOfHashOrder()
    {
        var values = RoundTrip(w =>
        {
            for (var i = 0; i < 40; i++)
            {
                w.AddResource($"key{i:D2}", i);
            }
        });

        Assert.Equal(40, values.Count);
        for (var i = 0; i < 40; i++)
        {
            Assert.Equal(i, values[$"key{i:D2}"]);
        }
    }

    [Fact]
    public void TheEnumeratorResetsAndRepeats()
    {
        var payload = new MemoryStream();
        using (var writer = new PreserializedResourceWriter(payload))
        {
            writer.AddResource("a", "1");
            writer.Generate();
        }

        using var reader = new DeserializingResourceReader(new MemoryStream(payload.ToArray()));
        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());

        enumerator.Reset();
        Assert.True(enumerator.MoveNext());
        Assert.Equal("a", enumerator.Key);
    }
}
