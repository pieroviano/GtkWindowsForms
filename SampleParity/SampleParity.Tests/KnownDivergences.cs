namespace SampleParity.Tests;

/// <summary>
/// Scenarios whose Gtk3 and Gtk4 traces are known to differ.
/// </summary>
/// <remarks>
/// <para>
/// This is a <b>backlog, not a suppression file</b>. Every entry is a difference between the reference
/// sample and the port that has been seen but not yet resolved, and each carries a note saying what it is.
/// Removing an entry is the definition of done for that item; adding one requires the same justification a
/// reviewer would expect in a commit message.
/// </para>
/// <para>
/// Entries fall into two kinds, and the distinction matters. A <i>defect</i> is the port behaving worse than
/// the reference and should be fixed. An <i>intentional divergence</i> is the port behaving deliberately
/// differently - the plan's §8 case, e.g. the Gtk3 build clamping every strip to 30px and ignoring the
/// designed size. Both are listed here; only the first kind is a bug.
/// </para>
/// </remarks>
internal static class KnownDivergences
{
    /// <summary>Scenario ids whose traces are not yet expected to match.</summary>
    /// <remarks>
    /// <para>
    /// <c>test1.structure</c> - <b>DEFECT, unresolved.</b> Walking <c>Controls</c> recursively reports 54
    /// controls on the reference (16 Labels, 5 Panels, 8 PictureBoxes) but only 29 on the port (2 Labels,
    /// 1 Panel, 1 PictureBox). The two designers declare exactly the same controls - 16, 5 and 8
    /// respectively - so this is not a difference in what the form contains. <c>Test1</c> nests most of its
    /// controls inside Panels, and <c>Form3</c>, which does not, agrees exactly (24 on both), so the likely
    /// cause is that a container's <c>Controls</c> collection does not expose its children on the Gtk4 side.
    /// That would affect any consumer enumerating a container, not just this test.
    /// </para>
    /// </remarks>
    public static readonly IReadOnlySet<string> Scenarios = new HashSet<string>(StringComparer.Ordinal)
    {
        "test1.structure",
    };
}
