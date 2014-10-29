using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        public Board()
        {
            int i, j;
            InitializeComponent();
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
            Graphics g = e.Graphics;
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

            p = PointToGrid(e.X, e.Y);

            if ((p.X >= 0) && (p.X <= 18) && (p.Y >= 0) && (p.Y <= 18))
            {
                if (go(p.X, p.Y) == 1)//如果此子可落
                {
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
            if (color == 0)
            {
                pictureBox[x, y].Image = img_black;
            }
            if (color == 1)
            {
                pictureBox[x, y].Image = img_white;
            }
            pictureBox[x, y].Show();//显示这个棋子
            if ((caneat / 1000) == 1)//待会儿要修改此处，没有考虑到双吃的情况。
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
            pictureBox[x, y].Image = null;
            pictureBox[x, y].Hide();
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
