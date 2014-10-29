using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoServer
{
    static class R
    {
        //res
        public const int MAX_BUFFER_NUM = 9999;


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
        public const int NOTE_SAMENAME = 2001;//"已经有相同的用户名登陆！请重试其他用户名！";
        public const string MSG_SAMENAME = "已经有相同的用户名登陆！请重试其他用户名！";
        public const int NOTE_SERVER_ERROR = 2002;//"服务器错误！";
        public const string MSG_SERVER_ERROR = "服务器错误！";
        public const int NOTE_ERROR_CODE = 2003;//"密码错误（当前设置用户名和密码相同即可登录）";
        public const string MSG_ERROR_CODE = "密码错误（当前设置用户名和密码相同即可登录）";
        public const int NOTE_BLANK_CODEORPASSWD = 2004;//"还没有输入用户名或密码！";
        public const string MSG_BLANK_CODEORPASSWD = "还没有输入用户名或密码！";
        public const int NOTE_SERVER_UNCONNECT = 2005;//"无法连接服务器！";
        public const string MSG_SERVER_UNCONNECT = "无法连接服务器！";
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
        public int note { get; set; }
        public string friendname { get; set; }
    }
}
