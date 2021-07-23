using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCameraTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Camera camera = new Camera(true);
            camera.Connected = true;
        }
    }
}
