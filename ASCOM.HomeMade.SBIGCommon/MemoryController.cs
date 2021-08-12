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
