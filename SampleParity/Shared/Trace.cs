using System.Globalization;
using System.Text;

namespace SampleParity;

/// <summary>The kind of thing a <see cref="TraceRecord" /> reports.</summary>
internal enum TraceKind
{
    /// <summary>Observable control or model state, e.g. a selected index.</summary>
    State,

    /// <summary>Widget geometry, recorded as an invariant rather than a raw pixel count.</summary>
    Geometry,

    /// <summary>A window was opened or closed.</summary>
    Window,

    /// <summary>The scenario could not run as written - a missing control, or a throw.</summary>
    Error,
}

/// <summary>
/// One observation made while running a scenario. A run of the catalogue produces an ordered sequence of
/// these, and the parity check is a plain ordered diff of two such sequences.
/// </summary>
internal sealed class TraceRecord
{
    /// <summary>Initialises a new instance of the <see cref="TraceRecord" /> class.</summary>
    /// <param name="scenario">The scenario id.</param>
    /// <param name="step">The 0-based step within the scenario.</param>
    /// <param name="kind">What sort of observation this is.</param>
    /// <param name="control">The logical control name, or the form name for window records.</param>
    /// <param name="property">The property or invariant observed.</param>
    /// <param name="value">The observed value, already normalised.</param>
    public TraceRecord(string scenario, int step, TraceKind kind, string control, string property, string value)
    {
        Scenario = scenario;
        Step = step;
        Kind = kind;
        Control = control;
        Property = property;
        Value = value;
    }

    /// <summary>Gets the scenario id.</summary>
    public string Scenario { get; }

    /// <summary>Gets the 0-based step within the scenario.</summary>
    public int Step { get; }

    /// <summary>Gets the kind of observation.</summary>
    public TraceKind Kind { get; }

    /// <summary>Gets the logical control name.</summary>
    public string Control { get; }

    /// <summary>Gets the property or invariant observed.</summary>
    public string Property { get; }

    /// <summary>Gets the observed value.</summary>
    public string Value { get; }

    /// <summary>Renders this record as one line of the trace file.</summary>
    /// <returns>A tab-separated line; the value is the last field so it may contain spaces.</returns>
    /// <remarks>
    /// Tab-separated rather than JSON so the harnesses need no serializer dependency and a failing diff is
    /// readable in the test output as-is.
    /// </remarks>
    public string ToLine()
    {
        return string.Join("\t",
            Scenario,
            Step.ToString(CultureInfo.InvariantCulture),
            Kind.ToString(),
            Control,
            Property,
            Escape(Value));
    }

    /// <summary>Parses a line produced by <see cref="ToLine" />.</summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>The record, or <see langword="null" /> for a blank or malformed line.</returns>
    public static TraceRecord? Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        var parts = line.Split('\t');
        if (parts.Length != 6)
        {
            return null;
        }

        return new TraceRecord(
            parts[0],
            int.Parse(parts[1], CultureInfo.InvariantCulture),
            (TraceKind)Enum.Parse(typeof(TraceKind), parts[2]),
            parts[3],
            parts[4],
            Unescape(parts[5]));
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
    }

    private static string Unescape(string value)
    {
        var builder = new StringBuilder(value.Length);

        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] != '\\' || i + 1 >= value.Length)
            {
                builder.Append(value[i]);
                continue;
            }

            i++;
            builder.Append(value[i] switch
            {
                't' => '\t',
                'r' => '\r',
                'n' => '\n',
                _ => value[i],
            });
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Scenario}[{Step}] {Kind} {Control}.{Property} = {Value}";
    }
}

/// <summary>
/// Collects the observations of one catalogue run and writes them out.
/// </summary>
internal sealed class TraceWriter
{
    private readonly List<TraceRecord> _records = new List<TraceRecord>();

    /// <summary>Gets the records collected so far, in order.</summary>
    public IReadOnlyList<TraceRecord> Records => _records;

    /// <summary>Records one observation.</summary>
    /// <param name="scenario">The scenario id.</param>
    /// <param name="step">The step index.</param>
    /// <param name="kind">The observation kind.</param>
    /// <param name="control">The logical control name.</param>
    /// <param name="property">The property observed.</param>
    /// <param name="value">The normalised value.</param>
    public void Add(string scenario, int step, TraceKind kind, string control, string property, string value)
    {
        _records.Add(new TraceRecord(scenario, step, kind, control, property, value));
    }

    /// <summary>Writes the trace to a file, one record per line.</summary>
    /// <param name="path">The destination path; its directory is created if needed.</param>
    public void Save(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        File.WriteAllLines(path, _records.Select(r => r.ToLine()), Encoding.UTF8);
    }
}
