using System.Text;
using Xunit;

namespace SampleParity.Tests;

/// <summary>
/// Drives the reference sample (Gtk3, in the <c>gtksystem-windows-forms</c> submodule) and the ported
/// sample (Gtk4) through the same scenario catalogue, and asserts they behave the same.
/// </summary>
/// <remarks>
/// The two harnesses run as child processes because GTK 3 and GTK 4 cannot be loaded into one process, and
/// the two samples' libraries declare the same managed type names. See
/// <c>SampleParityUITests-Plan.md</c> §0 B1.
/// </remarks>
[Collection("SampleParity")]
public class ParityTests
{
    private static readonly Lazy<IReadOnlyList<TraceRecord>> Gtk3 =
        new(() => HarnessRunner.Run("Gtk3Harness", "SampleParity.Gtk3Harness"));

    private static readonly Lazy<IReadOnlyList<TraceRecord>> Gtk4 =
        new(() => HarnessRunner.Run("Gtk4Harness", "SampleParity.Gtk4Harness"));

    [Fact]
    public void ReferenceHarness_DrivesTheSubmoduleSample()
    {
        var trace = Gtk3.Value;

        Assert.NotEmpty(trace);
        Assert.Contains(trace, r => r.Scenario == "form3.structure");
    }

    [Fact]
    public void PortedHarness_DrivesTheGtk4Sample()
    {
        var trace = Gtk4.Value;

        Assert.NotEmpty(trace);
        Assert.Contains(trace, r => r.Scenario == "form3.structure");
    }

    [Fact]
    public void BothHarnesses_RunTheSameScenarios()
    {
        // If the catalogue somehow ran differently on the two sides, every later comparison would be
        // comparing different things, so this is checked before the traces themselves.
        var gtk3 = Gtk3.Value.Select(r => r.Scenario).Distinct().OrderBy(s => s, StringComparer.Ordinal);
        var gtk4 = Gtk4.Value.Select(r => r.Scenario).Distinct().OrderBy(s => s, StringComparer.Ordinal);

        Assert.Equal(gtk3, gtk4);
    }

    [Fact]
    public void ReferenceHarness_ReportsNoErrors()
    {
        AssertNoErrors(Gtk3.Value, "Gtk3");
    }

    [Fact]
    public void PortedHarness_ReportsNoErrors()
    {
        AssertNoErrors(Gtk4.Value, "Gtk4");
    }

    [Fact]
    public void PortedSample_BehavesLikeTheReferenceSample()
    {
        var gtk3 = Gtk3.Value.Where(NotKnownToDiverge).ToList();
        var gtk4 = Gtk4.Value.Where(NotKnownToDiverge).ToList();

        var differences = Compare(gtk3, gtk4);

        Assert.True(differences.Count == 0, Describe(differences));
    }

    [Fact]
    public void KnownDivergences_AreStillDiverging()
    {
        // A backlog entry that no longer diverges is done, and leaving it listed would hide the next
        // regression in that scenario. This fails when an entry can be deleted.
        var stale = new List<string>();

        foreach (var scenario in KnownDivergences.Scenarios.OrderBy(s => s, StringComparer.Ordinal))
        {
            var gtk3 = Gtk3.Value.Where(r => r.Scenario == scenario).ToList();
            var gtk4 = Gtk4.Value.Where(r => r.Scenario == scenario).ToList();

            if (gtk3.Count > 0 && Compare(gtk3, gtk4).Count == 0)
            {
                stale.Add(scenario);
            }
        }

        Assert.True(stale.Count == 0,
            "These scenarios now match and should be removed from KnownDivergences: " + string.Join(", ", stale));
    }

    private static bool NotKnownToDiverge(TraceRecord record)
    {
        return !KnownDivergences.Scenarios.Contains(record.Scenario);
    }

    private static void AssertNoErrors(IReadOnlyList<TraceRecord> trace, string side)
    {
        // An Error record means a scenario could not run as written - a control it addresses is missing, or
        // it threw. Left unchecked, such a scenario would contribute nothing and the parity comparison would
        // pass vacuously.
        var errors = trace
            .Where(r => r.Kind == TraceKind.Error && !KnownDivergences.Scenarios.Contains(r.Scenario))
            .ToList();

        Assert.True(errors.Count == 0,
            $"{side} reported {errors.Count} error record(s):{Environment.NewLine}"
            + string.Join(Environment.NewLine, errors.Select(e => "  " + e)));
    }

    /// <summary>Compares two traces as ordered sequences.</summary>
    /// <param name="gtk3">The reference trace.</param>
    /// <param name="gtk4">The ported trace.</param>
    /// <returns>The differing positions, as (reference, ported) pairs; either side may be null.</returns>
    private static List<(TraceRecord? Reference, TraceRecord? Ported)> Compare(
        IReadOnlyList<TraceRecord> gtk3,
        IReadOnlyList<TraceRecord> gtk4)
    {
        var differences = new List<(TraceRecord?, TraceRecord?)>();

        for (var i = 0; i < Math.Max(gtk3.Count, gtk4.Count); i++)
        {
            var a = i < gtk3.Count ? gtk3[i] : null;
            var b = i < gtk4.Count ? gtk4[i] : null;

            if (a?.ToLine() != b?.ToLine())
            {
                differences.Add((a, b));
            }
        }

        return differences;
    }

    private static string Describe(IReadOnlyList<(TraceRecord? Reference, TraceRecord? Ported)> differences)
    {
        var builder = new StringBuilder();
        builder.Append(differences.Count).AppendLine(" difference(s) between the reference and the port:");

        foreach (var (reference, ported) in differences.Take(25))
        {
            builder.Append("  gtk3: ").AppendLine(reference?.ToString() ?? "<missing>");
            builder.Append("  gtk4: ").AppendLine(ported?.ToString() ?? "<missing>");
            builder.AppendLine();
        }

        if (differences.Count > 25)
        {
            builder.Append("  ... and ").Append(differences.Count - 25).AppendLine(" more.");
        }

        return builder.ToString();
    }
}

/// <summary>
/// Serialises the parity tests: each one may launch a harness, and two GTK processes fighting over the
/// display makes them flaky.
/// </summary>
[CollectionDefinition("SampleParity", DisableParallelization = true)]
public class SampleParityCollection
{
}
