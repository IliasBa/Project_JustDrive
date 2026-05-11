using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using Project_JustDrive.Services;
using System.Windows;
using Project_JustDrive.Windows.Company;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Project_JustDrive
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT u.User_Id, u.Email, u.Role, c.First_Name, c.Last_Name, c.Birthday, c.Licence_Number
                 FROM user u
                 LEFT JOIN customer c ON c.UserId = u.User_Id
                 WHERE u.Email = @email AND u.Password = @password";


                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["User_Id"]);
                        string role = reader["Role"].ToString();

                        if (role == "customer")
                        {
                            var customer = new Customer
                            {
                                UserId = userId,
                                Email = reader["Email"].ToString(),
                                Role = role,
                                FirstName = reader["First_Name"].ToString(),
                                LastName = reader["Last_Name"].ToString(),
                                Birthday = Convert.ToDateTime(reader["Birthday"]),
                                LicenceNumber = reader["Licence_Number"].ToString()
                            };

                            Session.CurrentUser = customer;      // Customer IS a User
                            Session.CurrentCustomer = customer;  // Also store as Customer
                        }

                        reader.Close();

                        if (role == "customer")
                        {
                            var dashboard = new Windows.Clients.Dashboard(userId);
                            dashboard.Show();
                            this.Close();
                        }
                        else if (role == "company")
                        {
                                CompanyDashboard companyDashboard = new CompanyDashboard(userId);
                                companyDashboard.Show();
                                this.Close();
                        }
                        else if (role == "admin")
                        {
                            MessageBox.Show("Admin dashboard komt binnenkort!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void TxtRegister_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Registratie komt binnenkort!");
        }
    }
}