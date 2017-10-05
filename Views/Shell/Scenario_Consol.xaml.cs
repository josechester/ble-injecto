using Injectoclean.Tools.Ford.Data;
using Injectoclean.Tools.Developers;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Injectoclean.Tools.BLE;
// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Injectoclean
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Scenario_Consol : Page
    {
        private MainPage rootPage;
        private static AutoResetEvent resetEvent = new AutoResetEvent(false);
        private SetupCJ4 setup;
        public Scenario_Consol()
        {
            
            InitializeComponent();
            setup = new SetupCJ4(MainPage.Current.Comunication, "pass.cj4", MainPage.Current.messageScreen);
            
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            /*Byte[] command = comunication.CommandBuilder(message.Text);
            printonshell("Send: " + BitConverter.ToString(command).Replace("-", " "));
            await comunication.SendCommand(command);
            printonshell("Response: " + comunication.getResponse());*/
            List<String[]> info = FordData.GetIds();
            foreach (String[] a in info)
                printonshell(a[0].ToString());
        }
        private void printonshell(String line)
        {
            shell.Text += line + String.Format(Environment.NewLine);
        }

    }
}
