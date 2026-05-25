using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using Project_JustDrive.Services;
using Project_JustDrive.Windows;
using Project_JustDrive.Windows.Admin;
using Project_JustDrive.Windows.Company;
using System;
using System.IO;
using System.Windows;

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

                    string query = @"SELECT u.User_Id, u.Email, u.Role, u.Phone_Number, 
                                            u.Adres, u.Postcode, u.City, u.PASSWORD,
                                            c.First_Name, c.Last_Name, c.Birthday, 
                                            c.Licence_Number, c.Actief_Sinds
                                     FROM user u
                                     LEFT JOIN customer c ON c.UserId = u.User_Id
                                     WHERE u.Email = @email";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedHash = reader["PASSWORD"].ToString();

                        // Verify password
                        if (!PasswordHelper.Verify(password, storedHash))
                        {
                            MessageBox.Show("Email of wachtwoord is fout.");
                            return;
                        }

                        int userId = Convert.ToInt32(reader["User_Id"]);
                        string role = reader["Role"].ToString();

                        if (role == "customer")
                        {
                            var customer = new Customer
                            {
                                UserId = userId,
                                Email = reader["Email"].ToString(),
                                Role = role,
                                PhoneNumber = reader["Phone_Number"].ToString(),
                                Adres = reader["Adres"].ToString(),
                                Postcode = reader["Postcode"].ToString(),
                                City = reader["City"].ToString(),
                                FirstName = reader["First_Name"].ToString(),
                                LastName = reader["Last_Name"].ToString(),
                                Birthday = Convert.ToDateTime(reader["Birthday"]),
                                LicenceNumber = reader["Licence_Number"].ToString(),
                                ActiefSinds = reader["Actief_Sinds"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["Actief_Sinds"])
                                    : DateTime.Today
                            };

                            Session.CurrentUser = customer;
                            Session.CurrentCustomer = customer;
                        }
                        else if (role == "company")
                        {
                            var user = new User
                            {
                                UserId = userId,
                                Email = reader["Email"].ToString(),
                                Role = role,
                                PhoneNumber = reader["Phone_Number"].ToString(),
                                Adres = reader["Adres"].ToString(),
                                Postcode = reader["Postcode"].ToString(),
                                City = reader["City"].ToString()
                            };

                            Session.CurrentUser = user;
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
                            AdminDashboard adminDashboard = new AdminDashboard(userId);
                            adminDashboard.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        
                        MessageBox.Show("Email of wachtwoord is fout.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void Registreren_Click(object sender, RoutedEventArgs e)
        {
            Registreren register = new Registreren();
            register.Show();
            this.Close();
        }

        private void WachtwoordVergeten_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgot = new ForgotPassword();
            forgot.Show();
            this.Close();
        }
    }
}