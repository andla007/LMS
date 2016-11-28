namespace LMS_System.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LMS_System.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMS_System.Models.ApplicationDbContext context)
        {
            //
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            foreach (var roleName in new[] { "teacher" })
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    var result = roleManager.Create(role);
                    if (result.Succeeded == false)
                    {
                        throw new Exception($"Error creating role {roleName} in seed. Error: {result.Errors}");
                    }
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var userSeed = new UserSeed[]
            {
               new UserSeed() {EMail="admin@lexicon.se",FirstName="Oscar",LastName="Jakobsson",TimeOfRegistration=DateTime.Today }
            };


            foreach (var us in userSeed)
            {
                if (!context.Users.Any(u => u.Email == us.EMail))
                {
                    var user = new ApplicationUser
                    {
                        UserName = us.EMail,
                        Email = us.EMail,
                        FirstName = us.FirstName,
                        LastName = us.LastName,
                        TimeOfRegistration = us.TimeOfRegistration
                    };
                    var result = userManager.Create(user, "lexicon");
                    if (result.Succeeded == false)
                    {
                        throw new Exception($"Error creating user {us.EMail} in seed. Error: {result.Errors}");
                    }
                }
            }

            var adminUser = userManager.FindByName("admin@lexicon.se");
            userManager.AddToRole(adminUser.Id, "teacher");


            context.Courses.AddOrUpdate(p => p.Name,
                new Course {
                    Name = "Seducing and merry Visual Studio",
                    Description = "The Seducing and Merry Visual Studio Course. Leave your girlfriend and focus on Loads of programming. No worrying about social life",
                    StartDate=DateTime.Now.AddMinutes(2),
                    EndDate=DateTime.Now.AddMinutes(4)
                    }
                );
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
