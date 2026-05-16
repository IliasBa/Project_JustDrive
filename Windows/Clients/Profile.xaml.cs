using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using Project_JustDrive.Models;
using Project_JustDrive.Services;

namespace Project_JustDrive.Windows.Clients
{
    public partial class Profile : Window
    {
        private int _userid = Session.CurrentUser.UserId;

        public Profile()
        {
            InitializeComponent();
            LoadProfileData();

            txtProfileInitials.Text = $"{Session.CurrentCustomer.FirstName[0]}{Session.CurrentCustomer.LastName[0]}";
            txtProfileName.Text = $"{Session.CurrentCustomer.FirstName} {Session.CurrentCustomer.LastName}";
            txtProfileEmail.Text = Session.CurrentUser.Email;
        }

        private void LoadProfileData()
        {
            var customer = Session.CurrentCustomer;
            var user = Session.CurrentUser;

            txtFirstName.Text = customer.FirstName;
            txtLastName.Text = customer.LastName;
            txtEmail.Text = user.Email;
            dpBirthday.SelectedDate = customer.Birthday;
            txtLicence.Text = customer.LicenceNumber;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Phone_Number, Adres, Postcode, City FROM user WHERE User_Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", user.UserId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtPhone.Text = reader["Phone_Number"].ToString();
                    txtAdres.Text = reader["Adres"].ToString();
                    txtPostcode.Text = reader["Postcode"].ToString();
                    txtCity.Text = reader["City"].ToString();
                }
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) ||
                string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Vul alle verplichte velden in.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string userQuery;
                    if (!string.IsNullOrEmpty(txtPassword.Password))
                    {
                        string hashedPassword = PasswordHelper.Hash(txtPassword.Password);
                        userQuery = @"UPDATE user SET Email = @email, Phone_Number = @phone, 
                                      Adres = @adres, Postcode = @postcode, City = @city, 
                                      Password = @password
                                      WHERE User_Id = @id";
                        var userCmd = new MySqlCommand(userQuery, conn);
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        userCmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        userCmd.Parameters.AddWithValue("@adres", txtAdres.Text);
                        userCmd.Parameters.AddWithValue("@postcode", txtPostcode.Text);
                        userCmd.Parameters.AddWithValue("@city", txtCity.Text);
                        userCmd.Parameters.AddWithValue("@password", hashedPassword);
                        userCmd.Parameters.AddWithValue("@id", Session.CurrentUser.UserId);
                        userCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        userQuery = @"UPDATE user SET Email = @email, Phone_Number = @phone, 
                                      Adres = @adres, Postcode = @postcode, City = @city
                                      WHERE User_Id = @id";
                        var userCmd = new MySqlCommand(userQuery, conn);
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        userCmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        userCmd.Parameters.AddWithValue("@adres", txtAdres.Text);
                        userCmd.Parameters.AddWithValue("@postcode", txtPostcode.Text);
                        userCmd.Parameters.AddWithValue("@city", txtCity.Text);
                        userCmd.Parameters.AddWithValue("@id", Session.CurrentUser.UserId);
                        userCmd.ExecuteNonQuery();
                    }

                    // Update customer table
                    string customerQuery = @"UPDATE customer SET First_Name = @firstName, 
                                             Last_Name = @lastName, Birthday = @birthday, 
                                             Licence_Number = @licence
                                             WHERE UserId = @id";

                    var customerCmd = new MySqlCommand(customerQuery, conn);
                    customerCmd.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                    customerCmd.Parameters.AddWithValue("@lastName", txtLastName.Text);
                    customerCmd.Parameters.AddWithValue("@birthday", dpBirthday.SelectedDate);
                    customerCmd.Parameters.AddWithValue("@licence", txtLicence.Text);
                    customerCmd.Parameters.AddWithValue("@id", Session.CurrentUser.UserId);
                    customerCmd.ExecuteNonQuery();

                    // Update session
                    Session.CurrentCustomer.FirstName = txtFirstName.Text;
                    Session.CurrentCustomer.LastName = txtLastName.Text;
                    Session.CurrentCustomer.Birthday = dpBirthday.SelectedDate ?? DateTime.Now;
                    Session.CurrentCustomer.LicenceNumber = txtLicence.Text;
                    Session.CurrentUser.Email = txtEmail.Text;
                    Session.CurrentUser.PhoneNumber = txtPhone.Text;
                    Session.CurrentUser.Adres = txtAdres.Text;
                    Session.CurrentUser.Postcode = txtPostcode.Text;
                    Session.CurrentUser.City = txtCity.Text;

                    MessageBox.Show("Profiel succesvol opgeslagen!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(Session.CurrentUser.UserId);
            dashboard.Show();
            this.Close();
        }

        private void HuurAuto_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCar = new RentCar(Session.CurrentUser.UserId);
            rentCar.Show();
            this.Close();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            Favorieten favorite = new Favorieten(_userid);
            favorite.Show();
            this.Close();
        }

        private void Uitloggen_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            Session.CurrentCustomer = null;
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}