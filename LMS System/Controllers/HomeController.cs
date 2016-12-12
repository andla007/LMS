using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_System.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("teacher")) { return RedirectToAction("Index", "Courses"); }
            else if (User.IsInRole("student"))
            {
                return RedirectToAction("Details", "Courses" , new { id = 1 }); ;
            }
            return View();
        }

        public ActionResult About()
        {
            //This is test 2
            ViewBag.Message = "LMS - A Learning Management System";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}