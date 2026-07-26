// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static GtkSystem.Resources.ResourceManager;
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Runtime.Serialization;

namespace System.Windows.Forms;

/// <summary>
/// Provides the data portion of an
/// <see cref="T:System.Windows.Forms.ImageList" />
/// </summary>
[Serializable] // This type is participating in resx serialization scenarios.
public sealed class ImageListStreamer : ISerializable, IDisposable
{

    private readonly ImageList? _imageList;

    private ImageList.NativeImageList? _nativeImageList;

    private static readonly byte[] HEADER_MAGIC = new byte[] { 0x4D, 0x53, 0x46, 0X74 };

    // Compressed magic header. If we see this, the image stream is compressed.
    //private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;
    private static readonly object s_syncObject = new();

    internal ImageListStreamer(ImageList imageList) => _imageList = imageList;

    internal ImageListStreamer(Stream stream)
    {
        if (stream is MemoryStream ms
            && ms.TryGetBuffer(out var buffer)
            && buffer.Offset == 0)
        {
            Deserialize(buffer.Array);
        }
        else
        {
            stream.Position = 0;
            using var copyStream = new MemoryStream(checked((int)stream.Length));
            stream.CopyTo(copyStream);
            Deserialize(copyStream.GetBuffer());
        }
    }

    internal ImageListStreamer(byte[] data) => Deserialize(data);

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
    /// <value>
    /// The resource information.
    /// </value>
    public ResourceInfo? ResourceInfo
    {
        get;
        set;
    }

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
    /// <param name="si">
    /// The si.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetObjectData(SerializationInfo si, StreamingContext context) =>
                si.AddValue("Data", Serialize());

    internal ImageList.NativeImageList? GetNativeImageList() => _nativeImageList;

    internal void GetObjectData(Stream stream)
    {
    }

    internal byte[] Serialize()
    {
        using var stream = new MemoryStream();

        ReadOnlySpan<byte> buffer = stream.GetBuffer().AsSpan(0, (int)stream.Length);

        return Compress(buffer);
    }

    /// <summary>
    /// Compresses the given input, returning a new array that represents the compressed data.
    /// </summary>
    private static byte[] Compress(ReadOnlySpan<byte> input)
    {
        return input.ToArray();
    }

    private static byte[] Decompress(byte[] input)
    {
        var finalLength = 0;
        var idx = 0;
        var outputIdx = 0;

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

        var output = new byte[finalLength];

        idx = HEADER_MAGIC.Length;

        while (idx < input.Length)
        {
            var runLength = input[idx++];
            var current = input[idx++];

            var startIdx = outputIdx;
            var endIdx = outputIdx + runLength;

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
    private static byte[] Decompress1(byte[] input)
    {
        return input;

    }

    private void Deserialize(byte[] data)
    {

    }

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