using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class HeartbeatMonitor
    {
        public static TimeSpan TIMEOUT = TimeSpan.FromSeconds(25);
        private bool signalFlag;
        private DateTime lastBeat = DateTime.Now;
        private BackgroundWorker bw = null;

        public HeartbeatMonitor(ref bool flag)
        {
            signalFlag = flag;
            signalFlag = false;

            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        public void processMessage(string message)
        {
            if (message == "PING") lastBeat = DateTime.Now;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!signalFlag)
            {
                if (lastBeat + TIMEOUT < DateTime.Now) signalFlag = true;
                Thread.Sleep(5000);
            }
        }
    }
}
