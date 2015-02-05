using CourseAider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CourseAider.Controllers
{
    public class FileRatingController : ApiController
    {
        // POST api/filerating
        [Authorize(Roles="Teacher")]
        [HttpPost]
        public string Post(int id)
        {
            using(var a = new CourseAiderContext())
            {
                var file = a.Files.Find(id);
                if (file != null)
                {
                    if (file.Uploader.Score == 0)
                    {
                        file.Score += 1;
                        file.Uploader.Score += 1;
                        a.SaveChanges();
                    }
                }
            }
            return "";
        }
    }
}
