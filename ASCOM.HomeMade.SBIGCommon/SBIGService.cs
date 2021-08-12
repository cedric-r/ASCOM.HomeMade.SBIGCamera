﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SbigSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGService
    {
        private const string driverID = "ASCOM.HomeMade.SBIGService";
        public const string _Url = "tcp://127.0.0.1:5557";
        private BackgroundWorker bw = null;
        private BackgroundWorker memoryControlWorker = null;
        private static SBIGService _Instance = null;
        private static Debug debug = null;
        private string Url = "";

        public bool Shutdown = false;

        #region Service management
        public static SBIGService CreateService(string url)
        {
            if (debug == null)
            {
                string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                strPath = Path.Combine(strPath, driverID);
                try
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }
                catch (Exception) { }
                debug = new ASCOM.HomeMade.SBIGCommon.Debug(driverID, Path.Combine(@"c:\temp\", "SBIGService_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".log"));
            }
            if (_Instance == null)
            {
                debug.LogMessage("SBIGService", "Creating instance");
                if (String.IsNullOrEmpty(url)) url = _Url;
                _Instance = new SBIGService(url);
            }
            debug.LogMessage("SBIGService", "Returning instance");
            return _Instance;
        }

        public static SBIGService CreateService()
        {
               return CreateService(_Url);
        }

        private SBIGService(string url)
        {
            Url = url;

            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
            Thread.Sleep(500);

            memoryControlWorker = new BackgroundWorker();
            memoryControlWorker.DoWork += memoryControlWorker_DoWork;
            memoryControlWorker.RunWorkerAsync();
        }

        private void memoryControlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Shutdown)
            {
                new MemoryController().ControlMemory();
                Thread.Sleep(60000); // Every minute
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Loopback, 5557);
                TcpClient clientSocket = default;

                int counter = 0;

                serverSocket.Start();
                //Console.WriteLine(" >> " + "Server Started");

                counter = 0;
                while (!Shutdown)
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");

                    SocketHandler sh = new SocketHandler(clientSocket, counter);
                    Task.Factory.StartNew(() => { sh.Handle(); });
                    }

                serverSocket.Stop();
            }
            catch(Exception ex)
            {
                debug.LogMessage("SBIGService dowork", "Error: "+Utils.DisplayException(ex));
            }
        }

        public void Stop()
        {
            debug.LogMessage("SBIGService", "Stopping service");
            Shutdown = true;
            /*
            try
            {
                bw.CancelAsync();
            }
            catch (Exception) { }

            try
            {
                memoryControlWorker.CancelAsync();
            }
            catch (Exception) { }

            if (server!=null)
            {
                try
                {
                    debug.LogMessage("SBIGService", "Disconnect USB connections");
                    server.DisconnectAll();
                }
                catch (Exception) { }
            }
            */
        }

        #endregion
    }
}
