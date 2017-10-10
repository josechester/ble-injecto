using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.BLE
{
    public class SetupCJ4
    {
        private ComunicationManager comunication;
        protected ILockScreen dialog;
        bool Error = false;
        int limit = 5;

        public SetupCJ4(ComunicationManager comunication, String program, ILockScreen dialog)
        {
            this.dialog = dialog;
            this.comunication = comunication;
            ExecuteSetup(program);
            
        }
        private SetupCJ4()
        {
           

        }
        public async void ExecuteSetup(String program) 
        {
            dialog.Show("Restarting CJ4...");
            await Task.Delay(1);
            Error = false;
            //String program = "pass.cj4";
            Byte[] arg = { (byte)0x00 };
            if (!comunication.IsReady())
                ComunicationManager.PutTaskDelay(1000);
            comunication.SendCommand(Key.Reset);
            ComunicationManager.PutTaskDelay(1000);
            //await Task.Delay(1000);
            dialog.setTitle("Accesing Remote Shell...");
            RemoteShellAccess();
            if (Error)
            {
                dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            dialog.setTitle("Accesing Files...");
            CdToFiles();
            if (Error)
            {
                dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            dialog.setTitle("Executing Program");

            ExecuteFile(program);
            if (Error)

                dialog.SetwithButton("could'n execute program" + program, "Please use a update device to this function or if your device is up to day please contact support", "Ok");
            else
                dialog.set("Sucess", "Program " + program + " is running", 1500);
        }

        private void RemoteShellAccess()
        {
            Byte[] arg = { (byte)0x00 };
            Byte[] command = CommandBuilder(arg, (byte)0x00);
            Byte[] response = null;
            int tries = 0;
            while (tries < limit )
            {
                //crear get last on comunicationmanager response
                response = comunication.GetLastResponse(command, 500, 1);
                //await PutTaskDelay(1500);
                if (response != null)
                {
                    if (response[response.Length - 1] == 88)
                        return;
                }
                tries++;
            }
            if (tries == limit+1)
                Error = true;
        }
        private void CdToFiles()
        {
            Byte[] arg = { (byte)0x00 };
            Byte[] response = null;
            Byte[] command = CommandBuilder(arg, (byte)0x09);
            int tries = 0;
            while (tries < limit && (response == null || response.Length < 5))
            {
                response = comunication.GetLastResponse(command, 300, 1);
                tries++;
            }
            if (tries == limit+1)
                Error = true;

        }
        private void ExecuteFile(String program)
        {
            Byte[] response = null;
            Byte[] command = CommandBuilder(Encoding.ASCII.GetBytes(program + " "), (byte)0x0b);
            bool running = false;
            int tries = 0;
            while (tries < limit && !running)
            {
                response = comunication.GetLastResponse(command, 300, 1);
                if (response == null)
                {
                    comunication.SendCommand(Key.Up);
                    response = comunication.GetLastResponse(300,1);
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
