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

        private void Landed_Load(object sender, EventArgs e)
        {

        }

        private void bt_landed_Click(object sender, EventArgs e)
        {
            if (tb_username.Text != null && tb_passwd.Text != null)
            {
                Form fm_Server = new Server();
                fm_Server.Show();
            }
            else
            {
 
            }
        }
    }
}
