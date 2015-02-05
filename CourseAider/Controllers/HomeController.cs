using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseAider.Models;

namespace CourseAider.Controllers
{
    public class HomeController : Controller
    {
        private CourseAiderContext db = new CourseAiderContext();
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult LatestCourses()
        {
            List<Course> courses = db.Courses.OrderByDescending(course => course.DateCreated).Take(4).ToList();
            return PartialView("_LatestCourses", courses);
        }

        [ChildActionOnly]
        public ActionResult TopUsers()
        {
            List<UserProfile> users = db.UserProfiles.OrderByDescending(user => user.Rating).Take(3).ToList();
            return PartialView("_TopUsers", users);
        }
    }
}
