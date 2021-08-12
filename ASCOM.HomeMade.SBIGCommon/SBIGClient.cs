using Newtonsoft.Json;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGClient
    {
        private const string driverID = "ASCOM.HomeMade.SBIGService";
        private Debug debug = null;
        private string Url = "";
        private TcpClient clientSocket = new TcpClient();
        private NetworkStream serverStream = null;

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
            debug.LogMessage("SBIGClient CreateService", "Service should already be running as a Windows Service");
        }

        private bool CheckServerPresence()
        {
            return PingServer();
        }

        public bool CheckSocket()
        {
            if (!clientSocket.Connected)
            {
                debug.LogMessage("CheckSocket", "Start client");
                clientSocket = new TcpClient();
                clientSocket.Connect(System.Net.IPAddress.Loopback, 5557);
                serverStream = clientSocket.GetStream();
            }
            return clientSocket.Connected;
        }

        public bool ConnectToServer()
        {
            debug.LogMessage("SBIGClient ConnectToServer", "Connecting...");
            try
            {
                bool temp = false;
                temp = CheckSocket();
                return temp;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient ConnectToServer", "Error: " + Utils.DisplayException(e));
                throw;
            }
        }

        public bool IsConnected 
        { get
            {
                if (!clientSocket.Connected)
                {
                    debug.LogMessage("SBIGClient IsConnected", "Socket is not connected.");
                    return false;
                }

                return true;
            }
        }

        public void Dispose()
        {
            if (clientSocket != null)
            {
                try
                {
                    if (IsConnected)
                        Close();
                }
                catch (Exception) { }
            }

        }

        public void Close()
        {
            if (clientSocket.Connected)
            {
                try
                {
                    debug.LogMessage("SBIGClient Close", "Closing socket.");
                    byte[] outStream = System.Text.Encoding.UTF8.GetBytes("BYE" + "<EOM>");
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    serverStream.Close();
                }
                catch (Exception) { }
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        #region Communication with service
        private bool lockAccess = false;
        private string SendMessageToServerWithTimeout(string message, int timeout = 0) // Timeout is in ms
        {
            string temp = "";
            try
            {
                byte[] outStream = System.Text.Encoding.UTF8.GetBytes(message+"<EOM>");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                DateTime start = DateTime.Now;
                while (!serverStream.DataAvailable && ((start + TimeSpan.FromMilliseconds(timeout)) > DateTime.Now)) Thread.Sleep(10);

                if (serverStream.DataAvailable)
                {
                    while (serverStream.DataAvailable)
                    {
                        byte[] bytesFrom = new byte[clientSocket.Available];
                        serverStream.Read(bytesFrom, 0, (int)clientSocket.Available);
                        temp += System.Text.Encoding.UTF8.GetString(bytesFrom);
                        bytesFrom = null;
                    }
                    //Console.WriteLine(" >> " + "From server-" + temp);
                    temp = temp.Substring(0, temp.IndexOf("<EOM>"));
                }
                else
                {
                    // It must have been a timeout
                    throw new TimeoutException("Timeout reading from service");
                }
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient SendMessage", "Error: " + Utils.DisplayException(e));
                throw;
            }
            return temp;
        }

        private SBIGResponse SendMessage(string type, SBIG.PAR_COMMAND command = 0, object payload = null, int timeout = 60000)
        {
            try
            {
                SBIGRequest request = new SBIGRequest() { type = type, command = (ushort)command, parameters = JsonConvert.SerializeObject(payload) };
                string results = SendMessageToServerWithTimeout(JsonConvert.SerializeObject(request), timeout);
                SBIGResponse response = JsonConvert.DeserializeObject<SBIGResponse>(results);
                return response;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient SendMessage", "Error: " + Utils.DisplayException(e));
                throw;
            }
        }

        private SBIGResponse SendMessage(string type, int timeout) // Timeout in ms
        {
            return SendMessage(type, 0, null, timeout);
        }

        public bool PingServer()
        {
            try
            {
                SBIGResponse response = SendMessage("Ping", 2000);
                return (bool)JsonConvert.DeserializeObject<bool>(response.payload);
            }
            catch(Exception e)
            {
                return false;
            }
        }

        #endregion
        #region Camera calls implementation
        public bool Connect(string ipAddress)
        {
            debug.LogMessage("SBIGClient", "Connect");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("Connect", 0, ipAddress);
            if (response.error != null) throw response.error;
            return (bool)JsonConvert.DeserializeObject<bool>(response.payload);
        }

        public void Disconnect()
        {
            debug.LogMessage("SBIGClient", "Disconnect");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("Disconnect");
            if (response.error != null) throw response.error;
            Close();
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

        public bool ExposureInProgress(SBIG.StartExposureParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "ExposureInProgress");
            if (!ConnectToServer()) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("ExposureInProgress", 0, tparam);
            if (response.error != null) throw response.error;
            return JsonConvert.DeserializeObject<bool>(response.payload);
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
            ushort[,] target = new ushort[sep2.height, sep2.width];
            Buffer.BlockCopy(deCompressedArray, 0, target, 0, deCompressedArray.Length);
            deCompressedArray = null;
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
