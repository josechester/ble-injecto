using Injectoclean.Tools.BLE;
using Injectoclean.Tools.Ford.Data;

namespace Injectoclean.Tools.Ford.GenericVin
{
    class OrchesterVinDetector
    {
        public static int ERROR_READING = -1;

        enum ErrorType { ERROR_READING, ERROR_DETECTING_PROTOCOL, UNKNOW }

        public static FordCarInfo autoDetectVINs(ComunicationManager comunication)
        {
            VinHelper vinHelper = new VinHelper(comunication);
            vinHelper.setProtocol(TrainInfo.NONE);
            if(vinHelper.autodetectProtocol())
            {
                    VinInfo vinInfo;
                    if ((vinInfo = vinHelper.getVINInfoFord(vinHelper.GetVin())) == null)
                        comunication.LogError("Could'n get VinInfo");
                    else
                        return FordData.getFordCarInfo(vinInfo);
            }
            return null; 
        }



    }
}
