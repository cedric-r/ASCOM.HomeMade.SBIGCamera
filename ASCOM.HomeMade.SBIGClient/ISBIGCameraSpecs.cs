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
using ASCOM.HomeMade.SBIGHub;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGClient
{
    public abstract class ISBIGCameraSpecs : ISBIGCamera
    {
        protected SBIGCommon.Debug debug = null;

        protected SBIGClient server = null;

        public CameraInfo cameraInfo = new CameraInfo();
        protected SBIG.QueryTemperatureStatusResults2 Cooling;

        protected int ccdWidth { get { return cameraInfo.cameraReadoutModes.Find(r => r.mode == 0).width; } }
        protected int ccdHeight { get { return cameraInfo.cameraReadoutModes.Find(r => r.mode == 0).height; } }
        protected double pixelSize { get { return ((double)cameraInfo.cameraReadoutModes.Find(r => r.mode == Binning).pixel_height) / 100; } } // We assume square pixels

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        protected override bool IsConnected
        {
            get
            {

                bool temp = false;

                if (server != null)
                    temp = server.IsConnected;

                debug.LogMessage("IsConnected", "connectionState=" + temp.ToString());
                return temp;
            }
        }

        protected CameraInfo GetCameraSpecs()
        {
            debug.LogMessage("Connected Set", $"Getting camera info");
            if (!IsConnected) throw new NotConnectedException("Not conneted to server");

            CameraInfo cameraInfo = new CameraInfo();

            try
            {
                // query camera info
                var gcir0 = server.CCD_INFO(new SBIG.GetCCDInfoParams
                {
                    request = CameraType == SBIG.CCD_REQUEST.CCD_IMAGING ? SBIG.CCD_INFO_REQUEST.CCD_INFO_IMAGING : SBIG.CCD_INFO_REQUEST.CCD_INFO_TRACKING
                });
                // now print it out
                cameraInfo.firmwareVersion = (gcir0.firmwareVersion >> 8).ToString() + "." + (gcir0.firmwareVersion & 0xFF).ToString();
                debug.LogMessage("Connected Set", $"Firmware version: {cameraInfo.firmwareVersion}");
                cameraInfo.cameraType = gcir0.cameraType;
                debug.LogMessage("Connected Set", $"Camera type: {cameraInfo.cameraType}");
                cameraInfo.name = gcir0.name;
                debug.LogMessage("Connected Set", $"Camera name: {cameraInfo.name}");
                debug.LogMessage("Connected Set", $"Readout modes: {gcir0.readoutModes}");
                Binning = SBIG.READOUT_BINNING_MODE.RM_1X1;
                for (int i = 0; i < gcir0.readoutModes; i++)
                {
                    CameraReadoutMode cameraMode = new CameraReadoutMode();
                    SBIG.READOUT_INFO ri = gcir0.readoutInfo[i];
                    ri.pixel_width = Utils.BCDToUInt(ri.pixel_width);
                    ri.pixel_height = Utils.BCDToUInt(ri.pixel_height);
                    cameraMode.pixel_width = ri.pixel_width;
                    cameraMode.pixel_height = ri.pixel_height;
                    cameraMode.mode = ri.mode;
                    cameraMode.width = ri.width;
                    cameraMode.height = ri.height;
                    cameraMode.gain = (ri.gain >> 8).ToString() + "." + (ri.gain & 0xFF).ToString();
                    cameraInfo.cameraReadoutModes.Add(cameraMode);
                    debug.LogMessage("Connected Set", $"    Binning mode: {cameraMode.mode}");
                    debug.LogMessage("Connected Set", $"    Width: {cameraMode.width}");
                    debug.LogMessage("Connected Set", $"    Height: {cameraMode.height}"); // Don't trust this, there is at least 1 bug in the STF-8300 that reports a height of 0 for mode RM_NX1
                    debug.LogMessage("Connected Set", $"    Gain: {cameraMode.gain} e-/ADU");
                    debug.LogMessage("Connected Set", $"    Pixel width: {cameraMode.pixel_width}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
                    debug.LogMessage("Connected Set", $"    Pixel height: {cameraMode.pixel_height}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
                }
            }
            catch (Exception e1)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e1));
                throw new ASCOM.DriverException(Utils.DisplayException(e1));
            }

            try
            {
                // get extended info
                var gcir2 = server.CCD_INFO_EXTENDED(new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED
                });
                // print it out
                debug.LogMessage("Connected Set", $"Bad columns: {gcir2.badColumns} = ");
                for (int i = 0; i < gcir2.badColumns; i++)
                {
                    debug.LogMessage("Connected Set", "Bad column: { gcir2.columns[i]}");
                        cameraInfo.badColumns.Add(gcir2.columns[i]);
                }
                cameraInfo.imagingABG = gcir2.imagingABG;
                debug.LogMessage("Connected Set", $"ABG: {cameraInfo.imagingABG}");
                cameraInfo.serialNumber = gcir2.serialNumber;
                debug.LogMessage("Connected Set", $"Serial number: {cameraInfo.serialNumber}");
            }
            catch (Exception e2)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e2));
                throw new ASCOM.DriverException(Utils.DisplayException(e2));
            }

            try
            {
                // get extended info
                var gcir3 = server.CCD_INFO_EXTENDED_PIXCEL(
                    new SBIG.GetCCDInfoParams
                    {
                        request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED_5C
                    });
                // print it out
                cameraInfo.filterType = gcir3.filterType;
                debug.LogMessage("Connected Set", $"Filter wheel: {cameraInfo.filterType}");
                switch (cameraInfo.filterType)
                {
                    case SBIG.FILTER_TYPE.FW_UNKNOWN:
                        debug.LogMessage("Connected Set", $"    Unknown");
                        break;
                    case SBIG.FILTER_TYPE.FW_VANE:
                        debug.LogMessage("Connected Set", $"    Vane");
                        break;
                    case SBIG.FILTER_TYPE.FW_EXTERNAL:
                        debug.LogMessage("Connected Set", $"    External");
                        break;
                    case SBIG.FILTER_TYPE.FW_FILTER_WHEEL:
                        debug.LogMessage("Connected Set", $"    Standard");
                        break;
                    default:
                        debug.LogMessage("Connected Set", $"    Unexpected");
                        break;
                }
                if (gcir3.adSize == SBIG.AD_SIZE.AD_12_BITS) cameraInfo.adSize = 12;
                else if (gcir3.adSize == SBIG.AD_SIZE.AD_16_BITS) cameraInfo.adSize = 16;
                else cameraInfo.adSize = 16; // Go with 16 bits default
            }
            catch (Exception e3)
            {
                debug.LogMessage("Connected Set", "Not PixCel camera");
            }

            try
            {
                // get extended info
                var gcir4 = server.CCD_INFO_EXTENDED2(new SBIG.GetCCDInfoParams
                {
                    request = CameraType == SBIG.CCD_REQUEST.CCD_IMAGING ? SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED2_IMAGING : SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED2_TRACKING
                });
                // print it out
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 0))
                {
                    cameraInfo.CCDFrameType = CameraInfo.FRAME_TRANSFER_CCD;
                    debug.LogMessage("Connected Set", $"CCD is frame transfer device");
                }
                else
                {
                    cameraInfo.CCDFrameType = CameraInfo.FULL_FRAME_CCD;
                    debug.LogMessage("Connected Set", $"CCD is full frame device");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 1))
                {
                    cameraInfo.electronicShutter = true;
                    debug.LogMessage("Connected Set", $"Electronic shutter");
                }
                else
                {
                    cameraInfo.electronicShutter = false;
                    debug.LogMessage("Connected Set", $"No electronic shutter");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 2))
                {
                    cameraInfo.remoteGuideHead = true;
                    debug.LogMessage("Connected Set", $"Remote guide head present");
                }
                else
                {
                    cameraInfo.remoteGuideHead = false;
                    debug.LogMessage("Connected Set", $"No remote guide head");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 3))
                {
                    cameraInfo.TDIAcquisitionMode = true;
                    debug.LogMessage("Connected Set", $"Supports Biorad TDI acquisition more");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 4))
                {
                    cameraInfo.AO8 = true;
                    debug.LogMessage("Connected Set", $"AO8 detected");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 5))
                {
                    cameraInfo.internalFrameBuffer = true;
                    debug.LogMessage("Connected Set", $"Camera contains an internal frame buffer");
                }
                if (Utils.IsBitSet(gcir4.capabilitiesBits, 6))
                {
                    cameraInfo.requiresStartExposure2 = true;
                    debug.LogMessage("Connected Set", $"Camera requires StartExposure2 command");
                }
                else
                {
                    cameraInfo.requiresStartExposure2 = false;
                    debug.LogMessage("Connected Set", $"Camera requires StartExposure command");
                }
            }
            catch (Exception e4)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e4));
                throw new ASCOM.DriverException(Utils.DisplayException(e4));
            }

            try
            {
                // get extended info
                var gcir6 = server.CCD_INFO_EXTENDED3(new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED3
                });
                // print it out
                if (Utils.IsBitSet(gcir6.cameraBits, 0))
                {
                    cameraInfo.STXLCamera = true;
                    debug.LogMessage("Connected Set", $"STXL camera");
                }
                else
                {
                    cameraInfo.STXLCamera = false;
                    debug.LogMessage("Connected Set", $"STX camera");
                }
                if (Utils.IsBitSet(gcir6.cameraBits, 1))
                {
                    cameraInfo.mechanicalShutter = false;
                    debug.LogMessage("Connected Set", $"No mechanical shutter");
                }
                else
                {
                    cameraInfo.mechanicalShutter = true;
                    debug.LogMessage("Connected Set", $"Mechanical shutter");
                }
                if (Utils.IsBitSet(gcir6.ccdBits, 0))
                {
                    debug.LogMessage("Connected Set", $"Color CCD");
                    cameraInfo.colourCamera = true;
                    if (Utils.IsBitSet(gcir6.ccdBits, 1))
                    {
                        debug.LogMessage("Connected Set", $"Truesense color matrix");
                        cameraInfo.bayer = CameraInfo.TRUESENSE;
                    }
                    else
                    {
                        debug.LogMessage("Connected Set", $"Bayer color matrix");
                        cameraInfo.bayer = CameraInfo.COLOR;
                    }
                }
                else
                {
                    cameraInfo.colourCamera = false;
                    debug.LogMessage("Connected Set", $"Mono CDD");
                }
            }
            catch (Exception e5)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e5));
                throw new ASCOM.DriverException(Utils.DisplayException(e5));
            }

            cameraNumX = cameraInfo.cameraReadoutModes.Find(r => r.mode == 0).width;
            cameraNumY = cameraInfo.cameraReadoutModes.Find(r => r.mode == 0).height;

            try
            {
                if (CameraType == SBIG.CCD_REQUEST.CCD_IMAGING)
                {
                    // query temperature
                    Cooling = server.CC_QUERY_TEMPERATURE_STATUS();
                    debug.LogMessage("Connected", "CCD temperature is " + Cooling.imagingCCDTemperature);
                    debug.LogMessage("Connected", "CCD power is " + Cooling.imagingCCDPower);
                    debug.LogMessage("Connected", "CCD cooler is " + Cooling.coolingEnabled.value.ToString());
                }
            }
            catch (Exception e6)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e6));
                throw new ASCOM.DriverException(Utils.DisplayException(e6));
            }
            return cameraInfo;
        }

        public  virtual SBIGBayerPattern GetBayerPattern()
        {
            return SBIGBayerPattern.Mono;
        }
    }

    public class CameraInfo
    {
        public static int FRAME_TRANSFER_CCD = 1;
        public static int FULL_FRAME_CCD = 0;
        public static int TRUESENSE = 0;
        public static int COLOR = 1;

        public string firmwareVersion;
        public SBIG.CAMERA_TYPE cameraType;
        public string name;
        public SBIG.IMAGING_ABG imagingABG;
        public string serialNumber;
        public List<ushort> badColumns = new List<ushort>();
        public SBIG.FILTER_TYPE filterType;
        public int adSize;
        public int CCDFrameType;
        public bool electronicShutter;
        public bool remoteGuideHead;
        public bool TDIAcquisitionMode;
        public bool AO8;
        public bool internalFrameBuffer;
        public bool requiresStartExposure2;
        public bool STXLCamera;
        public bool mechanicalShutter;
        public bool colourCamera;
        public int bayer;
        public List<CameraReadoutMode> cameraReadoutModes = new List<CameraReadoutMode>();
    }

    public class CameraReadoutMode
    {
        public SBIG.READOUT_BINNING_MODE mode;
        public uint pixel_width;
        public uint pixel_height;
        public string gain;
        public ushort width;
        public ushort height;
    }
}
