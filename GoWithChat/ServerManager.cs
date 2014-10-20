using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.IO;

namespace GoWithChat
{
    public class ServerManager
    {
        public event Action<string> ConsoleOutput;
        public int port { get; set; }
        public bool IsWorking { get; private set; }
        public TcpListener tcpListener;
        public RichTextBox tb_output;

        public ServerManager(int port, RichTextBox tb_output)
        {
            this.port = port;
            this.tb_output = tb_output;

            StartServer();
        }

        public void StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tb_output.AppendText("服务器已在"+ port +"端口创建成功！\n");
            }
            catch (Exception e)
            {
                tb_output.AppendText(e.ToString());
                tb_output.AppendText("端口创建异常，请重试其他端口！\n");
            }
            IsWorking = true;
            tcpListener.Start();

            Thread listenThread = new Thread(ListenThread);
            Control.CheckForIllegalCrossThreadCalls = false;
            listenThread.Start();
            tb_output.AppendText("接听开始\n");

        }

        public void ListenThread()
        {
            while (IsWorking)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    
                    tb_output.AppendText(tcpClient.Client.RemoteEndPoint.ToString()+"\n");
                    NetworkStream stream = tcpClient.GetStream();

                    Byte[] data = new Byte[256];
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    tb_output.AppendText(responseData + "!\n");


                }
                catch
                {
                    tb_output.AppendText("error\n");
                }
            }
        }
    }
}
