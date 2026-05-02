using Project_JustDrive;
using System.Windows;

namespace Project_JustDrive.Windows.Clients
{
    public partial class Dashboard : Window
    {
        private int _userId;

        public Dashboard(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        public Dashboard()
        {
        }

        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Auto's zoeken komt binnenkort!");
        }
        private void BtnHuurAuto_Click(object sender, RoutedEventArgs e)
        {
            RentCar rentCarWindow = new RentCar();
            rentCarWindow.Show();
            this.Close();
        }

        private void BtnReservaties_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reservaties komt binnenkort!");
        }

        private void BtnFavorieten_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Favorieten komt binnenkort!");
        }

        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profiel komt binnenkort!");
        }

        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}