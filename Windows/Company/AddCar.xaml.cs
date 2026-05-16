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
            LoadBrands();
        }

        private void LoadBrands()
        {
            CmbMerk.Items.Clear();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT DISTINCT Brand FROM carname ORDER BY Brand";
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    CmbMerk.Items.Add(reader["Brand"].ToString());
            }
        }

        private void LoadModels(string brand)
        {
            CmbModel.Items.Clear();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Model FROM carname WHERE Brand = @brand ORDER BY Model";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@brand", brand);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    CmbModel.Items.Add(reader["Model"].ToString());
            }
        }

        private void CmbMerk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMerk.SelectedItem != null)
                LoadModels(CmbMerk.SelectedItem.ToString());
        }

        private void BtnNieuwMerk_Click(object sender, RoutedEventArgs e)
        {
            PanelNieuwMerk.Visibility = PanelNieuwMerk.Visibility == Visibility.Collapsed
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnMerkToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtNieuwMerk.Text)) return;

            string newBrand = TxtNieuwMerk.Text.Trim();
            CmbMerk.Items.Add(newBrand);
            CmbMerk.SelectedItem = newBrand;
            TxtNieuwMerk.Text = "";
            PanelNieuwMerk.Visibility = Visibility.Collapsed;
        }

        private void BtnNieuwModel_Click(object sender, RoutedEventArgs e)
        {
            PanelNieuwModel.Visibility = PanelNieuwModel.Visibility == Visibility.Collapsed
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnModelToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtNieuwModel.Text)) return;

            string newModel = TxtNieuwModel.Text.Trim();
            CmbModel.Items.Add(newModel);
            CmbModel.SelectedItem = newModel;
            TxtNieuwModel.Text = "";
            PanelNieuwModel.Visibility = Visibility.Collapsed;
        }

        private int GetOrCreateCarNameId(MySqlConnection conn, string brand, string model)
        {
            string checkQuery = "SELECT Id FROM carname WHERE Brand = @brand AND Model = @model";
            var checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@brand", brand);
            checkCmd.Parameters.AddWithValue("@model", model);
            var result = checkCmd.ExecuteScalar();

            if (result != null)
                return Convert.ToInt32(result);

            string insertQuery = "INSERT INTO carname (Brand, Model) VALUES (@brand, @model)";
            var insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@brand", brand);
            insertCmd.Parameters.AddWithValue("@model", model);
            insertCmd.ExecuteNonQuery();

            return Convert.ToInt32(insertCmd.LastInsertedId);
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (CmbMerk.SelectedItem == null || CmbModel.SelectedItem == null ||
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

                    int carNameId = GetOrCreateCarNameId(conn,
                        CmbMerk.SelectedItem.ToString(),
                        CmbModel.SelectedItem.ToString());

                    string query = @"INSERT INTO car 
                                    (CarNameId, TYPE, Fuel, Transmission, 
                                     Price_Per_Day, Deposit, Price_Per_100km, LicensePlate, CompanyId)
                                    VALUES 
                                    (@carNameId, @type, @brandstof, @transmissie,
                                     @prijs, @borg, @verbruik, @nummerplaat, @companyId)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@carNameId", carNameId);
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