using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project_JustDrive.Services;

namespace Project_JustDrive.Windows.Clients
{

    public partial class RentCar : Window
    {
        public List<Car> Cars { get; set; }
        private List<Car> AllCars = new List<Car>();
        private int _userId;
        private string ActiveFilter = "Alle";
        private List<int> _selectedCarIds = new List<int>();

        public RentCar(int userId)
        {
            InitializeComponent();

            LoadCarsFromDatabase();

            txtProfileEmaill.Text = Session.CurrentUser.Email;
            txtProfileName.Text = Session.CurrentCustomer.FirstName + " " + Session.CurrentCustomer.LastName;
            txtProfileInitials.Text = $"{Session.CurrentCustomer.FirstName[0]}{Session.CurrentCustomer.LastName[0]}";

            DataContext = this;
            _userId = userId;
        }

        private void LoadCarsFromDatabase()
        {
            AllCars = new List<Car>();

            using (MySqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"SELECT c.*, cn.Brand, cn.Model 
                         FROM car c
                         JOIN carname cn ON cn.Id = c.CarNameId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AllCars.Add(new Car
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarBrand = reader["Brand"].ToString(),      // ← was Car_Brand
                        Model = reader["Model"].ToString(),         // ← now from carname
                        Type = reader["TYPE"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        CompanyId = Convert.ToInt32(reader["CompanyId"])
                    });
                }
            }
            Cars = new List<Car>(AllCars);
        }

        private void ApplyFilters(string searchText = "")
        {
            var filtered = AllCars.AsEnumerable();

            // Filter op type
            if (ActiveFilter != "Alle")
            {
                if (ActiveFilter == "Elektrisch")
                    filtered = filtered.Where(c => c.Fuel == "Electric");
                else
                    filtered = filtered.Where(c => c.Type == ActiveFilter);
            }

            // Filter op zoekterm
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(c =>
                    c.CarBrand.ToLower().Contains(searchText.ToLower()) ||
                    c.Type.ToLower().Contains(searchText.ToLower()));
            }

            Cars = filtered.ToList();
            ItemsControlCars.ItemsSource = Cars;
        }

        private void FilterAlle_Click(object sender, RoutedEventArgs e)
        {
            ActiveFilter = "Alle";
            ApplyFilters(TxtZoek.Text);
        }
        private void FilterSport_Click(object sender, RoutedEventArgs e)
        {
            ActiveFilter = "Sport";
            ApplyFilters(TxtZoek.Text);
        }

        private void FilterSUV_Click(object sender, RoutedEventArgs e)
        {
            ActiveFilter = "SUV";
            ApplyFilters(TxtZoek.Text);
        }

        private void FilterElektrisch_Click(object sender, RoutedEventArgs e)
        {
            ActiveFilter = "Elektrisch";
            ApplyFilters(TxtZoek.Text);
        }

        private void FilterMinivan_Click(object sender, RoutedEventArgs e)
        {
            ActiveFilter = "Minivan";
            ApplyFilters(TxtZoek.Text);
        }

        private void TxtZoek_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(TxtZoek.Text);
        }
        private void HuurNu_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Car geselecteerdeAuto = btn.DataContext as Car;

            CarDetail detailWindow = new CarDetail(geselecteerdeAuto.Id, _userId);
            detailWindow.Show();
            this.Close();
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(_userId);
            dashboard.Show();
            this.Close();
        }
        private void Uitloggen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            Favorieten favorite = new Favorieten(_userId);
            favorite.Show();
            this.Close();
        }

        private void Profiel_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile();
            profile.Show();
            this.Close();
        }

        private void Vergelijk_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int carId = (int)button.Tag;

            if (_selectedCarIds.Contains(carId))
            {
                MessageBox.Show("Deze auto is al geselecteerd.");
                return;
            }

            if (_selectedCarIds.Count >= 3)
            {
                MessageBox.Show("Je kan maximum 3 auto's vergelijken.");
                return;
            }

            _selectedCarIds.Add(carId);

            if (_selectedCarIds.Count >= 2)
            {
                CompareWindow compare = new CompareWindow(_selectedCarIds);
                compare.Show();
                _selectedCarIds.Clear();
            }
            else
            {
                MessageBox.Show($"Auto geselecteerd! Selecteer nog {2 - _selectedCarIds.Count} auto('s) om te vergelijken.");
            }
        }
    }
}
