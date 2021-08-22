﻿/**
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
using System.Linq;

namespace ASCOM.HomeMade.SBIGCamera
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    public abstract class SBIGCamera : ISBIGCameraSpecs, ICameraV3, ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal string DriverID { get { return "ASCOM.HomeMade.SBIGCamera"; } }
        // TODO Change the descriptive string for your driver then remove this line

        protected BackgroundWorker bw = null;
        protected BackgroundWorker imagingWorker = null;
        protected bool Shutdown = false;
        protected ImageTakerThread imagetaker = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMade"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public SBIGCamera()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, DriverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new ASCOM.HomeMade.SBIGCommon.Debug(DriverID, Path.Combine(strPath, "SBIGCamera_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            debug.LogMessage("Camera", "Starting initialisation");

            debug.LogMessage(DriverID + " v" + DriverVersion);
            int nProcessID = Process.GetCurrentProcess().Id;
            debug.LogMessage("Process ID: " + nProcessID);

            CameraType = SBIG.CCD_REQUEST.CCD_IMAGING;

            debug.LogMessage("Camera", "Completed initialisation");
        }

        protected void bw_DoWork(object sender, DoWorkEventArgs e)
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
        public abstract void SetupDialog();
        
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
                    cameraInfo = GetCameraSpecs();

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

                            cameraInfo = GetCameraSpecs();

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
                            catch (Exception ex)
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
                debug.LogMessage("Description Get", "ASCOM SBIG Camera driver");
                return "ASCOM SBIG Camera driver";
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

        protected const double MAX_EXPOSURE_TIME = 167777.16;
        protected const double MIN_EXPOSURE_TIME = 0; // This is camera dependent. So requesting a shorter exposure than the camera can do will result in minimum allowed exposure
        protected const double EXPOSURE_RESOLTION = 0.0;

        protected double CCDTempTarget = 0;
        protected bool CCDTempTargetSet = false;

        protected DateTime exposureStart = DateTime.MinValue;
        protected double cameraLastExposureDuration = 0.0;

        protected SBIG.StartExposureParams2 exposureParams2;

        public void AbortExposure()
        {
            debug.LogMessage("AbortExposure", "Get");
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
            catch (Exception ex)
            {
                debug.LogMessage("AbortExposure", "Error: " + Utils.DisplayException(ex));
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
                debug.LogMessage("BinX", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("BinX Get", "Bin size is " + ConvertReadoutModeToXBinning(Binning));
                return (short)ConvertReadoutModeToXBinning(Binning);
            }
            set
            {
                debug.LogMessage("BinX Set", value.ToString());
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (value < 1) throw new ASCOM.InvalidValueException("Bin cannot be 0 or less");
                if (value > MaxBinX) throw new ASCOM.InvalidValueException("Bin cannot be above " + MaxBinX);
                Binning = ConvertBinningToReadout(value);

            }
        }

        public short BinY
        {
            get
            {
                debug.LogMessage("BinY", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("BinY Get", "Bin size is " + ConvertReadoutModeToYBinning(Binning));
                return (short)ConvertReadoutModeToYBinning(Binning);
            }
            set
            {
                debug.LogMessage("BinY Set", value.ToString());
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                if (value < 1) throw new ASCOM.InvalidValueException("Bin cannot be 0 or less");
                if (value > MaxBinY) throw new ASCOM.InvalidValueException("Bin cannot be above " + MaxBinY);
                Binning = ConvertBinningToReadout(value);
            }
        }

        public double CCDTemperature
        {
            get
            {
                debug.LogMessage("CCDTemperature", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CCDTemperature Get", "CCD temperature is " + Cooling.imagingCCDTemperature);
                return Cooling.imagingCCDTemperature;
            }
        }

        public CameraStates CameraState
        {
            get
            {
                debug.LogMessage("CameraState", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CameraState Get", CurrentCameraState.ToString());
                return CurrentCameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                debug.LogMessage("CameraXSize", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CameraXSize Get", ccdWidth.ToString());
                return ccdWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                debug.LogMessage("CameraYSize", "Get");
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
                debug.LogMessage("CanAbortExposure", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CanAbortExposure Get", true.ToString());
                return true;
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                debug.LogMessage("CanAsymmetricBin", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CanAsymmetricBin Get", false.ToString());
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                debug.LogMessage("CanFastReadout", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                bool fr = cameraInfo.name.Contains("STF-8300") || cameraInfo.name.Contains("STT-8300"); // Only STF-8300 can use fastreadout
                debug.LogMessage("CanFastReadout Get", fr.ToString());
                return fr;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                debug.LogMessage("CanGetCoolerPower", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CanGetCoolerPower Get", true.ToString());
                return true;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                debug.LogMessage("CanPulseGuide", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                debug.LogMessage("CanSetCCDTemperature", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CanSetCCDTemperature Get", true.ToString());
                return true;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                debug.LogMessage("CanStopExposure", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
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
                debug.LogMessage("CoolerOn", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CoolerOn Get", "Cooler is " + Cooling.coolingEnabled.value.ToString());
                return Cooling.coolingEnabled.value == 0 ? false : true;
            }
            set
            {
                try
                {
                    debug.LogMessage("CoolerOn", "Set");
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
                debug.LogMessage("CoolerPower", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("CoolerPower Get", "Cooler power is " + Cooling.imagingCCDPower.ToString());
                return Cooling.imagingCCDPower;
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                debug.LogMessage("ElectronsPerADU", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ElectronsPerADU Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ElectronsPerADU", false);
            }
        }

        public double ExposureMax
        {
            get
            {
                debug.LogMessage("ExposureMax", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureMax Get", "Exposure max is " + MAX_EXPOSURE_TIME.ToString());
                return MAX_EXPOSURE_TIME;
            }
        }

        public double ExposureMin
        {
            get
            {
                debug.LogMessage("ExposureMin", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureMin Get", "Exposure min is " + MIN_EXPOSURE_TIME.ToString());
                return MIN_EXPOSURE_TIME;
            }
        }

        public double ExposureResolution
        {
            get
            {
                debug.LogMessage("ExposureResolution", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ExposureResolution Get", "Exposure resolution min is " + EXPOSURE_RESOLTION.ToString());
                return EXPOSURE_RESOLTION;
            }
        }

        public bool FastReadout
        {
            get
            {
                debug.LogMessage("FastReadout", "Get");
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
                debug.LogMessage("FastReadout", "Set");
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
                debug.LogMessage("HasShutter", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("HasShutter Get", cameraInfo.mechanicalShutter.ToString());
                return cameraInfo.mechanicalShutter;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                debug.LogMessage("HeatSinkTemperature", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("HeatSinkTemperature Get", "Heatsink temperature is " + Cooling.heatsinkTemperature.ToString());
                return Cooling.heatsinkTemperature;
            }
        }

        public object ImageArray
        {
            get
            {
                debug.LogMessage("ImageArray", "Get");
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
                debug.LogMessage("ImageArrayVariant", "Get");
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
                debug.LogMessage("ImageReady", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ImageReady Get", cameraImageReady.ToString());
                return cameraImageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                debug.LogMessage("IsPulseGuiding", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("IsPulseGuiding Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("IsPulseGuiding", false);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                debug.LogMessage("LastExposureDuration", "Get");
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
                debug.LogMessage("LastExposureStartTime", "Get");
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
                debug.LogMessage("MaxADU", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("MaxADU Get", "60000");
                return 50000;
            }
        }

        public short MaxBinX
        {
            get
            {
                debug.LogMessage("MaxBinX", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("MaxBinX Get", "3");
                return 3;
            }
        }

        public short MaxBinY
        {
            get
            {
                debug.LogMessage("MaxBinY", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
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
                debug.LogMessage("PixelSizeX", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("PixelSizeX Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public double PixelSizeY
        {
            get
            {
                debug.LogMessage("PixelSizeY", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
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
                debug.LogMessage("ReadoutMode", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ReadoutMode Get", "Binning is " + Binning);
                return (short)Binning;
            }
            set
            {
                debug.LogMessage("ReadoutMode", "Set");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("ReadoutMode Set", "Setting binning to " + value);
                Binning = (SBIG.READOUT_BINNING_MODE)value;
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                debug.LogMessage("ReadoutModes", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                ArrayList list = new ArrayList();
                foreach (var readoutmode in cameraInfo.cameraReadoutModes)
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
                debug.LogMessage("SensorName", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("SensorName Get", "Camera name is " + cameraInfo.name);
                return cameraInfo.name;
            }
        }

        public SensorType SensorType
        {
            get
            {
                debug.LogMessage("SensorType", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                SensorType type;
                if (cameraInfo.colourCamera)
                {
                    if (cameraInfo.bayer == CameraInfo.TRUESENSE) type = SensorType.RGGB; // Guessing here
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
                debug.LogMessage("SetCCDTemperature", "Get");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
                debug.LogMessage("SetCCDTemperature Get", "CCD temperature target is " + CCDTempTarget);
                return CCDTempTarget;
            }
            set
            {
                debug.LogMessage("SetCCDTemperature", "Set");
                if (!IsConnected) throw new NotConnectedException("Camera is not connected");
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

            debug.LogMessage("StartExposure", "Get");
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
            catch (Exception ex)
            {
                throw new ASCOM.DriverException(Utils.DisplayException(ex));
            }
        }

        protected void bw_TakeImage(object sender, DoWorkEventArgs e)
        {
            try
            {
                imagetaker = new ImageTakerThread(this, IPAddress);
                imagetaker.TakeImage();
                imagetaker = null;
            }
            catch (Exception ex)
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

        protected short ConvertReadoutModeToXBinning(SBIG.READOUT_BINNING_MODE binning)
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

        protected short ConvertReadoutModeToYBinning(SBIG.READOUT_BINNING_MODE binning)
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
                    bin = 1;
                    break;
                case SBIG.READOUT_BINNING_MODE.RM_NX3:
                    bin = 1;
                    break;
                default:
                    bin = 0;
                    break;
            }
            return bin;
        }

        protected CameraReadoutMode GetCurrentReadoutMode(SBIG.READOUT_BINNING_MODE binning)
        {
            return cameraInfo.cameraReadoutModes.Find(r => r.mode == Binning);
        }

        protected SBIG.READOUT_BINNING_MODE ConvertBinningToReadout(short binning)
        {
            double nominalPixelWidth = cameraInfo.cameraReadoutModes.Find(r1 => r1.mode == 0).pixel_width;
            CameraReadoutMode ri = cameraInfo.cameraReadoutModes.FindAll(r => (r.pixel_width / nominalPixelWidth) == binning).OrderBy(rm => rm.mode).First();
            SBIG.READOUT_BINNING_MODE readout = ri.mode;
            return readout;
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        protected void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
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
        #endregion
    }
}
