using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CourseAider.Helpers
{
    public static class FileHelper
    {
        public static string SaveFile(HttpPostedFileBase file, string type, string id)
        {
            string fileName = file.FileName.Split('\\').Last();
            string filePath = HostingEnvironment.MapPath("~/UserData/" + type + "/" + id);
            if (!Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            var imgPath = filePath + "\\" + fileName;
            file.SaveAs(imgPath);

            string webPath = "/UserData/" + type + '/' + id + "/" + fileName;
            return webPath;
        }
    }
}