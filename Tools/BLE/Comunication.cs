using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;

namespace Injectoclean.Tools.BLE
{
    public class Comunication
    {

        protected List<BluetoothLEAttributeDisplay> ServiceCollection = new List<BluetoothLEAttributeDisplay>();
        protected List<BluetoothLEAttributeDisplay> Custom = new List<BluetoothLEAttributeDisplay>();
        protected List<BluetoothLEAttributeDisplay> InmediateAlert = new List<BluetoothLEAttributeDisplay>();

        private BluetoothLEDevice bluetoothLeDevice = null;
        protected GattCharacteristicsResult characteristics;
        protected bool isValueChangedHandlerRegistered = false;
        protected bool isready = false;
        private Byte[] response;
        private BluetoothLEDeviceDisplay Deviceinfo;
        protected ILog Log;
        protected Byte[] GetResponse()
        {
            
            Byte[] temp=response;
            response = null;
            return temp;
        }
        protected Byte[] Response() => response;

        protected  Comunication()
        {
            Task t = GetServices();
        }
        protected Comunication(IDeviceInfo Deviceinfo,ILog Log)
        {
            this.Deviceinfo = Deviceinfo.Get(); 
            this.Log = Log;
            
        }
        #region getServices&Characteristics
        public async Task GetServices()
        {
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;

            bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(Deviceinfo.Id);
            GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
            int i = 0;
            foreach (GattDeviceService service in result.Services)
            {
               await getservices(service);
                i++;
            }
            i = 0;
            isready = true;
        }
        private async Task getservices(GattDeviceService service)
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
        #endregion
        protected async Task WriteInmediateAlert(Windows.Storage.Streams.IBuffer com)
        {
            try
            {
                var result = await InmediateAlert.ElementAt(0).characteristic.WriteValueAsync(com);
                if (!(result == GattCommunicationStatus.Success))
                    Log.LogMessageError($"Write failed: {result}");
                    
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                Log.LogMessageError(ex.Message);
            }
           
        }
        protected async Task sendrequest(Byte[] message)
        {
            try
            {
                var result = await Custom.ElementAt(1).characteristic.WriteValueAsync(
                    CryptographicBuffer.CreateFromByteArray(message));
                if (!(result == GattCommunicationStatus.Success))
                    Log.LogMessageError($"Write failed: {result}");
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                Log.LogMessageError(ex.Message);
            }
        }

        protected async Task waitaresponse()
        {
            response = null;
            try
            {
                var result = await Custom.ElementAt(0).characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (result == GattCommunicationStatus.Success)
                    AddValueChangedHandler();
                else
                    Log.LogMessageError($"Write failed: {result}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.LogMessageError(ex.Message);
            }
        }
        #region ResponseHandlers
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (args != null)
            {
                CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out response);
            }
        }
        protected void RemoveValueChangedHandler()
        {
            if (isValueChangedHandlerRegistered)
            {
                Custom.ElementAt(0).characteristic.ValueChanged -= Characteristic_ValueChanged;
                isValueChangedHandlerRegistered = false;
            }
        }
        protected void AddValueChangedHandler()
        {
            if (!isValueChangedHandlerRegistered)
            {
                Custom.ElementAt(0).characteristic.ValueChanged += Characteristic_ValueChanged;
                isValueChangedHandlerRegistered = true;
            }
        }
        #endregion
        
        protected String GetStringResponse()
        {
            Byte[] array = GetResponse();
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

    }
}