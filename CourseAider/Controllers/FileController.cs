using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CourseAider.Models;
using WebMatrix.WebData;
using CourseAider.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;
using CourseAider.DataStreamFormatters;

namespace CourseAider.Controllers
{
    public class FileController : ApiController
    {
        private CourseAiderContext db = new CourseAiderContext();

        // GET api/File
        public IEnumerable<CourseAider.Models.File> GetFiles(int id)
        {
            var group = db.Groups.Find(id);
            if(group == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return group.Files.AsEnumerable();
        }

        // GET api/File/5
        public CourseAider.Models.File GetFile(int id)
        {
            CourseAider.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return file;
        }

        // PUT api/File/5
        public HttpResponseMessage PutFile(int id, CourseAider.Models.File file)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != file.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(file).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Post(int id)
        {
            var result = new List<string>();
            //throw new Exception("Custom error thrown for script error handling test!");  
            var files = System.Web.HttpContext.Current.Request.Files;
            for (int i = 0; i < files.Count;i++ )
            {
                var file = db.Files.Create();
                var group = db.Groups.Find(id);
                if (group == null)
                {
                    ModelState.AddModelError("File", new ArgumentException("No group with id:" + id));
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                file.Group = db.Groups.Find(id);
                var uploader = db.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);
                if (uploader == null)
                {
                    ModelState.AddModelError("File", new ArgumentException("Invalid uploader"));
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                file.Uploader = uploader;
                file.Name = files[i].FileName;
                file.UploadTime = DateTime.Now;
                file.Visibility = System.Web.HttpContext.Current.Request.Form["visible"] == "true";
                db.Files.Add(file);
                db.SaveChanges();

                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/UserData/File/");
                string folderPath = basePath + "\\" + file.Id + "\\";
                if (!Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }
                string filePath = folderPath + "\\" + files[i].FileName;
                
                using(var fileStream = System.IO.File.OpenWrite(filePath))
                {
                    files[i].InputStream.CopyTo(fileStream);
                }

                file.Path = "/UserData/File/" + file.Id + "/" + files[i].FileName;

                result.Add(file.Path);
                
                db.SaveChanges();
                db.Files.Create();
            }
            return Request.CreateResponse(HttpStatusCode.Created, result);
        }  

        /*
        // POST api/File
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage PostFile(FileUploadModel fileModel, int id)
        {
            if (ModelState.IsValid)
            {
                var file = db.Files.Create();
                var group = db.Groups.Find(id);
                if(group == null)
                {
                    ModelState.AddModelError("File", new ArgumentException("No group with id:" + id));
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                file.Group = db.Groups.Find(id);
                var uploader = db.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);
                if(uploader == null)
                {
                    ModelState.AddModelError("File", new ArgumentException("Invalid uploader"));
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                file.Uploader = uploader;
                file.Name = fileModel.File.FileName.Split('\\', '/').Last();
                file.UploadTime = DateTime.Now;
                file.Visibility = !fileModel.Private;
                db.Files.Add(file);
                db.SaveChanges();

                file.Path = FileHelper.SaveFile(fileModel.File, "File", file.Id.ToString());

                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, file);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = file.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }*/

        // DELETE api/File/5
        public HttpResponseMessage DeleteFile(int id)
        {
            CourseAider.Models.File file = db.Files.Find(id);
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Files.Remove(file);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, file);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}