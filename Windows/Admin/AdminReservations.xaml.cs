using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminReservations : Window
    {
        private int _userId;

        public AdminReservations(int userId)
        {
            InitializeComponent();

            _userId = userId;

            LoadReservations();
        }

        private void LoadReservations()
        {
            var reservations = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT r.Id, CONCAT(cu.First_Name, ' ', cu.Last_Name) AS Klant, 
CONCAT(cn.Brand, ' ', cn.Model) AS Auto, r.Start_date, r.End_date, r.Total_price
FROM reservation r 
INNER JOIN car c ON r.CarId = c.Id INNER JOIN customer cu ON r.CustomerId = cu.UserId 
INNER JOIN carname cn ON c.CarNameId = cn.Id ORDER BY r.Start_date DESC";

                var cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new
                    {
                        Id = reader["Id"].ToString(),
                        Klant = reader["Klant"].ToString(),
                        Auto = reader["Auto"].ToString(),
                        StartDate = Convert.ToDateTime(reader["Start_date"]).ToString("dd/MM/yyyy"),
                        EndDate = Convert.ToDateTime(reader["End_date"]).ToString("dd/MM/yyyy"),
                        TotalPrice = reader["Total_price"].ToString(),
                        Status = Convert.ToDateTime(reader["End_date"]) >= DateTime.Now ? "Actief" : "Afgerond"
                    });
                }
            }

            ReservationsPanel.ItemsSource = reservations;

            TxtAantal.Text = $"{reservations.Count} reservaties gevonden";
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

        private void Verwijder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            dynamic reservatie = btn.DataContext;

            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je deze reservatie wil verwijderen?",
                "Bevestigen",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM reservation WHERE Id = @id";

                    var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", reservatie.Id);

                    cmd.ExecuteNonQuery();
                }

                LoadReservations();
            }
        }
    }
}