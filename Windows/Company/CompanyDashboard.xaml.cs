using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Project_JustDrive.Windows.Company
{
    public partial class CompanyDashboard : Window
    {
        private int _userId;

        public CompanyDashboard(int userId)
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

                // Bedrijfsnaam en email 
                string userQuery = "SELECT c.Company_Name, u.Email FROM company c INNER JOIN user u ON c.UserId = u.User_Id WHERE c.UserId = @id";
                var userCmd = new MySqlCommand(userQuery, conn);
                userCmd.Parameters.AddWithValue("@id", _userId);
                var userReader = userCmd.ExecuteReader();
                if (userReader.Read())
                {
                    string naam = userReader["Company_Name"].ToString();
                    TxtBedrijfNaam.Text = naam;
                    TxtInitiaal.Text = naam.Substring(0, 1).ToUpper();
                    TxtEmail.Text = userReader["Email"].ToString();
                    TxtWelkom.Text = $"Welkom, {naam}";
                }
                userReader.Close();

                // Totaal auto's
                string autosQuery = "SELECT COUNT(*) FROM car WHERE CompanyId = @id";
                var autosCmd = new MySqlCommand(autosQuery, conn);
                autosCmd.Parameters.AddWithValue("@id", _userId);
                TxtTotaalAutos.Text = autosCmd.ExecuteScalar().ToString();

                // Actieve reservaties
                string resQuery = @"SELECT COUNT(*) FROM reservation r 
                                    INNER JOIN car c ON r.CarId = c.Id 
                                    WHERE c.CompanyId = @id AND r.End_date >= CURDATE()";
                var resCmd = new MySqlCommand(resQuery, conn);
                resCmd.Parameters.AddWithValue("@id", _userId);
                TxtActieveReservaties.Text = resCmd.ExecuteScalar().ToString();

                // Schaderapport
                string schadeQuery = @"SELECT COUNT(*) FROM damagereport d
                                       INNER JOIN reservation r ON d.ReservationId = r.Id
                                       INNER JOIN car c ON r.CarId = c.Id
                                       WHERE c.CompanyId = @id";
                var schadeCmd = new MySqlCommand(schadeQuery, conn);
                schadeCmd.Parameters.AddWithValue("@id", _userId);
                TxtSchade.Text = schadeCmd.ExecuteScalar().ToString();

                // Totale omzet
                string omzetQuery = @"SELECT COALESCE(SUM(r.Total_price), 0) FROM reservation r
                                      INNER JOIN car c ON r.CarId = c.Id
                                      WHERE c.CompanyId = @id";
                var omzetCmd = new MySqlCommand(omzetQuery, conn);
                omzetCmd.Parameters.AddWithValue("@id", _userId);
                TxtOmzet.Text = "€" + omzetCmd.ExecuteScalar().ToString();

                // Recente reservaties
                string recentQuery = @"SELECT r.Id as ReservationId,
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
                               ORDER BY r.Start_date DESC
                               LIMIT 10";
                var recentCmd = new MySqlCommand(recentQuery, conn);
                recentCmd.Parameters.AddWithValue("@id", _userId);
                var recentReader = recentCmd.ExecuteReader();

                var reservaties = new List<dynamic>();
                while (recentReader.Read())
                {
                    reservaties.Add(new
                    {
                        ReservatieId = recentReader["ReservationId"].ToString(),
                        KlantNaam = recentReader["KlantNaam"].ToString(),
                        AutoNaam = recentReader["AutoNaam"].ToString(),
                        StartDate = Convert.ToDateTime(recentReader["StartDate"]).ToString("dd/MM/yyyy"),
                        EndDate = Convert.ToDateTime(recentReader["EndDate"]).ToString("dd/MM/yyyy"),
                        TotalPrice = recentReader["TotalPrice"].ToString()
                    });
                }
                RecenteReservaties.ItemsSource = reservaties;
            }
        }

        private void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"JustDrive_Export_{DateTime.Now:yyyyMMdd}",
                    DefaultExt = ".csv",
                    Filter = "CSV files (*.csv)|*.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        string query = @"SELECT 
                                            r.Id AS ReservatieId,
                                            CONCAT(cu.First_Name, ' ', cu.Last_Name) AS Klant,
                                            u.Email AS KlantEmail,
                                            CONCAT(cn.Brand, ' ', cn.Model) AS Auto,
                                            c.TYPE AS Type,
                                            c.Fuel AS Brandstof,
                                            c.Transmission AS Transmissie,
                                            r.Start_date AS Startdatum,
                                            r.End_date AS Einddatum,
                                            DATEDIFF(r.End_date, r.Start_date) AS AantalDagen,
                                            c.Price_Per_Day AS PrijsPerDag,
                                            r.Total_price AS TotaalPrijs
                                        FROM reservation r
                                        JOIN car c ON c.Id = r.CarId
                                        JOIN carname cn ON cn.Id = c.CarNameId
                                        JOIN customer cu ON cu.UserId = r.CustomerId
                                        JOIN user u ON u.User_Id = cu.UserId
                                        WHERE c.CompanyId = @companyId
                                        ORDER BY r.Start_date DESC";

                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@companyId", _userId);
                        var reader = cmd.ExecuteReader();

                        var sb = new System.Text.StringBuilder();

                        sb.AppendLine("ReservatieId;Klant;KlantEmail;Auto;Type;Brandstof;" +
                                      "Transmissie;Startdatum;Einddatum;AantalDagen;" +
                                      "PrijsPerDag;TotaalPrijs");

                        while (reader.Read())
                        {
                            sb.AppendLine(
                                $"{reader["ReservatieId"]};" +
                                $"{reader["Klant"]};" +
                                $"{reader["KlantEmail"]};" +
                                $"{reader["Auto"]};" +
                                $"{reader["Type"]};" +
                                $"{reader["Brandstof"]};" +
                                $"{reader["Transmissie"]};" +
                                $"{Convert.ToDateTime(reader["Startdatum"]):dd/MM/yyyy};" +
                                $"{Convert.ToDateTime(reader["Einddatum"]):dd/MM/yyyy};" +
                                $"{reader["AantalDagen"]};" +
                                $"{reader["PrijsPerDag"]};" +
                                $"{reader["TotaalPrijs"]};"
                            );
                        }

                        System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString(),
                            System.Text.Encoding.UTF8);

                        MessageBox.Show($"Data succesvol geëxporteerd naar:\n{saveDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij exporteren: {ex.Message}");
            }
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