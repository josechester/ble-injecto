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
        private BluetoothLEDeviceDisplay Deviceinfo;
        protected ILog Log;

        protected List<Byte[]> response;

        private  Comunication()
        {
            
           
        }
        protected Comunication(IDeviceInfo Deviceinfo,ILog Log)
        {
            this.Deviceinfo = Deviceinfo.Get(); 
            this.Log = Log;
            response = new List<Byte[]>();

    }
    #region getServices&Characteristics
    protected async Task GetServices()
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
            waitaresponse();
        }
        public void LogError(String message)
        {
            if (Log != null)
                Log.LogMessageError(message);
        }
        public void LogNotification(String message)
        {
            if (Log != null)
                Log.LogMessageNotification(message);
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
                    LogError($"Write failed: {result}");
                    
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                LogError(ex.Message);
            }
           
        }
        protected async Task sendrequest(Byte[] message)
        {
            try
            {

                var result = await Custom.ElementAt(1).characteristic.WriteValueAsync(
                    CryptographicBuffer.CreateFromByteArray(message));
                if (!(result == GattCommunicationStatus.Success))
                    LogError($"Write failed: {result}");
                   // else
                    //LogNotification($"Write success: {result}");
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                LogError(ex.Message);
            }
        }

        protected async Task waitaresponse()
        {
            try
            {
                var result = await Custom.ElementAt(0).characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (result == GattCommunicationStatus.Success)
                {
                    AddValueChangedHandler();
                   // LogNotification($"waiting results {result}");
                }
                else
                    LogError($"Write failed: {result}");

            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex.Message);
            }
        }
        #region ResponseHandlers
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Byte[] returned;
            if (args != null)
            {
                
                CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out returned);
               // LogNotification($"value:" + GetStringResponse(returned));
                response.Add(returned);
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
        protected String GetStringResponse(Byte[] array)
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

    }
}