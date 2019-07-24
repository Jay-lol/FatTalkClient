﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
namespace WpfApp1.Modules
{
    public class MessengerClient :Tcpclient
    {
        public Action<TCPmessage> domessage { get; set; }

        public bool requestLogin(string id,string password)
        {
            TCPmessage message = new TCPmessage();
            JsonHelp json = new JsonHelp();
            message.Command = Command.login;
            message.message = json.logininfo(id, password);
            return Send(message);
        }

        public bool requestIdcheck(string id)
        {
            TCPmessage message = new TCPmessage();
            JsonHelp json = new JsonHelp();
            message.Command = Command.Idcheck;
            message.message = json.idcheckinfo(id);
            return Send(message);
        }
        public bool requestJoin(string id,string password,string nickname,string phone)
        {
            TCPmessage message = new TCPmessage();
            JsonHelp json = new JsonHelp();
            message.Command = Command.Join;
            message.message = json.joininfo(id, password, nickname, phone);
            return Send(message);
        }
        public bool requestFindid(string id)
        {
            TCPmessage message = new TCPmessage();
            JsonHelp json = new JsonHelp();
            message.Command = Command.Findid;
            message.message = json.idcheckinfo(id);
            return Send(message);
        }
        public bool requestLogout()
        {
            TCPmessage message = new TCPmessage();
            JsonHelp json = new JsonHelp();
            message.Command = Command.logout;
            return Send(message);
        }
        public override void ResponseMessage(TCPmessage message)
        {
            if (domessage != null)
            {
                domessage(message);
            }
        }
    }
}
