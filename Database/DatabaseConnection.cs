using MySql.Data.MySqlClient;

namespace JustDrive.Database
{
    public class DatabaseConnection
    {
        private static string connectionString =
            "Server=localhost;Database=justdrive;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}