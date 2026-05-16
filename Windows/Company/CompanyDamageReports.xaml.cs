using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Project_JustDrive.Windows.Company
{
    public partial class CompanyDamageReports : Window
    {
        private int _userId;

        public CompanyDamageReports(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDamageReports();
        }

        private void LoadDamageReports()
        {
            var reports = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                            CONCAT(cu.First_Name, ' ', cu.Last_Name) AS KlantNaam,
                            CONCAT(cn.Brand, ' ', cn.Model) AS AutoNaam,
                            r.Start_date AS ReservatieDatum,
                            d.DamageLevel AS Schadeniveau,
                            d.Description AS Beschrijving
                         FROM damagereport d
                         INNER JOIN reservation r ON d.ReservationId = r.Id
                         INNER JOIN car c ON r.CarId = c.Id
                         JOIN carname cn ON cn.Id = c.CarNameId
                         INNER JOIN customer cu ON d.UserId = cu.UserId
                         WHERE c.CompanyId = @id
                         ORDER BY r.Start_date DESC";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reports.Add(new
                    {
                        KlantNaam = reader["KlantNaam"].ToString(),
                        AutoNaam = reader["AutoNaam"].ToString(),
                        ReservatieDatum = Convert.ToDateTime(reader["ReservatieDatum"]).ToString("dd/MM/yyyy"),
                        Schadeniveau = reader["Schadeniveau"].ToString(),  // ← add
                        Beschrijving = reader["Beschrijving"].ToString()
                    });
                }
            }

            DamageGrid.ItemsSource = reports;
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            CompanyDashboard dashboard = new CompanyDashboard(_userId);
            dashboard.Show();
            this.Close();
        }

        private void MijnAutos_Click(object sender, RoutedEventArgs e)
        {
            CompanyCars companyCars = new CompanyCars(_userId);
            companyCars.Show();
            this.Close();
        }

        private void Reservaties_Click(object sender, RoutedEventArgs e)
        {
            CompanyReservations reservations = new CompanyReservations(_userId);
            reservations.Show();
            this.Close();
        }

        private void Klanten_Click(object sender, RoutedEventArgs e)
        {
            CompanyCustomers customers = new CompanyCustomers(_userId);
            customers.Show();
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