using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoWithChat
{
    static class R
    {
        //res
        public const String IPADDRESS = "127.0.0.1";
        public const int PORT = 1212;

        //type
        public const int CMD_LOGIN = 1001;//登录
        public const int CMD_LOGOUT = 1002;//注销
        public const int CMD_FIND_FRIEND = 1003;//查找当前在线好友
        public const int CMD_APPLY_FIGHT = 1004;//申请比赛
        public const int CMD_AGREE_FIGHT = 1005;//（是否）同意比赛
        public const int CMD_FIGHT = 1006; //比赛信息传递

        //status
        public const int STATUS_SUCCESS = 1;
        public const int STATUS_FAILED = 0;

        //note
        public const string NOTE_SAMENAME = "已经有相同的用户名登陆！请重试其他用户名！";
        public const string NOTE_SERVER_ERROR = "服务器错误！";
        public const string NOTE_ERROR_CODE = "密码错误（当前设置用户名和密码相同即可登录）";
        public const string NOTE_BLANK_CODEORPASSWD = "还没有输入用户名或密码！";
    }

    class MsgBundle
    {
        public string username { get; set; }
        public string passwd { get; set; }
        public string fightInfo { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
        public string[] allOnlineName { get; set; }
        public string note { get; set; }
    }
}
