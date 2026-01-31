namespace ExtractTranslations;

public sealed class ExtractedComment
{
    public required string Text { get; init; }

    /// <summary>
    /// Line comment, block comment, or xml-doc
    /// </summary>
    public required string Kind { get; init; }

    /// <summary>
    /// Fully qualified method signature if available
    /// Example: void Save(string, int)
    /// </summary>
    public string? MethodSignature { get; init; }
}