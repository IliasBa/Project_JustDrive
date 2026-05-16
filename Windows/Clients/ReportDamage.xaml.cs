using Org.BouncyCastle.Bcpg;
using Project_JustDrive.Models;
using Project_JustDrive.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_JustDrive.Windows.Clients
{
    /// <summary>
    /// Interaction logic for ReportDamage.xaml
    /// </summary>
    public partial class ReportDamage : Window
    {
        private int _reservationId;

        public ReportDamage(int reservationId)
        {
            InitializeComponent();
            _reservationId = reservationId;
        }

        private void Melden_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("Vul een beschrijving in.");
                return;
            }

            if (cmbDamageLevel.SelectedItem == null)
            {
                MessageBox.Show("Kies een schadeniveau.");
                return;
            }

            string damageLevel = (cmbDamageLevel.SelectedItem as ComboBoxItem)?.Content.ToString();

            DamageReport report = new DamageReport
            {
                ReservationId = _reservationId,
                UserId = Session.CurrentUser.UserId,
                DamageLevel = damageLevel,    // ← add this
                Description = txtDescription.Text
            };

            DamageReportService service = new DamageReportService();
            bool success = service.AddDamageReport(report);

            if (success)
            {
                MessageBox.Show("Schaderapport succesvol ingediend!");
                this.Close();
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
