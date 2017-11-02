using System;
using System.Threading.Tasks;
using static Injectoclean.Tools.BLE.GattAttributes.InmediateAlert;

namespace Injectoclean.Tools.BLE
{
    public static class SetupCJ4
    {
        public class Programs
        {
            public const String MB = "SetMB.CJ4";
            public const String HD = "SetHD.CJ4";
            public const String Test = "TestD.CJ4";
            public const String Tester = "Test.CJ4";
            public const String Pass = "pass.CJ4";
            public const String nissan = "PN.CJ4";
        }
        private static int limit=5;
        public static async Task ExecuteSetup(ComunicationManager comunication, String program, ILockScreen dialog)
        {
            if (dialog != null)
                dialog.Show("Restarting CJ4...");
            if (!comunication.IsReady())
                ComunicationManager.PutTaskDelay(1000);
            comunication.SendCommand(Key.Reset);
            await Task.Delay(1000);
            if (dialog != null)
                dialog.setTitle("Accesing Remote Shell...");
            if (!Shell.RemoteShellAccess(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Accesing Files...");

            if (!Shell.CdToFiles(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n access to files", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Executing Program");

            if (!Shell.ExecuteFile(comunication, limit, program))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n execute program" + program, "Please use a update device to this function or if your device is up to day please contact support", "Ok");
            }
            else
            {
                if (dialog != null)
                   await dialog.set("Sucess", "Program " + program + " is running", 1500);
            }
        }
        public static async Task SetupTester(ComunicationManager comunication,String program, ILockScreen dialog)
        {
            if (dialog != null)
                dialog.Show("Restarting CJ4...");
            if (!comunication.IsReady())
                ComunicationManager.PutTaskDelay(1000);
            comunication.SendCommand(Key.Reset);
            await Task.Delay(1000);
            if (dialog != null)
                dialog.setTitle("Accesing Remote Shell...");
            if (!Shell.RemoteShellAccess(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Accesing Files...");

            if (!Shell.CdToFiles(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n access to files", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Copying "+program );
            if (!Shell.CopyPCToFlash(comunication, program, "\\Assets\\Data\\"))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n copy program", "Please contact support", "Ok");
                return;
            }
            dialog.setTitle("Copying "+Programs.Tester);
            if (!Shell.CopyPCToFlash(comunication, Programs.Tester, "\\Assets\\Data\\"))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n copy program", "Please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Opening test Program");

            if (!Shell.ExecuteFile(comunication, limit, Programs.Tester))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n execute program "+Programs.Tester, "Please contact support", "Ok");
            }
            else
            {
                if (dialog != null)
                    await dialog.set("Sucess", "Program "+ Programs.Tester + " is running", 1500);
            }
            //check if test is working if not execute the respective to copy to nfc then execute test.cj4
        }
        public static async Task SetupTest(ComunicationManager comunication, String program, ILockScreen dialog)
        {
            if (dialog != null)
                dialog.Show("Restarting CJ4...");
            if (!comunication.IsReady())
                ComunicationManager.PutTaskDelay(1000);
            comunication.SendCommand(Key.Reset);
            await Task.Delay(1000);
            if (dialog != null)
                dialog.setTitle("Accesing Remote Shell...");
            if (!Shell.RemoteShellAccess(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n conect to remote Shell", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Accesing Files...");

            if (!Shell.CdToFiles(comunication, limit))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n access to files", "Please use a update device to this function or if your device is up to day please contact support", "Ok");
                return;
            }
            if (dialog != null)
                dialog.setTitle("Copying " + program);
            if (!Shell.CopyPCToFlash(comunication, program, "\\Assets\\Data\\"))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n copy program", "Please contact support", "Ok");
                return;
            }
            if (!Shell.ExecuteFile(comunication, limit, program))
            {
                if (dialog != null)
                    dialog.SetwithButton("could'n execute program " + program, "Please contact support", "Ok");
            }
            else
            {
                if (dialog != null)
                    await dialog.set("Sucess", "Program " + program + " is running", 1500);
            }
        }
    }
}
