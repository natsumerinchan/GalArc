﻿// File: Utility/BigEndian.cs
// Date: 2024/08/28
// Description: BigEndian conversion functions. Supports 16, 32 and 64 bit integers.
//
// Copyright (C) 2014 by morkt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//

using System;

namespace Utility
{
    public static class BigEndian
    {
        public static uint Convert(uint u)
        {
            return u << 24 | (u & 0xff00) << 8 | (u & 0xff0000) >> 8 | u >> 24;
        }
        public static int Convert(int i)
        {
            return (int)Convert((uint)i);
        }
        public static ushort Convert(ushort u)
        {
            return (ushort)(u << 8 | u >> 8);
        }
        public static short Convert(short i)
        {
            return (short)Convert((ushort)i);
        }
        public static ulong Convert(ulong u)
        {
            return (ulong)Convert((uint)(u & 0xffffffff)) << 32
                 | (ulong)Convert((uint)(u >> 32));
        }
        public static long Convert(long i)
        {
            return (long)Convert((ulong)i);
        }
    }
}
