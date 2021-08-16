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
using ASCOM.DeviceInterface;
using ASCOM.HomeMade.SBIGCommon;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGImagingCamera
{
    public class CoolingInfoThread
    {
        private Debug debug = null;
        private const string driverID = "ASCOM.HomeMade.SBIGCamera.CoolingInfoThread";
        private SBIGClient.SBIGClient server = null;
        protected static object lockCameraImaging = new object();
        private string IPAddress = "";
        public bool StopExposure = false;

        public CoolingInfoThread(string ipAddress)
        {
            IPAddress = ipAddress;
            server = new SBIGClient.SBIGClient();
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGCoolingInfo_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            debug.LogMessage("CoolingInfoThread", "Starting initialisation");

            debug.LogMessage(driverID);
            server.Connect(IPAddress);

            debug.LogMessage("CoolingInfoThread", "Completed initialisation");
        }

        public SBIG.QueryTemperatureStatusResults2 GetCoolingInfo()
        {
            try
            {
                debug.LogMessage("CoolingInfoThread GetCoolingInfo", "Getting cooling information");
                // query temperature
                return  server.CC_QUERY_TEMPERATURE_STATUS();
            }
            catch (Exception e)
            {
                debug.LogMessage("CoolingInfoThread GetCoolingInfo", "Error: " + Utils.DisplayException(e));
                throw new ASCOM.DriverException(Utils.DisplayException(e));
            }
        }

        public void Close()
        {
            debug.LogMessage("CoolingInfoThread Close", "Closing client.");
            try
            {
                server.Disconnect();
            }
            catch(Exception e)
            {
                debug.LogMessage("CoolingInfoThread Close", "Error: "+Utils.DisplayException(e));
            }
        }
    }
}
