namespace XmlDocSync.Models;

public class UsingDifference
{
    public string FileName { get; set; }

    public List<string> AddedUsings { get; set; } = new();
}