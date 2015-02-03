using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CourseAider.Chat
{
    public class IrcConnection
    {
        public IrcConnection(string name, string password,string channel)
        {
            var server = WebConfigurationManager.AppSettings.Get("CourseAider.IRC.Server");
            IrcClient client = new IrcDotNet.IrcClient();
            client.Connect(new Uri(server), new CourseAiderIrcRegistrationInfo("testusername123555s","testtesttesttest"));
            client.Channels.Join(channel);
        }
    }
}