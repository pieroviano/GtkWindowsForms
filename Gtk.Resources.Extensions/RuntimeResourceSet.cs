namespace System.Resources.Extensions;

public sealed class RuntimeResourceSet : GtkSystem.Resources.Extensions.RuntimeResourceSet
{
    public RuntimeResourceSet(IResourceReader reader) : base(reader)
    {
    }
}