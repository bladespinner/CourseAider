using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CourseAider.Filters;

namespace CourseAider.Models
{
    public class FileUploadModel
    {
        [Required]
        [File("{0} must be an {1}, and be smaller than {2}",
            FileSize = 1024 * 1024 * 40
        )]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public bool Private { get; set; }
    }
}