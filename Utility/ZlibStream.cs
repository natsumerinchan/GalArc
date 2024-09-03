using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
namespace Utility
{
    public class ZlibStream : DeflateStream
    {
        public ZlibStream(Stream innerStream) : base(innerStream, CompressionMode.Compress, true)
        {
            byte[] header = { 0x78, 0x9C };
            BaseStream.Write(header, 0, 2);
        }
        public static byte[] ZlibDecompress(byte[] input)
        {
            byte magic1, magic2;
            MemoryStream ms = new(input);
            ms.SetLength(ms.Length - 4);//remove adler32
            BinaryReader br = new(ms);
            magic1 = br.ReadByte();
            magic2 = br.ReadByte();
            MemoryStream output = new();
            if (magic1 != 0x78 || (magic2 != 0x01 && magic2 != 0x9c && magic2 != 0xda))//7801:no/low compression, 789c:default compression, 78da:best compression
            {
                //.net compressed
                ms.Position -= 2;
                using (DeflateStream decompressed = new(ms, CompressionMode.Decompress, true))
                {
                    decompressed.CopyTo(output);
                }
                return output.ToArray();
            }
            else
            {
                //normal zlib
                using (DeflateStream decompressed = new(ms, CompressionMode.Decompress, true))
                {
                    decompressed.CopyTo(output);
                }
                return output.ToArray();
            }

        }

        public static byte[] ZlibCompressPartial(byte[] input, CompressionLevel level)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream dstream = new DeflateStream(output, level))
                {
                    dstream.Write(input, 0, input.Length);
                }
                return output.ToArray();
            }
        }

        public static byte[] ZlibCompress(byte[] input, CompressionLevel level)
        {
            byte[] before = new byte[2];
            before[0] = 0x78;
            byte[] after = new byte[4];
            if (level == CompressionLevel.NoCompression || level == CompressionLevel.Fastest)
            {
                before[1] = 0x01;
            }
            else if (level == CompressionLevel.Optimal)
            {
                before[1] = 0x9c;
            }
            else if (level == CompressionLevel.SmallestSize)
            {
                before[1] = 0xda;
            }
            using (MemoryStream originalFileStream = new(input))
            {
                using (MemoryStream compressedFileStream = new())
                {
                    using (DeflateStream compressionStream = new(compressedFileStream, CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressionStream);
                    }
                    byte[] compressedFile = compressedFileStream.ToArray();
                    byte[] chksm = BitConverter.GetBytes(Adler32.Calculate(before.Concat(compressedFile).ToArray())).ToArray();
                    return before.Concat(compressedFile).Concat(chksm).ToArray();
                }
            }
        }

        public static byte[] ZlibCompress_SharpZipLib(byte[] input)
        {
            MemoryStream compressed = new();
            DeflaterOutputStream outputStream = new(compressed);
            outputStream.Write(input, 0, input.Length);
            outputStream.Close();
            return compressed.ToArray();
        }

        /*
         * It is said that the zlib compressed or decompressed by .net is different from the normal zlib.
         * .net version lacks the zlib header(2 bytes) and trailer(adler32/checksum,4 bytes).
         * https://blog.csdn.net/compasslg/article/details/111995866
         */

        /*
         * Try using ZlibCompress_SharpZipLib when error occurs.
         */

    }
}
