using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCameraTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Camera camera = new Camera(true);
            System.Console.WriteLine("Connecting camera");
            camera.Connected = true;
            System.Console.WriteLine("Setting CCD temperature to 20");
            camera.SetCCDTemperature= 20.0;
            System.Console.WriteLine("Setting cooler on");
            camera.CoolerOn = true;
            System.Console.WriteLine("Setting filter 1");
            camera.Position = 1;
            System.Console.WriteLine("Current FW position is "+camera.Position);
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
