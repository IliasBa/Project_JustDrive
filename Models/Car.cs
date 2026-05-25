using Project_JustDrive.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace Project_JustDrive.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int CarNameId { get; set; }
        public string CarBrand { get; set; }
        public string Model { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Deposit { get; set; }
        public int CompanyId { get; set; }
        public decimal PricePer100km { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }
        public string Type { get; set; }
        public string LicensePlate { get; set; }
        public byte[] ImageData { get; set; }  // ← changed from string ImagePath

        public BitmapImage CarImage => ImageHelper.LoadFromBytes(ImageData);
    }
}
