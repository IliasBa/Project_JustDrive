using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class Customer : User
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime Birthday { get; set; }
        public string LicenceNumber { get; set; }
    }
}
