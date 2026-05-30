using JustDrive.Database;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using Project_JustDrive.Services;
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
        private byte[] _selectedImageData;

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

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Price_Per_100km, Image_Data FROM car WHERE Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _car.Id);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    TxtVerbruik.Text = reader["Price_Per_100km"].ToString();

                    if (reader["Image_Data"] != DBNull.Value)
                    {
                        _selectedImageData = (byte[])reader["Image_Data"];
                        ImgPreview.Source = ImageHelper.LoadFromBytes(_selectedImageData);
                    }
                }
            }

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
                _selectedImageData = System.IO.File.ReadAllBytes(dialog.FileName);

                // Show preview
                var bitmap = new BitmapImage();
                using (var stream = new System.IO.MemoryStream(_selectedImageData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                ImgPreview.Source = bitmap;
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
                                    Image_Data = @imageData
                                    WHERE Id = @id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@type", (CmbType.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@brandstof", (CmbBrandstof.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@transmissie", (CmbTransmissie.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@prijs", prijs);
                    cmd.Parameters.AddWithValue("@borg", borg);
                    cmd.Parameters.AddWithValue("@verbruik", verbruik);
                    cmd.Parameters.AddWithValue("@nummerplaat", TxtNummerplaat.Text);
                    cmd.Parameters.AddWithValue("@imageData",_selectedImageData ?? (object)DBNull.Value);
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