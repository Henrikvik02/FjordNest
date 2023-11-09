using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FjordNestPro.Models
{
    public class Property
    {
        [Key]
        public int PropertyID { get; set; }


        [MaxLength(450)]  // Standard length for Identity user IDs in ASP.NET Core
        public string? OwnerID { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public required int AddressID { get; set; }
        public virtual Address? Address { get; set; }

        [Required] //Requiered
        [MaxLength(255)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public required string PropertyType { get; set; }

        [Required]
        [MaxLength(1000)]  // Adjust depending on the average length of descriptions
        public required string Description { get; set; }

        [Required]
        [Range(1, 20)]  // Assuming a property can hold 1 to 20 guests, adjust if needed
        public required int MaxGuests { get; set; }

        [Required]
        [Range(1, 10000)]  // Assuming a price range, adjust if needed
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal PricePerNight { get; set; }

        [Required]
        public required bool LongTermStay { get; set; }


        [MaxLength(1000)]  // Standard length for URLs, can adjust if needed
        public string? ImageUrl { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}


