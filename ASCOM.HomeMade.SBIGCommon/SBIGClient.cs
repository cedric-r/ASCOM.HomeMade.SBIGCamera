using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGClient
    {
        private const string driverID = "ASCOM.HomeMade.SBIGService";
        private Debug debug = null;
        private SBIGService service = null;
        private string Url = "";
        private RequestSocket socket;
        public SBIGClient(string url = "")
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGClient_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            Url = url;

            if (String.IsNullOrEmpty(url)) Url = SBIGService._Url;

            ConnectToServer();
        }

        private void CreateService()
        {
            if (service == null) service = SBIGService.CreateService(Url);
        }

        private bool CheckServerPresence()
        {
            return PingServer();
        }

        public bool ConnectToServer()
        {
            try
            {
                if (socket == null) socket = new RequestSocket(">" + Url);

                if (!CheckServerPresence())
                {
                    debug.LogMessage("SBIGClient", "Can't connect to server. Trying to spin one up");
                    if (socket != null) try { socket.Close(); } catch (Exception) { }

                    // Create a new server, we can't find one. This essentially acts as a server election among the clients
                    CreateService();

                    try
                    {
                        debug.LogMessage("SBIGClient", "Checking new connection");
                        if (socket != null) try { socket.Close(); } catch (Exception) { }
                        socket = new RequestSocket(">" + Url);
                        return CheckServerPresence();
                    }
                    catch (Exception ex)
                    {
                        debug.LogMessage("SBIGClient", "Can't connect to server.");
                        debug.LogMessage("SBIGClient", "Error: " + Utils.DisplayException(ex));
                        throw;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient", "Can't connect to server. Trying to spin one up");
                debug.LogMessage("SBIGClient", "Error: " + Utils.DisplayException(e));

                CreateService();
                try
                {
                    socket = new RequestSocket(">" + Url);
                    return CheckServerPresence();
                }
                catch (Exception ex)
                {
                    debug.LogMessage("SBIGClient", "Can't connect to server.");
                    debug.LogMessage("SBIGClient", "Error: " + Utils.DisplayException(ex));
                    throw;
                }
            }
        }

        public bool IsConnected 
        { get
            {
                {
                    if (socket == null)
                    {
                        return false;
                    }
                    if (!socket.HasOut)
                    {
                        // We're disconnected but we used to be connected
                        socket = null;
                        return false;
                    }
                    return true;
                }
            }
        }

        public void Close()
        {
            if (socket != null) socket.Close();
        }

        #region Communication with service
        private object lockAccess = new object();
        private string SendMessageToServer(string message)
        {
            string temp = "";
            try
            {
                if (!IsConnected)
                {
                    throw new ApplicationException("Client is not connected to server");
                }
                lock (lockAccess)
                {
                    socket.SendFrame(message);
                    bool more = false;
                    bool r = socket.TryReceiveFrameString(new TimeSpan(0, 0, 10), System.Text.Encoding.UTF8, out temp, out more);
                    if (!r)
                    {
                        // Communication error!
                        throw new ApplicationException("Can't read from server");
                    }
                }
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient SendMessage", "Error: " + Utils.DisplayException(e));
                throw;
            }
            return temp;
        }

        private SBIGResponse SendMessage(string type, SBIG.PAR_COMMAND command = 0, object payload = null)
        {
            try
            {
                SBIGRequest request = new SBIGRequest() { type = type, command = (ushort)command, parameters = JsonConvert.SerializeObject(payload) };
                string results = SendMessageToServer(JsonConvert.SerializeObject(request));
                SBIGResponse response = JsonConvert.DeserializeObject<SBIGResponse>(results);
                return response;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient SendMessage", "Error: " + Utils.DisplayException(e));
                throw;
            }
        }

        public bool PingServer()
        {
            try
            {
                SBIGResponse response = SendMessage("Ping");
                return (bool)JsonConvert.DeserializeObject<bool>(response.payload);
            }
            catch(Exception)
            {
                return false;
            }
        }

        #endregion
        #region Camera calls implementation
        public bool Connect()
        {
            debug.LogMessage("SBIGClient", "Connect");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("Connect");
            if (response.error != null) throw response.error;
            return (bool)JsonConvert.DeserializeObject<bool>(response.payload);
        }

        public void Disconnect()
        {
            debug.LogMessage("SBIGClient", "Disconnect");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("Disconnect");
            if (response.error != null) throw response.error;
        }

        public void CC_SET_TEMPERATURE_REGULATION2(SBIG.SetTemperatureRegulationParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "CC_SET_TEMPERATURE_REGULATION2");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_SET_TEMPERATURE_REGULATION2", SBIG.PAR_COMMAND.CC_SET_TEMPERATURE_REGULATION2, tparam);
            if (response.error != null) throw response.error;
        }

        public void CC_START_EXPOSURE2(SBIG.StartExposureParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "CC_START_EXPOSURE2");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_START_EXPOSURE2", SBIG.PAR_COMMAND.CC_START_EXPOSURE2, tparam);
            if (response.error != null) throw response.error;
        }

        public SBIG.QueryTemperatureStatusResults2 CC_QUERY_TEMPERATURE_STATUS()
        {
            debug.LogMessage("SBIGClient", "CC_QUERY_TEMPERATURE_STATUS");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_QUERY_TEMPERATURE_STATUS", SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS, new SBIG.QueryTemperatureStatusParams()
            {
                request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
            });
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.QueryTemperatureStatusResults2>(response.payload);
        }

        public SBIG.GetCCDInfoResults0 CCD_INFO(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.GetCCDInfoResults0>(response.payload);
        }

        public SBIG.GetCCDInfoResults2 CCD_INFO_EXTENDED(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.GetCCDInfoResults2>(response.payload);
        }

        public SBIG.GetCCDInfoResults4 CCD_INFO_EXTENDED2(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED2");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED2", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.GetCCDInfoResults4>(response.payload);
        }

        public SBIG.GetCCDInfoResults6 CCD_INFO_EXTENDED3(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED3");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED3", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.GetCCDInfoResults6>(response.payload);
        }

        public void StopExposure(bool state)
        {
            debug.LogMessage("SBIGClient", "StopExposure(state)");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("StopExposure", 0, state);
            if (response.error != null) throw response.error;
        }

        public bool StopExposure()
        {
            debug.LogMessage("SBIGClient", "StopExposure");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("StopExposure");
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<bool>(response.payload);
        }

        public void AbortExposure(SBIG.StartExposureParams2 sep2)
        {
            debug.LogMessage("SBIGClient", "AbortExposure");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("AbortExposure", 0, sep2);
            if (response.error != null) throw response.error;
        }

        public void EndReadout(SBIG.CCD_REQUEST ccd)
        {
            debug.LogMessage("SBIGClient", "EndReadout");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("EndReadout", 0, ccd);
            if (response.error != null) throw response.error;
        }

        public bool ExposureInProgress()
        {
            debug.LogMessage("SBIGClient", "ExposureInProgress");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("ExposureInProgress");
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<bool>(response.payload);
        }

        public void WaitExposure()
        {
            debug.LogMessage("SBIGClient", "WaitExposure");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("WaitExposure");
            if (response.error != null) throw response.error;
        }

        public UInt16[,] ReadoutData(SBIG.StartExposureParams2 sep2)
        {
            debug.LogMessage("SBIGClient", "ReadoutData");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("ReadoutData", 0, sep2);
            if (response.error != null) throw response.error;
            //return JsonConvert.DeserializeObject<UInt16[,]>(response.payload);
            byte[] array = Convert.FromBase64String(JsonConvert.DeserializeObject<string>(response.payload));
            byte[] deCompressedArray = Utils.Decompress(array);
            array = null;
            GC.Collect();
            ushort[,] target = new ushort[sep2.height, sep2.width];
            Buffer.BlockCopy(deCompressedArray, 0, target, 0, deCompressedArray.Length);
            deCompressedArray = null;
            GC.Collect();
            return target;
        }
        #endregion

        #region FW calls implementation
        public SBIG.CFWResults CC_CFW(SBIG.CFWParams tparams)
        {
            debug.LogMessage("SBIGClient", "CC_CFW");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_CFW", SBIG.PAR_COMMAND.CC_CFW, tparams);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<SBIG.CFWResults>(response.payload);
        }

        #endregion
    }
}
