﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_System.Views.Helpers
{
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using LMS_System.Extensions;


        public static class ApplicationHelpers
        {

        public static string BuildBreadcrumbNavigation(this HtmlHelper helper)
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
            StringBuilder breadcrumb = new StringBuilder("<div class=\"breadcrumb\"><li>").Append(helper.ActionLink("Home", "Index", "Home").ToHtmlString()).Append("</li>");


            breadcrumb.Append("<li>");

            //Länktext, action och controller. Action blir index som default om inget annat hittas.
            breadcrumb.Append(helper.ActionLink(helper.ViewContext.RouteData.Values["controller"].ToString().Titleize(),
                                               "Index",
                                               helper.ViewContext.RouteData.Values["controller"].ToString()));
            breadcrumb.Append("</li>");

            if (helper.ViewContext.RouteData.Values["action"].ToString() != "Index")
            {
                breadcrumb.Append("<li>");
                breadcrumb.Append(helper.ActionLink(helper.ViewContext.RouteData.Values["action"].ToString().Titleize(),
                                                    helper.ViewContext.RouteData.Values["action"].ToString(),
                                                    helper.ViewContext.RouteData.Values["controller"].ToString()));
                breadcrumb.Append("</li>");
            }

            return breadcrumb.Append("</div>").ToString();
        }
    }
 
}