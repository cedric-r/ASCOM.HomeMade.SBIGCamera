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

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SbigSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ASCOM.HomeMade
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63ad-c913-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(Camera.driverID)]

    public class Camera : ICameraV2, IFilterWheelV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal const string driverID = "ASCOM.HomeMade.SBIGCamera";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM SBIG Camera Driver.";

        internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
        internal static string comPortDefault = "COM1";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";

        internal static string comPort; // Variables to hold the currrent device configuration

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        internal static int connections = 0;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMade"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            Debug.FileName = @"c:\temp\SBIGCamera.log";
            if (!Debug.Testing) ReadProfile(); // Read device configuration from the ASCOM Profile store

            Debug.LogMessage("Camera", "Starting initialisation");

            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here

            if (IsConnected) return; // No need to connect several times, e.g. when camera then FW are connected

            Debug.LogMessage("Camera", "Starting background worker");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();

            Debug.LogMessage("Camera", "Completed initialisation");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                GetCoolingInfo();
                Thread.Sleep(500);
            }
        }

        //
        // PUBLIC COM INTERFACE ICameraV2 IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected
            if (IsConnected)
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                Debug.LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
            throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        public void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
            // DO NOT have both these sections!  One or the other
        }

        public bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            string ret = CommandString(command, raw);
            // TODO decode the return string and return true or false
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBool");
            // DO NOT have both these sections!  One or the other
        }

        public string CommandString(string command, bool raw)
        {
            CheckConnected("CommandString");
            // it's a good idea to put all the low level communication with the device here,
            // then all communication calls this function
            // you need something to ensure that only one command is in progress at a time

            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            Debug.TraceEnabled = false;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public bool Connected
        {
            get
            {
                LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                try
                {
                    Debug.LogMessage("Connected", "Set {0}", value);

                    if (value == IsConnected)
                        return;

                    if (value)
                    {
                        SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_OPEN_DRIVER);

                        bool cameraFound = false;
                        Debug.LogMessage("Connected Set", $"Enumerating USB cameras");
                        SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_QUERY_USB, out SBIG.QueryUSBResults qur);
                        for (int i = 0; i < qur.camerasFound; i++)
                        {
                            if (!qur.usbInfo[i].cameraFound)
                                Debug.LogMessage("Connected Set", $"Cam {i}: not found");
                            else
                            {
                                Debug.LogMessage("Connected Set",
                                    $"Cam {i}: type={qur.usbInfo[i].cameraType} " +
                                    $"name={ qur.usbInfo[i].name} " +
                                    $"ser={qur.usbInfo[i].serialNumber}");
                                cameraFound = true;
                            }
                        }

                        if (!cameraFound)
                        {
                            Debug.LogMessage("Connected Set", $"No USB camera found");
                            throw new DriverException("No suitable camera found");
                        }
                        else
                        {
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_OPEN_DEVICE,
                                new SBIG.OpenDeviceParams
                                {
                                    deviceType = SBIG.SBIG_DEVICE_TYPE.DEV_USB
                                });
                            CameraType = SBIG.EstablishLink();
                            connections++;
                            Debug.LogMessage("Connected Set", $"Connected to USB camera");

                            Debug.LogMessage("Connected Set", $"Getting camera info");
                            // query camera info
                            var gcir0 = new SBIG.GetCCDInfoResults0();
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                                new SBIG.GetCCDInfoParams
                                {
                                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_IMAGING
                                },
                                out gcir0);
                            // now print it out
                            Debug.LogMessage("Connected Set", $"Firmware version: {gcir0.firmwareVersion >> 8}.{gcir0.firmwareVersion & 0xFF}");
                            Debug.LogMessage("Connected Set", $"Camera type: {gcir0.cameraType}");
                            Debug.LogMessage("Connected Set", $"Camera name: {gcir0.name}");
                            Debug.LogMessage("Connected Set", $"Readout modes: {gcir0.readoutModes}");
                            Binning = SBIG.READOUT_BINNING_MODE.RM_1X1;
                            for (int i = 0; i < gcir0.readoutModes; i++)
                            {
                                SBIG.READOUT_INFO ri = gcir0.readoutInfo[i];
                                ri.pixel_height = (uint)ConvertReadoutModeToBinning(ri.mode);
                                ri.pixel_width = (uint)ConvertReadoutModeToBinning(ri.mode);
                                ReadoutModeList.Add(ri);
                                Debug.LogMessage("Connected Set", $"    Binning mode: {ri.mode}");
                                Debug.LogMessage("Connected Set", $"    Width: {ri.width}");
                                Debug.LogMessage("Connected Set", $"    Height: {ri.height}");
                                Debug.LogMessage("Connected Set", $"    Gain: {ri.gain >> 8}.{ri.gain & 0xFF} e-/ADU");
                                Debug.LogMessage("Connected Set", $"    Pixel width: {ri.pixel_width}");
                                Debug.LogMessage("Connected Set", $"    Pixel height: {ri.pixel_height}");
                            }

                            // get extended info
                            var gcir2 = new SBIG.GetCCDInfoResults2();
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                                new SBIG.GetCCDInfoParams
                                {
                                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED
                                },
                                out gcir2);
                            // print it out
                            Debug.LogMessage("Connected Set", $"Bad columns: {gcir2.badColumns} = ");
                            Debug.LogMessage("Connected Set",
                                $"{gcir2.columns[0]}, {gcir2.columns[1] }, " +
                                $"{gcir2.columns[2]}, { gcir2.columns[3]}");
                            Debug.LogMessage("Connected Set", $"ABG: {gcir2.imagingABG}");
                            Debug.LogMessage("Connected Set", $"Serial number: {gcir2.serialNumber}");
                            /*
                            // get extended info
                            var gcir3 = new SBIG.GetCCDInfoResults3();
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                                new SBIG.GetCCDInfoParams
                                {
                                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED_5C
                                },
                                out gcir3);
                            // print it out
                            Debug.LogMessage("Connected Set", $"Filter wheel: {gcir3.filterType}");
                            switch (gcir3.filterType)
                            {
                                case SBIG.FILTER_TYPE.FW_UNKNOWN:
                                    Debug.LogMessage("Connected Set", $"    Unknown");
                                    break;
                                case SBIG.FILTER_TYPE.FW_VANE:
                                    Debug.LogMessage("Connected Set", $"    Vane");
                                    FilterWheelPresent = true;
                                    break;
                                case SBIG.FILTER_TYPE.FW_EXTERNAL:
                                    Debug.LogMessage("Connected Set", $"    External");
                                    FilterWheelPresent = true;
                                    break;
                                case SBIG.FILTER_TYPE.FW_FILTER_WHEEL:
                                    Debug.LogMessage("Connected Set", $"    Standard");
                                    FilterWheelPresent = true;
                                    break;
                                default:
                                    Debug.LogMessage("Connected Set", $"    Unexpected");
                                    break;
                            }
                            */
                            // get extended info
                            var gcir4 = new SBIG.GetCCDInfoResults4();
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                                new SBIG.GetCCDInfoParams
                                {
                                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED2_IMAGING
                                },
                                out gcir4);
                            // print it out
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 0)) Debug.LogMessage("Connected Set", $"CCD is frame transfer device");
                            else Debug.LogMessage("Connected Set", $"CCD is full frame device");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 1)) Debug.LogMessage("Connected Set", $"Electronic shutter");
                            else Debug.LogMessage("Connected Set", $"No electronic shutter");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 2)) Debug.LogMessage("Connected Set", $"Remote guide head present");
                            else Debug.LogMessage("Connected Set", $"No remote guide head");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 3)) Debug.LogMessage("Connected Set", $"Supports Biorad TDI acquisition more");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 4)) Debug.LogMessage("Connected Set", $"AO8 detected");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 5)) Debug.LogMessage("Connected Set", $"Camera contains an internal frame buffer");
                            if (Utils.IsBitSet(gcir4.capabilitiesBits, 6))
                            {
                                Debug.LogMessage("Connected Set", $"Camera requires StartExposure2 command");
                                RequiresExposureParams2 = true;
                            }
                            else
                            {
                                Debug.LogMessage("Connected Set", $"Camera requires StartExposure command");
                                RequiresExposureParams2 = false;
                            }

                            // get extended info
                            var gcir6 = new SBIG.GetCCDInfoResults6();
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                                new SBIG.GetCCDInfoParams
                                {
                                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED3
                                },
                                out gcir6);
                            // print it out
                            if (Utils.IsBitSet(gcir6.cameraBits, 0)) Debug.LogMessage("Connected Set", $"STXL camera");
                            else Debug.LogMessage("Connected Set", $"STX camera");
                            if (Utils.IsBitSet(gcir6.cameraBits, 1))
                            {
                                HasMechanicalShutter = false;
                                Debug.LogMessage("Connected Set", $"No mechanical shutter");
                            }
                            else
                            {
                                HasMechanicalShutter = true;
                                Debug.LogMessage("Connected Set", $"Mechanical shutter");
                            }
                            if (Utils.IsBitSet(gcir6.ccdBits, 0))
                            {
                                Debug.LogMessage("Connected Set", $"Color CCD");
                                ColorCamera = true;
                                if (Utils.IsBitSet(gcir6.ccdBits, 1)) Debug.LogMessage("Connected Set", $"Truesense color matrix");
                                else Debug.LogMessage("Connected Set", $"Bayer color matrix");
                            }
                            else
                            {
                                ColorCamera = false;
                                Debug.LogMessage("Connected Set", $"Mono CDD");
                            }

                            // query temperature
                            SBIG.UnivDrvCommand(
                                SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS,
                                 new SBIG.QueryTemperatureStatusParams()
                                 {
                                     request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
                                 },
                                out Cooling);
                            Debug.LogMessage("Connected", "CCD temperature is " + Cooling.imagingCCDTemperature);
                            Debug.LogMessage("Connected", "CCD power is " + Cooling.imagingCCDPower);
                            Debug.LogMessage("Connected", "CCD cooler is " + Cooling.coolingEnabled.value.ToString());

                            var fwresults = GetFWData();
                            if (fwresults.cfwError == SBIG.CFW_ERROR.CFWE_NONE)
                            {
                                string model = GetFWTypeName(fwresults);
                                Debug.LogMessage("Connected", "FW is " + model);
                                if (model != "Unknown") FilterWheelPresent = true;

                                SBIG.CFWResults fwstatus = GetFWStatus();
                                string status = GetFWStatusName(fwstatus);
                                Debug.LogMessage("Connected", "FW status is " + status);

                                Debug.LogMessage("Connected", "FW has " + fwresults.cfwResult2 + " positions");
                                FWPositions = (int)fwresults.cfwResult2;

                                SBIG.CFWResults fwfilter = GetFWPosition();
                                Debug.LogMessage("Connected", "FW position is " + GetFWFilterName(fwfilter));
                            }
                            else
                            {
                                Debug.LogMessage("Connected", "Error querying FW: " + fwresults.cfwError);
                            }
                        }
                    }
                    else
                    {
                        connections--;
                        if (!IsConnected)
                        {
                            try
                            {
                                // clean up
                                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DEVICE);
                                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DRIVER);
                            }
                            catch (Exception) { }
                        }
                        LogMessage("Connected Set", "Disconnecting camera");
                        // TODO disconnect from the device
                    }
                }
                catch (Exception e)
                {
                    Debug.LogMessage("Connected Set", "Error: " + e.Message + "\n" + e.StackTrace);
                }
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                Debug.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                Debug.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                Debug.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                LogMessage("InterfaceVersion Get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "ASCOM driver for SBIG cameras";
                Debug.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region ICamera Implementation

        private const double MAX_EXPOSURE_TIME = 167777.16;
        private const double MIN_EXPOSURE_TIME = 0; // This is camera dependent. So requesting a shorter exposure than the camera can do will result in minimum allowed exposure
        private const double EXPOSURE_RESOLTION = 0.0;

        private SBIG.CAMERA_TYPE CameraType;
        private bool ColorCamera = false;
        private bool RequiresExposureParams2 = true;
        private SBIG.READOUT_BINNING_MODE Binning = 0;
        private SBIG.QueryTemperatureStatusResults2 Cooling;
        private double CCDTempTarget = 0;
        private bool CCDTempTargetSet = false;
        private bool HasMechanicalShutter = false;
        private CameraStates CurrentCameraState = CameraStates.cameraIdle;
        private bool FilterWheelPresent = false;
        private int FWPositions = 0;
        private List<SBIG.READOUT_INFO> ReadoutModeList = new List<SBIG.READOUT_INFO>();
        private int ccdWidth { get { return ReadoutModeList.Find(r => r.mode == Binning).width; } }
        private int ccdHeight { get { return ReadoutModeList.Find(r => r.mode == Binning).height; } }
        private double pixelSize { get { return ReadoutModeList.Find(r => r.mode == Binning).pixel_height; } } // We assume square pixels

        private int cameraNumX = 0;
        private int cameraNumY = 0;
        private int cameraStartX = 0;
        private int cameraStartY = 0;
        private DateTime exposureStart = DateTime.MinValue;
        private double cameraLastExposureDuration = 0.0;
        private bool cameraImageReady = false;
        private Int32[,] cameraImageArray;
        private object[,] cameraImageArrayVariant;

        private SBIG.StartExposureParams2 exposureParams2;

        public void AbortExposure()
        {
            if (!IsConnected) throw new NotConnectedException("Camera is not connected");

            CurrentCameraState = CameraStates.cameraIdle;
            if (RequiresExposureParams2)
            {
                Debug.LogMessage("AbortExposure", "Aborting...");
                SBIG.AbortExposure(exposureParams2);
            }
            else
            {
                Debug.LogMessage("AbortExposure", "AbortExposure is not supported");
                throw new InvalidOperationException("This driver does not support legacy calls");
            }
        }

        public short BayerOffsetX
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("BayerOffsetX Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", false);
            }
        }

        public short BayerOffsetY
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("BayerOffsetY Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", true);
            }
        }

        public short BinX
        {
            get
            {
                Debug.LogMessage("BinX Get", "Bin size is "+ ConvertReadoutToBinning(Binning));
                return ConvertReadoutToBinning(Binning);
            }
            set
            {
                Debug.LogMessage("BinX Set", value.ToString());
                Binning = ConvertBinningToReadout(value);

            }
        }

        public short BinY
        {
            get
            {
                Debug.LogMessage("BinY Get", "Bin size is " + ConvertReadoutToBinning(Binning));
                return ConvertReadoutToBinning(Binning);
            }
            set
            {
                Debug.LogMessage("BinY Set", value.ToString());
                Binning = ConvertBinningToReadout(value);
            }
        }

        public double CCDTemperature
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CCDTemperature Get", "CCD temperature is " + Cooling.imagingCCDTemperature);
                return Cooling.imagingCCDTemperature;
            }
        }

        public CameraStates CameraState
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CameraState Get", CurrentCameraState.ToString());
                return CurrentCameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CameraXSize Get", ccdWidth.ToString());
                return ccdWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CameraYSize Get", ccdHeight.ToString());
                return ccdHeight;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (RequiresExposureParams2)
                {
                    Debug.LogMessage("CanAbortExposure Get", true.ToString());
                    return true;
                }
                else
                {
                    Debug.LogMessage("CanAbortExposure Get", false.ToString());
                    return false;
                }
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CanAsymmetricBin Get", false.ToString());
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                // At the moment we say no although some cameras, e.g. STF-8300 can
                Debug.LogMessage("CanFastReadout Get", false.ToString());
                return false;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CanGetCoolerPower Get", true.ToString());
                return true;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CanSetCCDTemperature Get", true.ToString());
                return true;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (RequiresExposureParams2)
                {
                    Debug.LogMessage("CanStopExposure Get", true.ToString());
                    return true;
                }
                else
                {
                    Debug.LogMessage("CanStopExposure Get", false.ToString());
                    return false;
                }
            }
        }

        public bool CoolerOn
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CoolerOn Get", "Cooler is " + Cooling.coolingEnabled.value.ToString());
                return Cooling.coolingEnabled.value == 0 ? false : true;
            }
            set
            {
                try
                {
                    if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                    var tparams = new SBIG.SetTemperatureRegulationParams2();
                    if (value) tparams.regulation = SBIG.TEMPERATURE_REGULATION.REGULATION_ON;
                    else tparams.regulation = SBIG.TEMPERATURE_REGULATION.REGULATION_OFF;
                    if (CCDTempTargetSet)
                    {
                        tparams.ccdSetpoint = CCDTempTarget;
                    }
                    else
                    {
                        tparams.ccdSetpoint = Cooling.ambientTemperature;
                    }
                    SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_SET_TEMPERATURE_REGULATION2, tparams);
                    if (value) Debug.LogMessage("CoolerOn Set", "Coller On at " + tparams.ccdSetpoint);
                    else Debug.LogMessage("CoolerOn Set", "Coller Off");
                }
                catch (Exception e)
                {
                    Debug.LogMessage("CoolerOn Set", "Error: " + e.Message + "\n" + e.StackTrace);
                    throw;
                }

            }
        }

        public double CoolerPower
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("CoolerPower Get", "Cooler power is " + Cooling.imagingCCDPower.ToString());
                return Cooling.imagingCCDPower;
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("ElectronsPerADU Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ElectronsPerADU", false);
            }
        }

        public double ExposureMax
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("ExposureMax Get", "Exposure max is " + MAX_EXPOSURE_TIME.ToString());
                return MAX_EXPOSURE_TIME;
            }
        }

        public double ExposureMin
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("ExposureMin Get", "Exposure min is " + MIN_EXPOSURE_TIME.ToString());
                return MIN_EXPOSURE_TIME;
            }
        }

        public double ExposureResolution
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("ExposureResolution Get", "Exposure resolution min is " + EXPOSURE_RESOLTION.ToString());
                return EXPOSURE_RESOLTION;
            }
        }

        public bool FastReadout
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("FastReadout Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
            }
            set
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("FastReadout Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", true);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("FullWellCapacity Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FullWellCapacity", false);
            }
        }

        public short Gain
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                SBIG.READOUT_INFO ri = ReadoutModeList.Find(r => r.mode == Binning);
                Debug.LogMessage("Gain Get", "Camera gain is " + ri.gain.ToString());
                return (short)ri.gain;
            }
            set
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("Gain Set", "Do nothing");
            }
        }

        public short GainMax
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                short maxgain = 0;
                foreach (var ri in ReadoutModeList)
                {
                    if (ri.gain > maxgain) maxgain = (short)ri.gain;
                }

                Debug.LogMessage("GainMax Get", "Max gain is " + maxgain);
                return maxgain;
            }
        }

        public short GainMin
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                short mingain = 0;
                foreach (var ri in ReadoutModeList)
                {
                    if (ri.gain < mingain) mingain = (short)ri.gain;
                }

                Debug.LogMessage("GainMax Get", "Min gain is " + mingain);
                return mingain;
            }
        }

        public ArrayList Gains
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                ArrayList list = new ArrayList();
                foreach(var ri in ReadoutModeList)
                {
                    if (!list.Contains(ri.gain)) list.Add(ri.gain);
                }
                Debug.LogMessage("Gains Get", "Returning existing gains");
                return list;
            }
        }

        public bool HasShutter
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("HasShutter Get", HasMechanicalShutter.ToString());
                return HasMechanicalShutter;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("HeatSinkTemperature Get", "Heatsink temperature is " + Cooling.heatsinkTemperature.ToString());
                return Cooling.heatsinkTemperature;
            }
        }

        public object ImageArray
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (!cameraImageReady)
                {
                    Debug.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArray before the first image has been taken!");
                }

                return cameraImageArray;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (!cameraImageReady)
                {
                    Debug.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArrayVariant before the first image has been taken!");
                }
                cameraImageArrayVariant = new object[cameraNumX, cameraNumY];
                for (int i = 0; i < cameraImageArray.GetLength(1); i++)
                {
                    for (int j = 0; j < cameraImageArray.GetLength(0); j++)
                    {
                        cameraImageArrayVariant[j, i] = cameraImageArray[j, i];
                    }

                }

                return cameraImageArrayVariant;
            }
        }

        public bool ImageReady
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("ImageReady Get", cameraImageReady.ToString());
                return cameraImageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("IsPulseGuiding Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("IsPulseGuiding", false);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (!cameraImageReady)
                {
                    Debug.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureDuration before the first image has been taken!");
                }
                Debug.LogMessage("LastExposureDuration Get", cameraLastExposureDuration.ToString());
                return cameraLastExposureDuration;
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (!cameraImageReady)
                {
                    Debug.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!");
                }
                Debug.LogMessage("LastExposureStartTime Get", exposureStart.ToString());
                return exposureStart.ToString();
            }
        }

        public int MaxADU
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                Debug.LogMessage("MaxADU Get", "50000");
                return 50000;
            }
        }

        public short MaxBinX
        {
            get
            {
                Debug.LogMessage("MaxBinX Get", "3");
                return 3;
            }
        }

        public short MaxBinY
        {
            get
            {
                Debug.LogMessage("MaxBinY Get", "3");
                return 3;
            }
        }

        public int NumX
        {
            get
            {
                Debug.LogMessage("NumX Get", cameraNumX.ToString());
                return cameraNumX;
            }
            set
            {
                cameraNumX = value;
                Debug.LogMessage("NumX set", value.ToString());
            }
        }

        public int NumY
        {
            get
            {
                Debug.LogMessage("NumY Get", cameraNumY.ToString());
                return cameraNumY;
            }
            set
            {
                cameraNumY = value;
                Debug.LogMessage("NumY set", value.ToString());
            }
        }

        public short PercentCompleted
        {
            get
            {
                Debug.LogMessage("PercentCompleted Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("PercentCompleted", false);
            }
        }

        public double PixelSizeX
        {
            get
            {
                Debug.LogMessage("PixelSizeX Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public double PixelSizeY
        {
            get
            {
                Debug.LogMessage("PixelSizeY Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            Debug.LogMessage("PulseGuide", "Not implemented");
            throw new ASCOM.MethodNotImplementedException("PulseGuide");
        }

        public short ReadoutMode
        {
            get
            {
                Debug.LogMessage("ReadoutMode Get", "Binning is " + Binning);
                return (short)Binning;
            }
            set
            {
                Debug.LogMessage("ReadoutMode Set", "Setting binning to " + value);
                Binning = (SBIG.READOUT_BINNING_MODE)value;
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                Debug.LogMessage("ReadoutModes Get", "Returning readout modes");
                ArrayList list = new ArrayList();
                foreach (var readoutmode in ReadoutModeList)
                {
                    list.Add(readoutmode.mode);
                }
                return list;
            }
        }

        public string SensorName
        {
            get
            {
                Debug.LogMessage("SensorName Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SensorName", false);
            }
        }

        public SensorType SensorType
        {
            get
            {
                Debug.LogMessage("SensorType Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SensorType", false);
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                Debug.LogMessage("SetCCDTemperature Get", "CCD temperature target is " + CCDTempTarget);
                return CCDTempTarget;
            }
            set
            {
                CCDTempTarget = value;
                CCDTempTargetSet = true;
                Debug.LogMessage("SetCCDTemperature Set", "CCD temperature target is " + CCDTempTarget);
            }
        }

        public void StartExposure(double Duration, bool Light)
        {
            try
            {
                if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");
                if (cameraNumX > ccdWidth) throw new InvalidValueException("StartExposure", cameraNumX.ToString(), ccdWidth.ToString());
                if (cameraNumY > ccdHeight) throw new InvalidValueException("StartExposure", cameraNumY.ToString(), ccdHeight.ToString());
                if (cameraStartX > ccdWidth) throw new InvalidValueException("StartExposure", cameraStartX.ToString(), ccdWidth.ToString());
                if (cameraStartY > ccdHeight) throw new InvalidValueException("StartExposure", cameraStartY.ToString(), ccdHeight.ToString());

                cameraLastExposureDuration = Duration;
                exposureStart = DateTime.Now;

                if (!RequiresExposureParams2)
                {
                    throw new InvalidOperationException("This driver does not support legacy calls");
                }
                exposureParams2 = new SBIG.StartExposureParams2
                {
                    ccd = SBIG.CCD_REQUEST.CCD_IMAGING,
                    abgState = SBIG.ABG_STATE7.ABG_LOW7,
                    openShutter = Light ? SBIG.SHUTTER_COMMAND.SC_OPEN_SHUTTER : SBIG.SHUTTER_COMMAND.SC_CLOSE_SHUTTER,
                    readoutMode = Binning,
                    exposureTime = Convert.ToUInt32(Duration),
                    width = Convert.ToUInt16(cameraNumX), // This is in binned pixels. Check is this is right
                    height = Convert.ToUInt16(cameraNumY),
                    left = (ushort)cameraStartX,
                    top = (ushort)cameraStartY
                };

                CurrentCameraState = CameraStates.cameraExposing;
                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_START_EXPOSURE2, exposureParams2);

                // read out the image
                CurrentCameraState = CameraStates.cameraReading;
                cameraImageArray = SBIG.WaitEndAndReadoutExposure(exposureParams2);

                CurrentCameraState = CameraStates.cameraIdle;
                Debug.LogMessage("StartExposure", Duration.ToString() + " " + Light.ToString());
                cameraImageReady = true;
            }
            catch (Exception e)
            {
                Debug.LogMessage("StartExposure", "Error: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        public int StartX
        {
            get
            {
                Debug.LogMessage("StartX Get", cameraStartX.ToString());
                return cameraStartX;
            }
            set
            {
                cameraStartX = value;
                Debug.LogMessage("StartX Set", value.ToString());
            }
        }

        public int StartY
        {
            get
            {
                Debug.LogMessage("StartY Get", cameraStartY.ToString());
                return cameraStartY;
            }
            set
            {
                cameraStartY = value;
                Debug.LogMessage("StartY set", value.ToString());
            }
        }

        public void StopExposure()
        {
            AbortExposure();
        }

        #endregion

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "Camera";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }

            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "FilterWheel";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        private void GetCoolingInfo()
        {
            if (IsConnected)
            {
                try {
                    Debug.LogMessage("Camera", "Getting cooling information");
                    // query temperature
                    SBIG.UnivDrvCommand(
                        SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS,
                         new SBIG.QueryTemperatureStatusParams()
                         {
                             request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
                         },
                        out Cooling);
                }
                catch (Exception e)
                {
                    Debug.LogMessage("GetCoolingInfo", "Error: " + e.Message + "\n" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                // TODO check that the driver hardware connection exists and is connected to the hardware
                return connections>0;
            }
        }

        private SBIG.CFWResults FWCommand(SBIG.CFWParams command)
        {
            try
            {
                var fwresults = new SBIG.CFWResults();
                SBIG.UnivDrvCommand(
                    SBIG.PAR_COMMAND.CC_CFW,
                    new SBIG.CFWParams
                    {
                        cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                        cfwCommand = SBIG.CFW_COMMAND.CFWC_OPEN_DEVICE
                    },
                    out fwresults);

                var fwresultsToReturn = new SBIG.CFWResults();
                SBIG.UnivDrvCommand(
                    SBIG.PAR_COMMAND.CC_CFW,
                    command,
                    out fwresultsToReturn);

                fwresults = new SBIG.CFWResults();
                SBIG.UnivDrvCommand(
                    SBIG.PAR_COMMAND.CC_CFW,
                    new SBIG.CFWParams
                    {
                        cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                        cfwCommand = SBIG.CFW_COMMAND.CFWC_CLOSE_DEVICE
                    },
                    out fwresults);

                return fwresultsToReturn;
            }
            catch (Exception e)
            {
                Debug.LogMessage("FWCommand", "Error: " + e.Message + "\n" + e.StackTrace);
                return new SBIG.CFWResults();
            }

        }

        private SBIG.CFWResults GetFWData()
        {
            try
            {
                return FWCommand(new SBIG.CFWParams
                {
                    cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                    cfwCommand = SBIG.CFW_COMMAND.CFWC_GET_INFO
                });
            }
            catch (Exception e)
            {
                Debug.LogMessage("GetFWData", "Error: " + e.Message + "\n" + e.StackTrace);
                throw new ASCOM.DriverException("Error getting DW data");
            }
        }

        private void WaitFWIdle()
        {
            SBIG.CFWResults results = new SBIG.CFWResults();
            while (results.cfwStatus != SBIG.CFW_STATUS.CFWS_IDLE)
            {
                results = FWCommand(new SBIG.CFWParams
                {
                    cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                    cfwCommand = SBIG.CFW_COMMAND.CFWC_QUERY
                });
            }
        }

        private SBIG.CFWResults GetFWStatus()
        {
            SBIG.CFWResults results = new SBIG.CFWResults();
            results = FWCommand(new SBIG.CFWParams
            {
                cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                cfwCommand = SBIG.CFW_COMMAND.CFWC_QUERY
            });

            return results;
        }

        private SBIG.CFWResults GetFWPosition()
        {
            try
            {
                WaitFWIdle();
                SBIG.CFWResults results = FWCommand(new SBIG.CFWParams
                {
                    cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                    cfwCommand = SBIG.CFW_COMMAND.CFWC_QUERY
                });
                return results;
            }
            catch (Exception e)
            {
                Debug.LogMessage("GetFWData", "Error: " + e.Message + "\n" + e.StackTrace);
                throw new ASCOM.DriverException("Error getting DW data");
            }
        }

        private SBIG.CFWResults SetFWPosition(short position)
        {
            try
            {
                WaitFWIdle();
                SBIG.CFWResults results = FWCommand(
                    new SBIG.CFWParams
                    {
                        cfwModel = SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO,
                        cfwCommand = SBIG.CFW_COMMAND.CFWC_GOTO,
                        cfwParam1 = (uint)position
                    });
                WaitFWIdle();
                return results;
            }
            catch (Exception e)
            {
                Debug.LogMessage("GetFWData", "Error: " + e.Message + "\n" + e.StackTrace);
                throw new ASCOM.DriverException("Error getting DW data");
            }
        }

        private string GetFWTypeName(SBIG.CFWResults fwresults)
        {
            string model = "";
            if (fwresults.cfwError == SBIG.CFW_ERROR.CFWE_NONE)
            {
                switch (fwresults.cfwModel)
                {
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW2:
                        model = "CFWSEL_CFW2";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW5:
                        model = "CFWSEL_CFW5";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW8:
                        model = "CFWSEL_CFW8";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFWL:
                        model = "CFWSEL_CFWL";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW402:
                        model = "CFWSEL_CFW402";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_AUTO:
                        model = "CFWSEL_AUTO";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW6A:
                        model = "CFWSEL_CFW6A";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW10:
                        model = "CFWSEL_CFW10";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW10_SERIAL:
                        model = "CFWSEL_CFW10_SERIAL";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW9:
                        model = "CFWSEL_CFW9";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFWL8:
                        model = "CFWSEL_CFWL8";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFWL8G:
                        model = "CFWSEL_CFWL8G";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_CFW1603:
                        model = "CFWSEL_CFW1603";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW5_STX:
                        model = "CFWSEL_FW5_STX";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW5_8300:
                        model = "CFWSEL_FW5_8300";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW8_8300:
                        model = "CFWSEL_FW8_8300";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW7_STX:
                        model = "CFWSEL_FW7_STX";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW8_STT:
                        model = "CFWSEL_FW8_STT";
                        break;
                    case SBIG.CFW_MODEL_SELECT.CFWSEL_FW5_STF_DETENT:
                        model = "CFWSEL_FW5_STF_DETENT";
                        break;
                    default:
                        model = "Unknown";
                        break;
                }
            }
            return model;
        }

        private string GetFWStatusName(SBIG.CFWResults fwresults)
        {
            string status = "";
            switch (fwresults.cfwStatus)
            {
                case SBIG.CFW_STATUS.CFWS_IDLE:
                    status = "Idle";
                    break;
                case SBIG.CFW_STATUS.CFWS_BUSY:
                    status = "Busy";
                    break;
                default:
                    status = "Unknown";
                    break;
            }
            return status;
        }

        private string GetFWFilterName(SBIG.CFWResults fwresults)
        {
            string filter = "";
            switch (fwresults.cfwPosition)
            {
                case SBIG.CFW_POSITION.CFWP_1:
                    filter = "Filter 1";
                    break;
                case SBIG.CFW_POSITION.CFWP_2:
                    filter = "Filter 2";
                    break;
                case SBIG.CFW_POSITION.CFWP_3:
                    filter = "Filter 3";
                    break;
                case SBIG.CFW_POSITION.CFWP_4:
                    filter = "Filter 4";
                    break;
                case SBIG.CFW_POSITION.CFWP_5:
                    filter = "Filter 5";
                    break;
                case SBIG.CFW_POSITION.CFWP_6:
                    filter = "Filter 6";
                    break;
                case SBIG.CFW_POSITION.CFWP_7:
                    filter = "Filter 7";
                    break;
                case SBIG.CFW_POSITION.CFWP_8:
                    filter = "Filter 8";
                    break;
                case SBIG.CFW_POSITION.CFWP_9:
                    filter = "Filter 9";
                    break;
                case SBIG.CFW_POSITION.CFWP_10:
                    filter = "Filter 10";
                    break;
                default:
                    filter = "Unknown";
                    break;
            }
            return filter;
        }

        private short ConvertReadoutModeToBinning(SBIG.READOUT_BINNING_MODE binning)
        {
            short bin = 0;
            switch (binning)
            {
                case SBIG.READOUT_BINNING_MODE.RM_1X1:
                    bin = 1;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_1X1_VOFFCHIP:
                    bin = 1;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_2X2:
                    bin = 2;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_2X2_VOFFCHIP:
                    bin = 2;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_3X3:
                    bin = 3;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_3X3_VOFFCHIP:
                    bin = 3;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_9X9:
                    bin = 9;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_NX1:
                    bin = 1;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_NX2:
                    bin = 2;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_NX3:
                    bin = 3;
                    break;
                default:
                    bin = 0;
                    break;
            }
            return bin;
        }


        private short ConvertReadoutToBinning(SBIG.READOUT_BINNING_MODE binning)
        {
            short bin = (short)ReadoutModeList.Find(r => r.mode == Binning).pixel_width;
            return bin;
        }

        private SBIG.READOUT_BINNING_MODE ConvertBinningToReadout(short binning)
        {
            SBIG.READOUT_BINNING_MODE readout = ReadoutModeList.Find(r => r.pixel_width == binning).mode;
            return readout;
        }

        public int[] FocusOffsets // Fake implementation of offsets. Who uses FW-provided offsets?
        {
            get
            {
                List<int> offsets = new List<int>();
                for (int i = 1; i <= FWPositions; i++)
                {
                    offsets.Add(0);
                }
                return offsets.ToArray();
            }

        }

        public string[] Names  // Fake implementation of Names. Set the names in your application.
        {
            get
            {
                List<string> positions = new List<string>();
                for (int i=1; i <= FWPositions; i++)
                {
                    positions.Add("Filter "+i);
                }
                return positions.ToArray();
            }
        }

        public short Position 
        { 
            get {
                SBIG.CFWResults fwresults = GetFWPosition();
                Debug.LogMessage("Position", "FW position is " + fwresults.cfwPosition);
                return (short)(fwresults.cfwPosition-1);
             }
            set {
                Debug.LogMessage("Position", "Set FW position to " + value+1);
                SetFWPosition((short)(value+1));
            }
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Camera";
                Debug.TraceEnabled = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
                comPort = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Camera";
                driverProfile.WriteValue(driverID, traceStateProfileName, Debug.TraceEnabled.ToString());
                driverProfile.WriteValue(driverID, comPortProfileName, comPort.ToString());
            }
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            Debug.LogMessage(identifier, msg);
        }
        #endregion
    }
}
