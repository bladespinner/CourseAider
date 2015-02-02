using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    [Table("Course")]
    public class Course
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public virtual IList<Group> Groups { get; set; }
        public virtual IList<UserProfile> Members { get; set; }
        public virtual UserProfile Creator { get; set; }
    }
}