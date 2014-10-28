using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace GoWithChat
{
    class ClientManager
    {
        public TcpClient tcpClient;
        public NetworkStream stream;

        public bool Landed(String username, String passwd, Form landedForm)
        {
            this.tcpClient = new TcpClient();
            tcpClient.Connect(R.IPADDRESS , R.PORT);
            this.stream = tcpClient.GetStream();

            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_LOGIN;
            sendBundle.username = username;
            sendBundle.passwd = passwd;
            sendMessage(JsonConvert.SerializeObject(sendBundle));

            String msg = receiveMsg();
            MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
            if (receiveBundle.type == R.CMD_LOGIN && receiveBundle.status == R.STATUS_SUCCESS)
            {
                //登录成功
                MainForm mainform = new MainForm();
                mainform.Show();
                landedForm.Hide();
            }
            else
            {
                Note newnote = new Note(R.NOTE_ERROR_CODE);
                newnote.Show();
            }

            return false;
        }

        public string receiveMsg()
        {
            Byte[] data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //tb_output.AppendText(responseData + "!\n");
            return responseData;
        }

        public void sendMessage(String msg)
        {

            Byte[] bytes = Encoding.UTF8.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);

        }

        public void showNote(String note)
        {
            Note newnote = new Note(note);
            newnote.Show();

        }
    }
}
