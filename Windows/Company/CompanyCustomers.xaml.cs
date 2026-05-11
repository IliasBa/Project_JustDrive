using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Company
{
    public partial class CompanyCustomers : Window
    {
        private int _userId;

        public CompanyCustomers(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadKlanten();
        }

        private void LoadKlanten()
        {
            var klanten = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT DISTINCT 
                                    cu.UserId,
                                    CONCAT(cu.First_Name, ' ', cu.Last_Name) AS KlantNaam,
                                    u.Email
                                 FROM customer cu
                                 INNER JOIN user u ON cu.UserId = u.User_Id
                                 INNER JOIN reservation r ON cu.UserId = r.CustomerId
                                 INNER JOIN car c ON r.CarId = c.Id
                                 WHERE c.CompanyId = @id
                                 ORDER BY cu.Last_Name";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    klanten.Add(new
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        KlantNaam = reader["KlantNaam"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }
            }

            KlantenList.ItemsSource = klanten;
        }

        private void KlantenList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KlantenList.SelectedItem == null) return;

            dynamic klant = KlantenList.SelectedItem;
            TxtKlantNaam.Text = klant.KlantNaam;
            TxtKlantEmail.Text = klant.Email;

            LoadGeschiedenis(klant.UserId);
        }

        private void LoadGeschiedenis(int klantId)
        {
            var geschiedenis = new List<dynamic>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                                    CONCAT(c.Car_Brand, ' ', c.Model) AS AutoNaam,
                                    r.Start_date AS StartDate,
                                    r.End_date AS EndDate,
                                    r.Total_price AS TotalPrice
                                 FROM reservation r
                                 INNER JOIN car c ON r.CarId = c.Id
                                 WHERE r.CustomerId = @klantId AND c.CompanyId = @companyId
                                 ORDER BY r.Start_date DESC";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@klantId", klantId);
                cmd.Parameters.AddWithValue("@companyId", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                    DateTime endDate = Convert.ToDateTime(reader["EndDate"]);
                    string status = endDate < DateTime.Now ? "Afgerond" : "Actief";

                    geschiedenis.Add(new
                    {
                        AutoNaam = reader["AutoNaam"].ToString(),
                        StartDate = startDate.ToString("dd/MM/yyyy"),
                        EndDate = endDate.ToString("dd/MM/yyyy"),
                        TotalPrice = reader["TotalPrice"].ToString(),
                        Status = status
                    });
                }
            }

            GeschiedenisGrid.ItemsSource = geschiedenis;
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