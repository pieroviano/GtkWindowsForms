// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using static GtkSystem.Resources.ResourceManager;

namespace System.Windows.Forms;

/// <summary>
/// Provides the data portion of an
/// <see cref="T:System.Windows.Forms.ImageList" />
/// </summary>
[Serializable] // This type is participating in resx serialization scenarios.
public sealed class ImageListStreamer : ISerializable, IDisposable
{
    /// <summary>
    /// The image list
    /// </summary>
    private readonly ImageList? _imageList;

    /// <summary>
    /// The native image list
    /// </summary>
    private ImageList.NativeImageList? _nativeImageList;

    /// <summary>
    /// The header magic
    /// </summary>
    private static readonly byte[] HEADER_MAGIC = new byte[] { 0x4D, 0x53, 0x46, 0X74 };

    /// <summary>
    /// The s synchronize object
    /// </summary>
    // Compressed magic header. If we see this, the image stream is compressed.
    //private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;
    private static readonly object s_syncObject = new object();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ImageListStreamer" /> class.
    /// </summary>
    /// <param name="imageList">The image list.
    /// </param>
    internal ImageListStreamer(ImageList imageList) => _imageList = imageList;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ImageListStreamer" /> class.
    /// </summary>
    /// <param name="stream">The stream.
    /// </param>
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

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ImageListStreamer" /> class.
    /// </summary>
    /// <param name="data">The data.
    /// </param>
    internal ImageListStreamer(byte[] data) => Deserialize(data);

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ImageListStreamer" /> class.
    /// </summary>
    /// <param name="info">The information.
    /// </param>
    /// <param name="context">The context.
    /// </param>
    // Used by binary serialization
    private ImageListStreamer(SerializationInfo info, StreamingContext context)
    {
        if (info.GetValue("Data", typeof(byte[])) is byte[] data)
        {
            Deserialize(data);
        }
    }

    /// <summary>
    /// Gets or sets the resource information.
    /// </summary>
    /// <value>The resource information.
    /// </value>
    public ResourceInfo ResourceInfo { get; set; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _nativeImageList?.Dispose();
        _nativeImageList = null;
    }

    /// <summary>
    /// Gets the object data.
    /// </summary>
    /// <param name="si">The si.
    /// </param>
    /// <param name="context">The context.
    /// </param>
    public void GetObjectData(SerializationInfo si, StreamingContext context) =>
                si.AddValue("Data", Serialize());

    /// <summary>
    /// Gets the native image list.
    /// </summary>
    /// <returns>System.Nullable{ImageList.NativeImageList}.
    /// </returns>
    internal ImageList.NativeImageList? GetNativeImageList() => _nativeImageList;

    /// <summary>
    /// Gets the object data.
    /// </summary>
    /// <param name="stream">The stream.
    /// </param>
    internal void GetObjectData(Stream stream)
    {
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns>System.Byte[].
    /// </returns>
    internal byte[] Serialize()
    {
        using MemoryStream stream = new MemoryStream();

        ReadOnlySpan<byte> buffer = stream.GetBuffer().AsSpan(0, (int)stream.Length);

        return Compress(buffer);
    }

    /// <summary>
    /// Compresses the given input, returning a new array that represents the compressed data.
    /// </summary>
    /// <param name="input">The input.
    /// </param>
    /// <returns>System.Byte[].
    /// </returns>
    private static byte[] Compress(ReadOnlySpan<byte> input)
    {
        return input.ToArray();
    }

    /// <summary>
    /// Decompresses the specified input.
    /// </summary>
    /// <param name="input">The input.
    /// </param>
    /// <returns>System.Byte[].
    /// </returns>
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

    /// <summary>
    /// Decompresses the given input, returning a new array that represents the uncompressed data.
    /// </summary>
    /// <param name="input">The input.
    /// </param>
    /// <returns>System.Byte[].
    /// </returns>
    private static byte[] Decompress1(byte[] input)
    {
        return input;

    }

    /// <summary>
    /// Deserializes the specified data.
    /// </summary>
    /// <param name="data">The data.
    /// </param>
    private void Deserialize(byte[] data)
    {

    }

    /// <summary>
    /// Writes the image list.
    /// </summary>
    /// <param name="stream">The stream.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>false
    /// </c> otherwise.
    /// </returns>
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

}