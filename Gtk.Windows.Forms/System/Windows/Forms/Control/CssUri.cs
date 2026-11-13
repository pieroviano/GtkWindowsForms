/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
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
    /// An <c>@import</c> statement, in either accepted form: <c>@import url("x.css");</c> or
    /// <c>@import "x.css";</c>.
    /// </summary>
    private static readonly Regex ImportStatement = new(
        @"@import\s+(?:url\(\s*(?<q1>['""]?)(?<t1>[^)'""]*)\k<q1>\s*\)|(?<q2>['""])(?<t2>[^'""]*)\k<q2>)\s*;",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>How deep a chain of nested imports is followed before giving up.</summary>
    private const int MaxImportDepth = 8;

    /// <summary>
    /// Replaces every resolvable <c>@import</c> with the text of the sheet it names, read tolerantly and
    /// with its own relative references already made absolute.
    /// </summary>
    /// <param name="css">The stylesheet text.</param>
    /// <param name="baseDirectories">The directories an import target is resolved against.</param>
    /// <returns>The stylesheet with its imports inlined.</returns>
    /// <remarks>
    /// <para>
    /// Leaving the import for Gtk to follow looks equivalent and is not, because Gtk then opens the file
    /// itself and its CSS parser requires valid UTF-8. A stylesheet in any other encoding - a national
    /// codepage in its comments is enough - makes the parser abandon the rules around the offending bytes.
    /// It reports nothing: no parsing error, nothing on stderr, the rules simply do not exist. Gtk3's
    /// parser did not validate, so those stylesheets worked, and this is the sort of file that has been
    /// sitting in a theme folder unchanged for years.
    /// </para>
    /// <para>
    /// Reading the sheet here instead keeps that contract: the text is decoded tolerantly, so a byte that
    /// is not valid UTF-8 can cost at most the character it belongs to - a comment or a label - and never a
    /// rule. Inlining also puts the imported rules exactly where the import stood, which is the cascade
    /// order they had.
    /// </para>
    /// <para>
    /// An import that cannot be resolved is left untouched for <see cref="ResolveUrls" /> to absolutise, so
    /// behaviour is unchanged wherever this cannot help.
    /// </para>
    /// </remarks>
    internal static string InlineImports(string css, params string[] baseDirectories)
    {
        return InlineImports(css, baseDirectories, 0);
    }

    private static string InlineImports(string css, string[] baseDirectories, int depth)
    {
        if (string.IsNullOrEmpty(css) || baseDirectories == null || baseDirectories.Length == 0
            || depth >= MaxImportDepth)
        {
            return css;
        }

        return ImportStatement.Replace(css, match =>
        {
            var target = (match.Groups["t1"].Success ? match.Groups["t1"].Value : match.Groups["t2"].Value)
                .Trim();

            if (target.Length == 0 || SchemePrefix.IsMatch(target))
            {
                return match.Value;
            }

            var path = Locate(target, baseDirectories);
            if (path == null)
            {
                return match.Value;
            }

            try
            {
                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    return match.Value;
                }

                // The imported sheet's own references are relative to where that sheet lives, which is what
                // Gtk would have used had it opened the file itself.
                var imported = InlineImports(ReadTolerantly(path), new[] { directory! }, depth + 1);

                return ResolveUrls(imported, directory!);
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug($"Unable to inline the stylesheet imported as {target}", ex);
                return match.Value;
            }
        });
    }

    /// <summary>
    /// Reads a stylesheet without ever failing on its encoding.
    /// </summary>
    /// <param name="path">The file to read.</param>
    /// <returns>The stylesheet text.</returns>
    /// <remarks>
    /// Strict UTF-8 first, so a well-formed sheet is decoded exactly. Failing that the bytes are taken as
    /// Latin-1, which cannot fail and maps every byte to one character: the rules - which are ASCII - come
    /// through intact, and only text in some other encoding, in practice a comment, reads as mojibake. That
    /// is the right trade here, because the alternative Gtk offers is losing the rules.
    /// </remarks>
    private static string ReadTolerantly(string path)
    {
        var bytes = File.ReadAllBytes(path);

        try
        {
            return new Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes);
        }
        catch (ArgumentException)
        {
            return Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
        }
    }

    /// <summary>
    /// Finds an import target under the candidate base directories.
    /// </summary>
    /// <param name="target">The reference as it appears in the stylesheet.</param>
    /// <param name="baseDirectories">The directories to try, most significant first.</param>
    /// <returns>The full path of the first directory that holds it, or <see langword="null" />.</returns>
    private static string? Locate(string target, string[] baseDirectories)
    {
        foreach (var directory in baseDirectories)
        {
            if (string.IsNullOrEmpty(directory))
            {
                continue;
            }

            try
            {
                var full = Path.GetFullPath(Path.Combine(directory, target));
                if (File.Exists(full))
                {
                    return full;
                }
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug($"Unable to locate {target} under {directory}", ex);
            }
        }

        return null;
    }

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
