using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace Project_JustDrive.Windows
{
    public partial class ForgotPassword : Window
    {
        private string _verificationCode;
        private string _userEmail;

        // ⚠️ Vul later in met Combell gegevens
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 465;
        private const string SMTP_EMAIL = "iliasbaaouch@gmail.com";
        private const string SMTP_PASSWORD = "ztez eiym kljr dtte";

        public ForgotPassword()
        {
            InitializeComponent();
        }

        private async void StuurCode_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtEmail.Text))
            {
                MessageBox.Show("Voer je e-mailadres in.");
                return;
            }

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM user WHERE Email = @email";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", TxtEmail.Text);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    MessageBox.Show("Dit e-mailadres is niet gekend in ons systeem.");
                    return;
                }
            }

            _userEmail = TxtEmail.Text;
            _verificationCode = new Random().Next(100000, 999999).ToString();

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var mail = new MailMessage();
                    mail.From = new MailAddress(SMTP_EMAIL, "JustDrive");
                    mail.To.Add(_userEmail);
                    mail.Subject = "JustDrive - Verificatiecode";
                    mail.Body = $"Hallo,\n\nJe verificatiecode is: {_verificationCode}";

                    var smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(SMTP_EMAIL, SMTP_PASSWORD),
                        EnableSsl = true
                    };

                    smtp.Send(mail);
                });

                StapEmail.Visibility = Visibility.Collapsed;
                StapCode.Visibility = Visibility.Visible;
                TxtCodeInfo.Text = $"We hebben een verificatiecode gestuurd naar {_userEmail}.";
                MessageBox.Show("Verificatiecode verstuurd!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void BevestigCode_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCode.Text))
            {
                MessageBox.Show("Voer de verificatiecode in.");
                return;
            }

            if (TxtCode.Text != _verificationCode)
            {
                MessageBox.Show("Verkeerde code. Probeer opnieuw.");
                return;
            }

            // Ga naar stap 3
            StapCode.Visibility = Visibility.Collapsed;
            StapNieuwWachtwoord.Visibility = Visibility.Visible;
        }

        private void OpslaanWachtwoord_Click(object sender, RoutedEventArgs e)
        {
            if (TxtNieuwWachtwoord.Password.Length == 0 || TxtBevestigWachtwoord.Password.Length == 0)
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            if (TxtNieuwWachtwoord.Password != TxtBevestigWachtwoord.Password)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE user SET Password = @password WHERE Email = @email";
                    var cmd = new MySqlCommand(query, conn);
                    string hashedPassword = PasswordHelper.Hash(TxtNieuwWachtwoord.Password);

                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@email", _userEmail);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Wachtwoord succesvol gewijzigd! Je kan nu inloggen.");
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void TerugNaarEmail_Click(object sender, RoutedEventArgs e)
        {
            StapCode.Visibility = Visibility.Collapsed;
            StapEmail.Visibility = Visibility.Visible;
        }

        private void TerugLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}