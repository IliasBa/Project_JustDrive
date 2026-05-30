using System;
using System.Net;
using System.Net.Mail;

namespace Project_JustDrive.Services
{
    public static class EmailService
    {
        private const string SmtpHost = "smtp-auth.mailprotect.be";
        private const int SmtpPort = 587;
        private const string FromEmail = "no-reply@projectinspirationlab.com";
        private const string FromPassword = "azertyno-reply2026?";

        private static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(SmtpHost, SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(FromEmail, FromPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(FromEmail, "JustDrive"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    message.To.Add(toEmail);
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Show full error including inner exception
                string errorDetails = ex.Message;
                if (ex.InnerException != null)
                    errorDetails += $"\n\nInner: {ex.InnerException.Message}";

                System.Windows.MessageBox.Show($"Email fout:\n{errorDetails}");
            }
        }

        public static void SendReservationConfirmation(string toEmail, string firstName,
        string carName, DateTime startDate, DateTime endDate, decimal totalPrice,
        int reservationId, string companyName, string companyAdres, string companyPostcode,
        string companyCity, string companyPhone, string companyEmail, string companyIban)
            {
                string subject = $"Bevestiging reservatie #{reservationId} - JustDrive";

                
                string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #0F1B2D;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h1 style='color: #F26B1F;'>Just Drive<span style='color: #F26B1F;'>.</span></h1>
                    <h2>Beste {firstName},</h2>
                    <p>Uw reservatie is succesvol bevestigd!</p>

                    <div style='background: #F5F6F8; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <h3 style='margin-top: 0;'>Reservatiedetails</h3>
                        <p><strong>Reservatie #:</strong> {reservationId}</p>
                        <p><strong>Auto:</strong> {carName}</p>
                        <p><strong>Ophaaldatum:</strong> {startDate:dd/MM/yyyy} om 09:00</p>
                        <p><strong>Retourdatum:</strong> {endDate:dd/MM/yyyy}</p>
                        <p><strong>Aantal dagen:</strong> {(endDate - startDate).Days}</p>
                        <p><strong>Totaalprijs:</strong> €{totalPrice}</p>
                    </div>

                    <div style='background: #FFF3E0; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #F26B1F;'>
                        <h3 style='margin-top: 0; color: #F26B1F;'>Ophaaladres</h3>
                        <p><strong>{companyName}</strong></p>
                        <p>{companyAdres}</p>
                        <p>{companyPostcode} {companyCity}</p>
                        <p>📞 {companyPhone}</p>
                        <p>✉️ {companyEmail}</p>
                        <p style='margin-top: 12px;'>🕘 Gelieve de auto op te halen om <strong>09:00</strong></p>
                    </div>

                    <div style='background: #F5F6F8; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <h3 style='margin-top: 0;'>Betaalgegevens</h3>
                        <p>Gelieve het bedrag van <strong>€{totalPrice}</strong> over te schrijven naar:</p>
                        <p><strong>Begunstigde:</strong> {companyName}</p>
                        <p><strong>IBAN:</strong> {companyIban}</p>
                        <p><strong>Mededeling:</strong> Reservatie #{reservationId}</p>
                    </div>

                    <p>Bedankt voor uw vertrouwen in JustDrive!</p>
                    <p style='color: #6B7280; font-size: 12px;'>
                        Dit is een automatisch gegenereerde e-mail. 
                        Gelieve niet te antwoorden op dit bericht.
                    </p>
                </div>
            </body>
            </html>";

                SendEmail(toEmail, subject, body);
        }

        public static void SendCancellationConfirmation(string toEmail, string firstName,
            string carName, DateTime startDate, DateTime endDate, int reservationId)
        {
            string subject = $"Annulering reservatie #{reservationId} - JustDrive";
            string body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; color: #0F1B2D;'>
            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                <h1 style='color: #F26B1F;'>Just Drive<span style='color: #F26B1F;'>.</span></h1>
                <h2>Beste {firstName},</h2>
                <p>Uw reservatie is geannuleerd.</p>

                <div style='background: #F5F6F8; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                    <h3 style='margin-top: 0;'>Geannuleerde reservatie</h3>
                    <p><strong>Reservatie #:</strong> {reservationId}</p>
                    <p><strong>Auto:</strong> {carName}</p>
                    <p><strong>Ophaaldatum:</strong> {startDate:dd/MM/yyyy}</p>
                    <p><strong>Retourdatum:</strong> {endDate:dd/MM/yyyy}</p>
                </div>

                <p>We hopen u binnenkort opnieuw te mogen verwelkomen bij JustDrive.</p>
                <p style='color: #6B7280; font-size: 12px;'>
                    Dit is een automatisch gegenereerde e-mail. 
                    Gelieve niet te antwoorden op dit bericht.
                </p>
            </div>
        </body>
        </html>";

            SendEmail(toEmail, subject, body);
        }
    }
}