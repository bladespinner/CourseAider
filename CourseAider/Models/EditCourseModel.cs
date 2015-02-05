using CourseAider.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    public class EditCourseModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }
    }
}