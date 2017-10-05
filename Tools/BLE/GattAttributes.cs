using System;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace Injectoclean.Tools.BLE
{
    public class GattAttributes
    {
        public static class UknownService
        {
            public static readonly Guid guid = new Guid("0003cdd0-0000-1000-8000-00805f9b0131");

            public static readonly Guid Rx = new Guid("0003cdd1-0000-1000-8000-00805f9b0131");
            public static readonly Guid Tx = new Guid("0003cdd2-0000-1000-8000-00805f9b0131");

        }
        public static class InmediateAlert
        {
            public static readonly Guid guid = new Guid("00001802-0000-1000-8000-00805f9b34fb");
            public static readonly Guid Alertlevel = new Guid("00002a06-0000-1000-8000-00805f9b34fb");
            public static class Key
            {
                public static readonly IBuffer Up = CryptographicBuffer.DecodeFromHexString("00");
                public static readonly IBuffer Esc = CryptographicBuffer.DecodeFromHexString("01");
                public static readonly IBuffer Left = CryptographicBuffer.DecodeFromHexString("02");
                public static readonly IBuffer Enter = CryptographicBuffer.DecodeFromHexString("03");
                public static readonly IBuffer Down = CryptographicBuffer.DecodeFromHexString("04");
                public static readonly IBuffer Right = CryptographicBuffer.DecodeFromHexString("05");
                public static readonly IBuffer Reset = CryptographicBuffer.DecodeFromHexString("06");

            }
        }

    }
    
}
