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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGHub
{
    public abstract class ISBIGCamera: ReferenceCountedObjectBase
    {
        public SBIG.CAMERA_TYPE CameraModel;
        public SBIG.CCD_REQUEST CameraType;
        public bool RequiresExposureParams2 = true;
        public SBIG.READOUT_BINNING_MODE Binning = 0;
        protected short binX = 1;
        protected short binY = 1;
        public CameraStates CurrentCameraState = CameraStates.cameraIdle;
        public double durationRequest = 0;
        public bool lightRequest = false;

        public int cameraNumX = 0;
        public int cameraNumY = 0;
        public int cameraStartX = 0;
        public int cameraStartY = 0;
        public bool cameraImageReady = false;
        public Array cameraImageArray;
        public object[,] cameraImageArrayVariant;
        public bool FastReadoutRequested = false;

        public short BinX
        {
            get
            {
                return GetBinX();
            }
            set
            {
                SetBinX(value);
            }
        }

        public short BinY
        {
            get
            {
                return GetBinY();
            }
            set
            {
                SetBinY(value);
            }
        }

        protected bool IsConnected
        {
            get
            {
                return GetConnected();
            }
        }

        public abstract SBIG.READOUT_BINNING_MODE ConvertBinningToReadout(short x, short y);
        protected abstract short GetBinX();
        protected abstract void SetBinX(short value);
        protected abstract short GetBinY();
        protected abstract void SetBinY(short value);

        protected abstract bool GetConnected();
    }
}
