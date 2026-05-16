using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Company
{
    public partial class CompanyCars : Window
    {
        private int _userId;

        public CompanyCars(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadCars();
        }

        private void LoadCars()
        {
            var cars = new List<Car>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT c.*, cn.Brand, cn.Model 
                         FROM car c
                         JOIN carname cn ON cn.Id = c.CarNameId
                         WHERE c.CompanyId = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cars.Add(new Car
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarBrand = reader["Brand"].ToString(),
                        Model = reader["Model"].ToString(),
                        Type = reader["TYPE"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        Deposit = Convert.ToDecimal(reader["Deposit"])
                    });
                }
            }

            CarsGrid.ItemsSource = cars;
        }

        private void Verwijderen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Car auto = btn.DataContext as Car;

            MessageBoxResult result = MessageBox.Show(
                $"Ben je zeker dat je {auto.CarBrand} {auto.Model} wil verwijderen?",
                "Bevestigen", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM car WHERE Id = @id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", auto.Id);
                    cmd.ExecuteNonQuery();
                }
                LoadCars();
            }
        }

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            AddCar addCar = new AddCar(_userId);
            addCar.ShowDialog();
            LoadCars();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            CompanyDashboard dashboard = new CompanyDashboard(_userId);
            dashboard.Show();
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