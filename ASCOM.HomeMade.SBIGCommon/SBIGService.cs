using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGService
    {
        private const string driverID = "ASCOM.HomeMade.SBIGService";
        public const string _Url = "tcp://127.0.0.1:5557";
        private static BackgroundWorker bw = null;
        private static SBIGService _Instance = null;
        private NetMQPoller _Poller = null;
        private static Debug debug = null;
        private string Url = "";
        private SBIGHandler server = new SBIGHandler(driverID);

        public static bool Shutdown = false;

        #region Service management
        public static SBIGService CreateService(string url)
        {
            if (debug == null)
            {
                string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                strPath = Path.Combine(strPath, driverID);
                try
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }
                catch (Exception) { }
                debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGService_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));
            }
            if (_Instance == null)
            {
                debug.LogMessage("SBIGService", "Creating instance");
                _Instance = new SBIGService(url);
            }
            debug.LogMessage("SBIGService", "Returning instance");
            return _Instance;
        }

        public static SBIGService CreateService()
        {
            return CreateService("@"+_Url);
        }

        private SBIGService(string url)
        {
            Url = url;

            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (var rep1 = new ResponseSocket(Url))
                using (_Poller = new NetMQPoller { rep1 })
                {
                    rep1.ReceiveReady += (s, a) =>
                    {
                        string msg = a.Socket.ReceiveFrameString();
                        string response = HandleMessage(msg);
                        a.Socket.SendFrame(response);
                    };
                    _Poller.Run();
                }
            }
            catch(Exception ex)
            {
                debug.LogMessage("SBIGService dowork", "Error: "+Utils.DisplayException(ex));
            }
        }

        private string HandleMessage(string message)
        {
            SBIGResponse response = new SBIGResponse();
            try
            {
                SBIGRequest request = JsonConvert.DeserializeObject<SBIGRequest>(message);
                debug.LogMessage("HandleMessage", "Message is " + request.type);

                switch (request.type)
                {
                    case "Connect":
                        response.payload = JsonConvert.SerializeObject(server.Connect());
                        break;
                    case "Disconnect":
                        server.Disconnect();
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "StopExposure":
                        if (JsonConvert.DeserializeObject<object>(request.parameters) == null)
                        {
                            response.payload = JsonConvert.SerializeObject(server.stopExposure);
                        }
                        else
                        {
                            server.stopExposure = JsonConvert.DeserializeObject<bool>(request.parameters);
                            response.payload = JsonConvert.SerializeObject(null);
                        }
                        break;
                    case "AbortExposure":
                        SBIG.StartExposureParams2 p0 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        server.AbortExposure(p0);
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "CC_SET_TEMPERATURE_REGULATION2":
                        SBIG.SetTemperatureRegulationParams2 p1 = JsonConvert.DeserializeObject<SBIG.SetTemperatureRegulationParams2>(request.parameters);
                        server.UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p1);
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "CC_START_EXPOSURE2":
                        SBIG.StartExposureParams2 p2 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
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
                        server.EndReadout(JsonConvert.DeserializeObject<SBIG.CCD_REQUEST>(request.parameters));
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "WaitExposure":
                        server.WaitExposure();
                        response.payload = JsonConvert.SerializeObject(null);
                        break;
                    case "ExposureInProgress":
                        bool inProgress = server.ExposureInProgress();
                        response.payload = JsonConvert.SerializeObject(inProgress);
                        break;
                    case "ReadoutData":
                        SBIG.StartExposureParams2 p8 = JsonConvert.DeserializeObject<SBIG.StartExposureParams2>(request.parameters);
                        var data = new UInt16[p8.height, p8.width];
                        server.ReadoutData(p8, ref data);
                        byte[] byteArray = new byte[data.Length * 2];
                        Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 2);
                        data = null;
                        GC.Collect();
                        byte[] compressedArray = Utils.Compress(byteArray);
                        byteArray = null;
                        GC.Collect();
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
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(e));
                response.error = e;
                return JsonConvert.SerializeObject(response);
            }
        }

        public void Stop()
        {
            debug.LogMessage("SBIGService", "Stopping service");
            _Poller.Stop();
            try
            {
                bw.CancelAsync();
            }
            catch (Exception) { }
        }

        #endregion
    }
}
