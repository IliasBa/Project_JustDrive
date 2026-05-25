using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Project_JustDrive.Windows.Company
{
    public partial class CompanyReservations : Window
    {
        private int _userId;

        public CompanyReservations(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadReservations();
        }

        private void LoadReservations()
        {
            var reservations = new List<dynamic>();
            int actief = 0, afgerond = 0;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                    CONCAT(cu.First_Name, ' ', cu.Last_Name) AS KlantNaam,
                    CONCAT(cn.Brand, ' ', cn.Model) AS AutoNaam,
                    r.Start_date AS StartDate,
                    r.End_date AS EndDate,
                    r.Total_price AS TotalPrice
                FROM reservation r
                INNER JOIN car c ON r.CarId = c.Id
                JOIN carname cn ON cn.Id = c.CarNameId
                INNER JOIN customer cu ON r.CustomerId = cu.UserId
                WHERE c.CompanyId = @id
                ORDER BY r.Start_date DESC";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                    DateTime endDate = Convert.ToDateTime(reader["EndDate"]);
                    string status = endDate >= DateTime.Now ? "Actief" : "Afgerond";

                    if (status == "Actief") actief++;
                    else afgerond++;

                    reservations.Add(new
                    {
                        KlantNaam = reader["KlantNaam"].ToString(),
                        AutoNaam = reader["AutoNaam"].ToString(),
                        StartDate = startDate.ToString("dd/MM/yyyy"),
                        EndDate = endDate.ToString("dd/MM/yyyy"),
                        TotalPrice = reader["TotalPrice"].ToString(),
                        Status = status
                    });
                }

                TxtTotaal.Text = reservations.Count.ToString();
                TxtActief.Text = actief.ToString();
                TxtAfgerond.Text = afgerond.ToString();
                TxtAantal.Text = $"{reservations.Count} reservaties gevonden";
            }


            ReservationsPanel.ItemsSource = reservations;
            TxtTotaal.Text = reservations.Count.ToString();
            TxtActief.Text = actief.ToString();
            TxtAfgerond.Text = afgerond.ToString();
            TxtAantal.Text = $"{reservations.Count} reservaties gevonden";
            ReservationsPanel.ItemsSource = reservations;

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

        private void Klanten_Click(object sender, RoutedEventArgs e)
        {
            CompanyCustomers customers = new CompanyCustomers(_userId);
            customers.Show();
            this.Close();
        }

        private void Schade_Click(object sender, RoutedEventArgs e)
        {
            CompanyDamageReports damage = new CompanyDamageReports(_userId);
            damage.Show();
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