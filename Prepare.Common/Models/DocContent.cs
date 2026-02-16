namespace Prepare.Common.Models;

internal sealed class DocContent
{
    public List<string> SummaryLines { get; } = new();
    public List<DocParam> Parameters { get; } = new();
    public List<DocException> Exceptions { get; } = new();
}