using JustDrive.Database;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Input;

namespace Project_JustDrive
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT User_Id, Role FROM user WHERE Email = @email AND PASSWORD = @password";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string role = reader["Role"].ToString();
                        int userId = Convert.ToInt32(reader["User_Id"]);
                        reader.Close();

                        if (role == "customer")
                        {
                            Windows.Clients.Dashboard dashboard = new Windows.Clients.Dashboard(userId);
                            dashboard.Show();
                            this.Close();
                        }
                        else if (role == "company")
                        {
                            MessageBox.Show("Bedrijf dashboard komt binnenkort!");
                        }
                        else if (role == "admin")
                        {
                            MessageBox.Show("Admin dashboard komt binnenkort!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
            }
        }

        private void TxtRegister_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Registratie komt binnenkort!");
        }
    }
}