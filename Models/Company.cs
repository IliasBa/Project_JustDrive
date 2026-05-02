using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class Company : User
    {
        public string CompanyName { get; set; }
        public string VATNumber { get; set; }
    }
}
