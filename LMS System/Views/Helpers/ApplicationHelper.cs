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

            try //I added this to a try catch block to protect from recurring missing Id values. When Course Id, Module Id or Activty Id is missing we can't extract values for breadcrums
            { 
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home")//Don't show breadcrum on the main page.
            {
                return string.Empty;
            }

            //Konverterar html från action link. 'Home' kommer bli den första breadcrum
            //HttpContext.Current.Request.UrlReferrer
            var User = HttpContext.Current.User;
            string Userid = User.Identity.GetUserId();
            var dbContext = new ApplicationDbContext(); //Will have the courses when used. 
            var enrolledCourse = (from course in dbContext.Courses //This code we know allready
                                  from student in course.Students
                                  where student.Id == Userid
                                  select course).FirstOrDefault();

            StringBuilder breadcrumb = new StringBuilder(); //User string builder for performance.

            breadcrumb.Append("<div class=\"breadcrumb\">"); //Start the breadcrum with class breadcrum in a div.

            if (User.IsInRole("teacher"))
            {
                breadcrumb.Append("<li>").Append(helper.ActionLink("Home", "Index", "Courses").ToHtmlString()).Append("</li>"); //Add the home link for teacher
            }
            else
            {
                if (enrolledCourse != null)
                {
                    //Enroller studenst will see the course details. The id of the course is added so that razor and courses action can work with it.
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Home", "Details", "Courses", new { Id = enrolledCourse.Id }, null).ToHtmlString()).Append("</li>");
                }
            }

                //Get the route data for action. Basically just grab the action value displayed in the browser navigation field
                string action = helper.ViewContext.RouteData.Values["action"].ToString().ToLower();
            
            //Get the controller action name (value) from the browser navigation field
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString().ToLower();

           
            string id = "0";
            if (helper.ViewContext.RouteData.Values.Count > 2)//We want the third parameter to be the Id
            {
                id = helper.ViewContext.RouteData.Values["id"].ToString();//Return the Id.
            }
            int Id = id == null ? 0 : int.Parse(id); //Assign id to zero if we didn't find any Id.
            if(Id == 0)//The zero is very uncertain so try grab some probabable id hiding places.
            {
                Id = HttpContext.Current.Request.QueryString["ModuleId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ModuleId"]); //If not null grab X in ?ModuleId=X
                if(Id == 0)
                {
                    Id = HttpContext.Current.Request.QueryString["ParentId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ParentId"]); //If not null grab X in ?ParentId=X
                        if (Id == 0)
                    {
                        Id = HttpContext.Current.Request.QueryString["ActivityId"] == null ? 0 : int.Parse(HttpContext.Current.Request.QueryString["ActivityId"]);//If not null grab X in ?ActivityId=X
                        }
                }
            }




            string addtoaction = ""; //This will always be used when the action is create. We want the title to be create module, create activity, create lussebulle etc.

            if (controller == "courses" && action == "create")
            {
                addtoaction = " course"; //This will be added inside the link title further below. In this case the result will be "create"+" course" 
            }
            else
            {
                if ((controller == "courses" && action != "index") || (controller == "modules" && action == "create")) //courses but not index or modules with create
                {
                    Course c = dbContext.Courses.Where(course => course.Id == Id).FirstOrDefault(); //Get the course with the given index.
                    if (c == null)//Old code which is not necessary any more because we use a try catch block instead.
                        return "";
                    //We add the name to the link title and add the id as a query param in the link. helper.ActionLink is taking care of the output.
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = c.Id }, null).ToHtmlString()).Append("</li>");
                  
                    if (controller == "modules" && action == "create") { addtoaction = " module"; } //Will be create module
                    if (controller == "courses" && action == "create") { addtoaction = " course"; } //Will be create course

                }
                if (controller == "modules" && action != "create" || (controller == "activities" && action == "create"))
                {
                    if (controller == "activities" && action == "create")
                    {
                        addtoaction = " activity"; //link title = create activity
                        
                    }
                    Module m = dbContext.Modules.Where(modules => modules.Id == Id).FirstOrDefault(); //We wan the name of the module
                    Course c = dbContext.Courses.Where(course => course.Id == m.CourseId).FirstOrDefault();//The name of the course
                        //Then just add that to bread crum. You know the drill.
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = m.CourseId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Module " + m.Name, "Details", "Modules", new { Id = m.Id }, null).ToHtmlString()).Append("</li>");

                }
                if (controller == "activities" && action != "create")
                {
                    Activity a = dbContext.Activities.Where(activity => activity.Id == Id).FirstOrDefault();//Get activity
                    if(a==null)
                    {
                        return "";
                    }
                    Module m = dbContext.Modules.Where(modules => modules.Id == a.ModuleId).FirstOrDefault();//Get modules
                    Course c = dbContext.Courses.Where(course => course.Id == m.CourseId).FirstOrDefault();//Get course

                        //Add name in title and id as a query.
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Course " + c.Name, "Details", "Courses", new { Id = m.CourseId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Module " + m.Name, "Details", "Modules", new { Id = a.ModuleId }, null).ToHtmlString()).Append("</li>");
                    breadcrumb.Append("<li>").Append(helper.ActionLink("Activity " + a.Name, "Details", "Activities", new { Id = a.Id }, null).ToHtmlString()).Append("</li>");
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
                linktitle = linktitle.Replace("registerteacher", "Register teacher"); //More friendly text. Add a space 
                linktitle = linktitle.Replace("courseteacherview", "Register student");
                linktitle = linktitle.Replace("indexfiles", "Documents");//The list of files will be named documents in the link title.


                breadcrumb.Append("<li>");
                breadcrumb.Append(helper.ActionLink(linktitle.Titleize() + addtoaction, action, controller)); //This is where we add create module, creat course etc.
                breadcrumb.Append("</li>");
            }
                return breadcrumb.Append("</div>").ToString();
            }
            catch (Exception ex)
            {
                return ""; //I know this is to lazy.

            }

          
        }


        public static int WeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }
 
}