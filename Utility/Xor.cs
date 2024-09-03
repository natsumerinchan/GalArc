namespace Utility
{
    class Xor
    {
        public static byte[] BytesXorBytes(byte[] data, byte[] key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
            return result;
        }
        public static byte[] BytesXorByte(byte[] data, byte key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key);
            }
            return result;
        }
        public static byte ByteXorByte(byte data, byte key)
        {
            return (byte)(data ^ key);
        }
    }
}
