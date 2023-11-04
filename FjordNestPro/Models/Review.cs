using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FjordNestPro.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [MaxLength(1000)]  // Adjust for typical review length
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public required int Rating { get; set; }

        [MaxLength(450)]  // Standard length for Identity user IDs in ASP.NET Core
        public string GuestID { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = default!;

        public int BookingID { get; set; }
        public Booking Booking { get; set; } = default!;

    }

}

