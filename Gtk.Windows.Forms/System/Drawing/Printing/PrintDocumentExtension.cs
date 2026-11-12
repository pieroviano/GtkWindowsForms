namespace System.Drawing.Printing;

public static class PrintDocumentExtension
{
    /// <summary>
    /// Prints this instance.
    /// </summary>
    public static void Print(this PrintDocument document)
    {
        var printDialog = new System.Windows.Forms.PrintDialog // Gtk4: disambiguate from Gtk.PrintDialog
        {
            Document = document,
            AllowPrintToFile = false,
            PrintToFile = false
        };
        printDialog.Document.DocumentName = document.DocumentName;
        // Print() used to show the print dialog. In WinForms it prints straight to the configured
        // printer; showing a dialog is the caller's choice via PrintDialog.
        printDialog.RunPrint(null, false);
    }

}
