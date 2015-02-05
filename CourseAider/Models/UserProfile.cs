using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public bool IsTeacher { get; set; }
        public string IrcCredential { get; set; }
        public int Rating { get; set; }
        public virtual IList<Course> Courses { get; set; }
        public virtual IList<Group> Groups { get; set; }
    }
}