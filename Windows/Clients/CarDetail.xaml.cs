using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project_JustDrive.Windows.Clients
{
    public partial class CarDetail : Window
    {
        private int _carId;
        private int _userId;
        private decimal _pricePerDay;
        private byte[] _imageData;

        public CarDetail(int carId, int userId)
        {
            InitializeComponent();
            _carId = carId;
            _userId = userId;
            LoadCarDetails();
            CheckIsFavoriet();

            DpStart.SelectedDateChanged += DateChanged;
            DpEnd.SelectedDateChanged += DateChanged;

            BlockBookedDates();
            this.Loaded += CarDetail_Loaded;
        }

        private void LoadCarDetails()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT c.*, cn.Brand, cn.Model 
                         FROM car c
                         JOIN carname cn ON cn.Id = c.CarNameId
                         WHERE c.Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _carId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    _pricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]);
                    TxtNaam.Text = reader["Brand"] + " " + reader["Model"];
                    TxtType.Text = reader["TYPE"].ToString();
                    TxtBrandstof.Text = reader["Fuel"].ToString();
                    TxtTransmissie.Text = reader["Transmission"].ToString();
                    TxtBorg.Text = "€" + reader["Deposit"].ToString();
                    TxtVerbruik.Text = reader["Price_Per_100km"] + " L/100km";
                    TxtNummerplaat.Text = reader["LicensePlate"].ToString();
                    TxtPrijs.Text = "€" + _pricePerDay;

                    _imageData = reader["Image_Data"] == DBNull.Value ? null : (byte[])reader["Image_Data"];    
                }
            }
        }
        private void CarDetail_Loaded(object sender, RoutedEventArgs e)
        {
            ImgCar.Source = ImageHelper.LoadFromBytes(_imageData); // ← changed
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DpStart.SelectedDate != null)
            {
                DpEnd.DisplayDateStart = DpStart.SelectedDate.Value.AddDays(1);
            }

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

            if (IsCarAlreadyBooked(DpStart.SelectedDate.Value, DpEnd.SelectedDate.Value))
            {
                MessageBox.Show("Deze auto is al gereserveerd voor de geselecteerde periode.");
                return;
            }

            decimal total = duration.Days * _pricePerDay;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Insert reservation
                    string query = @"INSERT INTO reservation (Start_date, End_date, Total_price, CustomerId, CarId)
                             VALUES (@start, @end, @total, @customerId, @carId)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@start", DpStart.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@end", DpEnd.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@customerId", _userId);
                    cmd.Parameters.AddWithValue("@carId", _carId);
                    cmd.ExecuteNonQuery();

                    // Get reservation Id
                    int reservationId = Convert.ToInt32(cmd.LastInsertedId);

                    // Get company data
                    string companyQuery = @"SELECT co.Company_Name, u.Adres, u.Postcode, u.City, 
                                           u.Phone_Number, u.Email, co.IBAN
                                    FROM car c
                                    JOIN company co ON co.UserId = c.CompanyId
                                    JOIN user u ON u.User_Id = c.CompanyId
                                    WHERE c.Id = @carId";
                    var companyCmd = new MySqlCommand(companyQuery, conn);
                    companyCmd.Parameters.AddWithValue("@carId", _carId);
                    var companyReader = companyCmd.ExecuteReader();

                    string companyName = "", companyAdres = "", companyPostcode = "",
                           companyCity = "", companyPhone = "", companyEmail = "", companyIban = "";

                    if (companyReader.Read())
                    {
                        companyName = companyReader["Company_Name"].ToString();
                        companyAdres = companyReader["Adres"].ToString();
                        companyPostcode = companyReader["Postcode"].ToString();
                        companyCity = companyReader["City"].ToString();
                        companyPhone = companyReader["Phone_Number"].ToString();
                        companyEmail = companyReader["Email"].ToString();
                        companyIban = companyReader["IBAN"].ToString();
                    }
                    companyReader.Close();

                    MessageBox.Show($"Reservatie geplaatst! Totaal: €{total}");

                    // Send confirmation email
                    EmailService.SendReservationConfirmation(
                        Session.CurrentUser.Email,
                        Session.CurrentCustomer.FirstName,
                        TxtNaam.Text,
                        DpStart.SelectedDate.Value,
                        DpEnd.SelectedDate.Value,
                        total,

                        reservationId,
                        companyName,
                        companyAdres,
                        companyPostcode,
                        companyCity,
                        companyPhone,
                        companyEmail,
                        companyIban
                        
                    );

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
        private bool IsCarAlreadyBooked(DateTime start, DateTime end)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Check if any existing reservation overlaps with the selected dates
                string query = @"SELECT COUNT(*) FROM reservation 
                         WHERE CarId = @carId
                         AND NOT (End_date <= @start OR Start_date >= @end)";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@carId", _carId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCar = new RentCar(_userId);
            rentCar.Show();
            this.Close();
        }

        private void Favoriet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM favorite WHERE UserId = @userId AND CarId = @carId";
                    var checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@userId", _userId);
                    checkCmd.Parameters.AddWithValue("@carId", _carId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        string deleteQuery = "DELETE FROM favorite WHERE UserId = @userId AND CarId = @carId";
                        var deleteCmd = new MySqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@userId", _userId);
                        deleteCmd.Parameters.AddWithValue("@carId", _carId);
                        deleteCmd.ExecuteNonQuery();

                        BtnFavoriet.Content = "♥  Toevoegen aan favorieten";
                        MessageBox.Show("Verwijderd uit favorieten.");
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO favorite (UserId, CarId) VALUES (@userId, @carId)";
                        var insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@userId", _userId);
                        insertCmd.Parameters.AddWithValue("@carId", _carId);
                        insertCmd.ExecuteNonQuery();

                        BtnFavoriet.Content = "💔  Verwijderen uit favorieten";
                        MessageBox.Show("Toegevoegd aan favorieten!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }
        private void CheckIsFavoriet()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM favorite WHERE UserId = @userId AND CarId = @carId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", _userId);
                cmd.Parameters.AddWithValue("@carId", _carId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                    BtnFavoriet.Content = "💔  Verwijderen uit favorieten";
            }
        }
        private void BlockBookedDates()
        {
            // Grey out past dates
            DpStart.DisplayDateStart = DateTime.Today;
            DpEnd.DisplayDateStart = DateTime.Today;

            // Block already booked dates with blackout
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = "SELECT Start_date, End_date FROM reservation WHERE CarId = @carId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@carId", _carId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime start = Convert.ToDateTime(reader["Start_date"]);
                    DateTime end = Convert.ToDateTime(reader["End_date"]);

                    for (DateTime date = start; date <= end; date = date.AddDays(1))
                    {
                        DpStart.BlackoutDates.Add(new CalendarDateRange(date, date));
                        DpEnd.BlackoutDates.Add(new CalendarDateRange(date, date));
                    }
                }
            }
        }
    }
}