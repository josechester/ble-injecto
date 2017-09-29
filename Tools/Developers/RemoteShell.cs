using Injectoclean.Tools.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Injectoclean.Tools.BLE.GattAttributes;
using Injectoclean.Tools.UserHelpers;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.Developers
{
    class RemoteShell
    {
        private MainPage rootPage;
        private Comunication comunication;
        bool Error = false;
        int limit = 5;
        private RemoteShell()
        {
        }

        public RemoteShell(MainPage mainPage)
        {
            this.rootPage = mainPage;
            comunication = new Comunication(mainPage);
        }
        public async void SetupCJ4()
        {

            Error = false;
            String program = "pass.cj4";
            Byte[] arg = { (byte)0x00 };
            MessageScreen dialog = new MessageScreen("Restarting CJ4...");
            if (!comunication.isready)
                await PutTaskDelay(1000);
            dialog.Show();
            await RestartCJ4();
            dialog.setTitle("Accesing Remote Shell...");
            await RemoteShellAccess();
            if (Error == true)
            {
                dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            dialog.setTitle("Accesing Files...");
            await CdToFiles();
            if (Error == true)
            {
                dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            dialog.setTitle("Executing Program");
            await ExecuteFile(program);
            if (Error == true)
                dialog.SetwithButton("could'n execute program" + program, "Please use a update device to this function or if your device is up to day please contact support", "Ok");
            else
                dialog.set("Sucess", "Program " + program + " is running", 1500);
        }
        private async Task RestartCJ4()
        {
            comunication.WriteInmediateAlert(Key.Reset);
            await PutTaskDelay(3000);
        }

        private async Task RemoteShellAccess()
        {
            Byte[] arg = { (byte)0x00 };
            Byte[] command = CommandBuilder(arg, (byte)0x00);
            Byte[] response = null;
            int tries = 0;
            while (tries < limit && (response == null || response[response.Length - 1] != 88))
            {
                comunication.sendrequest(command);
                comunication.waitaresponse();
                response = comunication.getResponse();
                await PutTaskDelay(1500);
                tries++;
            }
            if (tries == limit)
                Error = true;
        }
        private async Task CdToFiles()
        {
            Byte[] arg = { (byte)0x00 };
            Byte[] response = null;
            Byte[] command = CommandBuilder(arg, (byte)0x09);
            int tries = 0;
            while (tries < limit && (response == null || response.Length < 5))
            {
                comunication.sendrequest(command);
                comunication.waitaresponse();
                response = comunication.getResponse();
                await PutTaskDelay(1500);
                tries++;
            }
            if (tries == limit)
                Error = true;

        }
        private async Task ExecuteFile(String program)
        {
            Byte[] response = null;
            Byte[] command = CommandBuilder(Encoding.ASCII.GetBytes(program + " "), (byte)0x0b);
            bool running = false;
            int tries = 0;
            while (tries < limit && !running)
            {
                comunication.sendrequest(command);
                comunication.waitaresponse();
                response = comunication.getResponse();
                await PutTaskDelay(300);
                if (response == null)
                {
                    comunication.WriteInmediateAlert(Key.Up);
                    comunication.waitaresponse();
                    response = comunication.getResponse();
                    if (response == null)
                        running = true;
                    break;
                }
                else
                    tries++;
            }
            if (!running)
                Error = true;

        }
        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }
        private static byte[] CommandBuilder(byte[] arg, byte modeForCd)
        {

            arg[arg.Length - 1] = (byte)0x00;
            byte[] basecommand = { 0x77, modeForCd, (byte)(arg.Length / 256), (byte)(arg.Length % 256), 0x00 };
            byte[] command = new byte[basecommand.Length + arg.Length + 1];
            for (int i = 1; i < basecommand.Length - 1; i++)
                basecommand[basecommand.Length - 1] += basecommand[i];
            for (int i = 0; i < basecommand.Length; i++)
                command[i] = basecommand[i];
            for (int i = basecommand.Length; i < arg.Length + basecommand.Length; i++)
                command[i] = arg[i - basecommand.Length];
            command[command.Length - 1] = (byte)((int)command[0] + 1);
            return command;
        }
        public void RemoteKey(Windows.Storage.Streams.IBuffer com)
        {
            comunication.WriteInmediateAlert(com);
        }
        public async Task SendCommand(Byte[] Command)
        {
            comunication.sendrequest(Command);
            comunication.waitaresponse();
            await PutTaskDelay(500);
        }
        public string getResponse()
        {
            return comunication.GetstringResponse();
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
