using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGServer
    {
        private static bool connectionState = false;
        private static object lockSBIGAccess = new object();
        private Debug debug = null;
        private string deviceId = "";

        protected static int connections = 0;

        public bool IsConnected { get { return connectionState; } }

        public SBIGServer(string device)
        {
            deviceId = device;

            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, deviceId);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }

            debug = new Debug(deviceId, Path.Combine(strPath, "SBIGCommon_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));
        }

        public bool Connect()
        {
            debug.LogMessage("Connect", "Connection request");

            SBIG.CAMERA_TYPE CameraType; 

            SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_OPEN_DRIVER);

            bool cameraFound = false;
            debug.LogMessage("Connected Set", $"Enumerating USB cameras");
            SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_QUERY_USB, out SBIG.QueryUSBResults qur);
            for (int i = 0; i < qur.camerasFound; i++)
            {
                if (!qur.usbInfo[i].cameraFound)
                    debug.LogMessage("Connected Set", $"Cam {i}: not found");
                else
                {
                    debug.LogMessage("Connected Set",
                        $"Cam {i}: type={qur.usbInfo[i].cameraType} " +
                        $"name={ qur.usbInfo[i].name} " +
                        $"ser={qur.usbInfo[i].serialNumber}");
                    cameraFound = true;
                }
            }

            if (!cameraFound)
            {
                debug.LogMessage("Connected Set", $"No USB camera found");
                return false;
            }
            else
            {
                connectionState = true;
                connections++;

                SBIG.UnivDrvCommand(
                    SBIG.PAR_COMMAND.CC_OPEN_DEVICE,
                    new SBIG.OpenDeviceParams
                    {
                        deviceType = SBIG.SBIG_DEVICE_TYPE.DEV_USB
                    });
                CameraType = SBIG.EstablishLink();
                debug.LogMessage("Connected Set", $"Connected to USB camera");

                return true;
            }
        }

        public void Disconnect()
        {
            debug.LogMessage("Disconnect", "Disconnection requested");
            if (connectionState)
            {
                connectionState = false;
                connections--;
                if (connections < 0) connections = 0; // some software start with a disconnect

                if (connections == 0)
                {
                    try
                    {
                        debug.LogMessage("Disconnect", "Disconnecting device");
                        // clean up
                        SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DEVICE);
                        SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DRIVER);
                    }
                    catch (Exception e)
                    {
                        //debug.LogMessage("Connected", "Error: " + DisplayException(e));
                    }
                }
            }

        }

        public void UnivDrvCommand(SBIG.PAR_COMMAND command)
        {
            // make the call
            lock (lockSBIGAccess)
            {
                SBIG.UnivDrvCommand(command);
            }
        }

        public void UnivDrvCommand<TParams>(SBIG.PAR_COMMAND command, TParams Params)
            where TParams : SBIG.IParams
        {
            lock(lockSBIGAccess)
            {
                SBIG.UnivDrvCommand<TParams>(command, Params);
            }
        }

        public void UnivDrvCommand<TResults>(SBIG.PAR_COMMAND command, out TResults pResults)
            where TResults : SBIG.IResults
        {
            lock(lockSBIGAccess)
            {
                SBIG.UnivDrvCommand<TResults>(command, out pResults);
            }
        }

        public void UnivDrvCommand(SBIG.PAR_COMMAND command, SBIG.ReadoutLineParams Params, out UInt16[] data)
        {
            lock (lockSBIGAccess)
            {
                SBIG.UnivDrvCommand(command, Params, out data);
            }
        }

        public void UnivDrvCommand<TParams, TResults>(SBIG.PAR_COMMAND command, TParams Params, out TResults pResults)
            where TParams : SBIG.IParams
            where TResults : SBIG.IResults
        {
            lock (lockSBIGAccess)
            {
                SBIG.UnivDrvCommand<TParams, TResults>(command, Params, out pResults);
            }
        }
    }
}
