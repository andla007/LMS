using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Document
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Document> ModuleDocuments { get; set; }
    }
}