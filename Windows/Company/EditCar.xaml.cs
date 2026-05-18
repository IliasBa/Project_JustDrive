using JustDrive.Database;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project_JustDrive.Windows.Company
{
    public partial class EditCar : Window
    {
        private Car _car;
        private int _userId; 
        private string _selectedImagePath;

        public EditCar(Car car, int userId)
        {
            InitializeComponent();
            _car = car;
            _userId = userId;
            LoadCarData();
        }

        private void LoadCarData()
        {
            TxtMerk.Text = _car.CarBrand;
            TxtModel.Text = _car.Model;
            TxtPrijs.Text = _car.PricePerDay.ToString();
            TxtBorg.Text = _car.Deposit.ToString();
            TxtNummerplaat.Text = _car.LicensePlate;

            // Load PricePer100km from database
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Price_Per_100km FROM car WHERE Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _car.Id);
                var result = cmd.ExecuteScalar();
                TxtVerbruik.Text = result?.ToString();
            }

            // Set ComboBox values
            foreach (ComboBoxItem item in CmbType.Items)
                if (item.Content.ToString() == _car.Type)
                    CmbType.SelectedItem = item;

            foreach (ComboBoxItem item in CmbBrandstof.Items)
                if (item.Content.ToString() == _car.Fuel)
                    CmbBrandstof.SelectedItem = item;

            foreach (ComboBoxItem item in CmbTransmissie.Items)
                if (item.Content.ToString() == _car.Transmission)
                    CmbTransmissie.SelectedItem = item;
        }

        private void BtnFotoKiezen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                string brand = _car.CarBrand;
                string model = _car.Model.Replace(" ", "_");
                string fileName = $"{brand}_{model}.jpg";

                // Find the Images folder by going up from the executable
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectDir = System.IO.Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;
                string imagesFolder = System.IO.Path.Combine(projectDir, "Images");

                System.IO.Directory.CreateDirectory(imagesFolder);
                string destinationPath = System.IO.Path.Combine(imagesFolder, fileName);

                System.IO.File.Copy(dialog.FileName, destinationPath, true);

                _selectedImagePath = $"pack://application:,,,/Images/{fileName}";
                ImgPreview.Source = new BitmapImage(new Uri(destinationPath));
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (CmbType.SelectedItem == null || CmbBrandstof.SelectedItem == null ||
                CmbTransmissie.SelectedItem == null || string.IsNullOrEmpty(TxtPrijs.Text) ||
                string.IsNullOrEmpty(TxtBorg.Text) || string.IsNullOrEmpty(TxtNummerplaat.Text))
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            if (!decimal.TryParse(TxtPrijs.Text, out decimal prijs) ||
                !decimal.TryParse(TxtBorg.Text, out decimal borg) ||
                !decimal.TryParse(TxtVerbruik.Text, out decimal verbruik))
            {
                MessageBox.Show("Prijs, borg en verbruik moeten getallen zijn.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE car SET 
                                TYPE = @type, Fuel = @brandstof, Transmission = @transmissie,
                                Price_Per_Day = @prijs, Deposit = @borg, 
                                Price_Per_100km = @verbruik, LicensePlate = @nummerplaat,
                                Image_Path = @imagePath
                                WHERE Id = @id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@type", (CmbType.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@brandstof", (CmbBrandstof.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@transmissie", (CmbTransmissie.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@prijs", prijs);
                    cmd.Parameters.AddWithValue("@borg", borg);
                    cmd.Parameters.AddWithValue("@verbruik", verbruik);
                    cmd.Parameters.AddWithValue("@nummerplaat", TxtNummerplaat.Text);
                    cmd.Parameters.AddWithValue("@imagePath", _selectedImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", _car.Id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Auto succesvol bijgewerkt!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }
    }
}