// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Resources
{
    /// <summary>
    /// Gathers all items that represent an XML resource (.resx) file into a single object.
    /// </summary>
    public class ResXResourceSet : ResourceSet
    {
        /// <summary>
        /// Initializes a new instance of a
        /// <see cref="System.Resources.ResXResourceSet" /> class using the system default
        /// <see cref="System.Resources.ResXResourceReader" /> that opens and reads resources from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to read resources from.
        /// </param>
        public ResXResourceSet(string fileName) : base(new ResXResourceReader(fileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Resources.ResXResourceSet" /> class using the system default
        /// <see cref="System.Resources.ResXResourceReader" /> to read resources from the specified stream.
        /// </summary>
        /// <param name="stream">The
        /// <see cref="System.IO.Stream" /> of resources to be read. The stream should refer to an existing resource file.
        /// </param>
        public ResXResourceSet(Stream stream) : base(new ResXResourceReader(stream))
        {
        }

        /// <summary>
        /// Returns the preferred resource reader class for this kind of
        /// <see cref="System.Resources.ResXResourceSet" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Type" /> of the preferred resource reader for this kind of
        /// <see cref="System.Resources.ResXResourceSet" />
        /// </returns>
        public override Type GetDefaultReader() => typeof(ResXResourceReader);

        /// <summary>
        /// Returns the preferred resource writer class for this kind of
        /// <see cref="System.Resources.ResXResourceSet" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Type" /> of the preferred resource writer for this kind of
        /// <see cref="System.Resources.ResXResourceSet" />
        /// </returns>
        public override Type GetDefaultWriter() => typeof(ResXResourceWriter);

    }
}
