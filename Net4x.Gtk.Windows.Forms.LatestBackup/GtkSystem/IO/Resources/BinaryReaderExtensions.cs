namespace GtkSystem.IO;

internal static class BinaryReaderExtensions
{
    public static int Read7BitEncodedInt(this BinaryReader reader)
    {
        var result = 0;
        var bitPosition = 0;
        byte byteRead;
        do
        {
            if (bitPosition == 35)
            {
                throw new FormatException(SR.Format_Bad7BitInt32);
            }
            byteRead = reader.ReadByte();
            result |= (byteRead & 0x7F) << bitPosition;
            bitPosition += 7;
        }
        while ((byteRead & 0x80u) != 0);

        return result;
    }
}