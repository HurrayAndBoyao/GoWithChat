using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections; //使用Hashtable时，必须引入这个命名空间 
using System.Runtime.InteropServices;
//与服务器端的交互接口如下：
//1：构造函数，三个参数，第一个int为0表示单机，为1表示联机。
//                       第二个int为0表示黑棋，为1表示白棋。
//                       第三个是ClientManager的引用，用于传输消息，单机的话参数是0就好了。
//2：发送消息：已经调用ClientManager的get_from_board(String s)方法，请在ClientManager里面编辑它，把消息原封不动发给另一个用户。
//3：接收消息：Board类的public方法，get_from_server(String s)方法，请把另一个用户发送的消息原封不动地传到这里来。
namespace GoWithChat
{
    public partial class Board : Form
    {
        /** @author jby */

        Unit[,] unit = new Unit[19, 19];//单元格数组
        static int step = 0;//表示当前步数
        Image img_black = new Bitmap("../../resource/img/black.png");
        Image img_white = new Bitmap("../../resource/img/white.png");
        Boolean[,] p = new Boolean[19, 19];//应用与dfs的一个布尔数组
        public int isonline, color;
        public ClientManager clientmanager;
        private Graphics g;

        public TcpClient tcpClient;
        public NetworkStream stream;
        public String username;//【需要构造函数传进来】
        public String friendname;

        public Board(int isonline, int color,ClientManager clientmanager,String friendname)//Board的构造函数接收两个参数，第一个0表示单机，1表示联机。第二个表示这个板子是由哪个颜色下棋的。
        {
            int i, j,k;
            this.isonline = isonline;
            this.color = color;
            this.clientmanager = clientmanager;
            this.friendname = friendname;
            InitializeComponent();
            this.SuspendLayout();
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    for (k = 0; k < 2;k++ )
                    {
                        this.pictureBox[i, j,k] = new System.Windows.Forms.PictureBox();
                        ((System.ComponentModel.ISupportInitialize)(this.pictureBox[i, j,k])).BeginInit();
                        this.pictureBox[i, j,k].Location = new System.Drawing.Point(22 + i * 23, 22 + j * 23);
                        this.pictureBox[i, j,k].Name = "pictureBox[" + (i + 1) + "][" + (j + 1) + "]";
                        this.pictureBox[i, j,k].Size = new System.Drawing.Size(19, 19);
                        this.pictureBox[i, j,k].TabIndex = 0;
                        this.pictureBox[i, j,k].BackColor = Color.Transparent;
                        this.pictureBox[i, j,k].TabStop = false;
                        if (k == 0)
                        {
                            this.pictureBox[i, j, k].Image = img_black;
                        } 
                        else
                        {
                            this.pictureBox[i, j, k].Image = img_white;
                        }
                        this.Controls.Add(this.pictureBox[i, j,k]);
                        ((System.ComponentModel.ISupportInitialize)(this.pictureBox[i, j,k])).EndInit();
                        this.pictureBox[i, j,k].Show();
                        this.pictureBox[i, j,k].Hide();
                    }
                }
            }
            this.ResumeLayout(false);
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    unit[i, j] = new Unit(i, j);
                }
            }
        }
        private void PaintHandler(Object sender, PaintEventArgs e)
        {
            int i, j;
            //Graphics g = e.Graphics;
            g = e.Graphics;
            Brush brText = new SolidBrush(Color.Black);
            Brush brBoard = new SolidBrush(Color.Orange);
            Brush brStar = new SolidBrush(Color.Black);
            Brush brBlack = new SolidBrush(Color.Black);
            Brush brWhite = new SolidBrush(Color.White);
            Pen penLine = new Pen(Color.Black, 1);
            Pen penSide1 = new Pen(Color.WhiteSmoke, 2);
            Pen penSide2 = new Pen(Color.Gray, 2);
            string[] strH = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };
            string[] strV = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };

            if (step % 2 == 0)
            {
                textBox1.Text = "黑棋下棋";
            }
            else
            {
                textBox1.Text = "白棋下棋";
            }

            g.DrawLine(penSide2, 458, 19, 458, 458);
            g.DrawLine(penSide2, 19, 458, 458, 458);
            g.DrawLine(penSide1, 19, 19, 458, 19);
            g.DrawLine(penSide1, 19, 19, 19, 458);
            for (i = 0; i < 19; i++)
            {
                g.DrawString(strV[i], this.Font, brText, 0, 24 + 23 * i);//画坐标
                g.DrawString(strH[i], this.Font, brText, 24 + 23 * i, 0);
            }
            //画棋板
            g.FillRectangle(brBoard, 20, 20, 23 * 19, 23 * 19);

            //画线
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    if (i != 0)
                    {
                        g.DrawLine(penLine, 20 + i * 23, 31 + j * 23, 31 + i * 23, 31 + j * 23);
                    }
                    if (i != 18)
                    {
                        g.DrawLine(penLine, 31 + i * 23, 31 + j * 23, 43 + i * 23, 31 + j * 23);
                    }
                    if (j != 0)
                    {
                        g.DrawLine(penLine, 31 + i * 23, 20 + j * 23, 31 + i * 23, 31 + j * 23);
                    }
                    if (j != 18)
                    {
                        g.DrawLine(penLine, 31 + i * 23, 31 + j * 23, 31 + i * 23, 43 + j * 23);
                    }
                }
            }
            //画星
            g.FillRectangle(brStar, 30 + 3 * 23, 30 + 3 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 3 * 23, 30 + 9 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 3 * 23, 30 + 15 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 9 * 23, 30 + 3 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 9 * 23, 30 + 9 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 9 * 23, 30 + 15 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 15 * 23, 30 + 3 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 15 * 23, 30 + 9 * 23, 3, 3);
            g.FillRectangle(brStar, 30 + 15 * 23, 30 + 15 * 23, 3, 3);
        }
        public void get_from_server(String s)//收取来自对手的消息
        {
            if (s[0] == '0')//下棋
            {
                if (go((s[1] - '0') * 10 + (s[2] - '0'), (s[3] - '0') * 10 + (s[4] - '0')) == 1)//如果此子可落
                {
                    step = step + 1;
                } 
            }
        }
        private Point PointToGrid(int x, int y)
        {
            Point p = new Point(0, 0);
            p.X = (x - 20) / 23;
            p.Y = (y - 20) / 23;
            return p;
        }
        private void MouseUpHandler(Object sender, MouseEventArgs e)
        {
            Point p;
            String s;

            if (isonline == 1)
            {
                if (color != (step % 2))
                {
                    return;
                }
            }

            p = PointToGrid(e.X, e.Y);

            if ((p.X >= 0) && (p.X <= 18) && (p.Y >= 0) && (p.Y <= 18))
            {
                if (go(p.X, p.Y) == 1)//如果此子可落
                {
                    if (isonline == 1)
                    {
                        s = "0" + (p.X / 10) + (p.X % 10) + (p.Y / 10) + (p.Y % 10);
                        clientmanager.get_from_board(s,friendname);//向服务器发送信息。
                        //MessageBox.Show();
                    }
                    step = step + 1;
                } 
            }
        }
        private int go(int x, int y)
        {
            int color;
            int caneat;
            if (unit[x, y].color != -1) return 0;//如果当前点不是空的，下不了
            color = step % 2;
            caneat = can_eat_other(x, y, color);
            if (caneat != 0)//如果下了能吃别人的棋
            {
                if (ko(x, y,color) == 1) return 0;//如果落子在劫内，下不了
            }
            else//如果下了不能吃别人的棋
            {
                if (dfs_compute(x, y, color) == 0) return 0;//如果自己下了就死，下不了
            }
            unit[x, y].color = color;
            pictureBox[x, y,color].Show();//显示这个棋子
            if ((caneat / 1000) == 1)
            {
                be_eaten(x - 1, y);
            }
            if (((caneat / 100) % 10) == 1)
            {
                be_eaten(x + 1, y);
            }
            if (((caneat / 10) % 10) == 1)
            {
                be_eaten(x, y - 1);
            }
            if ((caneat % 10) == 1)
            {
                be_eaten(x, y + 1);
            }
            return 1;
        }
        private void be_eaten(int x,int y)//处理被吃的子的函数
        {
            int i, j;
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    p[i, j] = false;
                }
            }
            dfs(x, y,unit[x,y].color);
        }
        private int can_eat_other(int x,int y,int color)//判断周围有没有只剩一口气的异色的子
        {
            int eat = 0;
            if (x != 0)
            {
                if (unit[x - 1,y].color == 1 - color)
                {
                    if (dfs_compute(x - 1,y,1 - color) == 1) eat = eat + 1000;
                }
            } 
            if (x != 18)
            {
                if (unit[x + 1, y].color == 1 - color)
                {
                    if (dfs_compute(x + 1, y, 1 - color) == 1) eat = eat + 100;
                }
            }
            if (y != 0)
            {
                if (unit[x, y - 1].color == 1 - color)
                {
                    if (dfs_compute(x, y - 1, 1 - color) == 1) eat = eat + 10;
                }
            }
            if (y != 18)
            {
                if (unit[x, y + 1].color == 1 - color)
                {
                    if (dfs_compute(x, y + 1, 1 - color) == 1) eat = eat + 1;
                }
            } 
            return eat;
        }
        private int ko(int x,int y,int color)//判断是否造成全局同型
        {
            return 0;
        }
        private int dfs_compute(int x,int y,int color)//计算这个点的气
        {
            int liberty = 0;
            int i,j;
            for (i = 0; i < 19;i++)
            {
                for (j = 0; j < 19;j++)
                {
                    p[i, j] = false;
                }
            }
            liberty = dfs(x, y, color, liberty);
            return liberty;
        }
        private int dfs(int x,int y,int color,int liberty)//计算一个点的气的函数
        {
            p[x, y] = true;
            if (x != 0)
            {
                if (p[x - 1,y] == false)
                {
                    if (unit[x - 1, y].color == -1)
                    {
                        liberty = liberty + 1;
                        p[x - 1,y] = true;
                    }
                    if (unit[x - 1,y].color == color)
                    {
                        liberty = dfs(x - 1,y,color,liberty);
                    }
                }
            }
            if (x != 18)
            {
                if (p[x + 1, y] == false)
                {
                    if (unit[x + 1, y].color == -1)
                    {
                        liberty = liberty + 1;
                        p[x + 1, y] = true;
                    }
                    if (unit[x + 1, y].color == color)
                    {
                        liberty = dfs(x + 1, y, color, liberty);
                    }
                }
            }
            if (y != 0)
            {
                if (p[x, y - 1] == false)
                {
                    if (unit[x, y - 1].color == -1)
                    {
                        liberty = liberty + 1;
                        p[x, y - 1] = true;
                    }
                    if (unit[x, y - 1].color == color)
                    {
                        liberty = dfs(x, y - 1, color, liberty);
                    }
                }
            }
            if (y != 18)
            {
                if (p[x, y + 1] == false)
                {
                    if (unit[x, y + 1].color == -1)
                    {
                        liberty = liberty + 1;
                        p[x, y + 1] = true;
                    }
                    if (unit[x, y + 1].color == color)
                    {
                        liberty = dfs(x, y + 1, color, liberty);
                    }
                }
            }
            return liberty;
        }
        private void dfs(int x, int y, int color)//吃掉这片棋的函数
        {
            p[x, y] = true;
            pictureBox[x, y,color].Hide();
            unit[x, y].color = -1;
            if (x != 0)
            {
                if (p[x - 1, y] == false)
                {
                    if (unit[x - 1, y].color == color)
                    {
                        dfs(x - 1, y, color);
                    }
                }
            }
            if (x != 18)
            {
                if (p[x + 1, y] == false)
                {
                    if (unit[x + 1, y].color == color)
                    {
                        dfs(x + 1, y, color);
                    }
                }
            }
            if (y != 0)
            {
                if (p[x, y - 1] == false)
                {
                    if (unit[x, y - 1].color == color)
                    {
                        dfs(x, y - 1, color);
                    }
                }
            }
            if (y != 18)
            {
                if (p[x, y + 1] == false)
                {
                    if (unit[x, y + 1].color == color)
                    {
                        dfs(x, y + 1, color);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//PASS
        {

        }

        private void button2_Click(object sender, EventArgs e)//悔棋
        {

        }

        private void button3_Click(object sender, EventArgs e)//求和
        {

        }

        private void button4_Click(object sender, EventArgs e)//点目
        {

        }

        private void button5_Click(object sender, EventArgs e)//聊天
        {
            String s;
            if (isonline == 0)
            {
                MessageBox.Show("单机模式已禁用聊天功能！");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("聊天内容为空，无法发送！");
                return;
            }
            s = "5" + textBox2.Text;
            clientmanager.get_from_board(s,friendname);//向服务器发送信息。
        }

        private void textBox2_TextChanged(object sender, EventArgs e)//聊天内容
        {

        }


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
///                                                               Hurray's Part
///
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //获取好友列表（只发送一次请求，返回好友列表）
        public string[] GetFriendList(Form mainform)
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_FIND_FRIEND;
                sendMessage(JsonConvert.SerializeObject(sendBundle));

                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                if (receiveBundle.type == R.CMD_FIND_FRIEND)
                {
                    return receiveBundle.allOnlineName;
                }
                else
                {
                    showNote(R.NOTE_WRONG_PAKAGE);
                    return null;
                }
            }
            catch (Exception ex)
            {
                showNote(R.NOTE_SERVER_UNCONNECT);
                return null;//出错的话返回null
            }
        }

        // 建立对战 如果成功则返回true，否则返回false并跳出提示框
        public bool ApplyFight(string friendname)
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_APPLY_FIGHT;
                sendBundle.username = username;
                sendBundle.friendname = friendname;
                sendMessage(JsonConvert.SerializeObject(sendBundle));

                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                if (receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.type == R.CMD_APPLY_FIGHT)
                    return true;
                else if (receiveBundle.type != R.CMD_APPLY_FIGHT)
                {
                    showNote(R.NOTE_WRONG_PAKAGE);
                    return false;
                }
                else
                {
                    showNote(R.NOTE_CANNOT_FIGHT);
                    return false;
                }
            }
            catch (Exception ex)
            {
                showNote(R.NOTE_SERVER_UNCONNECT);
                return false;
            }
        }

        // 等待并获取对方落子
        public string WaitFriendLuoZi()
        {
            try
            {
                String msg = receiveMsg();
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                if (receiveBundle.type == R.CMD_FIGHT && receiveBundle.friendname.Equals(friendname))
                {
                    return receiveBundle.fightInfo;
                }
                else
                {
                    showNote(R.NOTE_WRONG_PAKAGE);
                    return null;
                }
            }
            catch (Exception ex)
            {
                showNote(R.NOTE_SERVER_UNCONNECT);
                return null;
            }
        }

        // 自己下棋传送给服务器，成功则返回true，否则返回flase并提示错误
        public bool SendFightinfoToServer(String fightInfo)
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_FIGHT;
                sendBundle.username = username;
                sendBundle.friendname = friendname;
                sendBundle.fightInfo = fightInfo;
                sendMessage(JsonConvert.SerializeObject(sendBundle));
                return true;
            }
            catch (Exception ex)
            {
                showNote(R.NOTE_SERVER_UNCONNECT);
                return false;
            }
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

    public class Unit
    {
        public int color;//-1表示空，0表示黑棋，1表示白棋
        //public int liberty;//气
        int x;
        int y;

        public Unit(int i, int j)
        {
            color = -1;
            //liberty = -1;
            x = i;
            y = j;
        }
    }
}
