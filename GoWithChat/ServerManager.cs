using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GoWithChat
{
    public class ServerManager
    {
        public event Action<string> ConsoleOutput;
        public int port { get; set; }
        public bool IsWorking { get; private set; }
        public TcpListener tcpListener;

        public ServerManager(int port)
        {
            this.port = port;
        }

        public void StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
            }
            catch (Exception e)
            {
 
            }
            IsWorking = true;
            tcpListener.Start();
            ConsoleOutput("Server started!");

        }
    }
}
