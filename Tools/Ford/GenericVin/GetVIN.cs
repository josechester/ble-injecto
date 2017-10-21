using Injectoclean.Tools.BLE;
using System;
using System.Linq;

namespace Injectoclean.Tools.Ford.GenericVin
{
    static class GetVIN
    {

        public static Byte[] getISO15765VIN(int protocol,ComunicationManager comunication)
        {
            Byte[] debug = new Byte[100];
            Byte[] input = new Byte[256];
            ArrayExtensions.Fill(input, 0, 255,(byte)0);
            Byte[] VIN = new Byte[20];
            ArrayExtensions.Fill(VIN, 0, VIN.Length-1, (byte)0);
            if ((input = executeSendReceiveMode09(protocol, 0x00, new VinComunication(comunication))) == null)
                comunication.LogError("no VIN");
            ComunicationManager.PutTaskDelay(200);
            if ((input = executeSendReceiveMode09(protocol,0x02, new VinComunication(comunication))) == null)
                    comunication.LogError("Comunication Fail");
            if (input == null)
            {
                comunication.LogError("getISO15765VIN could get VIN");
                return null;
            }
                if (input[7] == 0x7f && input[9] == 0x78)
            {
                long startTime =  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                int intentos = 0;
                do
                {
                    if(((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime) > 500)
                        comunication.LogError("Timeout finish on getISO15765VIN");
                    input = comunication.GetLastResponse(350,0);
                    if(input==null)
                    {
                        intentos++;
                        continue;
                    }

                }while((input[7] == 0x7f && input[9] == 0x7)  && intentos< 5);
            }
            else if((input[6] & 0xf0) != 0x10)
                comunication.LogError("");//i don't know the reason

            int numbytes = (input[6] & 0x0f);
            numbytes <<= 8;
            numbytes += input[7];
            int numresponses = (numbytes - 6) / 7;
            if(((numbytes - 6) % 7) == 1)
                numresponses++;
            int i = 11, j = 0;
            if (input.Length < 44) { }
                comunication.LogError("Message is to short");
            while (i<  44){
                VIN[j++] = Convert.ToByte(input[i++]);
                if((i % 15) == 14){
                    i += 8;
                }
            }
            VIN[j] = 0;
            return VIN;
        }
        public static Byte[] getISOVIN(int protocol, ComunicationManager comunication) 
        {
            int i = 0;
            int j = 0;
            int sequence = 1;
            Byte[] buffer = new Byte[50];
            Byte[] VIN = new Byte[20];
            ArrayExtensions.Fill(VIN, 0, VIN.Length - 1, (byte)0); // si esto llegase a fallar voy a tener que buscar los fill y cambiarlos
                if ((buffer = executeSendReceiveMode09(protocol, 0x02, new VinComunication(comunication))) == null) {
                    comunication.LogError("Comunication Fail");
            }
            while(i< 10) {
                if (buffer != null)
                {
                    if (buffer[5] == 0x7f)
                        comunication.LogError("Comunication Fail");
                    if (buffer[0] == 0 && buffer[1] == 0)
                        break;
                    i++;

                    if (/*buffer[3] == 0x6b &&*/  buffer[5] == 0x49 && buffer[6] == 0x02 && buffer[7] == sequence)
                    {
                        if (sequence != 1) {
                            if (buffer[8] != 0xff) VIN[j++] = buffer[8];
                            if (buffer[9] != 0xff) VIN[j++] = buffer[9];
                            if (buffer[10] != 0xff) VIN[j++] = buffer[10];
                        }

                        if (buffer[11] != 0xff) VIN[j++] = buffer[11];

                        if (++sequence == 6) break;
                    }
                }
                buffer=comunication.GetLastResponse(100,0);
                
            }
            VIN[j] = 0;
            return VIN;
        }

        public static Byte[] getJ1850PWMVIN(int protocol, ComunicationManager comunication)
        {
            int i = 0;
            int sequence = 1;
            int j = 0;
            Byte[] buffer = new Byte[42];
            Byte[] VIN = new Byte[20];
            ArrayExtensions.Fill(VIN,0,VIN.Length-1,(byte)0);
            ComunicationManager.PutTaskDelay(100);
            if((buffer = executeSendReceiveMode09(protocol,0x02, new VinComunication(comunication))) != null)  //era 3
            {
                executeSendReceiveHC12(0x00c8,1, new VinComunication(comunication));
                return buffer;
            }
            while (i < 40)
            {
                if (buffer[5] == 0x7f)
                    executeSendReceiveHC12(0x00c8, 1, new VinComunication(comunication));
                i++;
                if (buffer[3] == 0x6b && buffer[5] == 0x49 && buffer[6] == 0x02 && buffer[7] == sequence)
                {
                    if (buffer[8] != 0xff) VIN[j++] = buffer[8];
                    if (buffer[9] != 0xff) VIN[j++] = buffer[9];
                    if (buffer[10] != 0xff) VIN[j++] = buffer[10];
                    if (buffer[11] != 0xff) VIN[j++] = buffer[11];
                    if (++sequence == 6) break;
                }
                if (comunication.GetLastResponse(350,0).Length != 42)
                    break;
                ComunicationManager.PutTaskDelay(500);
            }
            VIN[j] = 0;
            executeSendReceiveHC12(0x00c8,1, new VinComunication(comunication));
            return VIN;
        }

        public static Byte[] getJ1850VPWVIN(int p, ComunicationManager comunication)
        {
            Byte[] VIN = new Byte[20];
            ArrayExtensions.Fill(VIN, 0, VIN.Length - 1, (byte)00);  // si esto llegase a fallar voy a tener que buscar los fill y cambiarlos
            int i = 0;
            int sequence = 1;
            int j = 0;
            Byte[] buffer = new Byte[30];
            Byte[] bufferParts = new Byte[30];
            VinComunication vincom = new VinComunication(comunication);
            byte intentos = 0;
            do {
                if (intentos > 0)
                    ComunicationManager.PutTaskDelay(100);
                Byte[] output = vincom.genericMode09(0x02, p);
                buffer = comunication.GetLastResponse(output, 700, 1);
                intentos++;
            } while (buffer.Length != 14 || intentos < 3);
            while (i < 10)
            {
                if (buffer[5] == 0x7f) {
                    comunication.LogError("Comunication Fail");
                }
                if (buffer[0] == 0 && buffer[1] == 0)
                    break;
                i++;

                if (buffer[3] == 0x6b && buffer[5] == 0x49 && buffer[6] == 0x02 && buffer[7] == sequence)
                {
                    if (sequence != 1)
                    {
                        if (buffer[8] != 0xff) VIN[j++] = buffer[8];
                        if (buffer[9] != 0xff) VIN[j++] = buffer[9];
                        if (buffer[10] != 0xff) VIN[j++] = buffer[10];
                    }
                    if (buffer[11] != 0xff)
                        VIN[j++] = buffer[11];
                    if (++sequence == 6)
                        break;
                    ComunicationManager.PutTaskDelay(250);
                }
                VIN[j] = 0;
                return VIN;
            }
            return null;
        }

        private static Byte[] executeSendReceiveMode09(int p, int SID, VinComunication vincom)
        {
            return executeSendReceiveMode09(p,SID,1,vincom);
        }

        private static Byte[] executeSendReceiveMode09(int p, int SID, int num, VinComunication vincom)
        {
           Byte[] output = vincom.genericMode09(SID, p);
                return vincom.sendAndReceiveMessage(output, num, false,p);
        }

        static Byte[] executeSendReceiveHC12(int SID, int num, VinComunication vincom)
        {
            Byte[] output = vincom.genericModefd(SID);
            return vincom.HC12SendAndReceiveMessage(output, num, false);
        }

    }
}
