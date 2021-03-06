﻿using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections; //使用Hashtable时，必须引入这个命名空间 
using System.Drawing;
using System.Runtime.InteropServices;

namespace GoWithChat
{
    public class ClientManager
    {
        public TcpClient tcpClient;
        public NetworkStream stream;
        public MainForm mainform;
        public Landed landedForm;
        public Hashtable hash_form;
        public String username;
        public String[] friendList;
        public Board board;
        public MethodInvoker sb;
        public MethodInvoker sm;
        public int black;
        public String friendname;
        public String fightinfo;

        public ClientManager()
        {
            //System.Threading.Thread.CurrentThread.Name = "2";
            sb = new MethodInvoker(showboard);
            sm = new MethodInvoker(sendmessage);
            //board = new Board(0,0,null,null);
            board.Show();
            board.Hide();
            hash_form = new Hashtable();
        }

        public void closeClient()
        {
            tcpClient.Close();
            landedForm.Close();
            Application.ExitThread();
            Application.Exit();
        }

        public void Landed(String username, String passwd, Landed landedForm)
        {
            this.landedForm = landedForm;
            this.username = username;
            try
            {
                this.tcpClient = new TcpClient();
                tcpClient.Connect(R.IPADDRESS, R.PORT);
                this.stream = tcpClient.GetStream();


                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_LOGIN;
                sendBundle.username = username;
                sendBundle.passwd = passwd;
                sendMessage(JsonConvert.SerializeObject(sendBundle));

                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                if (receiveBundle.type == R.CMD_LOGIN && receiveBundle.status == R.STATUS_SUCCESS)
                {
                    //登录成功
                    mainform = new MainForm(this);
                    mainform.Text = username;
                    mainform.Show();
                    landedForm.Hide();
                    //打开各线程
                    new Thread(ListenThread).Start();
                    new Thread(()=>getFriendList(mainform)).Start();
                }
                else
                {
                    new Note(receiveBundle.note).Show();
                }
            }
            catch (Exception e)
            {
                new Note(R.NOTE_SERVER_UNCONNECT + e).Show();
            }
        }

        //【博耀】不断更新好友列表展示的线程
        public void UpdateFriendListThread()//博耀编辑
        {
            //int i;
            //LinkLabel [] label = new LinkLabel [99];
            //mainform.panel1.Controls.Clear();
            //for (i = 0;i < ss.Length;i++)
            //{//接下来的工作：通过代理实现跨线程调用控件。
                //label[i] = new LinkLabel();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            mainform.panel1.Invoke(new MainForm.Setlabels(Addlabels),new object[] {friendList});
                //mainform.panel1.Controls.Add(label[i]);
                //label[i].Links.Add(0,0,"" + i);
                //label[i].ActiveLinkColor = Color.Blue;
                //label[i].LinkBehavior = LinkBehavior.HoverUnderline;
                //label[i].AutoEllipsis = true;
                //label[i].AutoSize = false;
                //label[i].Location = new System.Drawing.Point(15, 15 + 15 * i);
                //label[i].Name = "label" + i;
                //label[i].Size = new System.Drawing.Size(150, 15);
                //label[i].TabIndex = 0;
                //label[i].Text = ss[i];
                //label[i].LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.label_Click);
            //}
        }

        //清扫对方离线的对战
        public void SwapOfflineFightThread()
        {
            ArrayList ar = new ArrayList();//实例化一个ArrayList
            if (friendList.Length != 0)//博耀修改
            {
                ar.AddRange(friendList);//把数组赋到Arraylist对象
            }
            foreach (string s in hash_form.Keys)
            {
                if (!ar.Contains(s))
                {
                    //【博耀】停止该board继续的方法调用(可以考虑由联机版变单机版)
                    new Note(R.NOTE_FRIEND_NOT_ONLINE_FIGHT_END).Show();
                    hash_form.Remove(s);
                }
            }
        }

        public void ListenThread()
        {
            //mi.BeginInvoke(null,null);
            //board.Hide();
            while (true)
            {
                //board.Show();
                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                //失败包
                if (receiveBundle.status == R.STATUS_FAILED)
                {
                    new Note(receiveBundle.note).Show();
                }
                // 更新好友列表
                else if (receiveBundle.type == R.CMD_FIND_FRIEND && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.allOnlineName != null)
                {
                    friendList = receiveBundle.allOnlineName;
                    new Thread(UpdateFriendListThread).Start();
                    new Thread(SwapOfflineFightThread).Start();
                }
                // 申请对战
                else if (receiveBundle.type == R.CMD_APPLY_FIGHT && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.friendname != null)
                {
                    openFightBoard(receiveBundle.friendname, receiveBundle.isBlack);
                }
                else if (receiveBundle.type == R.CMD_FIGHT && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.friendname != null)
                {
                    //根据hash_form找到对应的board对象，然后进行相应操作
                    //Board board = (Board)hash_form[receiveBundle.friendname];
                    this.fightinfo = receiveBundle.fightInfo;
                    sm.BeginInvoke(null,null);
                    //board.get_from_server(receiveBundle.fightInfo);
                }
                else if (receiveBundle.type == R.CMD_FIGHT_CANCLE)
                {
                    //根据hash_form找到对应的board对象，然后将该页面改为单机版，并在hash_form中删除该对象，提示用户返回的note信息
                    new Note(receiveBundle.note).Show();
                }
            }
        }
        public void openFightBoard(String friendname, int black)
        {
            try
            {
                if (hash_form.Contains(friendname))
                {
                    throw new Exception(R.NOTE_ALLREADY_START);
                }
                else if(friendname.Equals(username))
                {
                    throw new Exception(R.NOTE_SELF_FIGHT);
                }
                else
                {
                    this.black = black;
                    this.friendname = friendname;
                    sb.BeginInvoke(null, null);
                    //Board board = new Board(1, black, this, friendname);
                    //Board board = new Board(0, 0, null, null);
                    //board.isonline = 1;
                    //board.color = black;
                    //board.clientmanager = this;
                    //board.friendname = friendname;
                    //board.Show();
                    //hash_form.Add(friendname, board);
                    
                }
            }
            catch(Exception e)
            {
                new Note(e.ToString()).Show(); 
            }
        }

        public void Fight(String fightInfo, String friendname)
        {
            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_FIGHT;
            sendBundle.username = username;
            sendBundle.friendname = friendname;
            sendBundle.fightInfo = fightInfo;
            sendMessage(JsonConvert.SerializeObject(sendBundle));
        }

        public void ApplyFight(String friendname)
        {
            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_APPLY_FIGHT;
            sendBundle.username = username;
            sendBundle.friendname = friendname;
            sendMessage(JsonConvert.SerializeObject(sendBundle));
        }

        public void getFriendList(Form mainform)
        {
            while (true)
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_FIND_FRIEND;
                sendMessage(JsonConvert.SerializeObject(sendBundle));

                //5秒刷新一次
                Thread.Sleep(R.UPDATE_TIME);
            }
        }

        public void BeginFight(String friendName)
        {
            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_APPLY_FIGHT;
            sendBundle.friendname = friendName;
            sendMessage(JsonConvert.SerializeObject(sendBundle));            
        }

        public string receiveMsg()
        {
            Byte[] data = new Byte[R.MAX_BUFFER_NUM];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
            //tb_output.AppendText(responseData + "!\n");
            return responseData;
        }

        public void sendMessage(String msg)
        {
            Byte[] bytes = Encoding.Unicode.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void showNote(String note)
        {
            Note newnote = new Note(note);
            newnote.Show();
        }
        public void get_from_board(String s,String friendname)//博耀添加
        {
            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_FIGHT;
            sendBundle.username = username;
            sendBundle.friendname = friendname;
            sendBundle.fightInfo = s;
            sendMessage(JsonConvert.SerializeObject(sendBundle));
        }
        public void Addlabels(String[] friends)//博耀编辑,和控件委托相关的处理函数
        {
            int i;
            LinkLabel[] label = new LinkLabel[99];
            mainform.panel1.Controls.Clear();
            for (i = 0; i < friends.Length; i++)
            {
                label[i] = new LinkLabel();
                mainform.panel1.Controls.Add(label[i]);
                label[i].Links.Add(0,0,friends[i]);
                //label[i].ActiveLinkColor = Color.Blue;
                //label[i].LinkColor = Color.Black;
                label[i].LinkBehavior = LinkBehavior.HoverUnderline;
                label[i].AutoEllipsis = true;
                label[i].AutoSize = false;
                label[i].Location = new System.Drawing.Point(15, 15 + 30 * i);
                label[i].Name = "label" + i;
                label[i].Size = new System.Drawing.Size(150, 30);
                label[i].TabIndex = 0;
                label[i].Text = friends[i];
                label[i].Font = new Font("宋体",20);
                label[i].LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.label_Click);
            }
        }
        private void label_Click(object sender, LinkLabelLinkClickedEventArgs e)//博耀编辑
        {

            
            String name = e.Link.LinkData.ToString();//name就是选中的好友的名字
            if (!hash_form.Contains(name))
            {
                //openFightBoard(name, R.WIGHT);
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_APPLY_FIGHT;
                sendBundle.username = username;
                sendBundle.friendname = name;
                sendMessage(JsonConvert.SerializeObject(sendBundle));
            }
            else
            {
                new Note(R.NOTE_ALREADY_FIGHT).Show();
            }
        }
        public void showboard()
        {
            MessageBox.Show("对战即将开始，请准备好哦！");
            board.isonline = 1;
            board.color = black;
            //board.clientmanager = this;
            board.friendname = friendname;
            if (black == 0)
            {
                board.Text = "黑方：" + username;
            }
            else
            {
                board.Text = "白方：" + username;
            }
            board.Show();
            //hash_form.Add(friendname, board);
        }
        public void sendmessage()
        {
            //board.get_from_server(fightinfo); 
        }
    }
}