using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace GoWithChat
{
    public partial class ClientDemo : Form
    {
        public ClientDemo()
        {
            InitializeComponent();
        }

        private void bt_connect_Click(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(tb_ip.Text),Int32.Parse(tb_port.Text));
            NetworkStream stream = tcpClient.GetStream();

            

            Byte[] bytes = Encoding.UTF8.GetBytes("zzzzaaaa");
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
