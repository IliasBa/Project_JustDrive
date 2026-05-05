using Project_JustDrive;
using System.Windows;
using Project_JustDrive.Services;

namespace Project_JustDrive.Windows.Clients
{
    public partial class Dashboard : Window
    {
        private int _userId;

        public Dashboard(int userId)
        {
            InitializeComponent();
            
            txtProfileEmaill.Text = Session.CurrentUser.Email;
            txtProfileName.Text = Session.CurrentCustomer.FirstName +" " + Session.CurrentCustomer.LastName;
            txtProfileInitials.Text = $"{Session.CurrentCustomer.FirstName[0]}{Session.CurrentCustomer.LastName[0]}";
            _userId = userId;

        }

        public Dashboard()
        {
        }

        private void Zoeken_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Auto's zoeken komt binnenkort!");
        }
        private void HuurAuto_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCarWindow = new RentCar(_userId);
            rentCarWindow.Show();
            this.Close();
        }

        private void Reservaties_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reservaties komt binnenkort!");
        }

        private void Favorieten_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Favorieten komt binnenkort!");
        }

        private void Profiel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profiel komt binnenkort!");
        }

        private void Uitloggen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}