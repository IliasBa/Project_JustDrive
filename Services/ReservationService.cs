using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using Project_JustDrive.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Windows.Controls.Primitives;


namespace Project_JustDrive.Services
{
    public class ReservationService
    {
        public List<Reservation> GetReservationsByCustomer(int userId)
        {
            List<Reservation> reservations = new List<Reservation>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"
                SELECT r.Id,
                       r.Start_date,
                       r.End_date,
                       c.Car_Brand,
                       c.Model,
                       c.Price_per_day,
                        r.CarId
                FROM reservation r
                JOIN car c ON c.Id = r.CarId
                WHERE r.CustomerId = @customerId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                
                cmd.Parameters.AddWithValue("@customerId", Session.CurrentCustomer.UserId);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = new Reservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        StartDate = Convert.ToDateTime(reader["Start_Date"]),
                        EndDate = Convert.ToDateTime(reader["End_Date"]),
                        CarBrand = reader["Car_Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_per_day"]),
                        CarId = Convert.ToInt32(reader["CarId"])

                    };

                    reservations.Add(reservation);
                }
            }

            return reservations;
        }
        public Reservation GetActiveReservationByCustomer(int userId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"
                        SELECT r.Id, r.Start_date, r.End_date, r.CarId,
                               c.Car_Brand, c.Model, c.Price_per_day
                        FROM reservation r
                        JOIN car c ON c.Id = r.CarId
                        WHERE r.CustomerId = @customerId
                        AND CURDATE() BETWEEN r.Start_date AND r.End_date
                        LIMIT 1";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", userId);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Reservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        StartDate = Convert.ToDateTime(reader["Start_date"]),
                        EndDate = Convert.ToDateTime(reader["End_date"]),
                        CarBrand = reader["Car_Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_per_day"]),
                        CarId = Convert.ToInt32(reader["CarId"])

                    };
                }

                return null;
            }
        }

        public List<Reservation> GetFutureReservationsByCustomer(int userId)
        {
            List<Reservation> reservations = new List<Reservation>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"SELECT r.Id, r.Start_date, r.End_date, r.CarId,
                                c.Car_Brand, c.Model, c.Price_per_day
                         FROM reservation r
                         JOIN car c ON c.Id = r.CarId
                         WHERE r.CustomerId = @customerId
                         AND r.Start_date > CURDATE()
                         ORDER BY r.Start_date ASC";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", userId);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservations.Add(new Reservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        CarBrand = reader["Car_Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_per_day"]),
                        StartDate = Convert.ToDateTime(reader["Start_date"]),
                        EndDate = Convert.ToDateTime(reader["End_date"])
                    });
                }
            }

            return reservations;
        }

        public void CancelReservation(int reservationId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM reservation WHERE Id = @id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", reservationId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
