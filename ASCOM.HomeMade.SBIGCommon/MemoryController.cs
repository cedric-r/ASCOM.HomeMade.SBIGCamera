using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class MemoryController
    {
        private const int MAXMEMORY = 500;

        public MemoryController()
        {

        }

        public void ControlMemory()
        {
            float memoryUsed = MemoryUsed();
            if (memoryUsed>MAXMEMORY)
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
        }

        private float MemoryUsed()
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            var counter = new PerformanceCounter("Process", "Working Set - Private", processName);
            return counter.NextValue() / 1024 / 1024; // in MB
        }
    }
}
