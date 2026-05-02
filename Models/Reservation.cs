using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
    }
}
