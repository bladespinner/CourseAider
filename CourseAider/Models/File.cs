using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    public class File
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime UploadTime { get; set; }
        public virtual Group Group { get; set; }
        public virtual UserProfile Uploader { get; set; }
    }
}