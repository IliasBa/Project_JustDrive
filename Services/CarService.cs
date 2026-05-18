using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Services
{
    public class CarService
    {
        public CarCompareModel GetCarById(int carId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string query = @"SELECT c.Id, cn.Brand, cn.Model, c.TYPE, c.Fuel, 
                                c.Transmission, c.Price_Per_Day, c.Deposit, 
                                c.Price_Per_100km,c.Image_Path, u.City
                         FROM car c
                         JOIN carname cn ON cn.Id = c.CarNameId
                         JOIN user u ON u.User_Id = c.CompanyId
                         WHERE c.Id = @id";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", carId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new CarCompareModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CarBrand = reader["Brand"].ToString(),
                        Model = reader["Model"].ToString(),
                        Type = reader["TYPE"].ToString(),
                        Fuel = reader["Fuel"].ToString(),
                        Transmission = reader["Transmission"].ToString(),
                        PricePerDay = Convert.ToDecimal(reader["Price_Per_Day"]),
                        Deposit = Convert.ToDecimal(reader["Deposit"]),
                        PricePerKm = Convert.ToDecimal(reader["Price_Per_100km"]),
                        City = reader["City"].ToString(),
                        ImagePath = reader["Image_Path"] == DBNull.Value ? null : reader["Image_Path"].ToString()

                    };
                }
                return null;
            }
        }
    }
}
