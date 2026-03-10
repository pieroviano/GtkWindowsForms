using Prepare.Common.Content;
using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class CsFileOrganizerRegionTests
{
    [Fact]
    public void Removes_Region_Directives_But_Preserves_Content()
    {
        var code = """
                   #region Fields
                   class A
                   {
                       #region Inner
                       private int x;
                       #endregion
                   }
                   #endregion
                   """;

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.DoesNotContain("#region", result);
        Assert.DoesNotContain("#endregion", result);

        Assert.Contains("class A", result);
        Assert.Contains("private int x", result);
    }

    private static string WriteTempFile(string contents)
    {
        var path = Path.GetTempFileName() + ".cs";
        CsFile.WriteAllText(path, contents);
        return path;
    }
}
