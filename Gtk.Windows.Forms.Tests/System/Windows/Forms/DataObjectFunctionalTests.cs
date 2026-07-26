namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the top-level <see cref="DataObject" /> — the in-memory, format-keyed store that
/// <c>new DataObject(...)</c> and drag-and-drop code use. It is independent of the clipboard-backed
/// <see cref="Clipboard.DataObject" />, so these assertions touch no GTK clipboard and need no form.
/// </summary>
public class DataObjectFunctionalTests
{
    [Fact]
    public void IsTopLevel_AndImplementsIDataObject()
    {
        Assert.Equal("System.Windows.Forms", typeof(DataObject).Namespace);
        Assert.False(typeof(DataObject).IsNested);
        Assert.True(typeof(IDataObject).IsAssignableFrom(typeof(DataObject)));
    }

    [Fact]
    public void SetData_GetData_ByStringFormat_RoundTrips()
    {
        var data = new DataObject();
        data.SetData("MyFormat", "payload");

        Assert.True(data.GetDataPresent("MyFormat"));
        Assert.Equal("payload", data.GetData("MyFormat"));
        Assert.Contains("MyFormat", data.GetFormats());
    }

    [Fact]
    public void SetData_ByType_UsesTypeName()
    {
        var data = new DataObject();
        data.SetData(typeof(int), 42);

        Assert.True(data.GetDataPresent(typeof(int)));
        Assert.Equal(42, data.GetData(typeof(int)));
    }

    [Fact]
    public void SetData_ObjectOnly_KeysByRuntimeType()
    {
        var data = new DataObject(new int[] { 1, 2, 3 });

        Assert.True(data.GetDataPresent(typeof(int[])));
        Assert.Equal(new[] { 1, 2, 3 }, (int[])data.GetData(typeof(int[]))!);
    }

    [Fact]
    public void Text_SetGetContains_RoundTrips()
    {
        var data = new DataObject();
        Assert.False(data.ContainsText());
        Assert.Equal(string.Empty, data.GetText());

        data.SetText("hello world");
        Assert.True(data.ContainsText());
        Assert.Equal("hello world", data.GetText());
    }

    [Fact]
    public void GetData_MissingFormat_ReturnsNull()
    {
        var data = new DataObject();
        Assert.Null(data.GetData("absent"));
        Assert.False(data.GetDataPresent("absent"));
    }

    [Fact]
    public void FileDropList_RoundTrips()
    {
        var data = new DataObject();
        var paths = new System.Collections.Specialized.StringCollection();
        paths.Add(@"C:\a.txt");
        paths.Add(@"C:\b.txt");
        data.SetFileDropList(paths);

        Assert.True(data.ContainsFileDropList());
        var back = data.GetFileDropList();
        Assert.Equal(2, back.Count);
        Assert.Equal(@"C:\a.txt", back[0]);
    }

    [Fact]
    public void Text_UnicodeAutoConvertsFromText()
    {
        var data = new DataObject();
        data.SetData(DataFormats.Text, "ascii");
        // UnicodeText is auto-convertible from Text in WinForms' conversion set.
        Assert.True(data.GetDataPresent(DataFormats.UnicodeText, true));
        Assert.Equal("ascii", data.GetData(DataFormats.UnicodeText, true));
    }
}
