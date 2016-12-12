﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS_System.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS_System.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles = "teacher,student")]
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }


        // GET: Courses/Details/5
        [Authorize(Roles = "teacher,student")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = db.Courses.Where(c => c.Id == id).FirstOrDefault();

            return View(course);
            //if (User.IsInRole("teacher"))
            //{
            //    return RedirectToAction("CourseTeacherView", "Account", new { id = course.Id });
            //}
            //else
            //{
            //    return View(course);
            //}
        }
        // GET: Courses/Create
        [Authorize(Roles = "teacher")]
        public ActionResult Create()
        {
            return View();
        }



        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Details", course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Student with no course attached
        [Authorize(Roles = "teacher,student")]
        public ActionResult Error()
        {
            return View();
        }



        // Schedule view
        [Authorize(Roles = "teacher,student")]
        public ActionResult Schedule(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            DateTime coursestartdate = course.StartDate;
            DateTime courseenddate = course.EndDate;

            var Modules =
                from modules in db.Modules
                orderby modules.StartDate
                where modules.CourseId == id
                select new ScheduleItem { Modulename = modules.Name, Id = modules.Id, ModuleId=modules.Id, ModuleStartDate = modules.StartDate, ModuleEndDate = modules.EndDate, CourseId = modules.CourseId };

            //var Activities =
            //    from activities in db.Activities
            //    orderby activities.StartDate
            //    where activities.ModuleId == 

            var Activities =
                from modules in db.Modules
                join activities in db.Activities on modules.Id equals activities.ModuleId
                orderby modules.StartDate, activities.StartDate
                where modules.CourseId == id
                select new ScheduleItem { Activityname = activities.Name, Activitystartdate = activities.StartDate, Activityenddate = activities.EndDate, ModuleId = modules.Id, ModuleStartDate = modules.StartDate };

            DateTime date = coursestartdate;
            List<ScheduleItem> schedule = new List<ScheduleItem>();
            for (int i = 0; i <= (courseenddate - coursestartdate).Days; i++)
            {
                date = coursestartdate.AddDays(i);
                List<ScheduleItem> msi = Modules.Where(s => s.ModuleStartDate <= date && s.ModuleEndDate >= date).ToList();

                ScheduleItem sItem = new ScheduleItem();


                foreach (var item in msi)
                {
                    sItem.Modulename += item.Modulename + ",";
                    sItem.ModuleStartDate = item.ModuleStartDate;
                    sItem.ModuleEndDate = item.ModuleEndDate;
                    sItem.CourseId = item.CourseId;
                    sItem.ModuleId = item.ModuleId;

                    List<ScheduleItem> asi = Activities.Where(s => s.Activitystartdate <= date && s.Activityenddate >= date).ToList();
                    foreach (var aitem in asi)
                    {
                        sItem.Activityname = aitem.Activityname + ",";
                        sItem.Activitystartdate = aitem.Activitystartdate;
                        sItem.Activityenddate = aitem.Activityenddate;
                        sItem.ModuleId = aitem.ModuleId;
                        
                    }

                }
                if(sItem.Modulename != null)
                sItem.Modulename = sItem.Modulename.TrimEnd(' ', ',');
                if(sItem.Activityname != null)
                sItem.Activityname = sItem.Activityname.TrimEnd(' ', ',');
                sItem.CourseId = id;
                sItem.Date = date;
                schedule.Add(sItem);



                //sItem.Date = date.AddDays(i);
                //ScheduleItem si = ModulesActivities.Where(s => s.Activitystartdate <= sItem.Date && s.Activityenddate >= sItem.Date).FirstOrDefault();
                //if (si != null)
                //{
                //    sItem.Activityname = si.Activityname;
                //    sItem.Modulename = si.Modulename;
                //    sItem.Activitystartdate = si.Activitystartdate;
                //    sItem.Activityenddate = si.Activityenddate;
                //}
                //schedule.Add(sItem);
            }
            return View(schedule);
        }
        //return View(innerJoinQuery.ToList());
        //return View(db.Modules.ToList().Where(m => m.CourseId == courseid).OrderBy(m => m.StartDate));


























        public ActionResult IndexFiles(int? parentId)
        {
            if (parentId == null)
            {
                return RedirectToAction("Details", "Courses");
            }
            ViewBag.parentId = parentId;

            var documentfiles = db.ModuleDocuments.Where(d => d.Course.Id == parentId).ToList();

            //db.Roles.FirstOrDefault(n => n.Id == db.Users.FirstOrDefault(m => m.Id == userfiles[))

            var teachers = GetUsersInRole("teacher");
            var doclist = new List<Document>();
            foreach (var item in documentfiles)
            {
                //var context = new ApplicationDbContext();

                var teacher = teachers.Where(t => t.Email == item.AppUser.Email).FirstOrDefault();
                if (teacher != null)
                {
                    doclist.Add(item);
                }

            }
            return View(doclist);

        }

        public List<AppUsers> GetUsersInRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role = roleManager.FindByName(roleName).Users.First();
            var usersInRole = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.RoleId)).ToList();
            return usersInRole;
        }


        public ActionResult CreateUpload(int? parentId)
        {
            if (parentId == null)
            {
                return RedirectToAction("Details", "Courses");
            }
            ViewBag.parentId = parentId;
            ViewBag.Parent = db.Courses.Where(a => a.Id == parentId).ToList();
            return View(db.ModuleDocuments.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int? parentId, string description)
        {
            if (parentId == null)
            {
                return RedirectToAction("Details", "Courses");
            }
            //Verifiering
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                Course course = db.Courses.Find(parentId);


                var fileName = System.IO.Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), User.Identity.Name + "_kurser_" + parentId + "_" + fileName);
                file.SaveAs(path);

                Document doc = new Document();
                doc.FilePath = path;
                doc.AppUser = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                doc.Course = course;

                doc.Name = fileName;
                doc.StartDate = DateTime.Now;

                //Jag vill ha description.
                doc.Description = description;

                db.ModuleDocuments.Add(doc);
                db.SaveChanges();
                //activity.
                //db.Entry(activity).State = EntityState.Modified;
                //db.SaveChanges();
            }
            ViewBag.parentId = parentId;

            return RedirectToAction("IndexFiles", "Courses", new { parentId = parentId });
        }

        public FileResult Download(string FilePath, string Name)
        {
            return File(FilePath, System.Net.Mime.MediaTypeNames.Application.Octet, Name);
        }

        public ActionResult DeleteFile(int? id, int? parentId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.ModuleDocuments.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentId = parentId;
            return View(document);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFileConfirmed(int id, int? parentId)
        {
            Document document = db.ModuleDocuments.Find(id);
            db.ModuleDocuments.Remove(document);
            db.SaveChanges();
            return RedirectToAction("IndexFiles", new { parentId = parentId });
        }


        public ActionResult EditFile(int? id, int? parentId)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.ModuleDocuments.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentId = parentId;
            return View(document);
        }

        [HttpPost]
        [Authorize(Roles = "teacher")]
        [ValidateAntiForgeryToken]
        public ActionResult EditFile([Bind(Include = "Id,Description")] Document document, int? parentId)
        {

            if (ModelState.IsValid)
            {
                var dbDocument = db.ModuleDocuments.Find(document.Id);
                dbDocument.Description = document.Description;
                db.Entry(dbDocument).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexFiles", "Courses", new { parentId = dbDocument.Course.Id });
            }
            return View();
        }


    }

    public class ScheduleItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Modulename { get; set; }
        public DateTime ModuleStartDate { get; set; }
        public DateTime ModuleEndDate { get; set; }
        public string Activityname { get; set; }
        public DateTime Activitystartdate { get; set; }
        public DateTime Activityenddate { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
    }
}

