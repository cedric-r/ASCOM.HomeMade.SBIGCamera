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
        private const string driverID = "ASCOM.HomeMade.SBIGImagingCamera.ImageTaker";
        private SBIGClient server = null;
        private ISBIGCamera camera = null;
        public bool StopExposure = false;

        public ImageTakerThread(ISBIGCamera callerCamera)
        {
            camera = callerCamera;
            server = new SBIGClient();
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGImageTaker_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            debug.LogMessage("ImageTakerThread", "Starting initialisation");

            debug.LogMessage(driverID);
            debug.LogMessage("ImageTakerThread", "Camera is "+camera.CameraType);

            debug.LogMessage("ImageTakerThread", "Completed initialisation");
        }

        private void AbortExposure(SBIG.StartExposureParams2 sep2)
        {
            try
            {
                debug.LogMessage("ImageTakerThread", "AbortExposure");
                server.AbortExposure(sep2);
                debug.LogMessage("ImageTakerThread", "Finishing readout");
                server.EndReadout(sep2.ccd);
            }
            catch(Exception e)
            {
                debug.LogMessage("ImageTakerThread TakeImage", "Error: " + Utils.DisplayException(e));
            }
        }

        public void TakeImage()
        {
            try
            {
                if (StopExposure) return;
                debug.LogMessage("ImageTakerThread TakeImage", "Starting imaging worker");

                if (!camera.RequiresExposureParams2)
                {
                    debug.LogMessage("ImageTakerThread TakeImage", "This driver does not support legacy calls. Using new call anyway");
                    //throw new InvalidOperationException("This driver does not support legacy calls");
                }
                SBIG.StartExposureParams2 exposureParams2 = new SBIG.StartExposureParams2()
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

                debug.LogMessage("ImageTakerThread TakeImage", "Exposing");
                camera.CurrentCameraState = CameraStates.cameraExposing;
                server.CC_START_EXPOSURE2(exposureParams2);

                if (StopExposure) {
                    AbortExposure(exposureParams2);
                    return;
                }

                // read out the image
                debug.LogMessage("ImageTakerThread TakeImage", "Waiting");
                camera.CurrentCameraState = CameraStates.cameraReading;

                if (StopExposure)
                {
                    AbortExposure(exposureParams2);
                    return;
                }

                //cameraImageArray = SBIG.WaitEndAndReadoutExposure32(exposureParams2);
                while (server.ExposureInProgress(exposureParams2)) 
                {
                    if (StopExposure)
                    {
                        AbortExposure(exposureParams2);
                        return;
                    }
                    Thread.Sleep(200); 
                }

                if (StopExposure) return;

                debug.LogMessage("ImageTakerThread TakeImage", "Reading");
                var data = server.ReadoutData(exposureParams2);

                if (StopExposure) return;

                camera.cameraImageArray = new int[exposureParams2.height, exposureParams2.width];
                for (int i = 0; i < exposureParams2.height; i++)
                    for (int j = 0; j < exposureParams2.width; j++)
                        camera.cameraImageArray[i, j] = data[i, j];

                data = null;
                debug.LogMessage("ImageTakerThread TakeImage", "Finishing readout");

                camera.CurrentCameraState = CameraStates.cameraIdle;
                debug.LogMessage("ImageTakerThread TakeImage", "Done");
                camera.cameraImageReady = true;
            }
            catch (Exception e)
            {
                debug.LogMessage("ImageTakerThread TakeImage", "Error: " + Utils.DisplayException(e));
                throw;
            }
            finally
            {
                server.Close();
            }
        }

    }
}
