using System;
using System.Collections.Generic;
using System.Text;
using Project_JustDrive.Models;

namespace Project_JustDrive.Services
{
    class Session
    {
        public static User CurrentUser { get; set; }
         public static Customer CurrentCustomer { get; set; }

        
    }
}
