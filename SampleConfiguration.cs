
using Injectoclean.Tools.BLE;
using Injectoclean.Tools.Developers;
using Injectoclean.Tools.UserHelpers;
using Injectoclean.Views.Shell;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Injectoclean
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "BLE";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Discover servers", ClassType=typeof(DiscoverBleServer) },
            new Scenario() { Title="Connect to a server", ClassType=typeof(ConnectBleServer) },
            new Scenario() { Title="Remote Control", ClassType=typeof(RemoteControl) },
            new Scenario() { Title="Shell", ClassType=typeof(Scenario_Consol) }
           

         };

        public static Log Log = new Log();
        public static MessageScreen messageScreen = new MessageScreen();

        private BLEContainer ble = new BLEContainer(Log,messageScreen);
        public  BLEContainer BLE =>  ble; 
        public class Scenario
        {
            public string Title { get; set; }
            public Type ClassType { get; set; }
        }
    }
}
