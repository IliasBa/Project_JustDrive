using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminCars : Window
    {
        private int _userId;
        private List<dynamic> _allCars = new List<dynamic>();

        public AdminCars(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadBedrijven();
            LoadCars();
        }

        private void LoadBedrijven()
        {
            var bedrijven = new List<string> { "Alle bedrijven" };

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Company_Name FROM company ORDER BY Company_Name";
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                    bedrijven.Add(reader["Company_Name"].ToString());
            }

            CmbBedrijf.ItemsSource = bedrijven;
            CmbBedrijf.SelectedIndex = 0;
        }

        private void LoadCars()
        {
            _allCars = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT 
                        c.Id,
                        CONCAT(cn.Brand, ' ', cn.Model) AS AutoNaam,
                        c.TYPE AS Type,
                        c.Price_Per_Day AS PricePerDay,
                        c.Fuel,
                        c.Transmission,
                        c.LicensePlate,
                        co.Company_Name AS BedrijfNaam
                    FROM car c
                    INNER JOIN carname cn ON c.CarNameId = cn.Id
                    INNER JOIN company co ON c.CompanyId = co.UserId
                    ORDER BY co.Company_Name, cn.Brand";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _allCars.Add(new
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        AutoNaam = reader["AutoNaam"].ToString(),
                        Type = reader["Type"].ToString(),
                        PricePerDay = reader["PricePerDay"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        LicensePlate = reader["LicensePlate"].ToString(),
                        BedrijfNaam = reader["BedrijfNaam"].ToString()
                    });
                }
            }

            CarsPanel.ItemsSource = _allCars;
            TxtAantal.Text = $"{_allCars.Count} auto's gevonden";
        }

        private void CmbBedrijf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbBedrijf.SelectedItem == null) return;

            string selected = CmbBedrijf.SelectedItem.ToString();

            if (selected == "Alle bedrijven")
            {
                CarsPanel.ItemsSource = _allCars;
                TxtAantal.Text = $"{_allCars.Count} auto's gevonden";
            }
            else
            {
                var filtered = _allCars.Where(c => c.BedrijfNaam == selected).ToList();
                CarsPanel.ItemsSource = filtered;
                TxtAantal.Text = $"{filtered.Count} auto's gevonden";
            }
        }

        private void Verwijder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic auto = btn.DataContext;

            MessageBoxResult result = MessageBox.Show(
                $"Ben je zeker dat je {auto.AutoNaam} wil verwijderen?",
                "Bevestigen", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string[] queries = {
                        "DELETE FROM favorite WHERE CarId = @id",
                        "DELETE FROM damagereport WHERE ReservationId IN (SELECT Id FROM reservation WHERE CarId = @id)",
                        "DELETE FROM reservation WHERE CarId = @id",
                        "DELETE FROM car WHERE Id = @id"
                    };

                    foreach (var query in queries)
                    {
                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", auto.Id);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadCars();
                MessageBox.Show("Auto verwijderd!");
            }
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

        private void Schade_Click(object sender, RoutedEventArgs e)
        {
            AdminDamageReports reports = new AdminDamageReports(_userId);
            reports.Show();
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