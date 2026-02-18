namespace UpdatePostProcessing.Model;

public class ToPersist
{
    public string SourceFileName { get; }
    public string TargetFileName { get; }
    public string SourceString { get; set; }
    public string TargetString { get; }

    public ToPersist(string sourceFileName, string targetFileName, string sourceString, string targetString)
    {
        SourceFileName = sourceFileName;
        TargetFileName = targetFileName;
        SourceString = sourceString;
        TargetString = targetString;
    }

    public override string ToString()
    {
        return $"{{ SourceFileName = {SourceFileName}, TargetFileName = {TargetFileName}, SourceString = {SourceString}, TargetString = {TargetString} }}";
    }
}