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
using System.IO;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class Debug
    {
        public static bool Testing = false;
        public static bool TraceEnabled = false;
        public string FileName = "";
        private string deviceId = "";

        public Debug(string device)
        {
            deviceId = device;
        }

        public Debug(string device, string filename)
        {
            deviceId = device;
            FileName = filename;
        }

        public void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            LogMessage(DateTime.Now.Day+"/"+ DateTime.Now.Month+"/"+ DateTime.Now.Year + " " + DateTime.Now.Hour+":"+ DateTime.Now.Minute+":"+ DateTime.Now.Second+"."+ DateTime.Now.Millisecond + ": " + identifier + ": " + msg);
        }

        readonly object fileLockObject = new object();
        public void LogMessage(string message)
        {
            lock (fileLockObject)
            {
                if (FileName != "")
                {
                    try
                    {
                        if (TraceEnabled) File.AppendAllText(FileName, message + "\n");
                    }
                    catch (Exception) { }
                }
            }
        }
    }
}
