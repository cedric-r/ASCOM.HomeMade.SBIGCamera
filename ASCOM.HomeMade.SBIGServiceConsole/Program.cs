using ASCOM.HomeMade.SBIGWindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SBIGWindowsProcessService service = new SBIGWindowsProcessService();
            if (Environment.UserInteractive)
            {
                service.RunAsConsole(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { service };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
