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
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS_System.Controllers
{
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activities
        public ActionResult Index()
        {
            return View(db.Activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        public ActionResult IndexFiles(int? parentId)
        {
            if (parentId == null)
            {
                return RedirectToAction("Index", "Activities");
            }
            ViewBag.parentId = parentId;


            //db.Roles.FirstOrDefault(n => n.Id == db.Users.FirstOrDefault(m => m.Id == userfiles[))

            if (User.IsInRole("student"))
            {

                var documentfiles = db.ModuleDocuments.ToList();

                var teachers = GetUsersInRole("teacher");
                var doclist = new List<Document>();
                foreach (var item in documentfiles)
                {
                    //var context = new ApplicationDbContext();

                    if (item.AppUser.Email == User.Identity.Name)
                    {
                        doclist.Add(item);
                    }
                    else
                    {
                        var teacher = teachers.Where(t => t.Email == item.AppUser.Email).FirstOrDefault();
                        if (teacher != null)
                        {
                            doclist.Add(item);
                        }
                    }


                }
                return View(doclist);
            }
            else if(User.IsInRole("teacher"))
            {
                return View(db.ModuleDocuments.ToList());
            }


            return View(new List<Document>());


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
                return RedirectToAction("Index", "Courses");
            }
            ViewBag.parentId = parentId;
            ViewBag.Parent = db.Activities.Where(a => a.Id == parentId).ToList();
            return View(db.ModuleDocuments.FirstOrDefault());
        }

        //// This action handles the form POST and the upload
        [HttpPost]
        public ActionResult ActivityUpload(HttpPostedFileBase file, int? parentId, string description)
        {
            if (parentId == null)
            {
                return RedirectToAction("Index", "Courses");
            }
            //Verifiering
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                Activity activity = db.Activities.Find(parentId);


                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), User.Identity.Name + "_aktiviteter_" + parentId + "_" + fileName);
                file.SaveAs(path);

                Document doc = new Document();
                doc.AppUser = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                doc.Activity = activity;
                doc.Name = path;
                doc.StartDate = DateTime.Now;
                if (activity.Assignment)
                {
                    doc.EndDate = activity.EndDate;


                    if (doc.StartDate > doc.EndDate)
                    {
                        ViewBag.Error = "Start date cannot be greater than end date stupid!!";
                    }
                }

                //Jag vill ha description.
                doc.Description = description;

                db.ModuleDocuments.Add(doc);
                db.SaveChanges();
                //activity.
                //db.Entry(activity).State = EntityState.Modified;
                //db.SaveChanges();
            }
            ViewBag.parentId = parentId;

            return RedirectToAction("IndexFiles", "Activities", new { parentId = parentId });
        }
        // GET: Activities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Assignment,ModuleId,Id,Name,Description,StartDate,EndDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Assignment,ModuleId,Id,Name,Description,StartDate,EndDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }



        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}