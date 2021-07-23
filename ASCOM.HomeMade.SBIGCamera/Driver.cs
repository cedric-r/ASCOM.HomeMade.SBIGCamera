//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for HomeMade
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Camera interface version: <To be completed by driver developer>
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define Camera

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.HomeMade
{
    //
    // Your driver's DeviceID is ASCOM.HomeMade.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.HomeMade.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _HomeMade from being created and used as the [default] interface
    //
    // TODO Replace the not implemented exceptions with code to implement the function or
    // throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63cd-c913-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal static string driverID = "ASCOM.HomeMade.Camera";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM Camera Driver for HomeMade.";

        internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
        internal static string comPortDefault = "COM1";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";

        internal static string comPort; // Variables to hold the currrent device configuration

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        internal static TraceLogger tl;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMade"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            tl = new TraceLogger("", "HomeMade");
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            tl.LogMessage("Camera", "Starting initialisation");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here

            tl.LogMessage("Camera", "Completed initialisation");
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
                tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
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
            // Clean up the tracelogger and util objects
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
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
                tl.LogMessage("Connected", "Set {0}", value);
                if (value == IsConnected)
                    return;

                if (value)
                {
                    connectedState = true;
                    LogMessage("Connected Set", "Connecting to port {0}", comPort);
                    // TODO connect to the device
                }
                else
                {
                    connectedState = false;
                    LogMessage("Connected Set", "Disconnecting from port {0}", comPort);
                    // TODO disconnect from the device
                }
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                tl.LogMessage("Description Get", driverDescription);
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
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
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
                string name = "Short driver name - please customise";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region ICamera Implementation

        private const int ccdWidth = 1394; // Constants to define the ccd pixel dimenstions
        private const int ccdHeight = 1040;
        private const double pixelSize = 6.45; // Constant for the pixel physical dimension

        private int cameraNumX = ccdWidth; // Initialise variables to hold values required for functionality tested by Conform
        private int cameraNumY = ccdHeight;
        private int cameraStartX = 0;
        private int cameraStartY = 0;
        private DateTime exposureStart = DateTime.MinValue;
        private double cameraLastExposureDuration = 0.0;
        private bool cameraImageReady = false;
        private int[,] cameraImageArray;
        private object[,] cameraImageArrayVariant;

        public void AbortExposure()
        {
            tl.LogMessage("AbortExposure", "Not implemented");
            throw new MethodNotImplementedException("AbortExposure");
        }

        public short BayerOffsetX
        {
            get
            {
                tl.LogMessage("BayerOffsetX Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", false);
            }
        }

        public short BayerOffsetY
        {
            get
            {
                tl.LogMessage("BayerOffsetY Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("BayerOffsetX", true);
            }
        }

        public short BinX
        {
            get
            {
                tl.LogMessage("BinX Get", "1");
                return 1;
            }
            set
            {
                tl.LogMessage("BinX Set", value.ToString());
                if (value != 1) throw new ASCOM.InvalidValueException("BinX", value.ToString(), "1"); // Only 1 is valid in this simple template
            }
        }

        public short BinY
        {
            get
            {
                tl.LogMessage("BinY Get", "1");
                return 1;
            }
            set
            {
                tl.LogMessage("BinY Set", value.ToString());
                if (value != 1) throw new ASCOM.InvalidValueException("BinY", value.ToString(), "1"); // Only 1 is valid in this simple template
            }
        }

        public double CCDTemperature
        {
            get
            {
                tl.LogMessage("CCDTemperature Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CCDTemperature", false);
            }
        }

        public CameraStates CameraState
        {
            get
            {
                tl.LogMessage("CameraState Get", CameraStates.cameraIdle.ToString());
                return CameraStates.cameraIdle;
            }
        }

        public int CameraXSize
        {
            get
            {
                tl.LogMessage("CameraXSize Get", ccdWidth.ToString());
                return ccdWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                tl.LogMessage("CameraYSize Get", ccdHeight.ToString());
                return ccdHeight;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                tl.LogMessage("CanAbortExposure Get", false.ToString());
                return false;
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                tl.LogMessage("CanAsymmetricBin Get", false.ToString());
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                tl.LogMessage("CanFastReadout Get", false.ToString());
                return false;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                tl.LogMessage("CanGetCoolerPower Get", false.ToString());
                return false;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                tl.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                tl.LogMessage("CanSetCCDTemperature Get", false.ToString());
                return false;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                tl.LogMessage("CanStopExposure Get", false.ToString());
                return false;
            }
        }

        public bool CoolerOn
        {
            get
            {
                tl.LogMessage("CoolerOn Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", false);
            }
            set
            {
                tl.LogMessage("CoolerOn Set Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", true);
            }
        }

        public double CoolerPower
        {
            get
            {
                tl.LogMessage("CoolerPower Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerPower", false);
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                tl.LogMessage("ElectronsPerADU Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ElectronsPerADU", false);
            }
        }

        public double ExposureMax
        {
            get
            {
                tl.LogMessage("ExposureMax Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ExposureMax", false);
            }
        }

        public double ExposureMin
        {
            get
            {
                tl.LogMessage("ExposureMin Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ExposureMin", false);
            }
        }

        public double ExposureResolution
        {
            get
            {
                tl.LogMessage("ExposureResolution Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ExposureResolution", false);
            }
        }

        public bool FastReadout
        {
            get
            {
                tl.LogMessage("FastReadout Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
            }
            set
            {
                tl.LogMessage("FastReadout Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", true);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                tl.LogMessage("FullWellCapacity Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("FullWellCapacity", false);
            }
        }

        public short Gain
        {
            get
            {
                tl.LogMessage("Gain Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("Gain", false);
            }
            set
            {
                tl.LogMessage("Gain Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("Gain", true);
            }
        }

        public short GainMax
        {
            get
            {
                tl.LogMessage("GainMax Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMax", false);
            }
        }

        public short GainMin
        {
            get
            {
                tl.LogMessage("GainMin Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMin", true);
            }
        }

        public ArrayList Gains
        {
            get
            {
                tl.LogMessage("Gains Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("Gains", true);
            }
        }

        public bool HasShutter
        {
            get
            {
                tl.LogMessage("HasShutter Get", false.ToString());
                return false;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                tl.LogMessage("HeatSinkTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("HeatSinkTemperature", false);
            }
        }

        public object ImageArray
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArray before the first image has been taken!");
                }

                cameraImageArray = new int[cameraNumX, cameraNumY];
                return cameraImageArray;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!");
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
                tl.LogMessage("ImageReady Get", cameraImageReady.ToString());
                return cameraImageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                tl.LogMessage("IsPulseGuiding Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("IsPulseGuiding", false);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureDuration before the first image has been taken!");
                }
                tl.LogMessage("LastExposureDuration Get", cameraLastExposureDuration.ToString());
                return cameraLastExposureDuration;
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!");
                }
                string exposureStartString = exposureStart.ToString("yyyy-MM-ddTHH:mm:ss");
                tl.LogMessage("LastExposureStartTime Get", exposureStartString.ToString());
                return exposureStartString;
            }
        }

        public int MaxADU
        {
            get
            {
                tl.LogMessage("MaxADU Get", "20000");
                return 20000;
            }
        }

        public short MaxBinX
        {
            get
            {
                tl.LogMessage("MaxBinX Get", "1");
                return 1;
            }
        }

        public short MaxBinY
        {
            get
            {
                tl.LogMessage("MaxBinY Get", "1");
                return 1;
            }
        }

        public int NumX
        {
            get
            {
                tl.LogMessage("NumX Get", cameraNumX.ToString());
                return cameraNumX;
            }
            set
            {
                cameraNumX = value;
                tl.LogMessage("NumX set", value.ToString());
            }
        }

        public int NumY
        {
            get
            {
                tl.LogMessage("NumY Get", cameraNumY.ToString());
                return cameraNumY;
            }
            set
            {
                cameraNumY = value;
                tl.LogMessage("NumY set", value.ToString());
            }
        }

        public short PercentCompleted
        {
            get
            {
                tl.LogMessage("PercentCompleted Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("PercentCompleted", false);
            }
        }

        public double PixelSizeX
        {
            get
            {
                tl.LogMessage("PixelSizeX Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public double PixelSizeY
        {
            get
            {
                tl.LogMessage("PixelSizeY Get", pixelSize.ToString());
                return pixelSize;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            tl.LogMessage("PulseGuide", "Not implemented");
            throw new ASCOM.MethodNotImplementedException("PulseGuide");
        }

        public short ReadoutMode
        {
            get
            {
                tl.LogMessage("ReadoutMode Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ReadoutMode", false);
            }
            set
            {
                tl.LogMessage("ReadoutMode Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ReadoutMode", true);
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                tl.LogMessage("ReadoutModes Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("ReadoutModes", false);
            }
        }

        public string SensorName
        {
            get
            {
                tl.LogMessage("SensorName Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SensorName", false);
            }
        }

        public SensorType SensorType
        {
            get
            {
                tl.LogMessage("SensorType Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SensorType", false);
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                tl.LogMessage("SetCCDTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", false);
            }
            set
            {
                tl.LogMessage("SetCCDTemperature Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", true);
            }
        }

        public void StartExposure(double Duration, bool Light)
        {
            if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");
            if (cameraNumX > ccdWidth) throw new InvalidValueException("StartExposure", cameraNumX.ToString(), ccdWidth.ToString());
            if (cameraNumY > ccdHeight) throw new InvalidValueException("StartExposure", cameraNumY.ToString(), ccdHeight.ToString());
            if (cameraStartX > ccdWidth) throw new InvalidValueException("StartExposure", cameraStartX.ToString(), ccdWidth.ToString());
            if (cameraStartY > ccdHeight) throw new InvalidValueException("StartExposure", cameraStartY.ToString(), ccdHeight.ToString());

            cameraLastExposureDuration = Duration;
            exposureStart = DateTime.Now;
            System.Threading.Thread.Sleep((int)Duration * 1000);  // Sleep for the duration to simulate exposure 
            tl.LogMessage("StartExposure", Duration.ToString() + " " + Light.ToString());
            cameraImageReady = true;
        }

        public int StartX
        {
            get
            {
                tl.LogMessage("StartX Get", cameraStartX.ToString());
                return cameraStartX;
            }
            set
            {
                cameraStartX = value;
                tl.LogMessage("StartX Set", value.ToString());
            }
        }

        public int StartY
        {
            get
            {
                tl.LogMessage("StartY Get", cameraStartY.ToString());
                return cameraStartY;
            }
            set
            {
                cameraStartY = value;
                tl.LogMessage("StartY set", value.ToString());
            }
        }

        public void StopExposure()
        {
            tl.LogMessage("StopExposure", "Not implemented");
            throw new MethodNotImplementedException("StopExposure");
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

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                // TODO check that the driver hardware connection exists and is connected to the hardware
                return connectedState;
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
                tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
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
                driverProfile.WriteValue(driverID, traceStateProfileName, tl.Enabled.ToString());
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
            tl.LogMessage(identifier, msg);
        }
        #endregion
    }
}
