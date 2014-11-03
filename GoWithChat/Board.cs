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

        private Unit[,] unit = new Unit[19, 19];//单元格数组
        private static int step = 0;//表示当前步数
        private Image img_black = new Bitmap("../../resource/img/black.png");
        private Image img_white = new Bitmap("../../resource/img/white.png");
        private Boolean[,] p = new Boolean[19, 19];//应用与dfs的一个布尔数组
        public int isonline, color, isfight;

        public TcpClient tcpClient;
        public NetworkStream stream;
        public String username;//【需要构造函数传进来】
        public String friendname;
        public String[] friendlist;
        public Thread thread;
        public int apply_fight = 0;
        public int shumu = -1;
        public int[, ,] history = new int[19, 19, 400];
        public int[] step_x = new int[400];
        public int[] step_y = new int[400];
        public int black = 0;
        public int white = 0;
       // public 

        public String friendFightInfo;

        public Board(String username,TcpClient tcpClient,NetworkStream stream)
        {
            int i, j,k;
            this.username = username;
            this.Text = username;
            this.tcpClient = tcpClient;
            //tcpClient.Connect(R.IPADDRESS, R.PORT);
            this.stream = stream;

            InitializeComponent();
            //this.Show();
            this.SuspendLayout();
            this.radioButton1.Hide();
            this.radioButton2.Hide();
            this.radioButton3.Hide();
            this.radioButton4.Hide();
            this.radioButton5.Hide();
            this.radioButton6.Hide();
            this.radioButton7.Hide();
            this.radioButton8.Hide();
            this.radioButton9.Hide();
            this.button6.Hide();
            this.button7.Hide();
            this.button8.Hide();
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
                        this.pictureBox[i,j,k].MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpForPic);
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
            if (username == null)
            {
                isonline = 0;
                isfight = 1;
            }
            else
            {
                isonline = 1;
                isfight = 0;
                thread = new Thread(ListenFightApplyThread);
                thread.Start();
            }
            this.Show();
            if (isfight == 0)
            {
                ShowFriends();
            }
        }
        private void PaintHandler(Object sender, PaintEventArgs e)
        {
            int i, j;
            Graphics g = e.Graphics;

            if (step % 2 == 0)
            {
                textBox1.Text = "黑棋下棋";
            }
            else
            {
                textBox1.Text = "白棋下棋";
            }
            if (isfight == 0)
            {
                textBox1.Text = "等待对战";
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
        public void beginfight()
        {
            String s;
            isfight = 1;
            if (color == 0)
            {
                s = "执黑";
                this.richTextBox1.Text += "由 " + friendname + " 发起的对战开始！";
                this.richTextBox1.Text += username + " 执黑, " + friendname + " 执白。";
            }
            else
            {
                s = "执白";
                this.richTextBox1.Text += "由 " + username + " 发起的对战开始！";
                this.richTextBox1.Text += friendname + " 执黑, " + username + " 执白。";
            }
            this.Text = username + s;
            this.button6.Hide();
            this.button7.Hide();
            this.button8.Hide();
            this.radioButton1.Hide();
            this.radioButton2.Hide();
            this.radioButton3.Hide();
            this.radioButton4.Hide();
            this.radioButton5.Hide();
            this.radioButton6.Hide();
            this.radioButton7.Hide();
            this.radioButton8.Hide();
            this.radioButton9.Hide();
            if (color == 1)
            {
                this.Refresh();
                while (friendFightInfo == null)
                {
                    System.Threading.Thread.Sleep(1);
                }
                get_from_server(friendFightInfo);
                friendFightInfo = null;
            }
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
        private void MouseUpForPic(Object sender,MouseEventArgs e)
        {
            int x, y;
            //if (shumu == -1) return;
            PictureBox p = sender as PictureBox;
            //MessageBox.Show(unit[0,0].color.ToString() + "hehe");
            //MessageBox.Show(p.Name);
            x = p.Name[11] - '0';
            if (p.Name[12] != ']')
            {
                x = x * 10 + (p.Name[12] - '0');
                y = p.Name[15] - '0';
                if (p.Name[16] != ']')
                {
                    y = y * 10 + (p.Name[16] - '0');
                }
            } 
            else
            {
                y = p.Name[14] - '0';
                if (p.Name[15] != ']')
                {
                    y = y * 10 + (p.Name[15] - '0');
                }
            }
            x = x - 1;
            y = y - 1;
            //MessageBox.Show("" + x + " " + y);
             if (shumu == 0)
                {
                    //MessageBox.Show(unit[x, y].color.ToString());
                    if (unit[x,y].color == 0)
                    {
                        unit[x, y].color = 1;
                        pictureBox[x, y, 0].Hide();
                        pictureBox[x, y, 1].Show();
                    }
                    else
                    {
                        MessageBox.Show("请点击您自己的棋！");
                    }
                }
                else if (shumu == 1)
                {
                    if (unit[x, y].color == 1)
                    {
                        unit[x, y].color = 0;
                        pictureBox[x, y, 1].Hide();
                        pictureBox[x, y, 0].Show();
                    }
                    else
                    {
                        MessageBox.Show("请点击您自己的棋！");
                    }
                }
        }
        private void MouseUpHandler(Object sender, MouseEventArgs e)
        {
            Point p;
            String s;
            if (isfight == 0) return;

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
                if (shumu == 0)
                {
                        MessageBox.Show("请点击您自己的棋！");
                }
                else if (shumu == 1)
                {
                        MessageBox.Show("请点击您自己的棋！");
                }
                else
                {
                    if (go(p.X, p.Y) == 1)//如果此子可落
                    {
                        if (isonline == 1)
                        {
                            s = "0" + (p.X / 10) + (p.X % 10) + (p.Y / 10) + (p.Y % 10);
                            this.SendFightinfoToServer(s);
                            //MessageBox.Show();
                        }
                        step = step + 1;
                        if (isonline == 1)
                        {
                            this.Refresh();
                            while (friendFightInfo == null)
                            {
                                System.Threading.Thread.Sleep(1);
                            }
                            get_from_server(friendFightInfo);
                            friendFightInfo = null;
                        }
                    }
                }
            }
        }
        private int go(int x, int y)
        {
            int i,j;
            int color;
            int caneat;
            String s;
            if (unit[x, y].color != -1) return 0;//如果当前点不是空的，下不了
            color = step % 2;
            caneat = can_eat_other(x, y, color);
            if (caneat != 0)//如果下了能吃别人的棋
            {
                if (ko(x, y,color,caneat) == 1) return 0;//如果落子在劫内，下不了
            }
            else//如果下了不能吃别人的棋
            {
                if (dfs_compute(x, y, color) == 0) return 0;//如果自己下了就死，下不了
            }
            unit[x, y].color = color;
            pictureBox[x, y,color].Show();//显示这个棋子
            if (color == 0) s = "黑方"; else s = "白方";
            richTextBox1.Text += "\n" + s + "着子于[" + (x + 1) + "],[" + (y + 1) + "]";
            //this.Refresh();
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
            for (i = 0; i < 19;i++ )
            {
                for (j = 0;j < 19;j++)
                {
                    history[i, j, step] = unit[i, j].color;
                }
            }
            step_x[step] = x;
            step_y[step] = y;
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
        private int ko(int x,int y,int color,int caneat)//判断是否造成全局同型
        {
            int i, j,xx,yy;
            xx = 0;
            yy = 0;
            if (step < 3) return 0;
            if (x != 0)
            {
                if (unit[x-1,y].color != 1 - color)
                {
                    return 0;
                }
            }
            if (x != 18)
            {
                if (unit[x + 1, y].color != 1 - color)
                {
                    return 0;
                }
            }
            if (y != 0)
            {
                if (unit[x, y - 1].color != 1 - color)
                {
                    return 0;
                }
            }
            if (y != 18)
            {
                if (unit[x, y + 1].color != 1 - color)
                {
                    return 0;
                }
            }
            if (((caneat / 1000) + ((caneat / 100) % 10) + ((caneat / 10) % 10) + (caneat % 10)) != 1)
            {
                return 0;
            }
            if ((caneat / 1000) == 1)
            {
                xx = x - 1;
                yy = y;
            }
            if (((caneat / 100) % 10) == 1)
            {
                xx = x + 1;
                yy = y;
            }
            if (((caneat / 10) % 10) == 1)
            {
                xx = x;
                yy = y - 1;
            }
            if ((caneat % 10) == 1)
            {
                xx = x;
                yy = y + 1;
            }
            if ((step_x[step - 1] == xx) && (step_y[step - 1] == yy))
            {
                if (history[x, y, step - 2] == color) return 1;
            }
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
            int i, j;
            String s;
            if (step == 0)
            {
                MessageBox.Show("第一步棋不能PASS");
                return;
            }
            step_x[step] = -1;
            step_y[step] = -1;
            for (i = 0;i < 19;i++)
            {
                for (j = 0;j < 19;j++)
                {
                    history[i, j, step] = history[i, j, step - 1];
                }
            }
            if (step % 2 == 0) s= "黑方"; else s = "白方";
            richTextBox1.Text += "\n" + s + "PASS";
            step = step + 1;
        }

        private void button2_Click(object sender, EventArgs e)//悔棋
        {
            int i,j;
            String s;
            if (step < 4)
            {
                MessageBox.Show("才下这么几步就想悔棋嘛~");
                return;
            }
            step = step - 3;
            for (i = 0;i < 19;i++)
            {
                for (j = 0;j < 19;j++)
                {
                    if (history[i,j,step] == -1)
                    {
                        unit[i, j].color = -1;
                        pictureBox[i, j, 0].Hide();
                        pictureBox[i, j, 1].Hide();
                    }
                    if (history[i,j,step] == 0)
                    {
                        unit[i, j].color = 0;
                        pictureBox[i, j, 0].Show();
                        pictureBox[i, j, 1].Hide();
                    }
                    if (history[i,j,step] == 1)
                    {
                        unit[i, j].color = 1;
                        pictureBox[i, j, 0].Hide();
                        pictureBox[i, j, 1].Show();
                    }
                }
            }
            if (step % 2 == 0) s = "黑方"; else s = "白棋";
            richTextBox1.Text += "\n" + s + "悔棋";
            step = step + 1;
        }

        private void button3_Click(object sender, EventArgs e)//求和
        {
            MessageBox.Show("抱歉，此功能尚未开放！");
        }
        private int dfshehe(int x,int y,int color)
        {
            int haha = 0;
            p[x, y] = true;
            if (unit[x, y].color == color) haha++;
            if (unit[x, y].color != -1) return haha;
            if (x != 0)
            {
                if (p[x -1,y] == false)
                {
                    haha += dfshehe(x - 1,y,color);
                }
            }
            if (x != 18)
            {
                if (p[x + 1, y] == false)
                {
                    haha += dfshehe(x + 1, y, color);
                }
            }
            if (y != 0)
            {
                if (p[x, y - 1] == false)
                {
                    haha+= dfshehe(x, y - 1, color);
                }
            }
            if (y != 18)
            {
                if (p[x, y + 1] == false)
                {
                    haha+= dfshehe(x, y + 1, color);
                }
            }
            return haha;
        }
        public void dfsdianmu(int x,int y)
        {
            int i, j;
            int hehe,haha;
            if (unit[x, y].color == 0) black = black + 1;
            if (unit[x, y].color == 1) white = white + 1;
            if (unit[x, y].color == -1)
            {
                for (i = 0; i < 19; i++)
                {
                    for (j = 0; j < 19; j++)
                    {
                        p[i, j] = false;
                    }
                }
                hehe = dfshehe(x, y, 0);
                for (i = 0; i < 19; i++)
                {
                    for (j = 0; j < 19; j++)
                    {
                        p[i, j] = false;
                    }
                }
                haha = dfshehe(x,y,1);
                //MessageBox.Show(x + " " + y  + " " + hehe + " " + haha);
                if ((hehe != 0)&&(haha != 0)) return;
                if ((hehe == 0) && (haha == 0)) return;
                if (hehe != 0)
                {
                    black = black + 1;
                }
                if (haha != 0)
                {
                    white = white + 1;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)//点目
        {
            int i,j;
            int x, y;
            String s;
            if (isonline == 1)
            {
                MessageBox.Show("抱歉，目前联机模式无法点目");
                return;
            }
            if (shumu == 1)
            {
                //双方都已经确认局势
                for (i = 0; i < 19;i++ )
                {
                    for (j = 0;j < 19;j++)
                    {
                        dfsdianmu(i,j);
                    }
                }
                richTextBox1.Text += "\n黑棋目数：" + black + ",白棋目数：" + white;
                x = black;
                y = white + 4;
                if (x <= y)
                {
                    s = ",白胜" + (y - x) + "又3/4目";
                } else
                {
                    s = ",黑胜" + (x - y - 1) + "又1/4目";
                }
                MessageBox.Show("黑棋子数为：" + black + ",白棋子数为：" + white + s);
                return;
            }
            if (shumu == 0)
            {
                MessageBox.Show("请白方点掉自己的死棋（目前此功能无法逆转，请谨慎）");
                button4.Text = "白棋确认";
                shumu = 1;
                return;
            }
            button1.Hide();
            button2.Hide();
            button3.Hide();
            button5.Hide();
            MessageBox.Show("请黑方点掉自己的死棋（目前此功能无法逆转，请谨慎）");
            button4.Text = "黑棋确认";
            shumu = 0;
            //MessageBox.Show(unit[0, 0].color.ToString());
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
            MessageBox.Show("抱歉，此功能尚未开放！");
            s = "5" + textBox2.Text;
            //clientmanager.get_from_board(s,friendname);//向服务器发送信息。
        }

        private void textBox2_TextChanged(object sender, EventArgs e)//聊天内容
        {

        }
        public void ShowFriends()
        {
            int i;
            String[] friends;
            friends = friendlist;
            radioButton1.Hide();
            radioButton2.Hide();
            radioButton3.Hide();
            radioButton4.Hide();
            radioButton5.Hide();
            radioButton6.Hide();
            radioButton7.Hide();
            radioButton8.Hide();
            radioButton9.Hide();
            button6.Show();
            button7.Show();
            button8.Show();
            GetFriendList();
            //this.richTextBox1.Hide();
            System.Threading.Thread.Sleep(100);
            while (this.friendlist == null)
            {

            }
            friends = friendlist;
            //if (friends != null)
            //this.pictureBox[0, 0, 0].Show();
            //MessageBox.Show(friends[0]);
            //friends = new String [] {"1","2","3","4"};
            int j;
            for (i = 0,j = 1; i < friends.Length; i++, j++)
            {
                if (!friends[i].Equals(username))
                {
                    //MessageBox.Show(radioButton1.Text + "hehe");
                    if (j == 1)
                    {
                        radioButton1.Text = friends[i];
                        radioButton1.Show();
                    }
                    else if (j == 2)
                    {
                        radioButton2.Text = friends[i];
                        radioButton2.Show();
                    }
                    else if (j == 3)
                    {
                        radioButton3.Text = friends[i];
                        radioButton3.Show();
                    }
                    else if (j == 4)
                    {
                        radioButton4.Text = friends[i];
                        radioButton4.Show();
                    }
                    else if (j == 5)
                    {
                        radioButton5.Text = friends[i];
                        radioButton5.Show();
                    }
                    else if (j == 6)
                    {
                        radioButton6.Text = friends[i];
                        radioButton6.Show();
                    }
                    else if (j == 7)
                    {
                        radioButton7.Text = friends[i];
                        radioButton7.Show();
                    }
                    else if (j == 8)
                    {
                        radioButton8.Text = friends[i];
                        radioButton8.Show();
                    }
                    else if (j == 9)
                    {
                        radioButton9.Text = friends[i];
                        radioButton9.Show();
                    }
                }
                else
                {
                    j--;
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton1.Text;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton2.Text;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton3.Text;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton4.Text;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton5.Text;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton6.Text;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton7.Text;
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton8.Text;
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            friendname = this.radioButton9.Text;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            ShowFriends();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (friendname == null)
            {
                MessageBox.Show("对不起，您未选中任何好友");
            } else
            {
                ApplyFight(friendname);
                System.Threading.Thread.Sleep(100);
                while (apply_fight == 0)
                {
                }
                if (apply_fight == 1)
                {
                    beginfight();
                }
                else if (apply_fight == -1)
                {
                    MessageBox.Show("对战未开启！");
                }
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (apply_fight == 1)
            {
                beginfight();
            }
            else if (apply_fight == -1)
            {
                MessageBox.Show("对战未开启！");
            }
        }


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
///                                                               Hurray's Part
///
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //线程类：刚登录或退出对战以后打开此线程，接听是否有用户请求对战(返回对方用户名)
        public void ListenFightApplyThread()
        {
            while(true)
            {
                //showNote("testtest1");
                String msg = receiveMsg();
                //showNote(msg);
                MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
                if (receiveBundle.type == R.CMD_FIND_FRIEND)
                {
                    //return receiveBundle.allOnlineName;
                    this.friendlist = receiveBundle.allOnlineName;
                    //MessageBox.Show(friendlist[0]);
                }
                else if (receiveBundle.type == R.CMD_FIGHT && receiveBundle.friendname.Equals(friendname))
                {
                    friendFightInfo = receiveBundle.fightInfo;
                }
                else if (receiveBundle.type == R.CMD_APPLY_FIGHT)
                {
                    if (receiveBundle.status == R.STATUS_SUCCESS)
                    {
                        if (receiveBundle.isBlack == 0)
                        {
                            friendname = receiveBundle.friendname;
                            MessageBox.Show(friendname + "向您发送的对战请求，请点击接受对战！",username);
                        }
                        color = receiveBundle.isBlack;
                        apply_fight = 1;
                    }
                    else
                    {
                        showNote(R.NOTE_CANNOT_FIGHT);
                        apply_fight = -1;
                    }
                }
                else
                {
                    MessageBox.Show(R.NOTE_WRONG_PAKAGE);
                    //return null;
                }
            }
            //return null;
        }

        //获取好友列表（只发送一次请求，返回好友列表）
        public void GetFriendList()
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_FIND_FRIEND;
                sendMessage(JsonConvert.SerializeObject(sendBundle));
            }
            catch (Exception ex)
            {
                MessageBox.Show(R.NOTE_SERVER_UNCONNECT);
            }
        }

        // 建立对战 如果成功则返回true，否则返回false并跳出提示框
        public void ApplyFight(string friendname)
        {
            try
            {
                MsgBundle sendBundle = new MsgBundle();
                sendBundle.type = R.CMD_APPLY_FIGHT;
                sendBundle.username = username;
                sendBundle.friendname = friendname;

                sendMessage(JsonConvert.SerializeObject(sendBundle));
            }
            catch (Exception ex)
            {
                showNote(R.NOTE_SERVER_UNCONNECT);
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
            MessageBox.Show(note);
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
