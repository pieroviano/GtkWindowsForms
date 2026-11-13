namespace System.Resources.Extensions;

public sealed class RuntimeResourceSet : Gtk.Resources.Extensions.RuntimeResourceSet
{
    public RuntimeResourceSet(IResourceReader reader) : base(reader)
    {
    }
}