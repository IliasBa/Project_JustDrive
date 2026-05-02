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

namespace Project_JustDrive.Windows.Clients
{
    /// <summary>
    /// Interaction logic for RentCar.xaml
    /// </summary>
    public partial class RentCar : Window
    {
        public List<Car> Cars { get; set; }

        public RentCar()
        {
            InitializeComponent();

            LoadCarsFromDatabase();

            DataContext = this;
        }

        // 👇 HIER komt die methode
        private void LoadCarsFromDatabase()
        {
            Cars = new List<Car>();

            using (MySqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM car";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Cars.Add(new Car
                    {
                        CarBrand = reader["Car_Brand"].ToString(),
                        Type = reader["TYPE"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"])
                    });
                }
            }
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is Dashboard)
                {
                    w.Show();
                    break;
                }
            }

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
