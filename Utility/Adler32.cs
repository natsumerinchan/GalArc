using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    class Adler32
    {
        //private const uint BASE = 65521;
        //private const int NMAX = 5552;

        //public static uint Calculate(byte[] data)
        //{
        //    uint s1 = 1;
        //    uint s2 = 0;
        //    int length = data.Length;
        //    int index = 0;

        //    while (length > 0)
        //    {
        //        int n = length < NMAX ? length : NMAX;
        //        length -= n;
        //        while (--n >= 0)
        //        {
        //            s1 += data[index++];
        //            s2 += s1;
        //        }
        //        s1 %= BASE;
        //        s2 %= BASE;
        //    }
        //    return (s2 << 16) | s1;
        //}

        private const uint Adler32Modulus = 65521;
        public static uint Calculate(byte[] data)
        {
            uint a = 1, b = 0;

            foreach (byte octet in data)
            {
                a = (a + octet) % Adler32Modulus;
                b = (b + a) % Adler32Modulus;
            }

            return (b << 16) | a;
        }
    }
}
