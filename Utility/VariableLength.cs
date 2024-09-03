using System.Text;

namespace Utility
{
    class VariableLength
    {
        public static uint UintUnpack(BinaryReader br)
        {
            uint value = 0;
            while ((value & 1) == 0)
            {
                value = value << 7 | br.ReadByte();
            }
            return value >> 1;
        }
        public static string StringUnpack(BinaryReader br, uint length)
        {
            var bytes = new byte[length];
            for (uint i = 0; i < length; ++i)
            {
                bytes[i] = (byte)UintUnpack(br);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(932).GetString(bytes);
        }

        public static byte[] UintPack(uint a)
        {
            List<byte> result = new();
            uint v = a;

            if (v == 0)
            {
                result.Add(0x01);
                return result.ToArray();
            }

            v = (v << 1) + 1;
            byte curByte = (byte)(v & 0xFF);
            while ((v & 0xFFFFFFFFFFFFFFFE) != 0)
            {
                result.Add(curByte);
                v >>= 7;
                curByte = (byte)(v & 0xFE);
            }

            result.Reverse();
            return result.ToArray();
        }
        public static byte[] StringPack(string s)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] bytes = Encoding.GetEncoding(932).GetBytes(s);
            List<byte> rst = new();
            foreach (byte b in bytes)
            {
                rst.AddRange(UintPack(b));
            }
            return rst.ToArray();
        }
    }
}
