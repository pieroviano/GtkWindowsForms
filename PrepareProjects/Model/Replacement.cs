namespace PrepareProjects.Model;

public class Replacement(string? fileName, string? source, string? target)
{
    public string FileName { get; } = fileName ?? string.Empty;
    public string Source { get; } = source ?? string.Empty;
    public string Target { get; } = target ?? string.Empty;

    public override string ToString()
    {
        return $"{{ FileName = {FileName}, Source = {Source}, Target = {Target} }}";
    }

    public override bool Equals(object? value)
    {
        return value is Replacement other && EqualityComparer<string>.Default.Equals(other.FileName, FileName) && EqualityComparer<string>.Default.Equals(other.Source, Source) && EqualityComparer<string>.Default.Equals(other.Target, Target);
    }

    public override int GetHashCode()
    {
        var hash = 0x7a2f0b42;
        hash = (-1521134295 * hash) + EqualityComparer<string>.Default.GetHashCode(FileName);
        hash = (-1521134295 * hash) + EqualityComparer<string>.Default.GetHashCode(Source);
        return (-1521134295 * hash) + EqualityComparer<string>.Default.GetHashCode(Target);
    }
}