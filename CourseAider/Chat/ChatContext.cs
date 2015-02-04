using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAider.Chat
{
    public class ChatContext
    {
        IrcClient ChatClient { get; set; }

    }
}