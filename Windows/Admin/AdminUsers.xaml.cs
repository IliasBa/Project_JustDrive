using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminUsers : Window
    {
        private int _userId;
        private List<dynamic> _allUsers = new List<dynamic>();

        public AdminUsers(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUsers();
        }

        private void LoadUsers()
        {
            _allUsers = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT u.User_Id, u.Email, u.Role, u.City,
                                    COALESCE(CONCAT(c.First_Name, ' ', c.Last_Name), co.Company_Name, 'Admin') AS Naam
                                 FROM user u
                                 LEFT JOIN customer c ON u.User_Id = c.UserId
                                 LEFT JOIN company co ON u.User_Id = co.UserId
                                 ORDER BY u.User_Id DESC";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string naam = reader["Naam"].ToString();
                    _allUsers.Add(new
                    {
                        UserId = Convert.ToInt32(reader["User_Id"]),
                        Email = reader["Email"].ToString(),
                        Naam = naam,
                        City = reader["City"].ToString(),
                        Role = reader["Role"].ToString(),
                        Initiaal = naam.Length > 0 ? naam.Substring(0, 1).ToUpper() : "?"
                    });
                }
            }

            UsersPanel.ItemsSource = _allUsers;
            TxtAantal.Text = $"{_allUsers.Count} gebruikers gevonden";
        }

        private void FilterAlle_Click(object sender, RoutedEventArgs e)
        {
            UsersPanel.ItemsSource = _allUsers;
            TxtAantal.Text = $"{_allUsers.Count} gebruikers gevonden";
        }

        private void FilterKlant_Click(object sender, RoutedEventArgs e)
        {
            var filtered = _allUsers.Where(u => u.Role == "customer").ToList();
            UsersPanel.ItemsSource = filtered;
            TxtAantal.Text = $"{filtered.Count} klanten gevonden";
        }

        private void FilterBedrijf_Click(object sender, RoutedEventArgs e)
        {
            var filtered = _allUsers.Where(u => u.Role == "company").ToList();
            UsersPanel.ItemsSource = filtered;
            TxtAantal.Text = $"{filtered.Count} bedrijven gevonden";
        }

        private void Verwijder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic user = btn.DataContext;

            MessageBoxResult result = MessageBox.Show(
                $"Ben je zeker dat je {user.Email} wil verwijderen? Alle gekoppelde data wordt ook verwijderd.",
                "Bevestigen", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Verwijder in juiste volgorde wegens foreign keys
                    string[] queries = {
                "DELETE FROM damagereport WHERE UserId = @id",
                "DELETE FROM favorite WHERE UserId = @id",
                "DELETE FROM damagereport WHERE ReservationId IN (SELECT Id FROM reservation WHERE CustomerId = @id)",
                "DELETE FROM reservation WHERE CustomerId = @id",
                "DELETE FROM reservation WHERE CarId IN (SELECT Id FROM car WHERE CompanyId = @id)",
                "DELETE FROM car WHERE CompanyId = @id",
                "DELETE FROM customer WHERE UserId = @id",
                "DELETE FROM company WHERE UserId = @id",
                "DELETE FROM user WHERE User_Id = @id"
            };

                    foreach (var query in queries)
                    {
                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", user.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadUsers();
                MessageBox.Show("Gebruiker verwijderd!");
            }
        }

        private void Bewerken_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic user = btn.DataContext;

            var popup = new AdminEditUser(user.UserId);
            popup.ShowDialog();
            LoadUsers();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard(_userId);
            dashboard.Show();
            this.Close();
        }

        private void Reservaties_Click(object sender, RoutedEventArgs e)
        {
            AdminReservations reservations = new AdminReservations(_userId);
            reservations.Show();
            this.Close();
        }

        private void Schade_Click(object sender, RoutedEventArgs e)
        {
            AdminDamageReports reports = new AdminDamageReports(_userId);
            reports.Show();
            this.Close();
        }

        private void Autos_Click(object sender, RoutedEventArgs e)
        {
            AdminCars adcars = new AdminCars(_userId);
            adcars.Show();
            this.Close();
        }

        private void Uitloggen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}