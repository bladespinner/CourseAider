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

namespace CourseAider.Controllers
{
    public class FileController : ApiController
    {
        private CourseAiderContext db = new CourseAiderContext();

        // GET api/File
        public IEnumerable<File> GetFiles()
        {
            return db.Files.AsEnumerable();
        }

        // GET api/File/5
        public File GetFile(int id)
        {
            File file = db.Files.Find(id);
            if (file == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return file;
        }

        // PUT api/File/5
        public HttpResponseMessage PutFile(int id, File file)
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

        // POST api/File
        public HttpResponseMessage PostFile(File file)
        {
            if (ModelState.IsValid)
            {
                db.Files.Add(file);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, file);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = file.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/File/5
        public HttpResponseMessage DeleteFile(int id)
        {
            File file = db.Files.Find(id);
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