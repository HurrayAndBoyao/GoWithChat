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
        public
        Unit[,] unit = new Unit[19, 19];//单元格数组
        static int step = 0;//表示当前步数
        Image img_black = new Bitmap("../../resource/img/black.png");
        Image img_white = new Bitmap("../../resource/img/white.png");

        public Board()
        {
            InitializeComponent();
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
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    unit[i, j] = new Unit(i, j, g);
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

            p = PointToGrid(e.X, e.Y);

            if ((p.X >= 0) && (p.X <= 18) && (p.Y >= 0) && (p.Y <= 18))
            {
                if (unit[p.X, p.Y].color == -1) go(p.X, p.Y);
                step = step + 1;
            }
        }
        private void go(int x, int y)
        {
            int i;
            i = step % 2;
            unit[x, y].color = i;
            if (i == 0)
            {
                pictureBox[x, y].Image = img_black;
            }
            else
            {
                pictureBox[x, y].Image = img_white;
            }
            pictureBox[x, y].Show();
        }
    }

    public class Unit
    {
        public
        int color;//-1表示空，0表示黑棋，1表示白棋
        int liberty;//气
        int x;
        int y;

        public Unit(int i, int j, Graphics g)
        {
            color = -1;
            liberty = -1;
            x = i;
            y = j;
        }
    }
}
