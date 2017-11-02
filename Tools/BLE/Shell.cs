using System;
using System.IO;
using System.Linq;
using System.Text;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.BLE
{
    internal  static class Shell
    {
        // Remote Shell Commands.
        internal static  Byte DETECT = 0x00;
        internal static  Byte FORMAT = 0x01;
        internal static  Byte OPEN_FILE = 0x02;
        internal static  Byte CLOSE_FILE = 0x03;
        internal static  Byte WRITE_FILE = 0x04;
        internal static  Byte READ_FILE = 0x05;
        internal static  Byte DELETE_FILE = 0x06;
        internal static  Byte RENAME_FILE = 0x07;
        internal static  Byte DIR = 0x08;
        internal static  Byte CD = 0x09;
        internal static  Byte READ_PAGE = 0x0a;
        internal static  Byte EXEC = 0x0b;
        internal static  Byte CHECK_RAM = 0x0c;
        internal static  Byte BOOT_HC12 = 0x0d;
        internal static  Byte ERASE_HC12 = 0x0e;
        internal static  Byte WRITE_HC12 = 0x0f;
        internal static  Byte EXIT_BOOT_HC12 = 0x10;
        internal static  Byte SERIAL_HC12 = 0x11;
        internal static  Byte PROGRAM = 0xa0;
        internal static  Byte ERASE_SECTOR =0xa1;
        internal static  Byte ERROR =0xfe;
        internal static  Byte END_SHELL =0xff;

        // Remote Shell Error Commands.
        internal static  Byte ERROR_DATAFLASH_INVALID = 0x01;
        internal static  Byte ERROR_INCORRECT_SIZE = 0x02;
        internal static  Byte ERROR_CANT_OPEN_FILE = 0x03;
        internal static  Byte ERROR_DATAFLASH_NOT_FORMATED = 0x04;
        internal static  Byte ERROR_NO_OPEN_FILE = 0x05;
        internal static  Byte ERROR_WRITE = 0x06;
        internal static  Byte ERROR_FILE_DOESNT_EXISTS = 0x07;
        internal  static bool RemoteShellAccess(ComunicationManager comunication, int limit)
        {
            Byte[] arg = { (byte)0x00 };
            Byte[] command = CommandBuilder(arg, (byte)0x00);
            Byte[] response = null;
            int tries = 0;
            while (tries < limit)
            {
                response = comunication.GetLastResponse(command, 500, 1);
                if (response != null)
                {
                    if (response[response.Length - 1] == 88)
                        return true;
                }
                tries++;
            }
            if (tries == limit + 1)
                return false;
            else
                return true;
        }
        internal  static bool CdToFiles(ComunicationManager comunication, int limit)
        {
            Byte[] response = null;
            Byte[] command = CommandBuilder(new Byte[] { Shell.DETECT }, Shell.CD);
            int tries = 0;
            while (tries < limit && (response == null || response.Length < 5))
            {
                response = comunication.GetLastResponse(command, 300, 1);
                tries++;
            }
            if (tries == limit + 1)
                return false;
            else
                return true;


        }
        internal  static bool ExecuteFile(ComunicationManager comunication, int limit, String program)
        {
            Byte[] response = null;
            Byte[] command = CommandBuilder(Encoding.ASCII.GetBytes(program + " "), Shell.EXEC);
            bool running = false;
            int tries = 0;
            while (tries < limit && !running)
            {
                response = comunication.GetLastResponse(command, 300, 1);
                if (response == null)
                {
                    comunication.SendCommand(Key.Up);
                    response = comunication.GetLastResponse(300, 1);
                    if (response == null)
                        return true;
                    break;
                }
                else
                    tries++;
            }
            return false;
        }
        internal  static byte[] CommandBuilder(byte[] arg, byte modeForCd)
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

        internal  static byte[] CommandBuilder(String line)
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
        internal  static Byte[] buildSendCommand(byte mode, int size, byte[] arg)
        {
            byte[] baseCommand = { 0x77, 0x00, 0x00, 0x00, 0x00 };
            byte[] command = new byte[baseCommand.Length + size + 1];
            baseCommand[1] = mode;
            baseCommand[2] = (byte)(size / 256);
            baseCommand[3] = (byte)(size % 256);
            for (int i = 1; i < 4; i++)
                baseCommand[4] += baseCommand[i];
            Array.Copy(baseCommand, 0, command, 0, baseCommand.Length);
            if (size > 0 && arg != null)
                Array.Copy(arg, 0, command, baseCommand.Length, arg.Length);
            command[command.Length - 1] = (byte)(command[0] + 1);
            return command;
        }
        internal static bool CopyPCToFlash(ComunicationManager comunication, String source, String path)
        {
            int offset = 14;
            byte[] filename = new byte[source.Length+2];
            filename[0] = 0x00;
            filename[filename.Length-1] = 0x00;
            Array.Copy(Encoding.ASCII.GetBytes(source), 0, filename, 1, source.Length);
            Byte[] request = buildSendCommand(Shell.OPEN_FILE, filename.Length, filename);
           
            Byte[] response = comunication.GetLastResponse(request, 3000, 1);
            if (!CheckResponse(response, Shell.OPEN_FILE))
              return false;
            //send file

            Byte[] buffer = File.ReadAllBytes(Directory.GetCurrentDirectory() + path + source);
            int frames = (buffer.Length / offset);
            for (int i = 0; i <=frames ; i++)
            {
                //if ((frames % 50) == 0)
                //    ComunicationManager.PutTaskDelay(1000);
                if (i == frames)
                    request = buildSendCommand(Shell.WRITE_FILE, buffer.Length - (frames * offset), buffer.Skip(i * offset).Take(buffer.Length - (frames * offset)).ToArray());
                else
                    request = buildSendCommand(Shell.WRITE_FILE, offset, buffer.Skip(i * offset).Take(offset).ToArray());
                response = comunication.GetLastResponse(request, 500, 1);
                if (!CheckResponse(response, Shell.WRITE_FILE))
                    return false;

            }
            //file close
            request = buildSendCommand(Shell.CLOSE_FILE, 0,null);
            if (!CheckResponse(comunication.GetLastResponse(request, 400, 1), Shell.CLOSE_FILE))
                return false;
            return true;
        }
        internal  static bool CheckResponse(Byte[] Response, Byte command)
        {
            if (Response != null)
            {
                if (Response[0] == 0x57 &&
                    Response[1] == command &&
                    Response[Response.Length - 2] == command &&
                    Response[Response.Length - 1] == 0x58)
                    return true;
            }
            return false;
        }
    }
}
