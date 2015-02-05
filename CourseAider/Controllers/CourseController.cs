using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseAider.Models;
using WebMatrix.WebData;
using CourseAider.Filters;
using System.IO;
using CourseAider.Helpers;

namespace CourseAider.Controllers
{
    [InitializeSimpleMembership]
    public class CourseController : Controller
    {
        private CourseAiderContext db = new CourseAiderContext();

        //
        // GET: /Course/

        public ActionResult Index()
        {
            bool isTeacher = false;
            using (CourseAiderContext context = new CourseAiderContext())
            {
                var profile = context.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);
                if (profile != null)
                {
                    isTeacher = profile.IsTeacher;
                }
            }
            ViewBag.isTeacher = isTeacher;
            return View(db.Courses.ToList());
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string dummy,int id = 0)
        { 
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            var profile = db.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);
            if(!course.Members.Any(member => member.UserName == profile.UserName))
            {
                course.Members.Add(profile);
            }
            db.SaveChanges();

            return View(course);
        }

        //
        // GET: /Course/Create
        [Authorize(Roles="Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(CreateCourseModel courseModel)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course();
                //set creator to current user
                course.Creator = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                course.Members = new List<UserProfile>();
                course.Members.Add(course.Creator);

                course.Description = courseModel.Description;
                course.Name = courseModel.Name;
                course.Image = courseModel.Image.FileName;

                course = db.Courses.Add(course);
                course.DateCreated = DateTime.Now;

                //persist changes and refresh so we get an courseId
                course.DateCreated = DateTime.Now;

                db.SaveChanges();
                db.Entry(course).GetDatabaseValues();

                course.Image = FileHelper.SaveFile(courseModel.Image, "Course", course.Id.ToString());         

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseModel);
        }

        //
        // GET: /Course/Edit/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(new EditCourseModel()
            {
                Description = course.Description,
                Name = course.Name,
                ImagePath = course.Image
            });
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(EditCourseModel courseModel)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course();
                //set creator to current user
                course.Creator = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                course.Members = new List<UserProfile>();
                course.Members.Add(course.Creator);

                course.Description = courseModel.Description;
                course.Name = courseModel.Name;
                course.Image = courseModel.Image.FileName;

                course = db.Courses.Add(course);
                course.DateCreated = DateTime.Now;

                //persist changes and refresh so we get an courseId
                course.DateCreated = DateTime.Now;

                db.SaveChanges();
                db.Entry(course).GetDatabaseValues();

                course.Image = FileHelper.SaveFile(courseModel.Image, "Course", course.Id.ToString());

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseModel);
        }

        //
        // GET: /Course/Delete/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            course.Members.Clear();
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}