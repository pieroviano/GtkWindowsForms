namespace Prepare.Common.Models;

internal sealed class DocParam(string Name, string Description)
{
    public string Name { get; } = Name;
    public string Description { get; } = Description;

    public void Deconstruct(out string Name, out string Description)
    {
        Name = this.Name;
        Description = this.Description;
    }
}
