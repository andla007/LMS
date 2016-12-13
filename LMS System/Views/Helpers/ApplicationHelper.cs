using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;

namespace LMS_System.Views.Helpers
{
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using LMS_System.Extensions;
    using Microsoft.AspNet.Identity;
    using Models;
    public static class ApplicationHelpers
        {

        public static string BuildBreadcrumbNavigation(this HtmlHelper helper, string Coursename, string Modulename, string Activityname)
        {
            //optional condition: I didn't wanted it to show on home and account controller
            //if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home" ||
            //    helper.ViewContext.RouteData.Values["controller"].ToString() == "Account")
            //{
            //    return string.Empty;
            //}
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home")
            {
                return string.Empty;
            }

            //Konverterar html från action link. 'Home' kommer bli den första breadcrum
            //HttpContext.Current.Request.UrlReferrer
            var User = HttpContext.Current.User;
            string Userid = User.Identity.GetUserId();
            var dbContext = new ApplicationDbContext();
            var enrolledCourse = (from course in dbContext.Courses
                                  from student in course.Students
                                  where student.Id == Userid
                                  select course).FirstOrDefault();

            StringBuilder breadcrumb = new StringBuilder();

            breadcrumb.Append("<div class=\"breadcrumb\">");

            if (User.IsInRole("teacher"))
            {
                breadcrumb.Append("<li>").Append(helper.ActionLink("Home", "Index", "Courses").ToHtmlString()).Append("</li>");
            }
            else
            {
                if (enrolledCourse != null)
                {
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Home", "Details", "Courses", new { Id = enrolledCourse.Id }, null).ToHtmlString()).Append("</li>");
                }
            }

            string action = helper.ViewContext.RouteData.Values["action"].ToString().ToLower();
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString().ToLower();

            string id = "0";
            if (helper.ViewContext.RouteData.Values.Count > 2)
            {
                id = helper.ViewContext.RouteData.Values["id"].ToString();
            }
            int Id = id == null ? 0 : int.Parse(id);
            if(Id == 0)
            {
                Id = HttpContext.Current.Request.QueryString["ModuleId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ModuleId"]);
                if(Id == 0)
                {
                    Id = HttpContext.Current.Request.QueryString["ParentId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ParentId"]);
                    if (Id == 0)
                    {
                        Id = HttpContext.Current.Request.QueryString["ActivityId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ActivityId"]);
                    }
                }
            }




            string addtoaction = "";

            if (controller == "courses" && action == "create")
            {
                addtoaction = " course";
            }
            else
            {
                if ((controller == "courses" && action != "index") || (controller == "modules" && action == "create"))
                {
                    Course c = dbContext.Courses.Where(course => course.Id == Id).FirstOrDefault();
                 
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = c.Id }, null).ToHtmlString()).Append("</li>");
                  
                    if (controller == "modules" && action == "create") { addtoaction = " module"; }
                    if (controller == "courses" && action == "create") { addtoaction = " course"; }

                }
                if (controller == "modules" && action != "create" || (controller == "activities" && action == "create"))
                {
                    if (controller == "activities" && action == "create")
                    {
                        addtoaction = " activity";
                        
                    }
                    Module m = dbContext.Modules.Where(modules => modules.Id == Id).FirstOrDefault();
                    Course c = dbContext.Courses.Where(course => course.Id == m.CourseId).FirstOrDefault();
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = m.CourseId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Module " + m.Name, "Details", "Modules", new { Id = m.Id }, null).ToHtmlString()).Append("</li>");

                }
                if (controller == "activities" && action != "create")
                {
                    Activity a = dbContext.Activities.Where(activity => activity.Id == Id).FirstOrDefault();
                    if(a==null)
                    {
                        return "";
                    }
                    Module m = dbContext.Modules.Where(modules => modules.Id == a.ModuleId).FirstOrDefault();
                    Course c = dbContext.Courses.Where(course => course.Id == m.CourseId).FirstOrDefault();
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = m.CourseId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Module " + m.Name, "Details", "Modules", new { Id = a.ModuleId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Activity " + a.Name, "Details", "Modules", new { Id = a.Id }, null).ToHtmlString()).Append("</li>");
                }




            }










            ////breadcrumb.Append("<li>");

            ////Länktext, action och controller. Action blir index som default om inget annat hittas.
            ////breadcrumb.Append(helper.ActionLink(helper.ViewContext.RouteData.Values["controller"].ToString().Titleize(),
            //                                   "Index",
            //                                   helper.ViewContext.RouteData.Values["controller"].ToString()));
            ////breadcrumb.Append("</li>");

            if (action != "index" && action != "details")
            {
                string linktitle = action;

                // Exceptions
                linktitle = linktitle.Replace("registerteacher", "Register teacher");
                linktitle = linktitle.Replace("courseteacherview", "Register student");
                linktitle = linktitle.Replace("indexfiles", "Documents");


                breadcrumb.Append("<li>");
                breadcrumb.Append(helper.ActionLink(linktitle.Titleize() + addtoaction, action, controller));
                breadcrumb.Append("</li>");
            }

            return breadcrumb.Append("</div>").ToString();
        }


        public static int WeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }
 
}