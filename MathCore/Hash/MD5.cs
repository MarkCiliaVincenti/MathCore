﻿using System;
using System.Diagnostics;
using System.IO;

// ReSharper disable InconsistentNaming

namespace MathCore.Hash;

/// <summary>
/// RFC for MD5 https://tools.ietf.org/html/rfc1321
/// Based on the pseudo code from Wikipedia: https://en.wikipedia.org/wiki/MD5
/// </summary>
public class MD5
{
    public static byte[] Compute(byte[] data)
    {
        uint[] result =
        {
            0x67452301U,
            0xefcdab89U,
            0x98badcfeU,
            0x10325476U,
        };

        ref var a0 = ref result[0];
        ref var b0 = ref result[1];
        ref var c0 = ref result[2];
        ref var d0 = ref result[3];

        var zero_length = (64 - data.Length % 64) - 1 - 8;

        if (zero_length < 0) zero_length += 64;

        const int length_x80 = 1;
        const int length_end = 8;

        var buffer64_length = data.Length + length_x80 + zero_length + length_end;

        var bufer64 = new byte[buffer64_length];

        Array.Copy(data, bufer64, data.Length);
        bufer64[data.Length] = 0x80;

        var length_bytes = BitConverter.GetBytes(data.Length << 3);

        Array.Copy(length_bytes, 0, bufer64, bufer64.Length - 8, 4);

        //Debug.WriteLine(bufer64.ToStringHex(8));

        var buffer16 = new uint[16];
        for (var i = 0; i < bufer64.Length / 64; i++)
        {
            Debug.WriteLine("i = " + i);

            Buffer.BlockCopy(bufer64, i * 64, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);

        return result_bytes;
    }

    public static byte[] Compute(Stream data)
    {
        uint[] result =
        {
            0x67452301,
            0xefcdab89,
            0x98badcfe,
            0x10325476,
        };

        var buffer64 = new byte[64];
        var buffer16 = new uint[16];

        var length = 0UL;
        int readed;
        do
        {
            readed = data.FillBuffer(buffer64);

            length += (ulong)readed;

            if (readed < 64)
            {
                Array.Clear(buffer64, readed + 1, 64 - readed - 8);
                buffer64[readed] = 0x80;

                var full_length = length << 3;

                buffer64[^8] = (byte)((full_length) & 0xff);
                buffer64[^7] = (byte)((full_length >> 8) & 0xff);
                buffer64[^6] = (byte)((full_length >> 16) & 0xff);
                buffer64[^5] = (byte)((full_length >> 24) & 0xff);
                buffer64[^4] = (byte)((full_length >> 32) & 0xff);
                buffer64[^3] = (byte)((full_length >> 40) & 0xff);
                buffer64[^2] = (byte)((full_length >> 48) & 0xff);
                buffer64[^1] = (byte)((full_length >> 56) & 0xff);
            }

            Debug.WriteLine("--------------------------");
            Debug.WriteLine(buffer64.ToStringHex(8));

            Buffer.BlockCopy(buffer64, 0, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }
        while (readed == 64);

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
        return result_bytes;
    }

    private static void Compute0(uint[] buffer16, ref uint a0, ref uint b0, ref uint c0, ref uint d0)
    {
        var (A, B, C, D) = (a0, b0, c0, d0);

        for (uint j = 0; j < 64; j++)
        {
            var (f, g) = j switch
            {
                <= 15           => ((B & C) | (~B & D), j               ),
                >= 16 and <= 31 => ((D & B) | (~D & C), (5 * j + 1) % 16),
                >= 32 and <= 47 => (B ^ C ^ D         , (3 * j + 5) % 16),
                >= 48           => (C ^ (B | ~D)      , 7 * j % 16      )
            };

            (A, B, C, D) = (D, B + LeftRotate(A + f + __K[j] + buffer16[g], __S[j]), B, C);

            static uint LeftRotate(uint x, int c) => (x << c) | (x >> (32 - c));
        }

        a0 += A;
        b0 += B;
        c0 += C;
        d0 += D;
    }

    private static void Compute(uint[] buffer16, ref uint a0, ref uint b0, ref uint c0, ref uint d0)
    {
        var (A, B, C, D) = (a0, b0, c0, d0);

        static uint LeftRotate(uint x, int c) => (x << c) | (x >> (32 - c));
        uint f;
        int  g;
            (f, g) = ((B & C) | (~B & D), 0);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[0] + buffer16[g], __S[0]), B, C);

            (f, g) = ((B & C) | (~B & D), 1);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[1] + buffer16[g], __S[1]), B, C);

            (f, g) = ((B & C) | (~B & D), 2);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[2] + buffer16[g], __S[2]), B, C);

        if (3 <= 15)
            (f, g) = ((B & C) | (~B & D), 3);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[3] + buffer16[g], __S[3]), B, C);

        if (4 <= 15)
            (f, g) = ((B & C) | (~B & D), 4);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[4] + buffer16[g], __S[4]), B, C);

        if (5 <= 15)
            (f, g) = ((B & C) | (~B & D), 5);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[5] + buffer16[g], __S[5]), B, C);

        if (6 <= 15)
            (f, g) = ((B & C) | (~B & D), 6);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[6] + buffer16[g], __S[6]), B, C);

        if (7 <= 15)
            (f, g) = ((B & C) | (~B & D), 7);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[7] + buffer16[g], __S[7]), B, C);

        if (8 <= 15)
            (f, g) = ((B & C) | (~B & D), 8);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[8] + buffer16[g], __S[8]), B, C);

        if (9 <= 15)
            (f, g) = ((B & C) | (~B & D), 9);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[9] + buffer16[g], __S[9]), B, C);

        if (10 <= 15)
            (f, g) = ((B & C) | (~B & D), 10);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[10] + buffer16[g], __S[10]), B, C);

        if (11 <= 15)
            (f, g) = ((B & C) | (~B & D), 11);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[11] + buffer16[g], __S[11]), B, C);

        if (12 <= 15)
            (f, g) = ((B & C) | (~B & D), 12);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[12] + buffer16[g], __S[12]), B, C);

        if (13 <= 15)
            (f, g) = ((B & C) | (~B & D), 13);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[13] + buffer16[g], __S[13]), B, C);

        if (14 <= 15)
            (f, g) = ((B & C) | (~B & D), 14);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[14] + buffer16[g], __S[14]), B, C);

        if (15 <= 15)
            (f, g) = ((B & C) | (~B & D), 15);
        else
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[15] + buffer16[g], __S[15]), B, C);

        if (16 <= 15)
        {
        }
        else if (16 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 16 + 1) % 16);
        else if (16 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 16 + 5) % 16);
        else if (16 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[16] + buffer16[g], __S[16]), B, C);

        if (17 <= 15)
        {
        }
        else if (17 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 17 + 1) % 16);
        else if (17 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 17 + 5) % 16);
        else if (17 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[17] + buffer16[g], __S[17]), B, C);

        if (18 <= 15)
        {
        }
        else if (18 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 18 + 1) % 16);
        else if (18 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 18 + 5) % 16);
        else if (18 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[18] + buffer16[g], __S[18]), B, C);

        if (19 <= 15)
        {
        }
        else if (19 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 19 + 1) % 16);
        else if (19 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 19 + 5) % 16);
        else if (19 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[19] + buffer16[g], __S[19]), B, C);

        if (20 <= 15)
        {
        }
        else if (20 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 20 + 1) % 16);
        else if (20 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 20 + 5) % 16);
        else if (20 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[20] + buffer16[g], __S[20]), B, C);

        if (21 <= 15)
        {
        }
        else if (21 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 21 + 1) % 16);
        else if (21 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 21 + 5) % 16);
        else if (21 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[21] + buffer16[g], __S[21]), B, C);

        if (22 <= 15)
        {
        }
        else if (22 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 22 + 1) % 16);
        else if (22 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 22 + 5) % 16);
        else if (22 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[22] + buffer16[g], __S[22]), B, C);

        if (23 <= 15)
        {
        }
        else if (23 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 23 + 1) % 16);
        else if (23 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 23 + 5) % 16);
        else if (23 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[23] + buffer16[g], __S[23]), B, C);

        if (24 <= 15)
        {
        }
        else if (24 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 24 + 1) % 16);
        else if (24 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 24 + 5) % 16);
        else if (24 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[24] + buffer16[g], __S[24]), B, C);

        if (25 <= 15)
        {
        }
        else if (25 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 25 + 1) % 16);
        else if (25 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 25 + 5) % 16);
        else if (25 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[25] + buffer16[g], __S[25]), B, C);

        if (26 <= 15)
        {
        }
        else if (26 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 26 + 1) % 16);
        else if (26 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 26 + 5) % 16);
        else if (26 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[26] + buffer16[g], __S[26]), B, C);

        if (27 <= 15)
        {
        }
        else if (27 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 27 + 1) % 16);
        else if (27 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 27 + 5) % 16);
        else if (27 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[27] + buffer16[g], __S[27]), B, C);

        if (28 <= 15)
        {
        }
        else if (28 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 28 + 1) % 16);
        else if (28 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 28 + 5) % 16);
        else if (28 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[28] + buffer16[g], __S[28]), B, C);

        if (29 <= 15)
        {
        }
        else if (29 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 29 + 1) % 16);
        else if (29 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 29 + 5) % 16);
        else if (29 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[29] + buffer16[g], __S[29]), B, C);

        if (30 <= 15)
        {
        }
        else if (30 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 30 + 1) % 16);
        else if (30 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 30 + 5) % 16);
        else if (30 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[30] + buffer16[g], __S[30]), B, C);

        if (31 <= 15)
        {
        }
        else if (31 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 31 + 1) % 16);
        else if (31 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 31 + 5) % 16);
        else if (31 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[31] + buffer16[g], __S[31]), B, C);

        if (32 <= 15)
        {
        }
        else if (32 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 32 + 1) % 16);
        else if (32 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 32 + 5) % 16);
        else if (32 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[32] + buffer16[g], __S[32]), B, C);

        if (33 <= 15)
        {
        }
        else if (33 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 33 + 1) % 16);
        else if (33 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 33 + 5) % 16);
        else if (33 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[33] + buffer16[g], __S[33]), B, C);

        if (34 <= 15)
        {
        }
        else if (34 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 34 + 1) % 16);
        else if (34 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 34 + 5) % 16);
        else if (34 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[34] + buffer16[g], __S[34]), B, C);

        if (35 <= 15)
        {
        }
        else if (35 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 35 + 1) % 16);
        else if (35 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 35 + 5) % 16);
        else if (35 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[35] + buffer16[g], __S[35]), B, C);

        if (36 <= 15)
        {
        }
        else if (36 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 36 + 1) % 16);
        else if (36 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 36 + 5) % 16);
        else if (36 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[36] + buffer16[g], __S[36]), B, C);

        if (37 <= 15)
        {
        }
        else if (37 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 37 + 1) % 16);
        else if (37 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 37 + 5) % 16);
        else if (37 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[37] + buffer16[g], __S[37]), B, C);

        if (38 <= 15)
        {
        }
        else if (38 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 38 + 1) % 16);
        else if (38 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 38 + 5) % 16);
        else if (38 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[38] + buffer16[g], __S[38]), B, C);

        if (39 <= 15)
        {
        }
        else if (39 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 39 + 1) % 16);
        else if (39 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 39 + 5) % 16);
        else if (39 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[39] + buffer16[g], __S[39]), B, C);

        if (40 <= 15)
        {
        }
        else if (40 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 40 + 1) % 16);
        else if (40 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 40 + 5) % 16);
        else if (40 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[40] + buffer16[g], __S[40]), B, C);

        if (41 <= 15)
        {
        }
        else if (41 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 41 + 1) % 16);
        else if (41 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 41 + 5) % 16);
        else if (41 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[41] + buffer16[g], __S[41]), B, C);

        if (42 <= 15)
        {
        }
        else if (42 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 42 + 1) % 16);
        else if (42 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 42 + 5) % 16);
        else if (42 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[42] + buffer16[g], __S[42]), B, C);

        if (43 <= 15)
        {
        }
        else if (43 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 43 + 1) % 16);
        else if (43 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 43 + 5) % 16);
        else if (43 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[43] + buffer16[g], __S[43]), B, C);

        if (44 <= 15)
        {
        }
        else if (44 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 44 + 1) % 16);
        else if (44 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 44 + 5) % 16);
        else if (44 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[44] + buffer16[g], __S[44]), B, C);

        if (45 <= 15)
        {
        }
        else if (45 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 45 + 1) % 16);
        else if (45 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 45 + 5) % 16);
        else if (45 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[45] + buffer16[g], __S[45]), B, C);

        if (46 <= 15)
        {
        }
        else if (46 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 46 + 1) % 16);
        else if (46 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 46 + 5) % 16);
        else if (46 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[46] + buffer16[g], __S[46]), B, C);

        if (47 <= 15)
        {
        }
        else if (47 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 47 + 1) % 16);
        else if (47 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 47 + 5) % 16);
        else if (47 >= 48)
        {
        }

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[47] + buffer16[g], __S[47]), B, C);

        if (48 <= 15)
        {
        }
        else if (48 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 48 + 1) % 16);
        else if (48 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 48 + 5) % 16);
        else if (48 >= 48) (f, g) = (C ^ (B | ~D), 7 * 48 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[48] + buffer16[g], __S[48]), B, C);

        if (49 <= 15)
        {
        }
        else if (49 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 49 + 1) % 16);
        else if (49 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 49 + 5) % 16);
        else if (49 >= 48) (f, g) = (C ^ (B | ~D), 7 * 49 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[49] + buffer16[g], __S[49]), B, C);

        if (50 <= 15)
        {
        }
        else if (50 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 50 + 1) % 16);
        else if (50 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 50 + 5) % 16);
        else if (50 >= 48) (f, g) = (C ^ (B | ~D), 7 * 50 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[50] + buffer16[g], __S[50]), B, C);

        if (51 <= 15)
        {
        }
        else if (51 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 51 + 1) % 16);
        else if (51 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 51 + 5) % 16);
        else if (51 >= 48) (f, g) = (C ^ (B | ~D), 7 * 51 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[51] + buffer16[g], __S[51]), B, C);

        if (52 <= 15)
        {
        }
        else if (52 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 52 + 1) % 16);
        else if (52 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 52 + 5) % 16);
        else if (52 >= 48) (f, g) = (C ^ (B | ~D), 7 * 52 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[52] + buffer16[g], __S[52]), B, C);

        if (53 <= 15)
        {
        }
        else if (53 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 53 + 1) % 16);
        else if (53 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 53 + 5) % 16);
        else if (53 >= 48) (f, g) = (C ^ (B | ~D), 7 * 53 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[53] + buffer16[g], __S[53]), B, C);

        if (54 <= 15)
        {
        }
        else if (54 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 54 + 1) % 16);
        else if (54 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 54 + 5) % 16);
        else if (54 >= 48) (f, g) = (C ^ (B | ~D), 7 * 54 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[54] + buffer16[g], __S[54]), B, C);

        if (55 <= 15)
        {
        }
        else if (55 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 55 + 1) % 16);
        else if (55 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 55 + 5) % 16);
        else if (55 >= 48) (f, g) = (C ^ (B | ~D), 7 * 55 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[55] + buffer16[g], __S[55]), B, C);

        if (56 <= 15)
        {
        }
        else if (56 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 56 + 1) % 16);
        else if (56 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 56 + 5) % 16);
        else if (56 >= 48) (f, g) = (C ^ (B | ~D), 7 * 56 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[56] + buffer16[g], __S[56]), B, C);

        if (57 <= 15)
        {
        }
        else if (57 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 57 + 1) % 16);
        else if (57 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 57 + 5) % 16);
        else if (57 >= 48) (f, g) = (C ^ (B | ~D), 7 * 57 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[57] + buffer16[g], __S[57]), B, C);

        if (58 <= 15)
        {
        }
        else if (58 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 58 + 1) % 16);
        else if (58 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 58 + 5) % 16);
        else if (58 >= 48) (f, g) = (C ^ (B | ~D), 7 * 58 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[58] + buffer16[g], __S[58]), B, C);

        if (59 <= 15)
        {
        }
        else if (59 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 59 + 1) % 16);
        else if (59 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 59 + 5) % 16);
        else if (59 >= 48) (f, g) = (C ^ (B | ~D), 7 * 59 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[59] + buffer16[g], __S[59]), B, C);

        if (60 <= 15)
        {
        }
        else if (60 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 60 + 1) % 16);
        else if (60 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 60 + 5) % 16);
        else if (60 >= 48) (f, g) = (C ^ (B | ~D), 7 * 60 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[60] + buffer16[g], __S[60]), B, C);

        if (61 <= 15)
        {
        }
        else if (61 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 61 + 1) % 16);
        else if (61 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 61 + 5) % 16);
        else if (61 >= 48) (f, g) = (C ^ (B | ~D), 7 * 61 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[61] + buffer16[g], __S[61]), B, C);

        if (62 <= 15)
        {
        }
        else if (62 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 62 + 1) % 16);
        else if (62 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 62 + 5) % 16);
        else if (62 >= 48) (f, g) = (C ^ (B | ~D), 7 * 62 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[62] + buffer16[g], __S[62]), B, C);

        if (63 <= 15)
        {
        }
        else if (63 is >= 16 and <= 31)
            (f, g) = ((D & B) | (~D & C), (5 * 63 + 1) % 16);
        else if (63 is >= 32 and <= 47)
            (f, g)                = (B ^ C ^ D, (3 * 63 + 5) % 16);
        else if (63 >= 48) (f, g) = (C ^ (B | ~D), 7 * 63 % 16);

        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[63] + buffer16[g], __S[63]), B, C);

        a0 += A;
        b0 += B;
        c0 += C;
        d0 += D;
    }

    private static readonly int[] __S =
    {
        7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
    };

    private static readonly uint[] __K =
    {
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    };
}
