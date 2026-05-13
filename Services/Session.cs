using System;
using System.Collections.Generic;
using System.Text;
using Project_JustDrive.Models;

namespace Project_JustDrive.Services
{
    class Session
    {
        public static User CurrentUser { get; set; }
         public static Customer CurrentCustomer { get; set; }

<<<<<<< Updated upstream
        
=======
        public Reservation GetActiveReservation(int customerId)
        {
            Reservation reservation = null;

            // Use date check instead of Status column since you don't have one
            string query = @"SELECT r.Id, r.Start_date, r.End_date, r.CarId,
                            c.Car_Brand, c.Model, c.Price_per_day
                     FROM reservation r
                     JOIN car c ON c.Id = r.CarId
                     WHERE r.CustomerId = @customerId
                     AND CURDATE() BETWEEN r.Start_date AND r.End_date
                     LIMIT 1";

            using (MySqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@customerId", customerId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reservation = new Reservation
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CarId = Convert.ToInt32(reader["CarId"]),
                                CarBrand = reader["Car_Brand"].ToString(),
                                CarModel = reader["Model"].ToString(),
                                PricePerDay = Convert.ToDecimal(reader["Price_per_day"]),
                                StartDate = Convert.ToDateTime(reader["Start_date"]),
                                EndDate = Convert.ToDateTime(reader["End_date"])
                            };
                        }
                    }
                }
            }
            return reservation;
        }
>>>>>>> Stashed changes
    }
}
