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
using System.Reflection;
using ASCOM.HomeMade.SBIGClient;
using ASCOM.HomeMade.SBIGHub;

namespace ASCOM.HomeMade.SBIGImagingCamera
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63ad-c913-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServedClassName(Camera.driverDescription)]
    [ProgId(Camera.driverID)]
    [ComVisible(true)]
    public class Camera : ISBIGCamera, ICameraV3, ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        public const string driverID = "ASCOM.HomeMade.SBIGImagingCamera";
        internal string DriverID {  get { return Camera.driverID; } }
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        public const string driverDescription = "ASCOM SBIG Imaging Camera Driver.";

        private SBIGCommon.Debug debug = null;

        private SBIGClient.SBIGClient server = null;
        internal static string IPAddress = "";

        private BackgroundWorker bw = null;
        private BackgroundWorker imagingWorker = null;
        private bool Shutdown = false;
        private ImageTakerThread imagetaker = null;

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

            debug.LogMessage(driverID + " v" + DriverVersion);
            int nProcessID = Process.GetCurrentProcess().Id;
            debug.LogMessage("Process ID: " + nProcessID);

            CameraType = SBIG.CCD_REQUEST.CCD_IMAGING;

            debug.LogMessage("Camera", "Completed initialisation");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            CoolingInfoThread cool = new CoolingInfoThread(IPAddress);
            while (!Shutdown)
            {
                Cooling = cool.GetCoolingInfo();
                Thread.Sleep(500);
            }
            cool.Close();
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
            if (server != null)
            {
                try
                {
                    if (IsConnected)
                    {
                        Shutdown = true;
                        server.Close();
                        server = null;
                    }
                }
                catch (Exception) { }
            }
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

                if (server == null) server = new SBIGClient.SBIGClient();

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
                        bool connectionState = server.Connect(IPAddress);

                        if (!connectionState)
                        { 
                            debug.LogMessage("Connected Set", $"No USB camera found");
                            throw new DriverException("No suitable camera found");
                        }
                        else
                        {
                            debug.LogMessage("Connected Set", $"Connected to camera");

                            GetCameraSpecs();

                            debug.LogMessage("Camera", "Starting background cooler worker");
                            Shutdown = false;
                            bw = new BackgroundWorker();
                            bw.DoWork += bw_DoWork;
                            bw.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        debug.LogMessage("Connected Set", "Disconnection requested");
                        if (IsConnected)
                        {
                            Shutdown = true;
                            try
                            {
                                AbortExposure();
                            }
                            catch(Exception ex)
                            {
                                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(ex));
                            }

                            try
                            {
                                server.Disconnect();
                            }
                            catch (Exception ex)
                            {
                                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(ex));
                            }
                            server = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e));
                    throw new ASCOM.DriverException(Utils.DisplayException(e));
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
                string driverInfo = "Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Build);
                debug.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Build);
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

        private string CameraName = "";
        private bool ColorCamera = false;
        private int ColorCameraType = 0;
        private int AdSize = 16;
        private double CCDTempTarget = 0;
        private bool CCDTempTargetSet = false;
        private bool HasMechanicalShutter = false;
        private List<SBIG.READOUT_INFO> ReadoutModeList = new List<SBIG.READOUT_INFO>();
        private int ccdWidth { get { return ReadoutModeList.Find(r => r.mode == 0).width; } }
        private int ccdHeight { get { return ReadoutModeList.Find(r => r.mode == 0).height; } }
        private double pixelSize { get { return ((double)ReadoutModeList.Find(r => r.mode == Binning).pixel_height)/100; } } // We assume square pixels

        private DateTime exposureStart = DateTime.MinValue;
        private double cameraLastExposureDuration = 0.0;

        private SBIG.StartExposureParams2 exposureParams2;
        private SBIG.QueryTemperatureStatusResults2 Cooling;

        public void AbortExposure()
        {
            if (!IsConnected) throw new NotConnectedException("Camera is not connected");

            try
            {
                // I removed the check that the camera requires StartExposureParams2
                debug.LogMessage("AbortExposure", "Aborting...");
                CurrentCameraState = CameraStates.cameraIdle;

                if (imagetaker != null)
                {
                    imagetaker.StopExposure = true;
                }
            }
            catch(Exception ex)
            {
                debug.LogMessage("AbortExposure", "Error: "+ Utils.DisplayException(ex));
                throw new ASCOM.DriverException(Utils.DisplayException(ex));
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
                debug.LogMessage("BinX Get", "Bin size is "+ ConvertReadoutModeToBinning(Binning));
                return (short)ConvertReadoutModeToBinning(Binning);
            }
            set
            {
                debug.LogMessage("BinX Set", value.ToString());
                if (value < 1) throw new ASCOM.InvalidValueException("Bin cannot be 0 or less");
                if (value > MaxBinX) throw new ASCOM.InvalidValueException("Bin cannot be above " + MaxBinX);
                Binning = ConvertBinningToReadout(value);

            }
        }

        public short BinY
        {
            get
            {
                debug.LogMessage("BinY Get", "Bin size is " + ConvertReadoutModeToBinning(Binning));
                return (short)ConvertReadoutModeToBinning(Binning);
            }
            set
            {
                debug.LogMessage("BinY Set", value.ToString());
                if (value < 1) throw new ASCOM.InvalidValueException("Bin cannot be 0 or less");
                if (value > MaxBinY) throw new ASCOM.InvalidValueException("Bin cannot be above " + MaxBinY);
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
                // I removed the check that the camera requires StartExposureParams2
                debug.LogMessage("CanAbortExposure Get", true.ToString());
                return true;
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
                bool fr = CameraName.Contains("STF-8300") || CameraName.Contains("STT-8300"); // Only STF-8300 can use fastreadout
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
                    if (value)
                    {
                        if (CCDTempTargetSet)
                        {
                            tparams.ccdSetpoint = CCDTempTarget;
                        }
                        else
                        {
                            tparams.ccdSetpoint = Cooling.ambientTemperature;
                        }
                        tparams.regulation = SBIG.TEMPERATURE_REGULATION.REGULATION_ON;
                    }
                    else tparams.regulation = SBIG.TEMPERATURE_REGULATION.REGULATION_OFF;
                    server.CC_SET_TEMPERATURE_REGULATION2(tparams);
                    if (value)
                    {
                        debug.LogMessage("CoolerOn Set", "Coller On at " + tparams.ccdSetpoint);
                    }
                    else
                    {
                        debug.LogMessage("CoolerOn Set", "Cooler Off");
                    }

                }
                catch (Exception e)
                {
                    debug.LogMessage("CoolerOn Set", "Error: " + Utils.DisplayException(e));
                    throw new ASCOM.DriverException(Utils.DisplayException(e));
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
                        cameraImageArrayVariant[j, i] = cameraImageArray.GetValue(j, i);
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
                return exposureStart.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
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
                SensorType type;
                if (ColorCamera)
                {
                    if (ColorCameraType == 0) type = SensorType.RGGB; // Guessing here
                    else type = SensorType.LRGB;
                }
                else type = SensorType.Monochrome;
                debug.LogMessage("SensorType Get", "Camera type is " + type.ToString());
                return type;
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
                if (value > 100 || value < -100)
                    throw new ASCOM.InvalidValueException("Target temperature is too low of too high");
                CCDTempTarget = value;
                CCDTempTargetSet = true;
                if (CoolerOn) CoolerOn = true;
                debug.LogMessage("SetCCDTemperature Set", "CCD temperature target is " + CCDTempTarget);
            }
        }

        public void StartExposure(double Duration, bool Light)
        {
            cameraImageReady = false;
            cameraImageArray = null;

            if (!IsConnected) throw new NotConnectedException("Camera is not connected");
            debug.LogMessage("StartExposure", Duration.ToString() + " " + Light.ToString());
            if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");
            if (cameraNumX > (ccdWidth / BinX)) throw new InvalidValueException("StartExposure", cameraNumX.ToString(), ccdWidth.ToString());
            if (cameraNumY > (ccdHeight / BinY)) throw new InvalidValueException("StartExposure", cameraNumY.ToString(), ccdHeight.ToString());
            if (cameraStartX > (ccdWidth / BinX)) throw new InvalidValueException("StartExposure", cameraStartX.ToString(), ccdWidth.ToString());
            if (cameraStartY > (ccdHeight / BinY)) throw new InvalidValueException("StartExposure", cameraStartY.ToString(), ccdHeight.ToString());

            try
            {
                durationRequest = Duration;
                lightRequest = Light;
                cameraLastExposureDuration = durationRequest;
                exposureStart = DateTime.Now;
                imagingWorker = new BackgroundWorker();
                imagingWorker.DoWork += bw_TakeImage;
                imagingWorker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                throw new ASCOM.DriverException(Utils.DisplayException(ex));
            }
        }

        private void bw_TakeImage(object sender, DoWorkEventArgs e)
        {
            try
            {
                imagetaker = new ImageTakerThread(this, IPAddress);
                imagetaker.TakeImage();
                imagetaker = null;
            }
            catch(Exception ex)
            {
                debug.LogMessage("bw_TakeImage", "Error: " + Utils.DisplayException(ex));
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

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
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

        private SBIG.READOUT_BINNING_MODE ConvertBinningToReadout(short binning)
        {
            double nominalPixelWidth = ReadoutModeList.Find(r1 => r1.mode == 0).pixel_width;
            SBIG.READOUT_INFO ri = ReadoutModeList.Find(r => (r.pixel_width / nominalPixelWidth) == binning);
            SBIG.READOUT_BINNING_MODE readout = ri.mode;
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
            if (!IsConnected) throw new NotConnectedException("Not conneted to server");

            try
            {
                // query camera info
                var gcir0 = server.CCD_INFO(new SBIG.GetCCDInfoParams
                {
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_IMAGING
                });
                // now print it out
                debug.LogMessage("Connected Set", $"Firmware version: {gcir0.firmwareVersion >> 8}.{gcir0.firmwareVersion & 0xFF}");
                debug.LogMessage("Connected Set", $"Camera type: {gcir0.cameraType}");
                CameraModel = gcir0.cameraType;
                debug.LogMessage("Connected Set", $"Camera name: {gcir0.name}");
                CameraName = gcir0.name;
                debug.LogMessage("Connected Set", $"Readout modes: {gcir0.readoutModes}");
                Binning = SBIG.READOUT_BINNING_MODE.RM_1X1;
                for (int i = 0; i < gcir0.readoutModes; i++)
                {
                    SBIG.READOUT_INFO ri = gcir0.readoutInfo[i];
                    ri.pixel_width = Utils.BCDToUInt(ri.pixel_width);
                    ri.pixel_height = Utils.BCDToUInt(ri.pixel_height);
                    ReadoutModeList.Add(ri);
                    debug.LogMessage("Connected Set", $"    Binning mode: {ri.mode}");
                    debug.LogMessage("Connected Set", $"    Width: {ri.width}");
                    debug.LogMessage("Connected Set", $"    Height: {ri.height}"); // Don't trust this, there is at least 1 bug in the STF-8300 that reports a height of 0 for mode RM_NX1
                    debug.LogMessage("Connected Set", $"    Gain: {ri.gain >> 8}.{ri.gain & 0xFF} e-/ADU");
                    debug.LogMessage("Connected Set", $"    Pixel width: {ri.pixel_width}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
                    debug.LogMessage("Connected Set", $"    Pixel height: {ri.pixel_height}"); // The SBIG documentation is wrong: this isn't pixel size in microns, this is pixel binning size
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
                debug.LogMessage("Connected Set",
                    $"{gcir2.columns[0]}, {gcir2.columns[1] }, " +
                    $"{gcir2.columns[2]}, { gcir2.columns[3]}");
                debug.LogMessage("Connected Set", $"ABG: {gcir2.imagingABG}");
                debug.LogMessage("Connected Set", $"Serial number: {gcir2.serialNumber}");
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
                debug.LogMessage("Connected Set", $"Filter wheel: {gcir3.filterType}");
                switch (gcir3.filterType)
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
                if (gcir3.adSize == SBIG.AD_SIZE.AD_12_BITS) AdSize = 12;
                else if (gcir3.adSize == SBIG.AD_SIZE.AD_16_BITS) AdSize = 16;
                else AdSize = 16; // Go with 16 bits default
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
                    request = SBIG.CCD_INFO_REQUEST.CCD_INFO_EXTENDED2_IMAGING
                });
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
                    if (Utils.IsBitSet(gcir6.ccdBits, 1))
                    {
                        debug.LogMessage("Connected Set", $"Truesense color matrix");
                        ColorCameraType = 1;
                    }
                    else
                    {
                        debug.LogMessage("Connected Set", $"Bayer color matrix");
                        ColorCameraType = 0;
                    }
                }
                else
                {
                    ColorCamera = false;
                    debug.LogMessage("Connected Set", $"Mono CDD");
                }
            }
            catch (Exception e5)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e5));
                throw new ASCOM.DriverException(Utils.DisplayException(e5));
            }

            cameraNumX = ccdWidth;
            cameraNumY = ccdHeight;

            try
            {
                // query temperature
                Cooling = server.CC_QUERY_TEMPERATURE_STATUS();
                debug.LogMessage("Connected", "CCD temperature is " + Cooling.imagingCCDTemperature);
                debug.LogMessage("Connected", "CCD power is " + Cooling.imagingCCDPower);
                debug.LogMessage("Connected", "CCD cooler is " + Cooling.coolingEnabled.value.ToString());
            }
            catch (Exception e6)
            {
                debug.LogMessage("Connected Set", "Error: " + Utils.DisplayException(e6));
                throw new ASCOM.DriverException(Utils.DisplayException(e6));
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
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
                try
                {
                    ASCOM.HomeMade.SBIGCommon.Debug.TraceEnabled = Convert.ToBoolean(driverProfile.GetValue(driverID, "TraceDebug", "", "false"));
                }
                catch (Exception) { }
                try
                {
                    IPAddress = driverProfile.GetValue(driverID, "IPAddress", "", "");
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
                driverProfile.WriteValue(driverID, "IPAddress", IPAddress);
            }
        }
        #endregion
    }
}
