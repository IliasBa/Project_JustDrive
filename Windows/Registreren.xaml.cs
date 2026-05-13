using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Media;

namespace Project_JustDrive.Windows
{
    public partial class Registreren : Window
    {
        private bool _isKlant = true;

        public Registreren()
        {
            InitializeComponent();
        }

        private void ToggleKlant_Click(object sender, RoutedEventArgs e)
        {
            _isKlant = true;
            ToggleKlant.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F26B1F"));
            ToggleKlant.Foreground = Brushes.White;
            ToggleKlant.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F26B1F"));

            ToggleBedrijf.Background = Brushes.White;
            ToggleBedrijf.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            ToggleBedrijf.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));

            KlantVelden.Visibility = Visibility.Visible;
            BedrijfVelden.Visibility = Visibility.Collapsed;
        }

        private void ToggleBedrijf_Click(object sender, RoutedEventArgs e)
        {
            _isKlant = false;
            ToggleBedrijf.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F26B1F"));
            ToggleBedrijf.Foreground = Brushes.White;
            ToggleBedrijf.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F26B1F"));

            ToggleKlant.Background = Brushes.White;
            ToggleKlant.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            ToggleKlant.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));

            KlantVelden.Visibility = Visibility.Collapsed;
            BedrijfVelden.Visibility = Visibility.Visible;
        }

        private void Registreer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtEmail.Text) || string.IsNullOrEmpty(TxtTelefoon.Text) ||
                string.IsNullOrEmpty(TxtAdres.Text) || string.IsNullOrEmpty(TxtStad.Text) ||
                TxtPassword.Password.Length == 0)
            {
                MessageBox.Show("Vul alle verplichte velden in.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // User aanmaken
                    string role = _isKlant ? "customer" : "company";
                    string userQuery = @"INSERT INTO user (Email, PASSWORD, Phone_Number, Role, Adres, City)
                                        VALUES (@email, @password, @telefoon, @role, @adres, @stad)";
                    var userCmd = new MySqlCommand(userQuery, conn);
                    userCmd.Parameters.AddWithValue("@email", TxtEmail.Text);
                    userCmd.Parameters.AddWithValue("@password", TxtPassword.Password);
                    userCmd.Parameters.AddWithValue("@telefoon", TxtTelefoon.Text);
                    userCmd.Parameters.AddWithValue("@role", role);
                    userCmd.Parameters.AddWithValue("@adres", TxtAdres.Text);
                    userCmd.Parameters.AddWithValue("@stad", TxtStad.Text);
                    userCmd.ExecuteNonQuery();

                    // Nieuw userId ophalen
                    long newUserId = userCmd.LastInsertedId;

                    if (_isKlant)
                    {
                        if (string.IsNullOrEmpty(TxtVoornaam.Text) || string.IsNullOrEmpty(TxtAchternaam.Text) ||
                            DpGeboortedatum.SelectedDate == null || string.IsNullOrEmpty(TxtRijbewijs.Text))
                        {
                            MessageBox.Show("Vul alle klant velden in.");
                            return;
                        }

                        string customerQuery = @"INSERT INTO customer (UserId, First_Name, Last_Name, Birthday, Licence_Number)
                                                VALUES (@userId, @voornaam, @achternaam, @geboortedatum, @rijbewijs)";
                        var customerCmd = new MySqlCommand(customerQuery, conn);
                        customerCmd.Parameters.AddWithValue("@userId", newUserId);
                        customerCmd.Parameters.AddWithValue("@voornaam", TxtVoornaam.Text);
                        customerCmd.Parameters.AddWithValue("@achternaam", TxtAchternaam.Text);
                        customerCmd.Parameters.AddWithValue("@geboortedatum", DpGeboortedatum.SelectedDate.Value);
                        customerCmd.Parameters.AddWithValue("@rijbewijs", TxtRijbewijs.Text);
                        customerCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TxtBedrijfsnaam.Text) || string.IsNullOrEmpty(TxtBTW.Text))
                        {
                            MessageBox.Show("Vul alle bedrijf velden in.");
                            return;
                        }

                        string companyQuery = @"INSERT INTO company (UserId, Company_Name, VAT_number)
                                               VALUES (@userId, @naam, @btw)";
                        var companyCmd = new MySqlCommand(companyQuery, conn);
                        companyCmd.Parameters.AddWithValue("@userId", newUserId);
                        companyCmd.Parameters.AddWithValue("@naam", TxtBedrijfsnaam.Text);
                        companyCmd.Parameters.AddWithValue("@btw", TxtBTW.Text);
                        companyCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Account aangemaakt! Je kan nu inloggen.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}