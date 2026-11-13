using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Gtk.IO;

namespace System.Resources.Extensions;

/// <summary>
/// DeserializingResourceReader is a class that provides an implementation of deserializing a resource file. It reads resources from a stream and allows access to them through the IResourceReader interface. The class supports both binary formatted resources and resources that use type converters for serialization. It also includes error handling to ensure that the resource file is properly formatted and that the resources can be deserialized correctly.
/// </summary>
/// <seealso cref="T:System.Resources.IResourceReader" />
/// <seealso cref="T:System.Collections.IEnumerable" />
/// <seealso cref="T:System.IDisposable" />
public class DeserializingResourceReader : Gtk.Resources.Extensions.DeserializingResourceReader
{
    public DeserializingResourceReader(string fileName) : base(fileName)
    {
    }

    public DeserializingResourceReader(Stream stream) : base(stream)
    {
    }
}