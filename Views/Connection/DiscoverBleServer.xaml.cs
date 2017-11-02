
using Injectoclean.Tools.BLE;
using Injectoclean.Tools.Developers;
using Injectoclean.Tools.UserHelpers;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Injectoclean
{

    public sealed partial class DiscoverBleServer : Page
    {
        private MainPage rootPage = MainPage.Current;

        private ObservableCollection<BluetoothLEDeviceDisplay> ResultCollection = new ObservableCollection<BluetoothLEDeviceDisplay>();



        public DiscoverBleServer()
        {
            InitializeComponent();
            ResultsListView.Header=
            
            bydevice.IsChecked = true;
        }
        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser(String.Empty, NotifyType.StatusMessage);
            MainPage.Current.BLE.Discover.Getnearlydevices(ref ResultCollection);
        }
        private void BConect_click(object sender, RoutedEventArgs e)
        {
            if(txt_id.Text.Length==5 && txt_id.Text.Length < 7)
            {
                MainPage.Current.BLE.Discover.GetService(txt_id.Text);
            }
                
            else
                rootPage.NotifyUser("Error input a correct ID or select by device", NotifyType.ErrorMessage);
        }

        private void ConectById_Checked(object sender, RoutedEventArgs e)
        {
            MainPage.Current.BLE.Discover.Clear();
            ResultCollection.Clear();
            //Connect.visibility = Visibility.Visible;
            Bconectdevice.Visibility = Visibility.Collapsed;
            EnumerateButton.Visibility = Visibility.Collapsed;
            Bconect.Visibility = Visibility.Visible;
            txt_id.Visibility = Visibility.Visible;
        }

        private void Bydevice_Checked(object sender, RoutedEventArgs e)
        {
            Bconectdevice.Visibility = Visibility.Visible;
            EnumerateButton.Visibility = Visibility.Visible;
            Bconect.Visibility = Visibility.Collapsed;
            txt_id.Visibility = Visibility.Collapsed;
        }


        private void BConectdevice_click(object sender, RoutedEventArgs e)
        {
            if(ResultsListView.SelectedItem!=null)
                MainPage.Current.BLE.Discover.GetService(ResultsListView.SelectedItem as BluetoothLEDeviceDisplay);
            else
                rootPage.NotifyUser("Please select a device on the list", NotifyType.ErrorMessage);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (MainPage.Current.BLE.Discover != null)
            {
                MainPage.Current.BLE.Discover.Clear();
            }
        }
    }
}