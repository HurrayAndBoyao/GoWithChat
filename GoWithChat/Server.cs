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
    public partial class Server : Form
    {

        public Server()
        {
            InitializeComponent();
        }

        

        public ServerManager servermanager;

        public void bt_start_Click(object sender, EventArgs e)
        {
            servermanager = new ServerManager(int.Parse(tb_port.Text), tb_output);
        }
    }
}
