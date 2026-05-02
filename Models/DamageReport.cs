using System;
using System.Collections.Generic;
using System.Text;

namespace Project_JustDrive.Models
{
    public class DamageReport
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }
}
