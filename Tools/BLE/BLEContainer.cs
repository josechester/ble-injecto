namespace Injectoclean.Tools.BLE
{
    public class BLEContainer : IDeviceInfo
    {
        private BluetoothLEDeviceDisplay device;
        public BluetoothLEDeviceDisplay Device => device;
        private ComunicationManager comunication;
        private ILog log;
        private ILockScreen lockscreen;
        private Discover discover;
        public ComunicationManager Comunication => comunication;
        public Discover Discover => discover;
        public BLEContainer(ILog log,ILockScreen lockscreen)
        {
            this.log = log;
            this.lockscreen = lockscreen;
            discover = new Discover(this, log, lockscreen);
        }
        public bool IsConnected() => (device != null);

        public void connect(string id)=> discover.GetService(id);

        //Interface Implementation
        public BluetoothLEDeviceDisplay Get() => device;

        public void Set(BluetoothLEDeviceDisplay DeviceInfo)
        {
            device = DeviceInfo;
        }

        public void SetandSetup(BluetoothLEDeviceDisplay DeviceInfo)
        {
            device = DeviceInfo;
            comunication = new ComunicationManager(log, this);
            lockscreen.Show("Setup Services ...");
            comunication.init();
            lockscreen.Close();
        }
        //Force to use the alternative constructor
        private BLEContainer(){ }        
    }
}
