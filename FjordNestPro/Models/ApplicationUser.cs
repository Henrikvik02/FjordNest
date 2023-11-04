using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FjordNestPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string? Password { get; set; }

        public ICollection<IdentityUserRole<string>>? Roles { get; set; }
        public ICollection<Property>? Properties { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }

    }
}

