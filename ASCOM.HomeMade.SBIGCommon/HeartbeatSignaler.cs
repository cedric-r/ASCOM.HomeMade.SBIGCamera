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
        private NetworkStream socket = null;
        private bool error = false;

        public HeartbeatSignaler(NetworkStream client)
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
                    byte[] outStream = System.Text.Encoding.UTF8.GetBytes("PING" + "<EOM>");
                    socket.Write(outStream, 0, outStream.Length);
                    socket.Flush();

                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
    }
}
