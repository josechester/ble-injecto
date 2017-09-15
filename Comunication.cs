using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
//notes make just one class to allow use asyncronous metods
namespace SDKTemplate
{
    class Comunication
    {

        private List<BluetoothLEAttributeDisplay> ServiceCollection = new List<BluetoothLEAttributeDisplay>();
        private List<BluetoothLEAttributeDisplay> GenericAccess = new List<BluetoothLEAttributeDisplay>();
        private List<BluetoothLEAttributeDisplay> GenericAttribute = new List<BluetoothLEAttributeDisplay>();
        private List<BluetoothLEAttributeDisplay> Custom = new List<BluetoothLEAttributeDisplay>();
        private List<BluetoothLEAttributeDisplay> InmediateAlert = new List<BluetoothLEAttributeDisplay>();

        private MainPage rootPage;
        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattCharacteristicsResult characteristics;
        private bool isValueChangedHandlerRegistered = false;
        private GattPresentationFormat presentationFormat;

        private Byte[] response;
        private Comunication()
        {


        }
        public Comunication(MainPage rootpage)
        {
            this.rootPage = rootpage;
            GetServices();
            //presentationFormat.FormatType.ToString();
        }
        async Task PutTaskDelay()
        {
            await Task.Delay(1500);
        }
        public async void SetupCJ4()
        {
            byte ModeForCd = (byte)0x0B;
            String argumento = "BLE.cj4 ";
            byte[] arg = Encoding.ASCII.GetBytes(argumento);
            arg[arg.Length - 1] = (byte)0x00;
            byte[] basecommand = { 0x77, ModeForCd, (byte)(arg.Length / 256), (byte)(arg.Length % 256), 0x00 };
            byte[] command = new byte[basecommand.Length + arg.Length + 1];
            for (int i = 1; i < basecommand.Length - 1; i++)
                basecommand[basecommand.Length - 1] += basecommand[i];
            for (int i = 0; i < basecommand.Length; i++)
                command[i] = basecommand[i];
            for (int i = basecommand.Length; i < arg.Length + basecommand.Length; i++)
                command[i] = arg[i - basecommand.Length];
            WriteInmediateAlert("06");
            await PutTaskDelay();
            while (response == null || response[8]!=(byte)58)
            {
                sendrequest(command);
                await waitaresponse();
                await PutTaskDelay();
            }
            rootPage.NotifyUser(GetstringResponse(), NotifyType.StatusMessage);
        }
    #region getServices&Characteristics
        private async void GetServices()
        {
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;

            bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(rootPage.Deviceinfo.Id);
            GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
            int i = 0;
            foreach (GattDeviceService service in result.Services)
            {
                getservices(new BluetoothLEAttributeDisplay(service), i);
                i++;
            }
            i = 0;
        }
        private async void getservices(BluetoothLEAttributeDisplay service, int list)
        {
            List<BluetoothLEAttributeDisplay> CharacteristicCollection = null;
            switch (list)
            {
                case 0:
                    CharacteristicCollection = GenericAccess;
                    break;
                case 1:
                    CharacteristicCollection = GenericAttribute;
                    break;
                case 2:
                    CharacteristicCollection = Custom;
                    break;
                case 3:
                    CharacteristicCollection = InmediateAlert;
                    break;
                default:
                    CharacteristicCollection = Custom;
                    break;

            }
            characteristics = await service.service.GetCharacteristicsAsync();
            foreach (GattCharacteristic c in characteristics.Characteristics)
            {
                CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
            }

        }
    # endregion
        public async void WriteInmediateAlert(String com)
        {
            try
            {
                var result = await InmediateAlert.ElementAt(0).characteristic.WriteValueAsync(
                    CryptographicBuffer.DecodeFromHexString(com));

                if (!(result == GattCommunicationStatus.Success))
                    rootPage.NotifyUser($"Write failed: {result}", NotifyType.ErrorMessage);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

        }
        
        public async void sendrequest(Byte[] message)
        {
            try
            {
                var result = await Custom.ElementAt(1).characteristic.WriteValueAsync(
                    CryptographicBuffer.CreateFromByteArray(message));
                if (!(result == GattCommunicationStatus.Success))
                    rootPage.NotifyUser($"Write failed: {result}", NotifyType.ErrorMessage);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
        private async Task waitaresponse()
        {
            try
            {
                var result = await
                        Custom.ElementAt(0).characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (result == GattCommunicationStatus.Success)
                    AddValueChangedHandler();
                else
                    rootPage.NotifyUser($"Error getting notification: {result}", NotifyType.ErrorMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
        private readonly CoreDispatcher dispatcher;
        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () => CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out response));
            RemoveValueChangedHandler();
        }
        private void RemoveValueChangedHandler()
        {
            if (isValueChangedHandlerRegistered)
            {
                Custom.ElementAt(0).characteristic.ValueChanged -= Characteristic_ValueChanged;
                isValueChangedHandlerRegistered = false;
            }
        }
        private void AddValueChangedHandler()
        {
            if (!isValueChangedHandlerRegistered)
            {
                Custom.ElementAt(0).characteristic.ValueChanged += Characteristic_ValueChanged;
                isValueChangedHandlerRegistered = true;
            }
        }
        #region ResponseFormats
        public string GetstringResponse()
        {
            String temp = "";
            for (int i = 0; i < response.Length; i++)
            {
                if (i == 0)
                    temp += response[i].ToString("X2");
                else
                    temp += "-" + response[i].ToString("X2");
            }
            return temp;
        }
        #endregion
    }
}