using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace GoWithChat
{
    public class ClientManager
    {
        public TcpClient tcpClient;
        public NetworkStream stream;

        public bool Landed(String username, String passwd, Form landedForm)
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
                    //登录成功
                    MainForm mainform = new MainForm(this);
                    mainform.Show();
                    landedForm.Hide();
                }
                else
                {
                    String note;
                    switch(receiveBundle.note)
                    {
                        case R.NOTE_ERROR_CODE: 
                            note = R.MSG_ERROR_CODE; break;

                        case R.NOTE_SERVER_ERROR:
                            note = R.MSG_SERVER_ERROR; break;

                        case R.NOTE_SAMENAME:
                            note = R.MSG_SAMENAME; break;

                        default:
                            note = R.MSG_UNKNOW_ERROR; break;

                    }
                    new Note(note).Show();
                }
            }
            catch (Exception e)
            {
                new Note(R.MSG_SERVER_UNCONNECT).Show();
            }

            return false;
        }

        public string[] getFriendList()
        {
            MsgBundle sendBundle = new MsgBundle();
            sendBundle.type = R.CMD_FIND_FRIEND;
            sendMessage(JsonConvert.SerializeObject(sendBundle));

            String msg = receiveMsg();
            MsgBundle receiveBundle = JsonConvert.DeserializeObject<MsgBundle>(msg);
            if (receiveBundle.type == R.CMD_FIND_FRIEND && receiveBundle.status == R.STATUS_SUCCESS && receiveBundle.allOnlineName != null)
            {
                return receiveBundle.allOnlineName;
            }
            else
                return null;
        }

        public string receiveMsg()
        {
            Byte[] data = new Byte[256];
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
