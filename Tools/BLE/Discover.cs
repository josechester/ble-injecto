using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.BLE
{
    public class Discover
    {
        private ObservableCollection<BluetoothLEDeviceDisplay> ResultCollection;
        private List<BluetoothLEAttributeDisplay> InmediateAlert = new List<BluetoothLEAttributeDisplay>();
        private DeviceWatcher deviceWatcher;
        private String id;
        //flags sync functions
        protected ILog log;
        protected ILockScreen dialog;
        protected IDeviceInfo DeviceInfo;
        private bool findbyid = false;
        private bool watcheron = false;
        public Discover(IDeviceInfo DeviceInfo, ILog log, ILockScreen dialog)
        {
            this.log = log;
            this.dialog = dialog;
            this.DeviceInfo = DeviceInfo;
        }

        private BluetoothLEDeviceDisplay Deviceinfo = null;

        public void Clear()
        {
            if (ResultCollection != null)
                ResultCollection.Clear();
            if (InmediateAlert != null)
                InmediateAlert.Clear();
            stop();
            findbyid = false;

        }
        private void stop()
        {
            if (watcheron == true)
            {
                StopBleDeviceWatcher();
                watcheron = false;
            }
        }

        private void start()
        {
            if (watcheron != true)
            {
                watcheron = true;
                StartBleDeviceWatcher();
            }
        }
        public void Getnearlydevices(ref ObservableCollection<BluetoothLEDeviceDisplay> ResultCollection)
        {
            this.Clear();
            if (log != null)
                log.LogMessageNotification("");
            this.ResultCollection = ResultCollection;
            ResultCollection.Clear();
            findbyid = false;
            start();

        }
        public void GetService(String id)
        {
            dialog.Show("Finding Device ...");
            Clear();
            if (log != null)
                log.LogMessageNotification("");
            ResultCollection = new ObservableCollection<BluetoothLEDeviceDisplay>();
            findbyid = true;
            char[] a = id.ToCharArray();
            switch (a.Length)
            {
                case 5:
                    this.id = ":0" + a[0] + ":" + a[1] + a[2] + ":" + a[3] + a[4];
                    break;
                case 6:
                    this.id = ":" + a[0] + a[1] + ":" + a[2] + a[3] + ":" + a[4] + a[5];
                    break;
                default:
                    this.id = id;
                    break;
            }
            if (Deviceinfo != null)
            {
                if (Deviceinfo.Id.Contains(this.id) == true)
                {
                    CheckConnectionAsync();
                    return;
                }
                else
                    Deviceinfo = null;
            }
            start();


        }
        public void GetService(BluetoothLEDeviceDisplay deviceinfo)
        {
            //rootPage.NotifyUser("Connecting ...", NotifyType.StatusMessage);

            Clear();
            if (deviceinfo == null)
            {
                if (log != null)
                    log.LogMessageError("Exeption DeviceInfo is null");
            }
            else
            {
                Deviceinfo = deviceinfo;
                CheckConnectionAsync();
            }
        }

        private async void CheckConnectionAsync()
        {
            DevicePairingResult results;
            dialog.Show("Connecting ...");
            if (Deviceinfo.IsPaired == false)
            {
                results = await Deviceinfo.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.Encryption);
                if (results.Status != DevicePairingResultStatus.Paired && results.Status != DevicePairingResultStatus.AlreadyPaired)
                {
                    if (log != null)
                        log.LogMessage("Pairing result: " + results.Status.ToString());
                    dialog.SetwithButton("Device Connection failed", "Problem: " + results.Status.ToString() , "Ok");
                    DeviceInfo.Set(null);
                    Deviceinfo = null;
                    this.Clear();
                    return;
                }
            }


            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(Deviceinfo.Id);
            GattDeviceServicesResult Gattservices = await bluetoothLeDevice.GetGattServicesAsync();
            BluetoothLEAttributeDisplay inmdiateAlert = null;
            var services = Gattservices.Services;
            int i = 0;

            foreach (GattDeviceService service in Gattservices.Services)
            {
                if (service.Uuid == GattAttributes.UknownService.guid)
                {
                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (GattCharacteristic c in characteristics.Characteristics)
                    {
                        //if(c.Uuid== GattAttributes.InmediateAlert.Alertlevel)
                        if (c.Uuid == GattAttributes.UknownService.Tx)
                            inmdiateAlert = new BluetoothLEAttributeDisplay(c);
                    }
                }
                i++;
            }
            if (inmdiateAlert == null)
            {
                dialog.SetwithButton("Device Connection failed", "This Program just works with a CJ4, please purchase with a Certified distributor", "Ok");
                this.Clear();
                DeviceInfo.Set(null);
                Deviceinfo = null;
                return;
            }
            try
            {
                GattWriteResult result = await inmdiateAlert.characteristic.WriteValueWithResultAsync(Key.Esc);
                //GattReadResult result = await inmdiateAlert.characteristic.ReadValueAsync();
                if (result.Status == GattCommunicationStatus.Success)
                {
                    await dialog.set("Correct", "Device Connection success", 1500);
                    //t.Wait();
                    //ComunicationManager.PutTaskDelay(1505);
                    DeviceInfo.SetandSetup(Deviceinfo);
                    this.Clear();
                }
                else
                {
                    dialog.SetwithButton("Device Connection failed", "Please Reset CJ4 manually and try again", "Ok");

                    DeviceInfo.Set(null);
                    Deviceinfo = null;
                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                if (log != null)
                    log.LogMessageError(ex.Message.ToString());
                dialog.SetwithButton("Device Connection failed", "This Program just works with a CJ4 with BLE Tecnology, please purchase with a Certified distributor", "Ok");
                DeviceInfo.Set(null);
                Deviceinfo = null;
            }
            if (watcheron == true)
            {
                StopBleDeviceWatcher();
                watcheron = false;
            }
        }
        #region Device discovery

        private void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }

        }

        /// <summary>
        ///     Starts a device watcher that looks for all nearby BT devices (paired or unpaired). Attaches event handlers and
        ///     populates the collection of devices.
        /// </summary>
        private void StartBleDeviceWatcher()
        {
            // Additional properties we would like about the device.
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            // BT_Code: Currently Bluetooth APIs don't provide a selector to get ALL devices that are both paired and non-paired.
            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                        "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")",
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            ResultCollection.Clear();

            // Start the watcher.
            deviceWatcher.Start();
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in ResultCollection)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }
        private BluetoothLEDeviceDisplay FindBluetoothLEbyPartofId(string id)
        {
            String temp;
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in ResultCollection)
            {
                temp = bleDeviceDisplay.Id;
                if (temp.Contains(id) == true)
                {
                    if (log != null)
                        log.LogMessageNotification("Device Finded,Please wait...");
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    // Make sure device name isn't blank or already present in the list.
                    if (deviceInfo.Name != string.Empty && FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                    {
                        ResultCollection.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                        //to make quickly the conection
                        if (findbyid == true && deviceInfo.Id.Contains(this.id) == true)
                        {
                            Deviceinfo = FindBluetoothLEDeviceDisplay(deviceInfo.Id);
                            findbyid = false;
                            CheckConnectionAsync();
                        }

                    }
                }
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {

            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        bleDeviceDisplay.Update(deviceInfoUpdate);
                    }
                }
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    // Find the corresponding DeviceInformation in the collection and remove it.
                    BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        ResultCollection.Remove(bleDeviceDisplay);
                    }
                }
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    dialog.SetwithButton("Device not found ", "Please retry with a CJ4 S/N", "Ok");

                    DeviceInfo.Set(null);
                    Deviceinfo = null;
                    this.Clear();
                }
            });
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    /* rootPage.NotifyUser($"No longer watching for devices.",
                             sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);*/
                }
            });
        }
        #endregion  
    }
}
