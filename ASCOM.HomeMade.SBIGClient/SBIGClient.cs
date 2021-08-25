/**
 * ASCOM.HomeMade.SBIGCamera - SBIG camera driver
 * Copyright (C) 2021 Cedric Raguenaud [cedric@raguenaud.earth]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using ASCOM.HomeMade.SBIGCommon;
using ASCOM.HomeMade.SBIGHub;
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

namespace ASCOM.HomeMade.SBIGClient
{
    public class SBIGClient
    {
        private const string driverID = "ASCOM.HomeMade.SBIGClient";
        private Debug debug = null;
        private string IPAddress = "";
        public bool IsConnected { get; private set; } = false;

        public SBIGClient()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new Debug(driverID, Path.Combine(strPath, "SBIGClient_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            try
            {
                if (IsConnected)
                {
                    SharedResources.DisConnect();
                    IsConnected = false;
                }
            }
            catch (Exception) { }
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

        private SBIGResponse SendMessage(string type, SBIG.PAR_COMMAND command = (SBIG.PAR_COMMAND)0, object payload = null)
        {
            try
            {
                SBIGRequest request = new SBIGRequest() { type = type, command = (ushort)command, parameters = payload };
                SBIGResponse response = SharedResources.SendMessage(request);
                return response;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGClient SendMessage", "Error: " + Utils.DisplayException(e));
                throw;
            }
        }

        private SBIGResponse SendMessage(string type) // Timeout in ms
        {
            return SendMessage(type, 0, null);
        }
        #endregion

        #region Camera calls implementation
        public bool Connect(string ipAddress)
        {
            debug.LogMessage("SBIGClient", "Connect");
            IPAddress = ipAddress;
            if (IsConnected) throw new ApplicationException("Already connected to server");
            SharedResources.Connect(IPAddress);
            IsConnected = true;
            return IsConnected;
        }

        public void Disconnect()
        {
            debug.LogMessage("SBIGClient", "Disconnect");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SharedResources.DisConnect();
            IsConnected = false;
        }

        public void CC_SET_TEMPERATURE_REGULATION2(SBIG.SetTemperatureRegulationParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "CC_SET_TEMPERATURE_REGULATION2");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_SET_TEMPERATURE_REGULATION2", SBIG.PAR_COMMAND.CC_SET_TEMPERATURE_REGULATION2, tparam);
            if (response.error != null) throw response.error;
        }

        public void CC_START_EXPOSURE2(SBIG.StartExposureParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "CC_START_EXPOSURE2");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_START_EXPOSURE2", SBIG.PAR_COMMAND.CC_START_EXPOSURE2, tparam);
            if (response.error != null) throw response.error;
        }

        public SBIG.QueryTemperatureStatusResults2 CC_QUERY_TEMPERATURE_STATUS()
        {
            debug.LogMessage("SBIGClient", "CC_QUERY_TEMPERATURE_STATUS");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_QUERY_TEMPERATURE_STATUS", SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS, new SBIG.QueryTemperatureStatusParams()
            {
                request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
            });
            if (response.error != null) throw response.error;
            return (SBIG.QueryTemperatureStatusResults2)response.payload;
        }

        public SBIG.GetCCDInfoResults0 CCD_INFO(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.GetCCDInfoResults0)response.payload;
        }

        public SBIG.GetCCDInfoResults2 CCD_INFO_EXTENDED(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.GetCCDInfoResults2)response.payload;
        }

        public SBIG.GetCCDInfoResults4 CCD_INFO_EXTENDED2(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED2");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED2", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.GetCCDInfoResults4)response.payload;
        }

        public SBIG.GetCCDInfoResults6 CCD_INFO_EXTENDED3(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED3");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED3", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.GetCCDInfoResults6)response.payload;
        }

        public SBIG.GetCCDInfoResults3 CCD_INFO_EXTENDED_PIXCEL(SBIG.GetCCDInfoParams tparams)
        {
            debug.LogMessage("SBIGClient", "CCD_INFO_EXTENDED_PIXCEL");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CCD_INFO_EXTENDED_PIXCEL", SBIG.PAR_COMMAND.CC_GET_CCD_INFO, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.GetCCDInfoResults3)response.payload;
        }

        public void AbortExposure(SBIG.StartExposureParams2 sep2)
        {
            debug.LogMessage("SBIGClient", "AbortExposure");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("AbortExposure", 0, sep2);
            if (response.error != null) throw response.error;
        }

        public void EndReadout(SBIG.CCD_REQUEST ccd)
        {
            debug.LogMessage("SBIGClient", "EndReadout");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("EndReadout", 0, ccd);
            if (response.error != null) throw response.error;
        }

        public bool ExposureInProgress(SBIG.StartExposureParams2 tparam)
        {
            debug.LogMessage("SBIGClient", "ExposureInProgress");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("ExposureInProgress", 0, tparam);
            if (response.error != null) throw response.error;
            return (bool)response.payload;
        }

        public UInt16[] ReadoutData(SBIG.StartExposureParams2 sep2)
        {
            debug.LogMessage("SBIGClient", "ReadoutData");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("ReadoutData", 0, sep2);
            if (response.error != null) throw response.error;
            return (UInt16[])response.payload;
        }
        #endregion

        #region FW calls implementation
        public SBIG.CFWResults CC_CFW(SBIG.CFWParams tparams)
        {
            debug.LogMessage("SBIGClient", "CC_CFW");
            if (!IsConnected) throw new ApplicationException("Not connected to server");
            SBIGResponse response = SendMessage("CC_CFW", SBIG.PAR_COMMAND.CC_CFW, tparams);
            if (response.error != null) throw response.error;
            return (SBIG.CFWResults)response.payload;
        }

        #endregion
    }
}
