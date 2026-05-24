using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminDamageReports : Window
    {
        private int _userId;

        public AdminDamageReports(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadReports();
        }

        private void LoadReports()
        {
            var reports = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT 
                        d.Id,
                        d.Description,
                        d.DamageLevel,
                        CONCAT(cu.First_Name, ' ', cu.Last_Name) AS KlantNaam,
                        u.Email,
                        CONCAT(cn.Brand, ' ', cn.Model) AS AutoNaam
                    FROM damagereport d
                    INNER JOIN user u ON d.UserId = u.User_Id
                    INNER JOIN customer cu ON d.UserId = cu.UserId
                    INNER JOIN reservation r ON d.ReservationId = r.Id
                    INNER JOIN car c ON r.CarId = c.Id
                    INNER JOIN carname cn ON c.CarNameId = cn.Id
                    ORDER BY d.Id DESC";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reports.Add(new
                    {
                        Id = reader["Id"].ToString(),
                        Beschrijving = reader["Description"].ToString(),
                        DamageLevel = reader["DamageLevel"].ToString(),
                        KlantNaam = reader["KlantNaam"].ToString(),
                        Email = reader["Email"].ToString(),
                        AutoNaam = reader["AutoNaam"].ToString()
                    });
                }
            }

            DamagePanel.ItemsSource = reports;
            TxtAantal.Text = $"{reports.Count} schaderapporten gevonden";
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard(_userId);
            dashboard.Show();
            this.Close();
        }

        private void Gebruikers_Click(object sender, RoutedEventArgs e)
        {
            AdminUsers users = new AdminUsers(_userId);
            users.Show();
            this.Close();
        }

        private void Reservaties_Click(object sender, RoutedEventArgs e)
        {
            AdminReservations reservations = new AdminReservations(_userId);
            reservations.Show();
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