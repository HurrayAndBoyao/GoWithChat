﻿using System;
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
        public const int MAX_BUFFER_NUM = 9999;
        public const int BLACK = 1;
        public const int WIGHT = 0;
        public const int UPDATE_TIME = 5000;//刷新好友的时间间隔（毫秒）

        //type
        public const int CMD_LOGIN = 1001;//登录
        public const int CMD_LOGOUT = 1002;//注销
        public const int CMD_FIND_FRIEND = 1003;//查找当前在线好友
        public const int CMD_APPLY_FIGHT = 1004;//申请比赛
        public const int CMD_AGREE_FIGHT = 1005;//（是否）同意比赛
        public const int CMD_FIGHT = 1006; //比赛信息传递
        public const int CMD_FIGHT_CANCLE = 1007; //比赛取消或结束
        public const int CMD_QUIT_FIGHT = 1008; //退出比赛

        //status
        public const int STATUS_SUCCESS = 1;
        public const int STATUS_FAILED = -1;

        //note
        public const string NOTE_SAMENAME = "已经有相同的用户名登陆！请重试其他用户名！";
        public const string NOTE_SERVER_ERROR = "服务器错误！";
        public const string NOTE_ERROR_CODE = "密码错误（当前设置用户名和密码相同即可登录）";
        public const string NOTE_BLANK_CODEORPASSWD = "还没有输入用户名或密码！";
        public const string NOTE_SERVER_UNCONNECT = "无法连接服务器！";
        public const string NOTE_UNKNOW_ERROR = "未知错误！";
        public const string NOTE_ALLREADY_START = "已经建立对战了，请勿重复操作！";
        public const string NOTE_FRIEND_NOT_ONLINE = "好友已经离线！";
        public const string NOTE_FRIEND_NOT_ONLINE_FIGHT_END = "好友已经离线！比赛结束！";
        public const string NOTE_SELF_FIGHT = "不能和自己对战！";
        public const string NOTE_ALREADY_FIGHT = "您已经和该好友在对战中了！";
        public const string NOTE_CANNOT_FIGHT = "不能与其对战（可能对方已经进入战斗）";
        public const string NOTE_WRONG_PAKAGE = "收到错误类型的包";
        public const string NOTE_SOMEONE_ALREADY_FIGHT = "对方已经开始对战";//自己对战的时候就不要发过来了= =
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
        public string friendname { get; set; }
        public int isBlack { get; set; }
    }
}
