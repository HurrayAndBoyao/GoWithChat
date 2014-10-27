using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections; //使用Hashtable时，必须引入这个命名空间 

namespace GoServer
{
    public class ServerManager
    {
        //public event Action<string> ConsoleOutput;
        public int port { get; set; }
        public bool IsWorking { get; private set; }
        public TcpListener tcpListener;
        public RichTextBox tb_output;
        public Hashtable hash;

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
            Hashtable hash = new Hashtable(); //创建一个Hashtable实例 
            tb_output.AppendText("建立HashMap\n");
            this.hash = hash;
        }

        public void ListenThread()
        {
            while (IsWorking)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    //hash.Add("test",tcpClient);
                    //tb_output.AppendText(tcpClient.Client.RemoteEndPoint.ToString()+"\n");//连接检测
                    //receiveMsg("test");
                    //sendMsg("test","hello Hurray!");
                    receiveJsonAndAns(tcpClient);
                }
                catch (Exception e)
                {
                    tb_output.AppendText("error:"+e+"\n");
                }
            }
        }
        /*
        public Boolean landed(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            Byte[] data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            tb_output.AppendText(responseData + "!\n");
            return true;
        }
         * */

        public void receiveJsonAndAns(TcpClient tcpclient)
        {
            String getJsonMsg = receiveMsg(tcpclient);
            MsgBundle newbundle = JsonConvert.DeserializeObject<MsgBundle>(getJsonMsg);

            switch (newbundle.type)
            {
                case R.CMD_LOGIN:
                    login(newbundle, tcpclient); break;

                case R.CMD_LOGOUT: 
                    logout(newbundle, tcpclient); break;

                case R.CMD_FIND_FRIEND: 
                    findFriend(newbundle, tcpclient); break;

                case R.CMD_APPLY_FIGHT:
                    applyFight(newbundle, tcpclient); break;

                case R.CMD_AGREE_FIGHT:
                    agreeFight(newbundle, tcpclient); break;

                case R.CMD_FIGHT:
                    fight(newbundle, tcpclient); break;
            }
        }

        private void fight(MsgBundle newbundle, TcpClient tcpclient)
        {
            throw new NotImplementedException();
        }

        private void agreeFight(MsgBundle newbundle, TcpClient tcpclient)
        {
            throw new NotImplementedException();
        }

        private void applyFight(MsgBundle newbundle, TcpClient tcpclient)
        {
            throw new NotImplementedException();
        }

        private void findFriend(MsgBundle newbundle, TcpClient tcpclient)
        {
            throw new NotImplementedException();
        }

        private void logout(MsgBundle newbundle, TcpClient tcpclient)
        {
            MsgBundle returnBundle = new MsgBundle();
            hash.Remove(newbundle.username);
            returnBundle.type = R.CMD_LOGOUT;
            returnBundle.status = R.STATUS_SUCCESS;
            sendMsg(tcpclient, JsonConvert.SerializeObject(returnBundle));
        }

        private void login(MsgBundle newbundle, TcpClient tcpclient)
        {
            if (hash.Contains(newbundle.username))
            {
                MsgBundle returnBundle = new MsgBundle();
                returnBundle.type = R.CMD_LOGIN;
                returnBundle.status = R.STATUS_FAILED;
                returnBundle.note = R.NOTE_SAMENAME;
                sendMsg(tcpclient, JsonConvert.SerializeObject(returnBundle));
            }
            else
            {
                if (newbundle.username.Equals(newbundle.passwd))
                {
                    MsgBundle returnBundle = new MsgBundle();
                    returnBundle.type = R.CMD_LOGIN;
                    returnBundle.status = R.STATUS_SUCCESS;
                    hash.Add(newbundle.username, tcpclient);
                    sendMsg(tcpclient, JsonConvert.SerializeObject(returnBundle));
                }
                else
                {
                    MsgBundle returnBundle = new MsgBundle();
                    returnBundle.type = R.CMD_LOGIN;
                    returnBundle.status = R.STATUS_FAILED;
                    returnBundle.note = R.NOTE_ERROR_CODE;
                    sendMsg(tcpclient, JsonConvert.SerializeObject(returnBundle));
                }
            }
        }

        public string receiveMsg(String name)
        {
            NetworkStream stream = ((TcpClient)hash[name]).GetStream();
            Byte[] data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            tb_output.AppendText(responseData + "!\n");
            return responseData;
        }

        public string receiveMsg(TcpClient socket)
        {
            NetworkStream stream = socket.GetStream();
            Byte[] data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            tb_output.AppendText(responseData + "!\n");
            return responseData;
        }

        public Boolean sendMsg(String name, String msg)
        {
            if (hash.Contains(name))
            {
                NetworkStream stream = ((TcpClient)hash[name]).GetStream();
                Byte[] bytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(bytes, 0, bytes.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean sendMsg(TcpClient socket, String msg)
        {
            if (socket!=null)
            {
                NetworkStream stream = socket.GetStream();
                Byte[] bytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(bytes, 0, bytes.Length);
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
