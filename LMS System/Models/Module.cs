using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Module
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        //public virtual ICollection<Activity> Activities { get; set; }
        //public virtual ICollection<Document> ModuleDocuments { get; set; }
    }
}