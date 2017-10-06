using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Injectoclean.Tools.Ford.GenericVin;
using Injectoclean.Tools.BLE;

namespace Injectoclean.Tools.Ford.GenericVin
{
    class VinComunication
    {
        int address;
        int address2;
        bool readAllAddress; // aun no estoy seguro si tengo que traer otros metodos donde se pone en false o true
        bool receivedTwo;
        public static int lastPID;
        Byte[] inputGlobalNew;
        ComunicationManager comunication;

        private VinComunication()
        {}
        public VinComunication(ComunicationManager comunication)
        {
            this.comunication = comunication;
            receivedTwo = false;
        }
       
        public Byte[] sendAndReceiveMessage(Byte[] output, int numResponses, bool detect, int protocol) 
        {
          
            Byte checksum = 0;
            for (int j = 1; j < 14; j++)
                checksum += output[j];
            output[14] = checksum;
            switch (protocol)
            {
                case TrainInfo.J1850PWM:
                    return J1850PWMSendAndReceiveMessage(output, numResponses, detect);
                case TrainInfo.J1850VPW:
                    return J1850VPWSendAndReceiveMessage(output, numResponses, detect);
                case TrainInfo.ISO9141:
                    return ISO9141SendAndReceiveMessage(output, numResponses, detect);
                default:
                {
                    if ((protocol & 0x0f) == TrainInfo.ISO14230)
                    {
                        if (receivedTwo == true && numResponses == 1)
                            numResponses = 2;
                        if (receivedTwo == true && numResponses == 2 && output[6] == 0x09 && output[7] == 0x02)
                            numResponses = 1;
                        return ISO14230SendAndReceiveMessage(output, numResponses, detect, protocol);
                    }
                    else if ((protocol & 0x0f) == TrainInfo.ISO15765)
                    {
                        lastPID = TrainInfo._ISO15765mode01[8];
                        return ISO15765SendAndReceiveMessage(output, numResponses, detect);
                    }
                    return null;
                } 
            }
        }

        public Byte[] HC12SendAndReceiveMessage(Byte[] output, int numResponses, bool detect) 
        {
            Byte[] input=null;
            for (int i = 0; i< 3; i++)
            {
            input = null;
                Task.Delay(30);
                Task t=comunication.GetCall(output,500,1);
            input = comunication.GetLastResponse();
                if (input!=null && input[1] != 0x00 && input[1] != 0x80 && input[1] == output[1])
                {
                    /*throw new ComnException("")*/;
                }
            }
            //if (input == null)
                //throw new ComnException("");
            return input;
        }


        private Byte[] J1850PWMSendAndReceiveMessage(Byte[] output, int numResponses, bool detect)
        {
            Byte[] input = null;
            int time=0;

            if (numResponses > 5)
                time = 1000;
            else
                time = 500;

            for (int i = 0; i < 12; i++)
            {

                Task t = comunication.GetCall(output, time, 1);
                input = comunication.GetLastResponse();
                if (input[1] != 0x00 && input[1] != 0x80 && input[2] == 0x41 && input[3] == 0x6b)
                {
                    //throw new ComnException("");
                }
            }
                if (input == null){ }
                //throw new ComnException("");
            return input;
        }

        private Byte[] J1850VPWSendAndReceiveMessage(Byte[] output, int numResponses, bool detect)
        {
            int i, error_count, num, offset;
            int ret = 0;
            Byte[] input = null;
            for (i = 0; i < 6; i++)  //era 3
            {
                if (detect == true) address = 0xFF;
                error_count = 0;
                num = 0;
                offset = 0;
                Task t=comunication.GetCall(output,400,1);
                input = comunication.GetLastResponse();
                t = comunication.waitresponses(400, 0);
                List<Byte[]> response = comunication.GetResponses();
                do
                {
                    //validar si se recibio un tren valido
                    if ((input[offset + 2] != 0x48 || input[offset + 3] != 0x6b || input[offset + 1] != 0x82 || input[offset] != 0x40))
                    {
                        error_count++;
                        if (input[offset] == 0x40 && input[offset + 1] == 0x80)
                        {
                            error_count = 3;
                            break;
                        }
                    }
                    else
                    {
                        if (detect == true && input[offset + 4] < address)
                            address = input[offset + 4];
                        else
                        {
                            if (readAllAddress == false && address != input[offset + 4]) // Aun no se si es falsa o verdadero
                                continue;
                            else
                            {
                                offset += 14;
                                if (++num >= numResponses) { }// throw new ComnException("");
                            }

                        }

                    }
                    ret = response.Last().Length;
                    response.RemoveAt(response.Count);
                } while (error_count < 3 && ret == 14);

                if (error_count < 3 && detect == true) { }//throw new ComnException("");
            }
            if (input == null)
            { }// throw new ComnException("");

            return input;
        }

        private Byte[] ISO9141SendAndReceiveMessage(Byte[] output, int numResponses, bool detect)
        {
            int num, offset;
            int ret = 0;
            byte brand = 0x10;
            Byte[] input = null;
            for (int i = 0; i < 4; i++)
            {
                if (detect == true) address = 0xFF;
                output[1] = brand;

                num = 0;
                offset = 0;
                ret = 0;
                if (brand == 0x01)
                    Task.Delay(4000);
                Task t=comunication.GetCall(output,400,1);
                input = comunication.GetLastResponse();
                do
                {
                    //validar si se recibio un tren valido de la pila
                    if (input[0] != 0x40 || input[1] == 0x80)
                    {
                        //tren invalido
                        brand = 0x01;
                        break;
                    }
                    else
                    {
                        brand = 0x10;
                        if (detect == true && (input[offset + 4] & 0x0f) < (address & 0x0f))
                            address = input[offset + 4];
                        else
                        {
                            if (readAllAddress == false && address == input[offset + 4])
                                continue;
                            else
                            {
                                offset += 14;
                                if (++num >= numResponses) { }// throw new ComnException("");
                            }
                        }
                    }
                    t = comunication.waitresponses(400, 1);
                    ret = comunication.GetLastResponse().Length;
                   
                } while (ret == 14);

                if (brand != 0x01 && detect == true) { }//throw new ComnException("");
                brand = 0x10;
                if (input == null) { }
                  //  throw new ComnException("Error");
                return input;
            }
            return input;
        }
        private Byte[] ISO14230SendAndReceiveMessage(Byte[] output, int numResponses, bool detect, int protocol)
        {
            int intentos;
            Byte[] temp = new Byte[20];
            int type = protocol & 0xf0;
            int bytes_received = 0;
            int offset = 0;
            int counts = 0;
            int val_input1 = 0;
            int val_input2 = 0;
            Byte[] input2 = new Byte[200];//era 200
            int ciclo = 0;
            Byte[] input = null;
            int time = 0;
            if (detect == true)
                address2 = 0xFF;
            for(int i = 0; i< 4; i++)
            {
                for(intentos = 0; intentos< 2; intentos++)
                {
                    switch (intentos)
                    {
                        case 0:
                            temp = output;
                            break;
                        case 1:
                            temp = TrainInfo._ISO14230Init;
                            break;
                    }
                    if(TrainInfo._ISO14230Init[1] == 0x85 && intentos == 1)  //iif response init
                    {
                        Task.Delay(4000);
                        time = 3500;
                    }
                    else
                    {                       
                        if (readAllAddress)
                            time = 3000;
                        else
                            time = 1000;

                    }
                    Task t = comunication.GetCall(output, time, 1);
                    input = comunication.GetLastResponse();
                    if (time < 3500)
                    {
                        bytes_received = input.Length;
                        if (bytes_received < 14)
                            continue;
                    }
               
                    if (input[0] == 0x40 && input[1] == 0x01 && (intentos==0))
                    {
                    
                        if(input[1] != 0x80)
                        {
                            if(detect == true)
                            {
                                if (bytes_received >25)
                                {
                                    val_input1 = input[4] & 0x0f;
                                    val_input2 = input[14 + 4] & 0x0f;
                                    if(val_input1<val_input2)
                                        address2 = input[4];
                                    else
                                    {
                                        address2 = input[14 + 4];
                                        for (ciclo=0;ciclo<14;ciclo++)
                                            input[0 + ciclo] = input[14 + ciclo];            
                                    }
                                    receivedTwo = true;
                                }
                                else
                                {
                                    if (bytes_received == 14)
                                        address2 = input[4];
                                }
                            }
                            else
                            {
                                if(readAllAddress == false)
                                {
                                    if (receivedTwo == true && address2 != input[4])
                                    {
                                        for (ciclo = 0; ciclo < 14; ciclo++)
                                            input[0 + ciclo] = input[14 + ciclo];
                                    }   
                                }
                                else
                                {
                                    offset=0;
                                    counts = 0;
                                    Byte container = input[0];
                                    input= Enumerable.Repeat<Byte>(0, 199).ToArray();
                                    input[0] = container;
                                    do
                                    {
                                        if(counts>198)break;
                                        if(input[0 + offset] != 0x40)break;
                                        if (input[offset + 4] == address2 && input[offset + 5] != 0x7f)
                                        {
                                            for (ciclo=0; ciclo<14;ciclo++)
                                                input2[counts++] = input[offset + ciclo];
                                        }
                                        offset+=14;
                                    }
                                    while (input[1 + offset] != 0x80);

                                    for (ciclo=0;ciclo<counts;ciclo++)//era 200
                                        input[ciclo] = input2[ciclo];
                                    //throw new ComnException("");

                                }

                            }
                            if (!validateChecksum(input, 13))
                            { }//throw new ComnException("");
                        }
                    }
                    else
                    {
                        if (TrainInfo._ISO14230Init[1] == 0x85)
                            Task.Delay(50);
                    }
                }
            }
            if (input == null)
            { }//  throw new ComnException("");
            return input;
        }
        private Byte[] ISO15765SendAndReceiveMessage(Byte[] output, int numResponses, bool detect)
        {
            Byte[] input = null;
            for(int i = 0; i < 3; i++)
            {
                Task t = comunication.GetCall(output, 400, 1);
                input = comunication.GetLastResponse();
               
                if (input[1] == 0xa8 || input[1] == 0xa9)
                {
                     if (input[7] == 0x7f) { }
                        //throw new ComnException("");
                    return input;
                }
                Task.Delay(250);
            }
           if (input == null) { }
                //throw new ComnException("");
            return input;
        }

        private bool validateChecksum(Byte[] input, int pos)
        {
            int i;
            int checksum = 0;
            for (i = 1; i < pos; i++)
                checksum += input[i];
            if (input[pos] != checksum)
                return true;
            else
                return false;
        }

        Byte[] genericMode01(int PID, int protocol)
        {
            readAllAddress = false;

            if ((protocol & 0x0f) == TrainInfo.ISO15765)
            {
                TrainInfo._ISO15765mode01[8] = (Byte)PID;
                return TrainInfo._ISO15765mode01;
            }
            else
            {
               TrainInfo._mode01[7] = (Byte)PID;

                if ((protocol & 0x0f) == TrainInfo.ISO14230)
                    TrainInfo._mode01[3] = 0xc2;
               return TrainInfo._mode01;
            }
        }

        Byte[] genericMode09(int SID, int protocol)
        {
            readAllAddress = false;

            if ((protocol & 0x0f) == TrainInfo.ISO15765)
            {
                TrainInfo._ISO15765mode09[8] = (Byte)SID;
                return TrainInfo._ISO15765mode09;
            }
            else
            {
                TrainInfo._mode09[7] = (Byte)SID;
                if ((protocol & 0x0f) == TrainInfo.ISO14230)
                    TrainInfo._mode09[3] = 0xc2;
                return TrainInfo._mode09;
            }
        }

        Byte[] genericModefd(int SID)
        {
            readAllAddress = false;
            
            TrainInfo._modefd[2] = (Byte)(SID >>8);
            TrainInfo._modefd[3] = (Byte)(SID & 0xff);

            return TrainInfo._modefd;
        }
    }
}
