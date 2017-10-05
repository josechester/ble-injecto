using Injectoclean.Tools.BLE;
using static Injectoclean.MainPage;
namespace Injectoclean.Tools.Developers
{
    public class DeviceInfo : IDeviceInfo
    {
        public BluetoothLEDeviceDisplay Get()=>Current.Deviceinfo;

        public void Set(BluetoothLEDeviceDisplay DeviceInfo)
        {
            Current.Deviceinfo = DeviceInfo;
        }
    }
}
