using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace LMS_System.Models
{
    // You can add profile data for the user by adding more properties to your AppUsers class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUsers : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUsers> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        //
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public System.DateTime TimeOfRegistration { get; set; }

        public string RoleName
        {
            get
            {
                string roleName = "";
                var context = new ApplicationDbContext();
                var userStore = new UserStore<AppUsers>(context);
                var userManager = new UserManager<AppUsers>(userStore);
                if (userManager.IsInRole(this.Id, "teacher")) { roleName = "teacher"; }
                if (userManager.IsInRole(this.Id, "student")) { roleName = "student"; }
                return roleName;
            }
        }




    }

    public class ApplicationDbContext : IdentityDbContext<AppUsers>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Activity> Activities { get; set; }

    }
}