﻿using System;
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
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int Days
        {
            get
            {
                return (this.EndDate - this.StartDate).Days + 1;
            }
        }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        //public virtual ICollection<Document> ModuleDocuments { get; set; }
    }
}