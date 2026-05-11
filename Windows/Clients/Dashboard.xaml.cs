using Project_JustDrive;
using Project_JustDrive.Services;
using System.Windows;
using System.Windows.Controls;
using static Project_JustDrive.Services.ReservationService;

namespace Project_JustDrive.Windows.Clients
{
    public partial class Dashboard : Window
    {
        private int _userId;
        private int CarId;
        private int _activeCarId;
        private int activeReservationId;

        public Dashboard(int userId)
        {
            InitializeComponent();
            LoadPreviousReservations(); 
            LoadActiveReservation();
            _userId = userId;

            // labels inladen
            txtProfileEmaill.Text = Session.CurrentUser.Email;
            txtProfileName.Text = Session.CurrentCustomer.FirstName + " " + Session.CurrentCustomer.LastName;
            txtWelcome.Text += Session.CurrentCustomer.FirstName;
            txtProfileInitials.Text = $"{Session.CurrentCustomer.FirstName[0]}{Session.CurrentCustomer.LastName[0]}";

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

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            Favorieten favorite = new Favorieten(_userId);
            favorite.Show();
            this.Close();
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
        private void LoadPreviousReservations()
        {
            ReservationService service = new ReservationService();
            var reservations = service.GetReservationsByCustomer(Session.CurrentCustomer.UserId);

            PreviousReservationsList.ItemsSource = reservations.Select(r => new ReservationViewModel
            {
                CarId = r.CarId,
                CarName = $"{r.CarBrand} {r.CarModel}",
                PricePerDayFormatted = $"€ {r.PricePerDay}",
                StartDate = r.StartDate,
                EndDate = r.EndDate
            }).ToList();
        }


        private void btnRentAgain_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var reservation = button.DataContext as ReservationViewModel;

            if (reservation != null)
            {
                var carDetail = new Windows.Clients.CarDetail(reservation.CarId, Session.CurrentUser.UserId);
                carDetail.Show();
                this.Close();         
            }
            else
            {
                MessageBox.Show("Reservation is null!");
            }
            
        }
        

        private void LoadActiveReservation()
        {
            ReservationService service = new ReservationService();
            var active = service.GetActiveReservationByCustomer(Session.CurrentCustomer.UserId);

            if (active != null)
            {
                _activeCarId = active.CarId;
                txtActiveCarName.Text = $"{active.CarBrand} {active.CarModel}";
                txtActiveDateRange.Text = $"{active.StartDate:dd/MM/yyyy} → {active.EndDate:dd/MM/yyyy}";
                txtActivePrice.Text = $"€ {active.PricePerDay}";
            }
            else
            {
                txtActiveCarName.Text = "Geen actieve reservatie";
                txtActiveDateRange.Text = "";
                txtActivePrice.Text = "";
            }
        }

        private void BekijkDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_activeCarId > 0)
            {
                var carDetail = new Windows.Clients.CarDetail(_activeCarId, Session.CurrentUser.UserId);
                carDetail.Show();
                this.Close();            
            }
        }

        private void SchadeMelden_Click(object sender, RoutedEventArgs e) 
        {
            if (activeReservationId > 0)
            {
                ReportDamage reportDamage = new ReportDamage(activeReservationId);
                reportDamage.Show();
            }
            else
            {
                MessageBox.Show("Geen actieve reservatie gevonden.");
            }
        }



    }
}