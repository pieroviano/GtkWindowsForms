namespace GtkWinForms.Tests.WebView2;

/// <summary>
/// GTK is not thread-safe and Application.Init is process-global, so every test that touches a widget
/// runs in one collection rather than in parallel.
/// </summary>
[CollectionDefinition("NonParallelCollection", DisableParallelization = true)]
public class NonParallelCollectionDefinition
{
}
