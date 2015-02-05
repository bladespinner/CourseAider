using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    [Table("Group")]
    public class Group
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string IRCChannel { get; set; }
        public string TwitterAccount { get; set; }
        public virtual Course Course { get; set; }
        public virtual IList<UserProfile> Members { get; set; }
        public virtual IList<File> Files { get; set; }
        public virtual UserProfile Creator { get; set; }
    }
}