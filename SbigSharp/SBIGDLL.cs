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
                LoadLibrary(Path.Combine(AssemblyDirectory, @"x64\HomeMade.SBIGUDrv.dll"));
            }
            else
            {
                //File.AppendAllText(@"c:\temp\SBIGDLL.txt", "Loading DLL from " + Path.Combine(AssemblyDirectory, @"x86\HomeMade.SBIGUDrv.dll") + "\n");
                LoadLibrary(Path.Combine(AssemblyDirectory, @"x86\HomeMade.SBIGUDrv.dll"));
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
