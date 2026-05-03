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