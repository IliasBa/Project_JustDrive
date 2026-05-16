using Project_JustDrive;
using Project_JustDrive.Services;
using System.Windows;
using System.Windows.Controls;
using static Project_JustDrive.Services.ReservationService;
using Project_JustDrive.Models;


namespace Project_JustDrive.Windows.Clients
{
    public partial class Dashboard : Window
    {
        private int _userId;
        //private int CarId;
        private int _activeCarId;
        private int activeReservationId;
        private int _activeReservationId;
        public Dashboard(int userId)
        {
            InitializeComponent();
            LoadActiveReservation();
            LoadFutureReservations();
            LoadPreviousReservations();
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

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            Favorieten favorite = new Favorieten(_userId);
            favorite.Show();
            this.Close();
        }
        private void Profiel_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile();
            profile.Show();
            this.Close();
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
        private void RentAgain_Click(object sender, RoutedEventArgs e)
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
                _activeReservationId = active.Id;
                txtActiveCarName.Text = $"{active.CarBrand} {active.CarModel}";
                txtActiveDateRange.Text = $"{active.StartDate:dd/MM/yyyy} - {active.EndDate:dd/MM/yyyy}";
                txtActivePrice.Text = $"€ {active.PricePerDay}";
                gridActiveReservation.Visibility = Visibility.Visible;
                stackNoActive.Visibility = Visibility.Collapsed; 
            }
            else
            {
                txtActiveCarName.Text = "";
                txtActiveDateRange.Text = "";
                txtActivePrice.Text = "";
                _activeCarId = 0;
                _activeReservationId = 0;
                gridActiveReservation.Visibility = Visibility.Collapsed;
                stackNoActive.Visibility = Visibility.Visible; 
            }
        }
        private void LoadFutureReservations()
        {
            ReservationService service = new ReservationService();
            var future = service.GetFutureReservationsByCustomer(Session.CurrentCustomer.UserId);

            if (future.Count > 0)
            {
                FutureReservationsList.ItemsSource = future.Select(r => new ReservationViewModel
                {
                    CarId = r.CarId,
                    ReservationId = r.Id,
                    CarName = $"{r.CarBrand} {r.CarModel}",
                    Price = $"€ {r.PricePerDay}",
                    StartDate = r.StartDate,
                    EndDate = r.EndDate
                }).ToList();

            }
            else
            {
                FutureReservationsList.Visibility = Visibility.Collapsed;
                FutureReservationsList.ItemsSource = null;
                stackNoFuture.Visibility = Visibility.Visible;
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
            if (_activeReservationId > 0)
            {
                ReportDamage reportDamage = new ReportDamage(_activeReservationId);
                reportDamage.Show();
            }
            else
            {
                MessageBox.Show("Geen actieve reservatie gevonden.");
            }
        }

        private void AnnuleerReservatie_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var reservation = button.DataContext as ReservationViewModel;

            if (reservation != null)
            {
                var result = MessageBox.Show("Ben je zeker dat je deze reservatie wil annuleren?",
                                             "Annuleren", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    ReservationService service = new ReservationService();
                    service.CancelReservation(reservation.ReservationId);

                    LoadActiveReservation();
                    LoadFutureReservations();
                    LoadPreviousReservations();
                }
            }
        }
    }
}