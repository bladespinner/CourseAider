using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using IrcDotNet;
using System.Configuration;
using CourseAider.Chat;
using CourseAider.Models;

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
            string message = "Hello ," + name;
            Clients.Caller.notify(message);
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string name = this.Context.User.Identity.Name;
            var client = clients[name];
            client.Disconnect();
            clients.Remove(name);
            return base.OnDisconnected();
        }

        public void Connect(string channel)
        {
            if(!this.Context.User.Identity.IsAuthenticated) return;
            string password;
            UserProfile prof;
            using(CourseAiderContext context = new CourseAiderContext())
            {
                prof = context.UserProfiles.FirstOrDefault(profile => profile.UserName == this.Context.User.Identity.Name);
                password = prof.IrcCredential;
                Clients.Caller.notify("Transfering credentials...");
            }
            
            IrcClient ircClient;
            if(clients.TryGetValue(this.Context.User.Identity.Name,out ircClient))
            {
                Action closure = () =>
                {
                    string user = this.Context.User.Identity.Name;
                    string pass = password;
                    string chan = channel;
                    var caller = Clients.Caller;
                    IrcClient client = ircClient;
                    ircClient.Registered += (object sender, EventArgs e) =>
                    {
                        Clients.Caller.notify("Joining chat channel...");
                        client.Channels.Join("#" + chan);
                        RegisterClientEvents(client);
                    };
                    ircClient.Error += (object sender, IrcErrorEventArgs e) =>
                    {
                        Clients.Caller.error("Web Server:" + e.Error.Message + e.Error.StackTrace);
                    };
                    ircClient.ErrorMessageReceived += (object sender, IrcErrorMessageEventArgs e) =>
                    {
                        Clients.Caller.error("IRC Server:" + e.Message);
                    };
                    ircClient.ProtocolError += (object sender, IrcProtocolErrorEventArgs e) =>
                    {
                        Clients.Caller.error("Protocol:" + e.Message + ", params:" + e.Parameters.Aggregate((a, b) => a + "," + b));
                    };
                };
                closure();

                Clients.Caller.notify("Attempting connection...");
                ircClient.Connect(new Uri(Configuration.IrcServerUri), new IrcUserRegistrationInfo()
                {
                    NickName = prof.UserName,
                    UserName = prof.UserName,
                    RealName = prof.RealName,
                    Password = password,
                });
                
                /*ircClient.Connect(new Uri(Configuration.IrcServerUri),
                    new CourseAiderIrcRegistrationInfo(this.Context.User.Identity.Name, password));*/

                //ircClient.IsRegistered
                //ircClient.Connect();
            }
        }

        public void BecomeOperator(string password)
        {
            IrcClient ircClient;
            if (clients.TryGetValue(this.Context.User.Identity.Name, out ircClient))
            {
            }
        }

        private void RegisterClientEvents(IrcClient client)
        {
            client.LocalUser.JoinedChannel += (object sender, IrcChannelEventArgs ie) =>
            {
                var channel = ie.Channel;
                var user = client.LocalUser;
                var caller = Clients.Caller;
                caller.notify("Connection established, you can now start talking");
                channel.MessageReceived += (object s, IrcMessageEventArgs e) =>
                {
                    caller.message(e.Source.Name, e.Text);
                };
                user.MessageReceived += (object s, IrcMessageEventArgs e) =>
                {
                    caller.privateMessage(e.Source.Name, e.Text);
                };
            };
        }

        public void Send(string message)
        {
            if(!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var client = clients[name];
            client.LocalUser.SendMessage(client.LocalUser.GetChannelUsers().First().Channel, message);
        }
    }
}