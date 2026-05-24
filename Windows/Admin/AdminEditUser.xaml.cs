using JustDrive.Database;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace Project_JustDrive.Windows.Admin
{
    public partial class AdminEditUser : Window
    {
        private int _userId;

        public AdminEditUser(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUser();
        }

        private void LoadUser()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Email, Phone_Number, Adres, City FROM user WHERE User_Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _userId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    TxtEmail.Text = reader["Email"].ToString();
                    TxtTelefoon.Text = reader["Phone_Number"].ToString();
                    TxtAdres.Text = reader["Adres"].ToString();
                    TxtStad.Text = reader["City"].ToString();
                }
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtEmail.Text))
            {
                MessageBox.Show("Email mag niet leeg zijn.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query;
                    MySqlCommand cmd;

                    if (!string.IsNullOrEmpty(TxtWachtwoord.Text))
                    {
                        query = @"UPDATE user SET Email = @email, Phone_Number = @telefoon, 
                                  Adres = @adres, City = @stad, PASSWORD = @wachtwoord
                                  WHERE User_Id = @id";
                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@wachtwoord", TxtWachtwoord.Text);
                    }
                    else
                    {
                        query = @"UPDATE user SET Email = @email, Phone_Number = @telefoon,
                                  Adres = @adres, City = @stad
                                  WHERE User_Id = @id";
                        cmd = new MySqlCommand(query, conn);
                    }

                    cmd.Parameters.AddWithValue("@email", TxtEmail.Text);
                    cmd.Parameters.AddWithValue("@telefoon", TxtTelefoon.Text);
                    cmd.Parameters.AddWithValue("@adres", TxtAdres.Text);
                    cmd.Parameters.AddWithValue("@stad", TxtStad.Text);
                    cmd.Parameters.AddWithValue("@id", _userId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Gebruiker succesvol bijgewerkt!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}