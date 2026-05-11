using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Project_JustDrive.Windows.Company
{
    public partial class AddCar : Window
    {
        private int _userId;

        public AddCar(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            // Validatie
            if (string.IsNullOrEmpty(TxtMerk.Text) || string.IsNullOrEmpty(TxtModel.Text) ||
                CmbType.SelectedItem == null || CmbBrandstof.SelectedItem == null ||
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
                    string query = @"INSERT INTO car 
                                    (Car_Brand, Model, TYPE, Fuel, Transmission, 
                                     Price_Per_Day, Deposit, Price_Per_100km, LicensePlate, CompanyId)
                                    VALUES 
                                    (@merk, @model, @type, @brandstof, @transmissie,
                                     @prijs, @borg, @verbruik, @nummerplaat, @companyId)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@merk", TxtMerk.Text);
                    cmd.Parameters.AddWithValue("@model", TxtModel.Text);
                    cmd.Parameters.AddWithValue("@type", (CmbType.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@brandstof", (CmbBrandstof.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@transmissie", (CmbTransmissie.SelectedItem as ComboBoxItem).Content.ToString());
                    cmd.Parameters.AddWithValue("@prijs", prijs);
                    cmd.Parameters.AddWithValue("@borg", borg);
                    cmd.Parameters.AddWithValue("@verbruik", verbruik);
                    cmd.Parameters.AddWithValue("@nummerplaat", TxtNummerplaat.Text);
                    cmd.Parameters.AddWithValue("@companyId", _userId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Auto succesvol toegevoegd!");
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