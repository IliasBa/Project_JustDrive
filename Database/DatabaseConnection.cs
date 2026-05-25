using MySql.Data.MySqlClient;
using System;

namespace JustDrive.Database
{
    public class DatabaseConnection
    {
        private static string connectionString =
            "Server=localhost;Database=dbjustdrive;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}