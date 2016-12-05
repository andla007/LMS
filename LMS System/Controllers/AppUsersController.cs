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

namespace LMS_System.Controllers
{
    public class AppUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AppUsers
        [Authorize]
        public ActionResult Index(string CourseId, string Role, string orderby)
        {
            IEnumerable<AppUsers> users = null;
            if (User.IsInRole("teacher"))
            {
                users = db.Users.ToList();
            }
            else
            {
                users = db.Users.ToList().Where(u => u.RoleName == "student");
            }

            if (Role != null && Role != "")
            {
                users = users.Where(u => u.RoleName == Role);
            }
            if (orderby != null)
            {
                switch (orderby.ToLower())
                {
                    case "firstname":
                        users = users.OrderBy(u => u.FirstName);
                        break;
                    case "lastname":
                        users = users.OrderBy(u => u.LastName);
                        break;
                    case "email":
                        users = users.OrderBy(u => u.Email);
                        break;
                    case "phonenumber":
                        users = users.OrderBy(u => u.PhoneNumber);
                        break;
                    case "rolename":
                        users = users.OrderBy(u => u.RoleName);
                        break;
                }
            }

            return View(users.ToList());
        }

        // GET: AppUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUsers appUsers = db.Users.Find(id);
            if (appUsers == null)
            {
                return HttpNotFound();
            }
            return View(appUsers);
        }

        // GET: AppUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,TimeOfRegistration,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUsers appUsers)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(appUsers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appUsers);
        }

        // GET: AppUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUsers appUsers = db.Users.Find(id);
            if (appUsers == null)
            {
                return HttpNotFound();
            }
            return View(appUsers);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,TimeOfRegistration,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUsers appUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUsers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RegisterTeacher", "Account");
            }
            return View(appUsers);
        }

        // GET: AppUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUsers appUsers = db.Users.Find(id);
            if (appUsers == null)
            {
                return HttpNotFound();
            }
            return View(appUsers);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AppUsers appUsers = db.Users.Find(id);
            db.Users.Remove(appUsers);
            db.SaveChanges();
            return RedirectToAction("RegisterTeacher", "Account");
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
