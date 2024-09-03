using System;
using System.Linq;
namespace Utility
{
    class LzssCompression
    {
        public static MemoryStream LzssDecompress(MemoryStream ms)
        {
            const int N = 4096;
            const int F = 18;
            const int THRESHOLD = 2;
            int[] lzBuffer = new int[N + F - 1];
            int r = N - F;
            int flags = 0;

            MemoryStream output = new MemoryStream();
            int bufferIndex = 0;

            while (true)
            {
                if (((flags >>= 1) & 256) == 0)
                {
                    if (bufferIndex >= ms.Length)
                        break;

                    flags = ms.ReadByte() | 0xff00;
                    bufferIndex++;
                }

                if ((flags & 1) != 0)
                {
                    if (bufferIndex >= ms.Length)
                        break;

                    int c = ms.ReadByte();
                    bufferIndex++;
                    output.WriteByte((byte)c);
                    lzBuffer[r++] = c;
                    r &= (N - 1);
                }
                else
                {
                    if (bufferIndex >= ms.Length)
                        break;

                    int i = ms.ReadByte();
                    bufferIndex++;
                    if (bufferIndex >= ms.Length)
                        break;

                    int j = ms.ReadByte();
                    bufferIndex++;
                    i |= ((j & 0xf0) << 4);
                    j = (j & 0x0f) + THRESHOLD;

                    for (int k = 0; k <= j; k++)
                    {
                        int c = lzBuffer[(i + k) & (N - 1)];
                        output.WriteByte((byte)c);
                        lzBuffer[r++] = c;
                        r &= (N - 1);
                    }
                }
            }
            return output;
        }
        //public static MemoryStream LzssCompress(MemoryStream ms)
        //{
        //    const int N = 4096;
        //    const int F = 18;
        //    const int THRESHOLD = 2;

        //    byte[] inputBytes = ms.ToArray();
        //    MemoryStream output = new MemoryStream();
        //    int[] lzBuffer = new int[N + F - 1];
        //    int r = N - F;
        //    int flagBits = 0;

        //    for (int i = 0; i < inputBytes.Length; i++)
        //    {
        //        int matchLength = 0;
        //        int matchPosition = 0;
        //        for (int j = Math.Max(0, i - N); j < i; j++)
        //        {
        //            int k;
        //            for (k = 0; k < F && i + k < inputBytes.Length && inputBytes[j + k] == inputBytes[i + k]; k++) ;
        //            if (k > matchLength)
        //            {
        //                matchLength = k;
        //                matchPosition = i - j;
        //            }
        //        }

        //        if (matchLength <= THRESHOLD)
        //        {
        //            // Output a literal byte
        //            flagBits >>= 1;
        //            flagBits |= 0x100;
        //            output.WriteByte(inputBytes[i]);
        //            lzBuffer[r++] = inputBytes[i];
        //            r &= (N - 1);
        //        }
        //        else
        //        {
        //            // Output a matched string
        //            flagBits >>= 1;
        //            int temp = ((matchPosition - 1) & 0xf00) | (matchLength - (THRESHOLD + 1));
        //            output.WriteByte((byte)temp);
        //            output.WriteByte((byte)((matchPosition - 1) & 0xff));
        //            for (int j = 0; j < matchLength; j++)
        //            {
        //                lzBuffer[r++] = inputBytes[i + j];
        //                r &= (N - 1);
        //            }
        //            i += matchLength - 1;
        //        }

        //        if ((flagBits & 0xff) == 0)
        //        {
        //            output.WriteByte((byte)flagBits);
        //            flagBits = 0x100;
        //        }
        //    }

        //    if ((flagBits & 0xff) != 0x100)
        //    {
        //        output.WriteByte((byte)(flagBits >> 1));
        //    }

        //    return output;
        //}
        //public static MemoryStream LzssCompress(MemoryStream ms)
        //{
        //    MemoryStream outStream = new MemoryStream(); // 创建内存流，用于存储压缩后的数据
        //    int plen = ms.ToArray().Length & (~7); // 取 ori 数组长度与 7 的按位非结果，以获得最接近 ori 长度的 8 的倍数
        //    for (int i = 0; i < plen; i += 8)
        //    {
        //        outStream.WriteByte(0xff);
        //        outStream.Write(ms.ToArray(), i, 8);
        //    }
        //    int llen = ms.ToArray().Length - plen;
        //    if (llen > 0)
        //    {
        //        outStream.WriteByte((byte)((1 << llen) - 1));
        //        outStream.Write(ms.ToArray(), plen, llen);
        //    }
        //    return outStream;
        //}
        //fail
    }
}
