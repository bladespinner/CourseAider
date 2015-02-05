using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseAider.Models;
using WebMatrix.WebData;

namespace CourseAider.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private CourseAiderContext db = new CourseAiderContext();

        //
        // GET: /Group/Details/5

        public ActionResult Details(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            bool isTeacher = false;
            using (CourseAiderContext context = new CourseAiderContext())
            {
                var profile = context.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);
                isTeacher = profile.IsTeacher;
            }

            ViewBag.isTeacher = isTeacher;
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string dummy,int id = 0)
        {
            Group group = db.Groups.Find(id);
           
            bool isTeacher = false;

            if (group == null)
            {
                return HttpNotFound();
            }

            var profile = db.UserProfiles.FirstOrDefault(p => p.UserName == WebSecurity.CurrentUserName);

            if (!group.Course.Members.Any(member => member.UserName == profile.UserName))
            {
                return new HttpUnauthorizedResult("You are not enrolled for this group's course");
            }
            if (!group.Members.Any(member => member.UserName == profile.UserName))
            {
                group.Members.Add(profile);
                isTeacher = profile.IsTeacher;
            }
            db.SaveChanges();

            ViewBag.isTeacher = isTeacher;
            return View(group);
        }

        //
        // GET: /Group/Create

        public ActionResult Create(int id = 0)
        {
            if (db.Courses.Find(id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.courseId = id;
            return View();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Group group, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var course = db.Courses.Find(id);
                if (course == null)
                {
                    return HttpNotFound();
                }
                group.Course = course;
                group.Creator = db.UserProfiles.FirstOrDefault(user => user.UserName == WebSecurity.CurrentUserName);
                group.Members = new List<UserProfile>();
                group.Members.Add(group.Creator);
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Details", "Group", new { id = group.Id });
            }
            ViewBag.courseId = id;
            return View(group);
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        //
        // GET: /Group/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            int parent = group.Course.Id;
            group.Members.Clear();
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Details", "Course", new { id = parent });
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult UploadFile(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            ViewData["groupId"] = id;
            return PartialView("_FileUpload", new FileUploadModel()
            {
                Private = false
            });
        }
        [HttpGet]
        [ChildActionOnly]
        public ActionResult ListFiles(int id = 0)
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
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return PartialView("_FileList", group.Files.ToList());
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult ListFiles(string dummy,int id = 0)
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
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return PartialView("_FileList", group.Files.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}