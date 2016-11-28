using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_System.Migrations
{ 
    public class UserSeed
    { 
        public string EMail { get; set; }//
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public System.DateTime TimeOfRegistration { get; set; }
    }
}