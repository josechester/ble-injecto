using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKTemplate
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Scenario_Consol : Page
    {
        private MainPage rootPage;
        private Comunication ble;
        private static AutoResetEvent resetEvent = new AutoResetEvent(false);
        public Scenario_Consol()
        {
            
            this.InitializeComponent();
            rootPage = MainPage.Current;
            ble = new Comunication(rootPage);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            ble.SetupCJ4();
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("00");
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("02");
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("04");
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("05");
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("03");
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("06");
        }

        private void escape_Click(object sender, RoutedEventArgs e)
        {
            ble.WriteInmediateAlert("01");
        }
    }
}
