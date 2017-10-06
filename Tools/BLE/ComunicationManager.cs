using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Injectoclean.Tools.BLE
{
    public class ComunicationManager : Comunication
    {
        private List<Byte[]> responses=null;
        public ComunicationManager(ILog Log,IDeviceInfo deviceInfo) : base(deviceInfo,Log)
        {
            responses = new List<Byte[]>();
        }

        public List<Byte[]> GetResponses() => responses;

        #region ResponseFormats
        public Byte[] GetLastResponse()
        {
            if(responses.Count==0)
                return null;
            return responses.Last(); ;
        }

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
        public async Task SendCommand(Windows.Storage.Streams.IBuffer com)
        {
            await base.WriteInmediateAlert(com);
        }
        
        public async Task GetCall(Byte[] message, int timeout,int NumMessages)
        {
            await base.sendrequest(message);
            await base.waitaresponse();
            await this.waitresponses(timeout, NumMessages);
        }


        public async Task waitresponses(int time, int nresponses)
        {
           
            responses.Clear();
            int timeout = time;
            while (timeout > 0)
            {
                await PutTaskDelay(10);
                timeout -= 10;
                if (base.Response() != null)
                {
                    responses.Add(base.GetResponse());
                    if(responses.Count == nresponses)
                        timeout = 0;
                }
                    
            }
            RemoveValueChangedHandler();
        }
        private async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }
    }

}
