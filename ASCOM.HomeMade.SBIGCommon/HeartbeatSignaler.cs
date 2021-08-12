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
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class HeartbeatSignaler
    {
        private BackgroundWorker bw = null;
        private SBIGClient socket = null;
        private bool error = false;

        public HeartbeatSignaler(SBIGClient client)
        {
            socket = client;

            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!error)
            {
                try
                {
                     error = !socket.PingServer();

                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
    }
}
