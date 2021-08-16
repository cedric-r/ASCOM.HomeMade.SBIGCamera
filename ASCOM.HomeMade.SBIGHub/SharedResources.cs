//
// ================
// Shared Resources
// ================
//
// This class is a container for all shared resources that may be needed
// by the drivers served by the Local Server. 
//
// NOTES:
//
//	* ALL DECLARATIONS MUST BE STATIC HERE!! INSTANCES OF THIS CLASS MUST NEVER BE CREATED!
//
// Written by:	Bob Denny	29-May-2007
// Modified by Chris Rowland and Peter Simpson to hamdle multiple hardware devices March 2011
//
using ASCOM;
using ASCOM.HomeMade.SBIGCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.HomeMade.SBIGHub
{
    /// <summary>
    /// The resources shared by all drivers and devices, in this example it's a serial port with a shared SendMessage method
    /// an idea for locking the message and handling connecting is given.
    /// In reality extensive changes will probably be needed.
    /// Multiple drivers means that several applications connect to the same hardware device, aka a hub.
    /// Multiple devices means that there are more than one instance of the hardware, such as two focusers.
    /// In this case there needs to be multiple instances of the hardware connector, each with it's own connection count.
    /// </summary>
    public static class SharedResources
    {
        // object used for locking to prevent multiple drivers accessing common code at the same time
        private static readonly object lockObject = new object();

        // Shared serial port. This will allow multiple drivers to use one single serial port.
        private static SBIGHandler s_sharedSBIGHandler = new SBIGHandler();        // Shared serial port
        private static int s_z = 0;     // counter for the number of connections to the serial port

        //
        // Public access to shared resources
        //

        #region single serial port connector
        //
        // this region shows a way that a single serial port could be connected to by multiple 
        // drivers.
        //
        // Connected is used to handle the connections to the port.
        //
        // SendMessage is a way that messages could be sent to the hardware without
        // conflicts between different drivers.
        //
        // All this is for a single connection, multiple connections would need multiple ports
        // and a way to handle connecting and disconnection from them - see the
        // multi driver handling section for ideas.
        //

        /// <summary>
        /// Shared serial port
        /// </summary>
        public static SBIGHandler SBIGHandlerShared { get { return s_sharedSBIGHandler; } }

        /// <summary>
        /// number of connections to the shared serial port
        /// </summary>
        public static int connections { get { return s_z; } set { s_z = value; } }

        /// <summary>
        /// Example of a shared SendMessage method, the lock
        /// prevents different drivers tripping over one another.
        /// It needs error handling and assumes that the message will be sent unchanged
        /// and that the reply will always be terminated by a "#" character.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static SBIGResponse SendMessage(SBIGRequest message)
        {
            return SBIGHandlerShared.Transmit(message);
        }

        /// <summary>
        /// Example of handling connecting to and disconnection from the
        /// shared serial port.
        /// Needs error handling
        /// the port name etc. needs to be set up first, this could be done by the driver
        /// checking Connected and if it's false setting up the port before setting connected to true.
        /// It could also be put here.
        /// </summary>
        public static void Connect(string ipAddress)
        {
            lock (lockObject)
            {
                if (s_z == 0)
                    SBIGHandlerShared.Connect(ipAddress);
                s_z++;
            }
        }

        public static void DisConnect()
        {
            lock (lockObject)
            {
                s_z--;
                if (s_z <= 0)
                {
                    SBIGHandlerShared.Disconnect();
                }
                if (s_z < 0) s_z = 0;
            }
        }

        public static bool IsConnected
        {
            get { return SBIGHandlerShared.IsConnected; }
        }

        #endregion
    }
}
