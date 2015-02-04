using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAider.Chat
{
    public class ChatContext
    {
        public IrcClient ChatClient { get; set; }
        public bool IsTeacher { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}