using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAider.Chat
{
    public class CourseAiderIrcRegistrationInfo : IrcRegistrationInfo
    {
        public CourseAiderIrcRegistrationInfo(string username, string password)
        {
            Password = password;
        }
    }
}