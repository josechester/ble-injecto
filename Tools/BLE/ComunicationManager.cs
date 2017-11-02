using System;
using System.Linq;
using System.Threading.Tasks;

namespace Injectoclean.Tools.BLE
{
    public class ComunicationManager : Comunication
    {

        public ComunicationManager(ILog Log, IDeviceInfo deviceInfo) : base(deviceInfo, Log)
        {
        }
        public void init()
        {
            Task t = base.GetServices();
        }

        #region ResponseFormats
        public String GetstringFromBytes(Byte[] array)
        {
            String temp = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                    temp += array[i].ToString("X2");
                else
                    temp += "-" + array[i].ToString("X2");
            }
            return temp;
        }
        public bool IsReady() => base.isready;
        #endregion
        public void SendCommand(Windows.Storage.Streams.IBuffer com)
        {
            Task t = base.WriteInmediateAlert(com);
        }

        public Byte[][] GetResponses(Byte[] message, int timeout, int NumMessages)
        {
            base.response.Clear();
            Task t = base.sendrequest(message);
            waiter(timeout, NumMessages);
            if (response.Count == 0)
                return null;
            return response.ToArray();
        }
      

        public Byte[] GetLastResponse(Byte[] message, int timeout, int nresponses)
        {

            base.response.Clear();
            Task t = base.sendrequest(message);
            waiter(timeout, nresponses);
            if (response.Count == 0)
                return null;
            return response.Last();
        }


        public Byte[] GetLastResponse(int timeout,int nresponses)
        {
            base.response.Clear();
            waiter(timeout, nresponses);
            if (response.Count == 0)
                return null;
            return response.Last();
        }
        public Byte[][] GetResponses(int timeout, int nresponses)
        {
            base.response.Clear();
            waiter(timeout, nresponses);
            if (response.Count == 0)
                return null;
            return response.ToArray();
        }
        private void waiter(int time, int nresponses)
        {
            int timeout = time;
            while (timeout > 0 && base.response.Count()<nresponses)
            {
                PutTaskDelay(10);
                timeout -= 10;
            }
        }
        public static void PutTaskDelay(int time)
        {
            Task t=Task.Delay(time);
            t.Wait();
        }
    }

}
