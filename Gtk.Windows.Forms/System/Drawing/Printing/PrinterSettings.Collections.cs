// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;

namespace System.Drawing.Printing;

/// <summary>
/// The nested capability-collection types (<see cref="PaperSizes" />, <see cref="PaperSources" />,
/// <see cref="PrinterResolutions" /> and installed printers) that a printer exposes. They were absent, so
/// code enumerating a printer's capabilities did not compile. With no print system behind the shim the
/// collections are empty, consistent with the virtual-device defaults documented in EMULATION-GAPS.md.
/// </summary>
public partial class PrinterSettings
{
    private PaperSizeCollection? _paperSizes;
    private PaperSourceCollection? _paperSources;
    private PrinterResolutionCollection? _printerResolutions;

    /// <summary>Gets the paper sizes supported by this printer.</summary>
    public PaperSizeCollection PaperSizes => _paperSizes ??= new PaperSizeCollection(Array.Empty<PaperSize>());

    /// <summary>Gets the paper source trays supported by this printer.</summary>
    public PaperSourceCollection PaperSources => _paperSources ??= new PaperSourceCollection(Array.Empty<PaperSource>());

    /// <summary>Gets the resolutions supported by this printer.</summary>
    public PrinterResolutionCollection PrinterResolutions =>
        _printerResolutions ??= new PrinterResolutionCollection(Array.Empty<PrinterResolution>());

    /// <summary>A collection of <see cref="PaperSize" /> values.</summary>
    public class PaperSizeCollection : ICollection, IEnumerable
    {
        private readonly List<PaperSize> _list;

        /// <summary>Initializes the collection with the given array.</summary>
        /// <param name="array">The paper sizes.</param>
        public PaperSizeCollection(PaperSize[] array) => _list = new List<PaperSize>(array ?? Array.Empty<PaperSize>());

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => _list.Count;

        /// <summary>Gets the item at the specified index.</summary>
        /// <param name="index">The zero-based index.</param>
        public virtual PaperSize this[int index] => _list[index];

        /// <summary>Adds an item to the collection and returns its index.</summary>
        /// <param name="paperSize">The item to add.</param>
        public int Add(PaperSize paperSize)
        {
            _list.Add(paperSize);
            return _list.Count - 1;
        }

        /// <summary>Copies the collection into the given array starting at the given index.</summary>
        /// <param name="paperSizes">The destination array.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(PaperSize[] paperSizes, int index) => _list.CopyTo(paperSizes, index);

        /// <summary>Returns an enumerator over the collection.</summary>
        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
    }

    /// <summary>A collection of <see cref="PaperSource" /> values.</summary>
    public class PaperSourceCollection : ICollection, IEnumerable
    {
        private readonly List<PaperSource> _list;

        /// <summary>Initializes the collection with the given array.</summary>
        /// <param name="array">The paper sources.</param>
        public PaperSourceCollection(PaperSource[] array) => _list = new List<PaperSource>(array ?? Array.Empty<PaperSource>());

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => _list.Count;

        /// <summary>Gets the item at the specified index.</summary>
        /// <param name="index">The zero-based index.</param>
        public virtual PaperSource this[int index] => _list[index];

        /// <summary>Adds an item to the collection and returns its index.</summary>
        /// <param name="paperSource">The item to add.</param>
        public int Add(PaperSource paperSource)
        {
            _list.Add(paperSource);
            return _list.Count - 1;
        }

        /// <summary>Copies the collection into the given array starting at the given index.</summary>
        /// <param name="paperSources">The destination array.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(PaperSource[] paperSources, int index) => _list.CopyTo(paperSources, index);

        /// <summary>Returns an enumerator over the collection.</summary>
        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
    }

    /// <summary>A collection of <see cref="PrinterResolution" /> values.</summary>
    public class PrinterResolutionCollection : ICollection, IEnumerable
    {
        private readonly List<PrinterResolution> _list;

        /// <summary>Initializes the collection with the given array.</summary>
        /// <param name="array">The resolutions.</param>
        public PrinterResolutionCollection(PrinterResolution[] array) =>
            _list = new List<PrinterResolution>(array ?? Array.Empty<PrinterResolution>());

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => _list.Count;

        /// <summary>Gets the item at the specified index.</summary>
        /// <param name="index">The zero-based index.</param>
        public virtual PrinterResolution this[int index] => _list[index];

        /// <summary>Adds an item to the collection and returns its index.</summary>
        /// <param name="printerResolution">The item to add.</param>
        public int Add(PrinterResolution printerResolution)
        {
            _list.Add(printerResolution);
            return _list.Count - 1;
        }

        /// <summary>Copies the collection into the given array starting at the given index.</summary>
        /// <param name="printerResolutions">The destination array.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(PrinterResolution[] printerResolutions, int index) => _list.CopyTo(printerResolutions, index);

        /// <summary>Returns an enumerator over the collection.</summary>
        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
    }

    /// <summary>A collection of printer-name strings.</summary>
    public class StringCollection : ICollection, IEnumerable
    {
        private readonly List<string> _list;

        /// <summary>Initializes the collection with the given array.</summary>
        /// <param name="array">The strings.</param>
        public StringCollection(string[] array) => _list = new List<string>(array ?? Array.Empty<string>());

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => _list.Count;

        /// <summary>Gets the item at the specified index.</summary>
        /// <param name="index">The zero-based index.</param>
        public virtual string this[int index] => _list[index];

        /// <summary>Adds an item to the collection and returns its index.</summary>
        /// <param name="value">The item to add.</param>
        public int Add(string value)
        {
            _list.Add(value);
            return _list.Count - 1;
        }

        /// <summary>Copies the collection into the given array starting at the given index.</summary>
        /// <param name="strings">The destination array.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(string[] strings, int index) => _list.CopyTo(strings, index);

        /// <summary>Returns an enumerator over the collection.</summary>
        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
    }
}
