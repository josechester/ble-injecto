using System;
using System.Collections.Generic;
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
            Task t = base.sendrequest(message);
            return GetResponses(timeout, NumMessages);
        }

        public Byte[] GetLastResponse(Byte[] message, int timeout, int NumMessages)
        {
            Byte[][] responses = GetResponses(message, timeout, NumMessages);
            if (responses == null)
                return null;
            if (responses.Length == 0)
                return null;
            return responses.Last();
        }


        public Byte[] GetLastResponse(int timeout,int nresponses)
        {
            Byte[][] responses = GetResponses(timeout, nresponses);
            if (responses == null)
                return null;
            if (responses.Length == 0)
                return null;
            return responses.Last();
        }
        public Byte[][] GetResponses(int time, int nresponses)
        {
            Task t = waitaresponse();
      
            List<Byte[]> responses = new List<Byte[]>();
            int timeout = time;
            while (timeout > 0)
            {
                PutTaskDelay(10);
                timeout -= 10;
                if (base.Response() != null)
                {
                    responses.Add(base.GetResponse());
                    if (nresponses != 0 && responses.Count == nresponses)
                        timeout = 0;
                }

            }
            RemoveValueChangedHandler();
            if (responses.Count == 0)
                return null;
            return responses.ToArray();
        }
        public static void PutTaskDelay(int time)
        {
            Task t=Task.Delay(time);
            t.Wait();
        }
    }

}
