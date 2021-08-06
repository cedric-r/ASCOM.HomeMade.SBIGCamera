using ASCOM.DeviceInterface;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class ImageTakerThread
    {
        private Debug debug = null;
        private const string driverID = "ASCOM.HomeMade.SBIGCamera.ImageTaker";
        private SBIGClient server = null;
        private ISBIGCamera camera = null;
        protected static object lockCameraImaging = new object();
        public bool StopExposure = false;

        public ImageTakerThread(ISBIGCamera callerCamera, SBIGClient client)
        {
            camera = callerCamera;
            server = client;
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGImageTaker_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            debug.LogMessage("ImageTaker", "Starting initialisation");

            debug.LogMessage(driverID);

            debug.LogMessage("ImageTaker", "Completed initialisation");
        }

        public void TakeImage()
        {
            try
            {
                if (StopExposure) return;
                debug.LogMessage("TakeImage", "Starting imaging worker");

                if (!camera.RequiresExposureParams2)
                {
                    debug.LogMessage("TakeImage", "This driver does not support legacy calls. Using new call anyway");
                    //throw new InvalidOperationException("This driver does not support legacy calls");
                }
                SBIG.StartExposureParams2 exposureParams2 = new SBIG.StartExposureParams2
                {
                    ccd = camera.CameraType,
                    abgState = SBIG.ABG_STATE7.ABG_LOW7,
                    openShutter = camera.lightRequest ? SBIG.SHUTTER_COMMAND.SC_OPEN_SHUTTER : SBIG.SHUTTER_COMMAND.SC_CLOSE_SHUTTER,
                    readoutMode = camera.Binning,
                    exposureTime = camera.FastReadoutRequested ? Convert.ToUInt32(camera.durationRequest * 100) | SBIG.EXP_FAST_READOUT : Convert.ToUInt32(camera.durationRequest * 100),
                    width = Convert.ToUInt16(camera.cameraNumX), // This is in binned pixels. Check is this is right
                    height = Convert.ToUInt16(camera.cameraNumY),
                    left = (ushort)camera.cameraStartX,
                    top = (ushort)camera.cameraStartY
                };

                debug.LogMessage("TakeImage", "Exposing");
                camera.CurrentCameraState = CameraStates.cameraExposing;
                server.CC_START_EXPOSURE2(exposureParams2);

                if (StopExposure) return;

                // read out the image
                debug.LogMessage("TakeImage", "Waiting");
                camera.CurrentCameraState = CameraStates.cameraReading;

                if (StopExposure) return;

                //cameraImageArray = SBIG.WaitEndAndReadoutExposure32(exposureParams2);
                while (server.ExposureInProgress()) 
                {
                    if (StopExposure) return;
                    Thread.Sleep(200); 
                }

                if (StopExposure) return;

                lock (lockCameraImaging)
                {
                    debug.LogMessage("TakeImage", "Reading");
                    var data = server.ReadoutData(exposureParams2);

                    if (StopExposure) return;

                    camera.cameraImageArray = new UInt32[exposureParams2.height, exposureParams2.width];
                    for (int i = 0; i < exposureParams2.height; i++)
                        for (int j = 0; j < exposureParams2.width; j++)
                            camera.cameraImageArray[i, j] = data[i, j];

                    debug.LogMessage("TakeImage", "Finishing readout");
                }
                camera.CurrentCameraState = CameraStates.cameraIdle;
                debug.LogMessage("TakeImage", "Done");
                camera.cameraImageReady = true;
            }
            catch (Exception e)
            {
                debug.LogMessage("TakeImage", "Error: " + Utils.DisplayException(e));
                throw;
            }

        }

    }
}
