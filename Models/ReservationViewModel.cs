using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public int CarId { get; set; }
        public string CarName { get; set; }
        public string Price { get; set; }
        public string PricePerDayFormatted { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateRange => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
    }
}
