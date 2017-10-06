namespace Injectoclean.Tools.BLE
{
    public interface IDeviceInfo
    {
        BluetoothLEDeviceDisplay Get();
        void Set(BluetoothLEDeviceDisplay DeviceInfo);
        void SetandSetup(BluetoothLEDeviceDisplay DeviceInfo);
    }
}
