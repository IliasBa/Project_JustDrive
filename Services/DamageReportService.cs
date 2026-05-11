using JustDrive.Database;
using MySql.Data.MySqlClient;
using Project_JustDrive.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Project_JustDrive.Services
{
    public class DamageReportService
    {
        public bool AddDamageReport(DamageReport report)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO damagereport (ReservationId, UserId, Description) 
                                 VALUES (@reservationId, @userId, @description)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@reservationId", report.ReservationId);
                    cmd.Parameters.AddWithValue("@userId", report.UserId);
                    cmd.Parameters.AddWithValue("@description", report.Description);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message);
                return false;
            }
        }
    }
}
