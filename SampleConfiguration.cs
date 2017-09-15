
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "BLE";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Discover servers", ClassType=typeof(Scenario1_DiscoverServer) },
            new Scenario() { Title="Connect to a server", ClassType=typeof(Scenario2_ConnectToServer) },
             new Scenario() { Title="Shell", ClassType=typeof(Scenario_Consol) }


         };
        
   
        private BluetoothLEDeviceDisplay deviceinfo = null;

        public BluetoothLEDeviceDisplay Deviceinfo
        {
            get
            {
                return deviceinfo;
            }
            set
            {
                this.deviceinfo = value;
            
                //this.OnPropertyChanged();
            }
        }
        /*private void OnPropertyChanged()
        {
            if (deviceinfo != null) { return; }
                
        }*/
        public class Scenario
        {
            public string Title { get; set; }
            public Type ClassType { get; set; }
        }
    }
}
