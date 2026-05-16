using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class CarCompareModel
    {
        public int Id { get; set; }
        public string CarBrand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Deposit { get; set; }
        public decimal PricePerKm { get; set; }
        public string City { get; set; }
    }
}
