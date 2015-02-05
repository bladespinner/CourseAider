using CourseAider.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAider.Models
{
    public class CreateCourseModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [File("{0} must be an {1}, and be smaller than {2}",
            FileSize = 1024 * 1024 * 20,
            FileTypes = new[] { "gif", "jpg", "jpeg", "png", "bmp", "tiff", "webm" }
        )]
        public HttpPostedFileBase Image { get; set; }

        public string Description { get; set; }
    }
}