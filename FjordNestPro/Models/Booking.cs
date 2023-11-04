using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FjordNestPro.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [MaxLength(450)]  // Standard length for Identity user IDs in ASP.NET Core
        public string GuestID { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        public Review? Review { get; set; }
        public int PropertyID { get; set; }
        public Property? Property { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public required DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public required DateTime CheckOutDate { get; set; }

    }
}

