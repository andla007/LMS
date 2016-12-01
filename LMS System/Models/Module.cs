﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMS_System.Models
{
    public class Module
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public int Course_Id { get; set; }
        public virtual Course Course { get; set; }
        //public virtual ICollection<Activity> Activities { get; set; }
        //public virtual ICollection<Document> ModuleDocuments { get; set; }
    }
}