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
        ClientManager clientmanager;

        public Landed()
        {
            InitializeComponent();
        }

        public void Landed_Load(object sender, EventArgs e)
        {
            clientmanager = new ClientManager();
        }

       public void bt_landed_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_username.Text) && !string.IsNullOrEmpty(tb_passwd.Text))
            {
                
                clientmanager.Landed(tb_username.Text, tb_passwd.Text, this);//登录成功的话登录页面自动隐藏
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
            Board board = new Board(0,0,null);
            board.Show();
        }
    }
}
