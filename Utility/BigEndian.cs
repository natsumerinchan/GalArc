using System;
using System.Linq;

namespace Utility
{
    class BigEndian
    {
        public static uint BEBytesToUint(byte[] buffer)
        {
            return BitConverter.ToUInt32(buffer.Reverse().ToArray(), 0);
        }

        public static byte[] IntToBEBytes(int value)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Reverse(result);
            return result;
        }
    }
}
