namespace GtkSystem.IO.Resources;

internal static class BinaryReaderExtensions
{
    public static int Read7BitEncodedInt(this BinaryReader reader)
    {
        var result = 0;
        var bitShift = 0;
        byte currentByte;
        do
        {
            if (bitShift == 35)
            {
                throw new FormatException(SR.Format_Bad7BitInt32);
            }
            currentByte = reader.ReadByte();
            result |= (currentByte & 0x7F) << bitShift;
            bitShift += 7;
        }
        while ((currentByte & 0x80u) != 0);

        return result;
    }
}