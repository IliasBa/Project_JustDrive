using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminDashboard : Window
    {
        private int _userId;

        public AdminDashboard(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Totaal klanten
                string klanten = "SELECT COUNT(*) FROM customer";
                TxtTotaalKlanten.Text = new MySqlCommand(klanten, conn).ExecuteScalar().ToString();

                // Totaal bedrijven
                string bedrijven = "SELECT COUNT(*) FROM company";
                TxtTotaalBedrijven.Text = new MySqlCommand(bedrijven, conn).ExecuteScalar().ToString();

                // Totaal autos
                string autos = "SELECT COUNT(*) FROM car";
                TxtTotaalAutos.Text = new MySqlCommand(autos, conn).ExecuteScalar().ToString();

                // Totaal reservaties
                string reservaties = "SELECT COUNT(*) FROM reservation";
                TxtTotaalReservaties.Text = new MySqlCommand(reservaties, conn).ExecuteScalar().ToString();

                // Recente gebruikers
                string query = "SELECT Email, Role, City FROM user ORDER BY User_Id DESC LIMIT 10";
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                var gebruikers = new List<dynamic>();
                while (reader.Read())
                {
                    gebruikers.Add(new
                    {
                        Email = reader["Email"].ToString(),
                        Role = reader["Role"].ToString(),
                        City = reader["City"].ToString()
                    });
                }
                RecenteGebruikers.ItemsSource = gebruikers;
            }
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