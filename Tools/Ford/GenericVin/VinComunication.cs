using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Injectoclean.Tools.Ford.GenericVin;
namespace Injectoclean.Tools.Ford.GenericVin
{
    class VinComunication
    {
        int address;
        int address2;
        bool readAllAddress; // aun no estoy seguro si tengo que traer otros metodos donde se pone en false o true
        bool receivedTwo;
        public static int lastPID;
        int[] inputGlobalNew;

        public VinComunication()
        {
            receivedTwo = false;
        }

        public int[] sendAndReceiveMessage(int[] output, int numResponses, bool detect, int protocol) 
        {
          
            int checksum = 0;
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

        private int[] fillArray(int numResponse, int scale, int fillNumber)
        {
            int end = numResponse * scale;
            int[] input = new int[numResponse * scale];
            input= Enumerable.Repeat(0,input.Length).ToArray();
            return input;
        }

        public int[] HC12SendAndReceiveMessage(int[] output, int numResponses, bool detect) 
        {
            int[] input = null; ;
                for (int i = 0; i< 3; i++)
                {
                    input = fillArray(numResponses, 14, 0);

                    /* try {
                         Thread.sleep(30);
                     } catch (InterruptedException e) {
                         e.printStackTrace();
                     }*/
                    /*try {
                        input = ComnManagerHelper.getSimpleCall(this, output,"HC12SENDANDRECEIVEMSSG",500);
                    } catch (ComnException e) {
                        continue;
                    }*/
                    if (input[1] != 0x00 && input[1] != 0x80 && input[1] == output[1])
                    {
                        /*throw new ComnException("")*/;
                    }
                }
                /*if (input == null)
                    input =new int[1];*/
                    //throw new ComnException("");
                return input;
        }


        private int[] J1850PWMSendAndReceiveMessage(int[] output, int numResponses, bool detect)
        {
            int i, j;
            int[] input = null;
            int time=0;

            if (numResponses > 5)
                time = 1000;
            else
                time = 500;

            for (i = 0; i < 12; i++)
            {
                input = fillArray(numResponses, 14, 0);
               /*
                    try
                    {
                        input = ComnManagerHelper.getSimpleCall(this, output, "J1850PWM:GENERIC", time);
                    }
                    catch (ComnException e)
                    {
                        continue;
                    }*/
                if (input[1] != 0x00 && input[1] != 0x80 && input[2] == 0x41 && input[3] == 0x6b)
                {
                    //throw new ComnException("");
                }
            }
                if (input == null){ }
                //throw new ComnException("");
            return input;
        }

        private int[] J1850VPWSendAndReceiveMessage(int[] output, int numResponses, bool detect)
        {
            int i, error_count, num, offset;
            int ret = 0;
            int[] input = null;
            for (i = 0; i < 6; i++)  //era 3
            {
                if (detect == true) address = 0xFF;
                error_count = 0;
                num = 0;
                offset = 0;
                //input = ComnManagerHelper.getSimpleCall(this, output, "J1850VPW:GENERIC", 400);

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

                    /*  try
                      {
                          ret = ComnManagerHelper.getLastOneReceived().length;
                      }
                      catch (ComnException e)
                      {
                          throw new ComnException("");
                      }

                              if (ret < 14 && readAllAddress == true && offset > 0) { }
                              /throw new ComnException("");*/

                } while (error_count < 3 && ret == 14);

                if (error_count < 3 && detect == true) { }//throw new ComnException("");
            }
            if (input == null)
            { }// throw new ComnException("");

            return input;
        }

        private int[] ISO9141SendAndReceiveMessage(int[] output, int numResponses, bool detect)
        {
            int i, j, num, offset;
            int ret = 0;
            int brand = 0x10;
            int[]
            input = null;
            for (i = 0; i < 4; i++)
            {
                if (detect == true) address = 0xFF;
                output[1] = brand;

                num = 0;
                offset = 0;
                ret = 0;
                if (brand == 0x01)
                {
                    /*//Delay(4000);
                    try
                    {
                        Thread.sleep(4000);
                    }
                    catch (InterruptedException e)
                    {
                        e.printStackTrace();
                    }*/
                }

                /*try
                {
                    input = ComnManagerHelper.getSimpleCall(this, output, "ISO9141:GENERIC", 3500);
                }
                catch (ComnException e)
                {
                    throw new ComnException("");
                }*/

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
                    /*try
                    {
                        ret = ComnManagerHelper.getLastOneReceived().length;
                    }
                    catch (ComnException e)
                    {
                        break;
                    }*/
                } while (ret == 14);

                if (brand != 0x01 && detect == true) { }//throw new ComnException("");
                brand = 0x10;
                if (input == null) { }
                  //  throw new ComnException("Error");
                return input;
            }
            return input;
        }
    private int[] ISO14230SendAndReceiveMessage(int[] output, int numResponses, bool detect, int protocol)
    {
        int i;
        int j;
        int intentos;
        int[] temp = new int[20];
        int type = protocol & 0xf0;
        int bytes_received = 0;
        int offset = 0;
        int counts = 0;
        int val_input1 = 0;
        int val_input2 = 0;
        int[] input2 = new int[200];//era 200
        int ciclo = 0;
        int[] input = null;
        int time = 0;
        if (detect == true)
            address2 = 0xFF;
        for(i = 0; i< 4; i++)
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
                        //Delay(4000);
                        /* try {
                                Thread.sleep(4000);
                            } catch (InterruptedException e) {
                                e.printStackTrace();
                            }*/
                        time = 3500;
                }
                else
                {                       
                    if (readAllAddress)
                        time = 3000;
                    else
                        time = 1000;

                }
                    /*try {
                           input = ComnManagerHelper.getSimpleCall(this, temp,"ISO14230:GENERIC",time);
                           } catch (ComnException e) {
                           continue;
                       }*/
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
                                input = fillArray(1,199,0);
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
                        {
                            //throw new ComnException("");
                        }


                     }
                }
                else
                {
                    if(TrainInfo._ISO14230Init[1] == 0x85)
                    {
                        //Delay(50);
                        /*try {
                            Thread.sleep(50);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }*/
                    }
                }
            }
        }
            if (input == null)
            { }//  throw new ComnException("");
        return input;
    }

    private int[] ISO15765SendAndReceiveMessage(int[] output, int numResponses, bool detect)
    {
        int i, j;
        int[] input = null;
        for(i = 0; i < 3; i++)
        {

       /* try
        {
            input = ComnManagerHelper.getSimpleCall(this, output, "ISO15765:GENERIC", 400);
        }
        catch (ComnException e)
        {
            continue;
        }*/
        if (input[1] == 0xa8 || input[1] == 0xa9)
        {
             if (input[7] == 0x7f) { }
                //throw new ComnException("");
            return input;
        }

        //Delay(250);
        /*try
        {
            Thread.sleep(250);
        }
        catch (InterruptedException e)
        {
            e.printStackTrace();
        }*/

        }

            if (input == null) { }
            //throw new ComnException("");
        return input;
    }

    private bool validateChecksum(int[] input, int pos)
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

int[] genericMode01(int PID, int protocol)
{
    readAllAddress = false;

    if ((protocol & 0x0f) == TrainInfo.ISO15765)
    {
        TrainInfo._ISO15765mode01[8] = PID;
        return TrainInfo._ISO15765mode01;
    }
    else
    {
       TrainInfo._mode01[7] = PID;

        if ((protocol & 0x0f) == TrainInfo.ISO14230)
            TrainInfo._mode01[3] = 0xc2;
       return TrainInfo._mode01;
    }
}

int[] genericMode09(int SID, int protocol)
{
    readAllAddress = false;

    if ((protocol & 0x0f) == TrainInfo.ISO15765)
    {
        TrainInfo._ISO15765mode09[8] = SID;
        return TrainInfo._ISO15765mode09;
    }
    else
    {
        TrainInfo._mode09[7] = SID;
        if ((protocol & 0x0f) == TrainInfo.ISO14230)
            TrainInfo._mode09[3] = 0xc2;
        return TrainInfo._mode09;
    }
}

int[] genericModefd(int SID)
{
    readAllAddress = false;
    TrainInfo._modefd[2] = SID >> 8;
    TrainInfo._modefd[3] = SID & 0xff;

    return TrainInfo._modefd;
}
    }
}
