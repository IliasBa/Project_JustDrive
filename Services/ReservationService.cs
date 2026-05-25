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

                string query = @"SELECT r.Id, r.Start_date, r.End_date, r.CarId,
                                cn.Brand, cn.Model, c.Price_Per_Day, c.Image_Data
                         FROM reservation r
                         JOIN car c ON c.Id = r.CarId
                         JOIN carname cn ON cn.Id = c.CarNameId
                         WHERE r.CustomerId = @customerId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", userId);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new Reservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        StartDate = Convert.ToDateTime(reader["Start_date"]),
                        EndDate = Convert.ToDateTime(reader["End_date"]),
                        CarBrand = reader["Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        ImageData = reader["Image_Data"] == DBNull.Value ? null : (byte[])reader["Image_Data"]
                    });
                }
            }
            return reservations;
        }

        public Reservation GetActiveReservationByCustomer(int userId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"SELECT r.Id, r.Start_date, r.End_date, r.CarId,
                                cn.Brand, cn.Model, c.Price_Per_Day, c.Image_Data
                         FROM reservation r
                         JOIN car c ON c.Id = r.CarId
                         JOIN carname cn ON cn.Id = c.CarNameId
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
                        CarBrand = reader["Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        CarId = Convert.ToInt32(reader["CarId"]),
                        ImageData = reader["Image_Data"] == DBNull.Value ? null : (byte[])reader["Image_Data"]
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
                                cn.Brand, cn.Model, c.Price_Per_Day, c.Image_Data
                         FROM reservation r
                         JOIN car c ON c.Id = r.CarId
                         JOIN carname cn ON cn.Id = c.CarNameId
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
                        CarBrand = reader["Brand"].ToString(),
                        CarModel = reader["Model"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        StartDate = Convert.ToDateTime(reader["Start_date"]),
                        EndDate = Convert.ToDateTime(reader["End_date"]),
                        ImageData = reader["Image_Data"] == DBNull.Value ? null : (byte[])reader["Image_Data"]
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
