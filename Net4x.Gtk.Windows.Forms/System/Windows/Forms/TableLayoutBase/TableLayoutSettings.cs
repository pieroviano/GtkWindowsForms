using System.Runtime.Serialization;

namespace System.Windows.Forms;

/// <summary>
/// Collects the characteristics associated with table layouts.
/// </summary>
[Serializable]
public sealed class TableLayoutSettings : ISerializable
{
    /// <summary>
    /// Populates a
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">
    /// The
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> to populate with data.
    /// </param>
    /// <param name="context">
    /// The destination (see
    /// <see cref="T:System.Runtime.Serialization.StreamingContext">
    /// </see>) for this serialization.
    /// </param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

}