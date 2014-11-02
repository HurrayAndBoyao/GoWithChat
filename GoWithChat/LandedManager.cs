using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections; //使用Hashtable时，必须引入这个命名空间 
using System.Drawing;
using System.Runtime.InteropServices;

namespace GoWithChat
{
    public class LandedManager
    {
        public TcpClient tcpClient;
        public NetworkStream stream;

        public LandedManager()
        {}

        public bool Landed(string username, string passwd)
        {
            if (!username.Equals("") && !username.Equals(""))
            {
                try
                {
                    this.tcpClient = new TcpClient();
                    tcpClient.Connect(R.IPADDRESS, R.PORT);
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
                        return true;
                    }
                    else
                    {
                        showNote(receiveBundle.note);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    showNote(ex.ToString());
                    return false;
                }
            }
            else
            {
                showNote(R.NOTE_BLANK_CODEORPASSWD);
                return false;
            }
        }

        public string receiveMsg()
        {
            Byte[] data = new Byte[R.MAX_BUFFER_NUM];
            Int32 bytes = stream.Read(data, 0, data.Length);
            String responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
            //tb_output.AppendText(responseData + "!\n");
            return responseData;
        }

        public void sendMessage(String msg)
        {
            Byte[] bytes = Encoding.Unicode.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void showNote(String note)
        {
            Note newnote = new Note(note);
            newnote.Show();
        }
    }
}
