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

using System.Threading;
using ASCOM.HomeMade.SBIGFW;
using ASCOM.HomeMade.SBIGImagingCamera;

namespace ASCOM.HomeMade.SBIGCameraTests
{
    class Program
    {
        static void Main(string[] args)
        {
            SBIGCommon.Debug.Testing = true;
            SBIGCommon.Debug.TraceEnabled = true;

            Camera camera = new Camera();
            var binx = camera.BinX;
            System.Console.WriteLine("Connecting camera");
            camera.Connected = true;

            FilterWheel fw = new FilterWheel();
            fw.Connected = true;

            fw.Position = 2;

            System.Console.WriteLine("Setting CCD temperature to 20");
            camera.SetCCDTemperature= 20.0;
            System.Console.WriteLine("Setting cooler on");
            camera.CoolerOn = true;
            camera.NumX = camera.CameraXSize;
            camera.NumY = camera.CameraYSize;
            camera.StartX = 0;
            camera.StartY = 0;
            System.Console.WriteLine("Taking exposure");
            camera.StartExposure(1.0, true);
            System.Console.WriteLine("Setting cooler off");
            camera.CoolerOn = false;
            System.Console.WriteLine("Waiting for cooler off");
            while (camera.CoolerOn)
            {
                Thread.Sleep(500);
            }
            camera.Connected = false;
        }
    }
}
