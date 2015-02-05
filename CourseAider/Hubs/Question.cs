using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAider.Hubs
{
    public class Question
    {
        public Question(string[] answers, int lifetime,int correct, int points)
        {
            _id = Guid.NewGuid();
            Answerers = new List<string>();
            Answers = new List<string>();
            Answers.AddRange(answers);
            ExpirationTime = DateTime.Now.AddMinutes(lifetime);
            CorrectAnswer = correct;
            Points = points;
        }

        public int Points
        {
            get;
            private set;
        }

        private Guid _id;

        public int CorrectAnswer
        {
            get;
            private set;
        }

        public string Id
        {
            get
            {
                return _id.ToString();
            }
        }

        public DateTime ExpirationTime
        {
            get;
            set;
        }

        public List<string> Answerers { get; set; }
        public List<string> Answers { get; set; }
    }
}