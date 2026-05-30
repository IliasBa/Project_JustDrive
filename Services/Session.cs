using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Text;
using JustDrive.Database;

namespace Project_JustDrive.Services
{
    internal class Session
    {
        public static User CurrentUser { get; set; }
        public static Customer CurrentCustomer { get; set; }
    }
}
