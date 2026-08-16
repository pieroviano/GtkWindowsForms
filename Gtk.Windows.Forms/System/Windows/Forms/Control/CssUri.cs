/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 */

using System.IO;
using System.Text.RegularExpressions;

namespace System.Windows.Forms;

/// <summary>
/// Makes the <c>url(...)</c> references of a stylesheet absolute before it is loaded from a string.
/// </summary>
/// <remarks>
/// Gtk3 gave a stylesheet parsed out of a string an implicit base directory: its parser resolved a
/// relative <c>url()</c> with <c>g_file_new_for_path()</c>, that is, against the process working
/// directory. Gtk4's <c>gtk_css_parser_resolve_url()</c> instead returns nothing whenever the parser has
/// no directory of its own, which is always the case for <c>LoadFromData</c>/<c>LoadFromString</c>. Every
/// relative reference - an image, and just as importantly an <c>@import</c> - is therefore dropped
/// <em>silently</em>: no parsing error is raised, nothing reaches stderr, the declaration simply never
/// exists. Rewriting the references to absolute <c>file:///</c> URIs before the text reaches the provider
/// restores the Gtk3 contract for every consumer of the library, and an imported sheet then gets a base
/// directory of its own, so relative URLs nested inside it resolve natively.
/// </remarks>
internal static class CssUri
{
    /// <summary>
    /// A <c>url()</c> token, with an optional single- or double-quoted target.
    /// </summary>
    private static readonly Regex UrlToken = new(
        @"url\(\s*(?<q>['""]?)(?<target>[^)'""]*)\k<q>\s*\)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// A URI scheme prefix. Two characters at least are required before the colon, so that a Windows
    /// drive letter ("C:/theme/x.png") is treated as a path rather than as a scheme.
    /// </summary>
    private static readonly Regex SchemePrefix = new(
        @"^[a-zA-Z][a-zA-Z0-9+.\-]+:",
        RegexOptions.CultureInvariant);

    /// <summary>
    /// Rewrites every relative <c>url(...)</c> in <paramref name="css" /> as an absolute
    /// <c>file:///</c> URI.
    /// </summary>
    /// <param name="css">The stylesheet text, as it would be handed to a <see cref="Gtk.CssProvider" />.</param>
    /// <param name="baseDirectories">
    /// The directories a relative reference is resolved against, most significant first. The first
    /// directory that actually holds the target wins; when none does, the first usable directory is used
    /// anyway, so that Gtk reports the reference as invalid exactly as it would have done before.
    /// </param>
    /// <returns>
    /// The stylesheet with its relative references made absolute. Tokens that are empty, that are a
    /// fragment (<c>url(#gradient)</c>) or that already carry a scheme (<c>file:</c>, <c>resource:</c>,
    /// <c>data:</c>, <c>http:</c>, ...) are returned untouched, as is any token that cannot be resolved.
    /// </returns>
    internal static string ResolveUrls(string css, params string[] baseDirectories)
    {
        if (string.IsNullOrEmpty(css) || baseDirectories == null || baseDirectories.Length == 0)
        {
            return css;
        }

        return UrlToken.Replace(css, match =>
        {
            var target = match.Groups["target"].Value.Trim();
            if (target.Length == 0 || target[0] == '#' || SchemePrefix.IsMatch(target))
            {
                return match.Value;
            }

            var resolved = ToAbsoluteUri(target, baseDirectories);

            return resolved == null ? match.Value : $"url(\"{resolved}\")";
        });
    }

    /// <summary>
    /// Resolves one relative reference against the candidate base directories.
    /// </summary>
    /// <param name="target">The reference as it appears in the stylesheet.</param>
    /// <param name="baseDirectories">The directories to try, most significant first.</param>
    /// <returns>An absolute <c>file:///</c> URI, or <see langword="null" /> if none could be built.</returns>
    private static string? ToAbsoluteUri(string target, string[] baseDirectories)
    {
        string? fallback = null;
        foreach (var directory in baseDirectories)
        {
            if (string.IsNullOrEmpty(directory))
            {
                continue;
            }

            try
            {
                var full = Path.GetFullPath(Path.Combine(directory, target));
                var uri = new Uri(full).AbsoluteUri;
                if (File.Exists(full) || Directory.Exists(full))
                {
                    return uri;
                }

                fallback ??= uri;
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug($"Unable to build a CSS URI for {target} under {directory}", ex);
            }
        }

        return fallback;
    }
}
