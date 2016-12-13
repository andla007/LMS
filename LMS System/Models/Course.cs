using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }   

        public void AddStudent(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            AppUsers student = context.Users.Where(u => u.Id == id).FirstOrDefault();
            Students.Add(student);
            context.SaveChanges();
        }

        public virtual ICollection<AppUsers> Students { get; set; }
        public virtual ICollection<AppUsers> Teachers { get; set; } 
        public virtual ICollection<Module> Modules { get; set; }
        //public virtual ICollection<Document> CourseDocuments { get; set; }
    }
}