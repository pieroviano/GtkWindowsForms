// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Drawing.Printing
{
    /// <summary>
    /// PaperSize specifies the size of the paper to use when printing. It is used by the <see cref="PageSettings"/> class to specify the size of the paper to use when printing a document.
    /// </summary>
    public partial class PaperSize
    {
        private readonly bool _createdByDefaultConstructor;

        private int _height;

        private PaperKind _kind;

        private string _name;

        // Standard hundredths of an inch units

        private int _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaperSize"/> class.
        /// </summary>
        public PaperSize()
        {
            _kind = PaperKind.Custom;
            _name = string.Empty;
            _createdByDefaultConstructor = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaperSize"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PaperSize(string name, int width, int height)
        {
            _kind = PaperKind.Custom;
            _name = name;
            _width = width;
            _height = height;
        }

        internal PaperSize(PaperKind kind, string name, int width, int height)
        {
            _kind = kind;
            _name = name;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        /// <exception cref="ArgumentException">PSizeNotCustom - value</exception>
        public int Height
        {
            get => _height;
            set
            {
                if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
                {
                    throw new ArgumentException("PSizeNotCustom", nameof(value));
                }

                _height = value;
            }
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public PaperKind Kind => PaperKind.Custom;

        /// <summary>
        /// Gets or sets the name of the paper.
        /// </summary>
        /// <value>
        /// The name of the paper.
        /// </value>
        /// <exception cref="ArgumentException">PSizeNotCustom - value</exception>
        public string PaperName
        {
            get => _name;
            set
            {
                if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
                {
                    throw new ArgumentException("PSizeNotCustom", nameof(value));
                }

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the kind of the raw.
        /// </summary>
        /// <value>
        /// The kind of the raw.
        /// </value>
        public int RawKind
        {
            get => (int)_kind;
            set => _kind = (PaperKind)value;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        /// <exception cref="ArgumentException">PSizeNotCustom - value</exception>
        public int Width
        {
            get => _width;
            set
            {
                if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
                {
                    throw new ArgumentException("PSizeNotCustom", nameof(value));
                }

                _width = value;
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"[PaperSize {PaperName} Kind={Kind} Height={Height.ToString(CultureInfo.InvariantCulture)} Width={Width.ToString(CultureInfo.InvariantCulture)}]";

    }
}
