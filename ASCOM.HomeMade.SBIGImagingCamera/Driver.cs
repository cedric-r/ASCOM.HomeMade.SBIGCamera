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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ASCOM.HomeMade.SBIGCommon;

namespace ASCOM.HomeMade.SBIGImagingCamera
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63ad-c913-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(Camera.driverID)]
    public class Camera : ICameraV3, ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal const string driverID = "ASCOM.HomeMade.SBIGImagingCamera";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        public static string driverDescription = "ASCOM SBIG Imaging Camera Driver.";

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        private ASCOM.HomeMade.SBIGCommon.Debug debug = null;
        private bool connectionState = false;

        private SBIGServer server = new SBIGServer(driverID);
        private BackgroundWorker bw = null;
        private bool Shutdown = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMade"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            } catch(Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGImagingCamera_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            if (!ASCOM.HomeMade.SBIGCommon.Debug.Testing) ReadProfile(); // Read device configuration from the ASCOM Profile store

            debug.LogMessage("Camera", "Starting initialisation");

            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object

            debug.LogMessage("Camera", "Completed initialisation");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Shutdown)
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
                debug.LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            debug.LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
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
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public bool Connected
        {
            get
            {
                debug.LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                debug.LogMessage("Connected", "Set {0}", value);
                if (value && IsConnected)
                {
                    debug.LogMessage("Connected", "Already connected");
                    GetCameraSpecs();

                    return;
                }
                if (!value && !IsConnected)
                {
                    debug.LogMessage("Connected", "Not connected yet");

                    return;
                }

                try
                {

                    if (value)
                    {
                        connectionState = server.Connect();

                        if (!connectionState)
                        { 
                            debug.LogMessage("Connected Set", $"No USB camera found");
                            throw new DriverException("No suitable camera found");
                        }
                        else
                        {
                            debug.LogMessage("Connected Set", $"Connected to USB camera");

                            GetCameraSpecs();
                        }
                    }
                    else
                    {
                        debug.LogMessage("Connected Set", "Disconnection requested");
                        if (IsConnected)
                        {
                            server.Disconnect();

                            if (bw!=null)
                            {
                                debug.LogMessage("Connected Set", "Shutting down background worker");
                                Shutdown = true;
                                try
                                {
                                    bw.CancelAsync();
                                }
                                catch (Exception) { }
                                bw = null;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e));
                }
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                debug.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description
                string driverInfo = "Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                debug.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                debug.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                debug.LogMessage("InterfaceVersion Get", "3");
                return Convert.ToInt16("3");
            }
        }

        public string Name
        {
            get
            {
                string name = "ASCOM driver for SBIG cameras";
                debug.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region ICamera Implementation

        private const double MAX_EXPOSURE_TIME = 167777.16;
        private const double MIN_EXPOSURE_TIME = 0; // This is camera dependent. So requesting a shorter exposure than the camera can do will result in minimum allowed exposure
        private const double EXPOSURE_RESOLTION = 0.0;

        private SBIG.CAMERA_TYPE CameraType;
        private string CameraName;
        private bool ColorCamera = false;
        private bool RequiresExposureParams2 = true;
        private SBIG.READOUT_BINNING_MODE Binning = 0;
        private SBIG.QueryTemperatureStatusResults2 Cooling;
        private double CCDTempTarget = 0;
        private bool CCDTempTargetSet = false;
        private bool HasMechanicalShutter = false;
        private CameraStates CurrentCameraState = CameraStates.cameraIdle;
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
        private UInt32[,] cameraImageArray;
        private object[,] cameraImageArrayVariant;
        private bool FastReadoutRequested = false;

        private SBIG.StartExposureParams2 exposureParams2;

        public void AbortExposure()
        {
            if (!IsConnected) throw new NotConnectedException("Camera is not connected");

            CurrentCameraState = CameraStates.cameraIdle;
            if (RequiresExposureParams2)
            {
                debug.LogMessage("AbortExposure", "Aborting...");
                server.AbortExposure(exposureParams2);
            }
            else
            {
                debug.LogMessage("AbortExposure", "AbortExposure is not supported");
                throw new InvalidOperationException("This driver does not support legacy calls");
            }
        }

        public short BayerOffsetX
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("BayerOffsetX Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", false);
            }
        }

        public short BayerOffsetY
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("BayerOffsetY Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", true);
            }
        }

        public short BinX
        {
            get
            {
                debug.LogMessage("BinX Get", "Bin size is "+ GetCurrentReadoutMode(Binning).pixel_width);
                return (short)GetCurrentReadoutMode(Binning).pixel_width == 0 ? (short)1 : (short)GetCurrentReadoutMode(Binning).pixel_width;
            }
            set
            {
                debug.LogMessage("BinX Set", value.ToString());
                Binning = ConvertBinningToReadout(value);

            }
        }

        public short BinY
        {
            get
            {
                debug.LogMessage("BinY Get", "Bin size is " + GetCurrentReadoutMode(Binning).pixel_height);
                return (short)GetCurrentReadoutMode(Binning).pixel_height == 0 ? (short)1 : (short)GetCurrentReadoutMode(Binning).pixel_height;
            }
            set
            {
                debug.LogMessage("BinY Set", value.ToString());
                Binning = ConvertBinningToReadout(value);
            }
        }

        public double CCDTemperature
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CCDTemperature Get", "CCD temperature is " + Cooling.imagingCCDTemperature);
                return Cooling.imagingCCDTemperature;
            }
        }

        public CameraStates CameraState
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CameraState Get", CurrentCameraState.ToString());
                return CurrentCameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CameraXSize Get", ccdWidth.ToString());
                return ccdWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CameraYSize Get", ccdHeight.ToString());
                return ccdHeight;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                if (RequiresExposureParams2)
                {
                    debug.LogMessage("CanAbortExposure Get", true.ToString());
                    return true;
                }
                else
                {
                    debug.LogMessage("CanAbortExposure Get", false.ToString());
                    return false;
                }
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                debug.LogMessage("CanAsymmetricBin Get", false.ToString());
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                bool fr = CameraName.Contains("STF-8300"); // Only STF-8300 can use fastreadout
                debug.LogMessage("CanFastReadout Get", fr.ToString());
                return fr;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                debug.LogMessage("CanGetCoolerPower Get", true.ToString());
                return true;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                debug.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                debug.LogMessage("CanSetCCDTemperature Get", true.ToString());
                return true;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                if (RequiresExposureParams2)
                {
                    debug.LogMessage("CanStopExposure Get", true.ToString());
                    return true;
                }
                else
                {
                    debug.LogMessage("CanStopExposure Get", false.ToString());
                    return false;
                }
            }
        }

        public bool CoolerOn
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CoolerOn Get", "Cooler is " + Cooling.coolingEnabled.value.ToString());
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
                    server.UnivDrvCommand(SBIG.PAR_COMMAND.CC_SET_TEMPERATURE_REGULATION2, tparams);
                    if (value) debug.LogMessage("CoolerOn Set", "Coller On at " + tparams.ccdSetpoint);
                    else debug.LogMessage("CoolerOn Set", "Coller Off");

                    if (bw == null)
                    {
                        debug.LogMessage("Camera", "Starting background worker");
                        Shutdown = false;
                        bw = new BackgroundWorker();
                        bw.DoWork += bw_DoWork;
                        bw.RunWorkerAsync();
                    }
                }
                catch (Exception e)
                {
                    debug.LogMessage("CoolerOn Set", "Error: " + Utils.DisplayException(e));
                    throw;
                }

            }
        }

        public double CoolerPower
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CoolerPower Get", "Cooler power is " + Cooling.imagingCCDPower.ToString());
                return Cooling.imagingCCDPower;
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ElectronsPerADU Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ElectronsPerADU", false);
            }
        }

        public double ExposureMax
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureMax Get", "Exposure max is " + MAX_EXPOSURE_TIME.ToString());
                return MAX_EXPOSURE_TIME;
            }
        }

        public double ExposureMin
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureMin Get", "Exposure min is " + MIN_EXPOSURE_TIME.ToString());
                return MIN_EXPOSURE_TIME;
            }
        }

        public double ExposureResolution
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureResolution Get", "Exposure resolution min is " + EXPOSURE_RESOLTION.ToString());
                return EXPOSURE_RESOLTION;
            }
        }

        public bool FastReadout
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (CanFastReadout)
                {
                    debug.LogMessage("FastReadout Get", "FastReadout=" + FastReadoutRequested.ToString());
                    return FastReadoutRequested;
                }
                else
                {
                    throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
                }
            }
            set
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (CanFastReadout)
                {
                    debug.LogMessage("FastReadout Set", "FastReadout=" + value.ToString());
                    FastReadoutRequested = value;
                }
                else
                {
                    throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
                }
            }
        }

        public double FullWellCapacity
        {
            get
            {
                debug.LogMessage("FullWellCapacity Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FullWellCapacity", false);
            }
        }

        public short Gain
        {
            get
            {
                debug.LogMessage("Gain Get", "CCD cameras don't set gain");
                throw new ASCOM.PropertyNotImplementedException("Gain", false);
                /*
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                SBIG.READOUT_INFO ri = ReadoutModeList.Find(r => r.mode == Binning);
                debug.LogMessage("Gain Get", "Camera gain is " + ri.gain.ToString());
                return (short)ri.gain;
                */
            }
            set
            {
                debug.LogMessage("Gain Set", "CCD cameras don't set gain");
                throw new ASCOM.PropertyNotImplementedException("Gain", false);
                /*
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("Gain Set", "Do nothing");
                */
            }
        }

        public short GainMax
        {
            get
            {
                debug.LogMessage("GainMax Get", "CCD cameras don't set gain");
                throw new ASCOM.PropertyNotImplementedException("GainMax", false);
                /*
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                short maxgain = 0;
                foreach (var ri in ReadoutModeList)
                {
                    if (ri.gain > maxgain) maxgain = (short)ri.gain;
                }

                debug.LogMessage("GainMax Get", "Max gain is " + maxgain);
                return maxgain;
                */
            }
        }

        public short GainMin
        {
            get
            {
                debug.LogMessage("GainMin Get", "CCD cameras don't set gain");
                throw new ASCOM.PropertyNotImplementedException("GainMin", false);
                /*
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                short mingain = 0;
                foreach (var ri in ReadoutModeList)
                {
                    if (ri.gain < mingain) mingain = (short)ri.gain;
                }

                debug.LogMessage("GainMax Get", "Min gain is " + mingain);
                return mingain;
                */
            }
        }

        public ArrayList Gains
        {
            get
            {
                debug.LogMessage("Gains Get", "CCD cameras don't set gain");
                throw new ASCOM.PropertyNotImplementedException("Gains", false);
                /*
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                ArrayList list = new ArrayList();
                foreach(var ri in ReadoutModeList)
                {
                    if (!list.Contains(ri.gain)) list.Add(ri.gain);
                }
                debug.LogMessage("Gains Get", "Returning existing gains");
                return list;
                */
            }
        }

        public bool HasShutter
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("HasShutter Get", HasMechanicalShutter.ToString());
                return HasMechanicalShutter;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("HeatSinkTemperature Get", "Heatsink temperature is " + Cooling.heatsinkTemperature.ToString());
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
                    debug.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArray before the first image has been taken!");
                }

                debug.LogMessage("ImageArray Get", "Returning image array");

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
                    debug.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArrayVariant before the first image has been taken!");
                }
                debug.LogMessage("ImageArrayVariant Get", "Returning image array");

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
                debug.LogMessage("ImageReady Get", cameraImageReady.ToString());
                return cameraImageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("IsPulseGuiding Get", "Not implemented");
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
                    debug.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureDuration before the first image has been taken!");
                }
                debug.LogMessage("LastExposureDuration Get", cameraLastExposureDuration.ToString());
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
                    debug.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!");
                }
                debug.LogMessage("LastExposureStartTime Get", exposureStart.ToString());
                return exposureStart.ToString();
            }
        }

        public int MaxADU
        {
            get
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("MaxADU Get", "60000");
                return 50000;
            }
        }

        public short MaxBinX
        {
            get
            {
                debug.LogMessage("MaxBinX Get", "3");
                return 3;
            }
        }

        public short MaxBinY
        {
            get
            {
                debug.LogMessage("MaxBinY Get", "3");
                return 3;
            }
        }

        public int NumX
        {
            get
            {
                debug.LogMessage("NumX Get", cameraNumX.ToString());
                return cameraNumX;
            }
            set
            {
                cameraNumX = value;
                debug.LogMessage("NumX set", value.ToString());
            }
        }

        public int NumY
        {
            get
            {
                debug.LogMessage("NumY Get", cameraNumY.ToString());
                return cameraNumY;
            }
            set
            {
                cameraNumY = value;
                debug.LogMessage("NumY set", value.ToString());
            }
        }

        public short PercentCompleted
        {
            get
            {
                debug.LogMessage("PercentCompleted Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("PercentCompleted", false);
            }
        }

        public double PixelSizeX
        {
            get
            {
                debug.LogMessage("PixelSizeX Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public double PixelSizeY
        {
            get
            {
                debug.LogMessage("PixelSizeY Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            debug.LogMessage("PulseGuide", "Not implemented");
            throw new ASCOM.MethodNotImplementedException("PulseGuide");
        }

        public short ReadoutMode
        {
            get
            {
                debug.LogMessage("ReadoutMode Get", "Binning is " + Binning);
                return (short)Binning;
            }
            set
            {
                debug.LogMessage("ReadoutMode Set", "Setting binning to " + value);
                Binning = (SBIG.READOUT_BINNING_MODE)value;
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                debug.LogMessage("ReadoutModes Get", "Returning readout modes");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                ArrayList list = new ArrayList();
                foreach (var readoutmode in ReadoutModeList)
                {
                    list.Add(readoutmode.mode.ToString());
                }
                return list;
            }
        }

        public string SensorName
        {
            get
            {
                debug.LogMessage("SensorName Get", "Camera name is "+CameraName);
                return CameraName;
            }
        }

        public SensorType SensorType
        {
            get
            {
                debug.LogMessage("SensorType Get", "Camera type is "+ColorCamera.ToString());
                if (ColorCamera) return SensorType.RGGB; // Guessing here
                else return SensorType.Monochrome;
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                debug.LogMessage("SetCCDTemperature Get", "CCD temperature target is " + CCDTempTarget);
                return CCDTempTarget;
            }
            set
            {
                CCDTempTarget = value;
                CCDTempTargetSet = true;
                if (CoolerOn) CoolerOn = true;
                debug.LogMessage("SetCCDTemperature Set", "CCD temperature target is " + CCDTempTarget);
            }
        }

        public void StartExposure(double Duration, bool Light)
        {
            try
            {
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("StartExposure", Duration.ToString() + " " + Light.ToString());
                if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");
                if (cameraNumX > ccdWidth) throw new InvalidValueException("StartExposure", cameraNumX.ToString(), ccdWidth.ToString());
                if (cameraNumY > ccdHeight) throw new InvalidValueException("StartExposure", cameraNumY.ToString(), ccdHeight.ToString());
                if (cameraStartX > ccdWidth) throw new InvalidValueException("StartExposure", cameraStartX.ToString(), ccdWidth.ToString());
                if (cameraStartY > ccdHeight) throw new InvalidValueException("StartExposure", cameraStartY.ToString(), ccdHeight.ToString());

                cameraLastExposureDuration = Duration;
                exposureStart = DateTime.Now;

                if (!RequiresExposureParams2)
                {
                    debug.LogMessage("StartExposure", "This driver does not support legacy calls. Using new call anyway");
                    //throw new InvalidOperationException("This driver does not support legacy calls");
                }
                exposureParams2 = new SBIG.StartExposureParams2
                {
                    ccd = SBIG.CCD_REQUEST.CCD_IMAGING,
                    abgState = SBIG.ABG_STATE7.ABG_LOW7,
                    openShutter = Light ? SBIG.SHUTTER_COMMAND.SC_OPEN_SHUTTER : SBIG.SHUTTER_COMMAND.SC_CLOSE_SHUTTER,
                    readoutMode = Binning,
                    exposureTime = FastReadoutRequested ? Convert.ToUInt32(Duration) | SBIG.EXP_FAST_READOUT : Convert.ToUInt32(Duration),
                    width = Convert.ToUInt16(cameraNumX), // This is in binned pixels. Check is this is right
                    height = Convert.ToUInt16(cameraNumY),
                    left = (ushort)cameraStartX,
                    top = (ushort)cameraStartY
                };

                debug.LogMessage("StartExposure", "Exposing");
                CurrentCameraState = CameraStates.cameraExposing;
                server.UnivDrvCommand(SBIG.PAR_COMMAND.CC_START_EXPOSURE2, exposureParams2);

                // read out the image
                debug.LogMessage("StartExposure", "Reading");
                CurrentCameraState = CameraStates.cameraReading;

                //cameraImageArray = SBIG.WaitEndAndReadoutExposure32(exposureParams2);
                server.WaitExposure();

                var data = new UInt16[exposureParams2.height, exposureParams2.width];
                server.ReadoutData(exposureParams2, ref data);

                cameraImageArray = new UInt32[exposureParams2.height, exposureParams2.width];
                for (int i = 0; i < exposureParams2.height; i++)
                    for (int j = 0; j < exposureParams2.width; j++)
                        cameraImageArray[i, j] = data[i, j];

                CurrentCameraState = CameraStates.cameraIdle;
                debug.LogMessage("StartExposure", "Done");
                cameraImageReady = true;
            }
            catch (Exception e)
            {
                debug.LogMessage("StartExposure", "Error: " + Utils.DisplayException(e));
                throw;
            }
        }

        public int StartX
        {
            get
            {
                debug.LogMessage("StartX Get", cameraStartX.ToString());
                return cameraStartX;
            }
            set
            {
                cameraStartX = value;
                debug.LogMessage("StartX Set", value.ToString());
            }
        }

        public int StartY
        {
            get
            {
                debug.LogMessage("StartY Get", cameraStartY.ToString());
                return cameraStartY;
            }
            set
            {
                cameraStartY = value;
                debug.LogMessage("StartY set", value.ToString());
            }
        }

        public void StopExposure()
        {
            AbortExposure();
        }
        public int OffsetMin { get { throw new ASCOM.PropertyNotImplementedException("OffsetMin", false); } }
        public int OffsetMax { get { throw new ASCOM.PropertyNotImplementedException("OffsetMax", false); } }
        public int Offset { get { throw new ASCOM.PropertyNotImplementedException("Offset", false); } set { throw new ASCOM.PropertyNotImplementedException("Offset", false); } }
        public ArrayList Offsets { get { throw new ASCOM.PropertyNotImplementedException("Offsets", false); } }
        public double SubExposureDuration { get { throw new ASCOM.PropertyNotImplementedException("SubExposureDuration", false); } set { throw new ASCOM.PropertyNotImplementedException("SubExposureDuration", false); } }

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
                if (CurrentCameraState == CameraStates.cameraIdle) // SBIG quirk: you can't access the cooler during exposures.
                {
                    try
                    {
                        debug.LogMessage("Camera", "Getting cooling information");
                        // query temperature
                        server.UnivDrvCommand(
                            SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS,
                             new SBIG.QueryTemperatureStatusParams()
                             {
                                 request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
                             },
                            out Cooling);
                    }
                    catch (Exception e)
                    {
                        debug.LogMessage("GetCoolingInfo", "Error: " + Utils.DisplayException(e));
                    }
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
                debug.LogMessage("IsConnected", "connectionState=" + connectionState.ToString());
                return connectionState;
            }
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

        private SBIG.READOUT_INFO GetCurrentReadoutMode(SBIG.READOUT_BINNING_MODE binning)
        {
            return ReadoutModeList.Find(r => r.mode == Binning);
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

        private void GetCameraSpecs()
        {
            debug.LogMessage("Connected Set", $"Getting camera info");
            // query camera info
            var gcir0 = new SBIG.GetCCDInfoResults0();
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_IMAGING
                },
                out gcir0);
            // now print it out
            debug.LogMessage("Connected Set", $"Firmware version: {gcir0.firmwareVersion >> 8}.{gcir0.firmwareVersion & 0xFF}");
            debug.LogMessage("Connected Set", $"Camera type: {gcir0.cameraType}");
            CameraType = gcir0.cameraType;
            debug.LogMessage("Connected Set", $"Camera name: {gcir0.name}");
            CameraName = gcir0.name;
            debug.LogMessage("Connected Set", $"Readout modes: {gcir0.readoutModes}");
            Binning = SBIG.READOUT_BINNING_MODE.RM_1X1;
            for (int i = 0; i < gcir0.readoutModes; i++)
            {
                SBIG.READOUT_INFO ri = gcir0.readoutInfo[i];
                ri.pixel_height = (uint)ConvertReadoutModeToBinning(ri.mode);
                ri.pixel_width = (uint)ConvertReadoutModeToBinning(ri.mode);
                ReadoutModeList.Add(ri);
                debug.LogMessage("Connected Set", $"    Binning mode: {ri.mode}");
                debug.LogMessage("Connected Set", $"    Width: {ri.width}");
                debug.LogMessage("Connected Set", $"    Height: {ri.height}"); // Don't trust this, there is at least 1 bug in the STF-8300 that reports a height of 0 for mode RM_NX1
                debug.LogMessage("Connected Set", $"    Gain: {ri.gain >> 8}.{ri.gain & 0xFF} e-/ADU");
                debug.LogMessage("Connected Set", $"    Pixel width: {ri.pixel_width}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
                debug.LogMessage("Connected Set", $"    Pixel height: {ri.pixel_height}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
            }

            // get extended info
            var gcir2 = new SBIG.GetCCDInfoResults2();
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED
                },
                out gcir2);
            // print it out
            debug.LogMessage("Connected Set", $"Bad columns: {gcir2.badColumns} = ");
            debug.LogMessage("Connected Set",
                $"{gcir2.columns[0]}, {gcir2.columns[1] }, " +
                $"{gcir2.columns[2]}, { gcir2.columns[3]}");
            debug.LogMessage("Connected Set", $"ABG: {gcir2.imagingABG}");
            debug.LogMessage("Connected Set", $"Serial number: {gcir2.serialNumber}");
            /*
            // get extended info
            var gcir3 = new SBIG.GetCCDInfoResults3();
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED_5C
                },
                out gcir3);
            // print it out
            debug.LogMessage("Connected Set", $"Filter wheel: {gcir3.filterType}");
            switch (gcir3.filterType)
            {
                case SBIG.FILTER_TYPE.FW_UNKNOWN:
                    debug.LogMessage("Connected Set", $"    Unknown");
                    break;
                case SBIG.FILTER_TYPE.FW_VANE:
                    debug.LogMessage("Connected Set", $"    Vane");
                    FilterWheelPresent = true;
                    break;
                case SBIG.FILTER_TYPE.FW_EXTERNAL:
                    debug.LogMessage("Connected Set", $"    External");
                    FilterWheelPresent = true;
                    break;
                case SBIG.FILTER_TYPE.FW_FILTER_WHEEL:
                    debug.LogMessage("Connected Set", $"    Standard");
                    FilterWheelPresent = true;
                    break;
                default:
                    debug.LogMessage("Connected Set", $"    Unexpected");
                    break;
            }
            */
            // get extended info
            var gcir4 = new SBIG.GetCCDInfoResults4();
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED2_IMAGING
                },
                out gcir4);
            // print it out
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 0)) debug.LogMessage("Connected Set", $"CCD is frame transfer device");
            else debug.LogMessage("Connected Set", $"CCD is full frame device");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 1)) debug.LogMessage("Connected Set", $"Electronic shutter");
            else debug.LogMessage("Connected Set", $"No electronic shutter");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 2)) debug.LogMessage("Connected Set", $"Remote guide head present");
            else debug.LogMessage("Connected Set", $"No remote guide head");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 3)) debug.LogMessage("Connected Set", $"Supports Biorad TDI acquisition more");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 4)) debug.LogMessage("Connected Set", $"AO8 detected");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 5)) debug.LogMessage("Connected Set", $"Camera contains an internal frame buffer");
            if (Utils.IsBitSet(gcir4.capabilitiesBits, 6))
            {
                debug.LogMessage("Connected Set", $"Camera requires StartExposure2 command");
                RequiresExposureParams2 = true;
            }
            else
            {
                debug.LogMessage("Connected Set", $"Camera requires StartExposure command");
                RequiresExposureParams2 = false;
            }

            // get extended info
            var gcir6 = new SBIG.GetCCDInfoResults6();
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_GET_CCD_INFO,
                new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED3
                },
                out gcir6);
            // print it out
            if (Utils.IsBitSet(gcir6.cameraBits, 0)) debug.LogMessage("Connected Set", $"STXL camera");
            else debug.LogMessage("Connected Set", $"STX camera");
            if (Utils.IsBitSet(gcir6.cameraBits, 1))
            {
                HasMechanicalShutter = false;
                debug.LogMessage("Connected Set", $"No mechanical shutter");
            }
            else
            {
                HasMechanicalShutter = true;
                debug.LogMessage("Connected Set", $"Mechanical shutter");
            }
            if (Utils.IsBitSet(gcir6.ccdBits, 0))
            {
                debug.LogMessage("Connected Set", $"Color CCD");
                ColorCamera = true;
                if (Utils.IsBitSet(gcir6.ccdBits, 1)) debug.LogMessage("Connected Set", $"Truesense color matrix");
                else debug.LogMessage("Connected Set", $"Bayer color matrix");
            }
            else
            {
                ColorCamera = false;
                debug.LogMessage("Connected Set", $"Mono CDD");
            }

            // query temperature
            server.UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_QUERY_TEMPERATURE_STATUS,
                 new SBIG.QueryTemperatureStatusParams()
                 {
                     request = SBIG.QUERY_TEMP_STATUS_REQUEST.TEMP_STATUS_ADVANCED2
                 },
                out Cooling);
            debug.LogMessage("Connected", "CCD temperature is " + Cooling.imagingCCDTemperature);
            debug.LogMessage("Connected", "CCD power is " + Cooling.imagingCCDPower);
            debug.LogMessage("Connected", "CCD cooler is " + Cooling.coolingEnabled.value.ToString());
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Camera";
                try
                {
                    ASCOM.HomeMade.SBIGCommon.Debug.TraceEnabled = Convert.ToBoolean(driverProfile.GetValue(driverID, "TraceDebug", "", "false"));
                }
                catch (Exception) { }
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
                driverProfile.WriteValue(driverID, "TraceDebug", ASCOM.HomeMade.SBIGCommon.Debug.TraceEnabled.ToString());
            }
        }
        #endregion
    }
}
