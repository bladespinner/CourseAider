using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseAider.Models;
using WebMatrix.WebData;

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
            List<UserProfile> courses = db.UserProfiles.OrderByDescending(course => course.Score).Take(3).ToList();
            if(WebSecurity.IsAuthenticated)
            {
                var u = db.UserProfiles.FirstOrDefault(user => user.UserName == WebSecurity.CurrentUserName);
                if (u != null)
                {
                    courses.Add(u);
                }
            }
            return PartialView("_TopUsers", courses);
        }
    }
}
