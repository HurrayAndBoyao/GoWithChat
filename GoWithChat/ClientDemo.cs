using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace GoWithChat
{
    public partial class ClientDemo : Form
    {
        public TcpClient tcpClient;
        public NetworkStream stream;

        public ClientDemo()
        {
            InitializeComponent();
        }

        public void printToConsole(String msg)
        {
            tb_output.AppendText(msg + "\n");        
        }

        private void bt_connect_Click(object sender, EventArgs e)
        {
            MsgBundle bundle = new MsgBundle();
            bundle.username = "Hurray";
            bundle.passwd = "123";
            //bundle.FightMsg = "haha";
            
            string getJson = JsonConvert.SerializeObject(bundle);
            
            this.tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(tb_ip.Text),Int32.Parse(tb_port.Text));
            this.stream = tcpClient.GetStream();

            sendMessage(getJson);
            receiveMsg();
            
            

            
            
            
        }

        public void receiveMsg()
        {
            Byte[] data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            tb_output.AppendText(responseData + "!\n");
        }

        public void sendMessage(String msg)
        {

            Byte[] bytes = Encoding.UTF8.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
            
        }
    }
}
