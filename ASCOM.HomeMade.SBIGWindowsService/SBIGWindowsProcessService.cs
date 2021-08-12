using ASCOM.HomeMade.SBIGCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Debug = ASCOM.HomeMade.SBIGCommon.Debug;

namespace ASCOM.HomeMade.SBIGWindowsService
{
    public partial class SBIGWindowsProcessService : ServiceBase
    {
        SBIGService service = null;

        public SBIGWindowsProcessService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Debug.TraceEnabled = true;
                service = SBIGService.CreateService();
            }
            catch (Exception) { }
        }

        protected override void OnStop()
        {
            try
            {
                if (service != null)
                {
                    service.Stop();
                }
            }
            catch (Exception) { }
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            OnStop();
        }
    }
}
