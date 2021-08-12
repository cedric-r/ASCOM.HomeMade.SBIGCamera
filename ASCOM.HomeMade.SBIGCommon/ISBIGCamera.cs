using ASCOM.DeviceInterface;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class ISBIGCamera
    {
        public SBIG.CAMERA_TYPE CameraModel;
        public SBIG.CCD_REQUEST CameraType;
        public bool RequiresExposureParams2 = true;
        public SBIG.READOUT_BINNING_MODE Binning = 0;
        public CameraStates CurrentCameraState = CameraStates.cameraIdle;
        public double durationRequest = 0;
        public bool lightRequest = false;

        public int cameraNumX = 0;
        public int cameraNumY = 0;
        public int cameraStartX = 0;
        public int cameraStartY = 0;
        public bool cameraImageReady = false;
        public int[,] cameraImageArray;
        public object[,] cameraImageArrayVariant;
        public bool FastReadoutRequested = false;
    }
}
