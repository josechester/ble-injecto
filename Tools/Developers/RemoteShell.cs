using Injectoclean.Tools.BLE;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Injectoclean.Tools.UserHelpers;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.Developers
{
    class RemoteShell: Comunication
    {
        private MainPage rootPage;
        
        bool Error = false;
        int limit = 5;
        public RemoteShell(MainPage mainPage):base(new DeviceInfo(), new Log())
        {
            this.rootPage = mainPage;
            
        }
        public async void SetupCJ4()
        {

            Error = false;
            String program = "pass.cj4";
            Byte[] arg = { (byte)0x00 };
            MessageScreen dialog = new MessageScreen();
            if (!base.isready)
                await PutTaskDelay(1000);
            dialog.Show("Restarting CJ4...");
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
            Task t=base.WriteInmediateAlert(Key.Reset);
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
                base.sendrequest(command);
                base.waitaresponse();
                response = base.GetResponse();
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
                base.sendrequest(command);
                base.waitaresponse();
                response = base.GetResponse();
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
                base.sendrequest(command);
                base.waitaresponse();
                response = base.GetResponse();
                await PutTaskDelay(300);
                if (response == null)
                {
                    base.WriteInmediateAlert(Key.Up);
                    base.waitaresponse();
                    response = base.GetResponse();
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
            base.WriteInmediateAlert(com);
        }
        public async Task SendCommand(Byte[] Command)
        {
            base.sendrequest(Command);
            base.waitaresponse();
            await PutTaskDelay(500);
        }
        public string getResponse()
        {
            return base.GetStringResponse();
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
