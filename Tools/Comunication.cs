using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private List<BluetoothLEAttributeDisplay> Custom = new List<BluetoothLEAttributeDisplay>();
        private List<BluetoothLEAttributeDisplay> InmediateAlert = new List<BluetoothLEAttributeDisplay>();

        private MainPage rootPage;
        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattCharacteristicsResult characteristics;
        private bool isValueChangedHandlerRegistered = false;
        private GattPresentationFormat presentationFormat;

        private Byte[] response;

        public Byte[] getResponse()
        {
            Byte[] temp=response;
            response = null;
            return temp; 
        }
        private Comunication(){}
        public  Comunication(MainPage rootpage)
        {
            this.rootPage = rootpage;
            GetServices();
            //presentationFormat.FormatType.ToString();
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
                getservices(service);
                i++;
            }
            i = 0;
        }
        private async void getservices(GattDeviceService service)
        {
            List<BluetoothLEAttributeDisplay> CharacteristicCollection = null;
            if (service.Uuid == GattAttributes.InmediateAlert.guid)
                CharacteristicCollection = InmediateAlert;
            else
                     if (service.Uuid == GattAttributes.UknownService.guid)
                            CharacteristicCollection = Custom;
                      else
                            return;
            characteristics = await service.GetCharacteristicsAsync();
            foreach (GattCharacteristic c in characteristics.Characteristics)
            {
                CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
            }

        }
    # endregion
        public async void WriteInmediateAlert(Windows.Storage.Streams.IBuffer com)
        {
            try
            {
                var result = await InmediateAlert.ElementAt(0).characteristic.WriteValueAsync(com);
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

        public async void waitaresponse()
        {
            try
            {
                var result = await Custom.ElementAt(0).characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
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
    #region ResponseHandlers
        //private readonly CoreDispatcher dispatcher=new CoreDispatcher();
        /*async //it was but works better lik that*/
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (args != null)
            {
                CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out response);
                RemoveValueChangedHandler(); //this is just for one response
            }
            
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
    #endregion

    #region ResponseFormats
        public String GetstringResponse()
        {
            String temp = "";
            if (response == null)
                return "Response is Null";
            for (int i = 0; i < response.Length; i++)
            {
                if (i == 0)
                    temp += response[i].ToString("X2");
                else
                    temp += "-" + response[i].ToString("X2");
            }
            return temp;
        }
        private String GetstringFromBytes(Byte[] array )
        {
            String temp = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                    temp += array[i].ToString("X2");
                else
                    temp += "-" + array[i].ToString("X2");
            }
            return temp;
        }
    #endregion
    }
}