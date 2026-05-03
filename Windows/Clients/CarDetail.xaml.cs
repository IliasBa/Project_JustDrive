using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace Project_JustDrive.Windows.Clients
{
    public partial class CarDetail : Window
    {
        private int _carId;
        private int _userId;
        private decimal _pricePerDay;

        public CarDetail(int carId, int userId)
        {
            InitializeComponent();
            _carId = carId;
            _userId = userId;
            LoadCarDetails();

            DpStart.SelectedDateChanged += DateChanged;
            DpEnd.SelectedDateChanged += DateChanged;
        }

        private void LoadCarDetails()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM car WHERE Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _carId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    _pricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]);
                    TxtNaam.Text = reader["Car_Brand"] + " " + reader["Model"];
                    TxtType.Text = reader["TYPE"].ToString();
                    TxtBrandstof.Text = reader["Fuel"].ToString();
                    TxtTransmissie.Text = reader["Transmission"].ToString();
                    TxtBorg.Text = "€" + reader["Deposit"].ToString();
                    TxtVerbruik.Text = reader["Price_Per_100km"] + " L/100km";
                    TxtNummerplaat.Text = reader["LicensePlate"].ToString();
                    TxtPrijs.Text = "€" + _pricePerDay;
                }
            }
        }

        private void DateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DpStart.SelectedDate != null && DpEnd.SelectedDate != null)
            {
                TimeSpan duration = DpEnd.SelectedDate.Value - DpStart.SelectedDate.Value;
                if (duration.Days > 0)
                {
                    decimal total = duration.Days * _pricePerDay;
                    TxtTotaal.Text = $"Totaal: €{total} ({duration.Days} dagen)";
                }
                else
                {
                    TxtTotaal.Text = "Retourdatum moet na ophaaldatum zijn.";
                }
            }
        }

        private void BtnReserveer_Click(object sender, RoutedEventArgs e)
        {
            if (DpStart.SelectedDate == null || DpEnd.SelectedDate == null)
            {
                MessageBox.Show("Kies een ophaal- en retourdatum.");
                return;
            }

            TimeSpan duration = DpEnd.SelectedDate.Value - DpStart.SelectedDate.Value;
            if (duration.Days <= 0)
            {
                MessageBox.Show("Retourdatum moet na ophaaldatum zijn.");
                return;
            }

            decimal total = duration.Days * _pricePerDay;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO reservation (Start_date, End_date, Total_price, CustomerId, CarId)
                                     VALUES (@start, @end, @total, @customerId, @carId)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@start", DpStart.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@end", DpEnd.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@customerId", _userId);
                    cmd.Parameters.AddWithValue("@carId", _carId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"Reservatie geplaatst! Totaal: €{total}");

                    RentCar rentCar = new RentCar(_userId);
                    rentCar.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCar = new RentCar(_userId);
            rentCar.Show();
            this.Close();
        }
    }
}