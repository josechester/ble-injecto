using Injectoclean.Tools.BLE;
using Injectoclean.Tools.Ford.Data;
using System;
using System.Text;

namespace Injectoclean.Tools.Ford.GenericVin
{
    class VinHelper : TrainInfo
    {
        private int protocol;
        //int[] receiver;
        private ComunicationManager comunication;
        public VinHelper(ComunicationManager comunication)
        {
            this.comunication = comunication;
        }
        protected VinHelper()
        {
        }
        public void setProtocol(int protocol)
        {
            setProtocol(protocol, false);
        }

        public void setProtocol2(int protocol)
        {
            setProtocol(protocol, true);
        }

        private void setProtocol(int protocol, bool df)
        {
            int i = 0;
            Byte[][] allModes = new Byte[][]{ _mode01,_mode02, _mode03, _mode04, _mode05, _mode06,
                _mode07, _mode08, _mode09, null };
            Byte[][] ISO15765AllModes = new Byte[][]{ _ISO15765mode01, _ISO15765mode02, _ISO15765mode03,
                _ISO15765mode04, _ISO15765mode06,
                _ISO15765mode07, _ISO15765mode09, null };
            this.protocol = protocol;
            Byte difference = 0xe0;
            if (df)
                difference = 0xdf;

            switch (protocol)
            {
                case J1850PWM:
                    while (allModes[i] != null)
                    {
                        if (allModes[i] == _mode03 || allModes[i] == _mode07 || allModes[i] == _mode06
                                || allModes[i] == _mode09)
                        {
                            allModes[i][1] = 0x04;
                        }
                        else
                        {
                            allModes[i][1] = 0x84;
                        }
                        allModes[i][3] = 0x61;
                        allModes[i][4] = 0x6a;
                        i++;
                    }
                    break;
                case J1850VPW:
                    while (allModes[i] != null)
                    {
                        allModes[i][1] = 0x02;
                        allModes[i][3] = 0x68;
                        allModes[i][4] = 0x6a;
                        i++;
                    }
                    break;
                case ISO9141:
                    while (allModes[i] != null)
                    {
                        allModes[i][1] = 0x10;
                        allModes[i][3] = 0x68;
                        allModes[i][4] = 0x6a;
                        i++;
                    }
                    break;
                case ISO14230 | FAST_INIT:
                    while (allModes[i] != null)
                    {
                        allModes[i][1] = 0x88;
                        allModes[i][4] = 0x33;
                        i++;
                    }
                    _ISO14230Init[1] = 0x87;
                    break;
                case ISO14230 | FIVE_BAUD:
                    while (allModes[i] != null)
                    {
                        allModes[i][1] = 0x88;
                        allModes[i][4] = 0x33;
                        i++;
                    }
                    _ISO14230Init[1] = 0x85;
                    break;
                case ISO15765 | CAN_500_11_GENERIC:
                    while (ISO15765AllModes[i] != null)
                    {
                        ISO15765AllModes[i][1] = 0xa4;
                        ISO15765AllModes[i][2] = 0x00;
                        ISO15765AllModes[i][3] = 0x00;
                        ISO15765AllModes[i][4] = 0x07;
                        ISO15765AllModes[i][5] = difference;  //df
                        i++;
                    }
                    break;
                case ISO15765 | CAN_500_29_GENERIC:
                    while (ISO15765AllModes[i] != null)
                    {
                        ISO15765AllModes[i][1] = 0xa5;
                        ISO15765AllModes[i][2] = 0x18;
                        ISO15765AllModes[i][3] = 0xdb;
                        ISO15765AllModes[i][4] = 0x33;
                        ISO15765AllModes[i][5] = 0xf1;
                        i++;
                    }
                    break;
                case ISO15765 | CAN_250_11_GENERIC:
                    while (ISO15765AllModes[i] != null)
                    {
                        ISO15765AllModes[i][1] = 0xa6;
                        ISO15765AllModes[i][2] = 0x00;
                        ISO15765AllModes[i][3] = 0x00;
                        ISO15765AllModes[i][4] = 0x07;
                        ISO15765AllModes[i][5] = difference;  ///df
                        i++;
                    }
                    break;
                case ISO15765 | CAN_250_29_GENERIC:
                    while (ISO15765AllModes[i] != null)
                    {
                        ISO15765AllModes[i][1] = 0xa7;
                        ISO15765AllModes[i][2] = 0x18;
                        ISO15765AllModes[i][3] = 0xdb;
                        ISO15765AllModes[i][4] = 0x33;
                        ISO15765AllModes[i][5] = 0xf1;
                        i++;
                    }
                    break;
            }
        }


        public void init()
        {
            int[] input = new int[50]; //era 15
            Mode01Parameters = 0;
            Mode01NumParameters = 0;
            Mode02Parameters = 0;
            Mode02NumParameters = 0;
            for (int i = 0; i < ResetCommands.Length; i++)
                sendMessage(ResetCommands[i], 350);
        }

        public bool autodetectProtocol()
        {
            //se va a modificar la opcion del ISO 14230  donde se probara si vienen mas respuestas en el programa del generico
            Byte[] input = new Byte[50];  //aqui era 1
            setProtocol(ISO15765_500_11);
            setProtocol2(ISO15765_500_11);
            VinComunication genericSendAndReceiveMssg = new VinComunication(comunication);
            Byte[] output = genericSendAndReceiveMssg.genericMode01(0x00, protocol);
            if ((input = genericSendAndReceiveMssg.sendAndReceiveMessage(output, 1, true, protocol)) != null)
                return false;
            if ((input = executeSendReceive(ISO15765_500_29, input)) == null)
                return false;
            if ((input = executeSendReceive(ISO15765_250_11, input)) != null)
                return false;
            if ((executeSendReceive(ISO15765_250_29, input) != null))
                return false;
            if ((input = executeSendReceive(J1850PWM, input)) != null)
                return false;
            if ((input = executeSendReceive(J1850VPW, input)) != null)
                return false;
            setProtocol(ISO14230_FAST_INIT);
            output = genericSendAndReceiveMssg.genericMode01(0x00, protocol);
            if ((input = genericSendAndReceiveMssg.sendAndReceiveMessage(output, 2, true, protocol)) != null)
            {
                if (input[5] != 0x7f) return false;
            }
            setProtocol(ISO9141);
            output = genericSendAndReceiveMssg.genericMode01(0x00, protocol);
            if ((input = genericSendAndReceiveMssg.sendAndReceiveMessage(output, 1, true, protocol)) != null)
            {
                //return 0;  cambio validado al 0x7f por Gio
                if (input[5] != 0x7f) return false;
            }
            setProtocol(ISO14230_FIVE_BAUD);
            output = genericSendAndReceiveMssg.genericMode01(0x00, protocol);
            if ((genericSendAndReceiveMssg.sendAndReceiveMessage(output, 1, true, protocol)) != null)
            {
                //return 0;  cambio validado al 0x7f por Gio
                if (input[5] != 0x7f) return false;
            }
            return true;
        }

        public Byte[] GetVin()
        {
            int Protocol = protocol & 0x0f;
            switch (Protocol)
            {
                case ISO15765:
                    return GetVIN.getISO15765VIN(protocol,comunication);
                case ISO9141:
                case ISO14230:
                    return GetVIN.getISOVIN(protocol,comunication);   //checked OK
                case J1850PWM:
                    return GetVIN.getJ1850PWMVIN(protocol,comunication);
                case J1850VPW:
                    return GetVIN.getJ1850VPWVIN(protocol,comunication);
            }
            return null;
        }
        public VinInfo getVINInfoFord(Byte[] vin)
        {
            //if(vin.length != 17)
            //  return true;/*not VIN?*/
            StringBuilder buffer = new StringBuilder();
            for (int i = 4; i < (4 + 3); i++)
                buffer.Append((char)vin[i]);
            //memcpy(g2, &vin[4], 3);
            //g2 = (char)vin[4];
            String g2 = buffer.ToString();
            String g3 = Convert.ToString(vin[7]);
            int year = 0;
            if (vin[9] > 0 && vin[9] < 9)
                year = 2000 + vin[9] - '0';
            else if (vin[9] > 'A' && vin[9] < 'Z')
                year = 2000 + vin[9] - 55;
            else if (vin[9] > 'a' && vin[9] < 'b')
                year = 2000 + vin[9] - 87;
            if (year == 0)
                return null;
            return new VinInfo(g2, g3, year);

        }
        private Byte[] executeSendReceive(int p, Byte[] input)
        {
            VinComunication vincomunication = new VinComunication(comunication);
            setProtocol(p);
            Byte[] output = vincomunication.genericMode01(0x00, protocol);
            if ((input = vincomunication.sendAndReceiveMessage(output, 2, true, protocol)) != null)
                if (input[5] != 0x7f) return input;
            return null;
        }
        private void sendMessage(Byte[] output, int timeout)
        {
            Byte checksum = (Byte)0;

            for (int i = 1; i < 14; i++)
                checksum += output[i];
            output[14] = checksum;
            comunication.GetLastResponse(output, timeout, 1);
        }
    }
}