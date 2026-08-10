using Gtk;

namespace System.Windows.Forms
{

    public static class Clipboard
    {

        public static void Clear() => SetDataObject(new DataObject());

        public static object? GetData(string format) => string.IsNullOrWhiteSpace(format) ? null : GetData(format, autoConvert: false);

        public static IDataObject? GetDataObject()
        {
            return new DataObject();
        }

        public static Drawing.Image? GetImage() => GetData(DataFormats.Bitmap, autoConvert: true) as Drawing.Image;

        public static string GetText() => GetText(TextDataFormat.UnicodeText);

        public static string GetText(TextDataFormat format)
        {
            return GetData(format.ToString()) as string ?? string.Empty;
        }

        public static void SetData(string format, object data)
        {
            SetDataObject(new DataObject(format, data), copy: true);
        }

        public static void SetDataObject(object data) => SetDataObject(data, copy: false);

        public static void SetDataObject(object data, bool copy) => SetDataObject(data, copy, retryTimes: 10, retryDelay: 100);

        public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
        {
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Serializable, true, data);
        }

        public static void SetImage(Drawing.Image image) => SetDataObject(new DataObject(image), copy: true);

        public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

        public static void SetText(string text, TextDataFormat format)
        {
            // TODO(gtk4): write to the async Gdk.Clipboard (Display.Clipboard.SetText) — deferred.
        }

        private static object? GetData(string format, bool autoConvert)
            => GetDataObject() is { } dataObject ? dataObject.GetData(format, autoConvert) : null;

        public class ClipDataObject
        {
            public ClipDataObject(object obj)
            {
                Data = obj;
            }

            public object Data
            {
                get;
                set;
            }
        }

        public partial class DataObject : IDataObject
        {
            // TODO(gtk4): system clipboard is now the async Gdk.Clipboard; this is stubbed inert.

            public DataObject()
            {

            }

            public DataObject(object data) : this(DataFormats.Serializable, data)
            {

            }

            public DataObject(string format, object data) : this() => SetData(format, data);

            internal DataObject(string format, bool autoConvert, object data) : this() => SetData(format, autoConvert, data);

            public object? GetData(string format, bool autoConvert)
            {
                if (format == DataFormats.Text || format == DataFormats.UnicodeText)
                {
                    return string.Empty; // TODO(gtk4): async Gdk.Clipboard read
                }
                else if (format == DataFormats.Bitmap)
                {
                    return null; // TODO(gtk4): async Gdk.Clipboard read
                }
                else
                {
                    return null; // TODO(gtk4): async Gdk.Clipboard read
                }
            }

            public object? GetData(string format)
            {
                return GetData(format, true);
            }

            public object? GetData(Type format)
            {
                return GetData(format.Name, true);
            }

            /// <summary>
            /// Indicates whether the clipboard holds data in the specified format.
            /// </summary>
            /// <param name="format">The format to look for.</param>
            /// <param name="autoConvert">Whether to consider formats the data can be converted to.</param>
            /// <remarks>
            /// These five members all threw, so the guard every well-written paste handler calls before
            /// GetData failed while the unguarded call worked. They now answer from the GTK clipboard's
            /// advertised targets.
            /// </remarks>
            public bool GetDataPresent(string format, bool autoConvert)
            {
                if (string.IsNullOrEmpty(format))
                {
                    return false;
                }

                foreach (var available in GetFormats(autoConvert))
                {
                    if (string.Equals(available, format, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Indicates whether the clipboard holds data in the specified format.
            /// </summary>
            /// <param name="format">The format to look for.</param>
            public bool GetDataPresent(string format)
            {
                return GetDataPresent(format, true);
            }

            /// <summary>
            /// Indicates whether the clipboard holds data of the specified type.
            /// </summary>
            /// <param name="format">The type to look for.</param>
            public bool GetDataPresent(Type format)
            {
                return format != null && GetDataPresent(format.FullName ?? format.Name, true);
            }

            /// <summary>
            /// Returns the formats the clipboard data is available in.
            /// </summary>
            /// <param name="autoConvert">Whether to include formats the data can be converted to.</param>
            public string[] GetFormats(bool autoConvert)
            {
                var formats = new List<string>();

                if (false) // TODO(gtk4): query Gdk.Clipboard formats
                {
                    formats.Add(DataFormats.Text);
                    formats.Add(DataFormats.UnicodeText);
                    if (autoConvert)
                    {
                        formats.Add(DataFormats.StringFormat);
                    }
                }

                if (false) // TODO(gtk4): query Gdk.Clipboard formats
                {
                    formats.Add(DataFormats.Bitmap);
                }

                // Only the formats GetData can actually return are reported; the GtkSharp WaitForTargets
                // binding cannot be called from managed code, so the advertised target list is unavailable.
                if (formats.Count > 0 && autoConvert && !formats.Contains(DataFormats.Serializable))
                {
                    formats.Add(DataFormats.Serializable);
                }

                return formats.ToArray();
            }

            /// <summary>
            /// Returns the formats the clipboard data is available in, including convertible ones.
            /// </summary>
            public string[] GetFormats()
            {
                return GetFormats(true);
            }

            public void SetData(string format, bool autoConvert, object data)
            {
                if (format == DataFormats.Text || format == DataFormats.UnicodeText)
                {
                    /* TODO(gtk4): Gdk.Clipboard.SetText */
                }
                else if (format == DataFormats.Bitmap)
                {
                    /* TODO(gtk4): Gdk.Clipboard set image */
                }
                else
                {
                    /* TODO(gtk4): Gdk.Clipboard.SetContent with a ContentProvider — deferred */
                }
            }

            public void SetData(string format, object data)
            {
                SetData(format, true, data);
            }

            public void SetData(Type format, object data)
            {
                SetData(format.Name, true, data);
            }

            public void SetData(object data)
            {
                SetData(DataFormats.Serializable, true, data);
            }
        }
    }
}