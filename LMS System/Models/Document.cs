using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Document
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }



        public virtual Course Course { get; set; }


        public virtual Module Module { get; set; }


        public virtual Activity Activity { get; set; }


        public virtual AppUsers AppUser { get; set; }
        //public virtual ICollection<Activity> Activities { get; set; }
        //public virtual ICollection<Document> ModuleDocuments { get; set; } 
    }
}