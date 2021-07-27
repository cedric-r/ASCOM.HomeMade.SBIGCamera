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
using ASCOM.Utilities;
using SbigSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using ASCOM.HomeMade.SBIGCommon;

namespace ASCOM.HomeMade.SBIGFW
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63ad-c912-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(FilterWheel.driverID)]

    public class FilterWheel : IFilterWheelV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal const string driverID = "ASCOM.HomeMade.SBIGFW";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        public static string driverDescription = "ASCOM SBIG FilterWheel Driver.";

        private SBIGCommon.Debug debug = null;

        private SBIGServer server = new SBIGServer(driverID);

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeMade"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public FilterWheel()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, driverID);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }
            debug = new SBIGCommon.Debug(driverID, Path.Combine(strPath, "SBIGFW_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));

            if (!SBIGCommon.Debug.Testing) ReadProfile(); // Read device configuration from the ASCOM Profile store

            debug.LogMessage("FW", "Starting initialisation");

            debug.LogMessage("FW", "Completed initialisation");
        }

        //
        // PUBLIC COM INTERFACE IFilterWheelV2 IMPLEMENTATION
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
                    GetFWSpecs();

                    return;
                }
                if (!value && !IsConnected)
                {
                    debug.LogMessage("Connected", "Not connected connected yet");

                    return;
                }

                try
                {

                    if (value)
                    {
                        bool connected = server.Connect();

                        if (!connected)
                        {
                            debug.LogMessage("Connected Set", $"No USB FW found");
                            throw new DriverException("No suitable camera found");
                        }
                        else
                        {
                            debug.LogMessage("Connected Set", $"Connected to USB FW");

                            GetFWSpecs();
                        }
                    }
                    else
                    {
                        debug.LogMessage("Connected Set", "Disconnection requested");
                        if (IsConnected)
                        {
                            server.Disconnect();
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
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
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
                debug.LogMessage("InterfaceVersion Get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "ASCOM driver for SBIG FW";
                debug.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IFilterWheel Implementation
        private bool FilterWheelPresent = false;
        private short CurrentFilter = -1;
        private static int FWPositions = 0;

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
                debug.LogMessage("FWCommand", "Error: " + Utils.DisplayException(e));
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
                debug.LogMessage("GetFWData", "Error: " + Utils.DisplayException(e));
                throw new ASCOM.DriverException("Error getting DW data");
            }
        }

        private void WaitFWIdle()
        {
            SBIG.CFWResults results = new SBIG.CFWResults();
            while (results.cfwStatus != SBIG.CFW_STATUS.CFWS_IDLE)
            {
                CurrentFilter = -1;
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
                CurrentFilter = (short)(results.cfwPosition - 1);
                return results;
            }
            catch (Exception e)
            {
                debug.LogMessage("GetFWData", "Error: " + Utils.DisplayException(e));
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
                debug.LogMessage("GetFWData", "Error: " + Utils.DisplayException(e));
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

        public int[] FocusOffsets // Fake implementation of offsets. Who uses FW-provided offsets?
        {
            get
            {
                debug.LogMessage("FocusOffsets Get", "Getting FW filter FocusOffsets");
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
                debug.LogMessage("Names Get", "Getting FW filter names");
                List<string> positions = new List<string>();
                for (int i = 1; i <= FWPositions; i++)
                {
                    positions.Add("Filter " + i);
                }
                return positions.ToArray();
            }
        }

        public short Position
        {
            get
            {
                SBIG.CFWResults fwresults = GetFWPosition();
                debug.LogMessage("Position Get", "FW position is " + fwresults.cfwPosition);
                return CurrentFilter;
            }
            set
            {
                debug.LogMessage("Position Set", "Set FW position to " + value + 1);
                SetFWPosition((short)(value + 1));
            }
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

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                debug.LogMessage("IsConnected", "connectionState=" + server.IsConnected.ToString());
                return server.IsConnected;
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

        private void GetFWSpecs()
        {
            debug.LogMessage("Connected Set", $"Getting FW info");

            var fwresults = GetFWData();
            if (fwresults.cfwError == SBIG.CFW_ERROR.CFWE_NONE)
            {
                string model = GetFWTypeName(fwresults);
                debug.LogMessage("Connected", "FW is " + model);
                if (model != "Unknown") FilterWheelPresent = true;

                SBIG.CFWResults fwstatus = GetFWStatus();
                string status = GetFWStatusName(fwstatus);
                debug.LogMessage("Connected", "FW status is " + status);

                debug.LogMessage("Connected", "FW has " + fwresults.cfwResult2 + " positions");
                FWPositions = (int)fwresults.cfwResult2;

                SBIG.CFWResults fwfilter = GetFWPosition();
                debug.LogMessage("Connected", "FW position is " + GetFWFilterName(fwfilter));
            }
            else
            {
                debug.LogMessage("Connected", "Error querying FW: " + fwresults.cfwError);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "FilterWheel";
                try
                {
                    ASCOM.HomeMade.SBIGCommon.Debug.TraceEnabled = Convert.ToBoolean(driverProfile.GetValue(driverID, "TraceDebug", "", "false"));
                }
                catch(Exception) { }
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "FilterWheel";
                driverProfile.WriteValue(driverID, "TraceDebug", ASCOM.HomeMade.SBIGCommon.Debug.TraceEnabled.ToString());
            }
        }
        #endregion
    }
}