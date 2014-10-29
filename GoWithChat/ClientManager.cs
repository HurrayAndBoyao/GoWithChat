using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections; //使用Hashtable时，必须引入这个命名空间 

namespace GoWithChat
{
    public class ClientManager
    {
        public TcpClient tcpClient;
        public NetworkStream stream;
        public Form mainform;
        public Form landedForm;
        public Hashtable hash_form;
        public String username;
        public String[] friendList;

        public ClientManager()
        {
            hash_form = new Hashtable();
        }

        public void closeClient()
        {
            tcpClient.Close();
            landedForm.Close();
            Application.ExitThread();
            Application.Exit();
        }

        public void Landed(String username, String passwd, Form landedForm)
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
                    mainform.Show();
                    landedForm.Hide();
                    //打开各线程
                    new Thread(ListenThread).Start();
                    new Thread(()=>getFriendList(mainform)).Start();
                    new Thread(SwapOfflineFightThread).Start();
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
        public void UpdateFriendListThread()
        {
            //好友列表string[] friendList直接调用
            //展示界面是MainForm 直接调用mainform对象
        }

        //清扫对方离线的对战
        public void SwapOfflineFightThread()
        {
            ArrayList ar = new ArrayList();//实例化一个ArrayList
            ar.AddRange(friendList);//把数组赋到Arraylist对象
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
            while (true)
            {
                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);

                // 更新好友列表
                if (receiveBundle.type == R.CMD_FIND_FRIEND && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.allOnlineName != null)
                {
                    friendList = receiveBundle.allOnlineName;
                    new Thread(UpdateFriendListThread).Start();
                }
                // 开启对战窗口
                else if (receiveBundle.type == R.CMD_APPLY_FIGHT && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.friendname != null)
                {
                    // 开启Form:Board（多人版）    , 记录form到HashTable中 
                    if (!hash_form.Contains(receiveBundle.friendname))
                    {
                        //hash_form.Add(receiveBundle.friendname, (form));
                    }
                    else
                    {
                        new Note(R.NOTE_ALLREADY_START).Show();
                    }
                   
                }
                else if (receiveBundle.type == R.CMD_FIGHT && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.friendname != null)
                {
                    //根据hash_form找到对应的board对象，然后进行相应操作
                }
                else if (receiveBundle.type == R.CMD_FIGHT_CANCLE)
                {
                    //根据hash_form找到对应的board对象，然后将该页面改为单机版，并在hash_form中删除该对象，提示用户返回的note信息
                    new Note(receiveBundle.note).Show();
                }
                
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
                Thread.Sleep(5000);
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
    }
}
