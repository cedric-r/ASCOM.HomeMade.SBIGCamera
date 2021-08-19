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
        private static bool lockAccess = false;
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
            return HandleMessage(request);
        }

        private SBIGResponse HandleMessage(SBIGRequest request)
        {
            SBIGResponse response = new SBIGResponse();
            try
            {
                debug.LogMessage("SBIGService HandleMessage", "Message is " + request.type);

                switch (request.type)
                {
                    case "PING":
                        response.payload = true;
                        break;
                    case "Connect":
                        string ip = "";
                        ip = (string)request.parameters;
                        response.payload = Connect(ip);
                        break;
                    case "Disconnect":
                        Disconnect();
                        response.payload = null;
                        break;
                    case "AbortExposure":
                        SBIG.StartExposureParams2 p0 = (SBIG.StartExposureParams2)request.parameters;
                        if (!exposures.Keys.Contains(p0.ccd)) exposures.Remove(p0.ccd);
                        try
                        {
                            Utils.AcquireLock(ref lockAccess);
                            AbortExposure(p0);
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        finally
                        {
                            Utils.ReleaseLock(ref lockAccess);
                        }
                        response.payload = null;
                        break;
                    case "CC_SET_TEMPERATURE_REGULATION2":
                        SBIG.SetTemperatureRegulationParams2 p1 = (SBIG.SetTemperatureRegulationParams2)request.parameters;
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p1);
                        response.payload = null;
                        break;
                    case "CC_START_EXPOSURE2":
                        SBIG.StartExposureParams2 p2 = (SBIG.StartExposureParams2)request.parameters;
                        Exposure exposure = new Exposure() { ccd = p2.ccd, start = DateTime.Now, duration = p2.exposureTime };
                        if (!exposures.Keys.Contains(p2.ccd)) exposures.Remove(p2.ccd);
                        exposures[p2.ccd] = exposure;
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p2);
                        response.payload = null;
                        break;
                    case "CC_QUERY_TEMPERATURE_STATUS":
                        SBIG.QueryTemperatureStatusParams p3 = (SBIG.QueryTemperatureStatusParams)request.parameters;
                        SBIG.QueryTemperatureStatusResults2 result3 = new SBIG.QueryTemperatureStatusResults2();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p3, out result3);
                        response.payload = result3;
                        break;
                    case "CCD_INFO":
                        SBIG.GetCCDInfoParams p4 = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults0 result4 = new SBIG.GetCCDInfoResults0();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p4, out result4);
                        response.payload = result4;
                        break;
                    case "CCD_INFO_EXTENDED":
                        SBIG.GetCCDInfoParams p5 = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults2 result5 = new SBIG.GetCCDInfoResults2();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p5, out result5);
                        response.payload = result5;
                        break;
                    case "CCD_INFO_EXTENDED2":
                        SBIG.GetCCDInfoParams p6 = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults4 result6 = new SBIG.GetCCDInfoResults4();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p6, out result6);
                        response.payload = result6;
                        break;
                    case "CCD_INFO_EXTENDED3":
                        SBIG.GetCCDInfoParams p7 = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults6 result7 = new SBIG.GetCCDInfoResults6();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p7, out result7);
                        response.payload = result7;
                        break;
                    case "CCD_INFO_EXTENDED_PIXCEL":
                        SBIG.GetCCDInfoParams p10 = (SBIG.GetCCDInfoParams)request.parameters;
                        SBIG.GetCCDInfoResults3 result10 = new SBIG.GetCCDInfoResults3();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p10, out result10);
                        response.payload = result10;
                        break;
                    case "EndReadout":
                        SBIG.CCD_REQUEST ccd = (SBIG.CCD_REQUEST)request.parameters;
                        if (!exposures.Keys.Contains(ccd)) exposures.Remove(ccd);
                        EndReadout(ccd);
                        response.payload = null;
                        break;
                    case "ExposureInProgress":
                        bool temp = false;
                        SBIG.StartExposureParams2 p11 = (SBIG.StartExposureParams2)request.parameters;
                        if (exposures.Keys.Contains(p11.ccd))
                        {
                            long duration = (long)exposures[p11.ccd].duration;
                            if (duration > SBIG.EXP_FAST_READOUT) duration -= SBIG.EXP_FAST_READOUT;
                            if (DateTime.Now < (exposures[p11.ccd].start + new TimeSpan(duration * TimeSpan.TicksPerSecond / 100)))
                                temp = true;
                        }
                        //bool inProgress = server.ExposureInProgress();
                        response.payload = temp;
                        break;
                    case "ReadoutData":
                        SBIG.StartExposureParams2 p8 = (SBIG.StartExposureParams2)request.parameters;
                        if (!exposures.Keys.Contains(p8.ccd)) exposures.Remove(p8.ccd);
                        var data = new UInt16[p8.height, p8.width];
                        try
                        {
                            Utils.AcquireLock(ref lockAccess);
                            ReadoutDataAndEnd(p8, ref data);
                        }
                        catch (Exception ex)
                        {
                            debug.LogMessage("SBIGService HandleMessage", "Error: " + Utils.DisplayException(ex));
                        }
                        finally
                        {
                            Utils.ReleaseLock(ref lockAccess);
                        }
                        response.payload = data;
                        break;
                    case "CC_CFW":
                        SBIG.CFWParams p9 = (SBIG.CFWParams)request.parameters;
                        SBIG.CFWResults result9 = new SBIG.CFWResults();
                        UnivDrvCommand((SBIG.PAR_COMMAND)request.command, p9, out result9);
                        response.payload = result9;
                        break;
                    default:
                        throw new NotImplementedException("Message type " + request.type + " not implemented");
                        break;
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
                if (String.IsNullOrEmpty(ipAddress))
                {
                    try
                    {
                        debug.LogMessage("Disconnect", "Disconnecting device");
                        // clean up
                        SBIG.UnivDrvCommand(SBIG.PAR_COMMAND.CC_CLOSE_DRIVER);
                    }
                    catch (Exception e)
                    {
                        //debug.LogMessage("Connected", "Error: " + DisplayException(e));
                    }

                }
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
                var cameraType = SBIG.EstablishLink();

                CameraType = SBIG.EstablishLink();
                debug.LogMessage("Connected Set", $"Connected to USB camera");

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
            try
            {
                Utils.AcquireLock(ref lockReadout);
                UnivDrvCommand(
                SBIG.PAR_COMMAND.CC_END_EXPOSURE,
                new SBIG.EndExposureParams()
                {
                    ccd = sep2.ccd
                });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Utils.ReleaseLock(ref lockReadout);
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

        public bool ExposureInProgress()
        {
            return SBIG.ExposureInProgress();
        }

        public void ReadoutData(SBIG.StartExposureParams2 sep2, ref UInt16[,] data)
        {
            try
            {
                Utils.AcquireLock(ref lockReadout);
                SBIG.ReadoutData(sep2, ref data);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Utils.ReleaseLock(ref lockReadout);
            }
        }

        private static bool lockReadout = false;
        public void ReadoutDataAndEnd(SBIG.StartExposureParams2 sep2, ref UInt16[,] data)
        {
            try
            {
                Utils.AcquireLock(ref lockReadout);
                SBIG.ReadoutData(sep2, ref data);
                UnivDrvCommand(SBIG.PAR_COMMAND.CC_END_READOUT,
                new SBIG.EndReadoutParams()
                    {
                        ccd = sep2.ccd
                    });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Utils.ReleaseLock(ref lockReadout);
            }
        }
    }
}

