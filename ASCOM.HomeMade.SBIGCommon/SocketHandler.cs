using Newtonsoft.Json;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SocketHandler
    {
        private class Exposure
        {
            public SBIG.CCD_REQUEST ccd;
            public DateTime start;
            public uint duration;
        }

        private static int SOCKETTIMEOUT = 10 * 60 * 60; // In seconds
        private static bool lockAccess = false;
        private Debug debug = null;
        private const string driverID = "ASCOM.HomeMade.SBIGImagingCamera.SocketHandler";
        private TcpClient client = null;
        private int clNo = 0;
        private SBIGHandler server = new SBIGHandler(driverID);
        private static Dictionary<SBIG.CCD_REQUEST, Exposure> exposures = new Dictionary<SBIG.CCD_REQUEST, Exposure>();
        private DateTime lastSignal = DateTime.Now;
        bool endCommunication = false;
        private BackgroundWorker bw = null;

        public SocketHandler(TcpClient c, int count)
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGSocketHandler_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            debug.LogMessage("ImageTakerThread", "Starting initialisation");

            debug.LogMessage(driverID);
            client = c;
            clNo = count;

            bw = new BackgroundWorker();
            bw.DoWork += bw_SocketTimeout;
            bw.RunWorkerAsync();

            debug.LogMessage("ImageTakerThread", "Completed initialisation");

        }
        private void bw_SocketTimeout(object sender, DoWorkEventArgs e)
        {
            while (!endCommunication)
            {
                if (lastSignal + TimeSpan.FromSeconds(SOCKETTIMEOUT) < DateTime.Now)
                {
                    debug.LogMessage("bw_SocketTimeout", "Socket has been inactive since " + lastSignal.ToLongTimeString());
                    endCommunication = true;
                }
                Thread.Sleep(5000);
            }
        }

        public void Handle()
        {
            try
            {
                Byte[] sendBytes = null;
                int requestCount = 0;


                using (NetworkStream networkStream = client.GetStream())
                {

                    while (!endCommunication)
                    {
                        if (networkStream.DataAvailable)
                        {
                            requestCount++;
                            string dataFromClient = null;
                            while (networkStream.DataAvailable)
                            {
                                byte[] bytesFrom = new byte[client.Available];
                                networkStream.Read(bytesFrom, 0, (int)client.Available);
                                dataFromClient += System.Text.Encoding.UTF8.GetString(bytesFrom);
                            }
                            if (!String.IsNullOrEmpty(dataFromClient))
                            {
                                lastSignal = DateTime.Now;
                                //Console.WriteLine(" >> " + "From client-" + clNo + dataFromClient);
                                string[] d = dataFromClient.Split(new string[] { "<EOM>" }, StringSplitOptions.RemoveEmptyEntries);
                                //dataFromClient = null;

                                foreach (string command in d)
                                {
                                    string cleanCommand = command.Replace("\0", "");
                                    if (!String.IsNullOrEmpty(cleanCommand))
                                    {
                                        if (cleanCommand == "BYE")
                                        {
                                            sendBytes = Encoding.UTF8.GetBytes("BYE" + "<EOM>");
                                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                                            networkStream.Flush();
                                            sendBytes = null;
                                            endCommunication = true;
                                        }
                                        else
                                        {
                                            string response = HandleMessage(cleanCommand);

                                            sendBytes = Encoding.UTF8.GetBytes(response + "<EOM>");
                                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                                            networkStream.Flush();
                                            sendBytes = null;
                                            response = null;
                                            //Console.WriteLine(" << " + response);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                debug.LogMessage("bw_DoWork Thread", "Received giperish: " + dataFromClient);
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                debug.LogMessage("bw_DoWork Thread", "Leaving server loop");
            }
            catch (Exception e1)
            {
                debug.LogMessage("bw_DoWork Thread", "Error: " + Utils.DisplayException(e1));
            }

        }

        private string HandleMessage(string message)
        {
            SBIGResponse response = new SBIGResponse();
            try
            {
                SBIGRequest request = JsonConvert.DeserializeObject<SBIGRequest>(message);
                debug.LogMessage("SBIGService HandleMessage", "Message is " + request.type);
                debug.LogMessage(message);

                switch (request.type)
                {
                    case "Ping":
                        response.payload = JsonConvert.SerializeObject(true);
                        break;
                    case "Connect":
                        string ip = "";
                        if (!String.IsNullOrEmpty(request.parameters))
                            ip = JsonConvert.DeserializeObject<string>(request.parameters);
                        response.payload = JsonConvert.SerializeObject(server.Connect(ip));
                        break;
                    case "Disconnect":
                        server.Disconnect();
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "AbortExposure":
                        SBIG.StartExposureParams2 p0 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        if (!exposures.Keys.Contains(p0.ccd)) exposures.Remove(p0.ccd);
                        try
                        {
                            Utils.AcquireLock(ref lockAccess);
                            server.AbortExposure(p0);
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        finally
                        {
                            Utils.ReleaseLock(ref lockAccess);
                        }
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "CC_SET_TEMPERATURE_REGULATION2":
                        SBIG.SetTemperatureRegulationParams2 p1 = JsonConvert.DeserializeObject<SBIG.SetTemperatureRegulationParams2>(request.parameters);
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p1);
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "CC_START_EXPOSURE2":
                        SBIG.StartExposureParams2 p2 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        Exposure exposure = new Exposure() { ccd = p2.ccd, start = DateTime.Now, duration = p2.exposureTime };
                        if (!exposures.Keys.Contains(p2.ccd)) exposures.Remove(p2.ccd);
                        exposures[p2.ccd] = exposure;
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p2);
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "CC_QUERY_TEMPERATURE_STATUS":
                        SBIG.QueryTemperatureStatusParams p3 = JsonConvert.DeserializeObject<SBIG.QueryTemperatureStatusParams>(request.parameters);
                        SBIG.QueryTemperatureStatusResults2 result3 = new SBIG.QueryTemperatureStatusResults2();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p3, out result3);
                        response.payload = JsonConvert.SerializeObject(result3);
                        break;
                    case "CCD_INFO":
                        SBIG.GetCCDInfoParams p4 = JsonConvert.DeserializeObject<SBIG.GetCCDInfoParams>(request.parameters);
                        SBIG.GetCCDInfoResults0 result4 = new SBIG.GetCCDInfoResults0();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p4, out result4);
                        response.payload = JsonConvert.SerializeObject(result4);
                        break;
                    case "CCD_INFO_EXTENDED":
                        SBIG.GetCCDInfoParams p5 = JsonConvert.DeserializeObject<SBIG.GetCCDInfoParams>(request.parameters);
                        SBIG.GetCCDInfoResults2 result5 = new SBIG.GetCCDInfoResults2();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p5, out result5);
                        response.payload = JsonConvert.SerializeObject(result5);
                        break;
                    case "CCD_INFO_EXTENDED2":
                        SBIG.GetCCDInfoParams p6 = JsonConvert.DeserializeObject<SBIG.GetCCDInfoParams>(request.parameters);
                        SBIG.GetCCDInfoResults4 result6 = new SBIG.GetCCDInfoResults4();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p6, out result6);
                        response.payload = JsonConvert.SerializeObject(result6);
                        break;
                    case "CCD_INFO_EXTENDED3":
                        SBIG.GetCCDInfoParams p7 = JsonConvert.DeserializeObject<SBIG.GetCCDInfoParams>(request.parameters);
                        SBIG.GetCCDInfoResults6 result7 = new SBIG.GetCCDInfoResults6();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p7, out result7);
                        response.payload = JsonConvert.SerializeObject(result7);
                        break;
                    case "EndReadout":
                        SBIG.CCD_REQUEST ccd = JsonConvert.DeserializeObject<SBIG.CCD_REQUEST>(request.parameters);
                        if (!exposures.Keys.Contains(ccd)) exposures.Remove(ccd);
                        server.EndReadout(ccd);
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "ExposureInProgress":
                        bool temp = false;
                        SBIG.StartExposureParams2 p11 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        if (exposures.Keys.Contains(p11.ccd))
                        {
                            if (DateTime.Now < (exposures[p11.ccd].start + new TimeSpan((long)exposures[p11.ccd].duration * TimeSpan.TicksPerSecond / 100)))
                                temp = true;
                        }
                        //bool inProgress = server.ExposureInProgress();
                        response.payload = JsonConvert.SerializeObject(temp);
                        break;
                    case "ReadoutData":
                        SBIG.StartExposureParams2 p8 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        if (!exposures.Keys.Contains(p8.ccd)) exposures.Remove(p8.ccd);
                        var data = new UInt16[p8.height, p8.width];
                        try
                        {
                            Utils.AcquireLock(ref lockAccess);
                            server.ReadoutDataAndEnd(p8, ref data);
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        finally
                        {
                            Utils.ReleaseLock(ref lockAccess);
                        }
                        byte[] byteArray = new byte[data.Length * 2];
                        Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 2);
                        data = null;
                        byte[] compressedArray = Utils.Compress(byteArray);
                        byteArray = null;
                        response.payload = JsonConvert.SerializeObject(Convert.ToBase64String(compressedArray, 0, compressedArray.Length, Base64FormattingOptions.None));
                        break;
                    case "CC_CFW":
                        SBIG.CFWParams p9 = JsonConvert.DeserializeObject<SBIG.CFWParams>(request.parameters);
                        SBIG.CFWResults result9 = new SBIG.CFWResults();
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p9, out result9);
                        response.payload = JsonConvert.SerializeObject(result9);
                        break;
                    default:
                        throw new NotImplementedException("Message type " + request.type + " not implemented");
                        break;
                }
                string jsonReponse = JsonConvert.SerializeObject(response);
                response = null;
                debug.LogMessage("SBIGService HandleMessage", "Done " + request.type);
                return jsonReponse;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(e));
                response.error = e;
                return JsonConvert.SerializeObject(response);
            }
        }

    }
}
