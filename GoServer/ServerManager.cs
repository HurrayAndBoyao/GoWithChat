﻿using Newtonsoft.Json;
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
        public Hashtable fightTable;

        public ServerManager(int port, RichTextBox tb_output)
        {
            this.port = port;
            this.tb_output = tb_output;

            StartServer();
        }

        public void ExitServer()
        {
            tcpListener.Stop();
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
            hash = new Hashtable(); //创建一个Hashtable实例 
            tb_output.AppendText("建立Hash\n");
            fightTable = new Hashtable();
            tb_output.AppendText("建立FightMap\n");
        }

        public void ListenThread()
        {
            while (IsWorking)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    tb_output.AppendText("一个新用户接入\n");
                    new Thread(() => UserThread(tcpClient)).Start();
                }
                catch (Exception e)
                {
                    tb_output.AppendText("error:"+e+"\n");
                }
            }
        }

        public void UserThread(TcpClient tcpClient)
        {
            while (true)
            {
                try
                {
                    if (!tcpClient.Connected)
                    {
                        throw new Exception("啊啊啊");
                    }

                    receiveJsonAndAns(tcpClient);
                }
                catch (Exception ex)
                {
                    tb_output.AppendText("一个用户离线！\n");
                    //清理hash
                    foreach (DictionaryEntry de in hash)
                    {
                        if (de.Value == tcpClient)
                        {
                            foreach (DictionaryEntry d in fightTable)
                            {
                                if (d.Key.Equals(de.Key))
                                {
                                    fightTable.Remove(d.Key);
                                    tb_output.AppendText("成功删除用户对战信息！\n");
                                }
                            }
                            hash.Remove(de.Key);
                            tb_output.AppendText("成功删除用户登录信息！\n");
                            
                            break;
                        }
                    }
                    break;
                }
            }
        }

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

                case R.CMD_QUIT_FIGHT:
                    quitFight(newbundle, tcpclient);break;
            }
        }

        private void quitFight(MsgBundle newbundle, TcpClient tcpclient)
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_QUIT_FIGHT;
                sendBundle.status = R.STATUS_SUCCESS;
                sendBundle.username = newbundle.friendname;
                sendBundle.friendname = newbundle.username;
                sendMsg(newbundle.friendname, JsonConvert.SerializeObject(sendBundle));
            }
            catch
            {
                newbundle.status = R.STATUS_FAILED;
                newbundle.note = R.NOTE_FRIEND_NOT_ONLINE;
                sendMsg(newbundle.friendname, JsonConvert.SerializeObject(newbundle));
            }
        }

        private void fight(MsgBundle newbundle, TcpClient tcpclient)
        {
           
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_FIGHT;
                sendBundle.status = R.STATUS_SUCCESS;
                sendBundle.username = newbundle.friendname;
                sendBundle.friendname = newbundle.username;
                sendBundle.fightInfo = newbundle.fightInfo;
                sendMsg(newbundle.friendname, JsonConvert.SerializeObject(sendBundle));
            }
            catch
            {
                newbundle.status = R.STATUS_FAILED;
                newbundle.note = R.NOTE_FRIEND_NOT_ONLINE;
                sendMsg(newbundle.friendname, JsonConvert.SerializeObject(newbundle));
            }
            
        }

        private void agreeFight(MsgBundle newbundle, TcpClient tcpclient)
        {
            throw new NotImplementedException();
        }

        private void applyFight(MsgBundle newbundle, TcpClient tcpclient)
        {
            if (!fightTable.ContainsKey(newbundle.username) && !fightTable.ContainsKey(newbundle.friendname))
            {
                try
                {
                    if (newbundle.friendname.Equals(newbundle.username))
                    {
                        MsgBundle sendBundle = new MsgBundle();
                        sendBundle.type = R.CMD_APPLY_FIGHT;
                        sendBundle.status = R.STATUS_FAILED;
                        sendBundle.username = newbundle.username;
                        sendBundle.friendname = newbundle.friendname;
                        sendBundle.note = R.NOTE_SELF_FIGHT;
                        sendMsg(newbundle.username, JsonConvert.SerializeObject(sendBundle));
                    }
                    else if (hash.ContainsKey(newbundle.friendname))
                    {
                        //先将二者加入已经对战的表中
                        fightTable.Add(newbundle.friendname, newbundle.username);
                        fightTable.Add(newbundle.username, newbundle.friendname);

                        //然后给双方返回开始对战的消息
                        MsgBundle sendBundle = new MsgBundle();
                        sendBundle.type = R.CMD_APPLY_FIGHT;
                        sendBundle.status = R.STATUS_SUCCESS;
                        sendBundle.username = newbundle.friendname;
                        sendBundle.friendname = newbundle.username;
                        sendBundle.isBlack = 0;
                        sendMsg(newbundle.friendname, JsonConvert.SerializeObject(sendBundle));
                        sendBundle.username = newbundle.username;
                        sendBundle.friendname = newbundle.friendname;
                        sendBundle.isBlack = 1;
                        sendMsg(newbundle.username, JsonConvert.SerializeObject(sendBundle));
                    }
                    else
                    {
                        MsgBundle sendBundle = new MsgBundle();
                        sendBundle.type = R.CMD_APPLY_FIGHT;
                        sendBundle.status = R.STATUS_FAILED;
                        sendBundle.username = newbundle.username;
                        sendBundle.friendname = newbundle.friendname;
                        sendBundle.note = R.NOTE_FRIEND_NOT_ONLINE;
                        sendMsg(newbundle.username, JsonConvert.SerializeObject(sendBundle));
                    }
                }
                catch
                {
                    newbundle.status = R.STATUS_FAILED;
                    newbundle.note = R.NOTE_FRIEND_NOT_ONLINE;
                    sendMsg(newbundle.friendname, JsonConvert.SerializeObject(newbundle));
                }
            }
            else
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_APPLY_FIGHT;
                sendBundle.status = R.STATUS_FAILED;
                sendBundle.username = newbundle.username;
                sendBundle.friendname = newbundle.friendname;
                sendBundle.note = R.NOTE_ALREADY_FIGHT;
                sendMsg(newbundle.username, JsonConvert.SerializeObject(sendBundle));
            }
        }

        private void findFriend(MsgBundle newbundle, TcpClient tcpclient)
        {
            String [] friendArray = new String[hash.Count];
            hash.Keys.CopyTo(friendArray, 0);
            MsgBundle returnBundle = new MsgBundle();
            returnBundle.type = R.CMD_FIND_FRIEND;
            returnBundle.status = R.STATUS_SUCCESS;
            returnBundle.allOnlineName = friendArray;
            sendMsg(tcpclient, JsonConvert.SerializeObject(returnBundle));
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
                    //登录成功
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
            Byte[] data = new Byte[R.MAX_BUFFER_NUM];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
            tb_output.AppendText("【接收】"+responseData + "\n");
            return responseData;
        }

        public string receiveMsg(TcpClient socket)
        {
            NetworkStream stream = socket.GetStream();
            Byte[] data = new Byte[R.MAX_BUFFER_NUM];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
            tb_output.AppendText("【接收】" + responseData + "\n");
            return responseData;
        }

        public Boolean sendMsg(String name, String msg)
        {
            if (hash.Contains(name))
            {
                try
                {
                    NetworkStream stream = ((TcpClient)hash[name]).GetStream();
                    Byte[] bytes = Encoding.Unicode.GetBytes(msg);
                    stream.Write(bytes, 0, bytes.Length);
                    tb_output.AppendText("【发送】" + msg + "\n");
                    return true;
                }
                catch (Exception notonline)
                {
                    tb_output.AppendText("【exception】该用户不在线\n");
                    return false;
                }
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
                try
                {
                    NetworkStream stream = socket.GetStream();
                    Byte[] bytes = Encoding.Unicode.GetBytes(msg);
                    stream.Write(bytes, 0, bytes.Length);
                    tb_output.AppendText("【发送】" + msg + "\n");
                    return true;
                }
                catch (Exception notonline)
                {
                    tb_output.AppendText("【exception】该用户不在线\n");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


    }
}
