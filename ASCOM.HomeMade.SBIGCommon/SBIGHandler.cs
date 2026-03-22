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

using SbigSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGHandler : ISBIGHandler
    {
        private class Exposure
        {
            public SBIG.CCD_REQUEST ccd;
            public DateTime start;
            public uint duration;
        }

        public const string deviceId = "ASCOM.HomeMade.SBIGCamera";
        private static Dictionary<SBIG.CCD_REQUEST, Exposure> exposures = new Dictionary<SBIG.CCD_REQUEST, Exposure>();
        private static readonly object _cameraLockObject = new object();
        private static readonly object _accessLockObject = new object();
        private static readonly object _readoutLockObject = new object();
        private Debug debug = null;
        private bool _Connected = false;

        public bool IsConnected { get { return _Connected; } }

        public SBIGHandler()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = Path.Combine(strPath, deviceId);
            try
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            catch (Exception) { }

            debug = new Debug(deviceId, Path.Combine(strPath, "SBIGCommon_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));
        }

        public SBIGResponse Transmit(SBIGRequest request)
        {
            lock (_cameraLockObject)
            {
                return HandleMessage(request);
            }
        }

        private SBIGResponse HandleMessage(SBIGRequest request)
        {
            SBIGResponse response = new SBIGResponse();
            try
            {
                debug.LogMessage("SBIGService HandleMessage", "Message is " + request.type);

                switch (request.type)
                {
                    case MessageType.Ping:
                        response.payload = true;
                        break;
                    case MessageType.Connect:
                        response.payload = Connect((string)request.parameters);
                        break;
                    case MessageType.Disconnect:
                        Disconnect();
                        response.payload = null;
                        break;
                    case MessageType.AbortExposure:
                        SBIG.StartExposureParams2 abortParams = (SBIG.StartExposureParams2)request.parameters;
                        if (exposures.Keys.Contains(abortParams.ccd)) exposures.Remove(abortParams.ccd);
                        try
                        {
                            lock (_accessLockObject)
                            {
                                AbortExposure(abortParams);
                            }
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        response.payload = null;
                        break;
                    case MessageType.SetTemperatureRegulation:
                        SBIG.SetTemperatureRegulationParams2 tempRegParams = (SBIG.SetTemperatureRegulationParams2)request.parameters;
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, tempRegParams);
                        response.payload = null;
                        break;
                    case MessageType.StartExposure:
                        SBIG.StartExposureParams2 startExpParams = (SBIG.StartExposureParams2)request.parameters;
                        Exposure exposure = new Exposure() { ccd = startExpParams.ccd, start = DateTime.Now, duration = startExpParams.exposureTime };
                        if (exposures.Keys.Contains(startExpParams.ccd)) exposures.Remove(startExpParams.ccd);
                        exposures.Add(startExpParams.ccd, exposure);
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, startExpParams);
                        response.payload = null;
                        break;
                    case MessageType.QueryTemperatureStatus:
                        SBIG.QueryTemperatureStatusParams tempStatusParams = (SBIG.QueryTemperatureStatusParams)request.parameters;
                        SBIG.QueryTemperatureStatusResults2 tempStatusResult = new SBIG.QueryTemperatureStatusResults2();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, tempStatusParams, out tempStatusResult);
                        response.payload = tempStatusResult;
                        break;
                    case MessageType.CcdInfo:
                        SBIG.GetCCDInfoParams ccdInfoParams = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults0 ccdInfoResult = new SBIG.GetCCDInfoResults0();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, ccdInfoParams, out ccdInfoResult);
                        response.payload = ccdInfoResult;
                        break;
                    case MessageType.CcdInfoExtended:
                        SBIG.GetCCDInfoParams ccdExtParams = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults2 ccdExtResult = new SBIG.GetCCDInfoResults2();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, ccdExtParams, out ccdExtResult);
                        response.payload = ccdExtResult;
                        break;
                    case MessageType.CcdInfoExtended2:
                        SBIG.GetCCDInfoParams ccdExt2Params = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults4 ccdExt2Result = new SBIG.GetCCDInfoResults4();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, ccdExt2Params, out ccdExt2Result);
                        response.payload = ccdExt2Result;
                        break;
                    case MessageType.CcdInfoExtended3:
                        SBIG.GetCCDInfoParams ccdExt3Params = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults6 ccdExt3Result = new SBIG.GetCCDInfoResults6();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, ccdExt3Params, out ccdExt3Result);
                        response.payload = ccdExt3Result;
                        break;
                    case MessageType.CcdInfoExtendedPixcel:
                        SBIG.GetCCDInfoParams ccdPixcelParams = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults3 ccdPixcelResult = new SBIG.GetCCDInfoResults3();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, ccdPixcelParams, out ccdPixcelResult);
                        response.payload = ccdPixcelResult;
                        break;
                    case MessageType.EndReadout:
                        SBIG.CCD_REQUEST endReadoutCcd = (SBIG.CCD_REQUEST)request.parameters;
                        if (exposures.Keys.Contains(endReadoutCcd)) exposures.Remove(endReadoutCcd);
                        EndReadout(endReadoutCcd);
                        response.payload = null;
                        break;
                    case MessageType.ExposureInProgress:
                        bool inProgress = false;
                        SBIG.StartExposureParams2 inProgressParams = (SBIG.StartExposureParams2)request.parameters;
                        if (exposures.Keys.Contains(inProgressParams.ccd))
                        {
                            long duration = (long)exposures[inProgressParams.ccd].duration;
                            if (duration >= SBIG.EXP_FAST_READOUT) duration -= SBIG.EXP_FAST_READOUT;
                            if (DateTime.Now < (exposures[inProgressParams.ccd].start + new TimeSpan(duration * TimeSpan.TicksPerSecond / 100)))
                                inProgress = true;
                        }
                        if (!inProgress)
                            inProgress = ExposureInProgress(inProgressParams.ccd); // Double check with the camera
                        response.payload = inProgress;
                        break;
                    case MessageType.ReadoutData:
                        SBIG.StartExposureParams2 readoutParams = (SBIG.StartExposureParams2)request.parameters;
                        if (exposures.Keys.Contains(readoutParams.ccd)) exposures.Remove(readoutParams.ccd);
                        var data = new UInt16[readoutParams.width * readoutParams.height];
                        try
                        {
                            lock (_accessLockObject)
                            {
                                ReadoutDataAndEnd(readoutParams, ref data);
                            }
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        response.payload = data;
                        break;
                    case MessageType.CcfCfw:
                        SBIG.CFWParams cfwParams = (SBIG.CFWParams)request.parameters;
                        SBIG.CFWResults cfwResult = new SBIG.CFWResults();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, cfwParams, out cfwResult);
                        response.payload = cfwResult;
                        break;
                    default:
                        throw new NotImplementedException("Message type " + request.type + " not implemented");
                }
                return response;
            }
            catch (Exception e)
            {
                debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(e));
                response.error = e;
                return response;
            }
        }

        public bool Connect(string ipAddress)
        {
            debug.LogMessage("Connect", "Connection request");

            SBIG.CAMERA_TYPE CameraType;

            SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_OPEN_DRIVER);

            bool cameraFound = false;
            if (String.IsNullOrEmpty(ipAddress))
            {
                debug.LogMessage("Connected Set", $"Enumerating USB cameras");
                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_QUERY_USB, out SBIG.QueryUSBResults qur);
                for (int i = 0; i < qur.camerasFound; i++)
                {
                    if (!qur.usbInfo[i].cameraFound)
                        debug.LogMessage("Connected Set", $"Cam {i}: not found");
                    else
                    {
                        debug.LogMessage("Connected Set",
                            $"Cam {i}: type={qur.usbInfo[i].cameraType} " +
                            $"name={ qur.usbInfo[i].name} " +
                            $"ser={qur.usbInfo[i].serialNumber}");
                        cameraFound = true;
                    }
                }
            }

            if (!cameraFound && String.IsNullOrEmpty(ipAddress))
            {
                debug.LogMessage("Connected Set", $"No camera found");
                try
                {
                    SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DRIVER);
                }
                catch (Exception) { }
                return false;
            }
            else
            {
                if (String.IsNullOrEmpty(ipAddress))
                {
                    SBIG.UnivDrvCommand(
                        SBIG.PAR_COMMAND.CC_OPEN_DEVICE,
                        new SBIG.OpenDeviceParams
                        {
                            deviceType = SBIG.SBIG_DEVICE_TYPE.DEV_USB
                        });
                }
                else
                {
                    SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_OPEN_DEVICE, 
                        new SBIG.OpenDeviceParams(ipAddress));
                }
                CameraType = SBIG.EstablishLink();
                debug.LogMessage("Connected Set", $"Connected to camera");

                return true;
            }
        }

        public void Disconnect()
        {
            debug.LogMessage("Disconnect", "Disconnection requested");
            try
            {
                debug.LogMessage("Disconnect", "Disconnecting device");
                // clean up
                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DEVICE);
                SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DRIVER);
            }
            catch (Exception e)
            {
                //debug.LogMessage("Connected", "Error: " + DisplayException(e));
            }
        }

        public void UnivDrvCommand(SBIG.PAR_COMMAND command)
        {
            // make the call
            SBIG.UnivDrvCommand(command);
        }

        public void UnivDrvCommand<TParams>(SBIG.PAR_COMMAND command, TParams Params)
            where TParams : SBIG.IParams
        {
            SBIG.UnivDrvCommand<TParams>(command, Params);
        }

        public void UnivDrvCommand<TResults>(SBIG.PAR_COMMAND command, out TResults pResults)
            where TResults : SBIG.IResults
        {
            SBIG.UnivDrvCommand<TResults>(command, out pResults);
        }

        public void UnivDrvCommand(SBIG.PAR_COMMAND command, SBIG.ReadoutLineParams Params, out UInt16[] data)
        {
            SBIG.UnivDrvCommand(command, Params, out data);
        }

        public void UnivDrvCommand<TParams, TResults>(SBIG.PAR_COMMAND command, TParams Params, out TResults pResults)
            where TParams : SBIG.IParams
            where TResults : SBIG.IResults
        {
            SBIG.UnivDrvCommand<TParams, TResults>(command, Params, out pResults);
        }

        public void AbortExposure(SBIG.StartExposureParams2 sep2)
        {
            lock (_readoutLockObject)
            {
                UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_END_EXPOSURE,
                new SBIG.EndExposureParams()
                {
                    ccd = sep2.ccd
                });
            }
        }

        public void EndReadout(SBIG.CCD_REQUEST ccd)
        {
            UnivDrvCommand(SBIG.PAR_COMMAND.CC_END_READOUT,
            new SBIG.EndReadoutParams()
            {
                ccd = ccd
            });
        }

        public bool ExposureInProgress(SBIG.CCD_REQUEST ccd = SBIG.CCD_REQUEST.CCD_IMAGING)
        {
            return SBIG.ExposureInProgress(ccd);
        }

        public void ReadoutData(SBIG.StartExposureParams2 sep2, ref UInt16[,] data)
        {
            lock (_readoutLockObject)
            {
                SBIG.ReadoutData(sep2, ref data);
            }
        }

        public void ReadoutDataAndEnd(SBIG.StartExposureParams2 sep2, ref UInt16[] data)
        {
            lock (_readoutLockObject)
            {
                SBIG.ReadoutData(sep2, ref data);
                EndReadout(sep2.ccd);
            }
        }
    }
}

