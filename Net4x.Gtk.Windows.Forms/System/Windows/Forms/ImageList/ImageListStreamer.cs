// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using static GtkSystem.Resources.ResourceManager;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides the data portion of an
    /// <see cref="System.Windows.Forms.ImageList" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [Serializable] // This type is participating in resx serialization scenarios.
    public sealed class ImageListStreamer : ISerializable, IDisposable
    {

        private static readonly byte[] HEADER_MAGIC = new byte[] { 0x4D, 0x53, 0x46, 0X74 };
        public ResourceInfo ResourceInfo { get; set; }
        // Compressed magic header. If we see this, the image stream is compressed.
        //private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;
        private static readonly object s_syncObject = new object();


        private readonly ImageList? _imageList;
        private ImageList.NativeImageList? _nativeImageList;

        internal ImageListStreamer(ImageList imageList) => _imageList = imageList;

        // Used by binary serialization
        private ImageListStreamer(SerializationInfo info, StreamingContext context)
        {
            if (info.GetValue("Data", typeof(byte[])) is byte[] data)
            {
                Deserialize(data);
            }
        }
        internal ImageListStreamer(Stream stream)
        {
            if (stream is MemoryStream ms
                && ms.TryGetBuffer(out ArraySegment<byte> buffer)
                && buffer.Offset == 0)
            {
                Deserialize(buffer.Array);
            }
            else
            {
                stream.Position = 0;
                using MemoryStream copyStream = new MemoryStream(checked((int)stream.Length));
                stream.CopyTo(copyStream);
                Deserialize(copyStream.GetBuffer());
            }
        }
        internal ImageListStreamer(byte[] data) => Deserialize(data);

        /// <summary>
        /// Compresses the given input, returning a new array that represents the compressed data.
        /// </summary>
        private static byte[] Compress(ReadOnlySpan<byte> input)
        {
            return input.ToArray();
        }

        /// <summary>
        /// Decompresses the given input, returning a new array that represents the uncompressed data.
        /// </summary>
        private static byte[] Decompress1(byte[] input)
        {
            return input;

        }
        private static byte[] Decompress(byte[] input)
        {
            int finalLength = 0;
            int idx = 0;
            int outputIdx = 0;

            // Check for our header. If we don't have one,
            // we're not actually decompressed, so just return
            // the original.
            //
            if (input.Length < HEADER_MAGIC.Length)
            {
                return input;
            }

            for (idx = 0; idx < HEADER_MAGIC.Length; idx++)
            {
                if (input[idx] != HEADER_MAGIC[idx])
                {
                    return input;
                }
            }

            // Ok, we passed the magic header test.

            for (idx = HEADER_MAGIC.Length; idx < input.Length; idx += 2)
            {
                finalLength += input[idx];
            }

            byte[] output = new byte[finalLength];

            idx = HEADER_MAGIC.Length;

            while (idx < input.Length)
            {
                byte runLength = input[idx++];
                byte current = input[idx++];

                int startIdx = outputIdx;
                int endIdx = outputIdx + runLength;

                while (startIdx < endIdx)
                {
                    output[startIdx++] = current;
                }

                outputIdx += runLength;
            }

            return output;
        }

        private void Deserialize(byte[] data)
        {

        }
        /// <summary>
        /// Populates a
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="si">The
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">The
        /// <see cref="System.Runtime.Serialization.StreamingContext" /> that is the destination for this serialization.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> cannot be populated with data because no data exists, or it is not in the correct format.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public void GetObjectData(SerializationInfo si, StreamingContext context) =>
                    si.AddValue("Data", Serialize());

        internal byte[] Serialize()
        {
            using MemoryStream stream = new MemoryStream();

            ReadOnlySpan<byte> buffer = stream.GetBuffer().AsSpan(0, (int)stream.Length);
            return Compress(buffer);
        }

        internal void GetObjectData(Stream stream)
        {
        }

        internal ImageList.NativeImageList? GetNativeImageList() => _nativeImageList;

        private bool WriteImageList(Stream stream)
        {
            return true;


            //try
            //{
            //    return PInvoke.ImageList.WriteEx(
            //        handle,
            //        IMAGE_LIST_WRITE_STREAM_FLAGS.ILP_DOWNLEVEL,
            //        stream).Succeeded;
            //}
            //catch (EntryPointNotFoundException)
            //{
            //    // Not running on ComCtl32 v6, fall back to the old API.
            //}

            //return PInvoke.ImageList.Write(handle, stream);
        }
        /// <summary>
        /// Releases all resources used by the current instance of the
        /// <see cref="System.Windows.Forms.ImageListStreamer" /> class.
        /// </summary>
        public void Dispose()
        {
            _nativeImageList?.Dispose();
            _nativeImageList = null;
        }
    }
}
