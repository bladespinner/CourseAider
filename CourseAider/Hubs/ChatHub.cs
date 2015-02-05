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
        private static Dictionary<string, ChatContext> contexts = new Dictionary<string, ChatContext>();
        private static Dictionary<string, Question> questions = new Dictionary<string, Question>();
        public override System.Threading.Tasks.Task OnConnected()
        {
            string name = this.Context.User.Identity.Name;
            var ircClient = new IrcClient();
            UserProfile profile;
            using(CourseAiderContext context = new CourseAiderContext())
            {
                profile = context.UserProfiles.FirstOrDefault(prof => prof.UserName == this.Context.User.Identity.Name);
                Clients.Caller.notify("Transfering credentials...");
            }
            var chatContext = new ChatContext()
            {
                ChatClient = ircClient,
                IsTeacher = profile.IsTeacher,
                UserId = profile.UserId,
                UserName = profile.UserName
            };
            contexts.Add(name, chatContext);
            string message = "Hello ," + name;
            Clients.Caller.notify(message);
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string name = this.Context.User.Identity.Name;
            var context = contexts[name];
            context.ChatClient.Disconnect();
            contexts.Remove(name);
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
            
            ChatContext ircContext;
            if (contexts.TryGetValue(this.Context.User.Identity.Name, out ircContext))
            {
                IrcClient ircClient = ircContext.ChatClient;
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
                        string error = e.Message;
                        if(e.Parameters != null && e.Parameters.Count > 0)
                        {
                            error += ", params:" + e.Parameters.Aggregate((a, b) => a + "," + b);
                        }
                        Clients.Caller.error("Protocol:" + error);
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
            }
        }

        public void BecomeOperator()
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            if (!context.IsTeacher) return;

            var channel = context.ChatClient.LocalUser.GetChannelUsers().First().Channel;

            channel.SetModes("+o", name);

            channel.ModesChanged += (object sender, EventArgs a) =>
            {
                Clients.All.notify("Group operator " + name + " has joined");
            };
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

        public void ToggleMode(string mode,string message,string targets)
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            if (!context.IsTeacher) return;

            var channel = context.ChatClient.LocalUser.GetChannelUsers().First().Channel;
            var modes = channel.Modes.ToList();
            string setting;
            if(modes.Contains(mode[0]))
            {
                modes.Remove(mode[0]);
                channel.SetModes(modes.ToArray());
                setting = "off";
            }
            else
            {
                channel.SetModes("+"+modes[0],targets.Split(',').Select(a => a.Trim()));
                setting = "on";
            }

            channel.ModesChanged += (object sender, EventArgs a) =>
            {
                Clients.All.notify(String.Format(message,setting));
            };
        }

        public void GetModes(string target)
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            if (!context.IsTeacher) return;

            var modes = context
                .ChatClient
                .LocalUser
                .GetChannelUsers()
                .First()
                .Channel
                .Users
                .Where(u => u.User.UserName == target)
                .Select(u => u.Modes
                    .Select(m => m.ToString())
                    .Aggregate((a, b) => a.ToString() + "," + b.ToString()));

            Clients.Caller.userModes(modes, target);
        }

        public void Kick(string user)
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            if (!context.IsTeacher) return;

            var channel = context.ChatClient.LocalUser.GetChannelUsers().First().Channel;

            Clients.All.notify(user + "has been kicked from chat.");

            channel.Kick(user);
        }

        public void List()
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            var chanuser = context.ChatClient.LocalUser.GetChannelUsers().FirstOrDefault();
            if (chanuser == null) return;
            var channel = chanuser.Channel;
            var users = channel.Users.GetUsers();
         
            Clients.Caller.listUpdate(users.Select(u => u.NickName).Aggregate((a,b) => a + "," + b));
        }

        public void AskQuestion(string question, int points, int lifetime, string[] answers, int correct)
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];

            if (!context.IsTeacher) return;

            Question q = new Question(answers, lifetime, correct, points);
            questions.Add(q.Id, q);
            Clients.Others.askQuestion(question, points, answers, q.ExpirationTime);
        }

        public void AnswerQuestion(string questionId, int answer)
        {
            if (!this.Context.User.Identity.IsAuthenticated) return;

            questions = questions.Where(q => q.Value.ExpirationTime > DateTime.Now).ToDictionary(a => a.Key, a => a.Value);
            if(questions.ContainsKey(questionId))
            {
                var question = questions[questionId];
                if(question.Answerers.Contains(this.Context.User.Identity.Name))
                {
                    return;
                }
                question.Answerers.Add(this.Context.User.Identity.Name);
                if(answer == question.CorrectAnswer)
                {
                    using(var db = new CourseAiderContext())
                    {
                        var user = db.UserProfiles.FirstOrDefault(a => a.UserName == this.Context.User.Identity.Name);
                        if (user == null) return;
                        user.Score += question.Points;
                        db.SaveChanges();
                    }
                    Clients.Caller.notify("You have answered correctly");
                }
                else
                {
                    Clients.Caller.notify("You have answered incorrectly , the correct answer was '"
                        + question.Answers[question.CorrectAnswer] + "'.");
                }
            }
        }

        public void Send(string message)
        {
            if(!this.Context.User.Identity.IsAuthenticated) return;

            string name = this.Context.User.Identity.Name;
            var context = contexts[name];
            context.ChatClient.LocalUser.SendMessage(context.ChatClient.LocalUser.GetChannelUsers().First().Channel, message);
        }
    }
}