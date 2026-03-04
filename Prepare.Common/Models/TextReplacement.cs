namespace Prepare.Common.Models;

internal class TextReplacement
{
    public string OldText { get; set; } = string.Empty;
    public string NewText { get; set; } = string.Empty;

    public override string ToString()
        {
            return $"{{ OldText = {OldText}, NewText = {NewText} }}";
    }
}
