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
    public partial class MainForm : Form
    {
        public ClientManager clientManager;
        public String[] friendList;
        public delegate void Setlabels(String [] friends);//博耀编辑，委托
        public MainForm(ClientManager clientManager)
        {
            this.clientManager = clientManager;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            clientManager.closeClient();
            Application.ExitThread();
            Application.Exit();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
