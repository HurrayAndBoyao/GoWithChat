using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoServer
{
    public partial class Server : Form
    {

        public Server()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Server_FormClosing);
        }

        private void Server_FormClosing(Object sender, FormClosingEventArgs e)
        {
            servermanager.ExitServer();
            Application.ExitThread();
        }

        public ServerManager servermanager;

        public void bt_start_Click(object sender, EventArgs e)
        {
            try
            {
                servermanager = new ServerManager(int.Parse(tb_port.Text), tb_output);
            }
            catch (Exception ex)
            {
                servermanager = new ServerManager(R.PORT, tb_output);
            }
        }

        private void bt_exitserver_Click(object sender, EventArgs e)
        {
            servermanager.ExitServer(); 
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }
    }
}
