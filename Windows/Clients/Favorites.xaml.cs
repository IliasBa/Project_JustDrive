using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Project_JustDrive.Services;

namespace Project_JustDrive.Windows.Clients
{
    public partial class Favorieten : Window
    {
        private int _userId;
        private List<Car> _favorieten = new List<Car>();

        public Favorieten(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadFavorieten();

            txtProfileInitials.Text = $"{Session.CurrentCustomer.FirstName[0]}{Session.CurrentCustomer.LastName[0]}";
            txtProfileName.Text = $"{Session.CurrentCustomer.FirstName} {Session.CurrentCustomer.LastName}";
            txtProfileEmail.Text = Session.CurrentUser.Email;
        }

        private void LoadFavorieten()
        {
            _favorieten = new List<Car>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT c.*, cn.Brand, cn.Model 
                         FROM car c
                         INNER JOIN favorite f ON c.Id = f.CarId
                         JOIN carname cn ON cn.Id = c.CarNameId
                         WHERE f.UserId = @userId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _favorieten.Add(new Car
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarBrand = reader["Brand"].ToString(),  
                        Model = reader["Model"].ToString(),     
                        Type = reader["TYPE"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        ImagePath = reader["Image_Path"] == DBNull.Value ? null : reader["Image_Path"].ToString()
                    });
                }
            }

            FavorietenList.ItemsSource = _favorieten;

            if (_favorieten.Count == 0)
                TxtLeeg.Visibility = Visibility.Visible;
        }

        private void BtnBekijk_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Car auto = btn.DataContext as Car;
            CarDetail detail = new CarDetail(auto.Id, _userId);
            detail.Show();
            this.Close();
        }

        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Car auto = btn.DataContext as Car;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM favorite WHERE UserId = @userId AND CarId = @carId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", _userId);
                cmd.Parameters.AddWithValue("@carId", auto.Id);
                cmd.ExecuteNonQuery();
            }

            LoadFavorieten();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(_userId);
            dashboard.Show();
            this.Close();
        }

        private void HuurAuto_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCar = new RentCar(_userId);
            rentCar.Show();
            this.Close();
        }

        private void Uitloggen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Profiel_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile();
            profile.Show();
            this.Close();
        }
    }
}