using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade
{
    public static class Debug
    {
        public static bool TraceEnabled = false;
        public static string FileName = "";
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            LogMessage(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + identifier + ": " + msg);
        }

        static readonly object fileLockObject = new object();
        internal static void LogMessage(string message)
        {
            lock (fileLockObject)
            {
                if (FileName == "") FileName = @"c:\temp\Focuser.log";
                if (TraceEnabled) File.AppendAllText(FileName, message + "\n");
            }
        }
    }
}
