using Prepare.Common.Languages;
using Prepare.Common.Logging;

namespace Align.Common;

/// <summary>
/// The one-time stop that lets the operator fill in the blank-English entries the enrichment pass just
/// added, before the transform/diff/apply that depends on them. Shared by the alignment tools;
/// AlignSamples adds its proposed resource keys and data-file placeholders to the same listing.
/// </summary>
public static class ManualTranslationPause
{
    /// <summary>
    /// The samples' variant (AlignSamples-Plan.md Phase 2): one combined stop listing the proposed
    /// resource keys (with their file/line context), the new blank dictionary entries, and the data-file
    /// siblings awaiting translation. The operator may rename any proposed key before it ossifies into
    /// OURS; anything left unfilled simply stays Chinese at apply time.
    /// </summary>
    public static void RunForSamples(SampleEnrichmentResult enrichment, string resourcesDir)
    {
        if (!enrichment.HasWork)
        {
            Console.WriteLine("No new Chinese in the samples - continuing.");
            return;
        }

        var details = new List<string>(enrichment.Describe())
        {
            string.Empty,
            "All of the above live under:",
            $"  {resourcesDir}"
        };

        Run(enrichment.NewDictionaryEntries.Count, enrichment.NewKeys.Count, null, resourcesDir, details);
    }

    /// <summary>
    /// Reports what was added and waits for ENTER. Skips the pause when there is nothing new to
    /// translate, or when input is redirected (non-interactive run).
    /// </summary>
    public static void Run(int dictAdded, int resxAdded, string? translationsXml, string resourcesDir,
        IReadOnlyList<string>? extraDetails = null)
    {
        if (dictAdded == 0 && resxAdded == 0 && (extraDetails == null || extraDetails.Count == 0))
        {
            Console.WriteLine("No new Chinese to translate - continuing.");
            return;
        }

        var separator = new string('=', 72);
        ConsoleColor.Yellow.Write(separator, true);
        ConsoleColor.Yellow.Write("MANUAL TRANSLATION REQUIRED", true);
        ConsoleColor.Yellow.Write(separator, true);

        if (dictAdded > 0)
        {
            Console.WriteLine($"{dictAdded} new blank-English entr{(dictAdded == 1 ? "y was" : "ies were")} added to:");
            Console.WriteLine($"  {translationsXml ?? Path.Combine(resourcesDir, "samplesTranslations.xml")}");
        }

        if (resxAdded > 0)
        {
            Console.WriteLine($"{resxAdded} blank resx entr{(resxAdded == 1 ? "y was" : "ies were")} added under:");
            Console.WriteLine($"  {resourcesDir}");
        }

        if (extraDetails != null)
        {
            foreach (var detail in extraDetails)
            {
                Console.WriteLine(detail);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Fill in the empty <English></English> values (and any blank resx <value/>),");
        Console.WriteLine("then return here to continue.");

        if (Console.IsInputRedirected)
        {
            ConsoleColor.Yellow.Write("Input is redirected (non-interactive) - not pausing; continuing.", true);
            return;
        }

        Console.Write("Press ENTER when the translations are done (or Ctrl+C to abort)... ");
        Console.ReadLine();
        ConsoleColor.Green.Write("Continuing.", true);
    }
}
