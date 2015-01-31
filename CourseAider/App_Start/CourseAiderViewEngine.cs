using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseAider.App_Start
{
    public class CourseAiderViewEngine : RazorViewEngine
    {
        public CourseAiderViewEngine()
        {
            var viewLocations = new[] {  
            "~/Views/{1}/{0}.cshtml",  
            "~/Views/Shared/{0}.cshtml",  
            "~/Views/Shared/Layout/{0}.cshtml",
        };

            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }
    }
}