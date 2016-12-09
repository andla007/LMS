using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS_System.Models;

namespace LMS_System.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles="teacher,student")]
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
        public ActionResult Schedule(int courseid)
        {
            var course = db.Courses.Where(c => c.Id == courseid).FirstOrDefault();
            DateTime coursestartdate = course.StartDate;
            DateTime courseenddate = course.EndDate;

            var Modules =
                from modules in db.Modules
                orderby modules.StartDate
                where modules.CourseId == courseid
                select new ScheduleItem { Modulename = modules.Name, Id = modules.Id, ModuleStartDate = modules.StartDate, ModuleEndDate = modules.EndDate, CourseId = modules.CourseId };

            //var Activities =
            //    from activities in db.Activities
            //    orderby activities.StartDate
            //    where activities.ModuleId == 

            var Activities =
                from modules in db.Modules
                join activities in db.Activities on modules.Id equals activities.ModuleId
                orderby modules.StartDate, activities.StartDate
                where modules.CourseId == courseid
                select new ScheduleItem { Activityname = activities.Name, Activitystartdate = activities.StartDate, Activityenddate = activities.EndDate, ModuleId = activities.ModuleId };

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

                    List<ScheduleItem> asi = Activities.Where(s => s.Activitystartdate <= date && s.Activityenddate >= date).ToList();
                    foreach (var aitem in asi)
                    {
                        sItem.Activityname = aitem.Activityname + ",";
                        sItem.Activitystartdate = aitem.Activitystartdate;
                        sItem.Activityenddate = aitem.Activityenddate;
                        sItem.ModuleId = aitem.ModuleId;
                    }

                }


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
            //return View(innerJoinQuery.ToList());
            //return View(db.Modules.ToList().Where(m => m.CourseId == courseid).OrderBy(m => m.StartDate));
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
