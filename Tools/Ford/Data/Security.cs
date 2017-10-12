using System;
using System.Collections;

namespace Injectoclean.Tools.Ford.Data
{
    static class FordSecurity
    {
        private static Hashtable secret_keys = new Hashtable()
        {
            { 0x726, "3F 9E 78 C5 96" }, { 0x727, "50 C8 6A 49 F1" },{ 0x733, "AA BB CC DD EE" },
            {0x736, "08 30 61 55 AA" }, {0x737, "52 6F 77 61 6E" }, {0x760, "5B 41 74 65 7D" },
            {0x765, "96 A2 3B 83 9B"}, {0x7a6, "50 C8 6A 49 F1" },{0x7e0, "08 30 61 A4 C5" }
        };

        private static Hashtable secret_keys2 = new Hashtable() { { 0x7e0, "44 49 4F 44 45" }, { 0x737, "5A 89 E4 41 72" } };

        public static Byte[] GetSecureCodeKey(Byte[] seed, Byte[] securecode, int mode)
        {
            //int[] seed = new int[5];
            /*String[] secret;
            if (mode == 1)
            {
                if (!secret_keys.Contains(wid))
                    return null;
                secret = secret_keys[wid].ToString().Split(' ');
            }
            else
            {
                if (!secret_keys2.Contains(wid))
                    return null;
                secret = secret_keys2[wid].ToString().Split(' ');
            }

            for (int a = 0; a < 5; a++)
                seed[a] = Convert.ToInt32(secret[a], 16);*/
            
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
            return new Byte[] { (Byte)(key & 0xff0000), (Byte)(key & 0xff00), (Byte)(key & 0xff) };

        }
    }
}
