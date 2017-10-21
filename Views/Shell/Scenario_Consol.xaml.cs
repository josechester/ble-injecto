using Injectoclean.Tools.Ford.Data;
using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Injectoclean.Tools.BLE;
using System.Linq;
using Injectoclean.Tools.Ford.GenericVin;
using System.Threading.Tasks;
using Windows.UI.Core;
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
        //private SetupCJ4 setup;
        ComunicationManager comunication=MainPage.Current.Comunication;
        public Scenario_Consol()
        {
            
            InitializeComponent();
            //setup = new SetupCJ4(MainPage.Current.Comunication, "pass.cj4", MainPage.Current.messageScreen);
            SetupCJ4.ExecuteSetup(MainPage.Current.Comunication, "pass.cj4", MainPage.Current.messageScreen);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            /*Byte[] command = CommandBuilder(message.Text);
            printonshell("Send: " + BitConverter.ToString(command).Replace("-", " "));
            Byte[] input=comunication.GetLastResponse(command,300,1);
            printonshell("Response: " + comunication.GetstringFromBytes(input));
            /*List<String[]> info = FordData.GetIds();
            foreach (String[] a in info)
                printonshell(a[0].ToString());*/

            /*String[] a = "24 31 DE F9 46".Split(' ');
            Byte[] seed = {Convert.ToByte(a[0],16), Convert.ToByte(a[1], 16), Convert.ToByte(a[2], 16),
                            Convert.ToByte(a[3],16),Convert.ToByte(a[4],16)};
            String[] Stringcode = this.message.Text.Split(' ');
            Byte[] code = { Convert.ToByte(Stringcode[0], 16), Convert.ToByte(Stringcode[1], 16), Convert.ToByte(Stringcode[2], 16) };
            code = FordSecurity.GetSecureCodeKey(seed, code);
            printonshell(comunication.GetstringFromBytes(code));*/
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                VinHelper vinHelper = new VinHelper(comunication);
                vinHelper.setProtocol(TrainInfo.NONE);
                if (!vinHelper.autodetectProtocol())
                    printonshell("Could'n outodetect protocol");
                Byte[] info = vinHelper.GetVin();
                if (info == null)
                {
                    printonshell("Could'n get train info");
                    return;
                }
                VinInfo vinInfo = vinHelper.getVINInfoFord(info);
                if (vinInfo == null)
                    printonshell("vininfo es null");
                else
                {
                    printonshell(vinInfo.ToString());
                    FordCarInfo car = FordData.getFordCarInfo(vinInfo);
                    printonshell("Forcarinfo okay");
                }
            }
            );
          //  DoWorkAsync();

        }
        private Task DoWorkAsync() // No async because the method does not need await
        {
            return Task.Run(() =>
            {
                VinHelper vinHelper = new VinHelper(comunication);
                vinHelper.setProtocol(TrainInfo.NONE);
                if (!vinHelper.autodetectProtocol())
                    printonshell("Could'n outodetect protocol");
                Byte[] info = vinHelper.GetVin();
                if (info == null)
                {
                    printonshell("Could'n get train info");
                    return;
                }
                VinInfo vinInfo = vinHelper.getVINInfoFord(info);
                if (vinInfo == null)
                    printonshell("vininfo es null");
                else
                {
                    printonshell(vinInfo.ToString());
                    FordCarInfo car = FordData.getFordCarInfo(vinInfo);
                    printonshell("Forcarinfo okay");
                }
            });
        }
        private void printonshell(String line)
        {
            shell.Text += line + String.Format(Environment.NewLine);
        }
        public byte[] CommandBuilder(String line)
        {

            String[] array = line.Split(' ');
            Byte[] temp = new byte[array.Length];
            Byte[] Command = Enumerable.Repeat((byte)0x00, 15).ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = (byte)Convert.ToInt32(array[i], 16);
                Command[i] = temp[i];
                Command[14] += Command[i];
            }
            Command[14] -= Command[0];
            return Command;
        }

    }
}
