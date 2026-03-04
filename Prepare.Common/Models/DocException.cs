namespace Prepare.Common.Models;

internal sealed class DocException(string Type, string Description)
{
    public string Type { get; } = Type;
    public string Description { get; } = Description;

    public void Deconstruct(out string Type, out string Description)
    {
        Type = this.Type;
        Description = this.Description;
    }
}
