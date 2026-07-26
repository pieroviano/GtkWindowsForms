namespace System.Drawing.Text;

/// <summary>
/// Represents the fonts installed on the system. The families are enumerated from the active Pango font map.
/// </summary>
public sealed class InstalledFontCollection : FontCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Drawing.Text.InstalledFontCollection" /> class.
    /// </summary>
    public InstalledFontCollection()
    {
        try
        {
            var installed = FontFamily.Families;
            if (installed != null)
            {
                families.AddRange(installed);
            }
        }
        catch (Exception)
        {
            // No active Pango font map (e.g. a headless process with no display): expose an empty
            // collection rather than throwing, matching the always-usable WinForms contract.
        }
    }
}
