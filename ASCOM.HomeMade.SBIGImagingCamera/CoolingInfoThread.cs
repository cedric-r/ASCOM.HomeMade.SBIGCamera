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
        private SBIGClient server = null;
        protected static object lockCameraImaging = new object();
        public bool StopExposure = false;

        public CoolingInfoThread()
        {
            server = new SBIGClient();
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
            server.Close();
        }
    }
}
