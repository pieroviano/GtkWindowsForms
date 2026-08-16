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
    /// Currently empty: the port matches the reference on every scenario in the catalogue.
    /// </para>
    /// <para>
    /// The first entry this list ever held is a good example of what belongs here.
    /// <c>test1.structure</c> reported 54 controls on the reference but only 29 on the port, because
    /// <see cref="TableLayoutPanel" /> declared its <c>Controls</c> property with <c>new</c> while the base
    /// kept a second, empty collection - so every control nested in a table was invisible to a walk over
    /// <c>Control.Controls</c>. It was a defect, it was fixed, and the entry was removed rather than left
    /// to rot.
    /// </para>
    /// </remarks>
    public static readonly IReadOnlySet<string> Scenarios = new HashSet<string>(StringComparer.Ordinal);
}
