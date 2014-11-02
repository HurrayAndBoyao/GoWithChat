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
    public partial class Landed : Form
    {
        LandedManager landedmanager;

        public Landed()
        {
            InitializeComponent();
        }

        public void Landed_Load(object sender, EventArgs e)
        {
            landedmanager = new LandedManager();
        }

       public void bt_landed_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_username.Text) && !string.IsNullOrEmpty(tb_passwd.Text))
            { 
                bool isLanded = landedmanager.Landed(tb_username.Text, tb_passwd.Text);//登录成功的话登录页面自动隐藏
                if (isLanded)
                {
                    //登录成功，建立board
                    Note newnote = new Note("登陆成功啦！");
                    newnote.Show();
                }
            }
            else
            {
                new Note(R.NOTE_BLANK_CODEORPASSWD).Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientDemo clientDemo = new ClientDemo();
            clientDemo.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Board board = new Board(0,0,null,null);
            board.Show();
        }
    }
}
