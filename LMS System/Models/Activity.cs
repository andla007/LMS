using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Activity
    {

      public int Id { get; set; }
      public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

      [DataType(DataType.Date)]
      public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int Days
        {
            get
            {
                return (this.EndDate - this.StartDate).Days + 1;
            }
        }

      public int ModuleId { get; set; }
      public virtual Module Module { get; set; }



      public virtual ICollection<Document> ModuleDocuments { get; set; }
         
    }
}