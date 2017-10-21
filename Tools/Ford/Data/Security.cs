using System;
using System.Collections;

namespace Injectoclean.Tools.Ford.Data
{
    static class FordSecurity
    {
        public static Byte[] GetSecureCodeKey(Byte[] seed, Byte[] securecode)
        {
            int SecureCode = (securecode[0] << 16) + (securecode[1] << 8) + securecode[2];
            int or_ed_seed = ((SecureCode & 0xFF0000) >> 16) | (SecureCode & 0xFF00) | (seed[0] << 24) | (SecureCode & 0xff) << 16;
            int mucked_value = 0xc541a9;
            int a_bit, v9, v10, v8;
            for (int i = 0; i < 32; i++)
            {
                a_bit = ((or_ed_seed >> i) & 1 ^ mucked_value & 1) << 23;
                v9 = v10 = v8 = a_bit | (mucked_value >> 1);
                mucked_value = v10 & 0xEF6FD7 | ((((v9 & 0x100000) >> 20) ^ ((v8 & 0x800000) >> 23)) << 20) |
                    (((((mucked_value >> 1) & 0x8000) >> 15) ^ ((v8 & 0x800000) >> 23)) << 15) |
                    (((((mucked_value >> 1) & 0x1000) >> 12) ^ ((v8 & 0x800000) >> 23)) << 12) |
                    32 * ((((mucked_value >> 1) & 0x20) >> 5) ^ ((v8 & 0x800000) >> 23)) |
                    8 * ((((mucked_value >> 1) & 8) >> 3) ^ ((v8 & 0x800000) >> 23));
            }
            int v14, v13, v12;
            int key = 0;
            for (int j = 0; j < 32; j++)
            {
                a_bit = ((((seed[4] << 24) | (seed[3] << 16) | seed[1] | (seed[2] << 8)) >> j) & 1 ^ mucked_value & 1) << 23;
                v14 = v13 = v12 = a_bit | (mucked_value >> 1);
                mucked_value = v14 & 0xEF6FD7 | ((((v13 & 0x100000) >> 20) ^ ((v12 & 0x800000) >> 23)) << 20) |
                (((((mucked_value >> 1) & 0x8000) >> 15) ^ ((v12 & 0x800000) >> 23)) << 15) |
                (((((mucked_value >> 1) & 0x1000) >> 12) ^ ((v12 & 0x800000) >> 23)) << 12) |
                32 * ((((mucked_value >> 1) & 0x20) >> 5) ^ ((v12 & 0x800000) >> 23)) | 8 *
                ((((mucked_value >> 1) & 8) >> 3) ^ ((v12 & 0x800000) >> 23));
                key = ((mucked_value & 0xF0000) >> 16) | 16 * (mucked_value & 0xF) | ((((mucked_value & 0xF00000) >> 20) | ((mucked_value & 0xF000) >> 8)) << 8) | ((mucked_value & 0xFF0) >> 4 << 16);
            }
            return new Byte[] { (Byte)(key >> 16), (Byte)(key >> 8), (Byte)(key) };
        }
    }
}
