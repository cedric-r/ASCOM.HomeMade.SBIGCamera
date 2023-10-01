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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.IO;

namespace SbigSharp
{
    public static class SBIGDLL
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        static SBIGDLL()
        {
            if (Environment.Is64BitProcess)
            {
                //File.AppendAllText(@"c:\temp\SBIGDLL.txt", "Loading DLL from " + Path.Combine(AssemblyDirectory, @"x64\HomeMade.SBIGUDrv.dll") + "\n");
                LoadLibrary(Path.Combine(Path.Combine(AssemblyDirectory, @"x64"), "HomeMade.SBIGUDrv.dll"));
            }
            else
            {
                //File.AppendAllText(@"c:\temp\SBIGDLL.txt", "Loading DLL from " + Path.Combine(AssemblyDirectory, @"x86\HomeMade.SBIGUDrv.dll") + "\n");
                LoadLibrary(Path.Combine(Path.Combine(AssemblyDirectory, @"x86"), "HomeMade.SBIGUDrv.dll"));
            }
        }

        [DllImport("HomeMade.SBIGUDrv.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 SBIGLogDebugMsg(UIntPtr pStr, UInt16 length);

        [DllImport("HomeMade.SBIGUDrv.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern SBIG.PAR_ERROR SBIGUnivDrvCommand(SBIG.PAR_COMMAND command, IntPtr Params, IntPtr pResults);

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
