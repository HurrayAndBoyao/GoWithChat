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
        public Landed()
        {
            InitializeComponent();
        }

        public void Landed_Load(object sender, EventArgs e)
        {

        }

       public void bt_landed_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_username.Text) && !string.IsNullOrEmpty(tb_passwd.Text))
            {
                
            }
            else
            {
                
            }
        }

        public void bt_server_Click(object sender, EventArgs e)
        {
            Form fm_Server = new Server();
            fm_Server.Show();
            this.Hide();
        }
    }
}
