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
using ASCOM.HomeMade.SBIGCamera;
using ASCOM.Utilities;

namespace ASCOM.HomeMade.SBIGImagingCamera
{
    /// <summary>
    /// ASCOM Camera Driver for HomeMade.
    /// </summary>
    [Guid("3a7e63ad-c913-44f0-9489-e1744c9c2991")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(Camera.driverID)]
    [ComVisible(true)]
    public class Camera : SBIGCamera.SBIGCamera
    {
        public const string driverID = "ASCOM.HomeMade.SBIGImagingCamera";

        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        public const string driverDescription = "ASCOM SBIG Imaging Camera Driver";

        internal string DriverID { get { return driverID; } }

        public static string IPAddress = "";
        public static bool HideReadout = true;
        public static SBIGBayerPattern BayerPattern = SBIGBayerPattern.BGGR;

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

        public new string Description
        {
            // TODO customise this device description
            get
            {
                debug.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }
        protected override string GetIPAddress()
        {
            return IPAddress;
        }

        protected override bool GetHideReadout()
        {
            return HideReadout;
        }

        public override SBIGBayerPattern GetBayerPattern()
        {
            return BayerPattern;
        }

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public override void SetupDialog()
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
                try
                {
                    HideReadout = Convert.ToBoolean(driverProfile.GetValue(driverID, "HideReadout", "", "True"));
                }
                catch (Exception) { }
                try
                {
                    BayerPattern = (SBIGBayerPattern)Convert.ToInt32(driverProfile.GetValue(driverID, "BayerPattern", "", ((int)SBIGBayerPattern.BGGR).ToString()));
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
                driverProfile.WriteValue(driverID, "HideReadout", HideReadout.ToString());
                driverProfile.WriteValue(driverID, "BayerPattern", ((int)BayerPattern).ToString());
            }
        }
    }
}
