using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;

namespace LMS_System.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        [Authorize(Roles = "teacher,student")]
        public ActionResult Index()
        {
            var courseID = db.Modules.FirstOrDefault().CourseId;
            ViewBag.CourseID = courseID;
            return View(db.Modules.ToList());
        }

        // GET: Modules/Details/5
        public ActionResult Details(int? id, int? courseID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = courseID;
            return View(module);
        }

        // GET: Modules/Create
        [Authorize(Roles = "teacher")]
        public ActionResult Create(int? id, string name, DateTime? Startdate)
        {


            var course = db.Courses.Where(m => m.Id == id).FirstOrDefault();

            Module module = new Module();
            module.Course = course;
            //if (Startdate != null) { ViewBag.StartDate = "2017-01-03"; }
            // Startdate.ToString("yyyy-MM-dd");
            ViewBag.CourseId = id;
            ViewBag.CourseName = name;
            return View(module);
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Module module, int id)
        {
            if (ModelState.IsValid)
            {
                module.CourseId = id;
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { Id = id });
            }
            return View(module);
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id, int? courseId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = courseId;
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult Edit([Bind(Include = "CourseId,Id,Name,Description,StartDate,EndDate")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { Id = module.CourseId });
            }
            return View(module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(int? id, int? courseId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = courseId;
            return View(module);
        }

        // POST: Modules/Delete/5
        [Authorize(Roles = "teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int courseID)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Details", "Courses", new { id = courseID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



















        public ActionResult IndexFiles(int? parentId)
        {
            if (parentId == null)
            {
                return RedirectToAction("Details", "Modules");
            }
            ViewBag.parentId = parentId;

            var documentfiles = db.ModuleDocuments.Where(d => d.Module.Id == parentId).ToList();

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
                return RedirectToAction("Details", "Modules");
            }
            ViewBag.parentId = parentId;
            ViewBag.Parent = db.Modules.Where(a => a.Id == parentId).ToList();
            return View(db.ModuleDocuments.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int? parentId, string description)
        {
            if (parentId == null)
            {
                return RedirectToAction("Details", "Modules");
            }
            //Verifiering
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
               Module module = db.Modules.Find(parentId);


                var fileName = System.IO.Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), User.Identity.Name + "_moduler_" + parentId + "_" + fileName);
                file.SaveAs(path);

                Document doc = new Document();
                doc.AppUser = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                doc.Module = module;
                doc.Name = path;
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

            return RedirectToAction("IndexFiles", "Modules", new { parentId = parentId });
        }

        public FileResult Download(string FileName)
        {
           
            return File(FileName, System.Net.Mime.MediaTypeNames.Application.Octet);
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
                return RedirectToAction("IndexFiles", "Modules", new { parentId = dbDocument.Module.Id });
            }
            return View();
        }









    }
}
