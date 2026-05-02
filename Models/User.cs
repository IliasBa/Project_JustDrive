using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Adres { get; set; }
        public string City { get; set; }
    }
}
