﻿using System;
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

        //
        // GET: /Course/Create
        [Authorize(Roles="Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        private string SaveFile(HttpPostedFileBase file, string type, string id)
        {
            string fileName = file.FileName.Split('\\').Last();
            string filePath = Server.MapPath("~/UserData/" + type + "/" + id);
            if (!Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            var imgPath = filePath + "\\" + fileName;
            file.SaveAs(imgPath);

            string webPath = "/UserData/" + type + '/' + id + "/" + fileName;
            return webPath;
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

                course.Name = courseModel.Name;
                course.Image = courseModel.Image.FileName;

                course = db.Courses.Add(course);

                //persist changes and refresh so we get an courseId
                course.DateCreated = DateTime.Now;
                db.SaveChanges();
                db.Entry(course).GetDatabaseValues();

                course.Image = SaveFile(courseModel.Image, "Course", course.Id.ToString());

            

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
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
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