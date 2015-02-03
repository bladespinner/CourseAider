using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using IrcDotNet;

namespace CourseAider.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static Dictionary<string, IrcClient> clients = new Dictionary<string, IrcClient>();
        public override System.Threading.Tasks.Task OnConnected()
        {
            string name = this.Context.User.Identity.Name;
            var ircClient = new IrcClient();
            clients.Add(name, ircClient);
            return base.OnConnected();
            string message = "Hello ," + name + ", please authenticate for the chat server";
            Clients.Caller.notify("Hello Please authenticate again");
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string name = this.Context.User.Identity.Name;
            IrcClient dummy;
            //clients.TryRemove(name, out dummy);
            return base.OnDisconnected();
        }

        public void Connect(string password,string channel)
        {
            IrcClient ircClient;
            if(clients.TryGetValue(this.Context.User.Identity.Name,out ircClient))
            {
                //ircClient.Connect();
            }
        }
        public void Send(string message)
        {
            string name = this.Context.User.Identity.Name;
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}